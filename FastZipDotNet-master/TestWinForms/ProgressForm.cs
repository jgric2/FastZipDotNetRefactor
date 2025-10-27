using FastZipDotNet.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWinForms
{
    public partial class ProgressForm : Form
    {
        private readonly System.Threading.CancellationTokenSource _cts;
        public System.Threading.CancellationToken Token => _cts.Token;

        public ProgressForm(string title)
        {
            InitializeComponent();
            Text = title;
            _cts = new System.Threading.CancellationTokenSource();
            btnCancel.Click += (s, e) => _cts.Cancel();
        }

        public IProgress<ZipProgress> CreateProgress()
        {
            return new Progress<ZipProgress>(p =>
            {
                int v = (int)Math.Round(p.Percent);
                if (v < progressBar1.Minimum) v = progressBar1.Minimum;
                if (v > progressBar1.Maximum) v = progressBar1.Maximum;
                progressBar1.Value = v;

                labelStatus.Text =
                    $"{p.Operation}: {p.Percent:F1}%  " +
                    $"{FormatBytes(p.BytesProcessedUncompressed)} / {FormatBytes(p.TotalBytesUncompressed)}  " +
                    $"{FormatBytes((long)p.SpeedBytesPerSec)}/s  " +
                    $"Files: {p.FilesProcessed}/{p.TotalFiles}  " +
                    $"{(string.IsNullOrEmpty(p.CurrentFile) ? "" : $"[{p.CurrentFile}]")}";
            });
        }

        private static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }
            return $"{size:F1} {units[unit]}";
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }
    }
}
