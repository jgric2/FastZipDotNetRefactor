using Microsoft.Win32.SafeHandles;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace FastZipDotNet.WinAPIHelper
{
    public static class WinAPI
    {

        public enum EMoveMethod : uint
        {
            Begin = 0,
            Current = 1,
            End = 2
        }


        // in FastZipDotNet.WinAPIHelper.WinAPI
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetFilePointerEx(
            nint hFile,
            long liDistanceToMove,
            out long lpNewFilePointer,
            uint dwMoveMethod);




        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SetFilePointer(nint handle, int lDistanceToMove, out int lpDistanceToMoveHigh, uint dwMoveMethod);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern unsafe uint SetFilePointerEx(
            [In] nint hFile,
            [In] long lDistanceToMove);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint SetFilePointerEx(
            [In] nint hFile,
            [In] int lDistanceToMove,
            [Out] out int lpDistanceToMoveHigh,
            [In] EMoveMethod dwMoveMethod);

        [Flags]
        public enum EFileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(nint hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, nint lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(
    nint hFile,
    IntPtr lpBuffer,
    uint nNumberOfBytesToRead,
    out uint lpNumberOfBytesRead,
    nint lpOverlapped);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFile(nint hFile, ref byte lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, nint overlapped);


        [DllImport("kernel32.dll")]
        public static extern bool WriteFile(nint hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, [In] nint lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(nint handle);



        /// <summary>
        /// Creates a file / directory or opens an handle for an existing file.
        /// <b>If you want to get an handle for an existing folder use <see cref="OpenReadWriteFileSystemEntryHandle"/> with ( 0x02000000 ) as attribute and FileMode ( 0x40000000 | 0x80000000 )</b>
        /// Otherwise it you'll get an invalid handle
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern nint CreateFileW(
             string fullName,
             [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
             [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
             nint lpSecurityAttributes,
             [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
             [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes,
             nint hTemplateFile);






        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api",
      Justification = "We explicitly need to call the native SetFileAttributes as the managed one does not support long paths.")]
        public static extern bool SetFileAttributesW(
      string lpFileName,
      FileAttributes dwFileAttributes);


        /// <nodoc />
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U4)]
        [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api",
            Justification = "We explicitly need to call the native GetFileAttributes as the managed one does not support long paths.")]
        public static extern uint GetFileAttributesW(
            string lpFileName);



        [StructLayout(LayoutKind.Sequential)]
        public struct FILE_TIME
        {
            public FILE_TIME(long fileTime)
            {
                ftTimeLow = (uint)fileTime;
                ftTimeHigh = (uint)(fileTime >> 32);
            }

            public long ToTicks()
            {
                return ((long)ftTimeHigh << 32) + ftTimeLow;
            }

            internal uint ftTimeLow;
            internal uint ftTimeHigh;
        }

        [DllImport(@"kernel32.dll", SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        public unsafe static extern bool SetFileTime(SafeFileHandle hFile, FILE_TIME* creationTime,
                    FILE_TIME* lastAccessTime, FILE_TIME* lastWriteTime);


        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "SetFileTime", ExactSpelling = true)]
        public static extern bool SetFileCreateTime(
        nint hFile,
        ref long lpCreationTime,
        nint lpLastAccessTimeUnused,
        nint lpLastWriteTimeUnused);


        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "SetFileTime", ExactSpelling = true)]
        public static extern bool SetFileLastAccessTime(
        nint hFile,
        nint lpCreationTimeUnused,
        ref long lpLastAccessTime,
        nint lpLastWriteTimeUnused);


        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "SetFileTime", ExactSpelling = true)]
        public static extern bool SetFileLastWriteTime(
        nint hFile,
        nint lpCreationTimeUnused,
        nint lpLastAccessTimeUnused,
        ref long lpLastWriteTime);


        /// <summary>
        /// Thin wrapper for native SetFileAttributesW that checks the win32 error upon failure
        /// </summary>
        public static bool TrySetFileAttributes(string path, FileAttributes attributes, out int hr)
        {
            if (!SetFileAttributesW(path, attributes))
            {
                hr = Marshal.GetLastWin32Error();
                return false;
            }

            hr = 0;
            return true;
        }



        public static bool TryGetFileAttributesViaGetFileAttributes(string path, out FileAttributes attributes, out int hr)
        {
            var fileAttributes = GetFileAttributesW(path);

            if (fileAttributes == -1)
            {
                hr = Marshal.GetLastWin32Error();
                attributes = FileAttributes.Normal;
                return false;
            }

            hr = 0;
            attributes = (FileAttributes)fileAttributes;
            return true;
        }


        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool PathIsDirectory([MarshalAs(UnmanagedType.LPWStr), In] string pszPath);


    }
}
