namespace FastZipDotNet.Zip.Structure
{
    public static class ZipEntryEnums
    {
        public enum Compression : ushort
        {
            Store = 0,
            Deflate = 8,
            Zstd = 93
        }
    }
}
