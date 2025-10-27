using FastZipDotNet.Zip.Helpers;
using System.Text;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

namespace FastZipDotNet.Zip.Readers
{
    public static class ZipReadersInfo
    {
        public class ZipInfoStruct
        {
            public ushort DiskNumber { get; set; }
            public ushort CentralDirDisk { get; set; }
            public ushort EntriesOnThisDisk { get; set; }
            public ushort TotalEntries { get; set; }
            public uint CentralDirSize { get; set; }
            public uint CentralDirOffset { get; set; }
            public ushort CommentLength { get; set; }

            public string ZipComment { get; set; } = "";

            public bool IsZip64 { get; set; }
            public ulong CentralDirSize64 { get; set; }
            public ulong CentralDirOffset64 { get; set; }
            public ulong TotalEntries64 { get; set; }

            public uint diskWithZip64EOCD { get; set; }
            public ulong zip64EOCDOffset { get; set; }
            public uint totalNumberOfDisks { get; set; }

            public ulong zip64EOCDSize { get; set; }
            public ushort versionMadeBy { get; set; }
            public ushort versionNeeded { get; set; }
            public uint zip64DiskNumber { get; set; }
            public uint zip64CentralDirDisk { get; set; }
            public ulong zip64EntriesOnThisDisk { get; set; }
            public ulong zip64TotalEntries { get; set; }
            public ulong zip64CentralDirSize { get; set; }
            public ulong zip64CentralDirOffset { get; set; }

            public int TotalParts { get; set; } = 1;
            public int CurrentPartNumber { get; set; } = 0;

            //public int CurrentDiskNumber { get; set; } = 0;
            public int TotalDisks { get; set; } = 1;

            public List<ZipFileEntry> ZipFileEntries { get; set; } = new List<ZipFileEntry>();

        }

