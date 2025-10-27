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
     

            public override string ToString()
            {
                return this.FilenameInZip;
            }
        }
    }
}
