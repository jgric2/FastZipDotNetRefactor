using FastZipDotNet.MultiThreading;
using FastZipDotNet.Zip;
using FastZipDotNet.Zip.Recovery;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;

namespace TestWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private CancellationTokenSource _cts;
        private void UpdateProgressUI(FastZipDotNet.Zip.ZipProgress p)
        {
            // Progress bar (0..100)
            progressBar1.Value = Math.Max(progressBar1.Minimum,
            Math.Min(progressBar1.Maximum, (int)Math.Round(p.Percent)));

            var eta = FormatEta(p.ETA);
            var currentRatio = FormatRatio(p.CurrentCompressionRatio);
            var archiveRatio = FormatRatio(p.ArchiveCompressionRatio);

            // Optional labels – customize to your controls
            labelOperation.Text = p.Operation.ToString(); // Extract / Build / Test
            labelCurrentFile.Text = string.IsNullOrEmpty(p.CurrentFile) ? "" : p.CurrentFile;

            labelCurrentFile.Text =
                $"{p.Percent:F1}%  " +
                $"{FormatBytes(p.BytesProcessedUncompressed)} / {FormatBytes(p.TotalBytesUncompressed)}  " +
                $"{FormatBytes((long)p.SpeedBytesPerSec)}/s  " +
                $"Files: {p.FilesProcessed}/{p.TotalFiles}  " +
                $"FPS: {p.FilesPerSec:0.0}  " +
                $"Elapsed: {p.Elapsed:hh\\:mm\\:ss}  " +
                $"ETA: {eta}  " +
                $"Current Ratio: {currentRatio}  " +
                // For Build, TotalBytesCompressed grows as we write; for Extract it’s fixed
                $"Archive Ratio: {archiveRatio}";
        }

        // Enable/disable UI safely while a job runs
        private void SetButtonsEnabled(bool enabled)
        {
            btnTest.Enabled = enabled;
            btnRepair.Enabled = enabled;
            // If you have other actions (build/update/extract), include them here too:
            btnBuild.Enabled = enabled;
            btnUpdate.Enabled = enabled;
            btnExtract.Enabled = enabled;
            btnCancel.Enabled = !enabled;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FastZipDotNet.Zip.FastZipDotNet zip = new FastZipDotNet.Zip.FastZipDotNet(@"C:\Users\Admin\Downloads\AlphaVSS-2.0.3 Kim WR.zip", FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Deflate))
            {
                //var res = zip.TestArchive();
                var tt1 = zip.ExtractArchiveAsync(@"C:\Users\Admin\Desktop\Test\scout");
            }
        }

        //public async Task<bool> AddFilesToArchiveAsync(string directoryToAdd, string outputDirectory)
        //{
        //    try
        //    {
        //        //var testP = @"C:\Users\Admin\Desktop\Test\Demo Data\Brutal Copy Screenshots";
        //        var filesList = new DirectoryInfo(directoryToAdd).GetFiles("*.*", SearchOption.AllDirectories).ToList();
        //        var lenB = directoryToAdd.Length;//item.FullPath.Length - item.InternalPath.Length;
        //                                //  var files = Directory.GetFiles(@"C:\Users\Admin\Desktop\Test\Demo Data", "*.*", EnumerationOptions)


        //        using (FastZipDotNet.Zip.FastZipDotNet zip = new FastZipDotNet.Zip.FastZipDotNet(@"C:\Users\Admin\Desktop\Test\Brutal Copy Test.zip", FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Deflate))
        //        {
        //            //  var res = zip.TestArchive();
        //            //  var tt1 = zip.ExtractArchive(@"C:\Users\Admin\Desktop\Test\scout");
        //            var procCount = Environment.ProcessorCount;
        //            var semaphore = new AdjustableSemaphore(procCount);
        //            var tasks = new List<Task>();
        //            var cts = new CancellationTokenSource();
        //            //AddFilesToZipAsync(filesList,)
        //            var tempList = new List<((string FullName, long Length) file, string internalPath)>();
        //            foreach (var itemrec in filesList)
        //            {
        //                // Wait on semaphore with cancellation support
        //                semaphore.WaitOne(cts.Token);

        //                var task = Task.Run(() =>
        //                {
        //                    try
        //                    {
        //                        string actualInternal = itemrec.FullName.Substring(lenB);
        //                        long compressedSize = zip.AddFile(itemrec.FullName, actualInternal);

        //                        lock (tempList)
        //                        {
        //                            tempList.Add(((itemrec.FullName, compressedSize), actualInternal));
        //                        }
        //                    }
        //                    finally
        //                    {
        //                        semaphore.Release();
        //                    }
        //                }, cts.Token);

        //                tasks.Add(task);
        //            }
        //            await Task.WhenAll(tasks);
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exception if extraction fails
        //        return false;
        //    }
        //}


        private async void button2_Click(object sender, EventArgs e)
        {
            //Stopwatch stpw = new Stopwatch();
            //stpw.Start();

            //using (FastZipDotNet.Zip.FastZipDotNet zip = new FastZipDotNet.Zip.FastZipDotNet(@"C:\Users\Admin\Desktop\Test\Brutal Copy Test rr21.zip", FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Deflate, 6, 1, false, "", 0 * 1024 * 1024))//734003200
            //{
            //    var task = Task.Run(async () => await zip.AddFilesToArchiveAsync(@"C:\Users\Admin\Desktop\Test\Demo Data\Brutal Copy Screenshots", 6));
            //    await task;
            //    //var res = zip.AddFilesToArchiveAsync(@"C:\Users\Admin\Desktop\Test\Demo Data\Brutal Copy Screenshots");
            //}

            //stpw.Stop();
            //var tt = stpw.Elapsed;
            //MessageBox.Show(tt.ToString());
            //var task = Task.Run(() => AddFilesToArchiveAsync(""));
        }

        private void button3_Click(object sender, EventArgs e)
        {

            ZipRecoveryHelper.RecoverCorruptedZipFile(@"C:\Users\Admin\Desktop\Test\Brutal Copy Test Damage.zip", @"C:\Users\Admin\Desktop\Test\Recovery"); ;

            //using (FastZipDotNet.Zip.FastZipDotNet zip = new FastZipDotNet.Zip.FastZipDotNet(@"C:\Users\Admin\Desktop\Test\Brutal Copy Test Damage.zip", FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Deflate))
            //{
            //    zip.RecoverCorruptedZip(@"C:\Users\Admin\Desktop\Test\Recovery");


            //    //var task = Task.Run(async () => await zip.AddFilesToArchiveAsync(@"C:\Users\Admin\Desktop\Test\Demo Data\Brutal Copy Screenshots"));
            //    //await task;
            //    //var res = zip.AddFilesToArchiveAsync(@"C:\Users\Admin\Desktop\Test\Demo Data\Brutal Copy Screenshots");
            //}
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            var progress = new Progress<FastZipDotNet.Zip.ZipProgress>(p =>
            {
                progressBar1.Value = (int)Math.Round(p.Percent);

                var eta = FormatEta(p.ETA);
                var archiveRatio = FormatRatio(p.ArchiveCompressionRatio);
                var currentRatio = FormatRatio(p.CurrentCompressionRatio);

                labelCurrentFile.Text =
                    $"{p.Percent:F1}%  " +
                    $"{FormatBytes(p.BytesProcessedUncompressed)} / {FormatBytes(p.TotalBytesUncompressed)}  " +
                    $"{FormatBytes((long)p.SpeedBytesPerSec)}/s  " +
                    $"Files: {p.FilesProcessed}/{p.TotalFiles}  " +
                    $"FPS: {p.FilesPerSec:0.0}  " +
                    $"Elapsed: {p.Elapsed:hh\\:mm\\:ss}  " +
                    $"ETA: {eta}  " +
                    $"Archive Ratio: {archiveRatio}  " +
                    $"Current Ratio: {currentRatio}  " +
                    $"{(string.IsNullOrEmpty(p.CurrentFile) ? "" : $"[{p.CurrentFile}]")}";
            });

            using (var zip = new FastZipDotNet.Zip.FastZipDotNet(
            @"C:\HDD\Test copy\1\Test\Extraction Point - Level 1 Beta.zip",
            FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Deflate))
            {
                var ok = await zip.ExtractArchiveAsync(
                @"C:\HDD\Test copy\1\Test\plz",
                progress,
                CancellationToken.None);
            }


        }

        static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1) { size /= 1024; unit++; }
            return $"{size:F1} {units[unit]}";
        }

        private static string FormatEta(TimeSpan? eta)
        {
            if (!eta.HasValue) return "--:--:--";
            var ts = eta.Value;
            if (ts < TimeSpan.Zero) ts = TimeSpan.Zero;

            // If >1 day, show days too
            return ts.TotalDays >= 1
                ? ts.ToString(@"d\.hh\:mm\:ss", CultureInfo.InvariantCulture)
                : ts.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        static string FormatRatio(double ratio)
        {
            if (ratio <= 0) return "—";
            // ratio < 1 means compression saved space
            double pct = (1.0 - ratio) * 100.0;
            return $"{ratio:0.000} ({pct:+0.0;-0.0;0.0}% saved)";
        }

        private void labelStatus_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
        }

        private async void btnBuild_Click(object sender, EventArgs e)
        {
            string sourceFolder = txtSourceFolder.Text;            // folder to add
            string outZipPath = txtOutputZip.Text;               // "C:\out\my.zip"
            int level = (int)numCompressionLevel.Value;            // 0..n

            btnBuild.Enabled = false;
            btnUpdate.Enabled = false;
            btnExtract.Enabled = false;
            _cts = new CancellationTokenSource();

            try
            {
                var progress = new Progress<FastZipDotNet.Zip.ZipProgress>(UpdateProgressUI);

                using (var zip = new FastZipDotNet.Zip.FastZipDotNet(
                    outZipPath,
                    FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Deflate,
                    compressionLevel: level,
                    threads: Environment.ProcessorCount))
                {
                    // Single-part recommended for best concurrency
                    // zip.PartSize = 0; // default

                    bool ok = await zip.AddFilesToArchiveAsync(
                        sourceFolder,
                        compressionlevel: level,
                        progress: progress,
                        ct: _cts.Token);

                    zip.Close(); // writes central directory

                    if (!ok)
                        labelCurrentFile.Text = "Build failed (or cancelled).";
                    else
                        labelCurrentFile.Text = "Build completed.";
                }
            }
            catch (OperationCanceledException)
            {
                labelCurrentFile.Text = "Build cancelled.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBuild.Enabled = true;
                btnUpdate.Enabled = true;
                btnExtract.Enabled = true;
                _cts.Dispose();
                _cts = null;
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            string sourceFolder = txtSourceFolder.Text;   // folder to sync from
            string zipPath = txtOutputZip.Text;       // existing zip path
            int level = (int)numCompressionLevel.Value;

            btnBuild.Enabled = false;
            btnUpdate.Enabled = false;
            btnExtract.Enabled = false;
            _cts = new CancellationTokenSource();

            try
            {
                var progress = new Progress<FastZipDotNet.Zip.ZipProgress>(UpdateProgressUI);

                using (var zip = new FastZipDotNet.Zip.FastZipDotNet(
                    zipPath,
                    FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Deflate,
                    compressionLevel: level,
                    threads: Environment.ProcessorCount))
                {
                    bool ok = await zip.UpdateFromDirectoryAsync(
                        sourceFolder,
                        compressionlevel: level,
                        progress: progress,
                        ct: _cts.Token);

                    zip.Close();

                    if (!ok)
                        labelCurrentFile.Text = "Update failed (or cancelled).";
                    else
                        labelCurrentFile.Text = "Update completed.";
                }
            }
            catch (OperationCanceledException)
            {
                labelCurrentFile.Text = "Update cancelled.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBuild.Enabled = true;
                btnUpdate.Enabled = true;
                btnExtract.Enabled = true;
                _cts?.Dispose();
                _cts = null;
            }
        }

        private async void btnExtract_Click(object sender, EventArgs e)
        {
            string zipPath = txtOutputZip.Text;
            string targetFolder = txtExtractTo.Text;

            btnBuild.Enabled = false;
            btnUpdate.Enabled = false;
            btnExtract.Enabled = false;
            _cts = new CancellationTokenSource();

            try
            {
                var progress = new Progress<FastZipDotNet.Zip.ZipProgress>(UpdateProgressUI);

                using (var zip = new FastZipDotNet.Zip.FastZipDotNet(
                    zipPath,
                    FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Deflate,
                    threads: Environment.ProcessorCount))
                {
                    bool ok = await zip.ExtractArchiveAsync(
                        targetFolder,
                        progress,
                        _cts.Token);

                    if (!ok)
                        labelCurrentFile.Text = "Extract failed (or cancelled).";
                    else
                        labelCurrentFile.Text = "Extract completed.";
                }
            }
            catch (OperationCanceledException)
            {
                labelCurrentFile.Text = "Extract cancelled.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBuild.Enabled = true;
                btnUpdate.Enabled = true;
                btnExtract.Enabled = true;
                _cts?.Dispose();
                _cts = null;
            }
        }

        private async void btnTest_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInputZip.Text) || !File.Exists(txtInputZip.Text))
            {
                MessageBox.Show(this, "Select a valid ZIP file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetButtonsEnabled(false);
            _cts = new CancellationTokenSource();

            try
            {
                var progress = new Progress<ZipProgress>(UpdateProgressUI);

                using var zip = new FastZipDotNet.Zip.FastZipDotNet(
                    txtInputZip.Text,
                    Compression.Deflate,
                    threads: Math.Max(1, Environment.ProcessorCount - 1)); // leave one core for UI/GC

                bool ok = await zip.TestArchiveAsync(progress, _cts.Token);

                labelStatus.Text = ok ? "Test completed." : "Test failed or cancelled.";
            }
            catch (OperationCanceledException)
            {
                labelStatus.Text = "Test cancelled.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Test error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cts.Dispose();
                _cts = null;
                SetButtonsEnabled(true);
            }
        }

        private async void btnRepair_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInputZip.Text) || !File.Exists(txtInputZip.Text))
            {
                MessageBox.Show(this, "Select a valid ZIP file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetButtonsEnabled(false);
            _cts = new CancellationTokenSource();

            try
            {
                var progress = new Progress<ZipProgress>(UpdateProgressUI);
                string zipPath = txtInputZip.Text;

                // 1) Try Quick Repair: rebuild central directory in place
                bool quickOk = await ZipRepair.RepairCentralDirectoryAsync(zipPath, progress, _cts.Token);
                if (quickOk)
                {
                    labelStatus.Text = "Quick repair completed (central directory rebuilt).";
                    return;
                }

                // 2) Fallback: Deep Repair (salvage to a new archive)
                int level = (int)numCompressionLevel.Value; // 0..n from your UI
                string repairedPath =
                    Path.Combine(Path.GetDirectoryName(zipPath) ?? "",
                                 Path.GetFileNameWithoutExtension(zipPath) + "_repaired" + Path.GetExtension(zipPath));

                bool deepOk = await ZipRepair.RepairToNewArchiveAsync(
                    zipPath,
                    repairedPath,
                    compressionLevel: level,
                    threads: Math.Max(1, Environment.ProcessorCount - 1),
                    progress: progress,
                    ct: _cts.Token);

                labelStatus.Text = deepOk
                    ? $"Deep repair completed: {repairedPath}"
                    : "Repair failed or cancelled.";
            }
            catch (OperationCanceledException)
            {
                labelStatus.Text = "Repair cancelled.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Repair error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cts.Dispose();
                _cts = null;
                SetButtonsEnabled(true);
            }
        }
    }
}
