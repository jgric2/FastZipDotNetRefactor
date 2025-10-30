using FastZipDotNet.MultiThreading;
using FastZipDotNet.WinAPIHelper;
using FastZipDotNet.Zip.Encryption;
using FastZipDotNet.Zip.Helpers;
using FastZipDotNet.Zip.LibDeflate;
using FastZipDotNet.Zip.Readers;
using FastZipDotNet.Zip.ZStd;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

namespace FastZipDotNet.Zip.Writers
{
    public class ZipDataWriter
    {
        public ZipDataWriter(FastZipDotNet fastZipDotNet)
        {
            FastZipDotNet = fastZipDotNet;
        
        }

        FastZipDotNet FastZipDotNet;
        // Write a byte[] at an absolute file offset; report compressed progress by chunk
        private void WriteAt(byte[] data, long fileOffset, Action<long> onCompressedWrite)
        {
            using (var fs = new FileStream(
                FastZipDotNet.ZipFileName,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.ReadWrite,
                Consts.ChunkSize,
                FileOptions.RandomAccess)) // changed to RandomAccess
            {
                fs.Position = fileOffset;

                int offset = 0;
                while (offset < data.Length)
                {
                    WaitWhilePaused();

                    int toWrite = Math.Min(Consts.ChunkSize, data.Length - offset);
                    fs.Write(data, offset, toWrite);
                    offset += toWrite;

                    Interlocked.Add(ref FastZipDotNet.BytesWritten, toWrite);
                    Interlocked.Add(ref FastZipDotNet.BytesPerSecond, toWrite);

                    onCompressedWrite?.Invoke(toWrite);
                }
                fs.Flush();
            }
        }





        // Reads from 'source' and writes to archive at destOffset.
        // Calls onUncompressedProgress and onCompressedWrite per chunk.
        // Reads from 'source' and writes to archive at destOffset.
        // Calls onUncompressedProgress and onCompressedWrite per chunk.
        private void WriteStreamAt(Stream source, long destOffset, Action<long> onUncompressedProgress, Action<long> onCompressedWrite)
        {
            using (var fs = new FileStream(
                FastZipDotNet.ZipFileName,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.ReadWrite,
                Consts.ChunkSize,
                FileOptions.RandomAccess)) // changed to RandomAccess
            {
                fs.Position = destOffset;

                byte[] buffer = new byte[Consts.ChunkSize];
                int read;
                while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    WaitWhilePaused();

                    fs.Write(buffer, 0, read);

                    onCompressedWrite?.Invoke(read);
                    onUncompressedProgress?.Invoke(read);

                    Interlocked.Add(ref FastZipDotNet.BytesWritten, read);
                    Interlocked.Add(ref FastZipDotNet.BytesPerSecond, read);
                }
                fs.Flush();
            }
        }


