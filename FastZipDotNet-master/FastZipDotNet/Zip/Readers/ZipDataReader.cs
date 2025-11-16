using FastZipDotNet.MultiThreading;
using FastZipDotNet.Zip.Cryptography;
using FastZipDotNet.Zip.Encryption;
using FastZipDotNet.Zip.Helpers;
using FastZipDotNet.Zip.ZStd;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
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

                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount * 2 : threads));
                var sem = FastZipDotNet.ConcurrencyLimiter;
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

                        try { await Task.Delay(50, reportCts.Token).ConfigureAwait(false); }
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
                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount * 2 : threads));
                var semaphore = FastZipDotNet.ConcurrencyLimiter;
                //var semaphore = new AdjustableSemaphore(threads);
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

                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount * 2 : threads));
                var semaphore = FastZipDotNet.ConcurrencyLimiter;

                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                //var semaphore = new AdjustableSemaphore(threads);
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

                            try { await Task.Delay(50, reportCts.Token).ConfigureAwait(false); }
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


        // Extract to path (reports uncompressed bytes via onBytes)
        public bool ExtractFile(ZipFileEntry zfe, string outPathFilename, Action<long> onBytes = null)
        {
            Stream output = null;
            try
            {
                // Ensure parent dir exists
                string? dir = Path.GetDirectoryName(outPathFilename);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Directory entry? Create dir and return
                if (zfe.FilenameInZip.EndsWith("/") || zfe.FilenameInZip.EndsWith("\\"))
                {
                    Directory.CreateDirectory(outPathFilename);
                    return true;
                }

                // Replace existing file
                if (File.Exists(outPathFilename))
                {
                    try { File.Delete(outPathFilename); }
                    catch { throw new InvalidOperationException($"File '{outPathFilename}' cannot be written"); }
                }

                while (FastZipDotNet.Pause) Thread.Sleep(50);

                output = new FileStream(outPathFilename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete, 1 << 20, FileOptions.SequentialScan);

                bool ok = ExtractFile(zfe, output, onBytes);
                output.Close();

                // Set times if OK
                if (ok)
                {
                    try
                    {
                        File.SetCreationTime(outPathFilename, zfe.ModifyTime);
                        File.SetLastWriteTime(outPathFilename, zfe.ModifyTime);
                    }
                    catch { /* ignore */ }
                }

                return ok;
            }
            finally
            {
                output?.Dispose();
            }
        }

        // Extract to an existing stream (reports uncompressed bytes via onBytes)
        public bool ExtractFile(ZipFileEntry zfe, Stream outStream, Action<long> onBytes = null)
        {
            if (outStream is null) throw new ArgumentNullException(nameof(outStream));
            if (!outStream.CanWrite) throw new InvalidOperationException("Stream cannot be written");

            FileStream zipStream = null;
            try
            {
                zipStream = new FileStream(
                    FastZipDotNet.ZipFileName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite,
                    1 << 20,
                    FileOptions.RandomAccess);

                zipStream.Seek((long)zfe.HeaderOffset, SeekOrigin.Begin);
                using var br = new BinaryReader(zipStream, Encoding.Default, leaveOpen: true);

                uint signature = br.ReadUInt32();
                if (signature != 0x04034b50) return false;

                ushort versionNeeded = br.ReadUInt16();
                ushort generalPurposeBitFlag = br.ReadUInt16();
                ushort compressionMethodLocal = br.ReadUInt16(); // may be 99 for AES
                ushort lastModTime = br.ReadUInt16();
                ushort lastModDate = br.ReadUInt16();
                uint crc32Local = br.ReadUInt32();
                uint compressedSizeLocal = br.ReadUInt32();
                uint uncompressedSizeLocal = br.ReadUInt32();
                ushort fileNameLength = br.ReadUInt16();
                ushort extraFieldLength = br.ReadUInt16();

                if (fileNameLength > 0) br.ReadBytes(fileNameLength);
                if (extraFieldLength > 0) br.ReadBytes(extraFieldLength);

                bool hasDataDescriptor = (generalPurposeBitFlag & 0x0008) != 0;
                bool isEncrypted = (generalPurposeBitFlag & 0x0001) != 0;

                // method decision
                Compression actualMethod = zfe.IsAes ? zfe.Method : (Compression)compressionMethodLocal;

                // Build input stream (AES or ZipCrypto or none)
                Stream inputStream = null;
                try
                {
                    if (isEncrypted)
                    {
                        if (zfe.IsAes)
                        {
                            if (string.IsNullOrEmpty(FastZipDotNet.Password))
                                throw new CryptographicException("Password required for AES-encrypted entry.");

                            // total encrypted size includes salt + pwv + ciphertext + 10 tag
                            long totalEncryptedSize = (long)zfe.CompressedSize;
                            if (totalEncryptedSize <= 0) throw new InvalidDataException("Invalid AES compressed size");

                            byte strength = zfe.AesStrength != 0 ? zfe.AesStrength : (byte)3; // default to 256 if unknown
                            var aes = new AesDecryptStream(zipStream, FastZipDotNet.Password, strength, totalEncryptedSize);

                            // Decompressor wraps the AES plaintext stream
                            if (actualMethod == Compression.Store)
                                inputStream = aes;
                            else if (actualMethod == Compression.Deflate)
                                inputStream = new DeflateStream(aes, CompressionMode.Decompress, leaveOpen: true);
                            else if (actualMethod == Compression.Zstd)
                                inputStream = new DecompressionStream(aes);
                            else
                                return false;
                        }
                        else
                        {
                            // ZipCrypto
                            if (string.IsNullOrEmpty(FastZipDotNet.Password))
                                throw new CryptographicException("Password required for encrypted ZIP entry.");

                            byte verifier = hasDataDescriptor ? (byte)(lastModTime >> 8) : (byte)(zfe.Crc32 >> 24);

                            long payloadLen = (long)zfe.CompressedSize - 12;
                            if (payloadLen < 0) throw new InvalidDataException("Invalid encrypted size");

                            var decrypt = new Encryption.ZipCryptoDecryptStream(zipStream, FastZipDotNet.Password, verifier, payloadLen);

                            if (actualMethod == Compression.Store)
                                inputStream = decrypt;
                            else if (actualMethod == Compression.Deflate)
                                inputStream = new DeflateStream(decrypt, CompressionMode.Decompress, leaveOpen: true);
                            else if (actualMethod == Compression.Zstd)
                                inputStream = new DecompressionStream(decrypt);
                            else
                                return false;
                        }
                    }
                    else
                    {
                        if (actualMethod == Compression.Store)
                            inputStream = zipStream;
                        else if (actualMethod == Compression.Deflate)
                            inputStream = new DeflateStream(zipStream, CompressionMode.Decompress, leaveOpen: true);
                        else if (actualMethod == Compression.Zstd)
                            inputStream = new DecompressionStream(zipStream);
                        else
                            return false;
                    }

                    uint crc32Calc = 0;
                    byte[] buffer = new byte[32768];
                    ulong remaining = zfe.FileSize;

                    while (remaining > 0)
                    {
                        while (FastZipDotNet.Pause) Thread.Sleep(50);

                        int toRead = (int)Math.Min((ulong)buffer.Length, remaining);
                        int bytesRead = inputStream.Read(buffer, 0, toRead);
                        if (bytesRead <= 0) break;

                        crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
                        outStream.Write(buffer, 0, bytesRead);
                        onBytes?.Invoke(bytesRead);

                        Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
                        Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);

                        remaining -= (ulong)bytesRead;
                    }

                    Interlocked.Increment(ref FastZipDotNet.FilesPerSecond);
                    Interlocked.Increment(ref FastZipDotNet.FilesWritten);
                    outStream.Flush();

                    // For AES, CRC field may be 0; do not strictly fail on CRC mismatch if AES; MAC already verified by stream
                    if (zfe.IsAes) return remaining == 0;
                    return remaining == 0 && (zfe.Crc32 == crc32Calc);
                }
                finally
                {
                    if (inputStream != null && inputStream != zipStream)
                        inputStream.Dispose();
                }
            }
            finally
            {
                zipStream?.Dispose();
            }
        }

        // Keep this convenience overload for compatibility
        public bool ExtractFile(ZipFileEntry zfe, string outPathFilename)
            => ExtractFile(zfe, outPathFilename, onBytes: null);


        /// <summary>
        /// Copy the contents of a stored file into a physical file
        /// </summary>
        /// <param name="zfe">Entry information of file to extract</param>
        /// <param name="outPathFilename">Name of file to store uncompressed data</param>
        /// <returns>True if success, false if not.</returns>
        //public bool ExtractFile(ZipFileEntry zfe, string outPathFilename)
        //{
        //    Stream output = null;

        //    try
        //    {
        //        // Ensure the parent directory exists
        //        string path = Path.GetDirectoryName(outPathFilename);
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        // Check if it's a directory. If so, do nothing
        //        if (Directory.Exists(outPathFilename))
        //        {
        //            return true;
        //        }

        //        // Delete file if it already exists
        //        if (File.Exists(outPathFilename))
        //        {
        //            try
        //            {
        //                File.Delete(outPathFilename);
        //            }
        //            catch
        //            {
        //                throw new InvalidOperationException("File '" + outPathFilename + "' cannot be written");
        //            }
        //        }

        //        while (FastZipDotNet.Pause)
        //        {
        //            Thread.Sleep(50);
        //        }


        //        while (IOHelpers.IsFileLocked(outPathFilename))
        //        {
        //            Thread.Sleep(5);
        //        }
        //        try
        //        {
        //            output = new FileStream(outPathFilename, FileMode.Create, FileAccess.Write);
        //        }
        //        catch
        //        {
        //            for (int i = 0; i < 32; i++)
        //            {
        //                while (IOHelpers.IsFileLocked(outPathFilename))
        //                {
        //                    Thread.Sleep(5);
        //                   // Application.DoEvents();
        //                }
        //                try
        //                {

        //                    Thread.Sleep(100);
        //                    output = new FileStream(outPathFilename, FileMode.Create, FileAccess.Write);
        //                    break;
        //                }
        //                catch
        //                {

        //                }
        //            }

        //        }

        //        // lock (lockObj)
        //        if (!ExtractFile(zfe, output))
        //        {
        //            return false;
        //        }

        //        // Change file datetimes
        //        output.Close();
        //        while (IOHelpers.IsFileLocked(outPathFilename))
        //        {
        //            Thread.Sleep(5);
        //        }
        //        File.SetCreationTime(outPathFilename, zfe.ModifyTime);
        //        File.SetLastWriteTime(outPathFilename, zfe.ModifyTime);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message + "\r\nIn BrutalUnZip.ExtractFile");
        //    }
        //    finally
        //    {
        //        if (output != null)
        //        {
        //            output.Close();
        //            output.Dispose();
        //        }
        //    }
        //}


        public bool ExtractFile(ZipFileEntry zfe, Stream outStream)
    => ExtractFile(zfe, outStream, onBytes: null);

        //public bool ExtractFile(ZipFileEntry zfe, Stream outStream)
        //{
        //{
        //    Stream zipStream = null;

        //    try
        //    {
        //        // Open a separate read handle so we never contend with the writer
        //        zipStream = new FileStream(FastZipDotNet.ZipFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1 << 20, FileOptions.SequentialScan);

        //        if (!outStream.CanWrite)
        //            throw new InvalidOperationException("Stream cannot be written");

        //        zipStream.Seek((long)zfe.HeaderOffset, SeekOrigin.Begin);
        //        using (var br = new BinaryReader(zipStream, Encoding.Default, leaveOpen: true))
        //        {
        //            uint signature = br.ReadUInt32();
        //            if (signature != 0x04034b50)
        //                return false;

        //            ushort versionNeeded = br.ReadUInt16();
        //            ushort generalPurposeBitFlag = br.ReadUInt16();
        //            ushort compressionMethod = br.ReadUInt16();
        //            ushort lastModTime = br.ReadUInt16();
        //            ushort lastModDate = br.ReadUInt16();
        //            uint crc32 = br.ReadUInt32();
        //            uint compressedSize = br.ReadUInt32();
        //            uint uncompressedSize = br.ReadUInt32();
        //            ushort fileNameLength = br.ReadUInt16();
        //            ushort extraFieldLength = br.ReadUInt16();

        //            br.ReadBytes(fileNameLength); // skip name

        //            ulong uncompressedSize64 = uncompressedSize;
        //            ulong compressedSize64 = compressedSize;

        //            if (compressedSize == 0xFFFFFFFF || uncompressedSize == 0xFFFFFFFF)
        //            {
        //                byte[] extraField = br.ReadBytes(extraFieldLength);
        //                using var extraFieldStream = new MemoryStream(extraField);
        //                using var extraFieldReader = new BinaryReader(extraFieldStream);
        //                while (extraFieldStream.Position < extraFieldLength)
        //                {
        //                    ushort headerId = extraFieldReader.ReadUInt16();
        //                    ushort dataSize = extraFieldReader.ReadUInt16();
        //                    if (headerId == 0x0001)
        //                    {
        //                        if (uncompressedSize == 0xFFFFFFFF)
        //                            uncompressedSize64 = extraFieldReader.ReadUInt64();
        //                        if (compressedSize == 0xFFFFFFFF)
        //                            compressedSize64 = extraFieldReader.ReadUInt64();
        //                        long remain = dataSize - (long)(extraFieldStream.Position - 4);
        //                        if (remain > 0) extraFieldReader.BaseStream.Seek(remain, SeekOrigin.Current);
        //                    }
        //                    else
        //                    {
        //                        extraFieldReader.ReadBytes(dataSize);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                br.ReadBytes(extraFieldLength);
        //            }

        //            Stream inputStream;
        //            if (compressionMethod == (ushort)Compression.Store)
        //                inputStream = zipStream;
        //            else if (compressionMethod == (ushort)Compression.Deflate)
        //                inputStream = new DeflateStream(zipStream, CompressionMode.Decompress, true);
        //            else if (compressionMethod == (ushort)Compression.Zstd)
        //                inputStream = new DecompressionStream(zipStream);
        //            else
        //                return false;

        //            uint crc32Calc = 0;
        //            byte[] buffer = new byte[32768];
        //            ulong bytesPending = uncompressedSize64;

        //            while (bytesPending > 0)
        //            {
        //                int bytesRead = inputStream.Read(buffer, 0, (int)Math.Min(bytesPending, (ulong)buffer.Length));
        //                if (bytesRead <= 0) break;

        //                crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
        //                bytesPending -= (ulong)bytesRead;
        //                outStream.Write(buffer, 0, bytesRead);

        //                Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
        //                Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);
        //            }

        //            Interlocked.Increment(ref FastZipDotNet.FilesPerSecond);
        //            Interlocked.Increment(ref FastZipDotNet.FilesWritten);
        //            outStream.Flush();

        //            if (compressionMethod == (ushort)Compression.Deflate || compressionMethod == (ushort)Compression.Zstd)
        //                inputStream.Dispose();

        //            return zfe.Crc32 == crc32Calc;
        //        }
        //    }
        //    finally
        //    {
        //        zipStream?.Dispose();
        //    }
        //}
        #endregion


    }
}