        // Reads the end-of-central-directory record and entries
        public static bool ReadZipInfo(Stream zipStream, ref ZipInfoStruct zipInfo)
        {
            try
            {
                // Minimum size of the End of Central Directory record (EOCD)
                if (zipStream.Length < 22)
                {
                    return false;
                }

                long fileSize = zipStream.Length;
                // Read the last part of the zipStream into a buffer
                // According to ZIP specification, the EOCD record can be located in the last 64KB + 22 bytes of the file
                long maxEOCDSearch = Math.Min(fileSize, 0xFFFF + 22);

                zipStream.Seek(fileSize - maxEOCDSearch, SeekOrigin.Begin);

                byte[] buffer = new byte[maxEOCDSearch];
                zipStream.Read(buffer, 0, buffer.Length);

                int pos = buffer.Length - 22; // Start from the minimal EOCD size

                // Scan backwards to find the EOCD signature
                for (; pos >= 0; pos--)
                {
                    if (buffer[pos] == 0x50 &&
                        buffer[pos + 1] == 0x4b &&
                        buffer[pos + 2] == 0x05 &&
                        buffer[pos + 3] == 0x06)
                    {
                        // Found EOCD signature
                        break;
                    }
                }

                if (pos < 0)
                {
                    return false; // EOCD not found
                }

                BinaryReader br = new BinaryReader(new MemoryStream(buffer, pos, buffer.Length - pos));

                uint signature = br.ReadUInt32();
                if (signature != 0x06054b50)
                    return false;

                ZipInfoStruct zipInfoStruct = new ZipInfoStruct();
                zipInfo = zipInfoStruct;

                zipInfoStruct.DiskNumber = br.ReadUInt16();
                zipInfoStruct.CentralDirDisk = br.ReadUInt16();
                zipInfoStruct.EntriesOnThisDisk = br.ReadUInt16();
                zipInfoStruct.TotalEntries = br.ReadUInt16();
                zipInfoStruct.CentralDirSize = br.ReadUInt32();
                zipInfoStruct.CentralDirOffset = br.ReadUInt32();
                ushort commentLength = br.ReadUInt16();

                // Read the comment if any
                byte[] zipCommentBytes = br.ReadBytes(commentLength);
                zipInfoStruct.ZipComment = Encoding.UTF8.GetString(zipCommentBytes);

                zipInfoStruct.CentralDirSize64 = zipInfoStruct.CentralDirSize;
                zipInfoStruct.CentralDirOffset64 = zipInfoStruct.CentralDirOffset;
                zipInfoStruct.TotalEntries64 = zipInfoStruct.TotalEntries;

                // Check if ZIP64 EOCD locator is present
                //bool isZip64 = false;
                zipInfoStruct.IsZip64 = false;
                // According to the ZIP specification, if any of these fields are at their max values, ZIP64 is used
                if (zipInfoStruct.CentralDirSize == 0xFFFFFFFF || zipInfoStruct.CentralDirOffset == 0xFFFFFFFF || zipInfoStruct.TotalEntries == 0xFFFF)
                {
                    zipInfoStruct.IsZip64 = true;
                }
                

                if (zipInfoStruct.IsZip64)
                {
                    // Need to find the ZIP64 EOCD locator, which is located before the EOCD record

                    // The ZIP64 EOCD locator is 20 bytes (4 bytes signature + 4 bytes disk number + 8 bytes offset + 4 bytes total disks)
                    int locatorPosInBuffer = pos - 20;

                    if (locatorPosInBuffer < 0)
                    {
                        // Not enough data to read the ZIP64 EOCD locator
                        throw new InvalidDataException("ZIP64 End of Central Directory Locator not found where expected.");
                    }

                    // Verify the signature of the ZIP64 EOCD locator
                    if (buffer[locatorPosInBuffer] == 0x50 &&
                        buffer[locatorPosInBuffer + 1] == 0x4b &&
                        buffer[locatorPosInBuffer + 2] == 0x06 &&
                        buffer[locatorPosInBuffer + 3] == 0x07)
                    {
                        // Read the ZIP64 EOCD locator
                        using (BinaryReader brLocator = new BinaryReader(new MemoryStream(buffer, locatorPosInBuffer, 20)))
                        {
                            uint locatorSignature = brLocator.ReadUInt32();
                            if (locatorSignature != 0x07064b50)
                                return false;

                            uint diskWithZip64EOCD = brLocator.ReadUInt32();
                            ulong zip64EOCDOffset = brLocator.ReadUInt64();
                            uint totalNumberOfDisks = brLocator.ReadUInt32();

                            // Now go to the ZIP64 EOCD record
                            zipStream.Seek((long)zip64EOCDOffset, SeekOrigin.Begin);

                            // Read ZIP64 EOCD record
                            br = new BinaryReader(zipStream,Encoding.Default,true);

                            uint zip64EOCDSignature = br.ReadUInt32();
                            if (zip64EOCDSignature != 0x06064b50)
                                return false;

                            ulong zip64EOCDSize = br.ReadUInt64();
                            ushort versionMadeBy = br.ReadUInt16();
                            ushort versionNeeded = br.ReadUInt16();
                            uint zip64DiskNumber = br.ReadUInt32();
                            uint zip64CentralDirDisk = br.ReadUInt32();
                            ulong zip64EntriesOnThisDisk = br.ReadUInt64();
                            ulong zip64TotalEntries = br.ReadUInt64();
                            ulong zip64CentralDirSize = br.ReadUInt64();
                            ulong zip64CentralDirOffset = br.ReadUInt64();

                            zipInfoStruct.TotalEntries64 = zip64TotalEntries;
                            zipInfoStruct.CentralDirSize64 = zip64CentralDirSize;
                            zipInfoStruct.CentralDirOffset64 = zip64CentralDirOffset;
                        }
                    }
                    else
                    {
                        // ZIP64 EOCD locator not found
                        throw new InvalidDataException("ZIP64 End of Central Directory Locator not found.");
                    }
                }

                // Copy entire central directory to a memory buffer
                ushort zipFiles = (ushort)zipInfoStruct.TotalEntries64;
                byte[] centralDirImage = new byte[zipInfoStruct.CentralDirSize64];
                zipStream.Seek((long)zipInfoStruct.CentralDirOffset64, SeekOrigin.Begin);
                int bytesRead = 0;
                while (bytesRead < (long)zipInfoStruct.CentralDirSize64)
                {
                    int read = zipStream.Read(centralDirImage, bytesRead, (int)Math.Min(zipInfoStruct.CentralDirSize64 - (ulong)bytesRead, 4096));
                    if (read <= 0)
                        throw new EndOfStreamException("Unexpected end of stream while reading central directory");
                    bytesRead += read;
                }

                // Parse central directory entries
                int pointer = 0;
                while (pointer < centralDirImage.Length)
                {
                    uint entrySignature = BitConverter.ToUInt32(centralDirImage, pointer);
                    if (entrySignature != 0x02014b50)
                    {
                        // No more entries
                        break;
                    }

                    bool encodeUTF8 = (BitConverter.ToUInt16(centralDirImage, pointer + 8) & 0x0800) != 0;
                    Encoding encoder = encodeUTF8 ? Encoding.UTF8 : Encoding.GetEncoding(437);

                    ZipFileEntry zfe = new ZipFileEntry();

                    zfe.EncodeUTF8 = encodeUTF8;
                    zfe.Method = (Compression)BitConverter.ToUInt16(centralDirImage, pointer + 10);
                    zfe.ModifyTime = DosHelpers.DosTimeToDateTime(BitConverter.ToUInt32(centralDirImage, pointer + 12));
                    zfe.Crc32 = BitConverter.ToUInt32(centralDirImage, pointer + 16);
                    uint compressedSize = BitConverter.ToUInt32(centralDirImage, pointer + 20);
                    uint uncompressedSize = BitConverter.ToUInt32(centralDirImage, pointer + 24);
                    ushort filenameSize = BitConverter.ToUInt16(centralDirImage, pointer + 28);
                    ushort extraFieldLength = BitConverter.ToUInt16(centralDirImage, pointer + 30);
                    ushort fileCommentLength = BitConverter.ToUInt16(centralDirImage, pointer + 32);
                    ushort diskNumberStart = BitConverter.ToUInt16(centralDirImage, pointer + 34); // Changed to ushort
                    ushort internalFileAttrs = BitConverter.ToUInt16(centralDirImage, pointer + 36);
                    uint externalFileAttrs = BitConverter.ToUInt32(centralDirImage, pointer + 38);
                    uint relativeOffsetOfLocalHeader = BitConverter.ToUInt32(centralDirImage, pointer + 42);

                    pointer += 46;

                    zfe.FilenameInZip = encoder.GetString(centralDirImage, pointer, filenameSize);
                    pointer += filenameSize;

                    // Read extra field
                    MemoryStream extraFieldStream = new MemoryStream(centralDirImage, pointer, extraFieldLength, false);
                    BinaryReader extraFieldReader = new BinaryReader(extraFieldStream);

                    ulong uncompressedSize64 = uncompressedSize;
                    ulong compressedSize64 = compressedSize;
                    ulong headerOffset64 = relativeOffsetOfLocalHeader;
                    uint diskNumberStart32 = diskNumberStart;

                    if (compressedSize == 0xFFFFFFFF || uncompressedSize == 0xFFFFFFFF || relativeOffsetOfLocalHeader == 0xFFFFFFFF || diskNumberStart == 0xFFFF)
                    {
                        // Need to read ZIP64 extra field
                        while (extraFieldStream.Position < extraFieldLength)
                        {
                            ushort headerId = extraFieldReader.ReadUInt16();
                            ushort dataSize = extraFieldReader.ReadUInt16();
                            if (headerId == 0x0001)
                            {
                                // ZIP64 Extended Information Extra Field
                                // The order of fields in the ZIP64 extra field is:
                                // [Uncompressed Size] [Compressed Size] [Relative Header Offset] [Disk Start Number]

                                if (uncompressedSize == 0xFFFFFFFF)
                                {
                                    uncompressedSize64 = extraFieldReader.ReadUInt64();
                                }
                                if (compressedSize == 0xFFFFFFFF)
                                {
                                    compressedSize64 = extraFieldReader.ReadUInt64();
                                }
                                if (relativeOffsetOfLocalHeader == 0xFFFFFFFF)
                                {
                                    headerOffset64 = extraFieldReader.ReadUInt64();
                                }
                                if (diskNumberStart == 0xFFFF)
                                {
                                    diskNumberStart32 = extraFieldReader.ReadUInt32();
                                }
                                // Skip any remaining data in this extra field
                                extraFieldReader.BaseStream.Seek(dataSize - (extraFieldReader.BaseStream.Position - 4), SeekOrigin.Current);
                            }
                            else
                            {
                                // Skip over non-ZIP64 extra fields
                                extraFieldReader.ReadBytes(dataSize);
                            }
                        }
                    }

                    pointer += extraFieldLength;

                    zfe.FileSize = uncompressedSize64;
                    zfe.CompressedSize = compressedSize64;
                    zfe.HeaderOffset = headerOffset64;
                    zfe.DiskNumberStart = (ushort)diskNumberStart32; // Assigning DiskNumberStart
                    zfe.HeaderSize = 0; // Will be set later if needed
                    zfe.ExternalFileAttr = externalFileAttrs;

                    // Read file comment
                    if (fileCommentLength > 0)
                    {
                        zfe.Comment = encoder.GetString(centralDirImage, pointer, fileCommentLength);
                    }
                    else
                    {
                        zfe.Comment = "";
                    }

                    pointer += fileCommentLength;

                    // Add the entry to the list
                    zipInfoStruct.ZipFileEntries.Add(zfe);
                }

                br.Dispose();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn BrutalUnZip.ReadZipInfo");
            }
        }
    }
}
