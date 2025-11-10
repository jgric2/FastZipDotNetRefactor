using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Brutal_Zip.Classes.Helpers
{
    internal static class ShellIntegration
    {
        // Single, unified parent in classic menu
        private const string ParentKey = @"Software\Classes\AllFileSystemObjects\shell\BrutalZip";

        // Optional: Windows 11 classic (Win10) context menu switch (same CLSID and technique RightClickTools uses)
        private const string Win11ClassicContextMenuKey = @"Software\Classes\CLSID\{86CA1AA0-34AA-4E8B-A509-50C905BAE2A2}\InprocServer32";

        public static void Install()
        {
            string exe = GetExePath();

            // Clean up previous layouts you might have created
            SafeDeleteTree(ParentKey);

            // Create parent (cascaded)
            using (var parent = Registry.CurrentUser.CreateSubKey(ParentKey))
            {
                // This tells Explorer it’s a cascaded item with subcommands under "shell\..."
                parent.SetValue("SubCommands", "", RegistryValueKind.String);
                parent.SetValue("MUIVerb", "Brutal Zip", RegistryValueKind.String);
                parent.SetValue("Icon", exe, RegistryValueKind.String);
                // Ensures it shows even with large multi-selection
                parent.SetValue("MultiSelectModel", "Player", RegistryValueKind.String);
            }

            // Create subcommands directly under ...\shell\
            using (var shell = Registry.CurrentUser.CreateSubKey($@"{ParentKey}\shell"))
            {
                // CREATE group (always visible)
                // We don’t use a second-level submenu for “Create” here — RightClickTools pattern is flatter and very reliable.
                // CREATE
                // CREATE
                CreateVerb(shell, "10-CreateQuick", "Compress using Brutal Zip", exe, $"\"{exe}\" --create-quick \"%1\"");
                CreateVerb(shell, "11-CreateTo", "Compress to...", exe, $"\"{exe}\" --create-to \"%1\"");
                CreateVerb(shell, "12-CreateEach", "Compress each selected item", exe, $"\"{exe}\" --create-each \"%1\"");
                CreateVerb(shell, "13-OpenWith", "Open Brutal Zip with selection", exe, $"\"{exe}\" --stage \"%1\"");

                // EXTRACT (only when a .zip is selected)
                const string OnlyZip = "System.FileExtension:=.zip";
                CreateVerb(shell, "20-ExtractHere", "Extract Here", exe, $"\"{exe}\" --extract-here \"%1\"", appliesTo: OnlyZip);
                CreateVerb(shell, "21-ExtractSmart", "Extract (Smart)", exe, $"\"{exe}\" --extract-smart \"%1\"", appliesTo: OnlyZip);
                CreateVerb(shell, "22-ExtractTo", "Extract to...", exe, $"\"{exe}\" --extract-to \"%1\"", appliesTo: OnlyZip);
                CreateVerb(shell, "23-ExtractEach", "Extract each selected zip", exe, $"\"{exe}\" --extract-each \"%1\"", appliesTo: OnlyZip);
            }

            RefreshExplorer();
        }

        public static void Uninstall()
        {
            SafeDeleteTree(ParentKey);
            RefreshExplorer();
        }

        // Optional: if you want to provide a toggle in Settings to show Win10-like classic menu on Win11
        public static void SetWin11ClassicContextMenu(bool enable)
        {
            // Add empty default value = enable; remove key = disable
            if (IsWindows11())
            {
                if (enable)
                {
                    using var k = Registry.CurrentUser.CreateSubKey(Win11ClassicContextMenuKey);
                    k.SetValue("", "", RegistryValueKind.String);
                }
                else
                {
                    SafeDeleteTree(Win11ClassicContextMenuKey);
                }
                RestartExplorer();
            }
        }

        private static void CreateVerb(RegistryKey parentShell, string keyName, string title, string exe, string command, string? appliesTo = null)
        {
            using var verb = parentShell.CreateSubKey(keyName);
            verb.SetValue("MUIVerb", title, RegistryValueKind.String);
            verb.SetValue("Icon", exe, RegistryValueKind.String);
            if (!string.IsNullOrEmpty(appliesTo))
                verb.SetValue("AppliesTo", appliesTo, RegistryValueKind.String);

            using var cmd = verb.CreateSubKey("command");
            cmd.SetValue(null, command, RegistryValueKind.String);
        }

        private static string GetExePath()
        {
            try
            {
                string exe = Process.GetCurrentProcess().MainModule?.FileName ?? AppDomain.CurrentDomain.BaseDirectory;
                if (!File.Exists(exe)) exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BrutalZip.exe");
                return exe;
            }
            catch
            {
                return "BrutalZip.exe";
            }
        }

        private static void SafeDeleteTree(string key)
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(key, false);
            }
            catch
            {
                var tt = "";
            }
        }

        private static bool IsWindows11()
        {
            try
            {
                var val = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion")?
                                      .GetValue("CurrentBuildNumber")?.ToString();
                if (int.TryParse(val, out int build)) return build >= 21996;
            }
            catch { }
            return false;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        private static void RefreshExplorer()
        {
            try { SHChangeNotify(0x08000000, 0x1000, IntPtr.Zero, IntPtr.Zero); } catch { }
        }

        private static void RestartExplorer()
        {
            try
            {
                foreach (var proc in Process.GetProcessesByName("explorer"))
                {
                    try { proc.Kill(); proc.WaitForExit(); } catch { }
                }
                Process.Start("explorer.exe");
            }
            catch { }
        }
    }
}








