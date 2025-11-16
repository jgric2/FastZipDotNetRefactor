using FastZipDotNet.MultiThreading;
using FastZipDotNet.Zip.Cryptography;
using FastZipDotNet.Zip.Encryption;
using FastZipDotNet.Zip.Helpers;
using FastZipDotNet.Zip.LibDeflate;
using FastZipDotNet.Zip.ZStd;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
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

        private FastZipDotNet FastZipDotNet;

        // For consistency with the writer. We also use this as the initial seed-size
        // of compressed input we want to have before calling the block API.
        private const int DefaultBufferThreshold = 4 * 1024 * 1024; // 4MB

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
                var tasks = new List<Task>();
                foreach (var currentEntry in FastZipDotNet.ZipFileEntries)
                {
                    await Task.Run(() => semaphore.WaitOne(cts.Token));
                    var entry = currentEntry;
                    var task = Task.Run(() =>
                    {
                        try
                        {
                            if (!TestFile(entry))
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

                var results = Task.WhenAll(tasks);
                return results.IsCompletedSuccessfully;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn BrutalUnZip.Test");
            }
        }

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

                int totalFiles = FastZipDotNet.ZipFileEntries.Count;
                long totalUncompressed = 0;
                long totalCompressed = 0;
                foreach (var e in FastZipDotNet.ZipFileEntries)
                {
                    totalUncompressed += (long)e.FileSize;
                    totalCompressed += (long)e.CompressedSize;
                }

                long processedUncompressed = 0;
                long processedCompressedApprox = 0;
                int filesDone = 0;
                string lastFileName = null;

                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount * 2 : threads));
                var semaphore = FastZipDotNet.ConcurrencyLimiter;

                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                var tasks = new List<Task>(totalFiles);
                var sw = Stopwatch.StartNew();

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
                    var entry = currentEntry;

                    var task = Task.Run(() =>
                    {
                        try
                        {
                            string outPath = Path.Combine(outputDirectory, entry.FilenameInZip);

                            double compPerUnc = (entry.FileSize > 0) ? (double)entry.CompressedSize / (double)entry.FileSize : 0.0;

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
                        BytesProcessedCompressed = totalCompressed,

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
                string? dir = Path.GetDirectoryName(outPathFilename);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Directory entry?
                if (zfe.FilenameInZip.EndsWith("/") || zfe.FilenameInZip.EndsWith("\\"))
                {
                    Directory.CreateDirectory(outPathFilename);
                    return true;
                }

                if (File.Exists(outPathFilename))
                {
                    try { File.Delete(outPathFilename); }
                    catch { throw new InvalidOperationException($"File '{outPathFilename}' cannot be written"); }
                }

                WaitWhilePaused();

                output = new FileStream(outPathFilename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete, 1 << 20, FileOptions.SequentialScan);

                bool ok = ExtractFile(zfe, output, onBytes);
                output.Close();

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

                Compression actualMethod = zfe.IsAes ? zfe.Method : (Compression)compressionMethodLocal;

                // Build an input stream bounded to this entry's compressed payload
                Stream payloadStream = null;
                long dataStartOffset = zipStream.Position; // start of compressed payload in the ZIP
                try
                {
                    if (isEncrypted)
                    {
                        if (zfe.IsAes)
                        {
                            if (string.IsNullOrEmpty(FastZipDotNet.Password))
                                throw new CryptographicException("Password required for AES-encrypted entry.");

                            long totalEncryptedSize = (long)zfe.CompressedSize;
                            var aes = new AesDecryptStream(zipStream, FastZipDotNet.Password, zfe.AesStrength != 0 ? zfe.AesStrength : (byte)3, totalEncryptedSize);
                            payloadStream = aes;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(FastZipDotNet.Password))
                                throw new CryptographicException("Password required for encrypted ZIP entry.");

                            byte verifier = hasDataDescriptor ? (byte)(lastModTime >> 8) : (byte)(zfe.Crc32 >> 24);
                            long payloadLen = (long)zfe.CompressedSize - 12;
                            if (payloadLen < 0) throw new InvalidDataException("Invalid encrypted size");

                            var decrypt = new Encryption.ZipCryptoDecryptStream(zipStream, FastZipDotNet.Password, verifier, payloadLen);
                            payloadStream = decrypt;
                        }
                    }
                    else
                    {
                        payloadStream = new BoundedReadStream(zipStream, (long)zfe.CompressedSize);
                    }

                    if (actualMethod == Compression.Store)
                    {
                        uint crc32Calc = 0;
                        byte[] buffer = new byte[1 << 20];
                        ulong remaining = zfe.FileSize;

                        while (remaining > 0)
                        {
                            WaitWhilePaused();
                            int toRead = (int)Math.Min((ulong)buffer.Length, remaining);
                            int bytesRead = payloadStream.Read(buffer, 0, toRead);
                            if (bytesRead <= 0) break;

                            crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
                            outStream.Write(buffer, 0, bytesRead);
                            onBytes?.Invoke(bytesRead);

                            Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
                            Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);

                            remaining -= (ulong)bytesRead;
                        }
                        outStream.Flush();

                        return remaining == 0 && (zfe.IsAes || zfe.Crc32 == crc32Calc);
                    }
                    else if (actualMethod == Compression.Deflate)
                    {
                        bool usedLibDeflate = TryInflateWithLibdeflate(payloadStream, zfe.FileSize, outStream, onBytes, out uint crc32Calc);
                        if (!usedLibDeflate)
                        {
                            // Fallback path: rebuild bounded payload stream
                            payloadStream.Dispose();
                            if (!isEncrypted)
                            {
                                zipStream.Seek(dataStartOffset, SeekOrigin.Begin);
                                payloadStream = new BoundedReadStream(zipStream, (long)zfe.CompressedSize);
                            }
                            else
                            {
                                if (zfe.IsAes)
                                {
                                    long totalEncryptedSize = (long)zfe.CompressedSize;
                                    payloadStream = new AesDecryptStream(zipStream, FastZipDotNet.Password, zfe.AesStrength != 0 ? zfe.AesStrength : (byte)3, totalEncryptedSize);
                                }
                                else
                                {
                                    byte verifier = hasDataDescriptor ? (byte)(lastModTime >> 8) : (byte)(zfe.Crc32 >> 24);
                                    long payloadLen = (long)zfe.CompressedSize - 12;
                                    payloadStream = new Encryption.ZipCryptoDecryptStream(zipStream, FastZipDotNet.Password, verifier, payloadLen);
                                }
                            }

                            using var dotnetInflate = new DeflateStream(payloadStream, CompressionMode.Decompress, leaveOpen: true);
                            crc32Calc = 0;

                            byte[] buffer = new byte[1 << 20];
                            ulong remaining = zfe.FileSize;

                            while (remaining > 0)
                            {
                                WaitWhilePaused();
                                int toRead = (int)Math.Min((ulong)buffer.Length, remaining);
                                int bytesRead = dotnetInflate.Read(buffer, 0, toRead);
                                if (bytesRead <= 0) break;

                                crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
                                outStream.Write(buffer, 0, bytesRead);
                                onBytes?.Invoke(bytesRead);

                                Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
                                Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);

                                remaining -= (ulong)bytesRead;
                            }
                            outStream.Flush();

                            return remaining == 0 && (zfe.IsAes || zfe.Crc32 == crc32Calc);
                        }
                        else
                        {
                            // libdeflate success; CRC already computed
                            return zfe.IsAes || zfe.Crc32 == crc32Calc;
                        }
                    }
                    else if (actualMethod == Compression.Zstd)
                    {
                        using var zstd = new DecompressionStream(payloadStream);
                        uint crc32Calc = 0;
                        byte[] buffer = new byte[1 << 20];
                        ulong remaining = zfe.FileSize;

                        while (remaining > 0)
                        {
                            WaitWhilePaused();
                            int toRead = (int)Math.Min((ulong)buffer.Length, remaining);
                            int bytesRead = zstd.Read(buffer, 0, toRead);
                            if (bytesRead <= 0) break;

                            crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
                            outStream.Write(buffer, 0, bytesRead);
                            onBytes?.Invoke(bytesRead);

                            Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
                            Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);

                            remaining -= (ulong)bytesRead;
                        }
                        outStream.Flush();

                        return remaining == 0 && (zfe.IsAes || zfe.Crc32 == crc32Calc);
                    }
                    else
                    {
                        return false;
                    }
                }
                finally
                {
                    if (payloadStream != null && payloadStream != zipStream)
                        payloadStream.Dispose();
                }
            }
            finally
            {
                zipStream?.Dispose();
            }
        }

        // Convenience overloads
        public bool ExtractFile(ZipFileEntry zfe, string outPathFilename)
            => ExtractFile(zfe, outPathFilename, onBytes: null);

        public bool ExtractFile(ZipFileEntry zfe, Stream outStream)
            => ExtractFile(zfe, outStream, onBytes: null);

        #endregion

        #region Libdeflate streaming inflate (raw DEFLATE + pause + AV guard)

        // We catch AccessViolationException here to avoid crashing the process
        // when the modified libdeflate function is called without a full DEFLATE
        // block in 'in_part'. If that happens, we return false so the caller can
        // fallback to DeflateStream.
        [HandleProcessCorruptedStateExceptions]
        private bool TryInflateWithLibdeflate(Stream compressedIn, ulong expectedUncompressedBytes, Stream outStream, Action<long> onBytes, out uint crc32Calc)
        {
            crc32Calc = 0;

            const int DictMax = 32 * 1024;
            int inChunk = Consts.ChunkSize;
            int outCap = Consts.ChunkSize;

            IntPtr decomp = IntPtr.Zero;
            try
            {
                decomp = LibDeflateWrapper.LibdeflateAllocDecompressor();
                if (decomp == IntPtr.Zero)
                    return false;

                LibDeflateWrapper.LibdeflateDeflateDecompressBlockReset(decomp);

                // compressed input buffer
                byte[] inBuf = new byte[Math.Max(inChunk * 2, DefaultBufferThreshold)];
                int inPos = 0;
                int inLen = 0;

                // dictionary buffer
                byte[] dictBuf = Array.Empty<byte>();
                int dictLen = 0;

                // output block = dict prefix + new output
                byte[] outBlock = new byte[DictMax + outCap];

                ulong totalOut = 0;
                bool finalBlock = false;

                // Seed input to at least 4MB or until EOF to reduce risk of calling without a full block
                while ((inLen - inPos) < DefaultBufferThreshold)
                {
                    WaitWhilePaused();
                    if (FillInput(compressedIn, ref inBuf, ref inPos, ref inLen) == 0) break;
                }

                while (!finalBlock)
                {
                    WaitWhilePaused();

                    // Ensure we have some input; if empty, try to read more
                    if (inLen - inPos == 0)
                    {
                        if (FillInput(compressedIn, ref inBuf, ref inPos, ref inLen) == 0)
                        {
                            // EOF on compressed input; accept only if we matched expected size (if known)
                            return expectedUncompressedBytes == 0 || totalOut == expectedUncompressedBytes;
                        }
                    }

                    // Prepare dictionary prefix
                    int prefixLen = dictLen;
                    int requiredOutSize = prefixLen + outCap;
                    if (outBlock.Length < requiredOutSize)
                        outBlock = new byte[requiredOutSize];
                    if (prefixLen > 0)
                        Buffer.BlockCopy(dictBuf, dictLen - prefixLen, outBlock, 0, prefixLen);

                    var pinIn = GCHandle.Alloc(inBuf, GCHandleType.Pinned);
                    var pinOut = GCHandle.Alloc(outBlock, GCHandleType.Pinned);

                    try
                    {
                        IntPtr ptrIn = pinIn.AddrOfPinnedObject() + inPos;
                        IntPtr ptrOut = pinOut.AddrOfPinnedObject();

                        ushort savedState = LibDeflateWrapper.LibdeflateDeflateDecompressGetState(decomp);

                        while (true)
                        {
                            WaitWhilePaused();

                            ulong actualIn, actualOut;
                            int isFinal;

                            LibDeflateWrapper.LibdeflateResult res;
                            try
                            {
                                // IMPORTANT: StopByAnyBlock — stop exactly at next block boundary
                                res = LibDeflateWrapper.LibdeflateDeflateDecompressBlock(
                                    decomp,
                                    ptrIn, (ulong)(inLen - inPos),
                                    ptrOut, (ulong)prefixLen, (ulong)outCap,
                                    out actualIn, out actualOut,
                                    LibDeflateWrapper.LibdeflateDecompressStopBy.StopByAnyBlock,
                                    out isFinal);
                            }
                            catch (AccessViolationException)
                            {
                                // Unsafe to continue with libdeflate; let caller fallback
                                return false;
                            }

                            if (res == LibDeflateWrapper.LibdeflateResult.Success)
                            {
                                if (actualOut > (ulong)outCap)
                                    return false;

                                int produced = checked((int)actualOut);

                                // Prevent overrun relative to expected size if known
                                if (expectedUncompressedBytes > 0 && totalOut + (ulong)produced > expectedUncompressedBytes)
                                    produced = (int)(expectedUncompressedBytes - totalOut);

                                if (produced > 0)
                                {
                                    crc32Calc = Crc32Algorithm.Append(crc32Calc, outBlock, prefixLen, produced);
                                    outStream.Write(outBlock, prefixLen, produced);
                                    onBytes?.Invoke(produced);

                                    Interlocked.Add(ref FastZipDotNet.BytesWritten, produced);
                                    Interlocked.Add(ref FastZipDotNet.BytesPerSecond, produced);
                                }

                                totalOut += (ulong)produced;
                                inPos += checked((int)actualIn);

                                // Update dictionary with last up to 32KB of (prefix+produced)
                                int newTotal = prefixLen + produced;
                                if (newTotal > 0)
                                {
                                    if (newTotal <= DictMax)
                                    {
                                        if (dictBuf.Length < newTotal) dictBuf = new byte[newTotal];
                                        Buffer.BlockCopy(outBlock, 0, dictBuf, 0, newTotal);
                                        dictLen = newTotal;
                                    }
                                    else
                                    {
                                        if (dictBuf.Length < DictMax) dictBuf = new byte[DictMax];
                                        Buffer.BlockCopy(outBlock, newTotal - DictMax, dictBuf, 0, DictMax);
                                        dictLen = DictMax;
                                    }
                                }

                                finalBlock = (isFinal != 0) || (expectedUncompressedBytes > 0 && totalOut >= expectedUncompressedBytes);
                                break;
                            }
                            else if (res == LibDeflateWrapper.LibdeflateResult.InsufficientSpace)
                            {
                                // Need more output capacity
                                LibDeflateWrapper.LibdeflateDeflateDecompressSetState(decomp, savedState);

                                int newCap = outCap < (1 << 26) ? outCap * 2 : outCap + (1 << 20);
                                if (newCap <= outCap) newCap = outCap + (1 << 20);
                                outCap = newCap;

                                requiredOutSize = prefixLen + outCap;

                                pinOut.Free(); // unpin before resize
                                if (outBlock.Length < requiredOutSize)
                                    outBlock = new byte[requiredOutSize];
                                pinOut = GCHandle.Alloc(outBlock, GCHandleType.Pinned);
                                ptrOut = pinOut.AddrOfPinnedObject();

                                // Re-copy dict prefix
                                if (prefixLen > 0)
                                    Buffer.BlockCopy(dictBuf, dictLen - prefixLen, outBlock, 0, prefixLen);

                                continue;
                            }
                            else if (res == LibDeflateWrapper.LibdeflateResult.BadData)
                            {
                                // Probably need more input to complete a full block
                                if (TryReadMore(compressedIn, ref inBuf, ref inPos, ref inLen) > 0)
                                {
                                    LibDeflateWrapper.LibdeflateDeflateDecompressSetState(decomp, savedState);
                                    ptrIn = pinIn.AddrOfPinnedObject() + inPos; // update pointer
                                    continue;
                                }
                                else
                                {
                                    // No more input -> fail
                                    return false;
                                }
                            }
                            else
                            {
                                // SHORT_OUTPUT or other unexpected
                                return false;
                            }
                        } // inner while
                    }
                    finally
                    {
                        if (pinIn.IsAllocated) pinIn.Free();
                        if (pinOut.IsAllocated) pinOut.Free();
                    }
                } // outer while

                outStream.Flush();

                return expectedUncompressedBytes == 0 || totalOut == expectedUncompressedBytes;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (decomp != IntPtr.Zero)
                    LibDeflateWrapper.LibdeflateFreeDecompressor(decomp);
            }
        }

        // Read fresh chunk into (inBuf). Compacts buffer if needed. Pause-aware.
        private int FillInput(Stream s, ref byte[] inBuf, ref int inPos, ref int inLen)
        {
            WaitWhilePaused();

            // compact if needed
            if (inPos > 0)
            {
                if (inPos < inLen)
                    Buffer.BlockCopy(inBuf, inPos, inBuf, 0, inLen - inPos);
                inLen -= inPos;
                inPos = 0;
            }

            int free = inBuf.Length - inLen;
            if (free < Consts.ChunkSize)
            {
                int newLen = Math.Max(inBuf.Length * 2, inBuf.Length + Consts.ChunkSize);
                Array.Resize(ref inBuf, newLen);
                free = newLen - inLen;
            }

            int r = s.Read(inBuf, inLen, Math.Min(Consts.ChunkSize, free));
            inLen += r;
            return r;
        }

        // Try read more; returns bytes read (0 on EOF). Pause-aware via FillInput.
        private int TryReadMore(Stream s, ref byte[] inBuf, ref int inPos, ref int inLen)
            => FillInput(s, ref inBuf, ref inPos, ref inLen);

        // Pause helper (same as writer)
        private void WaitWhilePaused()
        {
            while (FastZipDotNet.Pause)
                Thread.Sleep(50);
        }

        // Bounds the readable bytes of an underlying stream
        private sealed class BoundedReadStream : Stream
        {
            private readonly Stream _base;
            private long _remaining;

            public BoundedReadStream(Stream baseStream, long length)
            {
                _base = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
                if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
                _remaining = length;
            }

            public override bool CanRead => _base.CanRead;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public override void Flush() { }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (_remaining <= 0) return 0;
                int toRead = (int)Math.Min(_remaining, count);
                int r = _base.Read(buffer, offset, toRead);
                _remaining -= r;
                return r;
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        #endregion
    }
}



