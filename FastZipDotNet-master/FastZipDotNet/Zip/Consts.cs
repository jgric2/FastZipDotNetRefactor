namespace FastZipDotNet.Zip
{
    public static class Consts
    {
        public const int MaxBufferSize = 32 * 1024; // Define max buffer size for reading in chunks
        public const int MaxSizeFileSwitch = 128 * 1024 * 1024; // Define size to switch to large file handling
        public const int ChunkSize = 512 * 1024; // Define chunk size for copying to ZipFileStream

        public const ulong MaxByteArrayLength = 0x7FFFFFC7;
    }
}
