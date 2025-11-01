using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SfxStub
{
    internal static class ResourceLoader
    {
        private static readonly IntPtr RT_RCDATA = new IntPtr(10);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr lpType, IntPtr lpName, ushort wLanguage);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static byte[] LoadRCDATA(ushort id, ushort language = 0)
        {
            IntPtr hMod = GetModuleHandle(null);
            IntPtr hResInfo = FindResourceEx(hMod, RT_RCDATA, new IntPtr(id), language);
            if (hResInfo == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), $"FindResourceEx failed for id {id}");

            uint size = SizeofResource(hMod, hResInfo);
            IntPtr hResData = LoadResource(hMod, hResInfo);
            if (hResData == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), $"LoadResource failed for id {id}");

            IntPtr p = LockResource(hResData);
            if (p == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), $"LockResource failed for id {id}");

            byte[] buffer = new byte[size];
            Marshal.Copy(p, buffer, 0, buffer.Length);
            return buffer;
        }
    }
}