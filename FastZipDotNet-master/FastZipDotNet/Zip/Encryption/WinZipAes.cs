
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;

    namespace FastZipDotNet.Zip.Encryption
    {
        public static class WinZipAes
        {
            public static int GetSaltLength(byte strength) => strength switch
            {
                1 => 8,   // AES-128
                2 => 12,  // AES-192
                3 => 16,  // AES-256
                _ => throw new NotSupportedException("Unsupported AES strength")
            };

            public static int GetKeyLength(byte strength) => strength switch
            {
                1 => 16, // 128-bit
                2 => 24, // 192-bit
                3 => 32, // 256-bit
                _ => throw new NotSupportedException("Unsupported AES strength")
            };

            public static (byte[] encKey, byte[] macKey, byte[] pwv) DeriveKeys(
                string password, byte[] salt, byte strength, int iterations = 1000)
            {
                int keyLen = GetKeyLength(strength);
                int totalLen = (keyLen * 2) + 2; // encKey + macKey + 2-byte “password verification”
                using var pbkdf2 = new Rfc2898DeriveBytes(password ?? string.Empty, salt, iterations, HashAlgorithmName.SHA1);
                byte[] keyMaterial = pbkdf2.GetBytes(totalLen);

                var encKey = new byte[keyLen];
                var macKey = new byte[keyLen];
                var pwv = new byte[2];

                Buffer.BlockCopy(keyMaterial, 0, encKey, 0, keyLen);
                Buffer.BlockCopy(keyMaterial, keyLen, macKey, 0, keyLen);
                Buffer.BlockCopy(keyMaterial, keyLen * 2, pwv, 0, 2);

                Array.Clear(keyMaterial, 0, keyMaterial.Length);
                return (encKey, macKey, pwv);
            }
        }

        // AES-CTR for WinZip AE-2:
        // - 128-bit counter, LITTLE-ENDIAN increment
        // - FIRST BLOCK must use counter = 1
        // - Keystream = AES-ECB(counter)
        internal sealed class AesCtr : IDisposable
        {
            private readonly ICryptoTransform _ecb;
            private readonly byte[] _counter = new byte[16];   // starts at 0; we increment BEFORE encrypting
            private readonly byte[] _keystream = new byte[16];
            private int _ksIndex = 16;

            public AesCtr(byte[] key)
            {
                using var aes = Aes.Create();
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;
                aes.Key = key ?? throw new ArgumentNullException(nameof(key));
                _ecb = aes.CreateEncryptor();
                Array.Clear(_counter, 0, _counter.Length); // initial 0; first NextBlock() will inc to 1
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void NextBlock()
            {
                // Increment 128-bit little-endian counter (counter = counter + 1)
                for (int i = 0; i < 16; i++)
                {
                    if (++_counter[i] != 0) break;
                }

                _ecb.TransformBlock(_counter, 0, 16, _keystream, 0);
                _ksIndex = 0;
            }

            // XOR keystream over buffer
            public void XOR(byte[] buffer, int offset, int count)
            {
                int i = 0;
                while (i < count)
                {
                    if (_ksIndex >= 16) NextBlock();
                    int n = Math.Min(16 - _ksIndex, count - i);
                    for (int j = 0; j < n; j++)
                        buffer[offset + i + j] ^= _keystream[_ksIndex + j];
                    _ksIndex += n;
                    i += n;
                }
            }

            public void Dispose()
            {
                try { _ecb?.Dispose(); } catch { }
            }
        }

        // AE-2 decrypt stream: reads salt+pwv, derives keys, then decrypts ciphertext and checks 10-byte tag at the end.
        internal sealed class AesDecryptStream : Stream
        {
            private readonly Stream _base;
            private readonly string _password;
            private readonly byte _strength;
            private readonly long _ciphertextLength; // excludes salt+pwv+tag
            private readonly byte[] _salt;
            private readonly byte[] _pwvExpected;

            private HMACSHA1 _hmac;
            private AesCtr _ctr;

            private long _remaining;
            private bool _init;
            private bool _disposed;

            // baseStream is positioned at the start of SALT (immediately after local header extras)
            public AesDecryptStream(Stream baseStream, string password, byte strength, long totalEncryptedSize)
            {
                _base = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
                _password = password ?? string.Empty;
                _strength = strength;

                int saltLen = WinZipAes.GetSaltLength(strength);
                if (totalEncryptedSize < saltLen + 2 + 10)
                    throw new InvalidDataException("Invalid AES payload size");

                _ciphertextLength = totalEncryptedSize - saltLen - 2 - 10;

                _salt = new byte[saltLen];
                _pwvExpected = new byte[2];

                _remaining = _ciphertextLength;
            }

            private void EnsureInit()
            {
                if (_init) return;

                ReadExact(_base, _salt, 0, _salt.Length);
                ReadExact(_base, _pwvExpected, 0, 2);

                var (encKey, macKey, pwvDerived) = WinZipAes.DeriveKeys(_password, _salt, _strength);

                // Verify password verification value (2 bytes)
                if (pwvDerived[0] != _pwvExpected[0] || pwvDerived[1] != _pwvExpected[1])
                    throw new CryptographicException("Wrong password (AES verify failed).");

                _ctr = new AesCtr(encKey);
                _hmac = new HMACSHA1(macKey);

                Array.Clear(encKey, 0, encKey.Length);
                Array.Clear(macKey, 0, macKey.Length);
                Array.Clear(pwvDerived, 0, pwvDerived.Length);

                _init = true;
            }

            private static void ReadExact(Stream s, byte[] b, int off, int len)
            {
                int read = 0;
                while (read < len)
                {
                    int r = s.Read(b, off + read, len - read);
                    if (r <= 0) throw new EndOfStreamException("Unexpected EOF in AES header");
                    read += r;
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (_disposed) throw new ObjectDisposedException(nameof(AesDecryptStream));
                if (buffer == null) throw new ArgumentNullException(nameof(buffer));
                if ((uint)offset > buffer.Length || (uint)count > buffer.Length - offset)
                    throw new ArgumentOutOfRangeException();

                EnsureInit();
                if (_remaining <= 0)
                {
                    VerifyMacIfNeeded();
                    return 0;
                }

                int toRead = (int)Math.Min(count, _remaining);
                int r = _base.Read(buffer, offset, toRead);
                if (r <= 0) throw new EndOfStreamException("Unexpected EOF in AES payload");

                // Update HMAC with ciphertext then decrypt in place
                _hmac.TransformBlock(buffer, offset, r, null, 0);
                _ctr.XOR(buffer, offset, r);

                _remaining -= r;
                if (_remaining == 0)
                {
                    VerifyMacIfNeeded();
                }
                return r;
            }

            private void VerifyMacIfNeeded()
            {
                if (_remaining != 0) return;

                _hmac.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                byte[] tag = new byte[10];
                ReadExact(_base, tag, 0, 10);

                var mac = _hmac.Hash;
                for (int i = 0; i < 10; i++)
                {
                    if (mac[i] != tag[i])
                        throw new CryptographicException("AES authentication failed (MAC mismatch).");
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (_disposed) return;
                if (disposing)
                {
                    try { _hmac?.Dispose(); } catch { }
                    try { _ctr?.Dispose(); } catch { }
                }
                _disposed = true;
                base.Dispose(disposing);
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => _ciphertextLength;
            public override long Position { get => _ciphertextLength - _remaining; set => throw new NotSupportedException(); }
            public override void Flush() { }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        // Writer-side transform for AES: XORs CTR keystream and updates HMAC with ciphertext
        internal sealed class AesEncryptTransform : IBufferTransform, IDisposable
        {
            private readonly AesCtr _ctr;
            private readonly HMACSHA1 _hmac;

            public AesEncryptTransform(byte[] encKey, byte[] macKey)
            {
                _ctr = new AesCtr(encKey);
                _hmac = new HMACSHA1(macKey);
            }

            // Transform buffer IN PLACE to ciphertext, then update HMAC with ciphertext
            public void Transform(byte[] buffer, int offset, int count)
            {
                _ctr.XOR(buffer, offset, count);
                _hmac.TransformBlock(buffer, offset, count, null, 0);
            }

            public byte[] FinalizeMac()
            {
                _hmac.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                byte[] tag10 = new byte[10];
                Buffer.BlockCopy(_hmac.Hash, 0, tag10, 0, 10);
                return tag10;
            }

            public void Dispose()
            {
                try { _hmac?.Dispose(); } catch { }
                try { _ctr?.Dispose(); } catch { }
            }
        }
    }


