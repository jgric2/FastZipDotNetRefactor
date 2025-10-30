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

        public enum EncryptionAlgorithm
        {
            None = 0,
            ZipCrypto = 1,
            Aes128 = 2,
            Aes192 = 3,
            Aes256 = 4
        }
    }
}
