using System.Runtime.InteropServices;

namespace Brutal_Zip.Classes
{
    internal static class ResourceUpdater
    {
        // RT_ICON = 3, RT_GROUP_ICON = 14
        private static readonly IntPtr RT_ICON = new IntPtr(3);
        private static readonly IntPtr RT_GROUP_ICON = new IntPtr(14);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, IntPtr lpName, ushort wLanguage,
            byte[] lpData, int cbData);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        // .ico file structs
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ICONDIR
        {
            public ushort idReserved;   // 0
            public ushort idType;       // 1 for icons
            public ushort idCount;      // number of icons
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ICONDIRENTRY
        {
            public byte bWidth;
            public byte bHeight;
            public byte bColorCount;
            public byte bReserved;
            public ushort wPlanes;
            public ushort wBitCount;
            public uint dwBytesInRes;
            public uint dwImageOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct GRPICONDIRENTRY
        {
            public byte bWidth;
            public byte bHeight;
            public byte bColorCount;
            public byte bReserved;
            public ushort wPlanes;
            public ushort wBitCount;
            public uint dwBytesInRes;
            public ushort nID; // resource ID
        }

        // Replace EXE's icon resources with the images in .ico
        public static void ReplaceIcon(string exePath, string icoPath, ushort language = 0)
        {
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
                throw new FileNotFoundException("Target EXE not found", exePath);
            if (string.IsNullOrWhiteSpace(icoPath) || !File.Exists(icoPath))
                throw new FileNotFoundException("Icon (.ico) not found", icoPath);

            byte[] icoBytes = File.ReadAllBytes(icoPath);
            using var ms = new MemoryStream(icoBytes);
            using var br = new BinaryReader(ms);

            ICONDIR dir = new ICONDIR
            {
                idReserved = br.ReadUInt16(),
                idType = br.ReadUInt16(),
                idCount = br.ReadUInt16()
            };
            if (dir.idReserved != 0 || dir.idType != 1 || dir.idCount == 0)
                throw new InvalidDataException("Invalid .ico file.");

            ICONDIRENTRY[] entries = new ICONDIRENTRY[dir.idCount];
            for (int i = 0; i < dir.idCount; i++)
            {
                entries[i] = new ICONDIRENTRY
                {
                    bWidth = br.ReadByte(),
                    bHeight = br.ReadByte(),
                    bColorCount = br.ReadByte(),
                    bReserved = br.ReadByte(),
                    wPlanes = br.ReadUInt16(),
                    wBitCount = br.ReadUInt16(),
                    dwBytesInRes = br.ReadUInt32(),
                    dwImageOffset = br.ReadUInt32()
                };
            }

            // Build GRPICONDIR (group icon resource) in memory:
            // [ICONDIR] then [GRPICONDIRENTRY]*count
            int grpSize = 6 + (entries.Length * 14);
            byte[] grpData = new byte[grpSize];
            using (var gms = new MemoryStream(grpData))
            using (var bw = new BinaryWriter(gms))
            {
                bw.Write((ushort)0); // idReserved
                bw.Write((ushort)1); // idType = 1
                bw.Write((ushort)entries.Length);

                // Assign resource IDs 1..n for images
                for (int i = 0; i < entries.Length; i++)
                {
                    var e = entries[i];
                    bw.Write(e.bWidth);
                    bw.Write(e.bHeight);
                    bw.Write(e.bColorCount);
                    bw.Write(e.bReserved);
                    bw.Write(e.wPlanes);
                    bw.Write(e.wBitCount);
                    bw.Write(e.dwBytesInRes);
                    bw.Write((ushort)(i + 1)); // resource ID
                }
            }

            // Begin resource update
            IntPtr hUpdate = BeginUpdateResource(exePath, false);
            if (hUpdate == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "BeginUpdateResource failed");

            bool ok = false;
            try
            {
                // Update group icon (name = 1)
                if (!UpdateResource(hUpdate, RT_GROUP_ICON, new IntPtr(1), language, grpData, grpData.Length))
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "UpdateResource (group icon) failed");

                // Update each icon image
                for (int i = 0; i < entries.Length; i++)
                {
                    var e = entries[i];
                    ms.Position = e.dwImageOffset;
                    byte[] img = br.ReadBytes((int)e.dwBytesInRes);

                    if (!UpdateResource(hUpdate, RT_ICON, new IntPtr(i + 1), language, img, img.Length))
                        throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), $"UpdateResource (icon {i + 1}) failed");
                }

                ok = EndUpdateResource(hUpdate, false);
                if (!ok)
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "EndUpdateResource failed");
            }
            finally
            {
                if (!ok)
                {
                    try { EndUpdateResource(hUpdate, true); } catch { }
                }
            }
        }
    }
}