////using FastZipDotNet.MultiThreading;
////using FastZipDotNet.Zip.Cryptography;
////using FastZipDotNet.Zip.Encryption;
////using FastZipDotNet.Zip.Helpers;
////using FastZipDotNet.Zip.LibDeflate;
////using FastZipDotNet.Zip.ZStd;
////using System.Diagnostics;
////using System.IO.Compression;
////using System.Runtime.InteropServices;
////using System.Security.Cryptography;
////using System.Text;
////using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
////using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

////namespace FastZipDotNet.Zip.Readers
////{
////    public class ZipDataReader
////    {
////        public ZipDataReader(FastZipDotNet fastZipDotNet)
////        {
////            FastZipDotNet = fastZipDotNet;
////        }

////        private FastZipDotNet FastZipDotNet;

////        #region TestArchive

////        public async Task<bool> TestArchiveAsync(int threads, IProgress<ZipProgress> progress, CancellationToken ct = default)
////        {
////            try
////            {
////                await Task.Yield();

////                int totalFiles = FastZipDotNet.ZipFileEntries.Count;
////                long totalUncompressed = 0;
////                foreach (var e in FastZipDotNet.ZipFileEntries)
////                    totalUncompressed += (long)e.FileSize;

////                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount * 2 : threads));
////                var sem = FastZipDotNet.ConcurrencyLimiter;
////                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
////                var tasks = new List<Task>(totalFiles);

