using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FastZipDotNet.Zip.LibDeflate
{
    public sealed class LibDeflateWrapper
    {
        /* ========================================================================== */
        /*                           High-level helpers                               */
        /* ========================================================================== */

        // Original helper used by your AddBuffer path. Keeps your existing calls working.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Libdeflate(byte[] inBuffer, int compressionLevel, bool force, out byte[] outBuffer, out ulong deflatedSize, out uint crc32)
        {
            IntPtr ptrInBuffer = IntPtr.Zero;
            IntPtr ptrOutBuffer = IntPtr.Zero;
            GCHandle pinnedInArray = default;
            GCHandle pinnedOutArray = default;

            int maxCompressedSize;
            IntPtr compressor = IntPtr.Zero;

            try
            {
                if (inBuffer == null) inBuffer = Array.Empty<byte>();

                pinnedInArray = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
                ptrInBuffer = pinnedInArray.AddrOfPinnedObject();

                compressor = LibdeflateAllocCompressor(compressionLevel);
                if (compressor == IntPtr.Zero)
                    throw new Exception("libdeflate: Out of memory");

                crc32 = LibdeflateCrc32(0, ptrInBuffer, inBuffer.Length);

                if (force)
                    maxCompressedSize = LibdeflateDeflateCompressBound(compressor, inBuffer.Length);
                else
                    maxCompressedSize = inBuffer.Length - 1;

                if (maxCompressedSize < 0) maxCompressedSize = 0;

                outBuffer = new byte[maxCompressedSize];

                if (outBuffer.Length > 0)
                {
                    pinnedOutArray = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);
                    ptrOutBuffer = pinnedOutArray.AddrOfPinnedObject();

                    deflatedSize = LibdeflateDeflateCompress(compressor, ptrInBuffer, inBuffer.Length, ptrOutBuffer, outBuffer.Length);

                    pinnedOutArray.Free();
                }
                else
                {
                    deflatedSize = 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn Libdeflate.Deflate");
            }
            finally
            {
                if (compressor != IntPtr.Zero)
                    LibdeflateFreeCompressor(compressor);
                if (pinnedInArray.IsAllocated) pinnedInArray.Free();
            }
        }

        // Simple CRC32 for byte[]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetCrc32(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return 0;

            GCHandle pinned = default;
            try
            {
                pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                IntPtr ptr = pinned.AddrOfPinnedObject();
                return LibdeflateCrc32(0, ptr, buffer.Length);
            }
            finally
            {
                if (pinned.IsAllocated) pinned.Free();
            }
        }

        // Stream-to-stream compressor using libdeflate streaming API (raw DEFLATE)
        // Reads from input, writes compressed DEFLATE bytes to output.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong DeflateStreamToStream(
            Stream input,
            Stream output,
            int compressionLevel,
            int blockSize,
            bool outFlushToByteAlign = false,
            Action<long>? onReadProgress = null,
            Action<long>? onWriteProgress = null,
            Action? onBeforeRound = null)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (blockSize <= 0) throw new ArgumentOutOfRangeException(nameof(blockSize));
            if (!input.CanRead) throw new InvalidOperationException("Input stream must be readable.");
            if (!output.CanWrite) throw new InvalidOperationException("Output stream must be writable.");

            // We use input.CanSeek to know exactly when we reached the end for the final block flag.
            if (!input.CanSeek) throw new NotSupportedException("DeflateStreamToStream currently requires a seekable input stream.");

            const int DictMax = 32 * 1024;

            IntPtr compressor = IntPtr.Zero;

            try
            {
                compressor = LibdeflateAllocCompressor(compressionLevel);
                if (compressor == IntPtr.Zero)
                    throw new Exception("libdeflate: failed to allocate compressor");

                LibdeflateDeflateCompressBlockReset(compressor);

                // Sliding window input buffer: dictionary prefix + new block bytes
                byte[] inBuf = new byte[DictMax + blockSize];
                int dictLen = 0;

                // Reusable output buffer sized per-block bound
                byte[] outBuf = Array.Empty<byte>();

                ulong totalWritten = 0;

                while (true)
                {
                    onBeforeRound?.Invoke();

                    int toRead = blockSize;
                    int read = input.Read(inBuf, dictLen, toRead);
                    if (read <= 0)
                        break;

                    onReadProgress?.Invoke(read);

                    ulong outBoundU = LibdeflateDeflateCompressBoundBlock((ulong)read);
                    int outBound = checked((int)outBoundU);
                    if (outBuf.Length < outBound)
                        outBuf = new byte[outBound];

                    GCHandle pinIn = default;
                    GCHandle pinOut = default;

                    try
                    {
                        pinIn = GCHandle.Alloc(inBuf, GCHandleType.Pinned);
                        pinOut = GCHandle.Alloc(outBuf, GCHandleType.Pinned);

                        IntPtr ptrIn = pinIn.AddrOfPinnedObject();
                        IntPtr ptrOut = pinOut.AddrOfPinnedObject();

                        bool isFinal = (input.Position == input.Length);

                        ulong written = LibdeflateDeflateCompressBlock(
                            compressor,
                            ptrIn,
                            (ulong)dictLen,
                            (ulong)read,
                            isFinal,
                            ptrOut,
                            (ulong)outBound,
                            outFlushToByteAlign);

                        if (written == 0)
                            throw new Exception("libdeflate_deflate_compress_block: returned 0 (output too small or error).");

                        output.Write(outBuf, 0, checked((int)written));
                        totalWritten += written;
                        onWriteProgress?.Invoke((long)written);
                    }
                    finally
                    {
                        if (pinIn.IsAllocated) pinIn.Free();
                        if (pinOut.IsAllocated) pinOut.Free();
                    }

                    // Prepare dictionary for next iteration: last min(32KB, dictLen + read) bytes
                    int newDictLen = Math.Min(DictMax, dictLen + read);
                    Buffer.BlockCopy(inBuf, dictLen + read - newDictLen, inBuf, 0, newDictLen);
                    dictLen = newDictLen;
                }

                return totalWritten;
            }
            finally
            {
                if (compressor != IntPtr.Zero)
                    LibdeflateFreeCompressor(compressor);
            }
        }

        /* ========================================================================== */
        /*                             Compression (one-shot)                         */
        /* ========================================================================== */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr LibdeflateAllocCompressor(int compressionLevel)
        {
            switch (IntPtr.Size)
            {
                case 4: return libdeflate_alloc_compressor_x86(compressionLevel);
                case 8: return libdeflate_alloc_compressor_x64(compressionLevel);
                default: throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_alloc_compressor")]
        private static extern IntPtr libdeflate_alloc_compressor_x86(int compressionLevel);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_alloc_compressor")]
        private static extern IntPtr libdeflate_alloc_compressor_x64(int compressionLevel);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LibdeflateDeflateCompressBound(IntPtr compressor, int in_nbytes)
        {
            switch (IntPtr.Size)
            {
                case 4: return (int)libdeflate_deflate_compress_bound_x86(compressor, (UIntPtr)in_nbytes);
                case 8: return (int)libdeflate_deflate_compress_bound_x64(compressor, (UIntPtr)in_nbytes);
                default: throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound")]
        private static extern UIntPtr libdeflate_deflate_compress_bound_x86(IntPtr compressor, UIntPtr in_nbytes);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound")]
        private static extern UIntPtr libdeflate_deflate_compress_bound_x64(IntPtr compressor, UIntPtr in_nbytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /*                          Streaming Compression (raw)                       */
        /* ========================================================================== */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong LibdeflateDeflateCompressBlock(
            IntPtr compressor,
            IntPtr inBlockWithDict,
            ulong dictNBytes,
            ulong inBlockNBytes,
            bool inIsFinalBlock,
            IntPtr outPart,
            ulong outPartNBytesAvail,
            bool outFlushToByteAlign)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return (ulong)libdeflate_deflate_compress_block_x86(
                        compressor,
                        inBlockWithDict, (UIntPtr)dictNBytes, (UIntPtr)inBlockNBytes,
                        inIsFinalBlock ? 1 : 0,
                        outPart, (UIntPtr)outPartNBytesAvail, outFlushToByteAlign ? 1 : 0);
                case 8:
                    return (ulong)libdeflate_deflate_compress_block_x64(
                        compressor,
                        inBlockWithDict, (UIntPtr)dictNBytes, (UIntPtr)inBlockNBytes,
                        inIsFinalBlock ? 1 : 0,
                        outPart, (UIntPtr)outPartNBytesAvail, outFlushToByteAlign ? 1 : 0);
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_block")]
        private static extern UIntPtr libdeflate_deflate_compress_block_x86(
            IntPtr compressor,
            IntPtr in_block_with_dict, UIntPtr dict_nbytes, UIntPtr in_block_nbytes, int in_is_final_block,
            IntPtr out_part, UIntPtr out_part_nbytes_avail, int out_is_flush_to_byte_align);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_block")]
        private static extern UIntPtr libdeflate_deflate_compress_block_x64(
            IntPtr compressor,
            IntPtr in_block_with_dict, UIntPtr dict_nbytes, UIntPtr in_block_nbytes, int in_is_final_block,
            IntPtr out_part, UIntPtr out_part_nbytes_avail, int out_is_flush_to_byte_align);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong LibdeflateDeflateCompressBlockUncompressed(
            IntPtr compressor,
            IntPtr inBlock,
            ulong inBlockNBytes,
            bool inIsFinalBlock,
            IntPtr outPart,
            ulong outPartNBytesAvail)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return (ulong)libdeflate_deflate_compress_block_uncompressed_x86(
                        compressor, inBlock, (UIntPtr)inBlockNBytes, inIsFinalBlock ? 1 : 0, outPart, (UIntPtr)outPartNBytesAvail);
                case 8:
                    return (ulong)libdeflate_deflate_compress_block_uncompressed_x64(
                        compressor, inBlock, (UIntPtr)inBlockNBytes, inIsFinalBlock ? 1 : 0, outPart, (UIntPtr)outPartNBytesAvail);
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_block_uncompressed")]
        private static extern UIntPtr libdeflate_deflate_compress_block_uncompressed_x86(
            IntPtr compressor, IntPtr in_block, UIntPtr in_block_nbytes, int in_is_final_block,
            IntPtr out_part, UIntPtr out_part_nbytes_avail);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_block_uncompressed")]
        private static extern UIntPtr libdeflate_deflate_compress_block_uncompressed_x64(
            IntPtr compressor, IntPtr in_block, UIntPtr in_block_nbytes, int in_is_final_block,
            IntPtr out_part, UIntPtr out_part_nbytes_avail);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong LibdeflateDeflateCompressBlockContinue(
            IntPtr compressor,
            IntPtr inBlockWithDict,
            ulong dictNBytes,
            ulong inBlockNBytes,
            ulong inBorderNBytes,
            bool inIsFinalBlock,
            IntPtr outPart,
            ulong outPartNBytesAvail,
            bool outFlushToByteAlign)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return (ulong)libdeflate_deflate_compress_block_continue_x86(
                        compressor,
                        inBlockWithDict, (UIntPtr)dictNBytes, (UIntPtr)inBlockNBytes, (UIntPtr)inBorderNBytes,
                        inIsFinalBlock ? 1 : 0,
                        outPart, (UIntPtr)outPartNBytesAvail, outFlushToByteAlign ? 1 : 0);
                case 8:
                    return (ulong)libdeflate_deflate_compress_block_continue_x64(
                        compressor,
                        inBlockWithDict, (UIntPtr)dictNBytes, (UIntPtr)inBlockNBytes, (UIntPtr)inBorderNBytes,
                        inIsFinalBlock ? 1 : 0,
                        outPart, (UIntPtr)outPartNBytesAvail, outFlushToByteAlign ? 1 : 0);
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_block_continue")]
        private static extern UIntPtr libdeflate_deflate_compress_block_continue_x86(
            IntPtr compressor,
            IntPtr in_block_with_dict, UIntPtr dict_nbytes, UIntPtr in_block_nbytes, UIntPtr in_border_nbytes, int in_is_final_block,
            IntPtr out_part, UIntPtr out_part_nbytes_avail, int out_is_flush_to_byte_align);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_block_continue")]
        private static extern UIntPtr libdeflate_deflate_compress_block_continue_x64(
            IntPtr compressor,
            IntPtr in_block_with_dict, UIntPtr dict_nbytes, UIntPtr in_block_nbytes, UIntPtr in_border_nbytes, int in_is_final_block,
            IntPtr out_part, UIntPtr out_part_nbytes_avail, int out_is_flush_to_byte_align);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LibdeflateDeflateCompressBlockReset(IntPtr compressor)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    libdeflate_deflate_compress_block_reset_x86(compressor);
                    break;
                case 8:
                    libdeflate_deflate_compress_block_reset_x64(compressor);
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_block_reset")]
        private static extern void libdeflate_deflate_compress_block_reset_x86(IntPtr compressor);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_block_reset")]
        private static extern void libdeflate_deflate_compress_block_reset_x64(IntPtr compressor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort LibdeflateDeflateCompressGetState(IntPtr compressor)
        {
            switch (IntPtr.Size)
            {
                case 4: return libdeflate_deflate_compress_get_state_x86(compressor);
                case 8: return libdeflate_deflate_compress_get_state_x64(compressor);
                default: throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_get_state")]
        private static extern ushort libdeflate_deflate_compress_get_state_x86(IntPtr compressor);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_get_state")]
        private static extern ushort libdeflate_deflate_compress_get_state_x64(IntPtr compressor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LibdeflateDeflateCompressSetState(IntPtr compressor, ushort state)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    libdeflate_deflate_compress_set_state_x86(compressor, state);
                    break;
                case 8:
                    libdeflate_deflate_compress_set_state_x64(compressor, state);
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_set_state")]
        private static extern void libdeflate_deflate_compress_set_state_x86(IntPtr compressor, ushort state);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_set_state")]
        private static extern void libdeflate_deflate_compress_set_state_x64(IntPtr compressor, ushort state);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong LibdeflateDeflateCompressBoundBlock(ulong inBlockNBytes)
        {
            switch (IntPtr.Size)
            {
                case 4: return (ulong)libdeflate_deflate_compress_bound_block_x86((UIntPtr)inBlockNBytes);
                case 8: return (ulong)libdeflate_deflate_compress_bound_block_x64((UIntPtr)inBlockNBytes);
                default: throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound_block")]
        private static extern UIntPtr libdeflate_deflate_compress_bound_block_x86(UIntPtr in_block_nbytes);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound_block")]
        private static extern UIntPtr libdeflate_deflate_compress_bound_block_x64(UIntPtr in_block_nbytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong LibdeflateDeflateCompressBoundBlocks(ulong inStreamNBytes, ulong inBlockNBytes)
        {
            switch (IntPtr.Size)
            {
                case 4: return libdeflate_deflate_compress_bound_blocks_x86(inStreamNBytes, (UIntPtr)inBlockNBytes);
                case 8: return libdeflate_deflate_compress_bound_blocks_x64(inStreamNBytes, (UIntPtr)inBlockNBytes);
                default: throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound_blocks")]
        private static extern ulong libdeflate_deflate_compress_bound_blocks_x86(ulong in_stream_nbytes, UIntPtr in_block_nbytes);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound_blocks")]
        private static extern ulong libdeflate_deflate_compress_bound_blocks_x64(ulong in_stream_nbytes, UIntPtr in_block_nbytes);

        /* ========================================================================== */
        /*                                Checksums                                   */
        /* ========================================================================== */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LibdeflateCrc32(uint crc, IntPtr inBuffer, int len)
        {
            switch (IntPtr.Size)
            {
                case 4: return libdeflate_crc32_x86(crc, inBuffer, (UIntPtr)len);
                case 8: return libdeflate_crc32_x64(crc, inBuffer, (UIntPtr)len);
                default: throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_crc32")]
        private static extern uint libdeflate_crc32_x86(uint crc, IntPtr inBuffer, UIntPtr len);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_crc32")]
        private static extern uint libdeflate_crc32_x64(uint crc, IntPtr inBuffer, UIntPtr len);

        /* ========================================================================== */
        /*                          Streaming Decompression (raw)                     */
        /* ========================================================================== */

        public enum LibdeflateResult : int
        {
            Success = 0,
            BadData = 1,
            ShortOutput = 2,
            InsufficientSpace = 3
        }

        public enum LibdeflateDecompressStopBy : int
        {
            StopByFinalBlock = 0,
            StopByAnyBlock = 1,
            StopByAnyBlockAndFullInput = 2,
            StopByAnyBlockAndFullOutput = 3,
            StopByAnyBlockAndFullOutputAndInByteAlign = 4,
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr LibdeflateAllocDecompressor()
        {
            switch (IntPtr.Size)
            {
                case 4: return libdeflate_alloc_decompressor_x86();
                case 8: return libdeflate_alloc_decompressor_x64();
                default: throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_alloc_decompressor")]
        private static extern IntPtr libdeflate_alloc_decompressor_x86();

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_alloc_decompressor")]
        private static extern IntPtr libdeflate_alloc_decompressor_x64();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LibdeflateFreeDecompressor(IntPtr decompressor)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    libdeflate_free_decompressor_x86(decompressor);
                    break;
                case 8:
                    libdeflate_free_decompressor_x64(decompressor);
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_free_decompressor")]
        private static extern void libdeflate_free_decompressor_x86(IntPtr decompressor);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_free_decompressor")]
        private static extern void libdeflate_free_decompressor_x64(IntPtr decompressor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LibdeflateResult LibdeflateDeflateDecompressBlock(
            IntPtr decompressor,
            IntPtr inPart,
            ulong inPartNBytesBound,
            IntPtr outBlockWithInDict,
            ulong inDictNBytes,
            ulong outBlockNBytes,
            out ulong actualInNBytes,
            out ulong actualOutNBytes,
            LibdeflateDecompressStopBy stopType,
            out int isFinalBlock)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    {
                        var res = libdeflate_deflate_decompress_block_x86(
                            decompressor,
                            inPart, (UIntPtr)inPartNBytesBound,
                            outBlockWithInDict, (UIntPtr)inDictNBytes, (UIntPtr)outBlockNBytes,
                            out UIntPtr actualIn, out UIntPtr actualOut,
                            stopType, out isFinalBlock);
                        actualInNBytes = (ulong)actualIn;
                        actualOutNBytes = (ulong)actualOut;
                        return res;
                    }
                case 8:
                    {
                        var res = libdeflate_deflate_decompress_block_x64(
                            decompressor,
                            inPart, (UIntPtr)inPartNBytesBound,
                            outBlockWithInDict, (UIntPtr)inDictNBytes, (UIntPtr)outBlockNBytes,
                            out UIntPtr actualIn, out UIntPtr actualOut,
                            stopType, out isFinalBlock);
                        actualInNBytes = (ulong)actualIn;
                        actualOutNBytes = (ulong)actualOut;
                        return res;
                    }
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_decompress_block")]
        private static extern LibdeflateResult libdeflate_deflate_decompress_block_x86(
            IntPtr decompressor,
            IntPtr in_part, UIntPtr in_part_nbytes_bound,
            IntPtr out_block_with_in_dict, UIntPtr in_dict_nbytes, UIntPtr out_block_nbytes,
            out UIntPtr actual_in_nbytes_ret, out UIntPtr actual_out_nbytes_ret,
            LibdeflateDecompressStopBy stop_type, out int is_final_block_ret);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_decompress_block")]
        private static extern LibdeflateResult libdeflate_deflate_decompress_block_x64(
            IntPtr decompressor,
            IntPtr in_part, UIntPtr in_part_nbytes_bound,
            IntPtr out_block_with_in_dict, UIntPtr in_dict_nbytes, UIntPtr out_block_nbytes,
            out UIntPtr actual_in_nbytes_ret, out UIntPtr actual_out_nbytes_ret,
            LibdeflateDecompressStopBy stop_type, out int is_final_block_ret);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort LibdeflateDeflateDecompressGetState(IntPtr decompressor)
        {
            switch (IntPtr.Size)
            {
                case 4: return libdeflate_deflate_decompress_get_state_x86(decompressor);
                case 8: return libdeflate_deflate_decompress_get_state_x64(decompressor);
                default: throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_decompress_get_state")]
        private static extern ushort libdeflate_deflate_decompress_get_state_x86(IntPtr decompressor);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_decompress_get_state")]
        private static extern ushort libdeflate_deflate_decompress_get_state_x64(IntPtr decompressor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LibdeflateDeflateDecompressSetState(IntPtr decompressor, ushort state)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    libdeflate_deflate_decompress_set_state_x86(decompressor, state);
                    break;
                case 8:
                    libdeflate_deflate_decompress_set_state_x64(decompressor, state);
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_decompress_set_state")]
        private static extern void libdeflate_deflate_decompress_set_state_x86(IntPtr decompressor, ushort state);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_decompress_set_state")]
        private static extern void libdeflate_deflate_decompress_set_state_x64(IntPtr decompressor, ushort state);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LibdeflateDeflateDecompressBlockReset(IntPtr decompressor)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    libdeflate_deflate_decompress_block_reset_x86(decompressor);
                    break;
                case 8:
                    libdeflate_deflate_decompress_block_reset_x64(decompressor);
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
            }
        }

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_decompress_block_reset")]
        private static extern void libdeflate_deflate_decompress_block_reset_x86(IntPtr decompressor);

        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_decompress_block_reset")]
        private static extern void libdeflate_deflate_decompress_block_reset_x64(IntPtr decompressor);
    }
}


////////using System.Runtime.CompilerServices;
////////using System.Runtime.InteropServices;

////////namespace FastZipDotNet.Zip.LibDeflate
////////{
////////    public sealed partial class LibDeflateWrapper
////////    {
////////        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
////////        public static uint GetCrc32(byte[] buffer)
////////        {
////////            IntPtr ptrBuffer = IntPtr.Zero;
////////            GCHandle pinnedBuffer;

////////            try
////////            {
////////                pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
////////                ptrBuffer = pinnedBuffer.AddrOfPinnedObject();

////////                uint crc = LibdeflateCrc32(0, ptrBuffer, buffer.Length);

////////                pinnedBuffer.Free();

////////                return crc;
////////            }
////////            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn Libdeflate.Deflate"); }
////////        }

////////        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
////////        public static void Libdeflate(byte[] inBuffer, int compressionLevel, bool force, out byte[] outBuffer, out ulong deflatedSize, out uint crc32)
////////        {
////////            IntPtr ptrInBuffer = IntPtr.Zero;
////////            IntPtr ptrOutBuffer = IntPtr.Zero;
////////            GCHandle pinnedInArray;
////////            GCHandle pinnedOutArray;
////////            int maxCompressedSize;
////////            IntPtr compressor = IntPtr.Zero;
////////            try
////////            {
////////                pinnedInArray = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
////////                ptrInBuffer = pinnedInArray.AddrOfPinnedObject();

////////                compressor = LibdeflateAllocCompressor(compressionLevel);
////////                if (compressor == IntPtr.Zero)
////////                    throw new Exception("Out of memory");

////////                crc32 = LibdeflateCrc32(0, ptrInBuffer, inBuffer.Length);

////////                if (force)
////////                    maxCompressedSize = LibdeflateDeflateCompressBound(compressor, inBuffer.Length);
////////                else
////////                    maxCompressedSize = inBuffer.Length - 1;

////////                outBuffer = new byte[maxCompressedSize];
////////                pinnedOutArray = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);
////////                ptrOutBuffer = pinnedOutArray.AddrOfPinnedObject();

////////                deflatedSize = LibdeflateDeflateCompress(compressor, ptrInBuffer, inBuffer.Length, ptrOutBuffer, maxCompressedSize);

////////                LibdeflateFreeCompressor(compressor);
////////                pinnedInArray.Free();
////////                pinnedOutArray.Free();
////////            }
////////            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn Libdeflate.Deflate"); }
////////        }

////////        /* ========================================================================== */
////////        /*                             Compression                                    */
////////        /* ========================================================================== */

////////        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
////////        public static IntPtr LibdeflateAllocCompressor(int compressionLevel)
////////        {
////////            switch (IntPtr.Size)
////////            {
////////                case 4:
////////                    return libdeflate_alloc_compressor_x86(compressionLevel);
////////                case 8:
////////                    return libdeflate_alloc_compressor_x64(compressionLevel);
////////                default:
////////                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
////////            }
////////        }

////////        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_alloc_compressor")]
////////        private static extern IntPtr libdeflate_alloc_compressor_x86(int compressionLevel);

////////        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_alloc_compressor")]
////////        private static extern IntPtr libdeflate_alloc_compressor_x64(int compressionLevel);

////////        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
////////        public static ulong LibdeflateDeflateCompress(IntPtr compressor, IntPtr inBuffer, int in_nbytes, IntPtr outBuffer, int out_nbytes_avail)
////////        {
////////            switch (IntPtr.Size)
////////            {
////////                case 4:
////////                    return (ulong)libdeflate_deflate_compress_x86(compressor, inBuffer, (UIntPtr)in_nbytes, outBuffer, (UIntPtr)out_nbytes_avail);
////////                case 8:
////////                    return (ulong)libdeflate_deflate_compress_x64(compressor, inBuffer, (UIntPtr)in_nbytes, outBuffer, (UIntPtr)out_nbytes_avail);
////////                default:
////////                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
////////            }
////////        }

////////        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress")]
////////        private static extern UIntPtr libdeflate_deflate_compress_x86(IntPtr compressor, IntPtr inBuffer, UIntPtr in_nbytes, IntPtr outBuffer, UIntPtr out_nbytes_avail);

////////        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress")]
////////        private static extern UIntPtr libdeflate_deflate_compress_x64(IntPtr compressor, IntPtr inBuffer, UIntPtr in_nbytes, IntPtr outBuffer, UIntPtr out_nbytes_avail);

////////        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
////////        public static int LibdeflateDeflateCompressBound(IntPtr compressor, int in_nbytes)
////////        {
////////            switch (IntPtr.Size)
////////            {
////////                case 4:
////////                    return (int)libdeflate_deflate_compress_bound_x86(compressor, (UIntPtr)in_nbytes);
////////                case 8:
////////                    return (int)libdeflate_deflate_compress_bound_x64(compressor, (UIntPtr)in_nbytes);
////////                default:
////////                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
////////            }
////////        }

////////        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound")]
////////        private static extern UIntPtr libdeflate_deflate_compress_bound_x86(IntPtr compressor, UIntPtr in_nbytes);

////////        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound")]
////////        private static extern UIntPtr libdeflate_deflate_compress_bound_x64(IntPtr compressor, UIntPtr in_nbytes);

////////        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
////////        public static void LibdeflateFreeCompressor(IntPtr compressor)
////////        {
////////            switch (IntPtr.Size)
////////            {
////////                case 4:
////////                    libdeflate_free_compressor_x86(compressor);
////////                    break;
////////                case 8:
////////                    libdeflate_free_compressor_x64(compressor);
////////                    break;
////////                default:
////////                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
////////            }
////////        }

////////        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_free_compressor")]
////////        private static extern void libdeflate_free_compressor_x86(IntPtr compressor);

////////        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_free_compressor")]
////////        private static extern void libdeflate_free_compressor_x64(IntPtr compressor);

////////        /* ========================================================================== */
////////        /*                                Checksums                                   */
////////        /* ========================================================================== */

////////        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
////////        public static UInt32 LibdeflateCrc32(UInt32 crc, IntPtr inBuffer, int len)
////////        {
////////            switch (IntPtr.Size)
////////            {
////////                case 4:
////////                    return libdeflate_crc32_x86(crc, inBuffer, (UIntPtr)len);
////////                case 8:
////////                    return libdeflate_crc32_x64(crc, inBuffer, (UIntPtr)len);
////////                default:
////////                    throw new InvalidOperationException("Invalid platform. Cannot find proper function");
////////            }
////////        }

////////        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_crc32")]
////////        private static extern UInt32 libdeflate_crc32_x86(UInt32 crc, IntPtr inBuffer, UIntPtr len);

////////        [DllImport("libdeflate.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_crc32")]
////////        private static extern UInt32 libdeflate_crc32_x64(UInt32 crc, IntPtr inBuffer, UIntPtr len);
////////    }
////////}
