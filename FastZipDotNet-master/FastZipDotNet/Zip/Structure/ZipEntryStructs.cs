using static FastZipDotNet.Zip.Structure.ZipEntryEnums;

namespace FastZipDotNet.Zip.Structure
{
    public class ZipEntryStructs
    {
        public struct ZipFileEntry
        {
            public Compression Method;
            public string FilenameInZip;
            public ulong FileSize;
            public ulong CompressedSize;
            public ulong HeaderOffset;
            public ulong HeaderSize;
            public uint Crc32;
            public DateTime ModifyTime;
            public string Comment;
            public bool EncodeUTF8;
            public uint ExternalFileAttr;
            public ushort DiskNumberStart; // New field to track the part (disk) number

            // NEW: encryption metadata
            public bool IsEncrypted;    // central flag bit 0 set
            public bool IsAes;          // AES extra field 0x9901 present
            public byte AesStrength;    // 1=128,2=192,3=256
            public ushort AesVersion;   // usually 0x0002 for AE-2

            public override string ToString()
            {
                return this.FilenameInZip;
            }
        }
    }
}
