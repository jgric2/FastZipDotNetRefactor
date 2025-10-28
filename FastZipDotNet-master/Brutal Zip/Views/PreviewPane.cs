using System.Diagnostics;

namespace Brutal_Zip.Views
{
    public partial class PreviewPane : UserControl
    {
        public PreviewPane()
        {
            InitializeComponent();

            btnOpenExternal.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_currentFilePath) && File.Exists(_currentFilePath))
                {
                    try { Process.Start(new ProcessStartInfo(_currentFilePath) { UseShellExecute = true }); }
                    catch { }
                }
            };

            btnPlayPause.Click += (s, e) => TogglePlay();
            btnStop.Click += (s, e) => Stop();
        }

        private string _currentFilePath;
        private long _currentSize;

        public void Clear()
        {
            _currentFilePath = null;
            _currentSize = 0;
            lblFileName.Text = "-";
            lblInfo.Text = "";
            ShowUnsupported();
        }

        public async Task ShowFileAsync(string path)
        {
            Clear();
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            _currentFilePath = path;
            _currentSize = new FileInfo(path).Length;
            lblFileName.Text = Path.GetFileName(path);
            lblInfo.Text = $"{FormatBytes(_currentSize)}    {path}";

            string ext = Path.GetExtension(path).ToLowerInvariant();
            if (IsImage(ext))
            {
                await ShowImageAsync(path);
                return;
            }

            if (IsText(ext) && _currentSize <= 5 * 1024 * 1024)
            {
                await ShowTextAsync(path);
                return;
            }

            ShowUnsupported();
        }

        private async Task ShowImageAsync(string path)
        {
            try
            {
                // Load on background thread and clone to avoid locking file
                var bmp = await Task.Run(() =>
                {
                    using var fs = File.OpenRead(path);
                    using var img = Image.FromStream(fs, useEmbeddedColorManagement: false, validateImageData: false);
                    return new Bitmap(img);
                });

                if (IsDisposed) { bmp.Dispose(); return; }

                if (picture.Image != null) { var old = picture.Image; picture.Image = null; old.Dispose(); }
                picture.Image = bmp;

                picture.Visible = true;
                txtPreview.Visible = false;
                lblUnsupported.Visible = false;
            }
            catch
            {
                ShowUnsupported();
            }
        }

        private async Task ShowTextAsync(string path)
        {
            try
            {
                string text = await Task.Run(() =>
                {
                    using var sr = new StreamReader(path);
                    return sr.ReadToEnd();
                });

                txtPreview.Text = text;
                txtPreview.Visible = true;
                picture.Visible = false;
                lblUnsupported.Visible = false;
            }
            catch
            {
                ShowUnsupported();
            }
        }

        private void ShowUnsupported()
        {
            picture.Visible = false;
            txtPreview.Visible = false;
            lblUnsupported.Visible = true;
        }

        private void TogglePlay()
        {
            // Placeholder: media not implemented in this version.
            // If you add WMP later, hook it here. For now, just try opening externally.
            if (!string.IsNullOrEmpty(_currentFilePath))
            {
                try { Process.Start(new ProcessStartInfo(_currentFilePath) { UseShellExecute = true }); }
                catch { }
            }
        }

        private void Stop()
        {
            // Placeholder for future media stop.
        }

        private static bool IsImage(string ext)
        {
            switch (ext)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".gif":
                case ".tif":
                case ".tiff":
                case ".webp":
                case ".ico":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsText(string ext)
        {
            switch (ext)
            {
                case ".txt":
                case ".log":
                case ".md":
                case ".csv":
                case ".json":
                case ".xml":
                case ".ini":
                case ".cfg":
                case ".yml":
                case ".yaml":
                    return true;
                default:
                    return false;
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] u = { "B", "KB", "MB", "GB", "TB" };
            double s = bytes; int i = 0;
            while (s >= 1024 && i < u.Length - 1) { s /= 1024; i++; }
            return $"{s:F1} {u[i]}";
        }
    }
}
