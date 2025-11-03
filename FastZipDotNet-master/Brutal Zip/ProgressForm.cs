using Brutal_Zip.Classes.Helpers;
using BrutalZip2025.BrutalControls;
using FastZipDotNet.Zip;

namespace BrutalZip
{
    public partial class ProgressForm : ModernForm
    {
        private readonly System.Threading.CancellationTokenSource _cts;
        private Action<int> _onThreadsChanged;

        public System.Threading.CancellationToken Token => _cts.Token;

        public ProgressForm(string title)
        {
            InitializeComponent();

            //tbThreads.ValueChanged += (s, e) =>
            //{
            //    lblThreadsCur.Text = tbThreads.Value.ToString();
            //    _onThreadsChanged?.Invoke(tbThreads.Value);
            //};

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

       

        public IProgress<ZipProgress> CreateProgress()
        {
            DateTime last = DateTime.MinValue;
            return new Progress<ZipProgress>(p =>
            {
                var now = DateTime.UtcNow;
                if ((now - last).TotalMilliseconds < 10) return;
                last = now;

                //int overall = (int)Math.Round(p.Percent);
                //overall = Math.Max(0, Math.Min(100, overall));
                //progressOverall.Value = overall;

               // finalBarBrutal1.Progress = p.Percent;


                finalBarBrutal1.CurrentSpeed = (long)p.SpeedBytesPerSec;
                finalBarBrutal1.FilesPerSecond = (long)p.FilesPerSec;

                finalBarBrutal1.Maximum = p.TotalBytesUncompressed;
                finalBarBrutal1.Value = p.BytesProcessedUncompressed;


                labelPercentageBar.Text = p.Percent.ToString("0.00") + "%";

                if (p.ETA.HasValue)
                    labelTimeRem.Text = "Time Remaining:   " + UIHelper.FormatTime(p.ETA.Value);

                labelName.Text = p.CurrentFile;
                labelElapsed.Text = "Time Elapsed:   " + UIHelper.FormatTime(p.Elapsed);
                labelFilesRemaining.Text = $"Files Remaining:   {(p.TotalFiles - p.FilesProcessed).ToString("N0")} / {p.TotalFiles.ToString("N0")}";
                labelFilesProcessed.Text = $"Files Processed:   {p.FilesProcessed.ToString("N0")} / {p.TotalFiles.ToString("N0")}";
                

                //lblMetrics.Text =
                //    $"{p.Operation}: {p.Percent:F1}%   " +
                //    $"{FormatBytes(p.BytesProcessedUncompressed)} / {FormatBytes(p.TotalBytesUncompressed)}   " +
                //    $"{FormatBytes((long)p.SpeedBytesPerSec)}/s   " +
                //    $"Files: {p.FilesProcessed}/{p.TotalFiles}" +
                //    (string.IsNullOrEmpty(p.CurrentFile) ? "" : $"\n{p.CurrentFile}");
            });
        }

        private static string FormatBytes(long bytes)
        {
            string[] u = { "B", "KB", "MB", "GB", "TB" };
            double s = bytes; int i = 0;
            while (s >= 1024 && i < u.Length - 1) { s /= 1024; i++; }
            return $"{s:F1} {u[i]}";
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts.Cancel();
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }

        private void tbThreads_Scroll(object sender, EventArgs e)
        {

        }

        private void tbThreads_ValueChanged(object sender, EventArgs e)
        {
            lblThreadsCur.Text = tbThreads.Value.ToString();
            _onThreadsChanged?.Invoke(tbThreads.Value);
        }
    }
}