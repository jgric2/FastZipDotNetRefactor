using FastZipDotNet.Zip.Helpers;
using System.Security.Cryptography;
using System.Text;
using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

namespace Brutal_Zip.Views
{
    public partial class InfoPane : UserControl
    {
        private string _archivePath;
        private ZipFileEntry? _entry;
        private string _tempPath;

              
            void Key(Label l, string t) { l.Text = t; l.AutoSize = true; l.ForeColor = Color.DimGray; }
        public void Val(Label l) { l.Text = "-"; l.AutoSize = true; l.MaximumSize = new Size(600, 0); }

        public InfoPane()
        {
            InitializeComponent();

            btnCopyInsidePath.Click += (_, __) =>
            {
                if (!string.IsNullOrEmpty(lblPathVal.Text))
                    try { Clipboard.SetText(lblPathVal.Text); } catch { }
            };

            btnComputeCrc.Click += async (_, __) => await ComputeHashAsync("CRC32");
            btnComputeMd5.Click += async (_, __) => await ComputeHashAsync("MD5");
            btnComputeSha256.Click += async (_, __) => await ComputeHashAsync("SHA256");

            for (int i = 0; i < table.RowCount; i++) table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        public void Clear()
        {
            _entry = null;
            _tempPath = null;
            SetVal(lblNameVal, "-");
            SetVal(lblPathVal, "-");
            SetVal(lblTypeVal, "-");
            SetVal(lblSizeVal, "-");
            SetVal(lblPackedVal, "-");
            SetVal(lblRatioVal, "-");
            SetVal(lblMethodVal, "-");
            SetVal(lblEncryptVal, "-");
            SetVal(lblCrcVal, "-");
            SetVal(lblModifiedVal, "-");
            SetVal(lblAttrsVal, "-");

            SetVal(lblDimVal, "-");
            SetVal(lblTextVal, "-");
            lblCrcFile.Text = "CRC32: -";
            lblMd5File.Text = "MD5: -";
            lblSha256File.Text = "SHA-256: -";
        }

        public void ShowEntry(string archivePath, ZipFileEntry entry)
        {
            _archivePath = archivePath;
            _entry = entry;
            _tempPath = null;

            string name = Path.GetFileName(entry.FilenameInZip.Replace('\\', '/'));
            string type = entry.FilenameInZip.EndsWith("/") || entry.FilenameInZip.EndsWith("\\") ? "Folder" : "File";
            long unc = (long)entry.FileSize;
            long comp = (long)entry.CompressedSize;
            double ratio = unc > 0 ? 1.0 - ((double)comp / unc) : 0.0;

            SetVal(lblNameVal, name);
            SetVal(lblPathVal, entry.FilenameInZip);
            SetVal(lblTypeVal, type);
            SetVal(lblSizeVal, FormatBytes(unc));
            SetVal(lblPackedVal, FormatBytes(comp));
            SetVal(lblRatioVal, $"{ratio:P0}");
            SetVal(lblMethodVal, entry.Method.ToString());
            SetVal(lblEncryptVal, entry.IsEncrypted
                ? (entry.IsAes ? $"AES-{(entry.AesStrength == 1 ? 128 : entry.AesStrength == 2 ? 192 : 256)}"
                               : "ZipCrypto")
                : "None");
            SetVal(lblCrcVal, $"0x{entry.Crc32:X8}");
            SetVal(lblModifiedVal, entry.ModifyTime.ToString("yyyy-MM-dd HH:mm:ss"));
            SetVal(lblAttrsVal, $"0x{entry.ExternalFileAttr:X8}");

            SetVal(lblDimVal, "-");
            SetVal(lblTextVal, "-");
            lblCrcFile.Text = "CRC32: -";
            lblMd5File.Text = "MD5: -";
            lblSha256File.Text = "SHA-256: -";
        }

        public void SetTempFile(string path)
        {
            _tempPath = path;
            _ = Task.Run(UpdateContentInfoAsync);
        }

        private async Task UpdateContentInfoAsync()
        {
            try
            {
                string p = _tempPath;
                if (string.IsNullOrEmpty(p) || !File.Exists(p)) return;

                string dim = "-";
                string textInfo = "-";

                // Try image dimensions
                try
                {
                    using var fs = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.SequentialScan);
                    using var img = Image.FromStream(fs, useEmbeddedColorManagement: false, validateImageData: false);
                    dim = $"{img.Width} × {img.Height}";
                }
                catch
                {
                    // not an image
                }

                // Try text (size-limited)
                try
                {
                    var fi = new FileInfo(p);
                    if (fi.Length <= (32L << 20)) // 32 MB guard
                    {
                        int lines = 0;
                        using var sr = new StreamReader(p, Encoding.UTF8, true);
                        while ((await sr.ReadLineAsync()) != null) lines++;
                        textInfo = $"{lines:N0} lines";
                    }
                }
                catch
                {
                    // not a text file
                }

                BeginInvoke(new Action(() =>
                {
                    SetVal(lblDimVal, dim);
                    SetVal(lblTextVal, textInfo);
                }));
            }
            catch
            {
                // ignore
            }
        }

        private async Task ComputeHashAsync(string kind)
        {
            try
            {
                string p = _tempPath;
                if (string.IsNullOrEmpty(p) || !File.Exists(p))
                {
                    MessageBox.Show(this, "Preview the file first.", "Hash");
                    return;
                }

                if (kind == "CRC32")
                {
                    uint crc = await Task.Run(() =>
                    {
                        using var fs = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.SequentialScan);
                        return Crc32Helpers.ComputeCrc32(fs);
                    });
                    lblCrcFile.Text = $"CRC32: 0x{crc:X8}";
                }
                else if (kind == "MD5")
                {
                    var md5 = await Task.Run(() =>
                    {
                        using var fs = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.SequentialScan);
                        using var h = MD5.Create();
                        var bytes = h.ComputeHash(fs);
                        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
                    });
                    lblMd5File.Text = $"MD5: {md5}";
                }
                else if (kind == "SHA256")
                {
                    var sha = await Task.Run(() =>
                    {
                        using var fs = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.SequentialScan);
                        using var h = SHA256.Create();
                        var bytes = h.ComputeHash(fs);
                        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
                    });
                    lblSha256File.Text = $"SHA-256: {sha}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Hash");
            }
        }

        private static void SetVal(Label l, string s) => l.Text = s ?? "-";

        private static string FormatBytes(long bytes)
        {
            string[] u = { "B", "KB", "MB", "GB", "TB" };
            double s = bytes; int i = 0;
            while (s >= 1024 && i < u.Length - 1) { s /= 1024; i++; }
            return $"{s:F1} {u[i]}";
        }
    }
}