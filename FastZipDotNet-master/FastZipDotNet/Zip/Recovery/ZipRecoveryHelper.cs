using FastZipDotNet.Zip.Helpers;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
using static FastZipDotNet.Zip.Structure.ZipEntryStructs;
using System.Text;
using FastZipDotNet.Zip.Cryptography;
using FastZipDotNet.Zip.ZStd;
using System.IO.Compression;
using System.Collections.Concurrent;

namespace FastZipDotNet.Zip.Recovery
{
    public static class ZipRecoveryHelper
    {


        public static RecoveryModeResult RecoverCorruptedZipFile(string zipPath, string outputDirectory)
        {
            if (File.Exists(zipPath))
            {
                var ZipFileStream = new FileStream(zipPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 8388608);

                return RecoverCorruptedZipStream(ZipFileStream, outputDirectory);
            }

            return new RecoveryModeResult();
        }


        public static RecoveryModeResult RecoverCorruptedZipStream(Stream zipStream, string outputDirectory)
        {
            RecoveryModeResult result = new RecoveryModeResult();
            List<ZipFileEntry> entries = ScanForLocalFileHeadersCorruption(zipStream);

            ConcurrentBag<ZipFileEntry> RecoveredEntries = new ConcurrentBag<ZipFileEntry>();
            ConcurrentBag<ZipFileEntry> CorruptedEntries = new ConcurrentBag<ZipFileEntry>();
            Parallel.ForEach(entries, entry =>
            {
                try
                {
                    string outputPath = Path.Combine(outputDirectory, entry.FilenameInZip);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)); // Ensure directory exists

                    using (var outStream = new FileStream(outputPath, FileMode.Create))
                    {
                        bool success = ExtractFileRecovery(entry, outStream, zipStream);
                        if (success)
                        {
                            RecoveredEntries.Add(entry);
                        }
                        else
                        {
                            CorruptedEntries.Add(entry);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Debug.WriteLine($"Error extracting {entry.FilenameInZip}: {ex.Message}");
                    CorruptedEntries.Add(entry);
                    //result.CorruptedEntries.Add(entry); // Optionally log these as corrupted
                }
            });

            result.CorruptedEntries = CorruptedEntries.ToList();
            result.RecoveredEntries = RecoveredEntries.ToList();
            result.Succeeded = true;
            return result;
        }

        private static List<ZipFileEntry> ScanForLocalFileHeadersCorruption(Stream zipStream)
        {
            List<ZipFileEntry> entries = new List<ZipFileEntry>();
            long streamLength = zipStream.Length;
            zipStream.Seek(0, SeekOrigin.Begin);

            while (zipStream.Position < streamLength)
            {
                long headerOffset = zipStream.Position;
                BinaryReader br = new BinaryReader(zipStream, Encoding.Default, true);
                uint signature = 0;
                try
                {
                    signature = br.ReadUInt32();
                }
                catch (EndOfStreamException ex)
                {
                    return entries;
                }
          

                if (signature != 0x04034b50)
                {
                    // Not a local file header; move back 3 bytes and read the next byte
                    zipStream.Seek(-3, SeekOrigin.Current);
                    continue;
                }

                try
                {
                    ZipFileEntry zfe = new ZipFileEntry();
                    zfe.HeaderOffset = (ulong)headerOffset;
                    var VersionNeededToExtract = br.ReadUInt16();
                    var GeneralPurposeBitFlag = br.ReadUInt16();
                    //zfe.VersionNeededToExtract = br.ReadUInt16();
                    //zfe.GeneralPurposeBitFlag = br.ReadUInt16();
                    zfe.Method = (Compression)br.ReadUInt16();
                    zfe.ModifyTime = DosHelpers.DosTimeToDateTime(br.ReadUInt32());
                    zfe.Crc32 = br.ReadUInt32();
                    zfe.CompressedSize = br.ReadUInt32();
                    zfe.FileSize = br.ReadUInt32();
                    ushort fileNameLength = br.ReadUInt16();
                    ushort extraFieldLength = br.ReadUInt16();

                    byte[] fileNameBytes = br.ReadBytes(fileNameLength);
                    zfe.FilenameInZip = Encoding.UTF8.GetString(fileNameBytes);

                    byte[] extraField = br.ReadBytes(extraFieldLength);

                    // Handle ZIP64 extensions if necessary
                    if (zfe.CompressedSize == 0xFFFFFFFF || zfe.FileSize == 0xFFFFFFFF)
                    {
                        using (MemoryStream extraFieldStream = new MemoryStream(extraField))
                        using (BinaryReader extraFieldReader = new BinaryReader(extraFieldStream))
                        {
                            while (extraFieldStream.Position < extraFieldLength)
                            {
                                ushort headerId = extraFieldReader.ReadUInt16();
                                ushort dataSize = extraFieldReader.ReadUInt16();
                                if (headerId == 0x0001)
                                {
                                    // ZIP64 extended information
                                    if (zfe.FileSize == 0xFFFFFFFF)
                                    {
                                        zfe.FileSize = extraFieldReader.ReadUInt64();
                                    }
                                    if (zfe.CompressedSize == 0xFFFFFFFF)
                                    {
                                        zfe.CompressedSize = extraFieldReader.ReadUInt64();
                                    }
                                    break;
                                }
                                else
                                {
                                    extraFieldReader.ReadBytes(dataSize);
                                }
                            }
                        }
                    }

                    zfe.HeaderSize = (uint)(30 + fileNameLength + extraFieldLength);

                    // Move the stream past the file data
                    zipStream.Seek((long)zfe.CompressedSize, SeekOrigin.Current);

                    entries.Add(zfe);

                    // No need to adjust position; we're already at the next header or EOF
                }
                catch
                {
                    // If an error occurs, move back to the position after the signature and continue
                    zipStream.Seek(headerOffset + 4, SeekOrigin.Begin);
                    continue;
                }
            }

            return entries;
        }