////                long processedUncompressed = 0;
////                int filesDone = 0;
////                string lastFile = null;

////                var sw = Stopwatch.StartNew();

////                // Dedicated reporter
////                using var reportCts = CancellationTokenSource.CreateLinkedTokenSource(linkedCts.Token);
////                var reportingTask = Task.Factory.StartNew(async () =>
////                {
////                    while (!reportCts.IsCancellationRequested)
////                    {
////                        var elapsed = sw.Elapsed;
////                        long unc = Math.Min(Interlocked.Read(ref processedUncompressed), totalUncompressed);
////                        int f = Volatile.Read(ref filesDone);
////                        double speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;
////                        double fps = elapsed.TotalSeconds > 0 ? f / elapsed.TotalSeconds : 0.0;

////                        progress?.Report(new ZipProgress
////                        {
////                            Operation = ZipOperation.Test,
////                            CurrentFile = Volatile.Read(ref lastFile),
////                            TotalFiles = totalFiles,
////                            TotalBytesUncompressed = totalUncompressed,
////                            FilesProcessed = f,
////                            BytesProcessedUncompressed = unc,
////                            Elapsed = elapsed,
////                            SpeedBytesPerSec = speed,
////                            FilesPerSec = fps
////                        });

////                        try { await Task.Delay(50, reportCts.Token).ConfigureAwait(false); }
////                        catch (OperationCanceledException) { break; }
////                    }
////                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

////                foreach (var entry in FastZipDotNet.ZipFileEntries)
////                {
////                    await sem.WaitAsync(linkedCts.Token).ConfigureAwait(false);

////                    var task = Task.Run(() =>
////                    {
////                        try
////                        {
////                            void OnBytes(long n)
////                            {
////                                Interlocked.Add(ref processedUncompressed, n);
////                                Volatile.Write(ref lastFile, entry.FilenameInZip);
////                            }

////                            using var sink = Stream.Null;
////                            bool ok = ExtractFile(entry, sink, OnBytes);
////                            if (!ok)
////                            {
////                                linkedCts.Cancel();
////                                throw new InvalidDataException($"CRC mismatch or read error: {entry.FilenameInZip}");
////                            }

////                            Interlocked.Increment(ref filesDone);
////                        }
////                        finally
////                        {
////                            sem.Release();
////                        }
////                    }, linkedCts.Token);

////                    tasks.Add(task);
////                }

////                await Task.WhenAll(tasks).ConfigureAwait(false);

////                // Final 100% update
////                {
////                    var elapsed = sw.Elapsed;
////                    long unc = totalUncompressed;
////                    double speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;
////                    double fps = elapsed.TotalSeconds > 0 ? totalFiles / elapsed.TotalSeconds : 0.0;

////                    progress?.Report(new ZipProgress
////                    {
////                        Operation = ZipOperation.Test,
////                        CurrentFile = null,
////                        TotalFiles = totalFiles,
////                        TotalBytesUncompressed = totalUncompressed,
////                        FilesProcessed = totalFiles,
////                        BytesProcessedUncompressed = unc,
////                        Elapsed = elapsed,
////                        SpeedBytesPerSec = speed,
////                        FilesPerSec = fps
////                    });
////                }

////                reportCts.Cancel();
////                try { await reportingTask.ConfigureAwait(false); } catch { }

////                return true;
////            }
////            catch (OperationCanceledException) { return false; }
////            catch { return false; }
////        }

////        public async Task<bool> TestArchiveAsync(int threads = 6)
////        {
////            try
////            {
////                var cts = new CancellationTokenSource();
////                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount * 2 : threads));
////                var semaphore = FastZipDotNet.ConcurrencyLimiter;
////                var tasks = new List<Task>();
////                foreach (var currentEntry in FastZipDotNet.ZipFileEntries)
////                {
////                    await Task.Run(() => semaphore.WaitOne(cts.Token));
////                    var entry = currentEntry;
////                    var task = Task.Run(() =>
////                    {
////                        try
////                        {
////                            if (!TestFile(entry))
////                            {
////                                cts.Cancel();
////                                return false;
////                            }
////                            return true;
////                        }
////                        finally
////                        {
////                            semaphore.Release();
////                        }
////                    }, cts.Token);
////                    tasks.Add(task);
////                }

////                var results = Task.WhenAll(tasks);
////                return results.IsCompletedSuccessfully;
////            }
////            catch (Exception ex)
////            {
////                throw new Exception(ex.Message + "\r\nIn BrutalUnZip.Test");
////            }
////        }

////        public bool TestFile(ZipFileEntry zfe)
////        {
////            try
////            {
////                if (!ExtractFile(zfe, Stream.Null))
////                {
////                    return false;
////                }
////                return true;
////            }
////            catch (Exception ex)
////            {
////                throw new Exception(ex.Message + "\r\nIn BrutalUnZip.TestFile");
////            }
////        }
////        #endregion

////        #region Extraction

////        public async Task<bool> ExtractArchiveAsync(string outputDirectory, int threads = 6, IProgress<ZipProgress> progress = null, CancellationToken ct = default)
////        {
////            try
////            {
////                if (!Directory.Exists(outputDirectory))
////                    Directory.CreateDirectory(outputDirectory);

////                int totalFiles = FastZipDotNet.ZipFileEntries.Count;
////                long totalUncompressed = 0;
////                long totalCompressed = 0;
////                foreach (var e in FastZipDotNet.ZipFileEntries)
////                {
////                    totalUncompressed += (long)e.FileSize;
////                    totalCompressed += (long)e.CompressedSize;
////                }

////                long processedUncompressed = 0;
////                long processedCompressedApprox = 0;
////                int filesDone = 0;
////                string lastFileName = null;

////                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount * 2 : threads));
////                var semaphore = FastZipDotNet.ConcurrencyLimiter;

////                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
////                var tasks = new List<Task>(totalFiles);
////                var sw = Stopwatch.StartNew();

////                CancellationTokenSource reportCts = null;
////                Task reportingTask = Task.CompletedTask;

////                if (progress != null)
////                {
////                    reportCts = CancellationTokenSource.CreateLinkedTokenSource(linkedCts.Token);
////                    reportingTask = Task.Run(async () =>
////                    {
////                        while (!reportCts.IsCancellationRequested)
////                        {
////                            var elapsed = sw.Elapsed;
////                            var bytes = Interlocked.Read(ref processedUncompressed);
////                            var compBytes = Interlocked.Read(ref processedCompressedApprox);
////                            var files = Volatile.Read(ref filesDone);

////                            var speed = elapsed.TotalSeconds > 0 ? bytes / elapsed.TotalSeconds : 0.0;
////                            var fps = elapsed.TotalSeconds > 0 ? files / elapsed.TotalSeconds : 0.0;

////                            progress.Report(new ZipProgress
////                            {
////                                Operation = ZipOperation.Extract,
////                                CurrentFile = Volatile.Read(ref lastFileName),

