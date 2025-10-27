using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastZipDotNet.Zip.Readers
{
    public class MultiPartStream : Stream
    {
        private readonly FastZipDotNet _fastZip;
        private readonly ulong _startOffset;
        private readonly ulong _length;
        private ulong _position;
        private Stream _currentStream;
        private int _currentDiskNumber;
        private ulong _currentDiskPosition;

        public MultiPartStream(FastZipDotNet fastZip, ulong startOffset, ulong length, int startDiskNumber)
        {
            _fastZip = fastZip;
            _startOffset = startOffset;
            _length = length;
            _position = 0;
            _currentDiskNumber = startDiskNumber;
            _currentDiskPosition = startOffset;

            _currentStream = OpenCurrentStream(_currentDiskNumber, _currentDiskPosition);
        }

        private Stream OpenCurrentStream(int diskNumber, ulong offset)
        {
            string partFileName = _fastZip.GetPartFileName(diskNumber);
            var stream = new FileStream(partFileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            if (offset > 0)
            {
                stream.Seek((long)offset, SeekOrigin.Begin);
            }

            return stream;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_position >= _length)
            {
                return 0; // End of stream
            }

            int remaining = (int)Math.Min(count, (long)(_length - _position));

            int bytesRead = _currentStream.Read(buffer, offset, remaining);
            if (bytesRead == 0)
            {
                // End of current part, move to next disk
                _currentStream.Dispose();
                _currentDiskNumber++;
                _currentDiskPosition = 0; // Start at position 0 in the next disk

                if (_currentDiskNumber >= _fastZip.GetTotalDisks())
                {
                    return 0; // No more disks
                }

                _currentStream = OpenCurrentStream(_currentDiskNumber, _currentDiskPosition);
                return Read(buffer, offset, remaining); // Retry reading
            }

            _position += (ulong)bytesRead;
            _currentDiskPosition += (ulong)bytesRead;
            return bytesRead;
        }

        // Implement other abstract members
        public override bool CanRead => true;
        public override bool CanSeek => false; // Simplified for this implementation
        public override bool CanWrite => false;
        public override long Length => (long)_length;
        public override long Position { get => (long)_position; set => throw new NotSupportedException(); }

        public override void Flush() { } // No action needed

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(); // Not supported

        public override void SetLength(long value) => throw new NotSupportedException(); // Not supported

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException(); // Not supported

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _currentStream?.Dispose();
            }
        }
    }
}
