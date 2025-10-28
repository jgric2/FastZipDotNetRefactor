using FastZipDotNet.Zip;

namespace BrutalZip
{
    public partial class ProgressForm : Form
    {
        private readonly System.Threading.CancellationTokenSource _cts;

        public System.Threading.CancellationToken Token => _cts.Token;

        public ProgressForm(string title)
        {
            InitializeComponent();
            _cts = new System.Threading.CancellationTokenSource();

            lblTitle.Text = title;
        }

        private void ToggleDetails()
        {
            if (panelDetails.Visible)
            {
                // collapse
                panelDetails.Visible = false;
                panelDetails.Height = 0;
                btnDetails.Text = "Details ▸";
                ClientSize = new System.Drawing.Size(640, 190);
            }
            else
            {
                // expand
                panelDetails.Visible = true;
                panelDetails.Height = 180;
                btnDetails.Text = "Details ▾";
                ClientSize = new System.Drawing.Size(640, 340);
            }
        }

        public void AppendLog(string line)
        {
            if (!panelDetails.Visible) return;
            if (InvokeRequired) { BeginInvoke(new Action<string>(AppendLog), line); return; }
            txtLog.AppendText(line + Environment.NewLine);
        }

        public IProgress<ZipProgress> CreateProgress()
        {
            DateTime last = DateTime.MinValue;
            return new Progress<ZipProgress>(p =>
            {
                var now = DateTime.UtcNow;
                if ((now - last).TotalMilliseconds < 80) return;
                last = now;

                int overall = (int)Math.Round(p.Percent);
                overall = Math.Max(0, Math.Min(100, overall));
                progressOverall.Value = overall;

                lblMetrics.Text =
                    $"{p.Operation}: {p.Percent:F1}%   " +
                    $"{FormatBytes(p.BytesProcessedUncompressed)} / {FormatBytes(p.TotalBytesUncompressed)}   " +
                    $"{FormatBytes((long)p.SpeedBytesPerSec)}/s   " +
                    $"Files: {p.FilesProcessed}/{p.TotalFiles}" +
                    (string.IsNullOrEmpty(p.CurrentFile) ? "" : $"\n{p.CurrentFile}");
            });
        }

        private static string FormatBytes(long bytes)
        {
            string[] u = { "B", "KB", "MB", "GB", "TB" };
            double s = bytes; int i = 0;
            while (s >= 1024 && i < u.Length - 1) { s /= 1024; i++; }
            return $"{s:F1} {u[i]}";
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            ToggleDetails();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts.Cancel();
        }
    }
}