////                                TotalFiles = totalFiles,
////                                TotalBytesUncompressed = totalUncompressed,
////                                TotalBytesCompressed = totalCompressed,

////                                FilesProcessed = files,
////                                BytesProcessedUncompressed = bytes,
////                                BytesProcessedCompressed = compBytes,

////                                Elapsed = elapsed,
////                                SpeedBytesPerSec = speed,
////                                FilesPerSec = fps
////                            });

////                            try { await Task.Delay(50, reportCts.Token).ConfigureAwait(false); }
////                            catch (OperationCanceledException) { break; }
////                        }
////                    }, reportCts.Token);
////                }

////                foreach (var currentEntry in FastZipDotNet.ZipFileEntries)
////                {
////                    semaphore.WaitOne(linkedCts.Token);
////                    var entry = currentEntry;

////                    var task = Task.Run(() =>
////                    {
////                        try
////                        {
////                            string outPath = Path.Combine(outputDirectory, entry.FilenameInZip);

////                            double compPerUnc = (entry.FileSize > 0) ? (double)entry.CompressedSize / (double)entry.FileSize : 0.0;

////                            void OnBytes(long count)
////                            {
////                                Interlocked.Add(ref processedUncompressed, count);
////                                if (compPerUnc > 0)
////                                {
////                                    long approxComp = (long)Math.Round(count * compPerUnc);
////                                    Interlocked.Add(ref processedCompressedApprox, approxComp);
////                                }
////                                Volatile.Write(ref lastFileName, entry.FilenameInZip);
////                            }

////                            bool ok = ExtractFile(entry, outPath, OnBytes);
////                            if (!ok)
////                            {
////                                linkedCts.Cancel();
////                                throw new Exception($"Extraction failed for: {entry.FilenameInZip}");
////                            }

////                            Interlocked.Increment(ref filesDone);
////                        }
////                        finally
////                        {
////                            semaphore.Release();
////                        }
////                    }, linkedCts.Token);

////                    tasks.Add(task);
////                }

////                await Task.WhenAll(tasks).ConfigureAwait(false);

////                if (progress != null)
////                {
////                    var elapsed = sw.Elapsed;
////                    var speed = elapsed.TotalSeconds > 0 ? totalUncompressed / elapsed.TotalSeconds : 0.0;
////                    var fps = elapsed.TotalSeconds > 0 ? totalFiles / elapsed.TotalSeconds : 0.0;

////                    progress.Report(new ZipProgress
////                    {
////                        Operation = ZipOperation.Extract,
////                        CurrentFile = null,

////                        TotalFiles = totalFiles,
////                        TotalBytesUncompressed = totalUncompressed,
////                        TotalBytesCompressed = totalCompressed,

////                        FilesProcessed = totalFiles,
////                        BytesProcessedUncompressed = totalUncompressed,
////                        BytesProcessedCompressed = totalCompressed,

////                        Elapsed = elapsed,
////                        SpeedBytesPerSec = speed,
////                        FilesPerSec = fps
////                    });
////                }

////                reportCts?.Cancel();
////                try { await reportingTask.ConfigureAwait(false); } catch { /* ignore */ }

////                return true;
////            }
////            catch (OperationCanceledException)
////            {
////                return false;
////            }
////            catch
////            {
////                return false;
////            }
////        }

////        // Extract to path (reports uncompressed bytes via onBytes)
////        public bool ExtractFile(ZipFileEntry zfe, string outPathFilename, Action<long> onBytes = null)
////        {
////            Stream output = null;
////            try
////            {
////                string? dir = Path.GetDirectoryName(outPathFilename);
////                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
////                    Directory.CreateDirectory(dir);

////                // Directory entry?
////                if (zfe.FilenameInZip.EndsWith("/") || zfe.FilenameInZip.EndsWith("\\"))
////                {
////                    Directory.CreateDirectory(outPathFilename);
////                    return true;
////                }

////                if (File.Exists(outPathFilename))
////                {
////                    try { File.Delete(outPathFilename); }
////                    catch { throw new InvalidOperationException($"File '{outPathFilename}' cannot be written"); }
////                }

////                while (FastZipDotNet.Pause) Thread.Sleep(50);

////                output = new FileStream(outPathFilename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete, 1 << 20, FileOptions.SequentialScan);

////                bool ok = ExtractFile(zfe, output, onBytes);
////                output.Close();

////                if (ok)
////                {
////                    try
////                    {
////                        File.SetCreationTime(outPathFilename, zfe.ModifyTime);
////                        File.SetLastWriteTime(outPathFilename, zfe.ModifyTime);
////                    }
////                    catch { /* ignore */ }
////                }

////                return ok;
////            }
////            finally
////            {
////                output?.Dispose();
////            }
////        }

////        // Extract to an existing stream (reports uncompressed bytes via onBytes)
////        public bool ExtractFile(ZipFileEntry zfe, Stream outStream, Action<long> onBytes = null)
////        {
////            if (outStream is null) throw new ArgumentNullException(nameof(outStream));
////            if (!outStream.CanWrite) throw new InvalidOperationException("Stream cannot be written");

////            FileStream zipStream = null;
////            try
////            {
////                zipStream = new FileStream(
////                    FastZipDotNet.ZipFileName,
////                    FileMode.Open,
////                    FileAccess.Read,
////                    FileShare.ReadWrite,
////                    1 << 20,
////                    FileOptions.RandomAccess);

////                zipStream.Seek((long)zfe.HeaderOffset, SeekOrigin.Begin);
////                using var br = new BinaryReader(zipStream, Encoding.Default, leaveOpen: true);

////                uint signature = br.ReadUInt32();
////                if (signature != 0x04034b50) return false;

////                ushort versionNeeded = br.ReadUInt16();
////                ushort generalPurposeBitFlag = br.ReadUInt16();
////                ushort compressionMethodLocal = br.ReadUInt16(); // may be 99 for AES
////                ushort lastModTime = br.ReadUInt16();
////                ushort lastModDate = br.ReadUInt16();
////                uint crc32Local = br.ReadUInt32();
////                uint compressedSizeLocal = br.ReadUInt32();
////                uint uncompressedSizeLocal = br.ReadUInt32();
////                ushort fileNameLength = br.ReadUInt16();
////                ushort extraFieldLength = br.ReadUInt16();

////                if (fileNameLength > 0) br.ReadBytes(fileNameLength);
////                if (extraFieldLength > 0) br.ReadBytes(extraFieldLength);

////                bool hasDataDescriptor = (generalPurposeBitFlag & 0x0008) != 0;
////                bool isEncrypted = (generalPurposeBitFlag & 0x0001) != 0;

////                Compression actualMethod = zfe.IsAes ? zfe.Method : (Compression)compressionMethodLocal;

////                // Build an input stream that is bounded to this entry's compressed payload
////                Stream payloadStream = null;
////                long dataStartOffset = zipStream.Position; // start of compressed payload in the ZIP
////                try
////                {
////                    if (isEncrypted)
////                    {
////                        if (zfe.IsAes)
////                        {
////                            if (string.IsNullOrEmpty(FastZipDotNet.Password))
////                                throw new CryptographicException("Password required for AES-encrypted entry.");

////                            long totalEncryptedSize = (long)zfe.CompressedSize;
////                            var aes = new AesDecryptStream(zipStream, FastZipDotNet.Password, zfe.AesStrength != 0 ? zfe.AesStrength : (byte)3, totalEncryptedSize);

////                            // The AES decrypt stream itself is bounded (it will stop after payload)
////                            payloadStream = aes;
////                        }
////                        else
////                        {
////                            if (string.IsNullOrEmpty(FastZipDotNet.Password))
////                                throw new CryptographicException("Password required for encrypted ZIP entry.");

////                            byte verifier = hasDataDescriptor ? (byte)(lastModTime >> 8) : (byte)(zfe.Crc32 >> 24);

////                            long payloadLen = (long)zfe.CompressedSize - 12;
////                            if (payloadLen < 0) throw new InvalidDataException("Invalid encrypted size");

////                            var decrypt = new Encryption.ZipCryptoDecryptStream(zipStream, FastZipDotNet.Password, verifier, payloadLen);
////                            payloadStream = decrypt;
////                        }
////                    }
////                    else
////                    {
////                        // For unencrypted members, limit reads to exactly CompressedSize
////                        payloadStream = new BoundedReadStream(zipStream, (long)zfe.CompressedSize);
////                    }

////                    // Decompress or copy
////                    if (actualMethod == Compression.Store)
////                    {
////                        // Just copy; report onBytes
////                        uint crc32Calc = 0;
////                        byte[] buffer = new byte[1 << 20];
////                        ulong remaining = zfe.FileSize;

////                        while (remaining > 0)
////                        {
////                            while (FastZipDotNet.Pause) Thread.Sleep(50);
////                            int toRead = (int)Math.Min((ulong)buffer.Length, remaining);
////                            int bytesRead = payloadStream.Read(buffer, 0, toRead);
////                            if (bytesRead <= 0) break;

