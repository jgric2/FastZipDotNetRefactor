using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Brutal_Zip
{
    // Caches small shell icons by extension and by path, backed by SHGetFileInfo.
    // Caches small shell icons by extension and by path, backed by SHGetFileInfo.
    public sealed class IconCache : IDisposable
    {
        public ImageList ImageList { get; } = new ImageList
        {
            ColorDepth = ColorDepth.Depth32Bit,
            ImageSize = new Size(16, 16)
        };

        public string FolderKey { get; } = "__folder";
        public string FileKey { get; } = "__file";

        // Index helpers (avoid ImageKey quirks in VirtualMode)
        public int FolderIndex => EnsureFolderIndex();
        public int FileIndex => EnsureFileIndex();

        private int _folderIndex = -1;
        private int _fileIndex = -1;

        private readonly ConcurrentDictionary<string, string> _extToKey = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, string> _pathToKey = new(StringComparer.OrdinalIgnoreCase);

        public IconCache()
        {
            // Preload generic folder & file icons (use attributes for reliability)
            var folderIcon = GetShellIconCore("folder", isFolder: true, useAttributes: true);
            var fileIcon = GetShellIconCore("file", isFolder: false, useAttributes: true);

            ImageList.Images.Add(FolderKey, folderIcon);
            ImageList.Images.Add(FileKey, fileIcon);

            _folderIndex = ImageList.Images.IndexOfKey(FolderKey);
            _fileIndex = ImageList.Images.IndexOfKey(FileKey);
        }

        private int EnsureFolderIndex()
        {
            if (_folderIndex >= 0) return _folderIndex;
            _folderIndex = ImageList.Images.IndexOfKey(FolderKey);
            if (_folderIndex < 0)
            {
                ImageList.Images.Add(FolderKey, SystemIcons.WinLogo);
                _folderIndex = ImageList.Images.IndexOfKey(FolderKey);
            }
            return _folderIndex;
        }

        private int EnsureFileIndex()
        {
            if (_fileIndex >= 0) return _fileIndex;
            _fileIndex = ImageList.Images.IndexOfKey(FileKey);
            if (_fileIndex < 0)
            {
                ImageList.Images.Add(FileKey, SystemIcons.WinLogo);
                _fileIndex = ImageList.Images.IndexOfKey(FileKey);
            }
            return _fileIndex;
        }

        // Returns an ImageList key for a given filesystem path (file or folder)
        public string GetKeyForPath(string path, bool isFolder)
        {
            if (string.IsNullOrEmpty(path))
                return isFolder ? FolderKey : FileKey;

            return _pathToKey.GetOrAdd(path, p =>
            {
                try
                {
                    var icon = GetShellIconCore(p, isFolder, useAttributes: false);
                    string key = $"__path__{p.GetHashCode()}";
                    ImageList.Images.Add(key, icon);
                    return key;
                }
                catch
                {
                    return isFolder ? FolderKey : FileKey;
                }
            });
        }

        // Returns an ImageList key for extension (e.g., ".zip")
        public string GetKeyForExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return FileKey;

            return _extToKey.GetOrAdd(extension, ext =>
            {
                try
                {
                    var icon = GetShellIconCore("*" + ext, isFolder: false, useAttributes: true);
                    ImageList.Images.Add(ext, icon);
                    return ext;
                }
                catch
                {
                    return FileKey;
                }
            });
        }

        // For VirtualMode: resolve to ImageIndex instead of ImageKey
        public int GetIndexForExtension(string extension)
        {
            string key = GetKeyForExtension(extension);
            int idx = ImageList.Images.IndexOfKey(key);
            if (idx < 0) idx = FileIndex;
            return idx;
        }

        public void Dispose()
        {
            ImageList?.Dispose();
        }

        private static Icon GetShellIconCore(string pathOrPattern, bool isFolder, bool useAttributes)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            uint flags = SHGFI_ICON | SHGFI_SMALLICON;
            if (useAttributes) flags |= SHGFI_USEFILEATTRIBUTES;

            uint attrs = isFolder ? FILE_ATTRIBUTE_DIRECTORY : FILE_ATTRIBUTE_NORMAL;

            IntPtr res = SHGetFileInfo(pathOrPattern, attrs, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
            if (res == IntPtr.Zero || shinfo.hIcon == IntPtr.Zero)
                return SystemIcons.WinLogo;

            try
            {
                return (Icon)Icon.FromHandle(shinfo.hIcon).Clone();
            }
            finally
            {
                DestroyIcon(shinfo.hIcon);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);
    }
}
