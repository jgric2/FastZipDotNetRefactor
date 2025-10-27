using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

namespace FastZipDotNet.Zip.Recovery
{
    public static class ZipRepair
    {
        public static async Task<bool> RepairCentralDirectoryAsync(string zipPath, IProgress<ZipProgress> progress, CancellationToken ct = default)
        {
            try
            {
                await Task.Yield();

                using var fs = new FileStream(zipPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 1 << 20, FileOptions.SequentialScan);
                long length = fs.Length;

                var sw = Stopwatch.StartNew();
                long scanned = 0;

                // Report thread
                using var reportCts = new CancellationTokenSource();
                var reportingTask = Task.Factory.StartNew(async () =>
                {
                    while (!reportCts.IsCancellationRequested)
                    {
                        var elapsed = sw.Elapsed;
                        long s = Math.Min(Interlocked.Read(ref scanned), length);
                        double speed = elapsed.TotalSeconds > 0 ? s / elapsed.TotalSeconds : 0.0;

                        progress?.Report(new ZipProgress
                        {
                            Operation = ZipOperation.Repair,
                            CurrentFile = "Scanning...",
                            TotalFiles = 0,
                            TotalBytesUncompressed = length, // treat length as “total to scan”
                            BytesProcessedUncompressed = s,
                            FilesProcessed = 0,
                            Elapsed = elapsed,
                            SpeedBytesPerSec = speed
                        });

                        try { await Task.Delay(200, reportCts.Token).ConfigureAwait(false); }
                        catch (OperationCanceledException) { break; }
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

                // Scan using local header sizes
                bool anyDataDescriptors;
                var entries = ScanLocalHeadersUsingSizes(fs, out anyDataDescriptors, ref scanned, ct);

                reportCts.Cancel();
                try { await reportingTask.ConfigureAwait(false); } catch { }

                if (anyDataDescriptors || entries.Count == 0)
                {
                    // Quick repair cannot handle entries that use data descriptors (unknown sizes).
                    // Return false so caller can fall back to deep repair.
                    return false;
                }

                // Write central directory at EOF
                fs.Seek(0, SeekOrigin.End);
                ulong centralOffset = (ulong)fs.Position;
                ulong centralSize = 0;

                foreach (var e in entries)
                {
                    long start = fs.Position;
                    Writers.ZipWritersHeaders.WriteCentralDirRecord(e, fs);
                    centralSize += (ulong)(fs.Position - start);
                }

                bool zip64Needed = centralOffset >= uint.MaxValue ||
                                   centralSize >= uint.MaxValue ||
                                   entries.Count >= ushort.MaxValue;

                ulong zip64EOCDOffset = (ulong)fs.Position;

                if (zip64Needed)
                {
                    // Zip64 EOCD
                    fs.Write(new byte[] { 0x50, 0x4b, 0x06, 0x06 }, 0, 4);
                    fs.Write(BitConverter.GetBytes((ulong)44), 0, 8);
                    fs.Write(BitConverter.GetBytes((ushort)45), 0, 2);
                    fs.Write(BitConverter.GetBytes((ushort)45), 0, 2);
                    fs.Write(BitConverter.GetBytes((uint)0), 0, 4);
                    fs.Write(BitConverter.GetBytes((uint)0), 0, 4);
                    fs.Write(BitConverter.GetBytes((ulong)entries.Count), 0, 8);
                    fs.Write(BitConverter.GetBytes((ulong)entries.Count), 0, 8);
                    fs.Write(BitConverter.GetBytes(centralSize), 0, 8);
                    fs.Write(BitConverter.GetBytes(centralOffset), 0, 8);

                    // Locator
                    fs.Write(new byte[] { 0x50, 0x4b, 0x06, 0x07 }, 0, 4);
                    fs.Write(BitConverter.GetBytes((uint)0), 0, 4);
                    fs.Write(BitConverter.GetBytes(zip64EOCDOffset), 0, 8);
                    fs.Write(BitConverter.GetBytes((uint)1), 0, 4);
                }

                // EOCD
                fs.Write(new byte[] { 0x50, 0x4b, 0x05, 0x06 }, 0, 4);
                fs.Write(BitConverter.GetBytes((ushort)0), 0, 2);
                fs.Write(BitConverter.GetBytes((ushort)0), 0, 2);

                if (zip64Needed)
                {
                    fs.Write(BitConverter.GetBytes((ushort)0xFFFF), 0, 2);
                    fs.Write(BitConverter.GetBytes((ushort)0xFFFF), 0, 2);
                    fs.Write(BitConverter.GetBytes((uint)0xFFFFFFFF), 0, 4);
                    fs.Write(BitConverter.GetBytes((uint)0xFFFFFFFF), 0, 4);
                }
                else
                {
                    fs.Write(BitConverter.GetBytes((ushort)entries.Count), 0, 2);
                    fs.Write(BitConverter.GetBytes((ushort)entries.Count), 0, 2);
                    fs.Write(BitConverter.GetBytes((uint)centralSize), 0, 4);
                    fs.Write(BitConverter.GetBytes((uint)centralOffset), 0, 4);
                }

                fs.Write(BitConverter.GetBytes((ushort)0), 0, 2); // no comment

                progress?.Report(new ZipProgress
                {
                    Operation = ZipOperation.Repair,
                    CurrentFile = "Central directory rebuilt",
                    TotalFiles = entries.Count,
                    TotalBytesUncompressed = length,
                    BytesProcessedUncompressed = length,
                    FilesProcessed = entries.Count,
                    Elapsed = sw.Elapsed
                });

                return true;
            }
            catch (OperationCanceledException) { return false; }
            catch { return false; }
        }

        public static async Task<bool> RepairToNewArchiveAsync(string damagedZipPath, string repairedZipPath, int compressionLevel, int threads, IProgress<ZipProgress> progress, CancellationToken ct = default)
        {
            // 1) Extract salvageable entries to temp with progress
            string tempDir = Path.Combine(Path.GetTempPath(), "ZipRepair_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            bool okExtract = await RecoverToDirectoryWithProgressAsync(damagedZipPath, tempDir, progress, ct);
            if (!okExtract) { try { Directory.Delete(tempDir, true); } catch { } return false; }

            // 2) Build new archive from temp (with progress)
            using var zip = new FastZipDotNet(repairedZipPath,
                Compression.Deflate, compressionLevel, threads);
            bool okBuild = await zip.AddFilesToArchiveAsync(tempDir, compressionLevel, progress, ct);
            zip.Close();

            try { Directory.Delete(tempDir, true); } catch { }

            return okBuild;
        }

        private static async Task<bool> RecoverToDirectoryWithProgressAsync(string zipPath, string outputDir, IProgress<ZipProgress> progress, CancellationToken ct)
        {
            try
            {
                await Task.Yield();

                using var fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.SequentialScan);
                long totalLen = fs.Length;
                long processed = 0;
                int filesDone = 0;
                var sw = Stopwatch.StartNew();

                // Scan entries (we’ll skip those with data descriptors)
                long dummyScanned = 0;
                bool dd;
                var entries = ScanLocalHeadersUsingSizes(fs, out dd, ref dummyScanned, ct);
                // dd==true means some entries used descriptors (sizes unknown) -> those were skipped by the scanner

                // Reporter
                using var reportCts = new CancellationTokenSource();
                var reportingTask = Task.Factory.StartNew(async () =>
                {
                    while (!reportCts.IsCancellationRequested)
                    {
                        var elapsed = sw.Elapsed;
                        long p = Math.Min(Interlocked.Read(ref processed), totalLen);
                        double speed = elapsed.TotalSeconds > 0 ? p / elapsed.TotalSeconds : 0.0;

                        progress?.Report(new ZipProgress
                        {
                            Operation = ZipOperation.Repair,
                            CurrentFile = "Recovering...",
                            TotalFiles = entries.Count,
                            TotalBytesUncompressed = totalLen,
                            BytesProcessedUncompressed = p,
                            FilesProcessed = filesDone,
                            Elapsed = elapsed,
                            SpeedBytesPerSec = speed
                        });

                        try { await Task.Delay(200, reportCts.Token).ConfigureAwait(false); }
                        catch (OperationCanceledException) { break; }
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

                Directory.CreateDirectory(outputDir);

                int maxDop = Math.Max(1, Environment.ProcessorCount - 1);
                var sem = new SemaphoreSlim(maxDop, maxDop);
                var tasks = new List<Task>(entries.Count);

                foreach (var e in entries)
                {
                    await sem.WaitAsync(ct).ConfigureAwait(false);

                    var task = Task.Run(() =>
                    {
                        try
                        {
                            string outPath = Path.Combine(outputDir, e.FilenameInZip);
                            Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);

                            // Extract using your existing reader (without CRC failures crashing the loop)
                            using var outStream = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20, FileOptions.SequentialScan);

                            // Use your ZipDataReader-like logic, but we’ll re-open the file here:
                            using var zfs = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.SequentialScan);
                            bool ok = ExtractOneEntryFromStream(e, zfs, outStream, n => Interlocked.Add(ref processed, n));
                            if (ok) Interlocked.Increment(ref filesDone);
                        }
                        catch
                        {
                            // skip failures
                        }
                        finally
                        {
                            sem.Release();
                        }
                    }, ct);

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);

                reportCts.Cancel();
                try { await reportingTask.ConfigureAwait(false); } catch { }

                return true;
            }
            catch (OperationCanceledException) { return false; }
            catch { return false; }
        }

        private static bool ExtractOneEntryFromStream(ZipFileEntry zfe, Stream zipStream, Stream outStream, Action<long> onBytes)
        {
            try
            {
                zipStream.Seek((long)zfe.HeaderOffset, SeekOrigin.Begin);
                using var br = new BinaryReader(zipStream, Encoding.Default, leaveOpen: true);

                uint signature = br.ReadUInt32();
                if (signature != 0x04034b50) return false;

                br.ReadUInt16(); // version
                br.ReadUInt16(); // flags
                ushort compMethod = br.ReadUInt16();
                br.ReadUInt16(); // time
                br.ReadUInt16(); // date
                uint crc32 = br.ReadUInt32();
                br.ReadUInt32(); // compSize (we already have)
                br.ReadUInt32(); // uncompSize (we already have)
                ushort nameLen = br.ReadUInt16();
                ushort extraLen = br.ReadUInt16();
                br.ReadBytes(nameLen + extraLen);

                Stream input;
                if (compMethod == (ushort)Compression.Store)
                    input = zipStream;
                else if (compMethod == (ushort)Compression.Deflate)
                    input = new DeflateStream(zipStream, CompressionMode.Decompress, true);
                else if (compMethod == (ushort)Compression.Zstd)
                    input = new ZStd.DecompressionStream(zipStream);
                else
                    return false;

                uint crcCalc = 0;
                byte[] buffer = new byte[32768];
                ulong left = zfe.FileSize;

                while (left > 0)
                {
                    int r = input.Read(buffer, 0, (int)Math.Min((ulong)buffer.Length, left));
                    if (r <= 0) break;

                    crcCalc = Cryptography.Crc32Algorithm.Append(crcCalc, buffer, 0, r);
                    outStream.Write(buffer, 0, r);
                    onBytes?.Invoke(r);
                    left -= (ulong)r;
                }

                outStream.Flush();
                if (compMethod == (ushort)Compression.Deflate || compMethod == (ushort)Compression.Zstd) input.Dispose();

                // If CRC mismatches, we still return false (caller ignores or logs)
                return (crcCalc == zfe.Crc32);
            }
            catch { return false; }
        }


        // C# 12-safe scanner: no stackalloc in async; uses BinaryReader and position math
        private static List<ZipFileEntry> ScanLocalHeadersUsingSizes(Stream fs, out bool anyDataDescriptors, ref long scannedBytes, CancellationToken ct)
        {
            var list = new List<ZipFileEntry>();
            anyDataDescriptors = false;

            long len = fs.Length;
            long pos = 0;
            var br = new BinaryReader(fs, Encoding.Default, true);

            while (pos + 4 <= len)
            {
                ct.ThrowIfCancellationRequested();

                fs.Position = pos;
                uint sig;
                try
                {
                    sig = br.ReadUInt32();
                }
                catch
                {
                    break;
                }

                if (sig != 0x04034b50)
                {
                    pos++;
                    scannedBytes = pos;
                    continue;
                }

                long headerStart = pos;

                ushort versionNeeded = br.ReadUInt16();
                ushort gpFlags = br.ReadUInt16();
                ushort compMethod = br.ReadUInt16();
                uint dosTimeDate = br.ReadUInt32();
                uint crc32 = br.ReadUInt32();
                uint compSize32 = br.ReadUInt32();
                uint uncompSize32 = br.ReadUInt32();
                ushort nameLen = br.ReadUInt16();
                ushort extraLen = br.ReadUInt16();

                byte[] nameBytes = br.ReadBytes(nameLen);
                string name = Helpers.IOHelpers.NormalizedFilename(Encoding.UTF8.GetString(nameBytes));
                byte[] extra = br.ReadBytes(extraLen);

                if ((gpFlags & 0x0008) != 0)
                {
                    anyDataDescriptors = true;
                    break;
                }

                ulong compSize = compSize32;
                ulong uncompSize = uncompSize32;

                if (compSize32 == 0xFFFFFFFF || uncompSize32 == 0xFFFFFFFF)
                {
                    using var ms = new MemoryStream(extra);
                    using var er = new BinaryReader(ms);
                    while (ms.Position < ms.Length)
                    {
                        ushort id = er.ReadUInt16();
                        ushort sz = er.ReadUInt16();
                        if (id == 0x0001)
                        {
                            if (uncompSize32 == 0xFFFFFFFF && ms.Position + 8 <= ms.Length)
                                uncompSize = er.ReadUInt64();
                            else if (uncompSize32 == 0xFFFFFFFF)
                                ms.Seek(sz, SeekOrigin.Current);

                            if (compSize32 == 0xFFFFFFFF && ms.Position + 8 <= ms.Length)
                                compSize = er.ReadUInt64();
                            else if (compSize32 == 0xFFFFFFFF)
                                ms.Seek(sz, SeekOrigin.Current);

                            break;
                        }
                        else
                        {
                            er.ReadBytes(sz);
                        }
                    }
                }

                ulong headerSize = (ulong)(30 + nameLen + extraLen);

                var zfe = new ZipFileEntry
                {
                    FilenameInZip = name,
                    Method = (Compression)compMethod,
                    ModifyTime = Helpers.DosHelpers.DosTimeToDateTime(dosTimeDate),
                    Crc32 = crc32,
                    CompressedSize = compSize,
                    FileSize = uncompSize,
                    HeaderOffset = (ulong)headerStart,
                    HeaderSize = headerSize,
                    EncodeUTF8 = true,
                    DiskNumberStart = 0,
                    ExternalFileAttr = 0
                };

                list.Add(zfe);

                // Jump to next local header
                long next = headerStart + (long)headerSize + (long)compSize;
                if (next <= headerStart || next > len)
                    break;

                pos = next;
                scannedBytes = pos;
            }

            return list;
        }
    }
}