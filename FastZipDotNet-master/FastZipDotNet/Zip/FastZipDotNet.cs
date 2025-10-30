using FastZipDotNet.MultiThreading;
using FastZipDotNet.Zip.Helpers;
using FastZipDotNet.Zip.Readers;
using FastZipDotNet.Zip.Writers;
using FastZipDotNet.Zip.ZStd;
using System.IO.Compression;
using System.Text;
using static FastZipDotNet.Zip.Readers.ZipReadersInfo;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

namespace FastZipDotNet.Zip
{
    public class FastZipDotNet : IDisposable
    {


        public EncryptionAlgorithm Encryption { get; set; } = EncryptionAlgorithm.None;
        public string Password { get; set; } = null;

        public AdjustableSemaphore ConcurrencyLimiter { get; }
        // Internal: have we written anything new?
        private volatile bool _dirty = false;

        public Stream ZipFileStream;
        public List<ZipFileEntry> ZipFileEntries = new List<ZipFileEntry>();         // List of files data

        public ZipDataWriter ZipDataWriter;
        public ZipDataReader ZipDataReader;
        public ZipInfoStruct ZipInfoStruct;

        public long AppendOffset;

        public int Threads = 6;
        private ushort _zipFileCount = 0;

        private string _zipComment = "";

        public bool LimitRam = false;
        public bool Pause = false;
        public bool EncodeUTF8 = false;
       // public object lockObj = new object();
        public int CompressionLevelValue = 6;

        public Compression MethodCompress;
        public CompressionLevel CurrentCompression = CompressionLevel.Fastest;
        public CompressionOptions CompressionOptionsZstd = new CompressionOptions(6);

        public string ZipFileName;
        public long BytesWritten = 0;
        public long BytesPerSecond = 0;
        public long FilesWritten = 0;
        public long FilesPerSecond = 0;


        public long PartSize { get; set; } = 0; // Maximum size of each part in bytes
        public int CurrentPartNumber { get; set; } = 0; // Current part number
        public int CurrentDiskNumber { get; set; } = 0; // Start with disk number 0
                                                        /////////////////////////

        public Task<bool> TestArchiveAsync(IProgress<ZipProgress> progress, CancellationToken ct = default)
        {
            return ZipDataReader.TestArchiveAsync(Threads, progress, ct);
        }

        public Task<bool> UpdateFromDirectoryAsync(string directoryToSync, int compressionlevel, IProgress<ZipProgress> progress, CancellationToken ct = default)
        {
            return ZipDataWriter.UpdateArchiveFromDirectoryAsync(directoryToSync, Threads, compressionlevel, progress, ct);
        }
        public Task<bool> AddFilesToArchiveAsync(string directoryToAdd, int compressionlevel, IProgress<ZipProgress> progress, CancellationToken ct = default)
        {
            return ZipDataWriter.AddFilesToArchiveAsync(directoryToAdd, Threads, compressionlevel, progress, ct);
        }

        public Task<bool> RepairCentralDirectoryAsync(IProgress<ZipProgress> progress, CancellationToken ct = default)
        {
            return Zip.Recovery.ZipRepair.RepairCentralDirectoryAsync(ZipFileName, progress, ct);
        }

        public Task<bool> RepairToNewArchiveAsync(string repairedZipPath, int compressionLevel, IProgress<ZipProgress> progress, CancellationToken ct = default)
        {
            return Zip.Recovery.ZipRepair.RepairToNewArchiveAsync(ZipFileName, repairedZipPath, compressionLevel, Threads, progress, ct);
        }

        public Task<bool> ExtractArchiveAsync(string outputDirectory, IProgress<ZipProgress> progress, CancellationToken ct = default)
        {
            return ZipDataReader.ExtractArchiveAsync(outputDirectory, Threads, progress, ct);
        }

        internal long Reserve(long bytes)
        {
            _dirty = true; // mark archive as modified
            return Interlocked.Add(ref AppendOffset, bytes) - bytes;
        }


        public void SetMaxConcurrency(int max)
        {
            if (max < 0) throw new ArgumentOutOfRangeException(nameof(max));
            Threads = max;
            ConcurrencyLimiter.MaximumCount = max;
        }

        public void InitializeAppendOffsetForSinglePart()
        {
            if (ZipFileStream != null && ZipFileStream.Length > 0 && ZipInfoStruct.ZipFileEntries != null)
            {
                // CentralDirOffset64 is initialized from CentralDirOffset even for non-ZIP64 by your reader
                AppendOffset = (long)ZipInfoStruct.CentralDirOffset64;
            }
            else
            {
                AppendOffset = 0;
            }
        }

