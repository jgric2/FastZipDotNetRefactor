using FastZipDotNet.Zip.Helpers;
using FastZipDotNet.Zip.Readers;
using System.Text;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

namespace FastZipDotNet.Zip.Writers
{
    public static class ZipWritersHeaders
    {

        public static byte[] BuildLocalHeaderBytes(ref ZipFileEntry _zfe)
        {
            using var ms = new MemoryStream();

            // Signature PK\003\004
            ms.Write(new byte[] { 0x50, 0x4b, 0x03, 0x04 }, 0, 4);

            bool isDirectory = _zfe.FilenameInZip.EndsWith("/", StringComparison.Ordinal);
            bool zip64Needed = _zfe.FileSize >= uint.MaxValue || _zfe.CompressedSize >= uint.MaxValue;

            // Version needed
            ushort versionNeeded = (ushort)(zip64Needed ? 45 : 20);
            if (_zfe.IsAes) versionNeeded = (ushort)Math.Max(versionNeeded, (ushort)20);
            if (_zfe.Method == Compression.Zstd) versionNeeded = (ushort)Math.Max(versionNeeded, (ushort)63);
            ms.Write(BitConverter.GetBytes(versionNeeded), 0, 2);

            // General purpose flags
            ushort gpFlag = 0;
            if (_zfe.EncodeUTF8) gpFlag |= 0x0800;
            if (_zfe.IsEncrypted) gpFlag |= 0x0001;
            ms.Write(BitConverter.GetBytes(gpFlag), 0, 2);

            // Method (99 for AES)
            ushort methodToWrite = _zfe.IsAes ? (ushort)99 : (ushort)_zfe.Method;
            ms.Write(BitConverter.GetBytes(methodToWrite), 0, 2);

            // DOS time/date
            ms.Write(BitConverter.GetBytes(DosHelpers.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);

            // CRC in local header: 0 for AES; keep 0 for directories as well
            uint crcLocal = (_zfe.IsAes || isDirectory) ? 0u : _zfe.Crc32;
            ms.Write(BitConverter.GetBytes(crcLocal), 0, 4);

            // Sizes in local header
            uint compLocal, uncompLocal;
            if (isDirectory)
            {
                compLocal = 0;
                uncompLocal = 0;
                zip64Needed = false; // dir sizes are zero
            }
            else
            {
                compLocal = zip64Needed ? 0xFFFFFFFFu : (uint)_zfe.CompressedSize;
                uncompLocal = zip64Needed ? 0xFFFFFFFFu : (uint)_zfe.FileSize;
            }
            ms.Write(BitConverter.GetBytes(compLocal), 0, 4);
            ms.Write(BitConverter.GetBytes(uncompLocal), 0, 4);

            // Name
            var encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : EncodingHelper.DefaultEncoding;
            byte[] nameBytes = encoder.GetBytes(_zfe.FilenameInZip);
            ms.Write(BitConverter.GetBytes((ushort)nameBytes.Length), 0, 2);

            // Extra (minimal for dir; no NTFS 0x000A in LOCAL)
            using var msExtra = new MemoryStream();
            using var bwExtra = new BinaryWriter(msExtra);

            if (!isDirectory && zip64Needed)
            {
                bwExtra.Write((ushort)0x0001);
                ushort sz = 0;
                if (_zfe.FileSize >= uint.MaxValue) sz += 8;
                if (_zfe.CompressedSize >= uint.MaxValue) sz += 8;
                bwExtra.Write(sz);
                if (_zfe.FileSize >= uint.MaxValue) bwExtra.Write((ulong)_zfe.FileSize);
                if (_zfe.CompressedSize >= uint.MaxValue) bwExtra.Write((ulong)_zfe.CompressedSize);
            }

            if (_zfe.IsAes)
            {
                bwExtra.Write((ushort)0x9901);
                bwExtra.Write((ushort)7);
                bwExtra.Write((ushort)0x0002);
                bwExtra.Write((ushort)0x4541);
                bwExtra.Write(_zfe.AesStrength == 0 ? (byte)3 : _zfe.AesStrength);
                bwExtra.Write((ushort)((ushort)_zfe.Method));
            }

            // Extended Timestamp 0x5455 (mtime only) — OK in local
            {
                bwExtra.Write((ushort)0x5455);
                bwExtra.Write((ushort)5);
                bwExtra.Write((byte)0x01);
                uint unixMTime = (uint)new DateTimeOffset(_zfe.ModifyTime).ToUnixTimeSeconds();
                bwExtra.Write(unixMTime);
            }

            // DO NOT add NTFS 0x000A to LOCAL for directories (7-Zip is picky). For files it's fine:
            if (!isDirectory)
            {
                bwExtra.Write((ushort)0x000A);
                bwExtra.Write((ushort)32);
                bwExtra.Write(0u);
                bwExtra.Write((ushort)0x0001);
                bwExtra.Write((ushort)24);
                long ft = _zfe.ModifyTime.ToFileTimeUtc();
                bwExtra.Write((ulong)ft);
                bwExtra.Write((ulong)ft);
                bwExtra.Write((ulong)ft);
            }

            byte[] extraBytes = msExtra.ToArray();
            ms.Write(BitConverter.GetBytes((ushort)extraBytes.Length), 0, 2);

            if (nameBytes.Length > 0) ms.Write(nameBytes, 0, nameBytes.Length);
            if (extraBytes.Length > 0) ms.Write(extraBytes, 0, extraBytes.Length);

            _zfe.HeaderSize = (ulong)(30 + nameBytes.Length + extraBytes.Length);
            return ms.ToArray();
        }

        public static void WriteCentralDirRecord(ZipFileEntry _zfe, Stream zipFileStream)
        {
            try
            {
                var encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : EncodingHelper.DefaultEncoding;
                byte[] nameBytes = encoder.GetBytes(_zfe.FilenameInZip);
                byte[] commentBytes = encoder.GetBytes(_zfe.Comment ?? "");

                bool zip64Needed =
                    _zfe.FileSize >= uint.MaxValue ||
                    _zfe.CompressedSize >= uint.MaxValue ||
                    _zfe.HeaderOffset >= uint.MaxValue ||
                    _zfe.DiskNumberStart >= ushort.MaxValue;

                // Signature PK\001\002
                zipFileStream.Write(new byte[] { 0x50, 0x4b, 0x01, 0x02 }, 0, 4);

                // Version made by / needed
                ushort versionMadeBy = (ushort)(zip64Needed ? 45 : 20);
                if (_zfe.Method == Compression.Zstd) versionMadeBy = 63;
                zipFileStream.Write(BitConverter.GetBytes(versionMadeBy), 0, 2);

                ushort versionNeeded = (ushort)(zip64Needed ? 45 : 20);
                if (_zfe.Method == Compression.Zstd) versionNeeded = 63;
                zipFileStream.Write(BitConverter.GetBytes(versionNeeded), 0, 2);

                // Flags
                ushort gpFlag = 0;
                if (_zfe.EncodeUTF8) gpFlag |= 0x0800;
                if (_zfe.IsEncrypted) gpFlag |= 0x0001;
                zipFileStream.Write(BitConverter.GetBytes(gpFlag), 0, 2);

                // Method (99 for AES)
                ushort methodToWrite = _zfe.IsAes ? (ushort)99 : (ushort)_zfe.Method;
                zipFileStream.Write(BitConverter.GetBytes(methodToWrite), 0, 2);

                // DOS date/time
                zipFileStream.Write(BitConverter.GetBytes(DosHelpers.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);

                // Real CRC in central
                zipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);

                // Sizes
                zipFileStream.Write(BitConverter.GetBytes(zip64Needed ? 0xFFFFFFFFu : (uint)_zfe.CompressedSize), 0, 4);
                zipFileStream.Write(BitConverter.GetBytes(zip64Needed ? 0xFFFFFFFFu : (uint)_zfe.FileSize), 0, 4);

                // Name length
                zipFileStream.Write(BitConverter.GetBytes((ushort)nameBytes.Length), 0, 2);

                // Build extra (Zip64, AES, timestamps, NTFS)
                using var msExtra = new MemoryStream();
                using var bwExtra = new BinaryWriter(msExtra);

                if (zip64Needed)
                {
                    bwExtra.Write((ushort)0x0001);
                    ushort dataSize = 0;
                    if (_zfe.FileSize >= uint.MaxValue) dataSize += 8;
                    if (_zfe.CompressedSize >= uint.MaxValue) dataSize += 8;
                    if (_zfe.HeaderOffset >= uint.MaxValue) dataSize += 8;
                    if (_zfe.DiskNumberStart >= ushort.MaxValue) dataSize += 4;
                    bwExtra.Write(dataSize);
                    if (_zfe.FileSize >= uint.MaxValue) bwExtra.Write((ulong)_zfe.FileSize);
                    if (_zfe.CompressedSize >= uint.MaxValue) bwExtra.Write((ulong)_zfe.CompressedSize);
                    if (_zfe.HeaderOffset >= uint.MaxValue) bwExtra.Write((ulong)_zfe.HeaderOffset);
                    if (_zfe.DiskNumberStart >= ushort.MaxValue) bwExtra.Write((uint)_zfe.DiskNumberStart);
                }

                if (_zfe.IsAes)
                {
                    bwExtra.Write((ushort)0x9901);
                    bwExtra.Write((ushort)7);
                    bwExtra.Write((ushort)0x0002);
                    bwExtra.Write((ushort)0x4541);
                    bwExtra.Write(_zfe.AesStrength == 0 ? (byte)3 : _zfe.AesStrength);
                    bwExtra.Write((ushort)((ushort)_zfe.Method));
                }

                // Extended timestamp (mtime)
                {
                    bwExtra.Write((ushort)0x5455);
                    bwExtra.Write((ushort)5);
                    bwExtra.Write((byte)0x01);
                    uint unixMTime = (uint)new DateTimeOffset(_zfe.ModifyTime).ToUnixTimeSeconds();
                    bwExtra.Write(unixMTime);
                }

                // NTFS timestamps in CENTRAL (fine for both files and dirs)
                {
                    bwExtra.Write((ushort)0x000A);
                    bwExtra.Write((ushort)32);
                    bwExtra.Write(0u);
                    bwExtra.Write((ushort)0x0001);
                    bwExtra.Write((ushort)24);
                    long ft = _zfe.ModifyTime.ToFileTimeUtc();
                    bwExtra.Write((ulong)ft);
                    bwExtra.Write((ulong)ft);
                    bwExtra.Write((ulong)ft);
                }

                if (_zfe.Method == Zip.Structure.ZipEntryEnums.Compression.Zstd)
                {
                    bwExtra.Write((ushort)0xE07C);
                    bwExtra.Write((ushort)1);
                    bwExtra.Write((byte)0);
                }

                byte[] extraBytes = msExtra.ToArray();

                // Extra length
                zipFileStream.Write(BitConverter.GetBytes((ushort)extraBytes.Length), 0, 2);

                // Comment length
                zipFileStream.Write(BitConverter.GetBytes((ushort)commentBytes.Length), 0, 2);

                // Disk start
                zipFileStream.Write(BitConverter.GetBytes(zip64Needed ? (ushort)0xFFFF : _zfe.DiskNumberStart), 0, 2);

                // Internal attributes – leave 0 (don’t mark “text”)
                zipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);

                // External attributes – use low DOS attr (0x10 for dir, 0x20 archive for files if set)
                zipFileStream.Write(BitConverter.GetBytes(_zfe.ExternalFileAttr), 0, 4);

                // Rel offset of local header
                zipFileStream.Write(BitConverter.GetBytes(zip64Needed ? 0xFFFFFFFFu : (uint)_zfe.HeaderOffset), 0, 4);

                // Name/extra/comment payloads
                if (nameBytes.Length > 0) zipFileStream.Write(nameBytes, 0, nameBytes.Length);
                if (extraBytes.Length > 0) zipFileStream.Write(extraBytes, 0, extraBytes.Length);
                if (commentBytes.Length > 0) zipFileStream.Write(commentBytes, 0, commentBytes.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn ZipWritersHeaders.WriteCentralDirRecord");
            }
        }


        public static void WriteLocalHeader(ref ZipFileEntry _zfe, Stream zipFileStream)
        {
            try
            {
                Encoding encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : EncodingHelper.DefaultEncoding;
                byte[] encodedFilename = encoder.GetBytes(_zfe.FilenameInZip);

                bool zip64Needed = _zfe.FileSize >= uint.MaxValue || _zfe.CompressedSize >= uint.MaxValue;

                // Write local header signature
                zipFileStream.Write(new byte[] { 0x50, 0x4b, 0x03, 0x04 }, 0, 4); // PK\003\004

                // Version needed to extract
                ushort versionNeeded = (ushort)(zip64Needed ? 45 : 20);
                if (_zfe.Method == Compression.Zstd)
                    versionNeeded = 63; // As per spec, Zstd needs version 6.3 (63 in decimal)
                zipFileStream.Write(BitConverter.GetBytes(versionNeeded), 0, 2);

                // General Purpose Bit Flag
                ushort gpFlag = 0;
                if (_zfe.EncodeUTF8) gpFlag |= 0x0800;
                if (_zfe.IsEncrypted) gpFlag |= 0x0001;
                zipFileStream.Write(BitConverter.GetBytes(gpFlag), 0, 2);

                // Compression Method
                zipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);

                // Last Mod File Time and Date
                zipFileStream.Write(BitConverter.GetBytes(DosHelpers.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);

                // CRC32
                zipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);

                // Compressed size and uncompressed size
                zipFileStream.Write(BitConverter.GetBytes(zip64Needed ? 0xFFFFFFFF : (uint)_zfe.CompressedSize), 0, 4);
                zipFileStream.Write(BitConverter.GetBytes(zip64Needed ? 0xFFFFFFFF : (uint)_zfe.FileSize), 0, 4);

                // Filename length
                zipFileStream.Write(BitConverter.GetBytes((ushort)encodedFilename.Length), 0, 2);

                // Prepare the extra field
                MemoryStream msExtra = new MemoryStream();
                BinaryWriter bwExtra = new BinaryWriter(msExtra);

                if (zip64Needed)
                {
                    // Zip64 Extended Information Extra Field
                    bwExtra.Write((ushort)0x0001); // Header ID
                    bwExtra.Write((ushort)(16));    // Data Size
                    bwExtra.Write((ulong)_zfe.FileSize);       // Uncompressed Size
                    bwExtra.Write((ulong)_zfe.CompressedSize); // Compressed Size
                }

                // Add Zstd extra field if using Zstd compression
                if (_zfe.Method == Compression.Zstd)
                {
                    bwExtra.Write((ushort)0xE07C); // Zstandard extra field ID
                    bwExtra.Write((ushort)(1));    // Size of data (1 byte)
                    bwExtra.Write((byte)0);        // Property byte (set to 0)
                }

                byte[] extraBytes = msExtra.ToArray();

                // Extra field length
                zipFileStream.Write(BitConverter.GetBytes((ushort)extraBytes.Length), 0, 2);

                // Write filename
                zipFileStream.Write(encodedFilename, 0, encodedFilename.Length);

                // Write extra field
                if (extraBytes.Length > 0)
                    zipFileStream.Write(extraBytes, 0, extraBytes.Length);

                _zfe.HeaderSize = (ulong)(30 + encodedFilename.Length + extraBytes.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn ZipWritersHeaders.WriteLocalHeader");
            }
        }

    }
}
