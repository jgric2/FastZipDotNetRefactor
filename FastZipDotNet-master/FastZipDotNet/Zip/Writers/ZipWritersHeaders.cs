using FastZipDotNet.Zip.Helpers;
using FastZipDotNet.Zip.Readers;
using System.Text;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

namespace FastZipDotNet.Zip.Writers
{
    public static class ZipWritersHeaders
    {
        // public static Encoding DefaultEncoding = Encoding.GetEncoding(437);


        //public static void Close(ZipReadersInfo.ZipInfoStruct zipInfo, Stream zipFileStream, string zipComment = "")
        //{
        //    try
        //    {
        //        ulong centralOffset = (ulong)zipFileStream.Position;
        //        ulong centralSize = 0;

        //        // Write central directory records for all entries
        //        foreach (var file in zipInfo.ZipFileEntries)
        //        {
        //            long pos = zipFileStream.Position;
        //            WriteCentralDirRecord(file, zipFileStream);
        //            centralSize += (ulong)(zipFileStream.Position - pos);
        //        }

        //        bool zip64 = centralSize > uint.MaxValue || centralOffset > uint.MaxValue || zipInfo.ZipFileEntries.Count > ushort.MaxValue;

        //        // Record the offset of the Zip64 End of Central Directory Record
        //        ulong zip64EndOfCentralDirOffset = (ulong)zipFileStream.Position;

        //        if (zip64)
        //        {
        //            // Write Zip64 End of Central Directory Record
        //            zipFileStream.Write(new byte[] { 0x50, 0x4b, 0x06, 0x06 }, 0, 4); // PK\006\006
        //            zipFileStream.Write(BitConverter.GetBytes((ulong)44), 0, 8); // Size of Zip64 EOCD record minus 12 bytes
        //            zipFileStream.Write(BitConverter.GetBytes((ushort)45), 0, 2); // Version made by
        //            zipFileStream.Write(BitConverter.GetBytes((ushort)45), 0, 2); // Version needed to extract
        //            zipFileStream.Write(BitConverter.GetBytes((uint)0), 0, 4); // Number of this disk
        //            zipFileStream.Write(BitConverter.GetBytes((uint)0), 0, 4); // Number of the disk with the start
        //            zipFileStream.Write(BitConverter.GetBytes((ulong)zipInfo.ZipFileEntries.Count), 0, 8); // Total entries on this disk
        //            zipFileStream.Write(BitConverter.GetBytes((ulong)zipInfo.ZipFileEntries.Count), 0, 8); // Total entries
        //            zipFileStream.Write(BitConverter.GetBytes(centralSize), 0, 8); // Size of the central directory
        //            zipFileStream.Write(BitConverter.GetBytes(centralOffset), 0, 8); // Offset of central directory

        //            // Write Zip64 End of Central Directory Locator
        //            zipFileStream.Write(new byte[] { 0x50, 0x4b, 0x06, 0x07 }, 0, 4); // PK\006\007
        //            zipFileStream.Write(BitConverter.GetBytes((uint)0), 0, 4); // Number of the disk
        //            zipFileStream.Write(BitConverter.GetBytes(zip64EndOfCentralDirOffset), 0, 8); // Correct offset
        //            zipFileStream.Write(BitConverter.GetBytes((uint)1), 0, 4); // Total number of disks
        //        }

        //        // Write End of Central Directory Record
        //        zipFileStream.Write(new byte[] { 0x50, 0x4b, 0x05, 0x06 }, 0, 4); // PK\005\006
        //        zipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // Number of this disk
        //        zipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // Number of the disk with the start

        //        if (zip64)
        //        {
        //            zipFileStream.Write(BitConverter.GetBytes((ushort)0xFFFF), 0, 2); // Total entries on this disk
        //            zipFileStream.Write(BitConverter.GetBytes((ushort)0xFFFF), 0, 2); // Total entries
        //            zipFileStream.Write(BitConverter.GetBytes((uint)0xFFFFFFFF), 0, 4); // Size of the central directory
        //            zipFileStream.Write(BitConverter.GetBytes((uint)0xFFFFFFFF), 0, 4); // Offset of central directory
        //        }
        //        else
        //        {
        //            zipFileStream.Write(BitConverter.GetBytes((ushort)zipInfo.ZipFileEntries.Count), 0, 2); // Total entries on this disk
        //            zipFileStream.Write(BitConverter.GetBytes((ushort)zipInfo.ZipFileEntries.Count), 0, 2); // Total entries
        //            zipFileStream.Write(BitConverter.GetBytes((uint)centralSize), 0, 4); // Size of the central directory
        //            zipFileStream.Write(BitConverter.GetBytes((uint)centralOffset), 0, 4); // Offset of central directory
        //        }

        //        byte[] encodedComment = Encoding.UTF8.GetBytes(zipInfo.ZipComment);
        //        zipFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2); // Comment length
        //        zipFileStream.Write(encodedComment, 0, encodedComment.Length); // Comment

        //        // Flush and close the stream
        //        if (zipFileStream != null)
        //        {
        //            zipFileStream.Flush();
        //            zipFileStream.Dispose();
        //            zipFileStream = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message + "\r\nIn BrutalZip.Close");
        //    }
        //}



