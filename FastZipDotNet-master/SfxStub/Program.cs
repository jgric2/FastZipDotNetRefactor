using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using FastZipDotNet.Zip;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;

namespace SfxStub
{
    internal static class Program
    {
        private const string Magic = "BXZSFX10";
        private const int FooterSize = 8 + 4 + 8;     // magic(8) + configLen(4) + zipLen(8)
        private const int DotnetBundleFooterSize = 8; // single-file bundle footer that MUST remain last

        [STAThread]
        private static void Main(string[] args)
        {
            NativeResolver.Register();

            try
            {
                Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            }
            catch { /* ignore if not available */ }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                string exePath = GetExecutablePath();
                if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
                    throw new InvalidOperationException("Cannot locate executable path.");

                using var fs = new FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                if (fs.Length < FooterSize + 8)
                {
                    ShowNoPayloadMessage();
                    return;
                }

                // First try legacy location (EOF - FooterSize)
                long fileLen = fs.Length;
                long footerPosLegacy = fileLen - FooterSize;
                bool found = TryReadFooterAt(fs, footerPosLegacy, out uint configLen, out ulong zipLen);

                bool usedDotnetTailAware = false;
                if (!found)
                {
                    // Try our new safe layout (EOF - 8 - FooterSize), where we preserved the .NET bundle 8-byte tail
                    long footerPosWithDotnetTail = fileLen - DotnetBundleFooterSize - FooterSize;
                    if (footerPosWithDotnetTail >= 0)
                    {
                        found = TryReadFooterAt(fs, footerPosWithDotnetTail, out configLen, out zipLen);
                        if (found) usedDotnetTailAware = true;
                    }
                }

                if (!found)
                {
                    // No SFX payload attached; just exit gracefully (and inform)
                    ShowNoPayloadMessage();
                    return;
                }

                long zipStart, configStart;
                if (usedDotnetTailAware)
                {
                    zipStart = fileLen - DotnetBundleFooterSize - FooterSize - (long)zipLen;
                    configStart = zipStart - configLen;
                }
                else
                {
                    zipStart = fileLen - FooterSize - (long)zipLen;
                    configStart = zipStart - configLen;
                }

                if (configStart < 0 || zipStart < 0)
                    throw new InvalidOperationException("Invalid SFX layout (negative offsets).");

                // Read config JSON
                fs.Seek(configStart, SeekOrigin.Begin);
                var cfgBytes = new byte[configLen];
                ReadExact(fs, cfgBytes, 0, cfgBytes.Length);
                var cfg = JsonSerializer.Deserialize<SfxConfig>(cfgBytes) ?? new SfxConfig();

                // Branding resources for UI
                var icon = TryLoadIcon(cfg.IconBase64);
                var banner = TryLoadImage(cfg.BannerImageBase64);
                var theme = TryParseColor(cfg.ThemeColorStart);
                var themeEnd = TryParseColor(cfg.ThemeColorEnd);
                // Elevation if needed
                if (cfg.RequireElevation && !IsElevated())
                {
                    RelaunchElevated(exePath);
                    return;
                }

                // License
                if (!string.IsNullOrWhiteSpace(cfg.LicenseText) && !cfg.Silent)
                {
                    using var lic = new LicenseForm(banner, icon, cfg.Title, cfg.CompanyName, cfg.LicenseText);
                    var r = lic.ShowDialog();
                    if (cfg.RequireLicenseAccept)
                    {
                        if (r != DialogResult.OK || !lic.Accepted)
                            return;
                    }
                    else
                    {
                        if (r != DialogResult.OK) return;
                    }
                }

                // Resolve extract dir
                string title = string.IsNullOrWhiteSpace(cfg.Title) ? "Self Extracting Archive" : cfg.Title;
                string extractDir = ExpandMacros(cfg.DefaultExtractDir, exePath, title);

                if (!cfg.Silent)
                {
                    using var prompt = new PromptForm(banner, icon, theme, themeEnd, title, cfg.CompanyName);
                    prompt.SetFolder(extractDir);
                    var r = prompt.ShowDialog();
                    if (r != DialogResult.OK) return;
                    extractDir = prompt.Folder;
                }
                else
                {
                    try { Directory.CreateDirectory(extractDir); } catch { }
                }

                // Copy appended zip region to a temp file
                string tmpZip = Path.Combine(Path.GetTempPath(), "sfx_" + Guid.NewGuid().ToString("N") + ".zip");
                fs.Seek(zipStart, SeekOrigin.Begin);
                using (var outZip = new FileStream(tmpZip, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    CopyLimit(fs, outZip, (long)zipLen);
                }

                // Inspect zip for encryption and prompt password if needed
                string passwordToUse = cfg.Password;
                using (var z = new FastZipDotNet.Zip.FastZipDotNet(tmpZip, Compression.Deflate, 6, 1))
                {
                    if (z.ZipFileEntries.Any(e => e.IsEncrypted) && string.IsNullOrEmpty(passwordToUse))
                    {
                        using var pwd = new PasswordDialog(banner, icon, cfg.Title, "This archive is password-protected.");
                        if (pwd.ShowDialog() != DialogResult.OK) { try { File.Delete(tmpZip); } catch { } return; }
                        passwordToUse = pwd.Password;
                    }
                }

                // Extract with progress
                using (var zip = new FastZipDotNet.Zip.FastZipDotNet(tmpZip, Compression.Deflate, 6, Math.Max(1, Environment.ProcessorCount - 1)))
                {
                    if (!string.IsNullOrEmpty(passwordToUse))
                        zip.Password = passwordToUse;

                    if (!cfg.Overwrite && Directory.Exists(extractDir) && Directory.EnumerateFileSystemEntries(extractDir).Any())
                        throw new IOException("Destination exists and overwrite is disabled.");

                    zip.SetMaxConcurrency(Math.Max(1, Environment.ProcessorCount - 1));

                    if (!cfg.Silent)
                    {
                        using var pf = new ProgressForm(title, banner, icon, theme, themeEnd, cfg.ShowFileList);
                        pf.LabelTitle.Text = string.IsNullOrWhiteSpace(cfg.CompanyName) ? title : $"{cfg.CompanyName} — {title}";
                        var progress = pf.CreateProgress();

                        pf.Show();

                        var cts = pf.TokenSource;
                        var task = zip.ExtractArchiveAsync(extractDir, progress, cts.Token);

                        while (!task.IsCompleted)
                        {
                            Application.DoEvents();
                            System.Threading.Thread.Sleep(25);
                            if (pf.IsDisposed) break;
                        }

                        try
                        {
                            task.GetAwaiter().GetResult();
                            if (!pf.IsDisposed) pf.Close();
                        }
                        catch (OperationCanceledException)
                        {
                            if (!pf.IsDisposed) pf.Close();
                            MessageBox.Show("Extraction canceled.", title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            try { File.Delete(tmpZip); } catch { }
                            return;
                        }
                        catch (Exception ex)
                        {
                            if (!pf.IsDisposed) pf.Close();
                            MessageBox.Show(ex.Message, "SFX Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            try { File.Delete(tmpZip); } catch { }
                            return;
                        }
                    }
                    else
                    {
                        var progress = new Progress<ZipProgress>(_ => { });
                        var cts = new System.Threading.CancellationTokenSource();
                        zip.ExtractArchiveAsync(extractDir, progress, cts.Token).GetAwaiter().GetResult();
                    }
                }

                try { File.Delete(tmpZip); } catch { }

                // Run-after
                if (!string.IsNullOrWhiteSpace(cfg.RunAfter))
                {
                    string runPath = Path.Combine(extractDir, cfg.RunAfter);
                    if (File.Exists(runPath))
                    {
                        var psi = new ProcessStartInfo(runPath)
                        {
                            WorkingDirectory = extractDir,
                            UseShellExecute = true
                        };
                        try { Process.Start(psi); } catch { }
                    }
                }

                if (cfg.ShowCompletedDialog && !cfg.Silent)
                {
                    MessageBox.Show($"Extracted to:\n{extractDir}", title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SFX Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Robust exe path resolver for single-file and regular apps
        private static string GetExecutablePath()
        {
            // .NET 6+
            try
            {
                var p = Environment.ProcessPath;
                if (!string.IsNullOrWhiteSpace(p) && File.Exists(p)) return p;
            }
            catch { }

            // Fallback: Win32
            try
            {
                var sb = new StringBuilder(32767);
                uint len = GetModuleFileNameW(IntPtr.Zero, sb, sb.Capacity);
                if (len > 0)
                {
                    var p = sb.ToString();
                    if (File.Exists(p)) return p;
                }
            }
            catch { }

            // Fallback: MainModule
            try
            {
                string p = Process.GetCurrentProcess().MainModule?.FileName;
                if (!string.IsNullOrWhiteSpace(p) && File.Exists(p)) return p;
            }
            catch { }

            // Fallback: Windows Forms helper
            try
            {
                string p = Application.ExecutablePath;
                if (!string.IsNullOrWhiteSpace(p) && File.Exists(p)) return p;
            }
            catch { }

            // Fallback: BaseDirectory + friendly name
            try
            {
                string baseDir = AppContext.BaseDirectory?.TrimEnd('\\', '/');
                string name = AppDomain.CurrentDomain.FriendlyName;
                if (!string.IsNullOrWhiteSpace(baseDir) && !string.IsNullOrWhiteSpace(name))
                {
                    string p = Path.Combine(baseDir, name);
                    if (File.Exists(p)) return p;
                }
            }
            catch { }

            // Last resort: entry assembly (usually empty location for single-file)
            try
            {
                string p = Assembly.GetEntryAssembly()?.Location;
                if (!string.IsNullOrWhiteSpace(p) && File.Exists(p)) return p;
            }
            catch { }

            return null;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint GetModuleFileNameW(IntPtr hModule, StringBuilder lpFilename, int nSize);

        private static bool TryReadFooterAt(FileStream fs, long pos, out uint configLen, out ulong zipLen)
        {
            configLen = 0;
            zipLen = 0;
            if (pos < 0 || pos + FooterSize > fs.Length) return false;

            fs.Seek(pos, SeekOrigin.Begin);
            byte[] footer = new byte[FooterSize];
            ReadExact(fs, footer, 0, footer.Length);

            string magic = Encoding.ASCII.GetString(footer, 0, 8);
            if (!string.Equals(magic, Magic, StringComparison.Ordinal))
                return false;

            configLen = BitConverter.ToUInt32(footer, 8);
            zipLen = BitConverter.ToUInt64(footer, 12);
            return true;
        }

        private static string ExpandMacros(string pattern, string exePath, string title)
        {
            string exedir = Path.GetDirectoryName(exePath) ?? ".";
            string name = Path.GetFileNameWithoutExtension(exePath) ?? "SFX";
            string temp = Path.GetTempPath().TrimEnd('\\', '/');
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string p = (pattern ?? "%TEMP%\\SFX_%NAME%")
                .Replace("%TEMP%", temp, StringComparison.OrdinalIgnoreCase)
                .Replace("%DESKTOP%", desktop, StringComparison.OrdinalIgnoreCase)
                .Replace("%EXEDIR%", exedir, StringComparison.OrdinalIgnoreCase)
                .Replace("%APPDATA%", appdata, StringComparison.OrdinalIgnoreCase)
                .Replace("%NAME%", name, StringComparison.OrdinalIgnoreCase)
                .Replace("%TITLE%", title, StringComparison.OrdinalIgnoreCase);
            return p;
        }

        private static void ReadExact(Stream s, byte[] b, int off, int len)
        {
            int rTot = 0;
            while (rTot < len)
            {
                int r = s.Read(b, off + rTot, len - rTot);
                if (r <= 0) throw new EndOfStreamException("Unexpected EOF");
                rTot += r;
            }
        }

        private static void CopyLimit(Stream src, Stream dst, long count)
        {
            byte[] buf = new byte[1 << 20];
            long rem = count;
            while (rem > 0)
            {
                int toRead = (int)Math.Min(buf.Length, rem);
                int r = src.Read(buf, 0, toRead);
                if (r <= 0) throw new EndOfStreamException("Unexpected EOF while copying appended data");
                dst.Write(buf, 0, r);
                rem -= r;
            }
        }

        private static bool IsElevated()
        {
            try
            {
                using var id = WindowsIdentity.GetCurrent();
                var p = new WindowsPrincipal(id);
                return p.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch { return false; }
        }

        private static void RelaunchElevated(string exePath)
        {
            try
            {
                var psi = new ProcessStartInfo(exePath)
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(psi);
            }
            catch { }
        }

        private static Icon TryLoadIcon(string base64)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(base64)) return null;
                var bytes = Convert.FromBase64String(base64);
                using var ms = new MemoryStream(bytes);
                return new Icon(ms);
            }
            catch { return null; }
        }

        private static Image TryLoadImage(string base64)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(base64)) return null;
                var bytes = Convert.FromBase64String(base64);
                using var ms = new MemoryStream(bytes);
                return Image.FromStream(ms);
            }
            catch { return null; }
        }

        private static Color? TryParseColor(string hex)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hex)) return null;
                string s = hex.Trim();
                if (s.StartsWith("#")) s = s.Substring(1);
                if (s.Length == 6)
                {
                    byte r = Convert.ToByte(s.Substring(0, 2), 16);
                    byte g = Convert.ToByte(s.Substring(2, 2), 16);
                    byte b = Convert.ToByte(s.Substring(4, 2), 16);
                    return Color.FromArgb(r, g, b);
                }
            }
            catch { }
            return null;
        }

        private static void ShowNoPayloadMessage()
        {
            try
            {
                MessageBox.Show("No SFX payload is embedded in this stub.", "SFX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch { }
        }
    }
}