////                            crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
////                            outStream.Write(buffer, 0, bytesRead);
////                            onBytes?.Invoke(bytesRead);

////                            Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
////                            Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);

////                            remaining -= (ulong)bytesRead;
////                        }
////                        outStream.Flush();

////                        return remaining == 0 && (zfe.IsAes || zfe.Crc32 == crc32Calc);
////                    }
////                    else if (actualMethod == Compression.Deflate)
////                    {
////                        // First try libdeflate streaming (fast). Fall back to .NET DeflateStream if needed.
////                        bool usedLibDeflate = TryInflateWithLibdeflate(payloadStream, zfe.FileSize, outStream, onBytes, out uint crc32Calc);
////                        if (!usedLibDeflate)
////                        {
////                            // Fallback path: rebuild bounded payload stream (reset underlying stream)
////                            if (!isEncrypted)
////                            {
////                                zipStream.Seek(dataStartOffset, SeekOrigin.Begin);
////                                payloadStream.Dispose();
////                                payloadStream = new BoundedReadStream(zipStream, (long)zfe.CompressedSize);
////                            }
////                            else
////                            {
////                                // Recreate decrypt stream because we've consumed bytes already
////                                payloadStream.Dispose();
////                                if (zfe.IsAes)
////                                {
////                                    long totalEncryptedSize = (long)zfe.CompressedSize;
////                                    payloadStream = new AesDecryptStream(zipStream, FastZipDotNet.Password, zfe.AesStrength != 0 ? zfe.AesStrength : (byte)3, totalEncryptedSize);
////                                }
////                                else
////                                {
////                                    byte verifier = hasDataDescriptor ? (byte)(lastModTime >> 8) : (byte)(zfe.Crc32 >> 24);
////                                    long payloadLen = (long)zfe.CompressedSize - 12;
////                                    payloadStream = new Encryption.ZipCryptoDecryptStream(zipStream, FastZipDotNet.Password, verifier, payloadLen);
////                                }
////                            }

////                            using var dotnetInflate = new DeflateStream(payloadStream, CompressionMode.Decompress, leaveOpen: true);
////                            crc32Calc = 0;

////                            byte[] buffer = new byte[1 << 20];
////                            ulong remaining = zfe.FileSize;

////                            while (remaining > 0)
////                            {
////                                while (FastZipDotNet.Pause) Thread.Sleep(50);
////                                int toRead = (int)Math.Min((ulong)buffer.Length, remaining);
////                                int bytesRead = dotnetInflate.Read(buffer, 0, toRead);
////                                if (bytesRead <= 0) break;

////                                crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
////                                outStream.Write(buffer, 0, bytesRead);
////                                onBytes?.Invoke(bytesRead);

////                                Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
////                                Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);

////                                remaining -= (ulong)bytesRead;
////                            }
////                            outStream.Flush();

////                            return remaining == 0 && (zfe.IsAes || zfe.Crc32 == crc32Calc);
////                        }
////                        else
////                        {
////                            // libdeflate success; CRC already computed
////                            return zfe.IsAes || zfe.Crc32 == crc32Calc;
////                        }
////                    }
////                    else if (actualMethod == Compression.Zstd)
////                    {
////                        using var zstd = new DecompressionStream(payloadStream);
////                        uint crc32Calc = 0;
////                        byte[] buffer = new byte[1 << 20];
////                        ulong remaining = zfe.FileSize;

////                        while (remaining > 0)
////                        {
////                            while (FastZipDotNet.Pause) Thread.Sleep(50);
////                            int toRead = (int)Math.Min((ulong)buffer.Length, remaining);
////                            int bytesRead = zstd.Read(buffer, 0, toRead);
////                            if (bytesRead <= 0) break;

////                            crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
////                            outStream.Write(buffer, 0, bytesRead);
////                            onBytes?.Invoke(bytesRead);

////                            Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
////                            Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);

////                            remaining -= (ulong)bytesRead;
////                        }
////                        outStream.Flush();

////                        return remaining == 0 && (zfe.IsAes || zfe.Crc32 == crc32Calc);
////                    }
////                    else
////                    {
////                        // Unknown method
////                        return false;
////                    }
////                }
////                finally
////                {
////                    if (payloadStream != null && payloadStream != zipStream)
////                        payloadStream.Dispose();
////                }
////            }
////            finally
////            {
////                zipStream?.Dispose();
////            }
////        }

////        // Keep convenience overloads
////        public bool ExtractFile(ZipFileEntry zfe, string outPathFilename)
////            => ExtractFile(zfe, outPathFilename, onBytes: null);

////        public bool ExtractFile(ZipFileEntry zfe, Stream outStream)
////            => ExtractFile(zfe, outStream, onBytes: null);

////        #endregion

////        #region Libdeflate streaming inflate (raw DEFLATE)

////        // Try to decompress raw DEFLATE from 'compressedIn' to 'outStream' using the libdeflate block API.
////        // Returns true on success, and outputs crc32 of uncompressed data in crc32Calc.
////        // On failure (e.g., insufficient input semantics, or unexpected errors), returns false (caller should fallback).
////        private bool TryInflateWithLibdeflate(Stream compressedIn, ulong expectedUncompressedBytes, Stream outStream, Action<long> onBytes, out uint crc32Calc)
////        {
////            crc32Calc = 0;

////            const int DictMax = 32 * 1024;
////            int inChunk = Consts.ChunkSize;       // input chunk size (compressed)
////            int outCap = Consts.ChunkSize;        // starting output capacity per call (uncompressed)

////            IntPtr decomp = IntPtr.Zero;
////            try
////            {
////                decomp = LibDeflateWrapper.LibdeflateAllocDecompressor();
////                if (decomp == IntPtr.Zero)
////                    return false;

////                LibDeflateWrapper.LibdeflateDeflateDecompressBlockReset(decomp);

////                // input buffer
////                byte[] inBuf = new byte[inChunk * 2];
////                int inPos = 0;   // read pointer
////                int inLen = 0;   // data length in buffer

////                // dictionary buffer (last up to 32KB of output)
////                byte[] dictBuf = Array.Empty<byte>();
////                int dictLen = 0;

////                // current output block buffer (prefix dict + new output)
////                byte[] outBlock = new byte[DictMax + outCap];

////                ulong totalOut = 0;
////                bool finalBlock = false;

////                while (!finalBlock)
////                {
////                    // If we have no input left, read more
////                    if (inLen - inPos == 0)
////                    {
////                        int r = FillInput(compressedIn, ref inBuf, ref inPos, ref inLen);
////                        if (r == 0 && (inLen - inPos) == 0)
////                        {
////                            // No more input; if we've reached expected size, accept; else fail
////                            return totalOut == expectedUncompressedBytes;
////                        }
////                    }

////                    // Prepare out buffer with dictionary prefix
////                    int prefixLen = dictLen;
////                    int requiredOutSize = prefixLen + outCap;
////                    if (outBlock.Length < requiredOutSize)
////                        outBlock = new byte[requiredOutSize];
////                    if (prefixLen > 0)
////                        Buffer.BlockCopy(dictBuf, dictLen - prefixLen, outBlock, 0, prefixLen);

////                    // Pin buffers
////                    var pinIn = GCHandle.Alloc(inBuf, GCHandleType.Pinned);
////                    var pinOut = GCHandle.Alloc(outBlock, GCHandleType.Pinned);

////                    try
////                    {
////                        IntPtr ptrIn = pinIn.AddrOfPinnedObject() + inPos;
////                        IntPtr ptrOut = pinOut.AddrOfPinnedObject();

////                        // Save state before attempt (so we can retry with larger outCap if needed)
////                        ushort savedState = LibDeflateWrapper.LibdeflateDeflateDecompressGetState(decomp);

////                        // Try once; if INSUFFICIENT_SPACE, grow outCap and retry
////                        while (true)
////                        {
////                            ulong actualIn, actualOut;
////                            int isFinal;

////                            var res = LibDeflateWrapper.LibdeflateDeflateDecompressBlock(
////                                decomp,
////                                ptrIn, (ulong)(inLen - inPos),
////                                ptrOut, (ulong)prefixLen, (ulong)outCap,
////                                out actualIn, out actualOut,
////                                LibDeflateWrapper.LibdeflateDecompressStopBy.StopByAnyBlockAndFullInput,
////                                out isFinal);

////                            if (res == LibDeflateWrapper.LibdeflateResult.Success)
////                            {
////                                // We produced 'actualOut' bytes AFTER the prefix
////                                if (actualOut > (ulong)outCap)
////                                    return false; // shouldn't happen

////                                int produced = checked((int)actualOut);

////                                // Clamp to expected final size if needed
////                                if (expectedUncompressedBytes > 0 && totalOut + (ulong)produced > expectedUncompressedBytes)
////                                    produced = (int)(expectedUncompressedBytes - totalOut);