        public static byte[] BuildLocalHeaderBytes(ref ZipFileEntry zfe)
        {
            using (var ms = new MemoryStream())
            {
                WriteLocalHeader(ref zfe, ms);
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
                if (_zfe.EncodeUTF8)
                    gpFlag |= 0x0800; // UTF-8 encoding flag
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
        //private static void WriteCentralDirRecord(ZipFileEntry _zfe, Stream zipFileStream)
        //{
        //    try
        //    {

        //            Encoding encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : EncodingHelper.DefaultEncoding;
        //            byte[] encodedFilename = encoder.GetBytes(_zfe.FilenameInZip);
        //            byte[] encodedComment = encoder.GetBytes(_zfe.Comment);

        //            bool zip64Needed = _zfe.FileSize >= uint.MaxValue || _zfe.CompressedSize >= uint.MaxValue || _zfe.HeaderOffset >= uint.MaxValue;

        //            // Write central file header signature
        //           zipFileStream.Write(new byte[] { 0x50, 0x4b, 0x01, 0x02 }, 0, 4); // PK\001\002

        //            // Version made by (set to 63 for Zstd)
        //            ushort versionMadeBy = (ushort)(zip64Needed ? 45 : 20);
        //            if (_zfe.Method == Compression.Zstd)
        //                versionMadeBy = 63;
        //           zipFileStream.Write(BitConverter.GetBytes(versionMadeBy), 0, 2);

        //            // Version needed to extract
        //            ushort versionNeeded = (ushort)(zip64Needed ? 45 : 20);
        //            if (_zfe.Method == Compression.Zstd)
        //                versionNeeded = 63;
        //           zipFileStream.Write(BitConverter.GetBytes(versionNeeded), 0, 2);

        //            // General Purpose Bit Flag
        //            ushort gpFlag = 0;
        //            if (_zfe.EncodeUTF8)
        //                gpFlag |= 0x0800; // UTF-8 encoding flag
        //           zipFileStream.Write(BitConverter.GetBytes(gpFlag), 0, 2);

        //            // Compression Method
        //           zipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);

        //            // Last Mod File Time and Date
        //           zipFileStream.Write(BitConverter.GetBytes(DosHelpers.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);

        //            // CRC32
        //           zipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);

        //            // Compressed size and uncompressed size
        //           zipFileStream.Write(BitConverter.GetBytes(zip64Needed ? 0xFFFFFFFF : (uint)_zfe.CompressedSize), 0, 4);
        //           zipFileStream.Write(BitConverter.GetBytes(zip64Needed ? 0xFFFFFFFF : (uint)_zfe.FileSize), 0, 4);

        //            // Filename length
        //           zipFileStream.Write(BitConverter.GetBytes((ushort)encodedFilename.Length), 0, 2);

        //            // Prepare the extra field
        //            MemoryStream msExtra = new MemoryStream();
        //            BinaryWriter bwExtra = new BinaryWriter(msExtra);

        //            if (zip64Needed)
        //            {
        //                // Zip64 Extended Information Extra Field
        //                bwExtra.Write((ushort)0x0001); // Header ID
        //                bwExtra.Write((ushort)(28));    // Data Size
        //                bwExtra.Write((ulong)_zfe.FileSize);       // Uncompressed Size
        //                bwExtra.Write((ulong)_zfe.CompressedSize); // Compressed Size
        //                bwExtra.Write((ulong)_zfe.HeaderOffset);   // Relative Header Offset
        //                bwExtra.Write((uint)0);                    // Disk Start Number
        //            }

        //            // Add Zstd extra field if using Zstd compression
        //            if (_zfe.Method == Compression.Zstd)
        //            {
        //                bwExtra.Write((ushort)0xE07C); // Zstandard extra field ID
        //                bwExtra.Write((ushort)(1));    // Size of data (1 byte)
        //                bwExtra.Write((byte)0);        // Property byte (set to 0)
        //            }

        //            byte[] extraBytes = msExtra.ToArray();

        //            // Extra field length
        //           zipFileStream.Write(BitConverter.GetBytes((ushort)extraBytes.Length), 0, 2);

        //            // File comment length
        //           zipFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2);

        //            // Disk number start
        //           zipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);

        //            // Internal file attributes
        //            ushort internalFileAttributes = 0;
        //            if (_zfe.ExternalFileAttr != 0)
        //            {
        //                internalFileAttributes |= 0x01; // Set as read-only if needed
        //            }
        //           zipFileStream.Write(BitConverter.GetBytes(internalFileAttributes), 0, 2);

        //            // **External file attributes**
        //           zipFileStream.Write(BitConverter.GetBytes(_zfe.ExternalFileAttr), 0, 4);

        //            // Relative offset of local header
        //           zipFileStream.Write(BitConverter.GetBytes(zip64Needed ? 0xFFFFFFFF : (uint)_zfe.HeaderOffset), 0, 4);

        //            // Write filename
        //           zipFileStream.Write(encodedFilename, 0, encodedFilename.Length);

        //            // Write extra field
        //            if (extraBytes.Length > 0)
        //               zipFileStream.Write(extraBytes, 0, extraBytes.Length);

        //            // Write file comment
        //            if (encodedComment.Length > 0)
        //               zipFileStream.Write(encodedComment, 0, encodedComment.Length);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message + "\r\nBrutalZip error in WriteCentralDirRecord");
        //    }
        //}




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
                if (_zfe.EncodeUTF8)
                    gpFlag |= 0x0800; // UTF-8 encoding flag
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
