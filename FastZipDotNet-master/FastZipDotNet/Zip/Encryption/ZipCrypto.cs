using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.IO;

namespace FastZipDotNet.Zip.Encryption
{
    internal static class ZipCryptoUtils
    {
        private static readonly uint[] _crcTable = CreateCrcTable();

        private static uint[] CreateCrcTable()
        {
            const uint poly = 0xEDB88320u;
            var table = new uint[256];
            for (uint i = 0; i < 256; i++)
            {
                uint c = i;
                for (int k = 0; k < 8; k++)
                    c = (c & 1) != 0 ? poly ^ (c >> 1) : (c >> 1);
                table[i] = c;
            }
            return table;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Crc32Update(uint crc, byte b)
            => _crcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
    }

    // Traditional PKWARE stream cipher
    public sealed class TraditionalZipCrypto
    {
        private uint key0 = 0x12345678;
        private uint key1 = 0x23456789;
        private uint key2 = 0x34567890;

        public TraditionalZipCrypto(string password)
        {
            password ??= string.Empty;
            foreach (char ch in password)
                UpdateKeys((byte)ch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateKeys(byte b)
        {
            key0 = ZipCryptoUtils.Crc32Update(key0, b);
            key1 = unchecked(key1 + (byte)key0);
            key1 = unchecked(key1 * 134775813 + 1);
            key2 = ZipCryptoUtils.Crc32Update(key2, (byte)(key1 >> 24));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte NextKeyByte()
        {
            uint temp = (key2 | 2u);
            return (byte)((temp * (temp ^ 1u)) >> 8);
        }

        // Decrypt in-place; update keys with plaintext
        public void Decrypt(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                byte c = buffer[offset + i];
                byte k = NextKeyByte();
                byte p = (byte)(c ^ k);
                buffer[offset + i] = p;
                UpdateKeys(p);
            }
        }

        // Encrypt in-place; update keys with plaintext
        public void Encrypt(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                byte p = buffer[offset + i];
                byte k = NextKeyByte();
                byte c = (byte)(p ^ k);
                buffer[offset + i] = c;
                UpdateKeys(p);
            }
        }

        // Generate 12-byte header; last TWO bytes are high-16 bits of CRC-32 (when no data descriptor is used)
        public byte[] GenerateEncryptedHeaderFromCrc(uint crc32)
        {
            var header = new byte[12];
            RandomNumberGenerator.Fill(header.AsSpan(0, 10));
            header[10] = (byte)((crc32 >> 16) & 0xFF);
            header[11] = (byte)((crc32 >> 24) & 0xFF);
            Encrypt(header, 0, 12); // also advances key state for payload
            return header;
        }

        // (For completeness, if you ever emitted data descriptors and had to use modTime):
        public byte[] GenerateEncryptedHeaderFromTime(ushort dosTime)
        {
            var header = new byte[12];
            RandomNumberGenerator.Fill(header.AsSpan(0, 10));
            header[10] = (byte)((dosTime >> 8) & 0xFF);
            header[11] = (byte)(dosTime & 0xFF);
            Encrypt(header, 0, 12);
            return header;
        }
    }

    // Wraps base stream, consumes the 12-byte header then decrypts payloadLength bytes
    internal sealed class ZipCryptoDecryptStream : Stream
    {
        private readonly Stream _base;
        private readonly TraditionalZipCrypto _crypto;
        private readonly long _dataLength;
        private long _remaining;
        private bool _inited;
        private readonly byte _expectedVerifier;

        public ZipCryptoDecryptStream(Stream baseStream, string password, byte expectedVerifier, long encryptedDataLength)
        {
            _base = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            _crypto = new TraditionalZipCrypto(password ?? string.Empty);
            _dataLength = encryptedDataLength;
            _remaining = encryptedDataLength;
            _expectedVerifier = expectedVerifier;
        }

        private void EnsureHeader()
        {
            if (_inited) return;

            Span<byte> hdr = stackalloc byte[12];
            int got = 0;
            while (got < 12)
            {
                int r = _base.Read(hdr.Slice(got));
                if (r <= 0) throw new EndOfStreamException("Unexpected EOF reading ZipCrypto header");
                got += r;
            }

            byte[] header = hdr.ToArray();
            _crypto.Decrypt(header, 0, 12);

            // Spec requires checking at least the last byte; we do that for compat
            if (header[11] != _expectedVerifier)
                throw new CryptographicException("Invalid password (ZipCrypto verifier mismatch).");

            _inited = true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if ((uint)offset > buffer.Length || (uint)count > (buffer.Length - offset))
                throw new ArgumentOutOfRangeException();

            EnsureHeader();
            if (_remaining <= 0) return 0;

            int toRead = (int)Math.Min(count, _remaining);
            int r = _base.Read(buffer, offset, toRead);
            if (r <= 0) return 0;

            _crypto.Decrypt(buffer, offset, r);
            _remaining -= r;
            return r;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _dataLength;
        public override long Position { get => _dataLength - _remaining; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    internal interface IBufferTransform
    {
        void Transform(byte[] buffer, int offset, int count);
    }

    internal sealed class ZipCryptoEncryptTransform : IBufferTransform
    {
        private readonly TraditionalZipCrypto _crypto;
        public ZipCryptoEncryptTransform(TraditionalZipCrypto crypto) => _crypto = crypto;
        public void Transform(byte[] buffer, int offset, int count) => _crypto.Encrypt(buffer, offset, count);
    }
}