        private void WriteStreamAt(Stream source, long destOffset, Action<long> onUncompressedProgress, Action<long> onCompressedWrite, IBufferTransform transform)
        {
            using (var fs = new FileStream(
      FastZipDotNet.ZipFileName,
      FileMode.OpenOrCreate,
      FileAccess.Write,
      FileShare.ReadWrite,
      Consts.ChunkSize,
      FileOptions.RandomAccess))
            {
                fs.Position = destOffset;

                byte[] buffer = new byte[Consts.ChunkSize];
                int read;
                while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    WaitWhilePaused();

                    transform?.Transform(buffer, 0, read);

                    fs.Write(buffer, 0, read);

                    onCompressedWrite?.Invoke(read);
                    onUncompressedProgress?.Invoke(read);

                    Interlocked.Add(ref FastZipDotNet.BytesWritten, read);
                    Interlocked.Add(ref FastZipDotNet.BytesPerSecond, read);
                }
                fs.Flush();
            }
        }


        public static List<FileSystemInfo> GetFilesAndFoldersLevelByLevel(string rootDirectory)
        {
            var directoriesToProcess = new Queue<DirectoryInfo>();
            var resultList = new List<FileSystemInfo>();
            directoriesToProcess.Enqueue(new DirectoryInfo(rootDirectory));

            while (directoriesToProcess.Count > 0)
            {
                int levelCount = directoriesToProcess.Count;
                var currentLevelDirectories = new List<DirectoryInfo>();

                // Dequeue all directories at the current level
                for (int i = 0; i < levelCount; i++)
                {
                    var currentDirectory = directoriesToProcess.Dequeue();
                    currentLevelDirectories.Add(currentDirectory);
                }

                // Process directories at the current level in parallel
                Parallel.ForEach(currentLevelDirectories, directory =>
                {
                    // Add the current directory to the result list
                    lock (resultList)
                    {
                        resultList.Add(directory);
                    }

                    // Collect files in the current directory
                    var files = directory.GetFiles();
                    lock (resultList)
                    {
                        resultList.AddRange(files);
                    }

                    // Collect subdirectories and enqueue them for the next level
                    var subdirectories = directory.GetDirectories();
                    lock (directoriesToProcess)
                    {
                        foreach (var subdirectory in subdirectories)
                        {
                            directoriesToProcess.Enqueue(subdirectory);
                        }
                    }
                });
            }

            return resultList;
        }


        public long AddFileInner(string pathFilename, string filenameInZip, int compressionLevel, long fSize, DateTime modifyTime, string fileComment = "")
        {
            byte[] inBuffer = null;
            long compressedSize = 0;
            try
            {
                if (fSize > 0)
                {
                    while (FastZipDotNet.Pause)
                    {
                        Thread.Sleep(50);
                    }

                    inBuffer = new byte[fSize];
                    var sourceHandle = WinAPI.CreateFileW(pathFilename, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, (FileAttributes)WinAPI.EFileAttributes.SequentialScan, IntPtr.Zero);
                    bool successRead = WinAPI.ReadFile(sourceHandle, inBuffer, (uint)fSize, out var read, IntPtr.Zero);
                    WinAPI.CloseHandle(sourceHandle);
                }
                else
                {
                    inBuffer = Array.Empty<byte>();
                }

                while (FastZipDotNet.Pause)
                {
                    Thread.Sleep(50);
                }

                compressedSize = AddBuffer(inBuffer, filenameInZip, modifyTime, compressionLevel, fileComment);

                inBuffer = null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn BrutalZip.AddFileInner");
            }
          //  GC.WaitForPendingFinalizers();
        //    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            return compressedSize;
        }

        public long AddFileInnerLarge(string pathFilename, string filenameInZip, int compressionLevel, string fileComment = "")
        {
            long compressedSize = 0;
            try
            {
                using (FileStream fs = new FileStream(pathFilename, FileMode.Open, FileAccess.Read, FileShare.None, Consts.ChunkSize, FileOptions.SequentialScan))
                {
                    DateTime modifyTime = File.GetLastWriteTime(pathFilename);
                    compressedSize = AddStream(fs, filenameInZip, modifyTime, compressionLevel, fileComment);
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn BrutalZip.AddFileInnerLarge"); }

            return compressedSize;
        }



        public long AddBuffer(
      byte[] inBuffer,
      string filenameInZip,
      DateTime modifyTime,
      int compressionLevel,
      string fileComment,
      Action<long> onUncompressedProgress,
      Action<long> onCompressedWrite)
        {
            if (FastZipDotNet.PartSize != 0)
                throw new InvalidOperationException("Single-part only (PartSize must be 0).");

            byte[] outBuffer = null;

            var zfe = new ZipFileEntry
            {
                FileSize = (ulong)(inBuffer?.Length ?? 0),
                EncodeUTF8 = FastZipDotNet.EncodeUTF8,
                FilenameInZip = IOHelpers.NormalizedFilename(filenameInZip),
                Comment = fileComment,
                ModifyTime = modifyTime,
                Method = (compressionLevel == 0 || (inBuffer?.Length ?? 0) == 0) ? Compression.Store : FastZipDotNet.MethodCompress,
                Crc32 = Crc32Helpers.ComputeCrc32(inBuffer ?? Array.Empty<byte>())
            };

            if (zfe.Method == Compression.Deflate)
            {
                LibDeflateWrapper.Libdeflate(inBuffer ?? Array.Empty<byte>(), compressionLevel, false, out outBuffer, out zfe.CompressedSize, out zfe.Crc32);
                if (outBuffer != null && (ulong)outBuffer.Length != zfe.CompressedSize)
                    Array.Resize(ref outBuffer, (int)zfe.CompressedSize);

                if (zfe.CompressedSize == 0 || zfe.CompressedSize >= zfe.FileSize)
                {
                    zfe.Method = Compression.Store;
                    zfe.CompressedSize = zfe.FileSize;
                    outBuffer = null;
                }
            }
            else if (zfe.Method == Compression.Zstd)
            {
                using var compressor = new Compressor(FastZipDotNet.CompressionOptionsZstd);
                outBuffer = compressor.Wrap(inBuffer ?? Array.Empty<byte>());
                zfe.CompressedSize = (ulong)outBuffer.Length;
                if (zfe.CompressedSize >= zfe.FileSize)
                {
                    zfe.Method = Compression.Store;
                    zfe.CompressedSize = zfe.FileSize;
                    outBuffer = null;
                }
            }
            else
            {
                zfe.CompressedSize = zfe.FileSize;
            }

            bool doZipCrypto = FastZipDotNet.Encryption == EncryptionAlgorithm.ZipCrypto &&
                               !string.IsNullOrEmpty(FastZipDotNet.Password);

            zfe.IsEncrypted = doZipCrypto;
            if (doZipCrypto)
                zfe.CompressedSize += 12;

            var localHeader = ZipWritersHeaders.BuildLocalHeaderBytes(ref zfe);

            long totalLen = localHeader.Length + (long)zfe.CompressedSize;
            long start = FastZipDotNet.Reserve(totalLen);

            zfe.HeaderOffset = (ulong)start;
            zfe.DiskNumberStart = 0;

            // Write local header
            WriteAt(localHeader, start, onCompressedWrite);

            if (doZipCrypto)
            {
                var crypto = new TraditionalZipCrypto(FastZipDotNet.Password);
                byte[] encHeader = crypto.GenerateEncryptedHeaderFromCrc(zfe.Crc32);

                WriteAt(encHeader, start + localHeader.Length, onCompressedWrite);

                byte[] payload = (zfe.Method == Compression.Store) ? (inBuffer ?? Array.Empty<byte>()) : (outBuffer ?? Array.Empty<byte>());
                if (payload.Length > 0)
                {
                    // Encrypt payload in-place
                    crypto.Encrypt(payload, 0, payload.Length);

                    if (zfe.Method == Compression.Store)
                    {
                        Action<long> onCompWrapped = n => { onCompressedWrite?.Invoke(n); onUncompressedProgress?.Invoke(n); };
                        WriteAt(payload, start + localHeader.Length + encHeader.Length, onCompWrapped);
                    }
                    else
                    {
                        double invRatio = (zfe.CompressedSize - 12) > 0 ? (double)zfe.FileSize / (double)(zfe.CompressedSize - 12) : 0.0;
                        long uncSent = 0;
                        Action<long> onCompWrapped = n =>
                        {
                            onCompressedWrite?.Invoke(n);
                            if (invRatio <= 0) return;
                            long add = (long)Math.Round(n * invRatio);
                            long remaining = (long)zfe.FileSize - uncSent;
                            if (add > remaining) add = remaining;
                            if (add > 0) { onUncompressedProgress?.Invoke(add); uncSent += add; }
                        };

                        WriteAt(payload, start + localHeader.Length + encHeader.Length, onCompWrapped);

                        long finalRem = (long)zfe.FileSize - uncSent;
                        if (finalRem > 0) onUncompressedProgress?.Invoke(finalRem);
                    }
                }
            }
            else
            {
                if (zfe.Method == Compression.Store)
                {
                    Action<long> onCompWrapped = n => { onCompressedWrite?.Invoke(n); onUncompressedProgress?.Invoke(n); };
                    WriteAt(inBuffer ?? Array.Empty<byte>(), start + localHeader.Length, onCompWrapped);
                }
                else
                {
                    double invRatio = zfe.CompressedSize > 0 ? (double)zfe.FileSize / (double)zfe.CompressedSize : 0.0;
                    long uncSent = 0;
                    Action<long> onCompWrapped = n =>
                    {
                        onCompressedWrite?.Invoke(n);
                        if (invRatio <= 0) return;
                        long add = (long)Math.Round(n * invRatio);
                        long remaining = (long)zfe.FileSize - uncSent;
                        if (add > remaining) add = remaining;
                        if (add > 0) { onUncompressedProgress?.Invoke(add); uncSent += add; }
                    };

                    var payload = outBuffer ?? Array.Empty<byte>();
                    WriteAt(payload, start + localHeader.Length, onCompWrapped);

                    long finalRem = (long)zfe.FileSize - uncSent;
                    if (finalRem > 0) onUncompressedProgress?.Invoke(finalRem);
                }
            }

            lock (FastZipDotNet.ZipFileEntries)
                FastZipDotNet.ZipFileEntries.Add(zfe);

            Interlocked.Increment(ref FastZipDotNet.FilesWritten);
            Interlocked.Increment(ref FastZipDotNet.FilesPerSecond);

            return (long)zfe.CompressedSize;
        }



        ////   // Progress-capable AddBuffer
        ////   public long AddBuffer(byte[] inBuffer, string filenameInZip, DateTime modifyTime, int compressionLevel, string fileComment,
        ////Action<long> onUncompressedProgress, Action<long> onCompressedWrite)
        ////   {
        ////       if (FastZipDotNet.PartSize != 0)
        ////           throw new InvalidOperationException("Single-part only (PartSize must be 0).");

        ////       byte[] outBuffer = null;

        ////       var zfe = new ZipFileEntry
        ////       {
        ////           FileSize = (ulong)inBuffer.Length,
        ////           EncodeUTF8 = FastZipDotNet.EncodeUTF8,
        ////           FilenameInZip = IOHelpers.NormalizedFilename(filenameInZip),
        ////           Comment = fileComment,
        ////           ModifyTime = modifyTime,
        ////           Method = (compressionLevel == 0 || inBuffer.Length == 0) ? Compression.Store : FastZipDotNet.MethodCompress,
        ////           Crc32 = Crc32Helpers.ComputeCrc32(inBuffer)
        ////       };

        ////       if (zfe.Method == Compression.Deflate)
        ////       {
        ////           LibDeflateWrapper.Libdeflate(inBuffer, compressionLevel, false, out outBuffer, out zfe.CompressedSize, out zfe.Crc32);
        ////           if (zfe.CompressedSize == 0 || zfe.CompressedSize >= zfe.FileSize)
        ////           {
        ////               zfe.Method = Compression.Store;
        ////               zfe.CompressedSize = zfe.FileSize;
        ////           }
        ////       }
        ////       else if (zfe.Method == Compression.Zstd)
        ////       {
        ////           using (var compressor = new Compressor(FastZipDotNet.CompressionOptionsZstd))
        ////               outBuffer = compressor.Wrap(inBuffer);

        ////           zfe.CompressedSize = (ulong)outBuffer.Length;
        ////           if (zfe.CompressedSize >= zfe.FileSize)
        ////           {
        ////               zfe.Method = Compression.Store;
        ////               zfe.CompressedSize = zfe.FileSize;
        ////           }
        ////       }
        ////       else
        ////       {
        ////           zfe.CompressedSize = zfe.FileSize; // Store
        ////       }

        ////       var localHeader = ZipWritersHeaders.BuildLocalHeaderBytes(ref zfe);

        ////       long totalLen = localHeader.Length + (long)zfe.CompressedSize;
        ////       long start = FastZipDotNet.Reserve(totalLen);

        ////       zfe.HeaderOffset = (ulong)start;
        ////       zfe.DiskNumberStart = 0;

        ////       WriteAt(localHeader, start, onCompressedWrite);

        ////       if (zfe.Method == Compression.Store)
        ////       {
        ////           // For store, uncompressed progress equals compressed write bytes
        ////           Action<long> onCompWrapped = n =>
        ////           {
        ////               onCompressedWrite?.Invoke(n);
        ////               onUncompressedProgress?.Invoke(n);
        ////           };
        ////           WriteAt(inBuffer, start + localHeader.Length, onCompWrapped);
        ////       }
        ////       else
        ////       {
        ////           // Compressed: report uncompressed-equivalent during write
        ////           double invRatio = zfe.CompressedSize > 0 ? (double)zfe.FileSize / (double)zfe.CompressedSize : 0.0;
        ////           long uncSent = 0;

        ////           Action<long> onCompWrapped = n =>
        ////           {
        ////               onCompressedWrite?.Invoke(n);

        ////               if (invRatio <= 0) return;
        ////               long add = (long)Math.Round(n * invRatio);
        ////               long remaining = (long)zfe.FileSize - uncSent;
        ////               if (add > remaining) add = remaining;
        ////               if (add > 0)
        ////               {
        ////                   onUncompressedProgress?.Invoke(add);
        ////                   uncSent += add;
        ////               }
        ////           };

        ////           if (outBuffer == null) outBuffer = Array.Empty<byte>();
        ////           WriteAt(outBuffer, start + localHeader.Length, onCompWrapped);

        ////           long finalRem = (long)zfe.FileSize - uncSent;
        ////           if (finalRem > 0) onUncompressedProgress?.Invoke(finalRem);
        ////       }

        ////       lock (FastZipDotNet.ZipFileEntries)
        ////           FastZipDotNet.ZipFileEntries.Add(zfe);

        ////       Interlocked.Increment(ref FastZipDotNet.FilesWritten);
        ////       Interlocked.Increment(ref FastZipDotNet.FilesPerSecond);

        ////       return (long)zfe.CompressedSize;
        ////   }

        public long AddStream(Stream inStream, string filenameInZip, DateTime modifyTime, int compressionLevel, string fileComment,
     Action<long> onUncompressedProgress, Action<long> onCompressedWrite)
        {
            if (FastZipDotNet.PartSize != 0)
                throw new InvalidOperationException("Single-part only (PartSize must be 0).");

            long compressedSize = 0;
            string tempFilePath = null;

            try
            {
                inStream.Position = 0;

                var zfe = new ZipFileEntry
                {
                    FileSize = (ulong)inStream.Length,
                    EncodeUTF8 = FastZipDotNet.EncodeUTF8,
                    FilenameInZip = IOHelpers.NormalizedFilename(filenameInZip),
                    Comment = fileComment,
                    ModifyTime = modifyTime,
                    Method = (compressionLevel == 0 || inStream.Length == 0) ? Compression.Store : FastZipDotNet.MethodCompress
                };

                // CRC on plaintext
                zfe.Crc32 = Helpers.Crc32Helpers.ComputeCrc32(inStream);
                inStream.Position = 0;

                // Prepare payload source: compress or use store
                Stream dataSource;
                if (zfe.Method == Compression.Store)
                {
                    zfe.CompressedSize = zfe.FileSize;
                    dataSource = inStream;
                }
                else
                {
                    // Try compressing to temp
                    tempFilePath = Path.GetTempFileName();
                    using (var temp = new FileStream(tempFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Consts.ChunkSize, FileOptions.SequentialScan))
                    {
                        if (zfe.Method == Compression.Deflate)
                        {
                            using var defl = new DeflateStream(temp, FastZipDotNet.CurrentCompression, true);
                            byte[] buf = new byte[Consts.ChunkSize];
                            int r;
                            while ((r = inStream.Read(buf, 0, buf.Length)) > 0)
                            {
                                WaitWhilePaused();
                                defl.Write(buf, 0, r);
                            }
                        }
                        else if (zfe.Method == Compression.Zstd)
                        {
                            using var zstd = new CompressionStream(temp, FastZipDotNet.CompressionOptionsZstd);
                            byte[] buf = new byte[Consts.ChunkSize];
                            int r;
                            while ((r = inStream.Read(buf, 0, buf.Length)) > 0)
                            {
                                WaitWhilePaused();
                                zstd.Write(buf, 0, r);
                            }
                        }
                    }

                    using (var tf = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.SequentialScan))
                        zfe.CompressedSize = (ulong)tf.Length;

                    if (zfe.CompressedSize >= zfe.FileSize)
                    {
                        // Fallback to store: we must reuse the original stream as source — rewind it
                        zfe.Method = Compression.Store;
                        zfe.CompressedSize = zfe.FileSize;
                        dataSource = inStream;
                    }
                    else
                    {
                        // Keep compressed stream as source
                        dataSource = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Consts.ChunkSize, FileOptions.SequentialScan);
                    }
                }

                bool doZipCrypto = FastZipDotNet.Encryption == EncryptionAlgorithm.ZipCrypto &&
                                   !string.IsNullOrEmpty(FastZipDotNet.Password);

                zfe.IsEncrypted = doZipCrypto;
                if (doZipCrypto)
                {
                    zfe.CompressedSize += 12; // include ZipCrypto 12-byte header
                }

                var localHeader = ZipWritersHeaders.BuildLocalHeaderBytes(ref zfe);

                long totalLen = localHeader.Length + (long)zfe.CompressedSize;
                long start = FastZipDotNet.Reserve(totalLen);

                zfe.HeaderOffset = (ulong)start;
                zfe.DiskNumberStart = 0;

                // Write local header
                WriteAt(localHeader, start, onCompressedWrite);

                // IMPORTANT: ensure data source is at the beginning before writing payload
                if (ReferenceEquals(dataSource, inStream))
                {
                    if (inStream.CanSeek) inStream.Position = 0; // this fixes the bug for fallback-to-store
                }
                else if (dataSource.CanSeek)
                {
                    dataSource.Position = 0;
                }

                if (doZipCrypto)
                {
                    var crypto = new TraditionalZipCrypto(FastZipDotNet.Password);

                    // We do not use data descriptor, so verifier is CRC high 16 bits (both bytes set)
                    byte[] encHeader = crypto.GenerateEncryptedHeaderFromCrc(zfe.Crc32);
                    WriteAt(encHeader, start + localHeader.Length, onCompressedWrite);

                    IBufferTransform transform = new ZipCryptoEncryptTransform(crypto);

                    if (zfe.Method == Compression.Store)
                    {
                        // Store: uncompressed progress equals payload bytes
                        WriteStreamAt(
                            dataSource,
                            start + localHeader.Length + encHeader.Length,
                            onUncompressedProgress,
                            onCompressedWrite,
                            transform);
                    }
                    else
                    {
                        // Compressed: map compressed to uncompressed for progress
                        double invRatio = (zfe.CompressedSize - 12) > 0 ? (double)zfe.FileSize / (double)(zfe.CompressedSize - 12) : 0.0;
                        long uncSent = 0;
                        void OnUncEq(long n)
                        {
                            if (invRatio <= 0) return;
                            long add = (long)Math.Round(n * invRatio);
                            long remaining = (long)zfe.FileSize - uncSent;
                            if (add > remaining) add = remaining;
                            if (add > 0) { onUncompressedProgress?.Invoke(add); uncSent += add; }
                        }

                        WriteStreamAt(
                            dataSource,
                            start + localHeader.Length + encHeader.Length,
                            OnUncEq,
                            onCompressedWrite,
                            transform);

                        long finalRem = (long)zfe.FileSize - uncSent;
                        if (finalRem > 0) onUncompressedProgress?.Invoke(finalRem);
                    }
                }
                else
                {
                    // Not encrypted
                    if (zfe.Method == Compression.Store)
                    {
                        WriteStreamAt(
                            dataSource,
                            start + localHeader.Length,
                            onUncompressedProgress,
                            onCompressedWrite,
                            transform: null);
                    }
                    else
                    {
                        double invRatio = zfe.CompressedSize > 0 ? (double)zfe.FileSize / (double)zfe.CompressedSize : 0.0;
                        long uncSent = 0;
                        void OnUncEq(long n)
                        {
                            if (invRatio <= 0) return;
                            long add = (long)Math.Round(n * invRatio);
                            long remaining = (long)zfe.FileSize - uncSent;
                            if (add > remaining) add = remaining;
                            if (add > 0) { onUncompressedProgress?.Invoke(add); uncSent += add; }
                        }

                        WriteStreamAt(
                            dataSource,
                            start + localHeader.Length,
                            OnUncEq,
                            onCompressedWrite,
                            transform: null);

                        long finalRem = (long)zfe.FileSize - uncSent;
                        if (finalRem > 0) onUncompressedProgress?.Invoke(finalRem);
                    }
                }

                lock (FastZipDotNet.ZipFileEntries)
                    FastZipDotNet.ZipFileEntries.Add(zfe);

                Interlocked.Increment(ref FastZipDotNet.FilesWritten);
                Interlocked.Increment(ref FastZipDotNet.FilesPerSecond);

                compressedSize = (long)zfe.CompressedSize;

                if (!ReferenceEquals(dataSource, inStream))
                    dataSource.Dispose();
            }
            finally
            {
                if (tempFilePath != null && File.Exists(tempFilePath))
                {
                    try { File.Delete(tempFilePath); } catch { }
                }
            }

            return compressedSize;
        }



        public async Task<bool> AddFilesToArchiveAsync(string directoryToAdd,
    int threads,
    int compressionLevel,
    IProgress<ZipProgress> progress,
    CancellationToken ct = default)
        {
            try
            {
                // Immediately yield to keep UI responsive
                await Task.Yield();

                // Enumerate all files
                var files = Directory.EnumerateFiles(directoryToAdd, "*", SearchOption.AllDirectories).ToList();
                int totalFiles = files.Count;
                long totalUncompressed = 0;
                foreach (var f in files) totalUncompressed += new FileInfo(f).Length;

                // Throttle concurrency; leave 1 core for UI/GC if desired
                int maxDop = Math.Max(1, threads <= 0 ? Environment.ProcessorCount - 1 : threads);

                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount : threads));
                var sem = FastZipDotNet.ConcurrencyLimiter;

                //  var sem = new SemaphoreSlim(maxDop, maxDop);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                var tasks = new List<Task>(totalFiles);

                long processedUncompressed = 0;
                long processedCompressed = 0;
                int filesDone = 0;
                string lastFileName = null;

                var sw = Stopwatch.StartNew();

                // Progress reporter on a dedicated thread to avoid thread pool starvation
                using var reportCts = CancellationTokenSource.CreateLinkedTokenSource(linkedCts.Token);
                var reportingTask = Task.Factory.StartNew(async () =>
                {
                    while (!reportCts.IsCancellationRequested)
                    {
                        var elapsed = sw.Elapsed;
                        long uncRaw = Interlocked.Read(ref processedUncompressed);
                        long comp = Interlocked.Read(ref processedCompressed);
                        int filesProcessed = Volatile.Read(ref filesDone);

                        // Clamp to 100% max due to rounding
                        long unc = Math.Min(uncRaw, totalUncompressed);

                        double speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;
                        double fps = elapsed.TotalSeconds > 0 ? filesProcessed / elapsed.TotalSeconds : 0.0;

                        progress?.Report(new ZipProgress
                        {
                            Operation = ZipOperation.Build,
                            CurrentFile = Volatile.Read(ref lastFileName),

                            TotalFiles = totalFiles,
                            TotalBytesUncompressed = totalUncompressed,
                            TotalBytesCompressed = comp,

                            FilesProcessed = filesProcessed,
                            BytesProcessedUncompressed = unc,
                            BytesProcessedCompressed = comp,

                            Elapsed = elapsed,
                            SpeedBytesPerSec = speed,
                            FilesPerSec = fps
                        });

                        try { await Task.Delay(200, reportCts.Token).ConfigureAwait(false); }
                        catch (OperationCanceledException) { break; }
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

                int baseLen = directoryToAdd.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length;

                foreach (var fullPath in files)
                {
                    // Asynchronous wait – does not block UI thread
                    await sem.WaitAsync(linkedCts.Token).ConfigureAwait(false);

                    var task = Task.Run(() =>
                    {
                        try
                        {
                            // Compute internal name
                            string relative = fullPath.Substring(baseLen)
                                                      .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                            string internalName = IOHelpers.NormalizedFilename(relative);

                            void OnUnc(long n)
                            {
                                Interlocked.Add(ref processedUncompressed, n);
                            }

                            void OnComp(long n)
                            {
                                Interlocked.Add(ref processedCompressed, n);
                                Volatile.Write(ref lastFileName, internalName);
                            }

                            AddFileWithProgress(fullPath, internalName, compressionLevel, "", OnUnc, OnComp);

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

                // Final 100% report
                {
                    var elapsed = sw.Elapsed;
                    var unc = totalUncompressed;
                    var comp = Interlocked.Read(ref processedCompressed);
                    double speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;
                    double fps = elapsed.TotalSeconds > 0 ? totalFiles / elapsed.TotalSeconds : 0.0;

                    progress?.Report(new ZipProgress
                    {
                        Operation = ZipOperation.Build,
                        CurrentFile = null,

                        TotalFiles = totalFiles,
                        TotalBytesUncompressed = unc,
                        TotalBytesCompressed = comp,

                        FilesProcessed = totalFiles,
                        BytesProcessedUncompressed = unc,
                        BytesProcessedCompressed = comp,

                        Elapsed = elapsed,
                        SpeedBytesPerSec = speed,
                        FilesPerSec = fps
                    });
                }

                reportCts.Cancel();
                try { await reportingTask.ConfigureAwait(false); } catch { }

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

        public async Task<bool> UpdateArchiveFromDirectoryAsync(string directoryToSync, int threads, int compressionLevel, IProgress<ZipProgress> progress, CancellationToken ct = default)
        {
            try
            {
                // Build a lookup of existing entries by normalized name
                var existing = new Dictionary<string, ZipFileEntry>(StringComparer.OrdinalIgnoreCase);
                foreach (var e in FastZipDotNet.ZipFileEntries)
                    existing[e.FilenameInZip] = e;

                // Gather candidates to add/replace
                var files = Directory.EnumerateFiles(directoryToSync, "*", SearchOption.AllDirectories).ToList();

                var toProcess = new List<(string fullPath, string internalName, FileInfo info)>();
                int baseLen = directoryToSync.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length;

                foreach (var fullPath in files)
                {
                    var fi = new FileInfo(fullPath);
                    string relative = fullPath.Substring(baseLen).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    string internalName = IOHelpers.NormalizedFilename(relative);

                    bool needsUpdate = true;
                    if (existing.TryGetValue(internalName, out var zfe))
                    {
                        // Simple heuristics: update if size differs or source is newer
                        if ((ulong)fi.Length == zfe.FileSize && fi.LastWriteTime <= zfe.ModifyTime)
                            needsUpdate = false;
                    }

                    if (needsUpdate)
                        toProcess.Add((fullPath, internalName, fi));
                }

                int totalFiles = toProcess.Count;
                long totalUncompressed = toProcess.Sum(t => t.info.Length);

                long processedUncompressed = 0;
                long processedCompressed = 0;
                int filesDone = 0;
                string lastFileName = null;

                FastZipDotNet.SetMaxConcurrency(Math.Max(1, threads <= 0 ? Environment.ProcessorCount : threads));
                var semaphore = FastZipDotNet.ConcurrencyLimiter;

                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
               // var semaphore = new AdjustableSemaphore(threads);
                var tasks = new List<Task>(totalFiles);
                var sw = System.Diagnostics.Stopwatch.StartNew();

                // Periodic progress
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
                            var unc = Interlocked.Read(ref processedUncompressed);
                            var comp = Interlocked.Read(ref processedCompressed);
                            var filesProcessed = Volatile.Read(ref filesDone);

                            var speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;
                            var fps = elapsed.TotalSeconds > 0 ? filesProcessed / elapsed.TotalSeconds : 0.0;

                            progress.Report(new ZipProgress
                            {
                                Operation = ZipOperation.Build,
                                CurrentFile = Volatile.Read(ref lastFileName),

                                TotalFiles = totalFiles,
                                TotalBytesUncompressed = totalUncompressed,
                                TotalBytesCompressed = comp,

                                FilesProcessed = filesProcessed,
                                BytesProcessedUncompressed = unc,
                                BytesProcessedCompressed = comp,

                                Elapsed = elapsed,
                                SpeedBytesPerSec = speed,
                                FilesPerSec = fps
                            });

                            try { await Task.Delay(200, reportCts.Token).ConfigureAwait(false); }
                            catch (OperationCanceledException) { break; }
                        }
                    }, reportCts.Token);
                }

                foreach (var (fullPath, internalName, info) in toProcess)
                {
                    semaphore.WaitOne(linkedCts.Token);
                    var task = Task.Run(() =>
                    {
                        try
                        {
                            void OnUnc(long n) { Interlocked.Add(ref processedUncompressed, n); }
                            void OnComp(long n) { Interlocked.Add(ref processedCompressed, n); Volatile.Write(ref lastFileName, internalName); }

                            AddFileWithProgress(fullPath, internalName, compressionLevel, "", OnUnc, OnComp);

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

                // Final report
                if (progress != null)
                {
                    var elapsed = sw.Elapsed;
                    var unc = totalUncompressed;
                    var comp = Interlocked.Read(ref processedCompressed);
                    var speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;
                    var fps = elapsed.TotalSeconds > 0 ? totalFiles / elapsed.TotalSeconds : 0.0;

                    progress.Report(new ZipProgress
                    {
                        Operation = ZipOperation.Build,
                        CurrentFile = null,

                        TotalFiles = totalFiles,
                        TotalBytesUncompressed = unc,
                        TotalBytesCompressed = comp,

                        FilesProcessed = totalFiles,
                        BytesProcessedUncompressed = unc,
                        BytesProcessedCompressed = comp,

                        Elapsed = elapsed,
                        SpeedBytesPerSec = speed,
                        FilesPerSec = fps
                    });
                }

                reportCts?.Cancel();
                try { await reportingTask.ConfigureAwait(false); } catch { }

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


        private static byte[] ReadAllBytesWinApi(string path, long length)
        {
            if (length <= 0 || length > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be > 0 and <= int.MaxValue for WinAPI fast path.");

            // Open for read; allow other readers/writers; sequential scan hint
            nint handle = WinAPI.CreateFileW(
                path,
                FileAccess.Read,
                FileShare.ReadWrite,
                IntPtr.Zero,
                FileMode.Open,
                (FileAttributes)WinAPI.EFileAttributes.SequentialScan,
                IntPtr.Zero);

            // INVALID_HANDLE_VALUE is -1
            if (handle == nint.Zero || handle == (nint)(-1))
                return null;

            try
            {
                var buffer = new byte[(int)length];
                var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    IntPtr basePtr = gch.AddrOfPinnedObject();
                    int total = 0;

                    // Read in 1 MB chunks (tune if needed)
                    const int chunk = 1 << 20;
                    while (total < buffer.Length)
                    {
                        int toRead = Math.Min(buffer.Length - total, chunk);
                        IntPtr dest = IntPtr.Add(basePtr, total);

                        if (!WinAPI.ReadFile(handle, dest, (uint)toRead, out uint read, IntPtr.Zero))
                        {
                            // Optional: int err = Marshal.GetLastWin32Error();
                            return null; // fall back to FileStream path in caller
                        }

                        if (read == 0)
                        {
                            // EOF reached unexpectedly; shrink buffer to what we got
                            break;
                        }

                        total += (int)read;
                    }

                    if (total != buffer.Length)
                        Array.Resize(ref buffer, total);

                    return buffer;
                }
                finally
                {
                    gch.Free();
                }
            }
            finally
            {
                try { WinAPI.CloseHandle(handle); } catch { /* ignore */ }
            }
        }

        object LockObj = new object();

        public void AddEmptyFolder(string folderNameInZip, string fileComment = "")
        {
            try
            {
                // Ensure directory form ends with '/'
                if (!folderNameInZip.EndsWith("/"))
                    folderNameInZip += "/";

                var zfe = new ZipFileEntry
                {
                    FilenameInZip = IOHelpers.NormalizedFilename(folderNameInZip),
                    Comment = "",
                    Method = Compression.Store,        // no compression
                    EncodeUTF8 = FastZipDotNet.EncodeUTF8,
                    FileSize = 0,
                    CompressedSize = 0,
                    Crc32 = 0,
                    ModifyTime = DateTime.Now,

                    // MS-DOS directory attribute + “directory” bit in external attributes
                    ExternalFileAttr = (uint)((1 << 4) | 0x10),
                };

                // Build local header bytes (no data follows for an empty folder)
                var localHeader = ZipWritersHeaders.BuildLocalHeaderBytes(ref zfe);

                // Reserve space and get absolute offset (this advances AppendOffset atomically)
                long start = FastZipDotNet.Reserve(localHeader.Length);
                zfe.HeaderOffset = (ulong)start;
                zfe.HeaderSize = (ulong)localHeader.Length;

                // Random-access write at reserved offset (same pattern as WriteAt for files)
                using (var fs = new FileStream(
                    FastZipDotNet.ZipFileName,
                    FileMode.OpenOrCreate,
                    FileAccess.Write,
                    FileShare.ReadWrite,
                    Consts.ChunkSize,
                    FileOptions.RandomAccess))
                {
                    fs.Position = start;
                    fs.Write(localHeader, 0, localHeader.Length);
                    fs.Flush();
                }

                // Record in central list
                lock (FastZipDotNet.ZipFileEntries)
                    FastZipDotNet.ZipFileEntries.Add(zfe);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn ZipDataWriter.AddEmptyFolder (reserve‑based)");
            }
        }
    



private const long DefaultBufferThreshold = 16L * 1024 * 1024; // 16 MB


        public long AddFileWithProgress(
string pathFilename,
string filenameInZip,
int compressionLevel,
string fileComment,
Action<long> onUncompressedRead,
Action<long> onCompressedWrite,
long bufferThresholdBytes = DefaultBufferThreshold)
        {
            var fi = new FileInfo(pathFilename);

            // Directory?
            if ((fi.Attributes & FileAttributes.Directory) != 0)
            {
                string folderInZip = IOHelpers.NormalizedFilename(filenameInZip);
                if (!folderInZip.EndsWith("/")) folderInZip += "/";
                AddEmptyFolder(folderInZip, fileComment);
                return 0;
            }

            DateTime modifyTime = fi.LastWriteTime;

            // Zero-length file
            if (fi.Length == 0)
            {
                return AddBuffer(Array.Empty<byte>(),
                                 filenameInZip,
                                 modifyTime,
                                 compressionLevel,
                                 fileComment,
                                 onUncompressedRead,
                                 onCompressedWrite);
            }

            // Large files or RAM-constrained => streaming path
            if (FastZipDotNet.LimitRam || fi.Length > bufferThresholdBytes)
            {
                using var fs = new FileStream(pathFilename,
                                              FileMode.Open,
                                              FileAccess.Read,
                                              FileShare.Read,
                                              Consts.ChunkSize,
                                              FileOptions.SequentialScan);

                return AddStream(fs,
                                 filenameInZip,
                                 modifyTime,
                                 compressionLevel,
                                 fileComment,
                                 onUncompressedRead,
                                 onCompressedWrite);
            }
            else
            {
                //TODO: Optimize
                // Small/medium file => WinAPI fast path to memory, then AddBuffer
                byte[] inBuffer = new byte[fi.Length];
                using var fs = new FileStream(pathFilename,
                                              FileMode.Open,
                                              FileAccess.Read,
                                              FileShare.Read,
                                              1 << 20,
                                              FileOptions.SequentialScan);


                fs.Read(inBuffer, 0, inBuffer.Length);
                //int readTotal = 0;
                //while (readTotal < inBuffer.Length)
                //{
                //    int r = fs.Read(inBuffer, readTotal, inBuffer.Length - readTotal);
                //    if (r <= 0)
                //        throw new EndOfStreamException($"Unexpected end of file while reading '{pathFilename}'.");
                //    readTotal += r;
                //}


                return AddBuffer(inBuffer,
                                 filenameInZip,
                                 modifyTime,
                                 compressionLevel,
                                 fileComment,
                                 onUncompressedRead,
                                 onCompressedWrite);
            }
        }


        ////    public long AddFileWithProgress(
        ////string pathFilename,
        ////string filenameInZip,
        ////int compressionLevel,
        ////string fileComment,
        ////Action<long> onUncompressedRead,
        ////Action<long> onCompressedWrite,
        ////long bufferThresholdBytes = DefaultBufferThreshold)
        ////    {
        ////        var fi = new FileInfo(pathFilename);

        ////        // Directory?
        ////        if ((fi.Attributes & FileAttributes.Directory) != 0)
        ////        {
        ////            string folderInZip = IOHelpers.NormalizedFilename(filenameInZip);
        ////            if (!folderInZip.EndsWith("/")) folderInZip += "/";
        ////            AddEmptyFolder(folderInZip, fileComment);
        ////            return 0;
        ////        }

        ////        DateTime modifyTime = fi.LastWriteTime;

        ////        // Zero-length file
        ////        if (fi.Length == 0)
        ////        {
        ////            return AddBuffer(Array.Empty<byte>(), filenameInZip, modifyTime, compressionLevel, fileComment, onUncompressedRead, onCompressedWrite);
        ////        }

        ////        // Large files or RAM-constrained => streaming path
        ////        if (FastZipDotNet.LimitRam || fi.Length > bufferThresholdBytes)
        ////        {
        ////            using var fs = new FileStream(
        ////                pathFilename,
        ////                FileMode.Open,
        ////                FileAccess.Read,
        ////                FileShare.Read,
        ////                Consts.ChunkSize,
        ////                FileOptions.SequentialScan);

        ////            return AddStream(fs, filenameInZip, modifyTime, compressionLevel, fileComment, onUncompressedRead, onCompressedWrite);
        ////        }
        ////        else
        ////        {
        ////            // Small/medium file: WinAPI fast read to memory
        ////            byte[] inBuffer = ReadAllBytesWinApi(pathFilename, fi.Length);
        ////            if (inBuffer == null)
        ////            {
        ////                // Fallback to FileStream read if WinAPI path fails
        ////                inBuffer = new byte[fi.Length];
        ////                using var fs = new FileStream(pathFilename, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.SequentialScan);
        ////                int readTotal = 0;
        ////                while (readTotal < inBuffer.Length)
        ////                {
        ////                    int r = fs.Read(inBuffer, readTotal, inBuffer.Length - readTotal);
        ////                    if (r <= 0)
        ////                        throw new EndOfStreamException($"Unexpected EOF while reading '{pathFilename}'.");
        ////                    readTotal += r;
        ////                }
        ////            }

        ////            return AddBuffer(inBuffer, filenameInZip, modifyTime, compressionLevel, fileComment, onUncompressedRead, onCompressedWrite);
        ////        }
        ////    }

        //public long AddFileWithProgress(string pathFilename, string filenameInZip, int compressionLevel, string fileComment, Action<long> onUncompressedRead, Action<long> onCompressedWrite)
        //{
        //    using (var fs = new FileStream(pathFilename, FileMode.Open, FileAccess.Read, FileShare.Read, Consts.ChunkSize, FileOptions.SequentialScan))
        //    {
        //        DateTime modifyTime = File.GetLastWriteTime(pathFilename);
        //        return AddStream(fs, filenameInZip, modifyTime, compressionLevel, fileComment, onUncompressedRead, onCompressedWrite);
        //    }
        //}

        public long AddStream(Stream inStream, string filenameInZip, DateTime modifyTime, int compressionLevel, string fileComment = "")
        {
            return AddStream(inStream, filenameInZip, modifyTime, compressionLevel, fileComment, null, null);
        }

        public long AddBuffer(byte[] inBuffer, string filenameInZip, DateTime modifyTime, int compressionLevel, string fileComment = "")
        {
            return AddBuffer(inBuffer, filenameInZip, modifyTime, compressionLevel, fileComment, null, null);
        }

        // Helper Methods
        private void WaitWhilePaused()
        {
            while (FastZipDotNet.Pause)
            {
                Thread.Sleep(50);
            }
        }


        // Remove an entry from the ZipFileEntries list
        public void RemoveEntry(string filenameInZip)
        {
            int index = FastZipDotNet.ZipFileEntries.FindIndex(e => e.FilenameInZip.Equals(filenameInZip, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                FastZipDotNet.ZipFileEntries.RemoveAt(index);
            }
        }

        // Rebuild the zip archive to remove deleted entries
        public void RebuildZip()
        {
            // Create a temporary file
            string tempZipPath = Path.GetTempFileName();
            using (FileStream tempZipStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.ReadWrite))
            {
                // Create a new instance of BrutalZipEngine
                using (FastZipDotNet tempZip = new FastZipDotNet(tempZipStream, FastZipDotNet.MethodCompress))
                {
                    foreach (var entry in FastZipDotNet.ZipFileEntries)
                    {
                        // Extract the file data
                        using (MemoryStream ms = new MemoryStream())
                        {
                            if (FastZipDotNet.ZipDataReader.ExtractFile(entry, ms))
                            {
                                ms.Position = 0;
                                tempZip.ZipDataWriter.AddStream(ms, entry.FilenameInZip, entry.ModifyTime, FastZipDotNet.CompressionLevelValue, entry.Comment);
                            }
                            else
                            {
                                throw new Exception("Failed to extract entry: " + entry.FilenameInZip);
                            }
                        }
                    }
                    tempZip.Close();
                }
            }

            // Replace the original zip file with the new zip file
            FastZipDotNet.ZipFileStream.Dispose();
            File.Delete(FastZipDotNet.ZipFileName);
            File.Move(tempZipPath, FastZipDotNet.ZipFileName);
            // Reopen the zip file
            FastZipDotNet.ZipFileStream = new FileStream(FastZipDotNet.ZipFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 8388608);

            // Reload entries
            FastZipDotNet.ZipFileEntries.Clear();
            if (!ZipReadersInfo.ReadZipInfo(FastZipDotNet.ZipFileStream, ref FastZipDotNet.ZipInfoStruct))
            {
                throw new InvalidDataException("Invalid ZIP file after rebuild.");
            }
        }

    }
}
