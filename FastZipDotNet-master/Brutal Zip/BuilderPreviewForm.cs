using BrutalZip2025.BrutalControls;
using System.Diagnostics;
using System.Text;

namespace Brutal_Zip
{
    public partial class BuilderPreviewForm : ModernForm
    {
        private sealed class DemoFile
        {
            public string Name { get; init; }
            public long Size { get; init; }
            public long Done; // will be advanced by the demo
        }

        private readonly List<DemoFile> _files = new List<DemoFile>();
        private long _totalBytes;
        private long _processedBytes;
        private int _filesDone;

        private CancellationTokenSource _cts;
        private Task _demoTask;
        private Stopwatch _sw;

        private System.Windows.Forms.ListBox _fileListBox; // simple runtime file list to preview

        // ctor remains the same signature you already use in SfxBuilderForm.ShowPreview(...)
        public BuilderPreviewForm(string title, string company, Image banner, Icon icon, Color theme, Color themeEnd, bool showFileList)
        {
            InitializeComponent();

            if (banner != null) picBanner.Image = banner;
            Icon = icon ?? this.Icon;
            pnlTopAccent.StartColor = theme;
            pnlTopAccent.EndColor = themeEnd;
            lblTitle.Text = string.IsNullOrWhiteSpace(company) ? title : $"{company} — {title}";

            // Runtime list box (no designer change)
            _fileListBox = new System.Windows.Forms.ListBox
            {
                BackColor = Color.FromArgb(25, 25, 25),
                ForeColor = Color.White,
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                IntegralHeight = false,
                Visible = false
            };
            // Place it exactly where the details panel sits (toggle between them)
            _fileListBox.Bounds = brutalGradientPanel1.Bounds;
            _fileListBox.Anchor = brutalGradientPanel1.Anchor;
            Controls.Add(_fileListBox);

            // Make the button actually toggle the file list (we re-use your “Toggle File List” Button)
            btnClose.Click += (_, __) => ToggleFileList();

            // Start the demo when shown
            Shown += BuilderPreviewForm_Shown;
            FormClosing += BuilderPreviewForm_FormClosing;
        }

        private void BuilderPreviewForm_Shown(object sender, EventArgs e)
        {
            StartDemo();
        }

        private void BuilderPreviewForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            try { _cts?.Cancel(); } catch { }
        }

        private void ToggleFileList()
        {
            bool listVisible = _fileListBox.Visible;
            _fileListBox.Visible = !listVisible;
            brutalGradientPanel1.Visible = listVisible; // swap the detail panel visibility

            // Fill the list if becoming visible
            if (!listVisible)
            {
                _fileListBox.BeginUpdate();
                try
                {
                    _fileListBox.Items.Clear();
                    foreach (var f in _files)
                    {
                        _fileListBox.Items.Add($"{f.Name}  ({FormatBytes(f.Size)})");
                    }
                }
                finally { _fileListBox.EndUpdate(); }
            }
        }

        private void StartDemo()
        {
            _cts = new CancellationTokenSource();
            _sw = Stopwatch.StartNew();

            BuildDemoFiles(); // generate dummy file set
            InitUiToStart();

            _demoTask = Task.Run(async () =>
            {
                try
                {
                    var rnd = new Random(unchecked(Environment.TickCount * 31 + 7));

                    // simulate extraction of files
                    foreach (var f in _files)
                    {
                        if (_cts.IsCancellationRequested) break;

                        // Update “current file” label
                        BeginInvoke(new Action(() => labelName.Text = f.Name));

                        long remaining = f.Size;
                        long fileStartProcessed = _processedBytes;

                        // A per-file loop; chunk sizes vary to look natural
                        while (remaining > 0 && !_cts.IsCancellationRequested)
                        {
                            // “speed” shaping: 8–48 MB/s equivalent, translated into chunk sizes per tick
                            int chunk = rnd.Next(64 * 1024, 8 * 1024 * 1024); // 64 KB to 512 KB per tick

                            if (chunk > remaining) chunk = (int)remaining;
                            f.Done += chunk;
                            remaining -= chunk;

                            Interlocked.Add(ref _processedBytes, chunk);

                            UpdateUiTick();
                            var delay = rnd.Next(2, 50);
                            await Task.Delay(delay, _cts.Token).ConfigureAwait(false);
                        }

                        Interlocked.Increment(ref _filesDone);
                        // Mark file “done” (if the list is visible, reflect this)
                        if (_fileListBox.Visible)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                int idx = _files.IndexOf(f);
                                if (idx >= 0 && idx < _fileListBox.Items.Count)
                                {
                                    _fileListBox.Items[idx] = $"{f.Name}  ({FormatBytes(f.Size)}) ✓";
                                }
                            }));
                        }
                    }