        public class RecoveryModeResult
        {
            public bool Succeeded = false;
            public List<ZipFileEntry> RecoveredEntries { get; set; } = new List<ZipFileEntry>();
            public List<ZipFileEntry> CorruptedEntries { get; set; } = new List<ZipFileEntry>();
        }


     


        private static bool ExtractFileRecovery(ZipFileEntry zfe, Stream outStream, Stream zipStream)
        {
            try
            {
                if (!outStream.CanWrite)
                {
                    throw new InvalidOperationException("Stream cannot be written");
                }

                // Seek to the header offset
                zipStream.Seek((long)zfe.HeaderOffset, SeekOrigin.Begin);
                BinaryReader br = new BinaryReader(zipStream, Encoding.Default, true);

                // Read and validate local file header signature
                uint signature = br.ReadUInt32();
                if (signature != 0x04034b50)
                {
                    return false;
                }

                // Skip over fields we already have
                br.ReadUInt16(); // versionNeeded
                br.ReadUInt16(); // generalPurposeBitFlag
                br.ReadUInt16(); // compressionMethod
                br.ReadUInt16(); // lastModTime
                br.ReadUInt16(); // lastModDate
                br.ReadUInt32(); // crc32
                br.ReadUInt32(); // compressedSize
                br.ReadUInt32(); // uncompressedSize
                ushort fileNameLength = br.ReadUInt16();
                ushort extraFieldLength = br.ReadUInt16();

                // Skip over the filename and extra fields
                br.ReadBytes(fileNameLength + extraFieldLength);

                // At this point, the stream position is at the start of the compressed data
                Stream inputStream;
                if (zfe.Method == Compression.Store)
                {
                    inputStream = zipStream;
                }
                else if (zfe.Method == Compression.Deflate)
                {
                    inputStream = new DeflateStream(zipStream, CompressionMode.Decompress, true);
                }
                else if (zfe.Method == Compression.Zstd)
                {
                    inputStream = new DecompressionStream(zipStream);
                }
                else
                {
                    return false; // Unsupported compression method
                }

                uint crc32Calc = 0;
                byte[] buffer = new byte[32768];
                ulong bytesPending = zfe.FileSize;
                int bytesRead;

                while (bytesPending > 0)
                {
                    bytesRead = inputStream.Read(buffer, 0, (int)Math.Min(bytesPending, (ulong)buffer.Length));
                    if (bytesRead <= 0)
                        break;

                    crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
                    bytesPending -= (ulong)bytesRead;
                    outStream.Write(buffer, 0, bytesRead);
                }

                outStream.Flush();

                // Close streams
                if (zfe.Method == Compression.Deflate || zfe.Method == Compression.Zstd)
                {
                    inputStream.Dispose();
                }

                // Verify data integrity
                if (zfe.Crc32 != crc32Calc)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn BrutalUnZip.ExtractFileRecovery");
            }
        }
    }
}
