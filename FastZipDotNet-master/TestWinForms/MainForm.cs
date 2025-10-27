using FastZipDotNet.Zip;
using FastZipDotNet.Zip.Recovery;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

namespace TestWinForms
{
    public partial class MainForm : Form
    {
        private FastZipDotNet.Zip.FastZipDotNet _zip;      // opened archive
        private int _threads = Math.Max(1, Environment.ProcessorCount - 1);

        private DirNode _root;
        private DirNode _current;
        private string _currentPath = "/";

        private IconCache _icons = new IconCache();

        private readonly List<string> _stagedItems = new();

        public MainForm()
        {
            InitializeComponent();
            lvExtract.SmallImageList = _icons.ImageList;
            lvCreate.SmallImageList = _icons.ImageList;
            InitColumns();
            WireDragDrop();
            UpdatePathLabel("/");
        }

        private void InitColumns()
        {
            lvExtract.View = View.Details;
            lvExtract.FullRowSelect = true;
            lvExtract.HideSelection = false;
            if (lvExtract.Columns.Count == 0)
            {
                lvExtract.Columns.Add("Name", 420);
                lvExtract.Columns.Add("Size", 90, HorizontalAlignment.Right);
                lvExtract.Columns.Add("Compressed", 100, HorizontalAlignment.Right);
                lvExtract.Columns.Add("Method", 80);
                lvExtract.Columns.Add("Modified", 140);
            }

            lvCreate.View = View.Details;
            lvCreate.FullRowSelect = true;
            lvCreate.HideSelection = false;
            if (lvCreate.Columns.Count == 0)
            {
                lvCreate.Columns.Add("Path", 600);
                lvCreate.Columns.Add("Type", 80);
                lvCreate.Columns.Add("Size", 120, HorizontalAlignment.Right);
            }

            // Defaults for Create tab settings
            cmbMethod.SelectedIndex = 1; // Deflate
            numLevel.Minimum = 0; numLevel.Maximum = 22; numLevel.Value = 6;
        }