//using Microsoft.Win32;
//using System.Runtime.InteropServices;

//namespace Brutal_Zip.Classes.Helpers
//{
//    internal static class ShellIntegration
//    {
//        // COMPRESS for both files and folders (one place; visible even for large selections)
//        private const string CompressAllFSKey = @"Software\Classes\AllFileSystemObjects\shell\BrutalZip.Compress";

//        // EXTRACT for .zip (two places to cover built-in handler and SFA)
//        private const string ExtractZipSFAKey = @"Software\Classes\SystemFileAssociations\.zip\shell\BrutalZip.Extract";
//        private const string ExtractZipCFKey = @"Software\Classes\CompressedFolder\shell\BrutalZip.Extract";

//        // Clean-up keys from earlier experiments (optional)
//        private static readonly string[] OldKeys =
//        {
//        @"Software\Classes\*\shell\BrutalZip.Compress",
//        @"Software\Classes\Directory\shell\BrutalZip.Compress",
//        @"Software\Classes\*\shell\BrutalZip",
//        @"Software\Classes\Directory\shell\BrutalZip",
//        @"Software\Classes\SystemFileAssociations\.zip\shell\BrutalZip",
//        @"Software\Classes\CompressedFolder\shell\BrutalZip",
//        @"Software\Classes\*\shell\BrutalZip.Test",
//        @"Software\Classes\Directory\shell\BrutalZip.Test"
//    };

//        public static void Install()
//        {
//            string exe = GetExePath();

//            // Clean up older shapes
//            foreach (var k in OldKeys)
//                try { Registry.CurrentUser.DeleteSubKeyTree(k, false); } catch { }

//            // 1) Compress verb for files + folders
//            // MultiSelectModel=Player keeps the verb visible for >15 items. We pass only %1; the app will fetch full selection.
//            // Compress (visible for large multi-selection; we pass only %1)
//            CreateVerb(
//                key: CompressAllFSKey,
//                title: "Compress using Brutal Zip...",
//                exe: exe,
//                command: $"\"{exe}\" --stage \"%1\"",
//                multiSelectModel: "Player");

//            // Extract -> open viewer (user will extract from UI)
//            CreateVerb(ExtractZipSFAKey, "Extract Zip using Brutal Zip...", exe, $"\"{exe}\" \"%1\"");
//            CreateVerb(ExtractZipCFKey, "Extract Zip using Brutal Zip...", exe, $"\"{exe}\" \"%1\"");

//            RefreshExplorer();
//        }

//        public static void Uninstall()
//        {
//            try { Registry.CurrentUser.DeleteSubKeyTree(CompressAllFSKey, false); } catch { }
//            try { Registry.CurrentUser.DeleteSubKeyTree(ExtractZipSFAKey, false); } catch { }
//            try { Registry.CurrentUser.DeleteSubKeyTree(ExtractZipCFKey, false); } catch { }
//            RefreshExplorer();
//        }

//        private static void CreateVerb(string key, string title, string exe, string command, string multiSelectModel = null)
//        {
//            using var k = Registry.CurrentUser.CreateSubKey(key);
//            k.SetValue("MUIVerb", title, RegistryValueKind.String);
//            k.SetValue("Icon", exe, RegistryValueKind.String);
//            if (!string.IsNullOrEmpty(multiSelectModel))
//                k.SetValue("MultiSelectModel", multiSelectModel, RegistryValueKind.String);
//            using var cmd = k.CreateSubKey("command");
//            cmd.SetValue(null, command, RegistryValueKind.String);
//        }

//        private static string GetExePath()
//        {
//            string exe = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? AppDomain.CurrentDomain.BaseDirectory;
//            if (!File.Exists(exe)) exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BrutalZip.exe");
//            return exe;
//        }

//        private static void RefreshExplorer()
//        {
//            try { SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero); } catch { }
//        }

//        [DllImport("shell32.dll")]
//        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
//    }
//}