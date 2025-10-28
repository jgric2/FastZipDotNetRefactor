using Brutal_Zip.Views;
using BrutalZip;
using FastZipDotNet.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;
using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

namespace Brutal_Zip
{
    public partial class MainForm : Form
    {
        private FastZipDotNet.Zip.FastZipDotNet _zip;
        private string _zipPath;
        private DirNode _root, _current;
        private readonly List<Row> _rows = new();
        private readonly IconCache _icons = new IconCache();

        private readonly List<string> _stagedCreateItems = new();
        private readonly List<StagedItem> _staging = new();

        // Session temp for preview/open
        private readonly string _sessionTemp = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "BrutalZipSession_" + Environment.ProcessId);
        private CancellationTokenSource _previewCts;
        private string _lastPreviewTempFile;


        private int Threads => SettingsService.Current.ThreadsAuto
            ? Math.Max(1, Environment.ProcessorCount - 1)
            : Math.Max(1, SettingsService.Current.Threads);


        private class StagedItem
        {
            public string Path;
            public bool IsFolder;
            public long SizeBytes;     // total bytes (folders computed async)
            public int ItemsCount;     // files count (for folders)
            public string Name => System.IO.Path.GetFileName(Path.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar));
            public string Type => IsFolder ? "Folder" : "File";
        }

        public MainForm()
        {
            InitializeComponent();

            // Home events
            homeView.AddFilesClicked += AddCreateFiles;
            homeView.AddFolderClicked += AddCreateFolder;
            homeView.BrowseCreateDestinationClicked += BrowseCreateDest;
            homeView.CreateClicked += async () => await DoCreateAsync();

            homeView.FilesDroppedForCreate += paths => StageCreateFromDrop(paths);
            homeView.OpenArchiveClicked += OpenZipFileDialog;
            homeView.BrowseExtractDestinationClicked += BrowseExtractDest;
            homeView.ExtractClicked += async () => await DoExtractSmartAsync();
            homeView.ZipDroppedForExtract += path => OpenArchive(path);


            homeView.StagingListView.SmallImageList = _icons.ImageList;
            homeView.StagingListView.UseCompatibleStateImageBehavior = false;

            // Allow DnD to staging list (HomeView already raises FilesDroppedForCreate)
            homeView.StagingRemoveSelectedRequested += () => RemoveSelectedFromStaging();
            homeView.StagingClearRequested += () => { _staging.Clear(); RefreshStagingList(); };

            // Viewer events
            viewerView.AddFilesClicked += async () => await AddFilesIntoArchiveAsync();
            viewerView.AddFolderClicked += async () => await AddFolderIntoArchiveAsync();
            viewerView.BackHomeClicked += CloseArchiveAndShowHome;
            viewerView.ExtractSelectedToClicked += async () => await ExtractSelectedTo();
            viewerView.ExtractHereClicked += async () => await ExtractSelectedHere();
            viewerView.ExtractSmartClicked += async () => await ExtractSelectedSmart();
            viewerView.InfoClicked += ShowArchiveInfo;
            viewerView.TestClicked += async () => await TestArchiveAsync();
            viewerView.SettingsClicked += OpenSettings;
            viewerView.SearchTextChanged += text => RefreshRows(text);

            // ListView setup
            viewerView.lvArchive.SmallImageList = _icons.ImageList;
            viewerView.lvArchive.UseCompatibleStateImageBehavior = false;

            viewerView.lvArchive.RetrieveVirtualItem += (s, e) => e.Item = MakeItem(_rows[e.ItemIndex]);
            viewerView.lvArchive.DoubleClick += (s, e) => OpenSelected();
            viewerView.lvArchive.KeyDown += (s, e) => { if (e.KeyCode == Keys.Back) NavigateUp(); };


            mnuFileOpen.Click += (s, e) => OpenZipFileDialog();
            mnuFileExit.Click += (s, e) => Close();
            mnuToolsSettings.Click += (s, e) => OpenSettings();
            mnuHelpAbout.Click += (s, e) => { using var a = new AboutForm(); a.ShowDialog(this); };

            // Ensure session temp exists
            try { System.IO.Directory.CreateDirectory(_sessionTemp); } catch { }

            // Hook selection changed for preview
            viewerView.lvArchive.SelectedIndexChanged += async (s, e) => await PreviewSelectedAsync();

            // Toolbar extras
            viewerView.btnOpenFolder.Click += (s, e) => OpenArchiveFolderInExplorer();
            viewerView.btnTogglePreview.Click += (s, e) => TogglePreviewPane();

            // Assign ImageList for icons
     


            Load += MainForm_Load;
            FormClosing += (s, e) => { _zip?.Dispose(); };
        }



        private void OpenArchiveFolderInExplorer()
        {
            if (string.IsNullOrEmpty(_zipPath)) return;
            try { Process.Start("explorer.exe", $"/select,\"{_zipPath}\""); } catch { }
        }

        private void TogglePreviewPane()
        {
            viewerView.splitMain.Panel2Collapsed = !viewerView.splitMain.Panel2Collapsed;
        }

        // Extract one entry to temp and return path
        private async Task<string> ExtractEntryToTempAsync(ZipFileEntry e, CancellationToken ct)
        {
            string outPath = System.IO.Path.Combine(_sessionTemp, e.FilenameInZip.Replace('/', System.IO.Path.DirectorySeparatorChar));
            string? dir = System.IO.Path.GetDirectoryName(outPath);
            if (!string.IsNullOrEmpty(dir)) System.IO.Directory.CreateDirectory(dir);

            await Task.Run(() =>
            {
                void OnBytes(long n) { }
                _zip.ZipDataReader.ExtractFile(e, outPath, OnBytes);
            }, ct);

            return outPath;
        }

        private async Task PreviewSelectedAsync()
        {
            if (_zip == null) return;
            if (viewerView.lvArchive.SelectedIndices.Count == 0) { viewerView.previewPane.Clear(); return; }

            var r = _rows[viewerView.lvArchive.SelectedIndices[0]];
            if (r.Kind != RowKind.File) { viewerView.previewPane.Clear(); return; }

            _previewCts?.Cancel();
            _previewCts = new CancellationTokenSource();
            var ct = _previewCts.Token;

            try
            {
                string tempPath = await ExtractEntryToTempAsync(r.Entry, ct);
                _lastPreviewTempFile = tempPath;
                await viewerView.previewPane.ShowFileAsync(tempPath);
                if (viewerView.splitMain.Panel2Collapsed)
                    TogglePreviewPane();
            }
            catch (OperationCanceledException) { }
            catch { viewerView.previewPane.Clear(); }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            // Settings → defaults
            homeView.CreateMethodIndex = SettingsService.Current.DefaultMethod switch
            {
                "Store" => 0,
                "Zstd" => 2,
                _ => 1
            };
            homeView.CreateLevel = SettingsService.Current.DefaultLevel;
            if (SettingsService.Current.ExtractDefault == "Here")
                homeView.rdoExtractHere.Checked = true;
            else
                homeView.rdoExtractToFolderName.Checked = true;

            ShowHome();
        }

        private void ShowHome()
        {
            viewerView.Visible = false;
            homeView.Visible = true;
            Text = "Brutal Zip";
        }

        private void ShowViewer()
        {
            homeView.Visible = false;
            viewerView.Visible = true;
        }

        // Home actions
        private void AddCreateFiles()
        {
            using var ofd = new OpenFileDialog { Filter = "All files|*.*", Multiselect = true };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;
            StageCreateFromDrop(ofd.FileNames);
        }

        private void AddCreateFolder()
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) != DialogResult.OK) return;
            StageCreateFromDrop(new[] { fbd.SelectedPath });
        }

        private void BrowseCreateDest()
        {
            using var sfd = new SaveFileDialog { Filter = "Zip files|*.zip" };
            if (sfd.ShowDialog(this) != DialogResult.OK) return;
            homeView.CreateDestination = sfd.FileName;
        }

        private async void StageCreateFromDrop(IEnumerable<string> paths)
        {
            await AddToStagingAsync(paths);
        }

        private async Task AddToStagingAsync(IEnumerable<string> paths)
        {
            if (paths == null) return;

            foreach (var p in paths)
            {
                try
                {
                    var full = Path.GetFullPath(p);
                    if (!File.Exists(full) && !Directory.Exists(full)) continue;

                    if (_staging.Any(s => string.Equals(s.Path, full, StringComparison.OrdinalIgnoreCase)))
                        continue; // skip duplicates

                    var item = new StagedItem
                    {
                        Path = full,
                        IsFolder = Directory.Exists(full),
                        SizeBytes = 0,
                        ItemsCount = File.Exists(full) ? 1 : 0
                    };

                    if (!item.IsFolder && File.Exists(full))
                    {
                        try { item.SizeBytes = new FileInfo(full).Length; } catch { item.SizeBytes = 0; }
                    }

                    _staging.Add(item);
                    AddStagingRow(item); // add a row immediately

                    // For folders, compute stats asynchronously and update row
                    if (item.IsFolder)
                    {
                        _ = Task.Run(() =>
                        {
                            ComputeFolderStats(item.Path, out long bytes, out int count);
                            item.SizeBytes = bytes;
                            item.ItemsCount = count;

                            // update UI row
                            BeginInvoke(new Action(() => UpdateStagingRow(item)));
                        });
                    }
                }
                catch { /* ignore bad path */ }
            }

            UpdateStagingTotals();
        }


        private static void ComputeFolderStats(string folder, out long sizeBytes, out int fileCount)
        {
            sizeBytes = 0;
            fileCount = 0;
            try
            {
                var dirs = new Stack<string>();
                dirs.Push(folder);
                while (dirs.Count > 0)
                {
                    var d = dirs.Pop();
                    try
                    {
                        foreach (var f in Directory.EnumerateFiles(d))
                        {
                            try { sizeBytes += new FileInfo(f).Length; fileCount++; } catch { }
                        }
                        foreach (var sub in Directory.EnumerateDirectories(d))
                        {
                            dirs.Push(sub);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void AddStagingRow(StagedItem item)
        {
            var lv = homeView.StagingListView;
            string ext = item.IsFolder ? "" : Path.GetExtension(item.Path);
            int imageIndex = item.IsFolder ? _icons.FolderIndex : _icons.GetIndexForExtension(ext);

            var it = new ListViewItem(new[]
            {
    item.Name,
    item.Type,
    item.IsFolder ? "Calculating…" : FormatBytes(item.SizeBytes),
    item.IsFolder ? "…" : "1",
    item.Path
})
            {
                ImageIndex = imageIndex,
                Tag = item
            };
            lv.Items.Add(it);
            UpdateStagingTotals();
        }

        private void UpdateStagingRow(StagedItem item)
        {
            var lv = homeView.StagingListView;
            foreach (ListViewItem lvi in lv.Items)
            {
                if (lvi.Tag == item)
                {
                    lvi.SubItems[2].Text = FormatBytes(item.SizeBytes);
                    lvi.SubItems[3].Text = item.ItemsCount.ToString();
                    break;
                }
            }
            UpdateStagingTotals();
        }

        private void RefreshStagingList()
        {
            var lv = homeView.StagingListView;
            lv.BeginUpdate();
            lv.Items.Clear();
            foreach (var item in _staging)
                AddStagingRow(item);
            lv.EndUpdate();
            UpdateStagingTotals();
        }

        private void UpdateStagingTotals()
        {
            long bytes = 0;
            long files = 0;
            foreach (var s in _staging)
            {
                bytes += s.SizeBytes;
                files += s.IsFolder ? s.ItemsCount : 1;
            }
            homeView.SetStagingInfo(_staging.Count == 0
            ? "Staging: none"
            : $"Staging: {_staging.Count} item(s), {files} file(s), {FormatBytes(bytes)} total");
        }

        private void RemoveSelectedFromStaging()
        {
            var lv = homeView.StagingListView;
            if (lv.SelectedItems.Count == 0) return;

            foreach (ListViewItem lvi in lv.SelectedItems)
            {
                if (lvi.Tag is StagedItem s) _staging.Remove(s);
                lv.Items.Remove(lvi);
            }
            UpdateStagingTotals();
        }


        private void OpenZipFileDialog()
        {
            using var ofd = new OpenFileDialog { Filter = "Zip files|*.zip" };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;
            OpenArchive(ofd.FileName);
        }

        private void BrowseExtractDest()
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) != DialogResult.OK) return;
            homeView.ExtractDestination = fbd.SelectedPath;
        }

        private async System.Threading.Tasks.Task DoCreateAsync()
        {
            if (_stagedCreateItems.Count == 0)
            {
                MessageBox.Show(this, "Add or drop files/folders first.", "Nothing to do");
                return;
            }

            var dest = homeView.CreateDestination?.Trim();
            if (string.IsNullOrEmpty(dest))
            {
                using var sfd = new SaveFileDialog { Filter = "Zip files|*.zip" };
                if (sfd.ShowDialog(this) != DialogResult.OK) return;
                dest = homeView.CreateDestination = sfd.FileName;
            }

            var method = homeView.CreateMethodIndex switch
            {
                0 => Compression.Store,
                1 => Compression.Deflate,
                2 => Compression.Zstd,
                _ => Compression.Deflate
            };
            int level = homeView.CreateLevel;

            using var pf = new ProgressForm("Creating archive…");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                using var zip = new FastZipDotNet.Zip.FastZipDotNet(dest, method, level, Threads);

                foreach (var item in _staging.ToArray())
                {
                    if (pf.Token.IsCancellationRequested) break;

                    if (item.IsFolder && Directory.Exists(item.Path))
                    {
                        bool ok = await zip.ZipDataWriter.AddFilesToArchiveAsync(item.Path, Threads, level, progress, pf.Token);
                        if (!ok) break;
                    }
                    else if (!item.IsFolder && File.Exists(item.Path))
                    {
                        void OnUnc(long n) { }
                        void OnComp(long n) { }
                        zip.ZipDataWriter.AddFileWithProgress(item.Path, Path.GetFileName(item.Path), level, "", OnUnc, OnComp);
                    }
                }

                zip.Close();

                if (SettingsService.Current.OpenExplorerAfterCreate)
                    TryOpenExplorerSelect(dest);

                // clear
                _staging.Clear();
                RefreshStagingList();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Create error"); }
            finally { pf.Close(); }
        }

        private async System.Threading.Tasks.Task DoExtractSmartAsync()
        {
            if (string.IsNullOrEmpty(_zipPath))
            {
                using var ofd = new OpenFileDialog { Filter = "Zip files|*.zip" };
                if (ofd.ShowDialog(this) != DialogResult.OK) return;
                _zipPath = ofd.FileName;
            }

            string outDir;
            if (homeView.ExtractHere && !string.IsNullOrEmpty(homeView.ExtractDestination))
                outDir = homeView.ExtractDestination;
            else if (homeView.ExtractToArchiveName || SettingsService.Current.ExtractDefault == "Smart")
                outDir = Path.Combine(Path.GetDirectoryName(_zipPath) ?? ".", Path.GetFileNameWithoutExtension(_zipPath));
            else
                outDir = string.IsNullOrEmpty(homeView.ExtractDestination) ? Path.GetDirectoryName(_zipPath)! : homeView.ExtractDestination;

            Directory.CreateDirectory(outDir);

            using var zip = new FastZipDotNet.Zip.FastZipDotNet(_zipPath, Compression.Deflate, 6, Threads);
            using var pf = new ProgressForm("Extracting…");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                await zip.ExtractArchiveAsync(outDir, progress, pf.Token);
                if (SettingsService.Current.OpenExplorerAfterExtract)
                    TryOpenExplorer(outDir);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Extract error"); }
            finally { pf.Close(); }
        }

        // Viewer logic (unchanged behavior, just uses viewerView.* controls)
        private void OpenArchive(string path)
        {
            try
            {
                _zip?.Dispose();
                _zipPath = path;
                _zip = new FastZipDotNet.Zip.FastZipDotNet(path, Compression.Deflate, 6, Threads);

                if (_zip.ZipFileEntries.Any(e => e.DiskNumberStart != 0))
                {
                    MessageBox.Show(this, "Multi-part ZIP not supported.", "Not supported");
                    _zip.Dispose(); _zip = null; _zipPath = null;
                    return;
                }

                BuildTree(_zip.ZipFileEntries);
                NavigateTo(_root);
                ShowViewer();
                Text = "Brutal Zip — " + Path.GetFileName(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Open error");
                _zip?.Dispose(); _zip = null; _zipPath = null;
                ShowHome();
            }
        }

        private void CloseArchiveAndShowHome()
        {
            _zip?.Dispose(); _zip = null; _zipPath = null;
            _root = _current = null;
            _rows.Clear();
            ShowHome();
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
                var node = _root;
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
            BuildBreadcrumb();
            RefreshRows();
        }

        private void NavigateUp()
        {
            if (_current?.Parent != null) NavigateTo(_current.Parent);
        }

        private void BuildBreadcrumb()
        {
            viewerView.breadcrumb.Controls.Clear();
            var nodes = new List<DirNode>();
            var n = _current;
            while (n != null && n.Name != "")
            {
                nodes.Add(n);
                n = n.Parent;
            }
            nodes.Reverse();

            var rootLink = new LinkLabel { Text = "Root", AutoSize = true, Margin = new Padding(0, 6, 8, 0) };
            rootLink.Click += (s, e) => NavigateTo(_root);
            viewerView.breadcrumb.Controls.Add(rootLink);

            foreach (var dn in nodes)
            {
                var sep = new Label { Text = ">", AutoSize = true, Margin = new Padding(0, 6, 8, 0) };
                viewerView.breadcrumb.Controls.Add(sep);
                var lnk = new LinkLabel { Text = dn.Name, AutoSize = true, Margin = new Padding(0, 6, 8, 0) };
                var capture = dn;
                lnk.Click += (s, e) => NavigateTo(capture);
                viewerView.breadcrumb.Controls.Add(lnk);
            }
        }

        private void RefreshRows(string filter = "")
        {
            _rows.Clear();

            if (_current?.Parent != null)
                _rows.Add(Row.Up());

            var dirs = _current?.Dirs.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase) ?? Enumerable.Empty<string>();
            foreach (var d in dirs)
                _rows.Add(Row.Dir(d));

            var files = _current?.Files.OrderBy(f => f.FilenameInZip, StringComparer.OrdinalIgnoreCase) ?? Enumerable.Empty<ZipFileEntry>();
            foreach (var f in files)
            {
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    var name = Path.GetFileName(f.FilenameInZip.Replace('\\', '/'));
                    if (!name.Contains(filter, StringComparison.OrdinalIgnoreCase)) continue;
                }
                _rows.Add(Row.File(f));
            }

            viewerView.lvArchive.VirtualListSize = _rows.Count;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            long total = _current?.Files.Sum(f => (long)f.FileSize) ?? 0;
            viewerView.SetStatus($"{_rows.Count} items    Total: {FormatBytes(total)}");
        }

        private ListViewItem MakeItem(Row r)
        {
            if (r.Kind == RowKind.Up)
                return new ListViewItem(new[] { "..", "", "", "", "", "" })
                {
                    ImageIndex = _icons.FolderIndex
                };

            if (r.Kind == RowKind.Dir)
                return new ListViewItem(new[] { r.Name, "", "", "", "Folder", "" })
                {
                    ImageIndex = _icons.FolderIndex,
                    Tag = r
                };

            var f = r.Entry;
            string name = System.IO.Path.GetFileName(f.FilenameInZip.Replace('\\', '/'));
            string ext = System.IO.Path.GetExtension(name);
            int imageIndex = _icons.GetIndexForExtension(ext);

            double ratio = f.FileSize > 0 ? (double)f.CompressedSize / f.FileSize : 1.0;

            return new ListViewItem(new[]
            {
    name,
    FormatBytes((long)f.FileSize),
    FormatBytes((long)f.CompressedSize),
    (1.0 - ratio).ToString("P0"),
    f.Method.ToString(),
    f.ModifyTime.ToString("yyyy-MM-dd HH:mm:ss")
})
            {
                ImageIndex = imageIndex,
                Tag = r
            };
        }


        public async Task HandleCommandAsync(string[] args)
        {
            var cmd = Cli.Parse(args);
            switch (cmd.Type)
            {
                case Cli.CommandType.Open:
                    // Mixed: may include .zip or stage-only
                    if (cmd.Inputs != null && cmd.Inputs.Count > 0)
                    {
                        ShowHome();
                        StageCreateFromDrop(cmd.Inputs);
                        this.Activate();
                        return;
                    }
                    if (System.IO.File.Exists(cmd.Archive)) OpenArchive(cmd.Archive);
                    break;

                case Cli.CommandType.ExtractSmart:
                case Cli.CommandType.ExtractHere:
                case Cli.CommandType.ExtractTo:
                    if (!System.IO.File.Exists(cmd.Archive)) return;
                    {
                        string zipPath = cmd.Archive;
                        string dest = cmd.Type switch
                        {
                            Cli.CommandType.ExtractHere => System.IO.Path.GetDirectoryName(zipPath) ?? ".",
                            Cli.CommandType.ExtractTo => cmd.TargetDir ?? (System.IO.Path.GetDirectoryName(zipPath) ?? "."),
                            _ => System.IO.Path.Combine(System.IO.Path.GetDirectoryName(zipPath) ?? ".", System.IO.Path.GetFileNameWithoutExtension(zipPath))
                        };

                        using var zip = new FastZipDotNet.Zip.FastZipDotNet(zipPath, FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Deflate, 6, Threads);
                        using var pf = new ProgressForm("Extracting…");
                        var progress = pf.CreateProgress();
                        pf.Show(this);
                        try { await zip.ExtractArchiveAsync(dest, progress, pf.Token); }
                        catch (OperationCanceledException) { }
                        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Extract error"); }
                        finally { pf.Close(); }
                    }
                    break;

                case Cli.CommandType.Create:
                    {
                        using var pf = new ProgressForm("Creating archive…");
                        var progress = pf.CreateProgress();
                        pf.Show(this);
                        try
                        {
                            var method = SettingsService.Current.DefaultMethod == "Store" ? FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Store
                                       : SettingsService.Current.DefaultMethod == "Zstd" ? FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Zstd
                                       : FastZipDotNet.Zip.Structure.ZipEntryEnums.Compression.Deflate;
                            int level = SettingsService.Current.DefaultLevel;

                            using var zip = new FastZipDotNet.Zip.FastZipDotNet(cmd.OutArchive, method, level, Threads);
                            foreach (var p in cmd.Inputs)
                            {
                                if (pf.Token.IsCancellationRequested) break;
                                if (System.IO.Directory.Exists(p))
                                    await zip.ZipDataWriter.AddFilesToArchiveAsync(p, Threads, level, progress, pf.Token);
                                else if (System.IO.File.Exists(p))
                                {
                                    void OnUnc(long n) { }
                                    void OnComp(long n) { }
                                    zip.ZipDataWriter.AddFileWithProgress(p, System.IO.Path.GetFileName(p), level, "", OnUnc, OnComp);
                                }
                            }
                            zip.Close();
                        }
                        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Create error"); }
                        finally { pf.Close(); }
                    }
                    break;

                case Cli.CommandType.None:
                default:
                    // No-op: keep Home
                    break;
            }
        }

        private void OpenSelected()
        {
            if (viewerView.lvArchive.SelectedIndices.Count == 0) return;
            var r = _rows[viewerView.lvArchive.SelectedIndices[0]];
            if (r.Kind == RowKind.Up) { NavigateUp(); return; }
            if (r.Kind == RowKind.Dir)
            {
                if (_current.Dirs.TryGetValue(r.Name, out var d)) NavigateTo(d);
                return;
            }

            // File → extract to temp (if not already) and open
            _ = Task.Run(async () =>
            {
                try
                {
                    string path = _lastPreviewTempFile;
                    if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path))
                        path = await ExtractEntryToTempAsync(r.Entry, CancellationToken.None);
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                }
                catch { }
            });
        }


        private void OpenSettings()
        {
            using var dlg = new SettingsForm();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                // Apply changed defaults to the Home view
                homeView.CreateMethodIndex = SettingsService.Current.DefaultMethod switch
                {
                    "Store" => 0,
                    "Zstd" => 2,
                    _ => 1
                };
                homeView.CreateLevel = SettingsService.Current.DefaultLevel;
            }
        }

        private void ShowArchiveInfo()
        {
            if (_zip == null)
            {
                MessageBox.Show(this, "Open a zip first.", "Info");
                return;
            }

            var entries = _zip.ZipFileEntries;
            long totalUnc = entries.Sum(e => (long)e.FileSize);
            long totalCmp = entries.Sum(e => (long)e.CompressedSize);
            double ratio = totalUnc > 0 ? (double)totalCmp / totalUnc : 1.0;
            int files = entries.Count(e => !IsDirectory(e));
            int folders = entries.Count(e => IsDirectory(e));

            MessageBox.Show(this,
                $"Entries: {entries.Count} (Files: {files}, Folders: {folders})\n" +
                $"Total Uncompressed: {FormatBytes(totalUnc)}\n" +
                $"Total Compressed:   {FormatBytes(totalCmp)}\n" +
                $"Ratio: {(1.0 - ratio):P1} saved",
                "Archive Info");
        }

        private async Task TestArchiveAsync()
        {
            if (_zip == null)
            {
                MessageBox.Show(this, "Open a zip first.", "Info");
                return;
            }

            using var pf = new ProgressForm("Testing archive…");
            var progress = pf.CreateProgress();
            pf.Show(this);
            try
            {
                bool ok = await _zip.ZipDataReader.TestArchiveAsync(Threads, progress, pf.Token);
                MessageBox.Show(this, ok ? "Test OK" : "Test failed or cancelled");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Test error");
            }
            finally
            {
                pf.Close();
            }
        }

        private async System.Threading.Tasks.Task ExtractSelectedTo()
        {
            if (_zip == null) return;
            if (viewerView.lvArchive.SelectedIndices.Count == 0) { MessageBox.Show(this, "Select items."); return; }
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) != DialogResult.OK) return;
            await ExtractSelectionAsync(fbd.SelectedPath);
        }

        private async System.Threading.Tasks.Task ExtractSelectedHere()
        {
            if (_zip == null) return;
            string dir = Path.GetDirectoryName(_zipPath) ?? ".";
            await ExtractSelectionAsync(dir);
        }

        private async System.Threading.Tasks.Task ExtractSelectedSmart()
        {
            if (_zip == null) return;
            string dir = Path.Combine(Path.GetDirectoryName(_zipPath) ?? ".", Path.GetFileNameWithoutExtension(_zipPath));
            Directory.CreateDirectory(dir);
            await ExtractSelectionAsync(dir);
        }

        private async System.Threading.Tasks.Task ExtractSelectionAsync(string outDir)
        {
            if (_zip == null) return;
            if (viewerView.lvArchive.SelectedIndices.Count == 0) return;

            var files = new List<ZipFileEntry>();
            foreach (int idx in viewerView.lvArchive.SelectedIndices)
            {
                var r = _rows[idx];
                if (r.Kind == RowKind.File) files.Add(r.Entry);
                else if (r.Kind == RowKind.Dir && _current.Dirs.TryGetValue(r.Name, out var node)) CollectAllFiles(node, files);
            }
            if (files.Count == 0) return;

            using var pf = new ProgressForm("Extracting…");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                var sem = new System.Threading.SemaphoreSlim(Threads, Threads);
                var tasks = new List<Task>();

                long totalUnc = files.Sum(f => (long)f.FileSize);
                long processed = 0;
                var sw = Stopwatch.StartNew();

                var reportCts = new System.Threading.CancellationTokenSource();
                var reporter = System.Threading.Tasks.Task.Run(async () =>
                {
                    while (!reportCts.IsCancellationRequested)
                    {
                        long p = Math.Min(System.Threading.Interlocked.Read(ref processed), totalUnc);
                        double speed = sw.Elapsed.TotalSeconds > 0 ? p / sw.Elapsed.TotalSeconds : 0.0;
                        progress.Report(new ZipProgress
                        {
                            Operation = ZipOperation.Extract,
                            TotalFiles = files.Count,
                            TotalBytesUncompressed = totalUnc,
                            BytesProcessedUncompressed = p,
                            Elapsed = sw.Elapsed,
                            SpeedBytesPerSec = speed
                        });
                        try { await System.Threading.Tasks.Task.Delay(200, reportCts.Token); } catch { break; }
                    }
                });

                foreach (var e in files)
                {
                    await sem.WaitAsync();
                    tasks.Add(System.Threading.Tasks.Task.Run(() =>
                    {
                        try
                        {
                            string path = Path.Combine(outDir, e.FilenameInZip);
                            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                            void OnBytes(long n) => System.Threading.Interlocked.Add(ref processed, n);
                            _zip.ZipDataReader.ExtractFile(e, path, OnBytes);
                        }
                        finally { sem.Release(); }
                    }, pf.Token));
                }

                await System.Threading.Tasks.Task.WhenAll(tasks);
                reportCts.Cancel();
                try { await reporter; } catch { }

                if (SettingsService.Current.OpenExplorerAfterExtract)
                    TryOpenExplorer(outDir);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Extract error"); }
            finally { pf.Close(); }
        }

        // Shared helpers
        private string GetCurrentPathPrefix()
        {
            var parts = new List<string>();
            var n = _current;
            while (n != null && n.Name != "")
            {
                parts.Add(n.Name);
                n = n.Parent;
            }
            parts.Reverse();
            return parts.Count == 0 ? "" : string.Join("/", parts) + "/";
        }

        private void CollectAllFiles(DirNode node, List<ZipFileEntry> dest)
        {
            dest.AddRange(node.Files);
            foreach (var kv in node.Dirs) CollectAllFiles(kv.Value, dest);
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

        private static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1) { size /= 1024; unit++; }
            return $"{size:F1} {units[unit]}";
        }

        private static void TryOpenExplorer(string path)
        {
            try { Process.Start("explorer.exe", $"\"{path}\""); } catch { }
        }
        private static void TryOpenExplorerSelect(string path)
        {
            try { Process.Start("explorer.exe", $"/select,\"{path}\""); } catch { TryOpenExplorer(Path.GetDirectoryName(path) ?? "."); }
        }

        // Models & icons
        private enum RowKind { Up, Dir, File }
        private class Row
        {
            public RowKind Kind;
            public string Name;
            public ZipFileEntry Entry;
            public static Row Up() => new Row { Kind = RowKind.Up, Name = ".." };
            public static Row Dir(string name) => new Row { Kind = RowKind.Dir, Name = name };
            public static Row File(ZipFileEntry e) => new Row { Kind = RowKind.File, Entry = e };
        }

        private class DirNode
        {
            public DirNode Parent;
            public string Name;
            public Dictionary<string, DirNode> Dirs = new(StringComparer.OrdinalIgnoreCase);
            public List<ZipFileEntry> Files = new();
            public DirNode(DirNode parent, string name) { Parent = parent; Name = name; }
        }


        private async System.Threading.Tasks.Task AddFilesIntoArchiveAsync()
        {
            if (_zip == null) return;
            using var ofd = new OpenFileDialog { Filter = "All files|*.*", Multiselect = true };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;
            await AddIntoArchiveAsync(ofd.FileNames);
        }

        private async System.Threading.Tasks.Task AddFolderIntoArchiveAsync()
        {
            if (_zip == null) return;
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) != DialogResult.OK) return;
            await AddIntoArchiveAsync(new[] { fbd.SelectedPath });
        }

        private async System.Threading.Tasks.Task AddIntoArchiveAsync(IEnumerable<string> paths)
        {
            if (_zip == null) return;
            using var pf = new ProgressForm("Adding to archive…");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                foreach (var p in paths)
                {
                    if (pf.Token.IsCancellationRequested) break;

                    if (Directory.Exists(p))
                    {
                        bool ok = await _zip.ZipDataWriter.AddFilesToArchiveAsync(p, Threads, _zip.CompressionLevelValue, progress, pf.Token);
                        if (!ok) break;
                    }
                    else if (File.Exists(p))
                    {
                        string prefix = GetCurrentPathPrefix();
                        string internalName = prefix.Length == 0 ? Path.GetFileName(p) : (prefix + Path.GetFileName(p));
                        void OnUnc(long n) { }
                        void OnComp(long n) { }
                        _zip.ZipDataWriter.AddFileWithProgress(p, internalName, _zip.CompressionLevelValue, "", OnUnc, OnComp);
                    }
                }

                await System.Threading.Tasks.Task.Run(() => _zip.Close());
                OpenArchive(_zipPath);
                NavigateTo(_current);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Add error"); }
            finally { pf.Close(); }
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