        private void WireDragDrop()
        {
            lvCreate.AllowDrop = true;
            lvCreate.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
            };
            lvCreate.DragDrop += (s, e) =>
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                AddStagedItems(paths);
            };
        }

        private void UpdatePathLabel(string path)
        {
            _currentPath = path;
            lblCurrentPath.Text = path;
        }

        // =================== Extract tab logic ===================

        private void btnOpenZip_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "Zip Files|*.zip", Multiselect = false };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;
            OpenArchive(ofd.FileName);
        }

        private void OpenArchive(string zipPath)
        {
            try
            {
                _zip?.Dispose();
                _zip = new FastZipDotNet.Zip.FastZipDotNet(
                    zipPath,
                    Compression.Deflate,
                    compressionLevel: 6, // irrelevant for read
                    threads: _threads);

                // Detect multi-part or encrypted
                if (IsMultipartArchive(_zip) || IsEncryptedArchive(_zip))
                {
                    _zip.Dispose();
                    _zip = null;
                    MessageBox.Show(this, "This archive is multi-part or password-protected. The current version does not support it.", "Not supported");
                    lblExtractInfo.Text = "Not loaded";
                    lvExtract.Items.Clear();
                    _root = _current = null;
                    return;
                }

                BuildTree(_zip.ZipFileEntries);
                NavigateTo(_root);
                lblExtractInfo.Text = $"{Path.GetFileName(zipPath)} - {_zip.ZipFileEntries.Count} entries";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "OpenZip error");
                _zip?.Dispose();
                _zip = null;
            }
        }

        private static bool IsMultipartArchive(FastZipDotNet.Zip.FastZipDotNet zip)
        {
            // If any entry starts on a non-zero disk, it's multi-part
            return zip.ZipFileEntries.Any(e => e.DiskNumberStart != 0);
        }

        private static bool IsEncryptedArchive(FastZipDotNet.Zip.FastZipDotNet zip)
        {
            // If any entry is encrypted (either ZipCrypto or AES), reject
            return false;//zip.ZipFileEntries.Any(e => e.AesEncrypted || e.Encrypted);
        }

        private void BuildTree(List<ZipFileEntry> entries)
        {
            _root = new DirNode(null, "");
            foreach (var e in entries)
            {
                var isDir = IsDirectory(e);
                var full = (e.FilenameInZip ?? "").Replace('\\', '/').Trim('/');
                if (full.Length == 0)
                {
                    if (isDir) continue;
                    continue;
                }

                var parts = full.Split('/');
                DirNode node = _root;
                for (int i = 0; i < parts.Length - (isDir ? 0 : 1); i++)
                {
                    string part = parts[i];
                    if (!node.Dirs.TryGetValue(part, out var child))
                    {
                        child = new DirNode(node, part);
                        node.Dirs[part] = child;
                    }
                    node = child;
                }

                if (isDir)
                {
                    string dname = parts[^1];
                    if (!node.Dirs.ContainsKey(dname))
                        node.Dirs[dname] = new DirNode(node, dname);
                }
                else
                {
                    node.Files.Add(e);
                }
            }
        }

        private void NavigateTo(DirNode node)
        {
            _current = node ?? _root;

            // Build path string
            var parts = new List<string>();
            var p = _current;
            while (p != null && p.Name != "")
            {
                parts.Add(p.Name);
                p = p.Parent;
            }
            parts.Reverse();
            string path = "/" + string.Join("/", parts);
            UpdatePathLabel(path.Length == 0 ? "/" : path);

            PopulateExtractList(_current);
        }

        private void PopulateExtractList(DirNode node)
        {
            lvExtract.BeginUpdate();
            lvExtract.Items.Clear();

            if (node.Parent != null)
            {
                var upItem = new ListViewItem(new[] { "..", "", "", "", "" })
                {
                    Tag = "..",
                    ImageKey = _icons.FolderKey
                };
                lvExtract.Items.Add(upItem);
            }

            foreach (var kv in node.Dirs.OrderBy(d => d.Key, StringComparer.OrdinalIgnoreCase))
            {
                var item = new ListViewItem(new[]
                {
                    kv.Key, "", "", "Folder", ""
                })
                {
                    Tag = kv.Value,
                    ImageKey = _icons.FolderKey
                };
                lvExtract.Items.Add(item);
            }

            foreach (var f in node.Files.OrderBy(f => f.FilenameInZip, StringComparer.OrdinalIgnoreCase))
            {
                string fname = Path.GetFileName(f.FilenameInZip.Replace('\\', '/'));
                var item = new ListViewItem(new[]
                {
                    fname,
                    FormatBytes((long)f.FileSize),
                    FormatBytes((long)f.CompressedSize),
                    f.Method.ToString(),
                    f.ModifyTime.ToString("yyyy-MM-dd HH:mm:ss")
                })
                {
                    Tag = f,
                    ImageKey = _icons.GetFileIconKeyByExtension(Path.GetExtension(fname))
                };
                lvExtract.Items.Add(item);
            }

            lvExtract.EndUpdate();
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

        private static bool IsDirectory(ZipFileEntry zfe)
        {
            if (!string.IsNullOrEmpty(zfe.FilenameInZip))
            {
                char last = zfe.FilenameInZip[^1];
                if (last == '/' || last == '\\') return true;
            }
            if ((zfe.ExternalFileAttr & 0x10u) != 0) return true;
            uint unixMode = (zfe.ExternalFileAttr >> 16) & 0xFFFFu;
            return (unixMode & 0xF000u) == 0x4000u;
        }

        private void lvExtract_DoubleClick(object sender, EventArgs e)
        {
            if (lvExtract.SelectedItems.Count == 0) return;
            var it = lvExtract.SelectedItems[0].Tag;

            if (it is string s && s == "..")
            {
                NavigateTo(_current.Parent ?? _root);
            }
            else if (it is DirNode d)
            {
                NavigateTo(d);
            }
            else if (it is ZipFileEntry)
            {
                // Could add preview logic in the future
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (_current?.Parent != null)
                NavigateTo(_current.Parent);
        }

        private void btnExtractSelected_Click(object sender, EventArgs e)
        {
            if (_zip == null) { MessageBox.Show(this, "Open a zip first.", "Info"); return; }
            if (lvExtract.SelectedItems.Count == 0) { MessageBox.Show(this, "Select items.", "Info"); return; }

            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) != DialogResult.OK) return;

            var files = new List<ZipFileEntry>();
            foreach (ListViewItem it in lvExtract.SelectedItems)
            {
                if (it.Tag is ZipFileEntry f) files.Add(f);
                else if (it.Tag is DirNode dir) CollectAllFiles(dir, files);
            }

            _ = ExtractEntriesAsync(files, fbd.SelectedPath);
        }

        private void btnExtractAll_Click(object sender, EventArgs e)
        {
            if (_zip == null) { MessageBox.Show(this, "Open a zip first.", "Info"); return; }
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) != DialogResult.OK) return;

            var files = new List<ZipFileEntry>();
            CollectAllFiles(_root, files);
            _ = ExtractEntriesAsync(files, fbd.SelectedPath);
        }

        private void CollectAllFiles(DirNode node, List<ZipFileEntry> dest)
        {
            dest.AddRange(node.Files);
            foreach (var kv in node.Dirs)
                CollectAllFiles(kv.Value, dest);
        }

        private async Task ExtractEntriesAsync(List<ZipFileEntry> entries, string targetFolder)
        {
            if (entries == null || entries.Count == 0) return;

            using var pf = new ProgressForm("Extracting...");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                var sem = new SemaphoreSlim(_threads, _threads);
                var tasks = new List<Task>(entries.Count);

                long totalBytes = entries.Sum(e => (long)e.FileSize);
                long processed = 0;
                int filesDone = 0;
                var sw = Stopwatch.StartNew();

                using var reporterCts = new CancellationTokenSource();
                var reportingTask = Task.Factory.StartNew(async () =>
                {
                    while (!reporterCts.IsCancellationRequested)
                    {
                        var elapsed = sw.Elapsed;
                        long unc = Math.Min(Interlocked.Read(ref processed), totalBytes);
                        int files = Volatile.Read(ref filesDone);
                        double speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;

                        progress.Report(new ZipProgress
                        {
                            Operation = ZipOperation.Extract,
                            TotalFiles = entries.Count,
                            TotalBytesUncompressed = totalBytes,
                            FilesProcessed = files,
                            BytesProcessedUncompressed = unc,
                            Elapsed = elapsed,
                            SpeedBytesPerSec = speed
                        });

                        try { await Task.Delay(200, reporterCts.Token).ConfigureAwait(false); } catch { break; }
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

                foreach (var entry in entries)
                {
                    if (pf.Token.IsCancellationRequested) break;
                    await sem.WaitAsync().ConfigureAwait(false);

                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            string outPath = Path.Combine(targetFolder, entry.FilenameInZip);
                            Directory.CreateDirectory(Path.GetDirectoryName(outPath) ?? ".");

                            void OnBytes(long n) => Interlocked.Add(ref processed, n);

                            if (IsDirectory(entry))
                            {
                                Directory.CreateDirectory(outPath);
                                Interlocked.Increment(ref filesDone);
                                return;
                            }

                            bool ok = _zip.ZipDataReader.ExtractFile(entry, outPath, OnBytes);
                            if (ok) Interlocked.Increment(ref filesDone);
                        }
                        finally { sem.Release(); }
                    }, pf.Token));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
                reporterCts.Cancel();
                try { await reportingTask.ConfigureAwait(false); } catch { }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Extract error"); }
            finally 
            {
                Invoke((MethodInvoker)delegate
                {
                    pf.Close();
                });
               
            }
        }

        private void btnAddFilesExtract_Click(object sender, EventArgs e)
        {
            if (_zip == null) { MessageBox.Show(this, "Open a zip first.", "Info"); return; }
            using var ofd = new OpenFileDialog { Filter = "All files|*.*", Multiselect = true };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;
            _ = AddFilesIntoArchiveAtCurrentPathAsync(ofd.FileNames);
        }

        private void btnAddFolderExtract_Click(object sender, EventArgs e)
        {
            if (_zip == null) { MessageBox.Show(this, "Open a zip first.", "Info"); return; }
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) != DialogResult.OK) return;
            _ = AddFolderIntoArchiveAtCurrentPathAsync(fbd.SelectedPath);
        }


        private void CollectAllFilenames(DirNode node, HashSet<string> dest)
        {
            foreach (var f in node.Files) dest.Add(f.FilenameInZip);
            foreach (var kv in node.Dirs) CollectAllFilenames(kv.Value, dest);
        }

        private async void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (_zip == null) return;
            if (lvExtract.SelectedItems.Count == 0)
            {
                MessageBox.Show(this, "Select items to delete.");
                return;
            }
            // Collect entries to remove
            var toRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (ListViewItem it in lvExtract.SelectedItems)
            {
                if (it.Tag is ZipFileEntry f)
                    toRemove.Add(f.FilenameInZip);
                else if (it.Tag is DirNode d)
                    CollectAllFilenames(d, toRemove);
            }

            // Remove from in-memory list
            _zip.ZipFileEntries.RemoveAll(e2 => toRemove.Contains(e2.FilenameInZip));

            // Quick commit: write central directory
            using var pf = new ProgressForm("Updating archive...");
            pf.Show(this);
            await Task.Run(() => _zip.Close());
            pf.Close();

            OpenArchive(_zip.ZipFileName);
            NavigateTo(FindNodeByPath(_currentPath));
        }

        private async Task AddFilesIntoArchiveAtCurrentPathAsync(IEnumerable<string> paths)
        {
            using var pf = new ProgressForm("Adding files...");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                var sem = new SemaphoreSlim(_threads, _threads);
                var tasks = new List<Task>();

                long totalBytes = 0;
                foreach (var p in paths) totalBytes += new FileInfo(p).Length;
                long processed = 0;
                int filesDone = 0;
                var sw = Stopwatch.StartNew();

                using var reporterCts = new CancellationTokenSource();
                var reportingTask = Task.Factory.StartNew(async () =>
                {
                    while (!reporterCts.IsCancellationRequested)
                    {
                        var elapsed = sw.Elapsed;
                        long unc = Math.Min(Interlocked.Read(ref processed), totalBytes);
                        int files = Volatile.Read(ref filesDone);
                        double speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;

                        progress.Report(new ZipProgress
                        {
                            Operation = ZipOperation.Build,
                            TotalFiles = paths.Count(),
                            TotalBytesUncompressed = totalBytes,
                            FilesProcessed = files,
                            BytesProcessedUncompressed = unc,
                            Elapsed = elapsed,
                            SpeedBytesPerSec = speed
                        });

                        try { await Task.Delay(200, reporterCts.Token).ConfigureAwait(false); } catch { break; }
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

                string prefix = _current == _root ? "" : GetCurrentVirtualPathPrefix();

                foreach (var p in paths)
                {
                    if (pf.Token.IsCancellationRequested) break;
                    await sem.WaitAsync().ConfigureAwait(false);

                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            string name = Path.GetFileName(p);
                            string internalName = CombineZipPath(prefix, name);

                            void OnUnc(long n) => Interlocked.Add(ref processed, n);
                            void OnComp(long n) { }

                            _zip.ZipDataWriter.AddFileWithProgress(p, internalName, 6, "", OnUnc, OnComp);
                            Interlocked.Increment(ref filesDone);
                        }
                        finally { sem.Release(); }
                    }, pf.Token));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
                reporterCts.Cancel();
                try { await reportingTask.ConfigureAwait(false); } catch { }

                await Task.Run(() => _zip.Close());
                OpenArchive(_zip.ZipFileName);
                NavigateTo(FindNodeByPath(_currentPath));
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Add files error"); }
            finally { pf.Close(); }
        }

        private async Task AddFolderIntoArchiveAtCurrentPathAsync(string folder)
        {
            using var pf = new ProgressForm("Adding folder...");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                long totalBytes = files.Sum(f => new FileInfo(f).Length);
                long processed = 0;
                int filesDone = 0;

                var sem = new SemaphoreSlim(_threads, _threads);
                var tasks = new List<Task>(files.Length);
                var sw = Stopwatch.StartNew();

                using var reporterCts = new CancellationTokenSource();
                var reportingTask = Task.Factory.StartNew(async () =>
                {
                    while (!reporterCts.IsCancellationRequested)
                    {
                        var elapsed = sw.Elapsed;
                        long unc = Math.Min(Interlocked.Read(ref processed), totalBytes);
                        int filesCount = Volatile.Read(ref filesDone);
                        double speed = elapsed.TotalSeconds > 0 ? unc / elapsed.TotalSeconds : 0.0;

                        progress.Report(new ZipProgress
                        {
                            Operation = ZipOperation.Build,
                            TotalFiles = files.Length,
                            TotalBytesUncompressed = totalBytes,
                            FilesProcessed = filesCount,
                            BytesProcessedUncompressed = unc,
                            Elapsed = elapsed,
                            SpeedBytesPerSec = speed
                        });

                        try { await Task.Delay(200, reporterCts.Token).ConfigureAwait(false); } catch { break; }
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

                string prefix = _current == _root ? "" : GetCurrentVirtualPathPrefix();
                string baseDir = Path.GetFullPath(folder);

                foreach (var file in files)
                {
                    if (pf.Token.IsCancellationRequested) break;

                    string rel = Path.GetRelativePath(baseDir, file).Replace('\\', '/');
                    string internalName = CombineZipPath(prefix, rel);

                    await sem.WaitAsync().ConfigureAwait(false);
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            void OnUnc(long n) => Interlocked.Add(ref processed, n);
                            void OnComp(long n) { }
                            _zip.ZipDataWriter.AddFileWithProgress(file, internalName, 6, "", OnUnc, OnComp);
                            Interlocked.Increment(ref filesDone);
                        }
                        finally { sem.Release(); }
                    }, pf.Token));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
                reporterCts.Cancel();
                try { await reportingTask.ConfigureAwait(false); } catch { }

                await Task.Run(() => _zip.Close());
                OpenArchive(_zip.ZipFileName);
                NavigateTo(FindNodeByPath(_currentPath));
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Add folder error"); }
            finally { pf.Close(); }
        }

        private string GetCurrentVirtualPathPrefix()
        {
            var parts = new List<string>();
            var p = _current;
            while (p != null && p.Name != "")
            {
                parts.Add(p.Name);
                p = p.Parent;
            }
            parts.Reverse();
            return parts.Count == 0 ? "" : string.Join("/", parts) + "/";
        }

        private DirNode FindNodeByPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path == "/") return _root;
            var parts = path.Trim('/').Split('/');
            DirNode n = _root;
            foreach (var part in parts)
            {
                if (!n.Dirs.TryGetValue(part, out var next)) return _root;
                n = next;
            }
            return n;
        }

        private static string CombineZipPath(string prefix, string name)
        {
            prefix = prefix?.Trim('/') ?? "";
            if (string.IsNullOrEmpty(prefix)) return name.Replace('\\', '/');
            return (prefix + "/" + name).Replace('\\', '/');
        }

        private async void btnCompact_Click(object sender, EventArgs e)
        {
            if (_zip == null) return;
            using var pf = new ProgressForm("Compacting (rebuild)...");
            pf.Show(this);
            try
            {
                await Task.Run(() => _zip.ZipDataWriter.RebuildZip());
                OpenArchive(_zip.ZipFileName);
                NavigateTo(_root);
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Compact error"); }
            finally { pf.Close(); }
        }

        // Test & Repair & Info

        private async void btnTest_Click(object sender, EventArgs e)
        {
            if (_zip == null) { MessageBox.Show(this, "Open a zip first.", "Info"); return; }

            using var pf = new ProgressForm("Testing archive...");
            var progress = pf.CreateProgress();
            pf.Show(this);
            try
            {
                bool ok = await _zip.ZipDataReader.TestArchiveAsync(_threads, progress, pf.Token);
                MessageBox.Show(this, ok ? "Test completed successfully." : "Test failed or cancelled.", "Test");
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Test error"); }
            finally { pf.Close(); }
        }

        private async void btnRepair_Click(object sender, EventArgs e)
        {
            if (_zip == null) { MessageBox.Show(this, "Open a zip first.", "Info"); return; }

            using var pf = new ProgressForm("Repairing archive...");
            var progress = pf.CreateProgress();
            pf.Show(this);
            try
            {
                string zipPath = _zip.ZipFileName;

                bool okQuick = await ZipRepair.RepairCentralDirectoryAsync(zipPath, progress, pf.Token);
                if (okQuick)
                {
                    MessageBox.Show(this, "Quick repair completed (central directory rebuilt).", "Repair");
                    // Reload
                    OpenArchive(zipPath);
                    return;
                }

                string repaired = Path.Combine(Path.GetDirectoryName(zipPath) ?? "", Path.GetFileNameWithoutExtension(zipPath) + "_repaired.zip");
                bool okDeep = await ZipRepair.RepairToNewArchiveAsync(zipPath, repaired, 6, _threads, progress, pf.Token);
                MessageBox.Show(this, okDeep ? $"Deep repair completed: {repaired}" : "Deep repair failed or cancelled.", "Repair");

                if (okDeep) OpenArchive(repaired);
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Repair error"); }
            finally { pf.Close(); }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (_zip == null) { MessageBox.Show(this, "Open a zip first.", "Info"); return; }
            ShowArchiveInfo(_zip);
        }

        private void ShowArchiveInfo(FastZipDotNet.Zip.FastZipDotNet zip)
        {
            var entries = zip.ZipFileEntries;
            long totalUnc = entries.Sum(e => (long)e.FileSize);
            long totalCmp = entries.Sum(e => (long)e.CompressedSize);
            double ratio = totalUnc > 0 ? (double)totalCmp / totalUnc : 1.0;

            int files = entries.Count(e => !IsDirectory(e));
            int folders = entries.Count(e => IsDirectory(e));
            DateTime? minDate = entries.Count > 0 ? entries.Min(e => e.ModifyTime) : (DateTime?)null;
            DateTime? maxDate = entries.Count > 0 ? entries.Max(e => e.ModifyTime) : (DateTime?)null;

            string msg =
                $"File: {Path.GetFileName(zip.ZipFileName)}\r\n" +
                $"Entries: {entries.Count} (Files: {files}, Folders: {folders})\r\n" +
                $"Total Uncompressed: {FormatBytes(totalUnc)}\r\n" +
                $"Total Compressed:   {FormatBytes(totalCmp)}\r\n" +
                $"Ratio: {ratio:0.000} ({(1.0 - ratio) * 100:0.0}% saved)\r\n" +
                $"Modified: {(minDate.HasValue ? minDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "-")} .. {(maxDate.HasValue ? maxDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "-")}\r\n";

            MessageBox.Show(this, msg, "Archive Info");
        }

        // Context menu handlers (Extract ListView)
        private void mnuExtract_Open_Click(object sender, EventArgs e)
        {
            if (lvExtract.SelectedItems.Count == 0) return;
            var tag = lvExtract.SelectedItems[0].Tag;
            if (tag is string s && s == "..") NavigateTo(_current.Parent ?? _root);
            else if (tag is DirNode d) NavigateTo(d);
        }
        private void mnuExtract_ExtractSelected_Click(object sender, EventArgs e)
        {
            btnExtractSelected_Click(sender, e);
        }

        // Context menu handlers (Create ListView)
        private void mnuCreate_RemoveSelected_Click(object sender, EventArgs e)
        {
            if (lvCreate.SelectedItems.Count == 0) return;
            foreach (ListViewItem it in lvCreate.SelectedItems)
            {
                var path = (string)it.Tag;
                _stagedItems.Remove(path);
                lvCreate.Items.Remove(it);
            }
        }
        private void mnuCreate_Clear_Click(object sender, EventArgs e)
        {
            _stagedItems.Clear();
            lvCreate.Items.Clear();
        }

        // =================== Create tab logic ===================

        private void btnCreateAddFiles_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "All files|*.*", Multiselect = true };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;
            AddStagedItems(ofd.FileNames);
        }

        private void btnCreateAddFolder_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) != DialogResult.OK) return;
            AddStagedItems(new[] { fbd.SelectedPath });
        }

        private void AddStagedItems(IEnumerable<string> paths)
        {
            lvCreate.BeginUpdate();
            foreach (var p in paths)
            {
                if (Directory.Exists(p))
                {
                    _stagedItems.Add(p);
                    var item = new ListViewItem(new[] { p, "Folder", "" })
                    {
                        Tag = p,
                        ImageKey = _icons.FolderKey
                    };
                    lvCreate.Items.Add(item);
                }
                else if (File.Exists(p))
                {
                    var fi = new FileInfo(p);
                    _stagedItems.Add(p);
                    var item = new ListViewItem(new[] { p, "File", FormatBytes(fi.Length) })
                    {
                        Tag = p,
                        ImageKey = _icons.GetFileIconKeyByExtension(Path.GetExtension(p))
                    };
                    lvCreate.Items.Add(item);
                }
            }
            lvCreate.EndUpdate();
        }

        private async void btnBuildArchive_Click(object sender, EventArgs e)
        {
            if (_stagedItems.Count == 0)
            {
                MessageBox.Show(this, "Add files/folders to the staging list.", "Info");
                return;
            }

            using var sfd = new SaveFileDialog { Filter = "Zip Files|*.zip" };
            if (sfd.ShowDialog(this) != DialogResult.OK) return;

            // Get compression settings
            var method = cmbMethod.SelectedIndex switch
            {
                0 => "Store",
                1 => "Deflate",
                2 => "Zstd",
                _ => "Deflate"
            };
            int level = (int)numLevel.Value;

            using var pf = new ProgressForm("Building archive...");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                var methodEnum = Compression.Deflate;
                if (method == "Store") methodEnum = Compression.Store;
                else if (method == "Zstd") methodEnum = Compression.Zstd;

                using var zip = new FastZipDotNet.Zip.FastZipDotNet(sfd.FileName, methodEnum, level, _threads);

                // Add staged items
                foreach (var path in _stagedItems)
                {
                    if (pf.Token.IsCancellationRequested) break;

                    if (Directory.Exists(path))
                    {
                        bool ok = await zip.ZipDataWriter.AddFilesToArchiveAsync(path, _threads, level, progress, pf.Token);
                        if (!ok) break;
                    }
                    else if (File.Exists(path))
                    {
                        void OnUnc(long n) { }
                        void OnComp(long n) { }
                        zip.ZipDataWriter.AddFileWithProgress(path, Path.GetFileName(path), level, "", OnUnc, OnComp);
                    }
                }

                zip.Close();
                MessageBox.Show(this, "Archive built.", "Done");
                _stagedItems.Clear();
                lvCreate.Items.Clear();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Build error"); }
            finally { pf.Close(); }
        }

        // ============ Directory Model & IconCache ============

        private class DirNode
        {
            public DirNode Parent;
            public string Name;
            public Dictionary<string, DirNode> Dirs = new(StringComparer.OrdinalIgnoreCase);
            public List<ZipFileEntry> Files = new();
            public DirNode(DirNode parent, string name) { Parent = parent; Name = name; }
        }

        private class IconCache
        {
            public ImageList ImageList { get; } = new ImageList { ColorDepth = ColorDepth.Depth32Bit };
            public string FolderKey { get; } = "__folder";

            public IconCache()
            {
                var folderIcon = GetShellIcon(null, true);
                ImageList.Images.Add(FolderKey, folderIcon);
            }

            public string GetFileIconKeyByExtension(string ext)
            {
                if (string.IsNullOrEmpty(ext)) ext = "__noext";
                if (!ImageList.Images.ContainsKey(ext))
                {
                    var icon = GetShellIcon(ext, false);
                    ImageList.Images.Add(ext, icon);
                }
                return ext;
            }

            // SHGetFileInfo for small icons
            public static System.Drawing.Icon GetShellIcon(string extensionOrNull, bool folder)
            {
                SHFILEINFO shinfo = new SHFILEINFO();
                uint flags = SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES;
                uint attrs = folder ? FILE_ATTRIBUTE_DIRECTORY : FILE_ATTRIBUTE_NORMAL;

                string path = folder ? "folder" : (string.IsNullOrEmpty(extensionOrNull) ? "file" : "*" + extensionOrNull);

                IntPtr hImg = SHGetFileInfo(path, attrs, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
                if (shinfo.hIcon != IntPtr.Zero)
                {
                    var icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
                    DestroyIcon(shinfo.hIcon);
                    return icon;
                }
                return System.Drawing.SystemIcons.WinLogo;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct SHFILEINFO
            {
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            }

            private const uint SHGFI_ICON = 0x000000100;
            private const uint SHGFI_SMALLICON = 0x000000001;
            private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
            private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
            private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            private static extern IntPtr SHGetFileInfo(
                string pszPath,
                uint dwFileAttributes,
                ref SHFILEINFO psfi,
                uint cbFileInfo,
                uint uFlags);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern bool DestroyIcon(IntPtr hIcon);
        }
    }
}