        // Close for single-part archives (writes the central directory at AppendOffset)
        public void CloseSinglePart()
        {
            if (ZipFileStream == null) return;

            if (!_dirty)
            {
                ZipFileStream.Dispose();
                ZipFileStream = null;
                return;
            }

            if (PartSize != 0)
                throw new InvalidOperationException("CloseSinglePart is for single-part archives only.");

            ZipFileStream.Seek(AppendOffset, SeekOrigin.Begin);

            ZipInfoStruct.TotalDisks = 1;
            ZipInfoStruct.ZipFileEntries = ZipFileEntries;
            ZipInfoStruct.ZipComment = _zipComment;

            Close(ZipInfoStruct, ZipFileStream);
            ZipFileStream = null;
        }

        private int DetermineCurrentPartNumber(string baseFileName)
        {
            int partNumber = 0;
            string baseName = Path.GetFileNameWithoutExtension(baseFileName);
            string directory = Path.GetDirectoryName(baseFileName);

            // Check existence of the single .zip file first
            string currentFilePath = Path.Combine(directory, baseName + ".zip");

            if (File.Exists(currentFilePath))
            {
                // If only a single .zip file exists, return 0
                while (File.Exists(Path.Combine(directory, baseName + $".z{partNumber:D2}")))
                {
                    partNumber++;
                }

                // If no part files found, it's a single file archive
                if (partNumber == 0)
                {
                    return 0;
                }
            }
            else
            {
                // If the base .zip file doesn't exist, check for .zXX parts
                while (File.Exists(Path.Combine(directory, baseName + $".z{partNumber:D2}")))
                {
                    partNumber++;
                }

                // If no .zip or .zXX part found, return 0
                if (partNumber == 0)
                {
                    return 0;
                }
            }

            // Adjust for the last known part from multi-parts
            return partNumber > 0 ? partNumber - 1 : 0;
        }


        int lastPartNumber = 0;
        public string GetPartFileName(int partNumber)
        {
            string baseName = Path.GetFileNameWithoutExtension(ZipFileName);
            string directory = Path.GetDirectoryName(ZipFileName);
            string currentZipPath = Path.Combine(directory, baseName + ".zip");
            string extension = ".zip";

            //ISSUE HERE FOR MULTI PART TO SOLVE, IF COMMENTED OUT WORKS FOR SINGLE
            if (partNumber == 0)
            {
                // If it's the first part, it will naturally be the .zip file
                extension = ".zip";
                lastPartNumber = 0;
            }
            else
            {
                // When partNumber is not 0, rename the existing .zip to .zXX
                if (File.Exists(currentZipPath))
                {
                    string previousPartExtension = $".z{(partNumber):D2}";
                    string previousPartPath = Path.Combine(directory, baseName + previousPartExtension);

                    if (lastPartNumber < partNumber)
                    {
                        lastPartNumber = partNumber;
                        File.Move(currentZipPath, previousPartPath);
                    }

                }

                // Current part becomes .zip
                extension = ".zip";
            }

            return Path.Combine(directory, baseName + extension);
        }


        public int GetTotalDisks()
        {
            // Since disk numbers start from 0, total disks = CurrentDiskNumber + 1
            return CurrentDiskNumber + 1;
        }


        public FastZipDotNet(string pathFilename, Compression method = Compression.Deflate, int compressionLevel = 6, int threads = -1, bool limitRam = false, string zipComment = "", long partSize = 0)
        {
            EncodingHelper.Register437Encoding();
            ZipFileName = pathFilename;
            MethodCompress = method;
            CompressionLevelValue = compressionLevel;
            _zipComment = zipComment;
            LimitRam = limitRam;

            if (threads == -1)
            {
                threads = Environment.ProcessorCount;
                Threads = threads;
            }
            else
            {
                Threads = threads;
            }


            int dop = (threads == -1) ? Environment.ProcessorCount : Math.Max(1, threads);
            Threads = dop;

            ConcurrencyLimiter = new AdjustableSemaphore(dop);

            ZipDataWriter = new ZipDataWriter(this);
            ZipDataReader = new ZipDataReader(this);

            this.PartSize = partSize;
            this.CurrentDiskNumber = 0; // Start with disk number 0
            // Initialize part number if resuming an existing archive
            if (File.Exists(pathFilename))
            {
                // Determine the current part number
                this.CurrentPartNumber = DetermineCurrentPartNumber(pathFilename);
            }
            else
            {
                this.CurrentPartNumber = 0;
            }

            // Open the current part file
            string partFileName = GetPartFileName(CurrentPartNumber);
            ZipFileStream = new FileStream(partFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 8388608);

            // If existing archive:
            if (ZipFileStream.Length > 0)
            {
                ZipInfoStruct zipInfo = new ZipInfoStruct();
                if (!ZipReadersInfo.ReadZipInfo(ZipFileStream, ref zipInfo))
                    throw new InvalidDataException("Invalid ZIP file.");

                ZipInfoStruct = zipInfo;
                ZipFileEntries = ZipInfoStruct.ZipFileEntries;

                // Initialize append pointer for single-part mode
                InitializeAppendOffsetForSinglePart();
            }
            else
            {
                ZipInfoStruct = new ZipReadersInfo.ZipInfoStruct();
                ZipInfoStruct.ZipComment = _zipComment;
                AppendOffset = 0;
            }

            SetCompressionLevel(CompressionLevelValue);
        }

