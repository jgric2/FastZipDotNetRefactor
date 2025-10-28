using FastZipDotNet.Zip;

namespace BrutalZip
{
    public partial class ProgressForm : Form
    {
        private readonly System.Threading.CancellationTokenSource _cts;
        private Action<int> _onThreadsChanged;

        public System.Threading.CancellationToken Token => _cts.Token;

        public ProgressForm(string title)
        {
            InitializeComponent();

            tbThreads.ValueChanged += (s, e) =>
            {
                lblThreadsCur.Text = tbThreads.Value.ToString();
                _onThreadsChanged?.Invoke(tbThreads.Value);
            };

            _cts = new System.Threading.CancellationTokenSource();

            lblTitle.Text = title;
        }


        public void ConfigureThreads(int max, int current, Action<int> onChange)
        {
            if (max < 1) max = 1;
            _onThreadsChanged = onChange;

            lblThreads.Visible = true;
            tbThreads.Visible = true;
            lblThreadsCur.Visible = true;

            tbThreads.Minimum = 1;
            tbThreads.Maximum = max;
            tbThreads.Value = Math.Max(1, Math.Min(current, max));
            lblThreadsCur.Text = tbThreads.Value.ToString();
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

        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }
    }
}