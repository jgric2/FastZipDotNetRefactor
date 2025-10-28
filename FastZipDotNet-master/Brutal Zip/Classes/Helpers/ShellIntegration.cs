using Microsoft.Win32;

namespace Brutal_Zip.Classes.Helpers
{
    internal static class ShellIntegration
    {
        // Keys (per-user)
        private const string KeyFilesZip = @"Software\Classes\*\shell\BrutalZip.Zip";
        private const string KeyFilesZipCmd = @"Software\Classes\*\shell\BrutalZip.Zip\command";

        private const string KeyDirZip = @"Software\Classes\Directory\shell\BrutalZip.Zip";
        private const string KeyDirZipCmd = @"Software\Classes\Directory\shell\BrutalZip.Zip\command";

        private const string KeyZipAssocSmart = @"Software\Classes\SystemFileAssociations\.zip\shell\BrutalZip.UnzipSmart";
        private const string KeyZipAssocSmartCmd = @"Software\Classes\SystemFileAssociations\.zip\shell\BrutalZip.UnzipSmart\command";

        private const string KeyZipAssocHere = @"Software\Classes\SystemFileAssociations\.zip\shell\BrutalZip.UnzipHere";
        private const string KeyZipAssocHereCmd = @"Software\Classes\SystemFileAssociations\.zip\shell\BrutalZip.UnzipHere\command";

        public static void Install()
        {
            string exe = GetExePath();

            // Files: Zip using Brutal Zip (opens app and stages selection)
            using (var k = Registry.CurrentUser.CreateSubKey(KeyFilesZip))
            {
                k.SetValue("MUIVerb", "Zip using Brutal Zip");
                k.SetValue("Icon", exe);
            }
            using (var k = Registry.CurrentUser.CreateSubKey(KeyFilesZipCmd))
            {
                // NOTE: For multiple selection, Windows may invoke the command once per item.
                // App accepts "--stage" and any number of paths.
                k.SetValue(null, $"\"{exe}\" --stage \"%1\"");
            }

            // Directory: Zip using Brutal Zip
            using (var k = Registry.CurrentUser.CreateSubKey(KeyDirZip))
            {
                k.SetValue("MUIVerb", "Zip using Brutal Zip");
                k.SetValue("Icon", exe);
            }
            using (var k = Registry.CurrentUser.CreateSubKey(KeyDirZipCmd))
            {
                k.SetValue(null, $"\"{exe}\" --stage \"%1\"");
            }

            // .zip: Unzip (Smart)
            using (var k = Registry.CurrentUser.CreateSubKey(KeyZipAssocSmart))
            {
                k.SetValue("MUIVerb", "Unzip (Smart) using Brutal Zip");
                k.SetValue("Icon", exe);
            }
            using (var k = Registry.CurrentUser.CreateSubKey(KeyZipAssocSmartCmd))
            {
                k.SetValue(null, $"\"{exe}\" -x \"%1\"");
            }

            // .zip: Unzip Here
            using (var k = Registry.CurrentUser.CreateSubKey(KeyZipAssocHere))
            {
                k.SetValue("MUIVerb", "Unzip Here using Brutal Zip");
                k.SetValue("Icon", exe);
            }
            using (var k = Registry.CurrentUser.CreateSubKey(KeyZipAssocHereCmd))
            {
                k.SetValue(null, $"\"{exe}\" -xh \"%1\"");
            }
        }

        public static void Uninstall()
        {
            try { Registry.CurrentUser.DeleteSubKeyTree(KeyFilesZip, throwOnMissingSubKey: false); } catch { }
            try { Registry.CurrentUser.DeleteSubKeyTree(KeyDirZip, throwOnMissingSubKey: false); } catch { }
            try { Registry.CurrentUser.DeleteSubKeyTree(KeyZipAssocSmart, throwOnMissingSubKey: false); } catch { }
            try { Registry.CurrentUser.DeleteSubKeyTree(KeyZipAssocHere, throwOnMissingSubKey: false); } catch { }
        }

        private static string GetExePath()
        {
            string exe = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(exe)) exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BrutalZip.exe");
            return exe;
        }
    }
}