        public FastZipDotNet(Stream stream, Compression method = Compression.Deflate, int compressionLevel = 6, int threads = -1, bool limitRam = false, string zipComment = "", long partSize = 0)
        {
            EncodingHelper.Register437Encoding();
            MethodCompress = method;
            CompressionLevelValue = compressionLevel;
            _zipComment = zipComment;
            LimitRam = limitRam;

            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream cannot seek");


            if (threads == -1)
            {
                threads = Environment.ProcessorCount;
                Threads = threads;
            }
            else
            {
                Threads = threads;
            }

            int dop = (threads == -1) ? Environment.ProcessorCount : Math.Max(1, threads);
            Threads = dop;

            ConcurrencyLimiter = new AdjustableSemaphore(dop);

            ZipDataWriter = new ZipDataWriter(this);
            ZipDataReader = new ZipDataReader(this);

            this.PartSize = partSize;

            ZipFileStream = stream;
            ZipInfoStruct zipInfo = new ZipInfoStruct();

            if (!ReadZipInfo(ZipFileStream, ref zipInfo))
            {
                ZipInfoStruct = new ZipInfoStruct();
                throw new InvalidDataException("Invalid ZIP file.");
            }
            ZipInfoStruct = zipInfo;
            ZipFileEntries = ZipInfoStruct.ZipFileEntries;

            SetCompressionLevel(CompressionLevelValue);
        }

        public void SetCompressionLevel(int compressionLevel)
        {

            CompressionLevelValue = compressionLevel;
            if (CompressionLevelValue == 0)
            {
                CurrentCompression = CompressionLevel.NoCompression;
            }
            else if (CompressionLevelValue > 0 && CompressionLevelValue < 7)
            {
                CurrentCompression = CompressionLevel.Fastest;
            }
            else if (CompressionLevelValue >= 7 && CompressionLevelValue < 11)
            {
                CurrentCompression = CompressionLevel.Optimal;
            }
            else
            {
                CurrentCompression = CompressionLevel.SmallestSize;
            }
            CompressionOptionsZstd = new CompressionOptions(CompressionLevelValue);
        }

      

        #region Test Archive
        /// <summary>
        /// Test zip integrity
        /// </summary>
        /// <returns>True if zip file is ok</returns>
        public bool TestArchive()
        {
            return ZipDataReader.TestArchiveAsync(Threads).Result;
        }

        public async Task<bool> TestArchiveAsync()
        {
            return await ZipDataReader.TestArchiveAsync(Threads);
        }
        #endregion


        #region Extraction
        public async Task<bool> ExtractArchiveAsync(string outputDirectory)
        {
            return await ZipDataReader.ExtractArchiveAsync(outputDirectory, Threads);
        }

        public bool ExtractArchive(string outputDirectory)
        {
            return ZipDataReader.ExtractArchiveAsync(outputDirectory, Threads).Result;
        }
        #endregion


        #region Adding Files


        #endregion


        #region Close And Dispose

        public void Close()
        {
            if (ZipFileStream == null) return;

            if (!_dirty)
            {
                ZipFileStream.Dispose();
                ZipFileStream = null;
                return;
            }

            //// existing single-part write logic
            //if (PartSize != 0)
            //{
            //    // multi-part write path (if you keep it)
            //}

            ZipFileStream.Seek(AppendOffset, SeekOrigin.Begin);

            ZipInfoStruct.TotalDisks = 1;
            ZipInfoStruct.ZipFileEntries = ZipFileEntries;
            ZipInfoStruct.ZipComment = _zipComment;

            Close(ZipInfoStruct, ZipFileStream);

            ZipFileStream.Dispose();
            ZipFileStream = null;
        }


