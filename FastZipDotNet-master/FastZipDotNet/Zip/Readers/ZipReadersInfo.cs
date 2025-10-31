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
            public int TotalDisks { get; set; } = 1;
            public List<ZipFileEntry> ZipFileEntries { get; set; } = new List<ZipFileEntry>();
        }

        public static bool ReadZipInfo(Stream zipStream, ref ZipInfoStruct zipInfo)
        {
            try
            {
                if (zipStream.Length < 22) return false;

                long fileSize = zipStream.Length;
                long maxEOCDSearch = Math.Min(fileSize, 0xFFFF + 22);

                zipStream.Seek(fileSize - maxEOCDSearch, SeekOrigin.Begin);

                byte[] buffer = new byte[maxEOCDSearch];
                zipStream.Read(buffer, 0, buffer.Length);

                int pos = buffer.Length - 22;

                for (; pos >= 0; pos--)
                {
                    if (buffer[pos] == 0x50 && buffer[pos + 1] == 0x4b &&
                        buffer[pos + 2] == 0x05 && buffer[pos + 3] == 0x06)
                        break;
                }

                if (pos < 0) return false;

                BinaryReader br = new BinaryReader(new MemoryStream(buffer, pos, buffer.Length - pos));

                uint signature = br.ReadUInt32();
                if (signature != 0x06054b50) return false;

                ZipInfoStruct zipInfoStruct = new ZipInfoStruct();
                zipInfo = zipInfoStruct;

                zipInfoStruct.DiskNumber = br.ReadUInt16();
                zipInfoStruct.CentralDirDisk = br.ReadUInt16();
                zipInfoStruct.EntriesOnThisDisk = br.ReadUInt16();
                zipInfoStruct.TotalEntries = br.ReadUInt16();
                zipInfoStruct.CentralDirSize = br.ReadUInt32();
                zipInfoStruct.CentralDirOffset = br.ReadUInt32();
                ushort commentLength = br.ReadUInt16();

                byte[] zipCommentBytes = br.ReadBytes(commentLength);
                zipInfoStruct.ZipComment = Encoding.UTF8.GetString(zipCommentBytes);

                zipInfoStruct.CentralDirSize64 = zipInfoStruct.CentralDirSize;
                zipInfoStruct.CentralDirOffset64 = zipInfoStruct.CentralDirOffset;
                zipInfoStruct.TotalEntries64 = zipInfoStruct.TotalEntries;
                zipInfoStruct.IsZip64 = false;

                if (zipInfoStruct.CentralDirSize == 0xFFFFFFFF || zipInfoStruct.CentralDirOffset == 0xFFFFFFFF || zipInfoStruct.TotalEntries == 0xFFFF)
                {
                    zipInfoStruct.IsZip64 = true;
                }

                if (zipInfoStruct.IsZip64)
                {
                    int locatorPosInBuffer = pos - 20;
                    if (locatorPosInBuffer < 0)
                        throw new InvalidDataException("ZIP64 End of Central Directory Locator not found where expected.");

                    if (buffer[locatorPosInBuffer] == 0x50 && buffer[locatorPosInBuffer + 1] == 0x4b &&
                        buffer[locatorPosInBuffer + 2] == 0x06 && buffer[locatorPosInBuffer + 3] == 0x07)
                    {
                        using (BinaryReader brLocator = new BinaryReader(new MemoryStream(buffer, locatorPosInBuffer, 20)))
                        {
                            uint locatorSignature = brLocator.ReadUInt32();
                            if (locatorSignature != 0x07064b50) return false;

                            uint diskWithZip64EOCD = brLocator.ReadUInt32();
                            ulong zip64EOCDOffset = brLocator.ReadUInt64();
                            uint totalNumberOfDisks = brLocator.ReadUInt32();

                            zipStream.Seek((long)zip64EOCDOffset, SeekOrigin.Begin);
                            br = new BinaryReader(zipStream, Encoding.Default, true);

                            uint zip64EOCDSignature = br.ReadUInt32();
                            if (zip64EOCDSignature != 0x06064b50) return false;

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
                        throw new InvalidDataException("ZIP64 End of Central Directory Locator not found.");
                    }
                }

                ushort zipFiles = (ushort)zipInfoStruct.TotalEntries64;
                byte[] centralDirImage = new byte[zipInfoStruct.CentralDirSize64];
                zipStream.Seek((long)zipInfoStruct.CentralDirOffset64, SeekOrigin.Begin);
                int bytesRead = 0;
                while (bytesRead < (long)zipInfoStruct.CentralDirSize64)
                {
                    int read = zipStream.Read(centralDirImage, bytesRead, (int)Math.Min(zipInfoStruct.CentralDirSize64 - (ulong)bytesRead, 4096));
                    if (read <= 0) throw new EndOfStreamException("Unexpected end of stream while reading central directory");
                    bytesRead += read;
                }

                int pointer = 0;
                while (pointer < centralDirImage.Length)
                {
                    uint entrySignature = BitConverter.ToUInt32(centralDirImage, pointer);
                    if (entrySignature != 0x02014b50) break;

                    ushort flags = BitConverter.ToUInt16(centralDirImage, pointer + 8);
                    bool encodeUTF8 = (flags & 0x0800) != 0;
                    bool isEncrypted = (flags & 0x0001) != 0;

                    ZipFileEntry zfe = new ZipFileEntry();
                    zfe.EncodeUTF8 = encodeUTF8;
                    zfe.IsEncrypted = isEncrypted;

                    ushort compMethodRaw = BitConverter.ToUInt16(centralDirImage, pointer + 10);
                    zfe.Method = (Compression)compMethodRaw;
                    zfe.ModifyTime = DosHelpers.DosTimeToDateTime(BitConverter.ToUInt32(centralDirImage, pointer + 12));
                    zfe.Crc32 = BitConverter.ToUInt32(centralDirImage, pointer + 16);
                    uint compressedSize = BitConverter.ToUInt32(centralDirImage, pointer + 20);
                    uint uncompressedSize = BitConverter.ToUInt32(centralDirImage, pointer + 24);
                    ushort filenameSize = BitConverter.ToUInt16(centralDirImage, pointer + 28);
                    ushort extraFieldLength = BitConverter.ToUInt16(centralDirImage, pointer + 30);
                    ushort fileCommentLength = BitConverter.ToUInt16(centralDirImage, pointer + 32);
                    ushort diskNumberStart = BitConverter.ToUInt16(centralDirImage, pointer + 34);
                    ushort internalFileAttrs = BitConverter.ToUInt16(centralDirImage, pointer + 36);
                    uint externalFileAttrs = BitConverter.ToUInt32(centralDirImage, pointer + 38);
                    uint relativeOffsetOfLocalHeader = BitConverter.ToUInt32(centralDirImage, pointer + 42);

                    pointer += 46;

                    Encoding encoder = encodeUTF8 ? Encoding.UTF8 : Encoding.GetEncoding(437);
                    zfe.FilenameInZip = encoder.GetString(centralDirImage, pointer, filenameSize);
                    pointer += filenameSize;

                    MemoryStream extraFieldStream = new MemoryStream(centralDirImage, pointer, extraFieldLength, false);
                    BinaryReader extraFieldReader = new BinaryReader(extraFieldStream);

                    ulong uncompressedSize64 = uncompressedSize;
                    ulong compressedSize64 = compressedSize;
                    ulong headerOffset64 = relativeOffsetOfLocalHeader;
                    uint diskNumberStart32 = diskNumberStart;

                    bool aesSeen = false;
                    byte aesStrength = 0;
                    ushort aesVersion = 0;
                    DateTime? tsFromExt = null;

                    if (compressedSize == 0xFFFFFFFF || uncompressedSize == 0xFFFFFFFF || relativeOffsetOfLocalHeader == 0xFFFFFFFF || diskNumberStart == 0xFFFF || extraFieldLength > 0)
                    {
                        long endPos = extraFieldStream.Position + extraFieldLength;
                        while (extraFieldStream.Position < endPos)
                        {
                            ushort headerId = extraFieldReader.ReadUInt16();
                            ushort dataSize = extraFieldReader.ReadUInt16();
                            long next = extraFieldStream.Position + dataSize;

                            if (headerId == 0x0001) // ZIP64
                            {
                                if (uncompressedSize == 0xFFFFFFFF && extraFieldStream.Position + 8 <= next)
                                    uncompressedSize64 = extraFieldReader.ReadUInt64();
                                if (compressedSize == 0xFFFFFFFF && extraFieldStream.Position + 8 <= next)
                                    compressedSize64 = extraFieldReader.ReadUInt64();
                                if (relativeOffsetOfLocalHeader == 0xFFFFFFFF && extraFieldStream.Position + 8 <= next)
                                    headerOffset64 = extraFieldReader.ReadUInt64();
                                if (diskNumberStart == 0xFFFF && extraFieldStream.Position + 4 <= next)
                                    diskNumberStart32 = extraFieldReader.ReadUInt32();

                                extraFieldStream.Position = next;
                            }
                            else if (headerId == 0x9901 && dataSize >= 7) // AES
                            {
                                aesVersion = extraFieldReader.ReadUInt16();
                                ushort vendorId = extraFieldReader.ReadUInt16(); // 'AE'
                                aesStrength = extraFieldReader.ReadByte();
                                ushort actualMethod = extraFieldReader.ReadUInt16();
                                aesSeen = true;
                                zfe.Method = (Compression)actualMethod;
                                extraFieldStream.Position = next;
                            }
                            else if (headerId == 0x5455 && dataSize >= 1) // Extended Timestamp
                            {
                                byte tflags = extraFieldReader.ReadByte();
                                if ((tflags & 0x01) != 0 && dataSize >= 5)
                                {
                                    uint unixMTime = extraFieldReader.ReadUInt32();
                                    try { tsFromExt = DateTimeOffset.FromUnixTimeSeconds(unixMTime).LocalDateTime; } catch { }
                                }
                                extraFieldStream.Position = next;
                            }
                            else if (headerId == 0x000A && dataSize >= 32) // NTFS timestamps
                            {
                                extraFieldReader.ReadUInt32(); // reserved
                                ushort tag = extraFieldReader.ReadUInt16();
                                ushort sz = extraFieldReader.ReadUInt16();
                                if (tag == 0x0001 && sz == 24)
                                {
                                    ulong mft = extraFieldReader.ReadUInt64(); // mtime
                                    extraFieldReader.ReadUInt64(); // atime
                                    extraFieldReader.ReadUInt64(); // ctime
                                    try { tsFromExt = DateTime.FromFileTimeUtc((long)mft).ToLocalTime(); } catch { }
                                }
                                extraFieldStream.Position = next;
                            }
                            else
                            {
                                extraFieldStream.Position = next;
                            }
                        }
                    }

                    pointer += extraFieldLength;

                    zfe.FileSize = uncompressedSize64;
                    zfe.CompressedSize = compressedSize64;
                    zfe.HeaderOffset = headerOffset64;
                    zfe.DiskNumberStart = (ushort)diskNumberStart32;
                    zfe.HeaderSize = 0;
                    zfe.ExternalFileAttr = externalFileAttrs;

                    if (fileCommentLength > 0)
                        zfe.Comment = encoder.GetString(centralDirImage, pointer, fileCommentLength);
                    else
                        zfe.Comment = "";

                    pointer += fileCommentLength;

                    zfe.IsAes = aesSeen;
                    if (aesSeen)
                    {
                        zfe.AesStrength = aesStrength;
                        zfe.AesVersion = aesVersion;
                        zfe.IsEncrypted = true;
                    }

                    if (tsFromExt.HasValue)
                        zfe.ModifyTime = tsFromExt.Value;

                    zipInfoStruct.ZipFileEntries.Add(zfe);
                }

                br.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn ZipReadersInfo.ReadZipInfo");
            }
        }
    }
}
