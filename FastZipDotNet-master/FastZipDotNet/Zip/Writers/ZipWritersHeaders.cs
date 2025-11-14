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

            // PK\003\004
            ms.Write(new byte[] { 0x50, 0x4b, 0x03, 0x04 }, 0, 4);

            bool isDirectory = _zfe.FilenameInZip.EndsWith("/", StringComparison.Ordinal);

            // LOCAL header: keep version low, never Zip64 for directories
            ushort versionNeeded = 20;
            if (!isDirectory)
            {
                if (_zfe.FileSize >= uint.MaxValue || _zfe.CompressedSize >= uint.MaxValue)
                    versionNeeded = 45;
                if (_zfe.IsAes)
                    versionNeeded = (ushort)Math.Max(versionNeeded, (ushort)20);
                if (_zfe.Method == Zip.Structure.ZipEntryEnums.Compression.Zstd)
                    versionNeeded = (ushort)Math.Max(versionNeeded, (ushort)63);
            }
            ms.Write(BitConverter.GetBytes(versionNeeded), 0, 2);

            // Flags
            ushort gpFlag = 0;
            if (_zfe.EncodeUTF8) gpFlag |= 0x0800;
            // never set encrypted for directories
            if (!isDirectory && _zfe.IsEncrypted) gpFlag |= 0x0001;
            ms.Write(BitConverter.GetBytes(gpFlag), 0, 2);

            // Method (99 for AES), but for directories always store (0)
            ushort methodToWrite = isDirectory
            ? (ushort)0
            : (_zfe.IsAes ? (ushort)99 : (ushort)_zfe.Method);
            ms.Write(BitConverter.GetBytes(methodToWrite), 0, 2);

            // DOS time/date
            ms.Write(BitConverter.GetBytes(Zip.Helpers.DosHelpers.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);

            // CRC in LOCAL: 0 for AES and for directories
            uint crcLocal = (isDirectory || _zfe.IsAes) ? 0u : _zfe.Crc32;
            ms.Write(BitConverter.GetBytes(crcLocal), 0, 4);

            // Sizes in LOCAL
            uint csizeLocal, usizeLocal;
            if (isDirectory)
            {
                csizeLocal = 0;
                usizeLocal = 0;
            }
            else
            {
                bool zip64 = _zfe.FileSize >= uint.MaxValue || _zfe.CompressedSize >= uint.MaxValue;
                csizeLocal = zip64 ? 0xFFFFFFFFu : (uint)_zfe.CompressedSize;
                usizeLocal = zip64 ? 0xFFFFFFFFu : (uint)_zfe.FileSize;
            }
            ms.Write(BitConverter.GetBytes(csizeLocal), 0, 4);
            ms.Write(BitConverter.GetBytes(usizeLocal), 0, 4);

            // Name
            var encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : Zip.Helpers.EncodingHelper.DefaultEncoding;
            byte[] nameBytes = encoder.GetBytes(_zfe.FilenameInZip);
            ms.Write(BitConverter.GetBytes((ushort)nameBytes.Length), 0, 2);

            // LOCAL extra
            // For directories: NO extra at all; for files keep short, standard extras
            byte[] extraBytes;
            if (isDirectory)
            {
                extraBytes = Array.Empty<byte>();
            }
            else
            {
                using var msExtra = new MemoryStream();
                using var bw = new BinaryWriter(msExtra);

                bool zip64 = _zfe.FileSize >= uint.MaxValue || _zfe.CompressedSize >= uint.MaxValue;
                if (zip64)
                {
                    bw.Write((ushort)0x0001);
                    ushort sz = 0;
                    if (_zfe.FileSize >= uint.MaxValue) sz += 8;
                    if (_zfe.CompressedSize >= uint.MaxValue) sz += 8;
                    bw.Write(sz);
                    if (_zfe.FileSize >= uint.MaxValue) bw.Write((ulong)_zfe.FileSize);
                    if (_zfe.CompressedSize >= uint.MaxValue) bw.Write((ulong)_zfe.CompressedSize);
                }

                if (_zfe.IsAes)
                {
                    bw.Write((ushort)0x9901);
                    bw.Write((ushort)7);
                    bw.Write((ushort)0x0002);
                    bw.Write((ushort)0x4541);
                    bw.Write(_zfe.AesStrength == 0 ? (byte)3 : _zfe.AesStrength);
                    bw.Write((ushort)((ushort)_zfe.Method));
                }

                // Extended timestamp (mtime only)
                bw.Write((ushort)0x5455);
                bw.Write((ushort)5);
                bw.Write((byte)0x01);
                uint unixMTime = (uint)new DateTimeOffset(_zfe.ModifyTime).ToUnixTimeSeconds();
                bw.Write(unixMTime);

                // NTFS 0x000A – keep for files only (not directories)
                if (!_zfe.FilenameInZip.EndsWith("/", StringComparison.Ordinal))
                {
                    bw.Write((ushort)0x000A);
                    bw.Write((ushort)32);
                    bw.Write(0u);
                    bw.Write((ushort)0x0001);
                    bw.Write((ushort)24);
                    long ft = _zfe.ModifyTime.ToFileTimeUtc();
                    bw.Write((ulong)ft);
                    bw.Write((ulong)ft);
                    bw.Write((ulong)ft);
                }

                extraBytes = msExtra.ToArray();
            }

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

                bool isDirectory = _zfe.FilenameInZip.EndsWith("/", StringComparison.Ordinal);

                // Zip64 is needed if ANY central 32-bit field would overflow:
                // sizes (for files) or local header offset (for both files and directories), or disk number
                bool zip64Needed =
                    _zfe.FileSize >= uint.MaxValue ||
                    _zfe.CompressedSize >= uint.MaxValue ||
                    _zfe.HeaderOffset >= uint.MaxValue ||
                    _zfe.DiskNumberStart >= ushort.MaxValue;

                // PK\001\002
                zipFileStream.Write(new byte[] { 0x50, 0x4b, 0x01, 0x02 }, 0, 4);

                // Version made by / needed
                ushort verMade = (ushort)(zip64Needed ? 45 : 20);
                if (_zfe.Method == Zip.Structure.ZipEntryEnums.Compression.Zstd) verMade = 63;
                zipFileStream.Write(BitConverter.GetBytes(verMade), 0, 2);

                ushort verNeed = (ushort)(zip64Needed ? 45 : 20);
                if (_zfe.Method == Zip.Structure.ZipEntryEnums.Compression.Zstd) verNeed = 63;
                zipFileStream.Write(BitConverter.GetBytes(verNeed), 0, 2);

                // Flags
                ushort gpFlag = 0;
                if (_zfe.EncodeUTF8) gpFlag |= 0x0800;
                if (_zfe.IsEncrypted && !isDirectory) gpFlag |= 0x0001;
                zipFileStream.Write(BitConverter.GetBytes(gpFlag), 0, 2);

                // Method (99 for AES; 0 for directories)
                ushort methodToWrite = isDirectory ? (ushort)0 : (_zfe.IsAes ? (ushort)99 : (ushort)_zfe.Method);
                zipFileStream.Write(BitConverter.GetBytes(methodToWrite), 0, 2);

                // DOS time/date
                zipFileStream.Write(BitConverter.GetBytes(DosHelpers.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);

                // CRC in CENTRAL: 0 for directories, real for files
                uint crcCentral = isDirectory ? 0u : _zfe.Crc32;
                zipFileStream.Write(BitConverter.GetBytes(crcCentral), 0, 4);

                // Sizes in CENTRAL (0 for dirs; for files possibly Zip64)
                uint csizeCentral = (!isDirectory && _zfe.CompressedSize >= uint.MaxValue) ? 0xFFFFFFFFu : (uint)_zfe.CompressedSize;
                uint usizeCentral = (!isDirectory && _zfe.FileSize >= uint.MaxValue) ? 0xFFFFFFFFu : (uint)_zfe.FileSize;
                zipFileStream.Write(BitConverter.GetBytes(csizeCentral), 0, 4);
                zipFileStream.Write(BitConverter.GetBytes(usizeCentral), 0, 4);

                // Name length
                zipFileStream.Write(BitConverter.GetBytes((ushort)nameBytes.Length), 0, 2);

                // CENTRAL extra: Zip64 (if needed), AES (files), timestamps (both), NTFS (files only)
                using var msExtra = new MemoryStream();
                using var bw = new BinaryWriter(msExtra);

                if (zip64Needed)
                {
                    bw.Write((ushort)0x0001);

                    // Compute which 64-bit values we must include.
                    // Include only those whose 32-bit fields were set to 0xFFFF/0xFFFFFFFF.
                    ushort dataSize = 0;
                    bool writeUnc = (!isDirectory && _zfe.FileSize >= uint.MaxValue);
                    bool writeCmp = (!isDirectory && _zfe.CompressedSize >= uint.MaxValue);
                    bool writeOff = (_zfe.HeaderOffset >= uint.MaxValue);
                    bool writeDisk = (_zfe.DiskNumberStart >= ushort.MaxValue);

                    if (writeUnc) dataSize += 8;
                    if (writeCmp) dataSize += 8;
                    if (writeOff) dataSize += 8;
                    if (writeDisk) dataSize += 4;

                    bw.Write(dataSize);

                    if (writeUnc) bw.Write((ulong)_zfe.FileSize);
                    if (writeCmp) bw.Write((ulong)_zfe.CompressedSize);
                    if (writeOff) bw.Write((ulong)_zfe.HeaderOffset);
                    if (writeDisk) bw.Write((uint)_zfe.DiskNumberStart);
                }

                if (_zfe.IsAes && !isDirectory)
                {
                    bw.Write((ushort)0x9901);
                    bw.Write((ushort)7);
                    bw.Write((ushort)0x0002);
                    bw.Write((ushort)0x4541);
                    bw.Write(_zfe.AesStrength == 0 ? (byte)3 : _zfe.AesStrength);
                    bw.Write((ushort)((ushort)_zfe.Method));
                }

                // Extended timestamp (mtime)
                bw.Write((ushort)0x5455);
                bw.Write((ushort)5);
                bw.Write((byte)0x01);
                uint unixMTime = (uint)new DateTimeOffset(_zfe.ModifyTime).ToUnixTimeSeconds();
                bw.Write(unixMTime);

                // NTFS 0x000A only for files (skip for directories)
                if (!isDirectory)
                {
                    bw.Write((ushort)0x000A);
                    bw.Write((ushort)32);
                    bw.Write(0u);
                    bw.Write((ushort)0x0001);
                    bw.Write((ushort)24);
                    long ft = _zfe.ModifyTime.ToFileTimeUtc();
                    bw.Write((ulong)ft);
                    bw.Write((ulong)ft);
                    bw.Write((ulong)ft);
                }

                byte[] extraBytes = msExtra.ToArray();

                // Extra len
                zipFileStream.Write(BitConverter.GetBytes((ushort)extraBytes.Length), 0, 2);

                // Comment len
                zipFileStream.Write(BitConverter.GetBytes((ushort)commentBytes.Length), 0, 2);

                // Disk start (Zip64 marker if needed)
                ushort diskStartField = (_zfe.DiskNumberStart >= ushort.MaxValue) ? (ushort)0xFFFF : _zfe.DiskNumberStart;
                zipFileStream.Write(BitConverter.GetBytes(diskStartField), 0, 2);

                // Internal attrs
                zipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);

                // External attrs – DOS 0x10 for dir, etc.
                zipFileStream.Write(BitConverter.GetBytes(_zfe.ExternalFileAttr), 0, 4);

                // Rel offset of local header – 0xFFFFFFFF if Zip64 was used for offset
                uint relOff = (_zfe.HeaderOffset >= uint.MaxValue) ? 0xFFFFFFFFu : (uint)_zfe.HeaderOffset;
                zipFileStream.Write(BitConverter.GetBytes(relOff), 0, 4);

                // payloads
                if (nameBytes.Length > 0) zipFileStream.Write(nameBytes, 0, nameBytes.Length);
                if (extraBytes.Length > 0) zipFileStream.Write(extraBytes, 0, extraBytes.Length);
                if (commentBytes.Length > 0) zipFileStream.Write(commentBytes, 0, commentBytes.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn WriteCentralDirRecord");
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
