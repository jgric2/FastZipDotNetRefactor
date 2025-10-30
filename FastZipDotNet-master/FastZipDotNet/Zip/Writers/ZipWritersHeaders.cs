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
            using (var ms = new MemoryStream())
            {
                // Signature
                ms.Write(new byte[] { 0x50, 0x4b, 0x03, 0x04 }, 0, 4);

                bool zip64Needed = _zfe.FileSize >= uint.MaxValue || _zfe.CompressedSize >= uint.MaxValue;

                ushort versionNeeded = (ushort)(zip64Needed ? 45 : 20);
                if (_zfe.Method == Compression.Zstd)
                    versionNeeded = 63;

                // Version needed
                ms.Write(BitConverter.GetBytes(versionNeeded), 0, 2);

                // General purpose bit flag
                ushort gpFlag = 0;
                if (_zfe.EncodeUTF8) gpFlag |= 0x0800;
                if (_zfe.IsEncrypted) gpFlag |= 0x0001;
                ms.Write(BitConverter.GetBytes(gpFlag), 0, 2);

                // Compression method
                ms.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);

                // Last Mod File Time/Date
                ms.Write(BitConverter.GetBytes(DosHelpers.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);

                // CRC32
                ms.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);

                // Compressed/uncompressed sizes
                ms.Write(BitConverter.GetBytes(zip64Needed ? 0xFFFFFFFFu : (uint)_zfe.CompressedSize), 0, 4);
                ms.Write(BitConverter.GetBytes(zip64Needed ? 0xFFFFFFFFu : (uint)_zfe.FileSize), 0, 4);

                // Filename
                var encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : EncodingHelper.DefaultEncoding;
                byte[] encodedFilename = encoder.GetBytes(_zfe.FilenameInZip);
                ms.Write(BitConverter.GetBytes((ushort)encodedFilename.Length), 0, 2);

                // Extra field
                using var msExtra = new MemoryStream();
                using var bwExtra = new BinaryWriter(msExtra);

                if (zip64Needed)
                {
                    bwExtra.Write((ushort)0x0001); // Zip64
                    bwExtra.Write((ushort)16);
                    bwExtra.Write((ulong)_zfe.FileSize);
                    bwExtra.Write((ulong)_zfe.CompressedSize);
                }

                if (_zfe.Method == Compression.Zstd)
                {
                    bwExtra.Write((ushort)0xE07C);
                    bwExtra.Write((ushort)1);
                    bwExtra.Write((byte)0);
                }

                byte[] extraBytes = msExtra.ToArray();

                ms.Write(BitConverter.GetBytes((ushort)extraBytes.Length), 0, 2);

                if (encodedFilename.Length > 0) ms.Write(encodedFilename, 0, encodedFilename.Length);
                if (extraBytes.Length > 0) ms.Write(extraBytes, 0, extraBytes.Length);

                _zfe.HeaderSize = (ulong)(30 + encodedFilename.Length + extraBytes.Length);
                return ms.ToArray();
            }
        }


        public static void WriteCentralDirRecord(ZipFileEntry _zfe, Stream zipFileStream)
        {
            try
            {
                Encoding encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : EncodingHelper.DefaultEncoding;
                byte[] encodedFilename = encoder.GetBytes(_zfe.FilenameInZip);
                byte[] encodedComment = encoder.GetBytes(_zfe.Comment);

                bool zip64Needed = _zfe.FileSize >= uint.MaxValue ||
                                   _zfe.CompressedSize >= uint.MaxValue ||
                                   _zfe.HeaderOffset >= uint.MaxValue ||
                                   _zfe.DiskNumberStart >= ushort.MaxValue;

                // Write central file header signature
                zipFileStream.Write(new byte[] { 0x50, 0x4b, 0x01, 0x02 }, 0, 4); // PK\001\002

                // Version made by (set to 63 for Zstd)
                ushort versionMadeBy = (ushort)(zip64Needed ? 45 : 20);
                if (_zfe.Method == Compression.Zstd)
                    versionMadeBy = 63;
                zipFileStream.Write(BitConverter.GetBytes(versionMadeBy), 0, 2);

                // Version needed to extract
                ushort versionNeeded = (ushort)(zip64Needed ? 45 : 20);
                if (_zfe.Method == Compression.Zstd)
                    versionNeeded = 63;
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
                    ushort dataSize = 0;

                    // Calculate the size of the data in the ZIP64 extra field
                    dataSize += (ushort)((_zfe.FileSize >= uint.MaxValue) ? 8 : 0);
                    dataSize += (ushort)((_zfe.CompressedSize >= uint.MaxValue) ? 8 : 0);
                    dataSize += (ushort)((_zfe.HeaderOffset >= uint.MaxValue) ? 8 : 0);
                    dataSize += (ushort)((_zfe.DiskNumberStart >= ushort.MaxValue) ? 4 : 0);

                    bwExtra.Write(dataSize);

                    if (_zfe.FileSize >= uint.MaxValue)
                    {
                        bwExtra.Write((ulong)_zfe.FileSize); // Uncompressed Size
                    }
                    if (_zfe.CompressedSize >= uint.MaxValue)
                    {
                        bwExtra.Write((ulong)_zfe.CompressedSize); // Compressed Size
                    }
                    if (_zfe.HeaderOffset >= uint.MaxValue)
                    {
                        bwExtra.Write((ulong)_zfe.HeaderOffset); // Relative Header Offset
                    }
                    if (_zfe.DiskNumberStart >= ushort.MaxValue)
                    {
                        bwExtra.Write((uint)_zfe.DiskNumberStart); // Disk Start Number
                    }
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

                // File comment length
                zipFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2);

                // Disk number start
                zipFileStream.Write(BitConverter.GetBytes(zip64Needed ? (ushort)0xFFFF : _zfe.DiskNumberStart), 0, 2);

                // Internal file attributes
                ushort internalFileAttributes = 0;
                if (_zfe.ExternalFileAttr != 0)
                {
                    internalFileAttributes |= 0x01; // Set as read-only if needed
                }
                zipFileStream.Write(BitConverter.GetBytes(internalFileAttributes), 0, 2);

                // External file attributes
                zipFileStream.Write(BitConverter.GetBytes(_zfe.ExternalFileAttr), 0, 4);

                // Relative offset of local header
                zipFileStream.Write(BitConverter.GetBytes(zip64Needed ? 0xFFFFFFFF : (uint)_zfe.HeaderOffset), 0, 4);

                // Write filename
                zipFileStream.Write(encodedFilename, 0, encodedFilename.Length);

                // Write extra field
                if (extraBytes.Length > 0)
                    zipFileStream.Write(extraBytes, 0, extraBytes.Length);

                // Write file comment
                if (encodedComment.Length > 0)
                    zipFileStream.Write(encodedComment, 0, encodedComment.Length);
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
