using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FastZipDotNet.Zip.LibDeflate
{
    public sealed partial class LibDeflateWrapper
    {
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static uint GetCrc32(byte[] buffer)
        {
            IntPtr ptrBuffer = IntPtr.Zero;
            GCHandle pinnedBuffer;

            try
            {
                pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                ptrBuffer = pinnedBuffer.AddrOfPinnedObject();

                uint crc = LibdeflateCrc32(0, ptrBuffer, buffer.Length);

                pinnedBuffer.Free();

                return crc;
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn Libdeflate.Deflate"); }
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static void Libdeflate(byte[] inBuffer, int compressionLevel, bool force, out byte[] outBuffer, out ulong deflatedSize, out uint crc32)
        {
            IntPtr ptrInBuffer = IntPtr.Zero;
            IntPtr ptrOutBuffer = IntPtr.Zero;
            GCHandle pinnedInArray;
            GCHandle pinnedOutArray;
            int maxCompressedSize;
            IntPtr compressor = IntPtr.Zero;
            try
            {
                pinnedInArray = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
                ptrInBuffer = pinnedInArray.AddrOfPinnedObject();

                compressor = LibdeflateAllocCompressor(compressionLevel);
                if (compressor == IntPtr.Zero)
                    throw new Exception("Out of memory");

                crc32 = LibdeflateCrc32(0, ptrInBuffer, inBuffer.Length);

                if (force)
                    maxCompressedSize = LibdeflateDeflateCompressBound(compressor, inBuffer.Length);
                else
                    maxCompressedSize = inBuffer.Length - 1;

                outBuffer = new byte[maxCompressedSize];
                pinnedOutArray = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);
                ptrOutBuffer = pinnedOutArray.AddrOfPinnedObject();

                deflatedSize = LibdeflateDeflateCompress(compressor, ptrInBuffer, inBuffer.Length, ptrOutBuffer, maxCompressedSize);

                LibdeflateFreeCompressor(compressor);
                pinnedInArray.Free();
                pinnedOutArray.Free();
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn Libdeflate.Deflate"); }
        }

        /* ========================================================================== */
        /*                             Compression                                    */
        /* ========================================================================== */

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static IntPtr LibdeflateAllocCompressor(int compressionLevel)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return libdeflate_alloc_compressor_x86(compressionLevel);
                case 8:
                    return libdeflate_alloc_compressor_x64(compressionLevel);
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_alloc_compressor")]
        private static extern IntPtr libdeflate_alloc_compressor_x86(int compressionLevel);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_alloc_compressor")]
        private static extern IntPtr libdeflate_alloc_compressor_x64(int compressionLevel);

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static ulong LibdeflateDeflateCompress(IntPtr compressor, IntPtr inBuffer, int in_nbytes, IntPtr outBuffer, int out_nbytes_avail)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return (ulong)libdeflate_deflate_compress_x86(compressor, inBuffer, (UIntPtr)in_nbytes, outBuffer, (UIntPtr)out_nbytes_avail);
                case 8:
                    return (ulong)libdeflate_deflate_compress_x64(compressor, inBuffer, (UIntPtr)in_nbytes, outBuffer, (UIntPtr)out_nbytes_avail);
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress")]
        private static extern UIntPtr libdeflate_deflate_compress_x86(IntPtr compressor, IntPtr inBuffer, UIntPtr in_nbytes, IntPtr outBuffer, UIntPtr out_nbytes_avail);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress")]
        private static extern UIntPtr libdeflate_deflate_compress_x64(IntPtr compressor, IntPtr inBuffer, UIntPtr in_nbytes, IntPtr outBuffer, UIntPtr out_nbytes_avail);

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static int LibdeflateDeflateCompressBound(IntPtr compressor, int in_nbytes)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return (int)libdeflate_deflate_compress_bound_x86(compressor, (UIntPtr)in_nbytes);
                case 8:
                    return (int)libdeflate_deflate_compress_bound_x64(compressor, (UIntPtr)in_nbytes);
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound")]
        private static extern UIntPtr libdeflate_deflate_compress_bound_x86(IntPtr compressor, UIntPtr in_nbytes);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound")]
        private static extern UIntPtr libdeflate_deflate_compress_bound_x64(IntPtr compressor, UIntPtr in_nbytes);

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static void LibdeflateFreeCompressor(IntPtr compressor)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    libdeflate_free_compressor_x86(compressor);
                    break;
                case 8:
                    libdeflate_free_compressor_x64(compressor);
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_free_compressor")]
        private static extern void libdeflate_free_compressor_x86(IntPtr compressor);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_free_compressor")]
        private static extern void libdeflate_free_compressor_x64(IntPtr compressor);

        /* ========================================================================== */
        /*                                Checksums                                   */
        /* ========================================================================== */

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static UInt32 LibdeflateCrc32(UInt32 crc, IntPtr inBuffer, int len)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return libdeflate_crc32_x86(crc, inBuffer, (UIntPtr)len);
                case 8:
                    return libdeflate_crc32_x64(crc, inBuffer, (UIntPtr)len);
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_crc32")]
        private static extern UInt32 libdeflate_crc32_x86(UInt32 crc, IntPtr inBuffer, UIntPtr len);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_crc32")]
        private static extern UInt32 libdeflate_crc32_x64(UInt32 crc, IntPtr inBuffer, UIntPtr len);
    }
}
