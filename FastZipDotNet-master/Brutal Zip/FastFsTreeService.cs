using System.Runtime.InteropServices;

namespace Brutal_Zip
{
    internal sealed class FastFsTreeService
    {
        private readonly TreeView _tv;
        private readonly int _folderIconIndex;

        public FastFsTreeService(TreeView tv, int folderIconIndex)
        {
            _tv = tv ?? throw new ArgumentNullException(nameof(tv));
            _folderIconIndex = folderIconIndex;

            // Double-buffer to avoid flicker when adding many nodes
            EnableDoubleBuffer(_tv);

            // Hook expand
            _tv.BeforeExpand += Tv_BeforeExpand;
        }

        public void BuildRoots()
        {
            _tv.BeginUpdate();
            try
            {
                _tv.Nodes.Clear();

                // Desktop
                AddKnown("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

                // Common known folders
                TryAddKnown("Documents", GetKnownFolderPath(Documents));
                TryAddKnown("Downloads", GetKnownFolderPath(Downloads));
                TryAddKnown("Pictures", GetKnownFolderPath(Pictures));
                TryAddKnown("Music", GetKnownFolderPath(Music));
                TryAddKnown("Videos", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));

                // Drives
                foreach (var drive in DriveInfo.GetDrives())
                {
                    // Only show ready fixed/removable/network; adjust per taste
                    bool show =
                        drive.DriveType == DriveType.Fixed ||
                        drive.DriveType == DriveType.Removable ||
                        (drive.DriveType == DriveType.Network && drive.IsReady) ||
                        (drive.DriveType == DriveType.CDRom && drive.IsReady);

                    if (!show) continue;

                    AddRootNode(drive.Name.TrimEnd('\\'), drive.Name);
                }
            }
            finally
            {
                _tv.EndUpdate();
            }
        }

        private void Tv_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            var node = e.Node;
            if (node == null) return;

            // Only populate when a single dummy child exists
            if (node.Nodes.Count == 1 && node.Nodes[0].Tag == null)
            {
                var basePath = node.Tag as string;
                PopulateNode(node, basePath);
            }
        }

        private void PopulateNode(TreeNode node, string? basePath)
        {
            if (string.IsNullOrWhiteSpace(basePath) || !Directory.Exists(basePath))
            {
                // Clear dummy
                _tv.BeginUpdate();
                try { node.Nodes.Clear(); }
                finally { _tv.EndUpdate(); }
                return;
            }

            var children = new List<TreeNode>(256);
            try
            {
                // One enumeration only; no per-child probes
                foreach (var dir in Directory.GetDirectories(basePath))
                {
                    // Skip hidden/system/reparse points to avoid slow places (adjust to taste)
                    try
                    {
                        var attr = File.GetAttributes(dir);
                        if ((attr & FileAttributes.Hidden) != 0) continue;
                        if ((attr & FileAttributes.System) != 0) continue;
                        if ((attr & FileAttributes.ReparsePoint) != 0) continue;
                    }
                    catch { /* ignore attribute read errors */ }

                    string name;
                    try
                    {
                        name = Path.GetFileName(dir.TrimEnd('\\', '/'));
                        if (string.IsNullOrEmpty(name)) name = dir;
                    }
                    catch { continue; }

                    var child = new TreeNode(name)
                    {
                        Tag = dir,
                        ImageIndex = _folderIconIndex,
                        SelectedImageIndex = _folderIconIndex
                    };

                    // Always add a dummy so we never probe for “has subdirs”
                    child.Nodes.Add(new TreeNode("(loading)") { Tag = null });

                    children.Add(child);
                }
            }
            catch
            {
                // ignore bad folders
            }

            _tv.BeginUpdate();
            try
            {
                node.Nodes.Clear();            // clear the original dummy
                node.Nodes.AddRange(children.ToArray());
            }
            finally
            {
                _tv.EndUpdate();
            }
        }

        private void AddKnown(string text, string? path)
        {
            TryAddKnown(text, path);
        }

        private void TryAddKnown(string text, string? path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) return;
            AddRootNode(text, path);
        }

        private void AddRootNode(string text, string path)
        {
            var n = new TreeNode(text)
            {
                Tag = path,
                ImageIndex = _folderIconIndex,
                SelectedImageIndex = _folderIconIndex
            };
            // Dummy for lazy expand
            n.Nodes.Add(new TreeNode("(loading)") { Tag = null });
            _tv.Nodes.Add(n);
        }

        private static void EnableDoubleBuffer(TreeView tv)
        {
            try
            {
                // Set styles to reduce flicker (works since .NET 4.5+)
                //tv.SetStyle(ControlStyles.AllPaintingInWmPaint |
                //            ControlStyles.OptimizedDoubleBuffer |
                //            ControlStyles.UserPaint, false);

                // Also set extended style TVS_EX_DOUBLEBUFFER
                const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
                const int TVS_EX_DOUBLEBUFFER = 0x0004;
                SendMessage(tv.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
            }
            catch { /* ignore */ }
        }

        // Known folders we use via SHGetKnownFolderPath
        private static readonly Guid Documents = new("FDD39AD0-238F-46AF-ADB4-6C85480369C7");
        private static readonly Guid Downloads = new("374DE290-123F-4565-9164-39C4925E467B");
        private static readonly Guid Pictures = new("33E28130-4E1E-4676-835A-98395C3BC3BB");
        private static readonly Guid Music = new("4BD8D571-6D19-48D3-BE97-422220080E43");

        private static string? GetKnownFolderPath(Guid knownFolderId)
        {
            try { return SHGetKnownFolderPath(knownFolderId, 0, IntPtr.Zero); }
            catch { return null; }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        private static extern string SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
                                                          uint dwFlags, IntPtr hToken);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}