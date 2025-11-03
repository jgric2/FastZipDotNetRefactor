using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace FastZipDotNet.MultiPart
{
    public enum SegmentScheme
    {
        Conventional = 0, // file.z01 … file.zip (requires split signature PK00 at start of new parts)
        Dotted = 1,       // file.zip.001 … (does not require split signature)
    }

    public static class SegmentName
    {
        public static void SplitBase(string zipPath, out string directory, out string baseNameNoExt)
        {
            directory = Path.GetDirectoryName(zipPath) ?? ".";
            baseNameNoExt = Path.GetFileNameWithoutExtension(zipPath);
        }

        public static string GetConventionalPath(string zipPath, int disk, int? totalDisksIfKnown = null, bool preferZipForLast = false)
        {
            SplitBase(zipPath, out var dir, out var name);
            if (preferZipForLast && totalDisksIfKnown.HasValue && disk == totalDisksIfKnown.Value - 1)
                return Path.Combine(dir, name + ".zip");
            return Path.Combine(dir, name + ".z" + (disk + 1).ToString("00", CultureInfo.InvariantCulture));
        }

        public static string GetDottedPath(string zipPath, int disk, int digits, int startIndex)
        {
            int index = startIndex + disk;
            string suffix = index.ToString(new string('0', Math.Max(1, digits)), CultureInfo.InvariantCulture);
            return zipPath + "." + suffix;
        }

        public static string GetDiskPathForScheme(string zipPath, SegmentScheme scheme, int disk, int? totalDisksIfKnown, bool preferZipForLastConventional, int dottedDigits, int dottedStartIndex)
        {
            if (scheme == SegmentScheme.Conventional)
                return GetConventionalPath(zipPath, disk, totalDisksIfKnown, preferZipForLastConventional);
            else
                return GetDottedPath(zipPath, disk, dottedDigits, dottedStartIndex);
        }

        public static string GetLastDiskPathAsZip(string zipPath)
        {
            SplitBase(zipPath, out var dir, out var name);
            return Path.Combine(dir, name + ".zip");
        }

        public static void DetectExisting(string zipPath, out SegmentScheme scheme, out int totalDisks, out int dottedDigits, out int dottedStartIndex)
        {
            scheme = SegmentScheme.Conventional;
            totalDisks = 0;
            dottedDigits = 3;
            dottedStartIndex = 1;

            SplitBase(zipPath, out var dir, out var name);

            int countConv = 0;
            for (int i = 1; i < 1000; i++)
            {
                string znn = Path.Combine(dir, name + ".z" + i.ToString("00", CultureInfo.InvariantCulture));
                if (!File.Exists(znn)) break;
                countConv++;
            }
            bool convHasZip = File.Exists(GetLastDiskPathAsZip(zipPath));
            int totalConv = (countConv > 0 && convHasZip) ? countConv + 1 : (convHasZip ? 1 : 0);

            int bestDotTotal = 0;
            int bestDigits = 3;
            int startIndex = 1;
            for (int d = 2; d <= 6; d++)
            {
                int c = 0;
                for (int i = 0; i < 1000; i++)
                {
                    string p = GetDottedPath(zipPath, i, d, startIndex);
                    if (!File.Exists(p)) break;
                    c++;
                }
                if (c > bestDotTotal)
                {
                    bestDotTotal = c;
                    bestDigits = d;
                }
            }

            if (totalConv == 0 && bestDotTotal == 0)
            {
                if (File.Exists(GetLastDiskPathAsZip(zipPath)))
                {
                    scheme = SegmentScheme.Conventional;
                    totalDisks = 1;
                }
                else
                {
                    scheme = SegmentScheme.Conventional;
                    totalDisks = 0;
                }
                return;
            }

            if (totalConv >= bestDotTotal)
            {
                scheme = SegmentScheme.Conventional;
                totalDisks = Math.Max(totalConv, 1);
            }
            else
            {
                scheme = SegmentScheme.Dotted;
                totalDisks = Math.Max(bestDotTotal, 1);
                dottedDigits = bestDigits;
                dottedStartIndex = startIndex;
            }
        }

        public static long ComputePartSize(string zipPath, SegmentScheme scheme, int totalDisks, int dottedDigits, int dottedStartIndex)
        {
            try
            {
                if (totalDisks <= 1) return 0;
                if (scheme == SegmentScheme.Conventional)
                {
                    string p1 = GetConventionalPath(zipPath, 0);
                    if (File.Exists(p1)) return new FileInfo(p1).Length;
                }
                else
                {
                    string p1 = GetDottedPath(zipPath, 0, dottedDigits, dottedStartIndex);
                    if (File.Exists(p1)) return new FileInfo(p1).Length;
                }
            }
            catch { }
            return 0;
        }
    }

    public sealed class SegmentedFileWriter : IDisposable
    {
        private readonly string _zipPath;
        private readonly long _partSize;
        private readonly SegmentScheme _scheme;
        private readonly bool _appendExisting;
        private readonly int _existingTotalDisks;
        private readonly int _dottedDigits;
        private readonly int _dottedStartIndex;

        private readonly ConcurrentDictionary<int, FileStream> _open = new ConcurrentDictionary<int, FileStream>();
        private readonly ConcurrentDictionary<int, int> _diskBaseOffsets = new ConcurrentDictionary<int, int>(); // 0 or 4 (split sig)

        private long _maxWrittenOffset = 0;

        public SegmentedFileWriter(string zipPath, long partSize, SegmentScheme scheme, bool appendExisting, int existingTotalDisks, int dottedDigits, int dottedStartIndex)
        {
            if (partSize <= 0) throw new ArgumentOutOfRangeException(nameof(partSize), "PartSize must be > 0.");
            _zipPath = zipPath ?? throw new ArgumentNullException(nameof(zipPath));
            _partSize = partSize;
            _scheme = scheme;
            _appendExisting = appendExisting;
            _existingTotalDisks = existingTotalDisks;
            _dottedDigits = Math.Max(1, dottedDigits);
            _dottedStartIndex = Math.Max(1, dottedStartIndex);
        }

        public long PartSize => _partSize;

        public void SetInitialMaxOffset(long offset)
        {
            Interlocked.Exchange(ref _maxWrittenOffset, offset);
        }

        private int DiskFromOffset(long offset) => (int)(offset / _partSize);
        private long LocalOffsetInDisk(long globalOffset) => globalOffset % _partSize;

        private string ResolveDiskPathForWrite(int disk)
        {
            if (_scheme == SegmentScheme.Conventional)
            {
                if (_appendExisting && _existingTotalDisks > 0)
                {
                    if (disk == _existingTotalDisks - 1)
                        return SegmentName.GetLastDiskPathAsZip(_zipPath);
                    return SegmentName.GetConventionalPath(_zipPath, disk);
                }
                else
                {
                    return SegmentName.GetConventionalPath(_zipPath, disk);
                }
            }
            else
            {
                return SegmentName.GetDottedPath(_zipPath, disk, _dottedDigits, _dottedStartIndex);
            }
        }

        private int EnsureSplitSignatureIfNeeded(int disk, FileStream fs)
        {
            // Conventional scheme: write 4-byte PK00 at the start of parts with disk > 0
            // If we've already opened this disk and written the split sig, return cached base offset
            if (_diskBaseOffsets.TryGetValue(disk, out var existing)) return existing;

            if (_scheme == SegmentScheme.Conventional && disk > 0)
            {
                if (fs.Length == 0)
                {
                    // write PK00 (0x50 0x4B 0x30 0x30)
                    byte[] sig = new byte[] { 0x50, 0x4B, 0x30, 0x30 };
                    fs.Position = 0;
                    fs.Write(sig, 0, 4);
                    fs.Flush();
                    _diskBaseOffsets[disk] = 4;
                    return 4;
                }
            }

            _diskBaseOffsets[disk] = 0;
            return 0;
        }

        public int GetDiskBaseOffset(int disk)
        {
            if (_diskBaseOffsets.TryGetValue(disk, out var b)) return b;
            return 0;
        }

        private FileStream GetStreamForDisk(int disk)
        {
            return _open.GetOrAdd(disk, d =>
            {
                string path = ResolveDiskPathForWrite(d);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 1 << 20, FileOptions.RandomAccess);
                // Make sure split signature is in place (for conventional scheme)
                EnsureSplitSignatureIfNeeded(d, fs);
                return fs;
            });
        }

        public void WriteAt(byte[] buffer, int offsetInBuffer, int count, long globalOffset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (count <= 0) return;

            int remaining = count;
            int srcOff = offsetInBuffer;
            long dstOff = globalOffset;

            while (remaining > 0)
            {
                int disk = DiskFromOffset(dstOff);
                long local = LocalOffsetInDisk(dstOff);
                long cap = _partSize - local;
                int toWrite = (int)Math.Min(remaining, cap);

                var fs = GetStreamForDisk(disk);

                int baseOff = EnsureSplitSignatureIfNeeded(disk, fs);
                lock (fs)
                {
                    fs.Position = local + baseOff;
                    fs.Write(buffer, srcOff, toWrite);
                    fs.Flush();
                }

                srcOff += toWrite;
                remaining -= toWrite;
                dstOff += toWrite;
            }

            long endOffset = globalOffset + count;
            long prev;
            do
            {
                prev = Interlocked.Read(ref _maxWrittenOffset);
                if (endOffset <= prev) break;
            }
            while (Interlocked.CompareExchange(ref _maxWrittenOffset, endOffset, prev) != prev);
        }

        public void WriteAt(byte[] buffer, long globalOffset) => WriteAt(buffer, 0, buffer.Length, globalOffset);

        public Stream OpenAppendStream(long startOffset)
        {
            return new SegmentedAppendStream(this, startOffset);
        }

        public int TotalDisksUsed
        {
            get
            {
                long max = Interlocked.Read(ref _maxWrittenOffset);
                if (max <= 0) return Math.Max(_existingTotalDisks, 1);
                int disks = (int)((max - 1) / _partSize) + 1;
                if (_appendExisting && _existingTotalDisks > 0)
                    return Math.Max(disks, _existingTotalDisks);
                return Math.Max(disks, 1);
            }
        }

        public void FinalizeSegments()
        {
            foreach (var kv in _open)
            {
                try { kv.Value.Flush(); kv.Value.Dispose(); } catch { }
            }
            _open.Clear();

            if (_scheme == SegmentScheme.Conventional && !_appendExisting)
            {
                int total = TotalDisksUsed;
                if (total <= 0) total = 1;

                string lastZnn = SegmentName.GetConventionalPath(_zipPath, total - 1);
                string finalZip = SegmentName.GetLastDiskPathAsZip(_zipPath);

                try
                {
                    if (File.Exists(finalZip))
                    {
                        try { File.Delete(finalZip); } catch { }
                    }
                    if (File.Exists(lastZnn))
                        File.Move(lastZnn, finalZip);
                }
                catch { }
            }
        }

        public void Dispose()
        {
            foreach (var kv in _open)
            {
                try { kv.Value.Flush(); kv.Value.Dispose(); } catch { }
            }
            _open.Clear();
        }

        public sealed class SegmentedAppendStream : Stream
        {
            private readonly SegmentedFileWriter _owner;
            private long _pos;

            public SegmentedAppendStream(SegmentedFileWriter owner, long start)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _pos = start;
            }

            public override bool CanRead => false;
            public override bool CanSeek => true;
            public override bool CanWrite => true;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => _pos; set => _pos = value; }

            public override void Flush() { }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override long Seek(long offset, SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin: _pos = offset; break;
                    case SeekOrigin.Current: _pos += offset; break;
                    case SeekOrigin.End: throw new NotSupportedException();
                }
                return _pos;
            }

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count)
            {
                _owner.WriteAt(buffer, offset, count, _pos);
                _pos += count;
            }
        }
    }

    public sealed class MultiPartReadStream : Stream
    {
        private readonly string _zipPath;
        private readonly int _totalDisks;
        private readonly SegmentScheme _scheme;
        private readonly int _dottedDigits;
        private readonly int _dottedStartIndex;

        private int _curDisk;
        private long _curDiskPos;
        private FileStream _curFs;

        public MultiPartReadStream(string zipPath, int totalDisks, int startDisk, long startOffsetInDisk, SegmentScheme scheme, int dottedDigits, int dottedStartIndex)
        {
            _zipPath = zipPath ?? throw new ArgumentNullException(nameof(zipPath));
            _totalDisks = totalDisks > 0 ? totalDisks : 1;
            _scheme = scheme;
            _dottedDigits = Math.Max(1, dottedDigits);
            _dottedStartIndex = Math.Max(1, dottedStartIndex);
            _curDisk = startDisk;
            _curDiskPos = startOffsetInDisk; // physical offset (including 4-byte sig for conventional if present)
            OpenDisk();
        }

        private void OpenDisk()
        {
            CloseDisk();

            string path;
            if (_scheme == SegmentScheme.Conventional)
            {
                path = (_curDisk == _totalDisks - 1)
                    ? SegmentName.GetLastDiskPathAsZip(_zipPath)
                    : SegmentName.GetConventionalPath(_zipPath, _curDisk);
            }
            else
            {
                path = SegmentName.GetDottedPath(_zipPath, _curDisk, _dottedDigits, _dottedStartIndex);
            }

            _curFs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 1 << 20, FileOptions.SequentialScan);

            if (_curDiskPos > 0)
            {
                if (_curDiskPos > _curFs.Length)
                    throw new EndOfStreamException("Offset beyond segment length.");
                _curFs.Seek(_curDiskPos, SeekOrigin.Begin);
            }
        }

        private void CloseDisk()
        {
            if (_curFs != null)
            {
                try { _curFs.Dispose(); } catch { }
                _curFs = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) CloseDisk();
            base.Dispose(disposing);
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count <= 0) return 0;
            int total = 0;
            while (count > 0)
            {
                int r = _curFs.Read(buffer, offset, count);
                if (r > 0)
                {
                    total += r;
                    offset += r;
                    count -= r;
                    _curDiskPos += r;
                    continue;
                }
                if (_curDisk + 1 >= _totalDisks) break;
                _curDisk++;
                _curDiskPos = 0;
                OpenDisk();
            }
            return total;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}