        /// <summary>
        /// Close the zip file and write the central directory.
        /// </summary>
        public void Close(ZipInfoStruct zipInfo, Stream zipFileStream, string zipComment = "")
        {
            try
            {
                ulong centralOffset = (ulong)zipFileStream.Position;
                ulong centralSize = 0;

                // Write central directory records for all entries
                foreach (var file in zipInfo.ZipFileEntries)
                {
                    long pos = zipFileStream.Position;
                    ZipWritersHeaders.WriteCentralDirRecord(file, zipFileStream);
                    centralSize += (ulong)(zipFileStream.Position - pos);
                }

                bool zip64Needed = centralOffset >= uint.MaxValue || centralSize >= uint.MaxValue ||
                                   zipInfo.ZipFileEntries.Count >= ushort.MaxValue || zipInfo.TotalDisks > ushort.MaxValue;

                // Record the offset of the Zip64 End of Central Directory Record
                ulong zip64EndOfCentralDirOffset = (ulong)zipFileStream.Position;

                if (zip64Needed)
                {
                    // Write Zip64 End of Central Directory Record
                    zipFileStream.Write(new byte[] { 0x50, 0x4b, 0x06, 0x06 }, 0, 4); // PK\006\006
                    zipFileStream.Write(BitConverter.GetBytes((ulong)44), 0, 8); // Size of Zip64 EOCD record minus 12 bytes
                    zipFileStream.Write(BitConverter.GetBytes((ushort)45), 0, 2); // Version made by
                    zipFileStream.Write(BitConverter.GetBytes((ushort)45), 0, 2); // Version needed to extract
                    zipFileStream.Write(BitConverter.GetBytes((uint)CurrentDiskNumber), 0, 4); // Number of this disk
                    zipFileStream.Write(BitConverter.GetBytes((uint)CurrentDiskNumber), 0, 4); // Number of the disk with the start of central directory
                    zipFileStream.Write(BitConverter.GetBytes((ulong)zipInfo.ZipFileEntries.Count), 0, 8); // Total entries on this disk
                    zipFileStream.Write(BitConverter.GetBytes((ulong)zipInfo.ZipFileEntries.Count), 0, 8); // Total entries
                    zipFileStream.Write(BitConverter.GetBytes(centralSize), 0, 8); // Size of the central directory
                    zipFileStream.Write(BitConverter.GetBytes(centralOffset), 0, 8); // Offset of central directory

                    // Write Zip64 End of Central Directory Locator
                    zipFileStream.Write(new byte[] { 0x50, 0x4b, 0x06, 0x07 }, 0, 4); // PK\006\007
                    zipFileStream.Write(BitConverter.GetBytes((uint)CurrentDiskNumber), 0, 4); // Number of the disk with the Zip64 EOCD
                    zipFileStream.Write(BitConverter.GetBytes(zip64EndOfCentralDirOffset), 0, 8); // Offset of Zip64 EOCD
                    zipFileStream.Write(BitConverter.GetBytes((uint)zipInfo.TotalDisks), 0, 4); // Total number of disks
                }

                // Write End of Central Directory Record
                zipFileStream.Write(new byte[] { 0x50, 0x4b, 0x05, 0x06 }, 0, 4); // PK\005\006
                zipFileStream.Write(BitConverter.GetBytes((ushort)(ZipInfoStruct.TotalDisks - 1)), 0, 2); // Number of this disk
                zipFileStream.Write(BitConverter.GetBytes((ushort)(ZipInfoStruct.TotalDisks - 1)), 0, 2); // Disk with central directory

                if (zip64Needed)
                {
                    // Indicate Zip64 usage
                    zipFileStream.Write(BitConverter.GetBytes((ushort)0xFFFF), 0, 2); // Total entries on this disk
                    zipFileStream.Write(BitConverter.GetBytes((ushort)0xFFFF), 0, 2); // Total entries
                    zipFileStream.Write(BitConverter.GetBytes((uint)0xFFFFFFFF), 0, 4); // Size of the central directory
                    zipFileStream.Write(BitConverter.GetBytes((uint)0xFFFFFFFF), 0, 4); // Offset of central directory
                }
                else
                {
                    zipFileStream.Write(BitConverter.GetBytes((ushort)zipInfo.ZipFileEntries.Count), 0, 2); // Total entries on this disk
                    zipFileStream.Write(BitConverter.GetBytes((ushort)zipInfo.ZipFileEntries.Count), 0, 2); // Total entries
                    zipFileStream.Write(BitConverter.GetBytes((uint)centralSize), 0, 4); // Size of the central directory
                    zipFileStream.Write(BitConverter.GetBytes((uint)centralOffset), 0, 4); // Offset of central directory
                }

                byte[] encodedComment = Encoding.UTF8.GetBytes(zipInfo.ZipComment);
                zipFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2); // Comment length
                zipFileStream.Write(encodedComment, 0, encodedComment.Length); // Comment

                // Flush and close the stream
                if (zipFileStream != null)
                {
                    zipFileStream.Flush();
                    zipFileStream.Dispose();
                    zipFileStream = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn ZipWritersHeaders.Close");
            }
        }
        

        #region IDisposable Members
        bool isDisposed = false;
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                Close();
                if (ZipFileStream != null)
                {
                    ZipFileStream.Close();
                    ZipFileStream.Dispose();
                    ZipFileStream = null;
                }
                GC.Collect();
            }
        }
        #endregion

        #endregion
    }
}
