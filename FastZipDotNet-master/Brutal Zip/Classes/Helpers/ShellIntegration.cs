using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Brutal_Zip.Classes.Helpers
{
    internal static class ShellIntegration
    {
        // COMPRESS for both files and folders (one place; visible even for large selections)
        private const string CompressAllFSKey = @"Software\Classes\AllFileSystemObjects\shell\BrutalZip.Compress";

        // EXTRACT for .zip (two places to cover built-in handler and SFA)
        private const string ExtractZipSFAKey = @"Software\Classes\SystemFileAssociations\.zip\shell\BrutalZip.Extract";
        private const string ExtractZipCFKey = @"Software\Classes\CompressedFolder\shell\BrutalZip.Extract";

        // Clean-up keys from earlier experiments (optional)
        private static readonly string[] OldKeys =
        {
        @"Software\Classes\*\shell\BrutalZip.Compress",
        @"Software\Classes\Directory\shell\BrutalZip.Compress",
        @"Software\Classes\*\shell\BrutalZip",
        @"Software\Classes\Directory\shell\BrutalZip",
        @"Software\Classes\SystemFileAssociations\.zip\shell\BrutalZip",
        @"Software\Classes\CompressedFolder\shell\BrutalZip",
        @"Software\Classes\*\shell\BrutalZip.Test",
        @"Software\Classes\Directory\shell\BrutalZip.Test"
    };

        public static void Install()
        {
            string exe = GetExePath();

            // Clean up older shapes
            foreach (var k in OldKeys)
                try { Registry.CurrentUser.DeleteSubKeyTree(k, false); } catch { }

            // 1) Compress verb for files + folders
            // MultiSelectModel=Player keeps the verb visible for >15 items. We pass only %1; the app will fetch full selection.
            // Compress (visible for large multi-selection; we pass only %1)
            CreateVerb(
                key: CompressAllFSKey,
                title: "Compress using Brutal Zip...",
                exe: exe,
                command: $"\"{exe}\" --stage \"%1\"",
                multiSelectModel: "Player");

            // Extract -> open viewer (user will extract from UI)
            CreateVerb(ExtractZipSFAKey, "Extract Zip using Brutal Zip...", exe, $"\"{exe}\" \"%1\"");
            CreateVerb(ExtractZipCFKey, "Extract Zip using Brutal Zip...", exe, $"\"{exe}\" \"%1\"");

            RefreshExplorer();
        }

        public static void Uninstall()
        {
            try { Registry.CurrentUser.DeleteSubKeyTree(CompressAllFSKey, false); } catch { }
            try { Registry.CurrentUser.DeleteSubKeyTree(ExtractZipSFAKey, false); } catch { }
            try { Registry.CurrentUser.DeleteSubKeyTree(ExtractZipCFKey, false); } catch { }
            RefreshExplorer();
        }

        private static void CreateVerb(string key, string title, string exe, string command, string multiSelectModel = null)
        {
            using var k = Registry.CurrentUser.CreateSubKey(key);
            k.SetValue("MUIVerb", title, RegistryValueKind.String);
            k.SetValue("Icon", exe, RegistryValueKind.String);
            if (!string.IsNullOrEmpty(multiSelectModel))
                k.SetValue("MultiSelectModel", multiSelectModel, RegistryValueKind.String);
            using var cmd = k.CreateSubKey("command");
            cmd.SetValue(null, command, RegistryValueKind.String);
        }

        private static string GetExePath()
        {
            string exe = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(exe)) exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BrutalZip.exe");
            return exe;
        }

        private static void RefreshExplorer()
        {
            try { SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero); } catch { }
        }

        [DllImport("shell32.dll")]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}