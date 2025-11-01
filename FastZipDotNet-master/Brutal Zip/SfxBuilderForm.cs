﻿using Brutal_Zip.Classes;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace Brutal_Zip
{
    public partial class SfxBuilderForm : Form
    {
        private const string Magic = "BXZSFX10";
        private const int FooterSize = 8 + 4 + 8; // magic(8) + cfgLen(4) + zipLen(8)
        private const int DotnetBundleFooterSize = 8;

        private readonly MainForm _main;

        public SfxBuilderForm(MainForm main)
        {
            _main = main;
            InitializeComponent();

            rdoUseCurrent.CheckedChanged += (_, __) => UpdateSourceMode();
            rdoUseFile.CheckedChanged += (_, __) => UpdateSourceMode();
            btnBrowseZip.Click += (_, __) => BrowseZip();
            btnBrowseStub.Click += (_, __) => BrowseStub();
            btnBrowseIcon.Click += (_, __) => BrowseIcon();
            btnBrowseBanner.Click += (_, __) => BrowseBanner();
            btnPickColor.Click += (_, __) => PickThemeColor();
            btnLoadLicense.Click += (_, __) => LoadLicense();
            btnPreview.Click += (_, __) => ShowPreview(); // NEW
            btnBuild.Click += (_, __) => BuildSfx();
            btnClose.Click += (_, __) => Close();

            txtTitle.Text = "Self Extracting Archive";
            txtDefaultDir.Text = "%TEMP%\\SFX_%NAME%";
            chkShowFileList.Checked = true;
            chkOverwrite.Checked = true;
            chkShowDone.Checked = true;
            pnlThemeColor.BackColor = Color.DodgerBlue;

            // Prefill password convenience
            try
            {
                var fi = _main.GetType().GetField("_archivePassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (fi != null)
                {
                    var pw = fi.GetValue(_main) as string;
                    if (!string.IsNullOrEmpty(pw)) txtPassword.Text = pw;
                }
            }
            catch { }

            UpdateSourceMode();
        }

        private void UpdateSourceMode()
        {
            bool useFile = rdoUseFile.Checked;
            txtZipPath.Enabled = useFile;
            btnBrowseZip.Enabled = useFile;
        }

        private void BrowseZip()
        {
            using var ofd = new OpenFileDialog { Filter = "Zip files|*.zip|All files|*.*" };
            if (ofd.ShowDialog(this) == DialogResult.OK)
                txtZipPath.Text = ofd.FileName;
        }

        private void BrowseStub()
        {
            using var ofd = new OpenFileDialog { Filter = "SFX stub (.exe)|*.exe|All files|*.*" };
            if (ofd.ShowDialog(this) == DialogResult.OK)
                txtStubPath.Text = ofd.FileName;
        }

        private void BrowseIcon()
        {
            using var ofd = new OpenFileDialog { Filter = "Icon files|*.ico|All files|*.*" };
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                txtIconPath.Text = ofd.FileName;
                try { using var s = File.OpenRead(ofd.FileName); picIcon.Image = new Icon(s).ToBitmap(); }
                catch { picIcon.Image = null; }
            }
        }

        private void BrowseBanner()
        {
            using var ofd = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp|All files|*.*" };
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                txtBannerPath.Text = ofd.FileName;
                try { picBanner.Image = Image.FromFile(ofd.FileName); }
                catch { picBanner.Image = null; }
            }
        }

        private void PickThemeColor()
        {
            using var cd = new ColorDialog { Color = pnlThemeColor.BackColor, FullOpen = true };
            if (cd.ShowDialog(this) == DialogResult.OK)
                pnlThemeColor.BackColor = cd.Color;
        }

        private void LoadLicense()
        {
            using var ofd = new OpenFileDialog { Filter = "Text files|*.txt|All files|*.*" };
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                try { txtLicense.Text = File.ReadAllText(ofd.FileName, Encoding.UTF8); }
                catch (Exception ex) { MessageBox.Show(this, ex.Message, "Load License"); }
            }
        }

        private string ResolveSourceZip()
        {
            if (rdoUseCurrent.Checked)
            {
                var field = _main.GetType().GetField("_zipPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                string p = field?.GetValue(_main) as string;
                if (string.IsNullOrEmpty(p) || !File.Exists(p))
                    throw new InvalidOperationException("No archive is currently open.");
                return p;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtZipPath.Text) || !File.Exists(txtZipPath.Text))
                    throw new InvalidOperationException("Select a valid source ZIP.");
                return txtZipPath.Text;
            }
        }

     
        private void ShowPreview()
        {
            try
            {
                string title = txtTitle.Text?.Trim();
                string company = txtCompany.Text?.Trim();
                Icon icon = TryLoadIconFromPath(txtIconPath.Text);
                Image banner = TryLoadImageFromPath(txtBannerPath.Text);
                Color theme = pnlThemeColor.BackColor;
                bool showList = chkShowFileList.Checked;

                using var prev = new BuilderPreviewForm(title, company, banner, icon, theme, showList);
                prev.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Preview");
            }
        }

        private void BuildSfx()
        {
            try
            {
                string stubPath = txtStubPath.Text;
                if (string.IsNullOrWhiteSpace(stubPath) || !File.Exists(stubPath))
                {
                    MessageBox.Show(this, "Select the SFX stub executable.", "Build SFX");
                    return;
                }

                string sourceZip = rdoUseCurrent.Checked
                    ? ResolveCurrentZipOrThrow()
                    : ResolveZipFromFileOrThrow();

                // Build config used by stub at runtime for branding/options
                var cfg = new SfxBuildConfig
                {
                    Title = txtTitle.Text?.Trim(),
                    CompanyName = txtCompany.Text?.Trim(),
                    DefaultExtractDir = txtDefaultDir.Text?.Trim(),
                    Silent = chkSilent.Checked,
                    Overwrite = chkOverwrite.Checked,
                    RequireElevation = chkRequireElevation.Checked,
                    ShowCompletedDialog = chkShowDone.Checked,
                    RunAfter = string.IsNullOrWhiteSpace(txtRunAfter.Text) ? null : txtRunAfter.Text.Trim(),
                    Password = string.IsNullOrWhiteSpace(txtPassword.Text) ? null : txtPassword.Text,
                    LicenseText = string.IsNullOrWhiteSpace(txtLicense.Text) ? null : txtLicense.Text,
                    RequireLicenseAccept = chkRequireAccept.Checked,
                    ShowFileList = chkShowFileList.Checked,
                    IconBase64 = ToBase64File(txtIconPath.Text),
                    BannerImageBase64 = ToBase64File(txtBannerPath.Text),
                    ThemeColor = ToHexColor(pnlThemeColor.BackColor)
                };

                using var sfd = new SaveFileDialog { Filter = "Self-extracting exe|*.exe", FileName = "Setup.exe" };
                if (sfd.ShowDialog(this) != DialogResult.OK) return;
                string outExe = sfd.FileName;

                // 1) Ensure fresh target
                try { if (File.Exists(outExe)) File.Delete(outExe); } catch { }

                // 2) Copy stub first (do not UpdateResource icon on single-file stubs)
                File.Copy(stubPath, outExe, overwrite: false);

                // 3) Preserve the .NET single-file bundle trailer 8 bytes
                byte[] bundleTail8 = TryReadLast8Bytes(outExe);
                if (bundleTail8 == null || bundleTail8.Length != 8)
                {
                    MessageBox.Show(this,
                        "Warning: Could not read the .NET single-file footer. Proceeding without preserving it may break the runtime.",
                        "Build SFX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // 4) Append config + zip + SFX footer, then re-append the .NET trailer 8 bytes
                byte[] cfgBytes = JsonSerializer.SerializeToUtf8Bytes(cfg, new JsonSerializerOptions { WriteIndented = false });
                uint cfgLen = (uint)cfgBytes.Length;
                ulong zipLen = (ulong)new FileInfo(sourceZip).Length;

                using (var fout = OpenForAppendWithRetry(outExe))
                using (var bw = new BinaryWriter(fout, Encoding.UTF8, leaveOpen: true))
                {
                    // write config
                    bw.Write(cfgBytes);

                    // write zip (open with permissive share to avoid sharing violation)
                    // IMPORTANT: Share must allow READ and WRITE, because the viewer's existing handle desires WRITE.

                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            using var fsZip = new FileStream(sourceZip, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                            fsZip.CopyTo(fout);
                            break;
                        }
                        catch (IOException) { System.Threading.Thread.Sleep(100); if (i == 4) throw; }
                    }

                    ////using (var fsZip = new FileStream(
                    ////    sourceZip,
                    ////    FileMode.Open,
                    ////    FileAccess.Read,
                    ////    FileShare.ReadWrite | FileShare.Delete))
                    ////{
                    ////    fsZip.CopyTo(fout);
                    ////}

                    // write our custom footer (magic + lengths)
                    WriteFooter(bw, cfgLen, zipLen);

                    // restore the .NET single-file tail 8 bytes at EOF
                    if (bundleTail8 != null && bundleTail8.Length == 8)
                        bw.Write(bundleTail8);

                    bw.Flush();
                    fout.Flush(true);
                }

                MessageBox.Show(this, "SFX built successfully.", "Build SFX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Build SFX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ResolveCurrentZipOrThrow()
        {
            var field = _main.GetType().GetField("_zipPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string p = field?.GetValue(_main) as string;
            if (string.IsNullOrEmpty(p) || !File.Exists(p))
                throw new InvalidOperationException("No archive is currently open.");
            return p;
        }

        private string ResolveZipFromFileOrThrow()
        {
            if (string.IsNullOrWhiteSpace(txtZipPath.Text) || !File.Exists(txtZipPath.Text))
                throw new InvalidOperationException("Select a valid source ZIP.");
            return txtZipPath.Text;
        }


        private static FileStream OpenForAppendWithRetry(string path, int attempts = 8, int delayMs = 100)
        {
            Exception last = null;
            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    // Keep last-writer wins; ensure we have exclusive write here
                    return new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None);
                }
                catch (IOException ex) { last = ex; System.Threading.Thread.Sleep(delayMs); }
                catch (System.ComponentModel.Win32Exception ex2) { last = ex2; System.Threading.Thread.Sleep(delayMs); }
            }
            throw last ?? new IOException("Failed to open target EXE for appending.");
        }

        private static byte[] TryReadLast8Bytes(string path)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                if (fs.Length < 8) return null;
                fs.Seek(-8, SeekOrigin.End);
                byte[] tail = new byte[8];
                int r = fs.Read(tail, 0, 8);
                return (r == 8) ? tail : null;
            }
            catch { return null; }
        }







        private static void WriteFooter(BinaryWriter bw, uint cfgLen, ulong zipLen)
        {
            bw.Write(Encoding.ASCII.GetBytes(Magic)); // 8
            bw.Write(cfgLen);                          // 4
            bw.Write(zipLen);                          // 8
        }

        private static string ToBase64File(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;
                var bytes = File.ReadAllBytes(path);
                return Convert.ToBase64String(bytes);
            }
            catch { return null; }
        }


        private static string ToHexColor(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        private Icon TryLoadIconFromPath(string p)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(p) || !File.Exists(p)) return null;
                using var s = File.OpenRead(p);
                return new Icon(s);
            }
            catch { return null; }
        }

        private Image TryLoadImageFromPath(string p)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(p) || !File.Exists(p)) return null;
                return Image.FromFile(p);
            }
            catch { return null; }
        }













        private static bool IsSelfContainedStub(string stubPath)
        {
            try
            {
                var dir = Path.GetDirectoryName(stubPath) ?? ".";
                var name = Path.GetFileNameWithoutExtension(stubPath);
                var rc = Path.Combine(dir, name + ".runtimeconfig.json");
                // If there is a runtimeconfig next to it, it's framework-dependent
                return !File.Exists(rc);
            }
            catch { return false; }
        }

        // Show a quick reminder for stub .csproj publish settings
        private void ShowStubCsprojGuidance()
        {
            string guidance =
            @"Publish the SFX stub as a self-contained single-file:

<PropertyGroup> <TargetFramework>net8.0-windows</TargetFramework> <UseWindowsForms>true</UseWindowsForms> <RuntimeIdentifier>win-x64</RuntimeIdentifier> <!-- or win-x86 / win-arm64 --> <SelfContained>true</SelfContained> <PublishSingleFile>true</PublishSingleFile> <PublishTrimmed>false</PublishTrimmed> <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract> <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile> </PropertyGroup>
Then select the published EXE as the stub in the Builder.";
            MessageBox.Show(this, guidance, "Stub Publish Guidance", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // Safely embed the icon: close all file handles beforehand, clear RO attribute,
        // and retry a few times in case AV/indexer still holds a short lock.
        private static void TryEmbedIconWithRetry(string exePath, string icoPath, int maxAttempts = 8, int delayMs = 100)
        {
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath)) return;
            if (string.IsNullOrWhiteSpace(icoPath) || !File.Exists(icoPath)) return;

            // Ensure it's writable (clear read-only)
            try
            {
                var attr = File.GetAttributes(exePath);
                if ((attr & FileAttributes.ReadOnly) != 0)
                    File.SetAttributes(exePath, attr & ~FileAttributes.ReadOnly);
            }
            catch { }

            Exception last = null;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    // If any stale handle is open from us, GC/Wait one cycle (usually unnecessary)
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();

                    ResourceUpdater.ReplaceIcon(exePath, icoPath);
                    return; // success
                }
                catch (System.ComponentModel.Win32Exception wex)
                {
                    last = wex;
                    // ERROR_SHARING_VIOLATION (32) or ERROR_LOCK_VIOLATION (33)
                    int err = wex.NativeErrorCode;
                    if (err == 32 || err == 33)
                    {
                        System.Threading.Thread.Sleep(delayMs);
                        continue;
                    }
                    // Other Win32 error: rethrow
                    throw;
                }
                catch (Exception ex)
                {
                    last = ex;
                    System.Threading.Thread.Sleep(delayMs);
                }
            }

            if (last != null)
                throw last;
        }

    }
}