////                                if (produced > 0)
////                                {
////                                    // Update CRC/write/progress
////                                    crc32Calc = Crc32Algorithm.Append(crc32Calc, outBlock, prefixLen, produced);
////                                    outStream.Write(outBlock, prefixLen, produced);
////                                    onBytes?.Invoke(produced);

////                                    Interlocked.Add(ref FastZipDotNet.BytesWritten, produced);
////                                    Interlocked.Add(ref FastZipDotNet.BytesPerSecond, produced);
////                                }

////                                totalOut += (ulong)produced;

////                                // Advance input
////                                inPos += checked((int)actualIn);

////                                // Update dictionary to last up to 32KB bytes of (dict+new)
////                                int newTotal = prefixLen + produced;
////                                if (newTotal > 0)
////                                {
////                                    if (newTotal <= DictMax)
////                                    {
////                                        // Simplest: keep exactly (prefix+produced)
////                                        if (dictBuf.Length < newTotal) dictBuf = new byte[newTotal];
////                                        Buffer.BlockCopy(outBlock, 0, dictBuf, 0, newTotal);
////                                        dictLen = newTotal;
////                                    }
////                                    else
////                                    {
////                                        // Keep last 32KB
////                                        if (dictBuf.Length < DictMax) dictBuf = new byte[DictMax];
////                                        Buffer.BlockCopy(outBlock, newTotal - DictMax, dictBuf, 0, DictMax);
////                                        dictLen = DictMax;
////                                    }
////                                }

////                                finalBlock = (isFinal != 0) || (expectedUncompressedBytes > 0 && totalOut >= expectedUncompressedBytes);
////                                break;
////                            }
////                            else if (res == LibDeflateWrapper.LibdeflateResult.InsufficientSpace)
////                            {
////                                // Need larger outCap; restore state and grow
////                                LibDeflateWrapper.LibdeflateDeflateDecompressSetState(decomp, savedState);

////                                // grow outCap
////                                int newCap = outCap < (1 << 26) ? outCap * 2 : outCap + (1 << 20); // up to ~64MB, then +1MB steps
////                                if (newCap <= outCap) newCap = outCap + (1 << 20);
////                                outCap = newCap;

////                                // Resize outBlock
////                                requiredOutSize = prefixLen + outCap;
////                                pinOut.Free(); // unpin before replacing
////                                if (outBlock.Length < requiredOutSize)
////                                    outBlock = new byte[requiredOutSize];
////                                pinOut = GCHandle.Alloc(outBlock, GCHandleType.Pinned);
////                                ptrOut = pinOut.AddrOfPinnedObject();

////                                // Re-copy dict prefix
////                                if (prefixLen > 0)
////                                    Buffer.BlockCopy(dictBuf, dictLen - prefixLen, outBlock, 0, prefixLen);

////                                continue; // retry with bigger outCap
////                            }
////                            else if (res == LibDeflateWrapper.LibdeflateResult.BadData)
////                            {
////                                // Possibly need more input. If we can read more, do so and retry from saved state.
////                                if (TryReadMore(compressedIn, ref inBuf, ref inPos, ref inLen) > 0)
////                                {
////                                    LibDeflateWrapper.LibdeflateDeflateDecompressSetState(decomp, savedState);
////                                    ptrIn = pinIn.AddrOfPinnedObject() + inPos; // update pointer due to potential array replacement
////                                    continue; // retry
////                                }
////                                else
////                                {
////                                    // No more input -> truly bad/corrupt
////                                    return false;
////                                }
////                            }
////                            else
////                            {
////                                // SHORT_OUTPUT not expected here; treat as failure
////                                return false;
////                            }
////                        } // while retry
////                    }
////                    finally
////                    {
////                        if (pinIn.IsAllocated) pinIn.Free();
////                        if (pinOut.IsAllocated) pinOut.Free();
////                    }
////                } // while

////                outStream.Flush();

////                // If expected size known, verify it matches
////                return expectedUncompressedBytes == 0 || totalOut == expectedUncompressedBytes;
////            }
////            catch
////            {
////                return false;
////            }
////            finally
////            {
////                if (decomp != IntPtr.Zero)
////                    LibDeflateWrapper.LibdeflateFreeDecompressor(decomp);
////            }
////        }

////        // Read fresh chunk into (inBuf). Compacts buffer if needed.
////        private static int FillInput(Stream s, ref byte[] inBuf, ref int inPos, ref int inLen)
////        {
////            // compact if needed
////            if (inPos > 0)
////            {
////                if (inPos < inLen)
////                    Buffer.BlockCopy(inBuf, inPos, inBuf, 0, inLen - inPos);
////                inLen -= inPos;
////                inPos = 0;
////            }

////            int free = inBuf.Length - inLen;
////            if (free < Consts.ChunkSize)
////            {
////                // grow
////                int newLen = Math.Max(inBuf.Length * 2, inBuf.Length + Consts.ChunkSize);
////                Array.Resize(ref inBuf, newLen);
////                free = newLen - inLen;
////            }

////            int r = s.Read(inBuf, inLen, Math.Min(Consts.ChunkSize, free));
////            inLen += r;
////            return r;
////        }

////        // Try read more; returns bytes read (0 on EOF)
////        private static int TryReadMore(Stream s, ref byte[] inBuf, ref int inPos, ref int inLen)
////            => FillInput(s, ref inBuf, ref inPos, ref inLen);

////        // A simple stream wrapper that prevents reads beyond a fixed length from the underlying stream
////        private sealed class BoundedReadStream : Stream
////        {
////            private readonly Stream _base;
////            private long _remaining;

////            public BoundedReadStream(Stream baseStream, long length)
////            {
////                _base = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
////                if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
////                _remaining = length;
////            }

////            public override bool CanRead => _base.CanRead;
////            public override bool CanSeek => false;
////            public override bool CanWrite => false;
////            public override long Length => throw new NotSupportedException();
////            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

////            public override void Flush() { }

////            public override int Read(byte[] buffer, int offset, int count)
////            {
////                if (_remaining <= 0) return 0;
////                int toRead = (int)Math.Min(_remaining, count);
////                int r = _base.Read(buffer, offset, toRead);
////                _remaining -= r;
////                return r;
////            }

////            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
////            public override void SetLength(long value) => throw new NotSupportedException();
////            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
////        }

////        #endregion
////    }
////}




//LEGACY

////using FastZipDotNet.MultiThreading;
////using FastZipDotNet.Zip.Cryptography;
////using FastZipDotNet.Zip.Encryption;
////using FastZipDotNet.Zip.Helpers;
////using FastZipDotNet.Zip.ZStd;
////using System.Diagnostics;
////using System.IO.Compression;
////using System.Security.Cryptography;
////using System.Text;
////using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
////using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

////namespace FastZipDotNet.Zip.Readers
////{
////    public class ZipDataReader
////    {
////        public ZipDataReader(FastZipDotNet fastZipDotNet)
////        {
////            FastZipDotNet = fastZipDotNet;
////        }

////        FastZipDotNet FastZipDotNet;


////        #region TestArchive

////        public async Task<bool> TestArchiveAsync(int threads, IProgress<ZipProgress> progress, CancellationToken ct = default)
////        {
////            try
////            {
////                await Task.Yield();

////                int totalFiles = FastZipDotNet.ZipFileEntries.Count;
////                long totalUncompressed = 0;
////                foreach (var e in FastZipDotNet.ZipFileEntries)
////                    totalUncompressed += (long)e.FileSize;

////                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount * 2 : threads));
////                var sem = FastZipDotNet.ConcurrencyLimiter;
////                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
////                var tasks = new List<Task>(totalFiles);

////                long processedUncompressed = 0;
////                int filesDone = 0;
////                string lastFile = null;

////                var sw = Stopwatch.StartNew();

////                // Dedicated reporter
////                using var reportCts = CancellationTokenSource.CreateLinkedTokenSource(linkedCts.Token);
////                var reportingTask = Task.Factory.StartNew(async () =>
////                {
////                    while (!reportCts.IsCancellationRequested)
////                    {
////                        var elapsed = sw.Elapsed;
////                        long unc = Math.Min(Interlocked.Read(ref processedUncompressed), totalUncompressed);
////                        int f = Volatile.Read(ref filesDone);
////                        double speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;
////                        double fps = elapsed.TotalSeconds > 0 ? f / elapsed.TotalSeconds : 0.0;

////                        progress?.Report(new ZipProgress
////                        {
////                            Operation = ZipOperation.Test,
////                            CurrentFile = Volatile.Read(ref lastFile),
////                            TotalFiles = totalFiles,
////                            TotalBytesUncompressed = totalUncompressed,
////                            FilesProcessed = f,
////                            BytesProcessedUncompressed = unc,
////                            Elapsed = elapsed,
////                            SpeedBytesPerSec = speed,
////                            FilesPerSec = fps
////                        });

////                        try { await Task.Delay(50, reportCts.Token).ConfigureAwait(false); }
////                        catch (OperationCanceledException) { break; }
////                    }
////                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

