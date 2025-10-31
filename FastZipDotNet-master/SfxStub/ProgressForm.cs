using FastZipDotNet.Zip;

namespace SfxStub
{
    public partial class ProgressForm : Form
    {
        public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

        private readonly Dictionary<string, ListViewItem> _items = new(StringComparer.OrdinalIgnoreCase);
        private string _lastFile;

        // Expose the last received progress snapshot for any external consumer
        public ZipProgress LastProgress { get; private set; }

        public ProgressForm(string title, Image banner = null, Icon icon = null, Color? theme = null, bool showFileList = true)
        {
            InitializeComponent();

            if (banner != null) picBanner.Image = banner;
            if (icon != null) this.Icon = icon;
            if (theme.HasValue) pnlTopAccent.BackColor = theme.Value;
            if (!string.IsNullOrWhiteSpace(title)) lblTitle.Text = title;

            lvFiles.Visible = showFileList;
            if (!showFileList)
            {
                // Simplify layout if file list is hidden
                btnCancel.Top = progressOverall.Bottom + 20;
                Height = 240;
            }

            btnCancel.Click += (_, __) => TokenSource.Cancel();
        }

        public IProgress<ZipProgress> CreateProgress()
        {
            return new Progress<ZipProgress>(Report);
        }

        private void Report(ZipProgress p)
        {
            LastProgress = p; // Cache for any consumer

            if (IsDisposed) return;

            if (InvokeRequired)
            {
                try { BeginInvoke(new Action<ZipProgress>(Report), p); } catch { }
                return;
            }

            try
            {
                // Percent
                int percent = (int)Math.Round(p.Percent);
                if (percent < 0) percent = 0;
                if (percent > 100) percent = 100;
                progressOverall.Value = percent;

                // Current file (and maintain file list)
                string cur = p.CurrentFile ?? "";
                if (!string.IsNullOrEmpty(cur))
                {
                    lblFile.Text = cur;
                    AddOrUpdateFile(cur, "Extracting…", null);
                }

                if (!string.IsNullOrEmpty(_lastFile) &&
                    !string.Equals(_lastFile, cur, StringComparison.OrdinalIgnoreCase))
                {
                    AddOrUpdateFile(_lastFile, "Done", null);
                }
                _lastFile = cur;

                // Full stats text – match main form fields (operation, percent, bytes, speed, files, files/s, ETA, elapsed)
                string op = p.Operation.ToString();
                string bytes = $"{FormatBytes(p.BytesProcessedUncompressed)} / {FormatBytes(p.TotalBytesUncompressed)}";
                string speedBps = p.SpeedBytesPerSec > 0 ? $"{FormatBytes((long)p.SpeedBytesPerSec)}/s" : "0 B/s";
                string fps = p.FilesPerSec > 0 ? $"{p.FilesPerSec:F2} file/s" : "0.00 file/s";
                string files = $"{p.FilesProcessed}/{p.TotalFiles}";
                string eta = p.ETA.HasValue ? $"{p.ETA.Value:hh\\:mm\\:ss}" : "--:--:--";
                string elapsed = $"{p.Elapsed:hh\\:mm\\:ss}";

                // Example: "Extract: 42%   123.4 MB / 512.0 MB   45.6 MB/s   8/120 files   1.23 file/s   ETA: 00:02:34   Time: 00:00:47"
                lblStat.Text = $"{op}: {percent}%   {bytes}   {speedBps}   {files} files   {fps}   ETA: {eta}   Time: {elapsed}";

                // Completion – mark last file done
                if (percent >= 100 && !string.IsNullOrEmpty(_lastFile))
                {
                    AddOrUpdateFile(_lastFile, "Done", null);
                    _lastFile = null;
                }
            }
            catch
            {
                // Ignore UI exceptions to keep the progress responsive
            }
        }

        private void AddOrUpdateFile(string name, string status, long? sizeBytes)
        {
            if (string.IsNullOrEmpty(name)) return;

            if (!_items.TryGetValue(name, out var it))
            {
                it = new ListViewItem(new[] { name, status, sizeBytes.HasValue ? FormatBytes(sizeBytes.Value) : "" });
                _items[name] = it;
                lvFiles.Items.Add(it);
                lvFiles.EnsureVisible(lvFiles.Items.Count - 1);
            }
            else
            {
                it.SubItems[1].Text = status;
                if (sizeBytes.HasValue) it.SubItems[2].Text = FormatBytes(sizeBytes.Value);
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