using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Brutal_Zip.Controls.BrutalControls.FileOrFolderDialog
{
    public class FileExplorer
    {
        public FileExplorer()
        {
        }

        public enum KnownFolder
        {
            Documents,
            Downloads,
            Music,
            Pictures,
            SavedGames,
            // ...
        }

        private static readonly Dictionary<KnownFolder, Guid> _knownFolderGuids = new()
        {
            [KnownFolder.Documents] = new("FDD39AD0-238F-46AF-ADB4-6C85480369C7"),
            [KnownFolder.Downloads] = new("374DE290-123F-4565-9164-39C4925E467B"),
            [KnownFolder.Music] = new("4BD8D571-6D19-48D3-BE97-422220080E43"),
            [KnownFolder.Pictures] = new("33E28130-4E1E-4676-835A-98395C3BC3BB"),
            [KnownFolder.SavedGames] = new("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"),
        };

        public static string GetSpecialPath(KnownFolder folder)
        {
            return SHGetKnownFolderPath(_knownFolderGuids[folder], 0);
        }


        public bool CreateTree(TreeView treeView)
        {
            bool returnValue = false;

            try
            {
                // Initialize ImageList with Color Depth and Transparency
                if (treeView.ImageList == null)
                {
                    treeView.ImageList = new ImageList
                    {
                        ColorDepth = ColorDepth.Depth32Bit,
                        TransparentColor = Color.Transparent
                    };
                }

                // Attach the BeforeExpand event
                treeView.BeforeExpand += TreeView_BeforeExpand;

                // Create Desktop
                TreeNode desktop = new TreeNode
                {
                    Text = "Desktop",
                    Tag = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };
                desktop.Nodes.Add(new TreeNode()); // Dummy node for expansion

                treeView.Nodes.Add(desktop);

                // Create Documents
                TreeNode documents = new TreeNode
                {
                    Text = "Documents",
                    Tag = GetSpecialPath(KnownFolder.Documents)
                };
                documents.Nodes.Add(new TreeNode()); // Dummy node for expansion

                treeView.Nodes.Add(documents);


                // Create Downloads
                TreeNode downloads = new TreeNode
                {
                    Text = "Downloads",
                    Tag = GetSpecialPath(KnownFolder.Downloads)
                };
                downloads.Nodes.Add(new TreeNode()); // Dummy node for expansion

                treeView.Nodes.Add(downloads);

                // Create Pictures
                TreeNode pictures = new TreeNode
                {
                    Text = "Pictures",
                    Tag = GetSpecialPath(KnownFolder.Pictures)
                };
                pictures.Nodes.Add(new TreeNode()); // Dummy node for expansion

                treeView.Nodes.Add(pictures);


                // Create Pictures
                TreeNode video = new TreeNode
                {
                    Text = "Videos",
                    Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
                };
                video.Nodes.Add(new TreeNode()); // Dummy node for expansion

                treeView.Nodes.Add(video);

                // Create Music
                TreeNode music = new TreeNode
                {
                    Text = "Music",
                    Tag = GetSpecialPath(KnownFolder.Music)
                };
                music.Nodes.Add(new TreeNode()); // Dummy node for expansion

                treeView.Nodes.Add(music);

                // Get driveInfo
                foreach (DriveInfo drv in DriveInfo.GetDrives())
                {
                    TreeNode fChild = new TreeNode
                    {
                        Text = drv.Name,
                        Tag = drv.Name
                    };
                    Icon icon = ExtractIconFromPath(drv.Name);
                    if (icon != null)
                    {
                        treeView.ImageList.Images.Add(icon);
                        fChild.ImageIndex = treeView.ImageList.Images.Count - 1;
                        fChild.SelectedImageIndex = fChild.ImageIndex;
                    }
                    fChild.Nodes.Add(new TreeNode()); // Dummy node for expansion

                    treeView.Nodes.Add(fChild);
                    returnValue = true;
                }




            }
            catch (Exception ex)
            {
                // Handle exception properly (e.g., logging)
                returnValue = false;
            }
            return returnValue;
        }





        [DllImport("shell32", CharSet = CharSet.Unicode,
            ExactSpelling = true, PreserveSig = false)]
        private static extern string SHGetKnownFolderPath(
    [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
               uint dwFlags, nint hToken = default);

        private void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "")
            {
                e.Node.Nodes.Clear();
                EnumerateDirectory(e.Node);
            }
        }

        // Add files
        Dictionary<string, Icon> iconCache = new Dictionary<string, Icon>();
        public TreeNode EnumerateDirectory(TreeNode parentNode)
        {
            try
            {
                DirectoryInfo rootDir;
                string path = parentNode.Tag.ToString();

                if (parentNode.Nodes.Count > 0)
                    if (parentNode.Nodes[0].Text == "")
                    {
                        parentNode.Nodes.RemoveAt(0);
                    }

                rootDir = new DirectoryInfo(path);

                // Add directories
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                {
                    TreeNode node = new TreeNode
                    {
                        Text = dir.Name,
                        Tag = dir.FullName
                    };
                    Icon icon; //= ExtractIconFromPath(dir.FullName);
                    if (iconCache.ContainsKey("?folderIcon@@"))
                    {
                        icon = iconCache["?folderIcon@@"];
                    }
                    else
                    {
                        icon = ExtractIconFromPath(dir.FullName);
                        iconCache.Add("?folderIcon@@", icon);
                    }

                    if (icon != null)
                    {
                        if (parentNode.TreeView.ImageList.Images.ContainsKey("?folderIcon@@"))
                        {
                            node.ImageIndex = parentNode.TreeView.ImageList.Images.IndexOfKey("?folderIcon@@");
                            node.SelectedImageIndex = node.ImageIndex;
                        }
                        else
                        {
                            parentNode.TreeView.ImageList.Images.Add("?folderIcon@@", icon);
                            node.ImageIndex = parentNode.TreeView.ImageList.Images.Count - 1;
                            node.SelectedImageIndex = node.ImageIndex;
                        }

                    }
                    node.Nodes.Add(new TreeNode()); // Dummy node for further expansion
                    parentNode.Nodes.Add(node);
                }



                foreach (FileInfo file in rootDir.GetFiles())
                {
                    TreeNode node = new TreeNode
                    {
                        Text = file.Name,
                        Tag = file.FullName
                    };

                    Icon icon;
                    string ext = Path.GetExtension(file.FullName);
                    if (iconCache.ContainsKey(ext))
                    {
                        icon = iconCache[ext];
                    }
                    else if (ext == ".lnk" || ext == ".exe")
                    {
                        icon = ExtractIconFromPath(file.FullName);
                    }
                    else
                    {
                        icon = ExtractIconFromPath(file.FullName);
                        iconCache.Add(ext, icon);
                    }

                    // Icon icon = ExtractIconFromPath(file.FullName);
                    if (icon != null)
                    {
                        if (parentNode.TreeView.ImageList.Images.ContainsKey(ext))
                        {
                            node.ImageIndex = parentNode.TreeView.ImageList.Images.IndexOfKey(ext);
                            node.SelectedImageIndex = node.ImageIndex;
                        }
                        else if (ext == ".lnk" || ext == ".exe")
                        {
                            parentNode.TreeView.ImageList.Images.Add(icon);
                            node.ImageIndex = parentNode.TreeView.ImageList.Images.Count - 1;
                            node.SelectedImageIndex = node.ImageIndex;
                        }
                        else
                        {
                            parentNode.TreeView.ImageList.Images.Add(ext, icon);
                            node.ImageIndex = parentNode.TreeView.ImageList.Images.Count - 1;
                            node.SelectedImageIndex = node.ImageIndex;
                        }

                    }
                    parentNode.Nodes.Add(node);
                }
            }
            catch (Exception ex)
            {
                // Handle exception properly (e.g., logging)
            }

            return parentNode;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_SMALLICON = 0x001;
        private const uint SHGFI_LARGEICON = 0x000;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x10;

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
        };

        private Icon ExtractIconFromPath(string path)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            uint flags = SHGFI_ICON | SHGFI_LARGEICON;
            IntPtr hImg = SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);

            if (hImg != IntPtr.Zero && shinfo.hIcon != IntPtr.Zero)
            {
                Icon icon = Icon.FromHandle(shinfo.hIcon).Clone() as Icon;
                DestroyIcon(shinfo.hIcon); // Release the handle
                return icon;
            }
            return null;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);
    }
}
