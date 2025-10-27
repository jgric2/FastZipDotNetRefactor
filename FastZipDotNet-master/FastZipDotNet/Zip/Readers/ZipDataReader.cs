using FastZipDotNet.MultiThreading;
using FastZipDotNet.Zip.Cryptography;
using FastZipDotNet.Zip.Helpers;
using FastZipDotNet.Zip.ZStd;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

namespace FastZipDotNet.Zip.Readers
{
    public class ZipDataReader
    {
        public ZipDataReader(FastZipDotNet fastZipDotNet)
        {
            FastZipDotNet = fastZipDotNet;
        }

        FastZipDotNet FastZipDotNet;


        #region TestArchive


        public async Task<bool> TestArchiveAsync(int threads, IProgress<ZipProgress> progress, CancellationToken ct = default)
        {
            try
            {
                await Task.Yield();

                int totalFiles = FastZipDotNet.ZipFileEntries.Count;
                long totalUncompressed = 0;
                foreach (var e in FastZipDotNet.ZipFileEntries)
                    totalUncompressed += (long)e.FileSize;

                int maxDop = Math.Max(1, threads <= 0 ? Environment.ProcessorCount - 1 : threads);
                var sem = new SemaphoreSlim(maxDop, maxDop);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                var tasks = new List<Task>(totalFiles);

                long processedUncompressed = 0;
                int filesDone = 0;
                string lastFile = null;

                var sw = Stopwatch.StartNew();

                // Dedicated reporter
                using var reportCts = CancellationTokenSource.CreateLinkedTokenSource(linkedCts.Token);
                var reportingTask = Task.Factory.StartNew(async () =>
                {
                    while (!reportCts.IsCancellationRequested)
                    {
                        var elapsed = sw.Elapsed;
                        long unc = Math.Min(Interlocked.Read(ref processedUncompressed), totalUncompressed);
                        int f = Volatile.Read(ref filesDone);
                        double speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;
                        double fps = elapsed.TotalSeconds > 0 ? f / elapsed.TotalSeconds : 0.0;

                        progress?.Report(new ZipProgress
                        {
                            Operation = ZipOperation.Test,
                            CurrentFile = Volatile.Read(ref lastFile),
                            TotalFiles = totalFiles,
                            TotalBytesUncompressed = totalUncompressed,
                            FilesProcessed = f,
                            BytesProcessedUncompressed = unc,
                            Elapsed = elapsed,
                            SpeedBytesPerSec = speed,
                            FilesPerSec = fps
                        });

                        try { await Task.Delay(200, reportCts.Token).ConfigureAwait(false); }
                        catch (OperationCanceledException) { break; }
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

                foreach (var entry in FastZipDotNet.ZipFileEntries)
                {
                    await sem.WaitAsync(linkedCts.Token).ConfigureAwait(false);

                    var task = Task.Run(() =>
                    {
                        try
                        {
                            void OnBytes(long n)
                            {
                                Interlocked.Add(ref processedUncompressed, n);
                                Volatile.Write(ref lastFile, entry.FilenameInZip);
                            }

                            // Test by fully reading/decompressing, but writing to Null
                            using var sink = Stream.Null;
                            bool ok = ExtractFile(entry, sink, OnBytes);
                            if (!ok)
                            {
                                linkedCts.Cancel();
                                throw new InvalidDataException($"CRC mismatch or read error: {entry.FilenameInZip}");
                            }

                            Interlocked.Increment(ref filesDone);
                        }
                        finally
                        {
                            sem.Release();
                        }
                    }, linkedCts.Token);

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);

                // Final 100% update
                {
                    var elapsed = sw.Elapsed;
                    long unc = totalUncompressed;
                    double speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;
                    double fps = elapsed.TotalSeconds > 0 ? totalFiles / elapsed.TotalSeconds : 0.0;

                    progress?.Report(new ZipProgress
                    {
                        Operation = ZipOperation.Test,
                        CurrentFile = null,
                        TotalFiles = totalFiles,
                        TotalBytesUncompressed = totalUncompressed,
                        FilesProcessed = totalFiles,
                        BytesProcessedUncompressed = unc,
                        Elapsed = elapsed,
                        SpeedBytesPerSec = speed,
                        FilesPerSec = fps
                    });
                }

                reportCts.Cancel();
                try { await reportingTask.ConfigureAwait(false); } catch { }

                return true;
            }
            catch (OperationCanceledException) { return false; }
            catch { return false; }
        }


        public async Task<bool> TestArchiveAsync(int threads = 6)
        {
            try
            {
                var cts = new CancellationTokenSource();
                var semaphore = new AdjustableSemaphore(threads);
                var tasks = new List<Task>();
                bool success = true;
                foreach (var currentEntry in FastZipDotNet.ZipFileEntries)
                {
                    // Wait for semaphore with cancellation token support
                    await Task.Run(() => semaphore.WaitOne(cts.Token));

                    var task = Task.Run(() =>
                    {
                        try
                        {
                            if (!TestFile(currentEntry))
                            {
                                cts.Cancel();
                                return false;
                            }
                            return true;
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }, cts.Token);
                    tasks.Add(task);
                }

                // Wait for all the tasks to complete
                var results = Task.WhenAll(tasks);

                // Check if any of the results are false
                return results.IsCompletedSuccessfully;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn BrutalUnZip.Test");
            }
        }

        /// <summary>
        /// Test the contents of a stored file
        /// </summary>
        /// <param name="zfe">Entry information of file to test</param>
        /// <returns>True if success, false if not.</returns>
        public bool TestFile(ZipFileEntry zfe)
        {
            try
            {
                if (!ExtractFile(zfe, Stream.Null))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn BrutalUnZip.TestFile");
            }
        }
        #endregion

        #region Extraction
        public async Task<bool> ExtractArchiveAsync(string outputDirectory, int threads = 6, IProgress<ZipProgress> progress = null, CancellationToken ct = default)
        {
            try
            {
                if (!Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);

                // Totals from central directory
                int totalFiles = FastZipDotNet.ZipFileEntries.Count;
                long totalUncompressed = 0;
                long totalCompressed = 0;
                foreach (var e in FastZipDotNet.ZipFileEntries)
                {
                    totalUncompressed += (long)e.FileSize;
                    totalCompressed += (long)e.CompressedSize;
                }

                // Shared counters
                long processedUncompressed = 0;             // aggregate bytes extracted (uncompressed)
                long processedCompressedApprox = 0;         // approximate compressed bytes consumed
                int filesDone = 0;

                // Name of last file that reported progress
                string lastFileName = null;

                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                var semaphore = new AdjustableSemaphore(threads);
                var tasks = new List<Task>(totalFiles);
                var sw = Stopwatch.StartNew();

                // Periodic reporter
                CancellationTokenSource reportCts = null;
                Task reportingTask = Task.CompletedTask;

                if (progress != null)
                {
                    reportCts = CancellationTokenSource.CreateLinkedTokenSource(linkedCts.Token);
                    reportingTask = Task.Run(async () =>
                    {
                        while (!reportCts.IsCancellationRequested)
                        {
                            var elapsed = sw.Elapsed;
                            var bytes = Interlocked.Read(ref processedUncompressed);
                            var compBytes = Interlocked.Read(ref processedCompressedApprox);
                            var files = Volatile.Read(ref filesDone);

                            var speed = elapsed.TotalSeconds > 0 ? bytes / elapsed.TotalSeconds : 0.0;
                            var fps = elapsed.TotalSeconds > 0 ? files / elapsed.TotalSeconds : 0.0;

                            progress.Report(new ZipProgress
                            {
                                Operation = ZipOperation.Extract,
                                CurrentFile = Volatile.Read(ref lastFileName),

                                TotalFiles = totalFiles,
                                TotalBytesUncompressed = totalUncompressed,
                                TotalBytesCompressed = totalCompressed,

                                FilesProcessed = files,
                                BytesProcessedUncompressed = bytes,
                                BytesProcessedCompressed = compBytes,

                                Elapsed = elapsed,
                                SpeedBytesPerSec = speed,
                                FilesPerSec = fps
                            });

                            try { await Task.Delay(200, reportCts.Token).ConfigureAwait(false); }
                            catch (OperationCanceledException) { break; }
                        }
                    }, reportCts.Token);
                }

                foreach (var currentEntry in FastZipDotNet.ZipFileEntries)
                {
                    semaphore.WaitOne(linkedCts.Token);
                    var entry = currentEntry; // capture

                    var task = Task.Run(() =>
                    {
                        try
                        {
                            string outPath = Path.Combine(outputDirectory, entry.FilenameInZip);

                            // Ratio to approximate compressed bytes progressed
                            double compPerUnc = (entry.FileSize > 0) ? (double)entry.CompressedSize / (double)entry.FileSize : 0.0;

                            // Per-chunk callback (uncompressed bytes)
                            void OnBytes(long count)
                            {
                                Interlocked.Add(ref processedUncompressed, count);
                                if (compPerUnc > 0)
                                {
                                    long approxComp = (long)Math.Round(count * compPerUnc);
                                    Interlocked.Add(ref processedCompressedApprox, approxComp);
                                }
                                Volatile.Write(ref lastFileName, entry.FilenameInZip);
                            }

                            bool ok = ExtractFile(entry, outPath, OnBytes);
                            if (!ok)
                            {
                                linkedCts.Cancel();
                                throw new Exception($"Extraction failed for: {entry.FilenameInZip}");
                            }

                            Interlocked.Increment(ref filesDone);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }, linkedCts.Token);

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);

                // Final report @ 100%
                if (progress != null)
                {
                    var elapsed = sw.Elapsed;
                    var speed = elapsed.TotalSeconds > 0 ? totalUncompressed / elapsed.TotalSeconds : 0.0;
                    var fps = elapsed.TotalSeconds > 0 ? totalFiles / elapsed.TotalSeconds : 0.0;

                    progress.Report(new ZipProgress
                    {
                        Operation = ZipOperation.Extract,
                        CurrentFile = null,

                        TotalFiles = totalFiles,
                        TotalBytesUncompressed = totalUncompressed,
                        TotalBytesCompressed = totalCompressed,

                        FilesProcessed = totalFiles,
                        BytesProcessedUncompressed = totalUncompressed,
                        BytesProcessedCompressed = totalCompressed, // now exact

                        Elapsed = elapsed,
                        SpeedBytesPerSec = speed,
                        FilesPerSec = fps
                    });
                }

                reportCts?.Cancel();
                try { await reportingTask.ConfigureAwait(false); } catch { /* ignore */ }

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }


        public bool ExtractFile(ZipFileEntry zfe, string outPathFilename, Action<long> onBytes = null)
        {
            Stream output = null;

            try
            {
                // Ensure the parent directory exists
                string path = Path.GetDirectoryName(outPathFilename);
                if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // Check if it's a directory (entry might represent a folder)
                if (zfe.FilenameInZip.EndsWith("/") || zfe.FilenameInZip.EndsWith("\\"))
                    return true;

                // Delete file if it already exists
                if (File.Exists(outPathFilename))
                {
                    try { File.Delete(outPathFilename); }
                    catch { throw new InvalidOperationException($"File '{outPathFilename}' cannot be written"); }
                }

                while (FastZipDotNet.Pause)
                    Thread.Sleep(50);

                output = new FileStream(outPathFilename, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20, FileOptions.SequentialScan);

                // Use the stream-based overload that reports per-chunk progress
                if (!ExtractFile(zfe, output, onBytes))
                    return false;

                // Close stream before touching times
                output.Close();

                // Set file date/time
                File.SetCreationTime(outPathFilename, zfe.ModifyTime);
                File.SetLastWriteTime(outPathFilename, zfe.ModifyTime);

                return true;
            }
            finally
            {
                output?.Dispose();
            }
        }

        public bool ExtractFile(ZipFileEntry zfe, Stream outStream, Action<long> onBytes = null)
        {
            Stream zipStream = null;

            try
            {
                // Open a fresh read handle (no shared position)
                zipStream = new FileStream(FastZipDotNet.ZipFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1 << 20, FileOptions.SequentialScan);

                if (!outStream.CanWrite)
                    throw new InvalidOperationException("Stream cannot be written");

                // Seek to local header
                zipStream.Seek((long)zfe.HeaderOffset, SeekOrigin.Begin);
                using (var br = new BinaryReader(zipStream, Encoding.Default, leaveOpen: true))
                {
                    uint signature = br.ReadUInt32();
                    if (signature != 0x04034b50)
                        return false;

                    ushort versionNeeded = br.ReadUInt16();
                    ushort generalPurposeBitFlag = br.ReadUInt16();
                    ushort compressionMethod = br.ReadUInt16();
                    ushort lastModTime = br.ReadUInt16();
                    ushort lastModDate = br.ReadUInt16();
                    uint crc32 = br.ReadUInt32();
                    uint compressedSize = br.ReadUInt32();
                    uint uncompressedSize = br.ReadUInt32();
                    ushort fileNameLength = br.ReadUInt16();
                    ushort extraFieldLength = br.ReadUInt16();

                    br.ReadBytes(fileNameLength); // Skip filename

                    // Handle Zip64 sizes if needed
                    ulong uncompressedSize64 = uncompressedSize;
                    ulong compressedSize64 = compressedSize;

                    if (compressedSize == 0xFFFFFFFF || uncompressedSize == 0xFFFFFFFF)
                    {
                        byte[] extraField = br.ReadBytes(extraFieldLength);
                        using var extraFieldStream = new MemoryStream(extraField);
                        using var extraFieldReader = new BinaryReader(extraFieldStream);
                        while (extraFieldStream.Position < extraFieldLength)
                        {
                            ushort headerId = extraFieldReader.ReadUInt16();
                            ushort dataSize = extraFieldReader.ReadUInt16();
                            if (headerId == 0x0001)
                            {
                                if (uncompressedSize == 0xFFFFFFFF)
                                    uncompressedSize64 = extraFieldReader.ReadUInt64();
                                if (compressedSize == 0xFFFFFFFF)
                                    compressedSize64 = extraFieldReader.ReadUInt64();
                                long remain = dataSize - (long)(extraFieldStream.Position - 4);
                                if (remain > 0) extraFieldReader.BaseStream.Seek(remain, SeekOrigin.Current);
                            }
                            else
                            {
                                extraFieldReader.ReadBytes(dataSize);
                            }
                        }
                    }
                    else
                    {
                        br.ReadBytes(extraFieldLength);
                    }

                    // Set up the input stream for file data
                    Stream inputStream;
                    if (compressionMethod == (ushort)Compression.Store)
                        inputStream = zipStream;
                    else if (compressionMethod == (ushort)Compression.Deflate)
                        inputStream = new DeflateStream(zipStream, CompressionMode.Decompress, true);
                    else if (compressionMethod == (ushort)Compression.Zstd)
                        inputStream = new DecompressionStream(zipStream);
                    else
                        return false;

                    uint crc32Calc = 0;
                    byte[] buffer = new byte[32768];
                    ulong bytesPending = uncompressedSize64;

                    while (bytesPending > 0)
                    {
                        while (FastZipDotNet.Pause)
                            Thread.Sleep(50);

                        int bytesRead = inputStream.Read(buffer, 0, (int)Math.Min(bytesPending, (ulong)buffer.Length));
                        if (bytesRead <= 0)
                            break;

                        crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
                        bytesPending -= (ulong)bytesRead;

                        outStream.Write(buffer, 0, bytesRead);

                        // Report per-chunk progress to the aggregator
                        onBytes?.Invoke(bytesRead);

                        Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
                        Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);
                    }

                    Interlocked.Increment(ref FastZipDotNet.FilesPerSecond);
                    Interlocked.Increment(ref FastZipDotNet.FilesWritten);
                    outStream.Flush();

                    if (compressionMethod == (ushort)Compression.Deflate || compressionMethod == (ushort)Compression.Zstd)
                        inputStream.Dispose();

                    return zfe.Crc32 == crc32Calc;
                }
            }
            finally
            {
                zipStream?.Dispose();
            }
        }

        /// <summary>
        /// Copy the contents of a stored file into a physical file
        /// </summary>
        /// <param name="zfe">Entry information of file to extract</param>
        /// <param name="outPathFilename">Name of file to store uncompressed data</param>
        /// <returns>True if success, false if not.</returns>
        public bool ExtractFile(ZipFileEntry zfe, string outPathFilename)
        {
            Stream output = null;

            try
            {
                // Ensure the parent directory exists
                string path = Path.GetDirectoryName(outPathFilename);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Check if it's a directory. If so, do nothing
                if (Directory.Exists(outPathFilename))
                {
                    return true;
                }

                // Delete file if it already exists
                if (File.Exists(outPathFilename))
                {
                    try
                    {
                        File.Delete(outPathFilename);
                    }
                    catch
                    {
                        throw new InvalidOperationException("File '" + outPathFilename + "' cannot be written");
                    }
                }

                while (FastZipDotNet.Pause)
                {
                    Thread.Sleep(50);
                    //if (Cancel)
                    //    Dispose();
                }

                //if (Cancel)
                //    Dispose();

                while (IOHelpers.IsFileLocked(outPathFilename))
                {
                    Thread.Sleep(5);
                    //Application.DoEvents();
                }
                try
                {
                    output = new FileStream(outPathFilename, FileMode.Create, FileAccess.Write);
                }
                catch
                {
                    for (int i = 0; i < 32; i++)
                    {
                        while (IOHelpers.IsFileLocked(outPathFilename))
                        {
                            Thread.Sleep(5);
                           // Application.DoEvents();
                        }
                        try
                        {

                            Thread.Sleep(100);
                            output = new FileStream(outPathFilename, FileMode.Create, FileAccess.Write);
                            break;
                        }
                        catch
                        {

                        }
                    }

                }

                // lock (lockObj)
                if (!ExtractFile(zfe, output))
                {
                    return false;
                }

                // Change file datetimes
                output.Close();
                while (IOHelpers.IsFileLocked(outPathFilename))
                {
                    Thread.Sleep(5);
                }
                File.SetCreationTime(outPathFilename, zfe.ModifyTime);
                File.SetLastWriteTime(outPathFilename, zfe.ModifyTime);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn BrutalUnZip.ExtractFile");
            }
            finally
            {
                if (output != null)
                {
                    output.Close();
                    output.Dispose();
                }
            }
        }

        private long GetCumulativeDiskSize(int diskNumber)
        {
            long totalSize = 0;
            for (int i = 0; i < diskNumber; i++)
            {
                string partFileName = FastZipDotNet.GetPartFileName(i);
                using (var fs = new FileStream(partFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    totalSize += fs.Length;
                }
            }
            return totalSize;
        }

        /// <summary>
        /// Copy the contents of a stored file into an opened stream
        /// </summary>
        /// <param name="zfe">Entry information of file to extract</param>
        /// <param name="outStream">Stream to store the uncompressed data</param>
        /// <returns>True if success, false if not.</returns>
        /// 
        /// aaaaaa
        /// 
        ////////////////////////public bool ExtractFile(ZipFileEntry zfe, Stream outStream)
        ////////////////////////{
        ////////////////////////    Stream inputStream = null;

        ////////////////////////    try
        ////////////////////////    {
        ////////////////////////        if (!outStream.CanWrite)
        ////////////////////////        {
        ////////////////////////            throw new InvalidOperationException("Stream cannot be written");
        ////////////////////////        }

        ////////////////////////        // Calculate the cumulative size of all preceding disks
        ////////////////////////        long cumulativeDiskSize = 0;//GetCumulativeDiskSize(zfe.DiskNumberStart);

        ////////////////////////        // Open the disk stream where the local header is located
        ////////////////////////        string partFileName = FastZipDotNet.GetPartFileName(zfe.DiskNumberStart);
        ////////////////////////        using (FileStream diskStream = new FileStream(partFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        ////////////////////////        {
        ////////////////////////            // Adjust the offset to be relative to the start of the disk
        ////////////////////////            long localHeaderOffsetInDisk = (long)zfe.HeaderOffset - cumulativeDiskSize;

        ////////////////////////            if (localHeaderOffsetInDisk < 0)
        ////////////////////////            {
        ////////////////////////                throw new InvalidDataException("Negative local header offset. The ZIP archive may be corrupt.");
        ////////////////////////            }

        ////////////////////////            // Seek to the local header in the disk stream
        ////////////////////////            diskStream.Seek(localHeaderOffsetInDisk, SeekOrigin.Begin);

        ////////////////////////            BinaryReader br = new BinaryReader(diskStream);

        ////////////////////////            // Read and validate the local file header signature
        ////////////////////////            uint signature = br.ReadUInt32();
        ////////////////////////            if (signature != 0x04034b50) // PK\003\004
        ////////////////////////            {
        ////////////////////////                throw new InvalidDataException($"Invalid local file header signature at offset {localHeaderOffsetInDisk} in disk {zfe.DiskNumberStart}");
        ////////////////////////            }

        ////////////////////////            // Read local file header fields
        ////////////////////////            ushort versionNeeded = br.ReadUInt16();
        ////////////////////////            ushort generalPurposeBitFlag = br.ReadUInt16();
        ////////////////////////            ushort compressionMethod = br.ReadUInt16();
        ////////////////////////            uint lastModDateTime = br.ReadUInt32();
        ////////////////////////            uint crc32 = br.ReadUInt32();
        ////////////////////////            uint compressedSize = br.ReadUInt32();
        ////////////////////////            uint uncompressedSize = br.ReadUInt32();
        ////////////////////////            ushort fileNameLength = br.ReadUInt16();
        ////////////////////////            ushort extraFieldLength = br.ReadUInt16();

        ////////////////////////            // Read file name and extra field
        ////////////////////////            byte[] fileNameBytes = br.ReadBytes(fileNameLength);
        ////////////////////////            byte[] extraField = br.ReadBytes(extraFieldLength);

        ////////////////////////            // Process extra field to handle Zip64
        ////////////////////////            ulong compressedSize64 = compressedSize;
        ////////////////////////            ulong uncompressedSize64 = uncompressedSize;

        ////////////////////////            if (compressedSize == 0xFFFFFFFF || uncompressedSize == 0xFFFFFFFF)
        ////////////////////////            {
        ////////////////////////                using (MemoryStream extraFieldStream = new MemoryStream(extraField))
        ////////////////////////                {
        ////////////////////////                    BinaryReader extraFieldReader = new BinaryReader(extraFieldStream);
        ////////////////////////                    while (extraFieldStream.Position < extraFieldStream.Length)
        ////////////////////////                    {
        ////////////////////////                        ushort headerId = extraFieldReader.ReadUInt16();
        ////////////////////////                        ushort dataSize = extraFieldReader.ReadUInt16();
        ////////////////////////                        if (headerId == 0x0001)
        ////////////////////////                        {
        ////////////////////////                            // ZIP64 Extended Information Extra Field
        ////////////////////////                            if (uncompressedSize == 0xFFFFFFFF)
        ////////////////////////                            {
        ////////////////////////                                uncompressedSize64 = extraFieldReader.ReadUInt64();
        ////////////////////////                            }
        ////////////////////////                            if (compressedSize == 0xFFFFFFFF)
        ////////////////////////                            {
        ////////////////////////                                compressedSize64 = extraFieldReader.ReadUInt64();
        ////////////////////////                            }
        ////////////////////////                            // Skip any remaining data
        ////////////////////////                            extraFieldReader.BaseStream.Seek(dataSize - (extraFieldReader.BaseStream.Position - 4), SeekOrigin.Current);
        ////////////////////////                        }
        ////////////////////////                        else
        ////////////////////////                        {
        ////////////////////////                            // Skip over non-ZIP64 extra fields
        ////////////////////////                            extraFieldReader.ReadBytes(dataSize);
        ////////////////////////                        }
        ////////////////////////                    }
        ////////////////////////                }
        ////////////////////////            }

        ////////////////////////            // Calculate the size of the local header
        ////////////////////////            ulong localHeaderSize = 30 + (ulong)fileNameLength + (ulong)extraFieldLength;

        ////////////////////////            // Prepare the MultiPartStream for the compressed data
        ////////////////////////            ulong compressedDataOffset = zfe.HeaderOffset + localHeaderSize;
        ////////////////////////            ulong compressedDataSize = compressedSize64;

        ////////////////////////            inputStream = new MultiPartStream(FastZipDotNet, compressedDataOffset, compressedDataSize, zfe.DiskNumberStart);

        ////////////////////////            // Select input stream for inflating or just reading
        ////////////////////////            Stream decompressionStream;
        ////////////////////////            if (zfe.Method == Compression.Store)
        ////////////////////////            {
        ////////////////////////                decompressionStream = inputStream;
        ////////////////////////            }
        ////////////////////////            else if (zfe.Method == Compression.Deflate)
        ////////////////////////            {
        ////////////////////////                decompressionStream = new DeflateStream(inputStream, CompressionMode.Decompress, true);
        ////////////////////////            }
        ////////////////////////            else if (zfe.Method == Compression.Zstd)
        ////////////////////////            {
        ////////////////////////                decompressionStream = new DecompressionStream(inputStream); // Ensure you have a Zstd decompression stream implementation
        ////////////////////////            }
        ////////////////////////            else
        ////////////////////////            {
        ////////////////////////                throw new NotSupportedException($"Unsupported compression method: {zfe.Method}");
        ////////////////////////            }

        ////////////////////////            // Initialize CRC
        ////////////////////////            uint crc32Calc = 0;

        ////////////////////////            // Buffered copy
        ////////////////////////            byte[] buffer = new byte[32768];
        ////////////////////////            ulong bytesPending = uncompressedSize64;
        ////////////////////////            int bytesRead;
        ////////////////////////            while (bytesPending > 0)
        ////////////////////////            {
        ////////////////////////                bytesRead = decompressionStream.Read(buffer, 0, (int)Math.Min(bytesPending, (ulong)buffer.Length));
        ////////////////////////                if (bytesRead <= 0)
        ////////////////////////                {
        ////////////////////////                    break;
        ////////////////////////                }

        ////////////////////////                crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
        ////////////////////////                bytesPending -= (ulong)bytesRead;

        ////////////////////////                outStream.Write(buffer, 0, bytesRead);

        ////////////////////////                Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
        ////////////////////////                Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);
        ////////////////////////            }

        ////////////////////////            outStream.Flush();

        ////////////////////////            if (compressionMethod == (ushort)Compression.Deflate || compressionMethod == (ushort)Compression.Zstd)
        ////////////////////////            {
        ////////////////////////                inputStream.Dispose();
        ////////////////////////            }
        ////////////////////////            if (FastZipDotNet.ZipFileStream == null)
        ////////////////////////            {
        ////////////////////////                decompressionStream.Dispose();
        ////////////////////////              //  inputStream.Dispose();
        ////////////////////////            }

        ////////////////////////            // Verify data integrity
        ////////////////////////            if (zfe.Crc32 != crc32Calc)
        ////////////////////////            {
        ////////////////////////                throw new InvalidDataException("CRC32 mismatch. Data corrupted.");
        ////////////////////////            }

        ////////////////////////            // Close streams
        ////////////////////////           // decompressionStream.Dispose();
        ////////////////////////            //inputStream.Dispose();
        ////////////////////////        }

        ////////////////////////        return true;
        ////////////////////////    }
        ////////////////////////    catch (Exception ex)
        ////////////////////////    {
        ////////////////////////        throw new Exception(ex.Message + "\r\nIn ExtractFile", ex);
        ////////////////////////    }
        ////////////////////////    finally
        ////////////////////////    {
        ////////////////////////      //  inputStream?.Dispose();
        ////////////////////////    }
        ////////////////////////}


        ////public bool ExtractFile(ZipFileEntry zfe, Stream outStream)
        ////{
        ////    Stream zipStream = null;

        ////    try
        ////    {
        ////        // Open the part where the local header is located
        ////        string partFileName = FastZipDotNet.GetPartFileName(zfe.DiskNumberStart);
        ////        zipStream = new FileStream(partFileName, FileMode.Open, FileAccess.Read);

        ////        if (!outStream.CanWrite)
        ////        {
        ////            throw new InvalidOperationException("Stream cannot be written");
        ////        }

        ////        // Check signature
        ////        zipStream.Seek((long)zfe.HeaderOffset, SeekOrigin.Begin);
        ////        BinaryReader br = new BinaryReader(zipStream, Encoding.Default, true);
        ////        uint signature = br.ReadUInt32();
        ////        if (signature != 0x04034b50) // PK\003\004
        ////        {
        ////            return false;
        ////        }

        ////        // Local file header
        ////        ushort versionNeeded = br.ReadUInt16();
        ////        ushort generalPurposeBitFlag = br.ReadUInt16();
        ////        ushort compressionMethod = br.ReadUInt16();
        ////        ushort lastModTime = br.ReadUInt16();
        ////        ushort lastModDate = br.ReadUInt16();
        ////        uint crc32 = br.ReadUInt32();
        ////        uint compressedSize = br.ReadUInt32();
        ////        uint uncompressedSize = br.ReadUInt32();
        ////        ushort fileNameLength = br.ReadUInt16();
        ////        ushort extraFieldLength = br.ReadUInt16();

        ////        br.ReadBytes(fileNameLength); // Skip filename

        ////        // Read extra field
        ////        ulong uncompressedSize64 = uncompressedSize;
        ////        ulong compressedSize64 = compressedSize;

        ////        if (compressedSize == 0xFFFFFFFF || uncompressedSize == 0xFFFFFFFF)
        ////        {
        ////            // Need to read ZIP64 extended information
        ////            byte[] extraField = br.ReadBytes(extraFieldLength);
        ////            MemoryStream extraFieldStream = new MemoryStream(extraField);
        ////            BinaryReader extraFieldReader = new BinaryReader(extraFieldStream);

        ////            while (extraFieldStream.Position < extraFieldLength)
        ////            {
        ////                ushort headerId = extraFieldReader.ReadUInt16();
        ////                ushort dataSize = extraFieldReader.ReadUInt16();
        ////                if (headerId == 0x0001)
        ////                {
        ////                    // ZIP64 Extended Information Extra Field
        ////                    if (uncompressedSize == 0xFFFFFFFF)
        ////                    {
        ////                        uncompressedSize64 = extraFieldReader.ReadUInt64();
        ////                    }
        ////                    if (compressedSize == 0xFFFFFFFF)
        ////                    {
        ////                        compressedSize64 = extraFieldReader.ReadUInt64();
        ////                    }
        ////                    // Skip any remaining data
        ////                    extraFieldReader.BaseStream.Seek(dataSize - (extraFieldReader.BaseStream.Position - 4), SeekOrigin.Current);
        ////                }
        ////                else
        ////                {
        ////                    // Skip over non-ZIP64 extra fields
        ////                    extraFieldReader.ReadBytes(dataSize);
        ////                }
        ////            }
        ////        }
        ////        else
        ////        {
        ////            // Read and discard extra field
        ////            br.ReadBytes(extraFieldLength);
        ////        }

        ////        // At this point, the zipStream position is at the start of the compressed data

        ////        // Select input stream for inflating or just reading
        ////        Stream inputStream;
        ////        if (compressionMethod == (ushort)Compression.Store)
        ////        {
        ////            inputStream = zipStream;
        ////        }
        ////        else if (compressionMethod == (ushort)Compression.Deflate)
        ////        {
        ////            inputStream = new DeflateStream(zipStream, CompressionMode.Decompress, true);
        ////        }
        ////        else if (compressionMethod == (ushort)Compression.Zstd)
        ////        {
        ////            //using var decompressionStream = new DecompressionStream(zipStream);
        ////            // decompressionStream.CopyTo(inputStream);

        ////            inputStream = new DecompressionStream(zipStream);//new DeflateStream(zipStream, CompressionMode.Decompress, true);
        ////        }
        ////        else
        ////        {
        ////            return false; // Unsupported compression method
        ////        }

        ////        // Initialize CRC
        ////        uint crc32Calc = 0;//0xffffffff;

        ////        // Buffered copy
        ////        byte[] buffer = new byte[32768];//new byte[16384];
        ////        ulong bytesPending = uncompressedSize64;
        ////        int bytesRead;
        ////        while (bytesPending > 0)
        ////        {
        ////            // Read from current part
        ////            bytesRead = inputStream.Read(buffer, 0, (int)Math.Min(bytesPending, (ulong)buffer.Length));
        ////            if (bytesRead <= 0)
        ////            {
        ////                // Check if there is next part to switch to
        ////                if (FastZipDotNet.CurrentDiskNumber < FastZipDotNet.GetTotalDisks() - 1)
        ////                {
        ////                    // Switch to next part
        ////                    zipStream.Dispose();
        ////                    FastZipDotNet.CurrentDiskNumber++;
        ////                    string nextPartName = FastZipDotNet.GetPartFileName(FastZipDotNet.CurrentDiskNumber);
        ////                    zipStream = new FileStream(nextPartName, FileMode.Open, FileAccess.Read);
        ////                    inputStream = zipStream;

        ////                    continue;
        ////                }
        ////                else
        ////                {
        ////                    break;
        ////                }
        ////            }

        ////            crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
        ////            //crc32Calc = Crc32Algorithm.Append(buffer, 0, bytesRead);//ComputeCrc32(crc32Calc, ref buffer, bytesRead);
        ////            bytesPending -= (ulong)bytesRead;

        ////            outStream.Write(buffer, 0, bytesRead);
        ////            bytesPending -= (ulong)bytesRead;

        ////            Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
        ////            Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);
        ////        }
        ////        Interlocked.Increment(ref FastZipDotNet.FilesPerSecond);
        ////        Interlocked.Increment(ref FastZipDotNet.FilesWritten);
        ////        outStream.Flush();

        ////        // Close streams
        ////        if (compressionMethod == (ushort)Compression.Deflate || compressionMethod == (ushort)Compression.Zstd)
        ////        {
        ////            inputStream.Dispose();
        ////        }
        ////        if (FastZipDotNet.ZipFileStream == null)
        ////        {
        ////            zipStream.Dispose();
        ////        }

        ////        // Verify data integrity
        ////        // crc32Calc ^= 0xffffffff;
        ////        if (zfe.Crc32 != crc32Calc)
        ////        {
        ////            return false;
        ////        }
        ////        return true;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        throw new Exception(ex.Message + "\r\nIn BrutalUnZip.ExtractFile");
        ////    }
        ////}

        public bool ExtractFile(ZipFileEntry zfe, Stream outStream)
        {
            Stream zipStream = null;

            try
            {
                // Open a separate read handle so we never contend with the writer
                zipStream = new FileStream(FastZipDotNet.ZipFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1 << 20, FileOptions.SequentialScan);

                if (!outStream.CanWrite)
                    throw new InvalidOperationException("Stream cannot be written");

                zipStream.Seek((long)zfe.HeaderOffset, SeekOrigin.Begin);
                using (var br = new BinaryReader(zipStream, Encoding.Default, leaveOpen: true))
                {
                    uint signature = br.ReadUInt32();
                    if (signature != 0x04034b50)
                        return false;

                    ushort versionNeeded = br.ReadUInt16();
                    ushort generalPurposeBitFlag = br.ReadUInt16();
                    ushort compressionMethod = br.ReadUInt16();
                    ushort lastModTime = br.ReadUInt16();
                    ushort lastModDate = br.ReadUInt16();
                    uint crc32 = br.ReadUInt32();
                    uint compressedSize = br.ReadUInt32();
                    uint uncompressedSize = br.ReadUInt32();
                    ushort fileNameLength = br.ReadUInt16();
                    ushort extraFieldLength = br.ReadUInt16();

                    br.ReadBytes(fileNameLength); // skip name

                    ulong uncompressedSize64 = uncompressedSize;
                    ulong compressedSize64 = compressedSize;

                    if (compressedSize == 0xFFFFFFFF || uncompressedSize == 0xFFFFFFFF)
                    {
                        byte[] extraField = br.ReadBytes(extraFieldLength);
                        using var extraFieldStream = new MemoryStream(extraField);
                        using var extraFieldReader = new BinaryReader(extraFieldStream);
                        while (extraFieldStream.Position < extraFieldLength)
                        {
                            ushort headerId = extraFieldReader.ReadUInt16();
                            ushort dataSize = extraFieldReader.ReadUInt16();
                            if (headerId == 0x0001)
                            {
                                if (uncompressedSize == 0xFFFFFFFF)
                                    uncompressedSize64 = extraFieldReader.ReadUInt64();
                                if (compressedSize == 0xFFFFFFFF)
                                    compressedSize64 = extraFieldReader.ReadUInt64();
                                long remain = dataSize - (long)(extraFieldStream.Position - 4);
                                if (remain > 0) extraFieldReader.BaseStream.Seek(remain, SeekOrigin.Current);
                            }
                            else
                            {
                                extraFieldReader.ReadBytes(dataSize);
                            }
                        }
                    }
                    else
                    {
                        br.ReadBytes(extraFieldLength);
                    }

                    Stream inputStream;
                    if (compressionMethod == (ushort)Compression.Store)
                        inputStream = zipStream;
                    else if (compressionMethod == (ushort)Compression.Deflate)
                        inputStream = new DeflateStream(zipStream, CompressionMode.Decompress, true);
                    else if (compressionMethod == (ushort)Compression.Zstd)
                        inputStream = new DecompressionStream(zipStream);
                    else
                        return false;

                    uint crc32Calc = 0;
                    byte[] buffer = new byte[32768];
                    ulong bytesPending = uncompressedSize64;

                    while (bytesPending > 0)
                    {
                        int bytesRead = inputStream.Read(buffer, 0, (int)Math.Min(bytesPending, (ulong)buffer.Length));
                        if (bytesRead <= 0) break;

                        crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
                        bytesPending -= (ulong)bytesRead;
                        outStream.Write(buffer, 0, bytesRead);

                        Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
                        Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);
                    }

                    Interlocked.Increment(ref FastZipDotNet.FilesPerSecond);
                    Interlocked.Increment(ref FastZipDotNet.FilesWritten);
                    outStream.Flush();

                    if (compressionMethod == (ushort)Compression.Deflate || compressionMethod == (ushort)Compression.Zstd)
                        inputStream.Dispose();

                    return zfe.Crc32 == crc32Calc;
                }
            }
            finally
            {
                zipStream?.Dispose();
            }
        }
        #endregion

        #region Recovery





        #endregion

    }
}