                    // Final UI clamp (100%)
                    if (!_cts.IsCancellationRequested)
                    {
                        _processedBytes = _totalBytes;
                        UpdateUiTick(forceComplete: true);
                    }
                }
                catch (OperationCanceledException) { /* ignore */ }
                catch { /* ignore for demo */ }
            }, _cts.Token);
        }

        private void BuildDemoFiles()
        {
            _files.Clear();
            _processedBytes = 0;
            _filesDone = 0;

            // Create a “mix” of archive-like paths and sizes (in bytes)
            var rnd = new Random(unchecked(Environment.TickCount * 37 + 13));
            string[] roots = new[] { "assets", "bin", "res", "images", "docs", "data", "scripts", "content" };
            string[] exts = new[] { ".dll", ".png", ".jpg", ".json", ".txt", ".dat", ".wav", ".mp4", ".js", ".css" };

            int count = rnd.Next(40, 90); // feels busy but not forever
            for (int i = 0; i < count; i++)
            {
                string root = roots[rnd.Next(roots.Length)];
                string name = MakeName(rnd, exts);
                string sub = MakeSubPath(rnd);
                string path = string.IsNullOrEmpty(sub) ? $"{root}/{name}" : $"{root}/{sub}/{name}";

                long size = MakeSize(rnd); // 32KB – 16MB (random)
                _files.Add(new DemoFile { Name = path.Replace('/', '\\'), Size = size, Done = 0 });
            }

            // Compute “total bytes”
            long total = 0;
            foreach (var f in _files) total += f.Size;
            _totalBytes = total;
        }

        private static string MakeName(Random rnd, string[] exts)
        {
            string baseName;
            switch (rnd.Next(5))
            {
                case 0: baseName = "module"; break;
                case 1: baseName = "image"; break;
                case 2: baseName = "data"; break;
                case 3: baseName = "file"; break;
                default: baseName = "asset"; break;
            }
            int n = rnd.Next(1, 9999);
            string ext = exts[rnd.Next(exts.Length)];
            return $"{baseName}_{n:D4}{ext}";
        }

        private static string MakeSubPath(Random rnd)
        {
            // Create a shallow nested path sometimes
            int depth = rnd.Next(0, 3);
            if (depth == 0) return "";
            var sb = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                if (sb.Length > 0) sb.Append('/');
                sb.Append("sub").Append(rnd.Next(1, 15));
            }
            return sb.ToString();
        }

        private static long MakeSize(Random rnd)
        {
            // 32 KB – 16 MB, skewed towards smaller files
            int bucket = rnd.Next(100);
            if (bucket < 55) return rnd.Next(32 * 1024, 512 * 1024);        // 32KB–512KB
            if (bucket < 85) return rnd.Next(512 * 1024, 4 * 1024 * 1024);  // 512KB–4MB
            if (bucket < 97) return rnd.Next(4 * 1024 * 1024, 12 * 1024 * 1024); // 4–12MB
            return rnd.Next(12 * 1024 * 1024, 16 * 1024 * 1024);            // 12–16MB
        }

        private void InitUiToStart()
        {
            try
            {
                finalBarBrutal1.Maximum = _totalBytes;
                finalBarBrutal1.Value = 0;
                finalBarBrutal1.CurrentSpeed = 0;
                finalBarBrutal1.FilesPerSecond = 0;

                labelPercentageBar.Text = "0.00%";
                labelName.Text = "Starting…";

                labelElapsed.Text = "Time Elapsed:   0 Milliseconds.";
                labelTimeRem.Text = "Time Remaining:   --";
                labelFilesProcessed.Text = $"Files Processed:   0 / {_files.Count}";
                labelFilesRemaining.Text = $"Files Remaining:   {_files.Count} / {_files.Count}";
            }
            catch { }
        }

        private void UpdateUiTick(bool forceComplete = false)
        {
            try
            {
                var elapsed = _sw.Elapsed;
                long processed = _processedBytes;
                long total = _totalBytes;
                int filesDone = _filesDone;
                int filesTotal = _files.Count;

                // Derive speeds
                double secs = Math.Max(0.05, elapsed.TotalSeconds);
                double bps = processed / secs;
                double fps = filesDone / secs;

                double percent = total > 0 ? (double)processed / total * 100.0 : 0.0;

                // ETA
                TimeSpan? eta = null;
                if (bps > 1000 && processed < total)
                {
                    double left = total - processed;
                    eta = TimeSpan.FromSeconds(left / bps);
                }

                // Apply to UI
                BeginInvoke(new Action(() =>
                {
                    // progress bar control
                    finalBarBrutal1.Maximum = total;
                    finalBarBrutal1.Value = processed;
                    finalBarBrutal1.CurrentSpeed = (long)bps;
                    finalBarBrutal1.FilesPerSecond = (long)fps;

                    labelPercentageBar.Text = percent.ToString("0.00") + "%";

                    labelElapsed.Text = "Time Elapsed:   " + Classes.Helpers.UIHelper.FormatTime(elapsed);
                    if (eta.HasValue)
                        labelTimeRem.Text = "Time Remaining:   " + Classes.Helpers.UIHelper.FormatTime(eta.Value);
                    else
                        labelTimeRem.Text = "Time Remaining:   --";

                    labelFilesProcessed.Text = $"Files Processed:   {filesDone:N0} / {filesTotal:N0}";
                    labelFilesRemaining.Text = $"Files Remaining:   {(filesTotal - filesDone):N0} / {filesTotal:N0}";

                    if (forceComplete)
                    {
                        labelName.Text = "Completed.";
                        labelPercentageBar.Text = "100.00%";
                    }
                }));
            }
            catch { }
        }

        // Button currently labeled “Toggle File List”
        private void btnClose_Click(object sender, EventArgs e)
        {
            ToggleFileList();
        }

        // Formatting helper
        private static string FormatBytes(long bytes)
        {
            string[] u = { "B", "KB", "MB", "GB", "TB" };
            double s = bytes; int i = 0;
            while (s >= 1024 && i < u.Length - 1) { s /= 1024; i++; }
            return $"{s:F1} {u[i]}";
        }
    }
}
