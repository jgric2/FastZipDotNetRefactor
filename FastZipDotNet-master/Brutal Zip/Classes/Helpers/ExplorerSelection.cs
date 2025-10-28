using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Brutal_Zip.Classes.Helpers
{
    public static class ExplorerSelection
    {
        // Call this early (before showing any UI that could steal foreground)
        public static List<string> TryGetSelectedFileSystemPaths(IEnumerable<string>? seedArgs = null, int settleMs = 20)
        {
            if (settleMs > 0)
                Thread.Sleep(settleMs); // let the context menu close

            var result = new List<string>();

            // Use the first seed as a hint to pick the correct tab/window quickly
            string seedFirst = seedArgs?.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)) ?? string.Empty;
            string seedDir = GetParentDirectoryIfPath(seedFirst);

            IntPtr fgRoot = GetAncestor(GetForegroundWindow(), GA_ROOT);

            Type? shellType = Type.GetTypeFromProgID("Shell.Application");
            if (shellType == null) return result;

            object? shellObj = null;
            object? winsObj = null;
            object? chosenWin = null;
            object? doc = null;
            object? selected = null;

            try
            {
                dynamic shell = Activator.CreateInstance(shellType)!;
                shellObj = shell;
                dynamic wins = shell.Windows();
                winsObj = wins;

                // First pass: collect Explorer windows whose HWND == foreground root (fast, no Document call)
                var matchingHwndWins = new List<object>();

                foreach (var w in wins)
                {
                    try
                    {
                        dynamic win = w;
                        long hwndVal = (long)win.HWND;
                        IntPtr hwnd = new IntPtr(hwndVal);
                        if (hwnd == IntPtr.Zero || !IsWindow(hwnd))
                            continue;

                        if (!IsExplorerTopLevel(hwnd))
                            continue;

                        if (fgRoot != IntPtr.Zero && hwnd == fgRoot)
                        {
                            matchingHwndWins.Add(win);
                        }
                    }
                    catch
                    {
                        // ignore this window and continue
                    }
                }

                // If exactly one Explorer window matches the foreground HWND, pick it
                if (matchingHwndWins.Count == 1)
                {
                    chosenWin = matchingHwndWins[0];
                }
                else if (matchingHwndWins.Count > 1 && !string.IsNullOrEmpty(seedDir))
                {
                    // Multiple tabs share the same HWND (Windows 11). Disambiguate by folder path (LocationURL)
                    foreach (var w in matchingHwndWins)
                    {
                        try
                        {
                            dynamic win = w;
                            string url = (string)win.LocationURL; // "file:///C:/...", "shell:Downloads", etc.
                            string? path = UrlToPath(url);
                            if (!string.IsNullOrEmpty(path) && PathEquals(path, seedDir))
                            {
                                chosenWin = w;
                                break;
                            }
                        }
                        catch { /* ignore and try next */ }
                    }

                    // If still not picked, take the first one
                    chosenWin ??= matchingHwndWins[0];
                }
                else
                {
                    // No match by HWND. Try to match by seed folder across all Explorer windows.
                    if (!string.IsNullOrEmpty(seedDir))
                    {
                        foreach (var w in wins)
                        {
                            try
                            {
                                dynamic win = w;
                                long hwndVal = (long)win.HWND;
                                IntPtr hwnd = new IntPtr(hwndVal);
                                if (hwnd == IntPtr.Zero || !IsWindow(hwnd))
                                    continue;
                                if (!IsExplorerTopLevel(hwnd))
                                    continue;

                                string url = (string)win.LocationURL;
                                string? path = UrlToPath(url);
                                if (!string.IsNullOrEmpty(path) && PathEquals(path, seedDir))
                                {
                                    chosenWin = w;
                                    break;
                                }
                            }
                            catch { /* ignore and continue */ }
                        }
                    }

                    // Last resort: just take the first Explorer window
                    if (chosenWin == null)
                    {
                        foreach (var w in wins)
                        {
                            try
                            {
                                dynamic win = w;
                                long hwndVal = (long)win.HWND;
                                IntPtr hwnd = new IntPtr(hwndVal);
                                if (hwnd == IntPtr.Zero || !IsWindow(hwnd))
                                    continue;
                                if (!IsExplorerTopLevel(hwnd))
                                    continue;

                                chosenWin = w;
                                break;
                            }
                            catch { /* ignore and continue */ }
                        }
                    }
                }

                if (chosenWin != null)
                {
                    // Fast path: IDataObject + CF_HDROP (no late-bound per-item calls)
                    var fast = TryGetSelectionViaHDrop(chosenWin);
                    if (fast.Count > 0)
                        return fast;

                    // Optional fallback (rarely needed): only if you still want to support non-filesystem items
                    // var slow = TryGetSelectionViaDocument(chosenWin);
                    // if (slow.Count > 0) return slow;
                }
            }
            catch
            {
                // Fail silently; return whatever we've collected
            }
            finally
            {
                TryReleaseComObject(selected);
                TryReleaseComObject(doc);
                TryReleaseComObject(winsObj);
                TryReleaseComObject(shellObj);
            }

            return result;
        }

        // Heuristic: Only treat as Explorer if the top-level class is CabinetWClass or ExploreWClass
        private static bool IsExplorerTopLevel(IntPtr hwnd)
        {
            const int max = 256;
            var sb = new StringBuilder(max);
            int len = GetClassNameW(hwnd, sb, max);
            if (len <= 0) return false;

            string cls = sb.ToString();
            return cls.Equals("CabinetWClass", StringComparison.Ordinal) ||
                   cls.Equals("ExploreWClass", StringComparison.Ordinal); // legacy
        }

        private static string GetParentDirectoryIfPath(string p)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(p)) return string.Empty;
                if (!LooksLikeFileSystemPath(p)) return string.Empty;
                string? dir = Path.GetDirectoryName(p);
                return dir ?? string.Empty;
            }
            catch { return string.Empty; }
        }

        private static string? UrlToPath(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            try
            {
                // Handle file:/// URLs; ignore "shell:" and "::{GUID}" special locations
                if (url.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = new Uri(url);
                    return uri.LocalPath;
                }
            }
            catch { /* ignore */ }
            return null;
        }

        private static bool PathEquals(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            a = NormalizeDir(a);
            b = NormalizeDir(b);
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeDir(string p)
        {
            // Remove trailing directory separator and normalize to backslashes
            p = p.Replace('/', '\\');
            if (p.EndsWith("\\", StringComparison.Ordinal) && p.Length > 3) // keep root "C:\"
                p = p.TrimEnd('\\');
            return p;
        }

        private static bool LooksLikeFileSystemPath(string? p)
        {
            if (string.IsNullOrWhiteSpace(p)) return false;
            p = p.Trim();

            // Drive letter root or child
            if (p.Length >= 3 && char.IsLetter(p[0]) && p[1] == ':' && (p[2] == '\\' || p[2] == '/'))
                return true;

            // UNC path
            if (p.StartsWith(@"\\", StringComparison.Ordinal))
                return true;

            return false;
        }

        private static void TryReleaseComObject(object? obj)
        {
            if (obj == null) return;
            try { Marshal.FinalReleaseComObject(obj); } catch { /* ignore */ }
        }

        // P/Invoke
        private const uint GA_ROOT = 2;

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindow(IntPtr hWnd);

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetClassNameW")]
        private static extern int GetClassNameW(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);


        // IDataObject -> CF_HDROP
        private static List<string> TryGetSelectionViaHDrop(object chosenWin)
        {
            var result = new List<string>();

            IntPtr unk = IntPtr.Zero;
            IntPtr pSP = IntPtr.Zero;
            IntPtr pBrowser = IntPtr.Zero;
            IShellView? view = null;
            IntPtr pDataObj = IntPtr.Zero;

            try
            {
                // QI the chosen Explorer window for IServiceProvider
                unk = Marshal.GetIUnknownForObject(chosenWin);
                if (unk == IntPtr.Zero) return result;

                Guid IID_IServiceProvider = new Guid("6d5140c1-7436-11ce-8034-00aa006009fa");
                int hr = Marshal.QueryInterface(unk, ref IID_IServiceProvider, out pSP);
                if (hr != 0 || pSP == IntPtr.Zero) return result;

                var sp = (IServiceProvider)Marshal.GetObjectForIUnknown(pSP);

                // Get IShellBrowser for the top-level browser
                Guid SID_STopLevelBrowser = new Guid("4C96BE40-915C-11CF-99D3-00AA004AE837");
                Guid IID_IShellBrowser = new Guid("000214E2-0000-0000-C000-000000000046");

                hr = sp.QueryService(ref SID_STopLevelBrowser, ref IID_IShellBrowser, out pBrowser);
                if (hr != 0 || pBrowser == IntPtr.Zero) return result;

                var browser = (IShellBrowser)Marshal.GetObjectForIUnknown(pBrowser);

                // Get the active Shell view
                hr = browser.QueryActiveShellView(out view);
                if (hr != 0 || view == null) return result;

                // Ask the view for an IDataObject describing the selection
                Guid IID_IDataObject = new Guid("0000010e-0000-0000-C000-000000000046");
                hr = view.GetItemObject((uint)SVGIO.SELECTION, ref IID_IDataObject, out pDataObj);
                if (hr != 0 || pDataObj == IntPtr.Zero) return result;

                var dataObject = (System.Runtime.InteropServices.ComTypes.IDataObject)Marshal.GetObjectForIUnknown(pDataObj);

                // Request CF_HDROP (filesystem paths)
                var fmt = new FORMATETC
                {
                    cfFormat = (short)CF_HDROP,
                    ptd = IntPtr.Zero,
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = TYMED.TYMED_HGLOBAL
                };

                STGMEDIUM medium;
                dataObject.GetData(ref fmt, out medium);

                try
                {
                    IntPtr hDrop = medium.unionmember;

                    // Number of files
                    uint count = DragQueryFile(hDrop, 0xFFFFFFFF, null, 0);
                    if (count == 0) return result;

                    var sb = new StringBuilder(260);
                    for (uint i = 0; i < count; i++)
                    {
                        uint len = DragQueryFile(hDrop, i, null, 0);
                        if (len == 0) continue;

                        if (sb.Capacity < len + 1) sb.Capacity = (int)len + 1;
                        sb.Length = 0;

                        DragQueryFile(hDrop, i, sb, sb.Capacity);
                        result.Add(sb.ToString());
                    }
                }
                finally
                {
                    ReleaseStgMedium(ref medium);
                    Marshal.ReleaseComObject(dataObject);
                }
            }
            catch
            {
                // ignore and return whatever we have
            }
            finally
            {
                if (pDataObj != IntPtr.Zero) Marshal.Release(pDataObj);
                if (view != null) Marshal.ReleaseComObject(view);
                if (pBrowser != IntPtr.Zero) Marshal.Release(pBrowser);
                if (pSP != IntPtr.Zero) Marshal.Release(pSP);
                if (unk != IntPtr.Zero) Marshal.Release(unk);
            }

            return result;
        }

        // Optional: retain your existing document-based fallback if you want it
        private static List<string> TryGetSelectionViaDocument(object chosenWin)
        {
            var result = new List<string>();
            try
            {
                dynamic d = ((dynamic)chosenWin).Document;
                dynamic sel = d.SelectedItems();
                foreach (var it in sel)
                {
                    string p = (string)((dynamic)it).Path;
                    // CF_HDROP ensures filesystem already; keep your filter if you want
                    result.Add(p);
                }
            }
            catch { }
            return result;
        }

        // Interop: IServiceProvider
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
        private interface IServiceProvider
        {
            [PreserveSig]
            int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject);
        }

        // Interop: IShellBrowser (only need QueryActiveShellView; preserve vtable order)
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214E2-0000-0000-C000-000000000046")]
        private interface IShellBrowser
        {
            [PreserveSig] int GetWindow(out IntPtr phwnd);
            [PreserveSig] int ContextSensitiveHelp(bool fEnterMode);
            [PreserveSig] int InsertMenusSB(IntPtr hmenuShared, IntPtr lpMenuWidths);
            [PreserveSig] int SetMenuSB(IntPtr hmenuShared, IntPtr holemenuRes, IntPtr hwndActiveObject);
            [PreserveSig] int RemoveMenusSB(IntPtr hmenuShared);
            [PreserveSig] int SetStatusTextSB([MarshalAs(UnmanagedType.LPWStr)] string pszStatusText);
            [PreserveSig] int EnableModelessSB(bool fEnable);
            [PreserveSig] int TranslateAcceleratorSB(ref MSG pmsg, ushort wID);
            [PreserveSig] int BrowseObject(IntPtr pidl, uint wFlags);
            [PreserveSig] int GetViewStateStream(uint grfMode, out IntPtr ppstm);
            [PreserveSig] int GetControlWindow(uint id, out IntPtr phwnd);
            [PreserveSig] int SendControlMsg(uint id, uint uMsg, IntPtr wParam, IntPtr lParam, out IntPtr pret);
            [PreserveSig] int QueryActiveShellView(out IShellView ppshv);
            [PreserveSig] int OnViewWindowActive([MarshalAs(UnmanagedType.IUnknown)] object pshv);
            [PreserveSig] int SetToolbarItems(IntPtr lpButtons, uint nButtons, uint uFlags);
        }

        // Interop: IShellView (only need GetItemObject; preserve vtable order)
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214E3-0000-0000-C000-000000000046")]
        private interface IShellView
        {
            [PreserveSig] int GetWindow(out IntPtr phwnd);
            [PreserveSig] int ContextSensitiveHelp(bool fEnterMode);
            [PreserveSig] int TranslateAccelerator(ref MSG pmsg);
            [PreserveSig] int EnableModeless(bool fEnable);
            [PreserveSig] int UIActivate(uint uState);
            [PreserveSig] int Refresh();
            [PreserveSig] int CreateViewWindow(IntPtr psvPrevious, IntPtr pfs, IntPtr psb, ref RECT prcView, out IntPtr phWnd);
            [PreserveSig] int DestroyViewWindow();
            [PreserveSig] int GetCurrentInfo(IntPtr pfs);
            [PreserveSig] int AddPropertySheetPages(uint dwReserved, IntPtr pfn, IntPtr lparam);
            [PreserveSig] int SaveViewState();
            [PreserveSig] int SelectItem(IntPtr pidlItem, uint uFlags);
            [PreserveSig] int GetItemObject(uint uItem, ref Guid riid, out IntPtr ppv);
        }

        // P/Invoke for CF_HDROP enumeration
        private const ushort CF_HDROP = 15;

        private enum SVGIO : uint
        {
            BACKGROUND = 0x00000000,
            SELECTION = 0x00000001,
            ALLVIEW = 0x00000002,
            CHECKED = 0x00000003,
            TYPEMASK = 0x0000000F
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint DragQueryFile(IntPtr hDrop, uint iFile, StringBuilder? lpszFile, int cch);

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("ole32.dll")]
        private static extern void ReleaseStgMedium(ref STGMEDIUM pmedium);

        // Ancillary structs to keep vtable signatures correct
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int x, y; }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int left, top, right, bottom; }


    }
}