////                foreach (var entry in FastZipDotNet.ZipFileEntries)
////                {
////                    await sem.WaitAsync(linkedCts.Token).ConfigureAwait(false);

////                    var task = Task.Run(() =>
////                    {
////                        try
////                        {
////                            void OnBytes(long n)
////                            {
////                                Interlocked.Add(ref processedUncompressed, n);
////                                Volatile.Write(ref lastFile, entry.FilenameInZip);
////                            }

////                            // Test by fully reading/decompressing, but writing to Null
////                            using var sink = Stream.Null;
////                            bool ok = ExtractFile(entry, sink, OnBytes);
////                            if (!ok)
////                            {
////                                linkedCts.Cancel();
////                                throw new InvalidDataException($"CRC mismatch or read error: {entry.FilenameInZip}");
////                            }

////                            Interlocked.Increment(ref filesDone);
////                        }
////                        finally
////                        {
////                            sem.Release();
////                        }
////                    }, linkedCts.Token);

////                    tasks.Add(task);
////                }

////                await Task.WhenAll(tasks).ConfigureAwait(false);

////                // Final 100% update
////                {
////                    var elapsed = sw.Elapsed;
////                    long unc = totalUncompressed;
////                    double speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;
////                    double fps = elapsed.TotalSeconds > 0 ? totalFiles / elapsed.TotalSeconds : 0.0;

////                    progress?.Report(new ZipProgress
////                    {
////                        Operation = ZipOperation.Test,
////                        CurrentFile = null,
////                        TotalFiles = totalFiles,
////                        TotalBytesUncompressed = totalUncompressed,
////                        FilesProcessed = totalFiles,
////                        BytesProcessedUncompressed = unc,
////                        Elapsed = elapsed,
////                        SpeedBytesPerSec = speed,
////                        FilesPerSec = fps
////                    });
////                }

////                reportCts.Cancel();
////                try { await reportingTask.ConfigureAwait(false); } catch { }

////                return true;
////            }
////            catch (OperationCanceledException) { return false; }
////            catch { return false; }
////        }


////        public async Task<bool> TestArchiveAsync(int threads = 6)
////        {
////            try
////            {
////                var cts = new CancellationTokenSource();
////                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount * 2 : threads));
////                var semaphore = FastZipDotNet.ConcurrencyLimiter;
////                //var semaphore = new AdjustableSemaphore(threads);
////                var tasks = new List<Task>();
////                bool success = true;
////                foreach (var currentEntry in FastZipDotNet.ZipFileEntries)
////                {
////                    // Wait for semaphore with cancellation token support
////                    await Task.Run(() => semaphore.WaitOne(cts.Token));

////                    var task = Task.Run(() =>
////                    {
////                        try
////                        {
////                            if (!TestFile(currentEntry))
////                            {
////                                cts.Cancel();
////                                return false;
////                            }
////                            return true;
////                        }
////                        finally
////                        {
////                            semaphore.Release();
////                        }
////                    }, cts.Token);
////                    tasks.Add(task);
////                }

////                // Wait for all the tasks to complete
////                var results = Task.WhenAll(tasks);

////                // Check if any of the results are false
////                return results.IsCompletedSuccessfully;

////            }
////            catch (Exception ex)
////            {
////                throw new Exception(ex.Message + "\r\nIn BrutalUnZip.Test");
////            }
////        }

////        /// <summary>
////        /// Test the contents of a stored file
////        /// </summary>
////        /// <param name="zfe">Entry information of file to test</param>
////        /// <returns>True if success, false if not.</returns>
////        public bool TestFile(ZipFileEntry zfe)
////        {
////            try
////            {
////                if (!ExtractFile(zfe, Stream.Null))
////                {
////                    return false;
////                }
////                return true;
////            }
////            catch (Exception ex)
////            {
////                throw new Exception(ex.Message + "\r\nIn BrutalUnZip.TestFile");
////            }
////        }
////        #endregion

////        #region Extraction
////        public async Task<bool> ExtractArchiveAsync(string outputDirectory, int threads = 6, IProgress<ZipProgress> progress = null, CancellationToken ct = default)
////        {
////            try
////            {
////                if (!Directory.Exists(outputDirectory))
////                    Directory.CreateDirectory(outputDirectory);

////                // Totals from central directory
////                int totalFiles = FastZipDotNet.ZipFileEntries.Count;
////                long totalUncompressed = 0;
////                long totalCompressed = 0;
////                foreach (var e in FastZipDotNet.ZipFileEntries)
////                {
////                    totalUncompressed += (long)e.FileSize;
////                    totalCompressed += (long)e.CompressedSize;
////                }

////                // Shared counters
////                long processedUncompressed = 0;             // aggregate bytes extracted (uncompressed)
////                long processedCompressedApprox = 0;         // approximate compressed bytes consumed
////                int filesDone = 0;

////                // Name of last file that reported progress
////                string lastFileName = null;

////                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount * 2 : threads));
////                var semaphore = FastZipDotNet.ConcurrencyLimiter;

////                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
////                //var semaphore = new AdjustableSemaphore(threads);
////                var tasks = new List<Task>(totalFiles);
////                var sw = Stopwatch.StartNew();

////                // Periodic reporter
////                CancellationTokenSource reportCts = null;
////                Task reportingTask = Task.CompletedTask;

////                if (progress != null)
////                {
////                    reportCts = CancellationTokenSource.CreateLinkedTokenSource(linkedCts.Token);
////                    reportingTask = Task.Run(async () =>
////                    {
////                        while (!reportCts.IsCancellationRequested)
////                        {
////                            var elapsed = sw.Elapsed;
////                            var bytes = Interlocked.Read(ref processedUncompressed);
////                            var compBytes = Interlocked.Read(ref processedCompressedApprox);
////                            var files = Volatile.Read(ref filesDone);

////                            var speed = elapsed.TotalSeconds > 0 ? bytes / elapsed.TotalSeconds : 0.0;
////                            var fps = elapsed.TotalSeconds > 0 ? files / elapsed.TotalSeconds : 0.0;

////                            progress.Report(new ZipProgress
////                            {
////                                Operation = ZipOperation.Extract,
////                                CurrentFile = Volatile.Read(ref lastFileName),

////                                TotalFiles = totalFiles,
////                                TotalBytesUncompressed = totalUncompressed,
////                                TotalBytesCompressed = totalCompressed,

////                                FilesProcessed = files,
////                                BytesProcessedUncompressed = bytes,
////                                BytesProcessedCompressed = compBytes,

////                                Elapsed = elapsed,
////                                SpeedBytesPerSec = speed,
////                                FilesPerSec = fps
////                            });

////                            try { await Task.Delay(50, reportCts.Token).ConfigureAwait(false); }
////                            catch (OperationCanceledException) { break; }
////                        }
////                    }, reportCts.Token);
////                }

////                foreach (var currentEntry in FastZipDotNet.ZipFileEntries)
////                {
////                    semaphore.WaitOne(linkedCts.Token);
////                    var entry = currentEntry; // capture

////                    var task = Task.Run(() =>
////                    {
////                        try
////                        {
////                            string outPath = Path.Combine(outputDirectory, entry.FilenameInZip);

////                            // Ratio to approximate compressed bytes progressed
////                            double compPerUnc = (entry.FileSize > 0) ? (double)entry.CompressedSize / (double)entry.FileSize : 0.0;

////                            // Per-chunk callback (uncompressed bytes)
////                            void OnBytes(long count)
////                            {
////                                Interlocked.Add(ref processedUncompressed, count);
////                                if (compPerUnc > 0)
////                                {
////                                    long approxComp = (long)Math.Round(count * compPerUnc);
////                                    Interlocked.Add(ref processedCompressedApprox, approxComp);
////                                }
////                                Volatile.Write(ref lastFileName, entry.FilenameInZip);
////                            }

////                            bool ok = ExtractFile(entry, outPath, OnBytes);
////                            if (!ok)
////                            {
////                                linkedCts.Cancel();
////                                throw new Exception($"Extraction failed for: {entry.FilenameInZip}");
////                            }

////                            Interlocked.Increment(ref filesDone);
////                        }
////                        finally
////                        {
////                            semaphore.Release();
////                        }
////                    }, linkedCts.Token);

////                    tasks.Add(task);
////                }

////                await Task.WhenAll(tasks).ConfigureAwait(false);

////                // Final report @ 100%
////                if (progress != null)
////                {
////                    var elapsed = sw.Elapsed;
////                    var speed = elapsed.TotalSeconds > 0 ? totalUncompressed / elapsed.TotalSeconds : 0.0;
////                    var fps = elapsed.TotalSeconds > 0 ? totalFiles / elapsed.TotalSeconds : 0.0;

////                    progress.Report(new ZipProgress
////                    {
////                        Operation = ZipOperation.Extract,
////                        CurrentFile = null,

////                        TotalFiles = totalFiles,
////                        TotalBytesUncompressed = totalUncompressed,
////                        TotalBytesCompressed = totalCompressed,

