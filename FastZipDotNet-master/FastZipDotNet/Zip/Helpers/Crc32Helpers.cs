using FastZipDotNet.Zip.Cryptography;

namespace FastZipDotNet.Zip.Helpers
{
    public static class Crc32Helpers
    {
   
        public static uint ComputeCrc32(Stream stream)
        {
            uint result = 0;

            // Ensure the stream position is at the beginning
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            byte[] buffer = new byte[Consts.MaxBufferSize];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            result = Crc32Algorithm.Compute(buffer, 0, bytesRead);

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                result = Crc32Algorithm.Append(result, buffer, 0, bytesRead);
            }

            // Reset the stream position to beginning for subsequent operations
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            return result;
        }

        public static uint ComputeCrc32(byte[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            uint result = Crc32Algorithm.Compute(array);
            return result;
        }
    }
}