////                        FilesProcessed = totalFiles,
////                        BytesProcessedUncompressed = totalUncompressed,
////                        BytesProcessedCompressed = totalCompressed, // now exact

////                        Elapsed = elapsed,
////                        SpeedBytesPerSec = speed,
////                        FilesPerSec = fps
////                    });
////                }

////                reportCts?.Cancel();
////                try { await reportingTask.ConfigureAwait(false); } catch { /* ignore */ }

////                return true;
////            }
////            catch (OperationCanceledException)
////            {
////                return false;
////            }
////            catch
////            {
////                return false;
////            }
////        }


////        // Extract to path (reports uncompressed bytes via onBytes)
////        public bool ExtractFile(ZipFileEntry zfe, string outPathFilename, Action<long> onBytes = null)
////        {
////            Stream output = null;
////            try
////            {
////                // Ensure parent dir exists
////                string? dir = Path.GetDirectoryName(outPathFilename);
////                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
////                    Directory.CreateDirectory(dir);

////                // Directory entry? Create dir and return
////                if (zfe.FilenameInZip.EndsWith("/") || zfe.FilenameInZip.EndsWith("\\"))
////                {
////                    Directory.CreateDirectory(outPathFilename);
////                    return true;
////                }

////                // Replace existing file
////                if (File.Exists(outPathFilename))
////                {
////                    try { File.Delete(outPathFilename); }
////                    catch { throw new InvalidOperationException($"File '{outPathFilename}' cannot be written"); }
////                }

////                while (FastZipDotNet.Pause) Thread.Sleep(50);

////                output = new FileStream(outPathFilename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete, 1 << 20, FileOptions.SequentialScan);

////                bool ok = ExtractFile(zfe, output, onBytes);
////                output.Close();

////                // Set times if OK
////                if (ok)
////                {
////                    try
////                    {
////                        File.SetCreationTime(outPathFilename, zfe.ModifyTime);
////                        File.SetLastWriteTime(outPathFilename, zfe.ModifyTime);
////                    }
////                    catch { /* ignore */ }
////                }

////                return ok;
////            }
////            finally
////            {
////                output?.Dispose();
////            }
////        }

////        // Extract to an existing stream (reports uncompressed bytes via onBytes)
////        public bool ExtractFile(ZipFileEntry zfe, Stream outStream, Action<long> onBytes = null)
////        {
////            if (outStream is null) throw new ArgumentNullException(nameof(outStream));
////            if (!outStream.CanWrite) throw new InvalidOperationException("Stream cannot be written");

////            FileStream zipStream = null;
////            try
////            {
////                zipStream = new FileStream(
////                    FastZipDotNet.ZipFileName,
////                    FileMode.Open,
////                    FileAccess.Read,
////                    FileShare.ReadWrite,
////                    1 << 20,
////                    FileOptions.RandomAccess);

////                zipStream.Seek((long)zfe.HeaderOffset, SeekOrigin.Begin);
////                using var br = new BinaryReader(zipStream, Encoding.Default, leaveOpen: true);

////                uint signature = br.ReadUInt32();
////                if (signature != 0x04034b50) return false;

////                ushort versionNeeded = br.ReadUInt16();
////                ushort generalPurposeBitFlag = br.ReadUInt16();
////                ushort compressionMethodLocal = br.ReadUInt16(); // may be 99 for AES
////                ushort lastModTime = br.ReadUInt16();
////                ushort lastModDate = br.ReadUInt16();
////                uint crc32Local = br.ReadUInt32();
////                uint compressedSizeLocal = br.ReadUInt32();
////                uint uncompressedSizeLocal = br.ReadUInt32();
////                ushort fileNameLength = br.ReadUInt16();
////                ushort extraFieldLength = br.ReadUInt16();

////                if (fileNameLength > 0) br.ReadBytes(fileNameLength);
////                if (extraFieldLength > 0) br.ReadBytes(extraFieldLength);

////                bool hasDataDescriptor = (generalPurposeBitFlag & 0x0008) != 0;
////                bool isEncrypted = (generalPurposeBitFlag & 0x0001) != 0;

////                // method decision
////                Compression actualMethod = zfe.IsAes ? zfe.Method : (Compression)compressionMethodLocal;

////                // Build input stream (AES or ZipCrypto or none)
////                Stream inputStream = null;
////                try
////                {
////                    if (isEncrypted)
////                    {
////                        if (zfe.IsAes)
////                        {
////                            if (string.IsNullOrEmpty(FastZipDotNet.Password))
////                                throw new CryptographicException("Password required for AES-encrypted entry.");

////                            // total encrypted size includes salt + pwv + ciphertext + 10 tag
////                            long totalEncryptedSize = (long)zfe.CompressedSize;
////                            if (totalEncryptedSize <= 0) throw new InvalidDataException("Invalid AES compressed size");

////                            byte strength = zfe.AesStrength != 0 ? zfe.AesStrength : (byte)3; // default to 256 if unknown
////                            var aes = new AesDecryptStream(zipStream, FastZipDotNet.Password, strength, totalEncryptedSize);

////                            // Decompressor wraps the AES plaintext stream
////                            if (actualMethod == Compression.Store)
////                                inputStream = aes;
////                            else if (actualMethod == Compression.Deflate)
////                                inputStream = new DeflateStream(aes, CompressionMode.Decompress, leaveOpen: true);
////                            else if (actualMethod == Compression.Zstd)
////                                inputStream = new DecompressionStream(aes);
////                            else
////                                return false;
////                        }
////                        else
////                        {
////                            // ZipCrypto
////                            if (string.IsNullOrEmpty(FastZipDotNet.Password))
////                                throw new CryptographicException("Password required for encrypted ZIP entry.");

////                            byte verifier = hasDataDescriptor ? (byte)(lastModTime >> 8) : (byte)(zfe.Crc32 >> 24);

////                            long payloadLen = (long)zfe.CompressedSize - 12;
////                            if (payloadLen < 0) throw new InvalidDataException("Invalid encrypted size");

////                            var decrypt = new Encryption.ZipCryptoDecryptStream(zipStream, FastZipDotNet.Password, verifier, payloadLen);

////                            if (actualMethod == Compression.Store)
////                                inputStream = decrypt;
////                            else if (actualMethod == Compression.Deflate)
////                                inputStream = new DeflateStream(decrypt, CompressionMode.Decompress, leaveOpen: true);
////                            else if (actualMethod == Compression.Zstd)
////                                inputStream = new DecompressionStream(decrypt);
////                            else
////                                return false;
////                        }
////                    }
////                    else
////                    {
////                        if (actualMethod == Compression.Store)
////                            inputStream = zipStream;
////                        else if (actualMethod == Compression.Deflate)
////                            inputStream = new DeflateStream(zipStream, CompressionMode.Decompress, leaveOpen: true);
////                        else if (actualMethod == Compression.Zstd)
////                            inputStream = new DecompressionStream(zipStream);
////                        else
////                            return false;
////                    }

////                    uint crc32Calc = 0;
////                    byte[] buffer = new byte[32768];
////                    ulong remaining = zfe.FileSize;

////                    while (remaining > 0)
////                    {
////                        while (FastZipDotNet.Pause) Thread.Sleep(50);

////                        int toRead = (int)Math.Min((ulong)buffer.Length, remaining);
////                        int bytesRead = inputStream.Read(buffer, 0, toRead);
////                        if (bytesRead <= 0) break;

////                        crc32Calc = Crc32Algorithm.Append(crc32Calc, buffer, 0, bytesRead);
////                        outStream.Write(buffer, 0, bytesRead);
////                        onBytes?.Invoke(bytesRead);

////                        Interlocked.Add(ref FastZipDotNet.BytesWritten, bytesRead);
////                        Interlocked.Add(ref FastZipDotNet.BytesPerSecond, bytesRead);

////                        remaining -= (ulong)bytesRead;
////                    }

////                    Interlocked.Increment(ref FastZipDotNet.FilesPerSecond);
////                    Interlocked.Increment(ref FastZipDotNet.FilesWritten);
////                    outStream.Flush();

////                    // For AES, CRC field may be 0; do not strictly fail on CRC mismatch if AES; MAC already verified by stream
////                    if (zfe.IsAes) return remaining == 0;
////                    return remaining == 0 && (zfe.Crc32 == crc32Calc);
////                }
////                finally
////                {
////                    if (inputStream != null && inputStream != zipStream)
////                        inputStream.Dispose();
////                }
////            }
////            finally
////            {
////                zipStream?.Dispose();
////            }
////        }

////        // Keep this convenience overload for compatibility
////        public bool ExtractFile(ZipFileEntry zfe, string outPathFilename)
////            => ExtractFile(zfe, outPathFilename, onBytes: null);


////        public bool ExtractFile(ZipFileEntry zfe, Stream outStream)
////    => ExtractFile(zfe, outStream, onBytes: null);


////        #endregion


////    }
////}
