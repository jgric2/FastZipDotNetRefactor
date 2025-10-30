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


        private string _typeFilter = "";
        private System.Windows.Forms.Timer _typeFilterTimer;

        private int Threads => SettingsService.Current.ThreadsAuto
            ? Math.Max(1, Environment.ProcessorCount - 1)
            : Math.Max(1, SettingsService.Current.Threads);



        // Encryption state for Create
        private string _createPassword;
        private EncryptionAlgorithm _createAlgorithm = EncryptionAlgorithm.ZipCrypto;

        // Encryption for additions in open archive
        private bool _encryptNewAdds = false;
        private string _addPassword;
        private EncryptionAlgorithm _addAlgorithm = EncryptionAlgorithm.ZipCrypto;

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



            // HomeView encryption UI
            homeView.CreateEncryptChanged += on =>
            {
                // nothing special; UI enabled state handled in view
            };
            homeView.CreateEncryptAlgorithmChanged += idx =>
            {
                // 0 = ZipCrypto, 1 = AES-256 (soon)
                _createAlgorithm = (idx == 0) ? EncryptionAlgorithm.ZipCrypto : EncryptionAlgorithm.Aes256;
            };
            homeView.CreateSetPasswordClicked += () =>
            {
                using var dlg = new PasswordDialog();
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _createPassword = dlg.Password;
                    homeView.SetCreatePasswordStatus(!string.IsNullOrEmpty(_createPassword));
                }
            };

            // ViewerView encryption dropdown
            viewerView.EncryptNewItemsChanged += on => _encryptNewAdds = on;
            viewerView.SetAddPasswordClicked += () =>
            {
                using var dlg = new PasswordDialog();
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _addPassword = dlg.Password;
                    // we can show a small tooltip or change dropdown text, but not required
                }
            };
            viewerView.AddEncryptionAlgorithmChanged += algo => _addAlgorithm = algo;



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

            homeView.BatchZipsDroppedForExtract += async (zips, separate) => await BatchExtractAsync(zips, separate);

            homeView.QuickCreateClicked += async () => await DoQuickCreateAsync();

            homeView.StagingListView.SmallImageList = _icons.ImageList;
            homeView.StagingListView.UseCompatibleStateImageBehavior = false;
            mnuToolsFind.Click += (s, e) => DoFindInArchive();

            homeView.StagingListView.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    var data = Clipboard.GetDataObject();
                    if (data != null && data.GetDataPresent(DataFormats.FileDrop))
                    {
                        var paths = (string[])data.GetData(DataFormats.FileDrop);
                        StageCreateFromDrop(paths);
                    }
                }
            };


            homeView.StagingRemoveMissingRequested += () =>
            {
                _staging.RemoveAll(s => !(s.IsFolder ? Directory.Exists(s.Path) : File.Exists(s.Path)));
                RefreshStagingList();
            };


            int maxThreads = Math.Max(1, Environment.ProcessorCount * 2);
            int initialThreads = Threads;
            homeView.SetThreadSlider(maxThreads, initialThreads, SettingsService.Current.ThreadsAuto);

            // When slider changed on Home screen, persist and apply to active Zip
            homeView.ThreadsSliderChanged += n =>
            {
                if (!homeView.ExtractHere) { } // no-op, just example; not used
                SettingsService.Current.ThreadsAuto = false;
                SettingsService.Current.Threads = n;
                try { SettingsService.Save(); } catch { }
                try { _zip?.SetMaxConcurrency(n); } catch { }
            };

            this.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Space && viewerView.Visible)
                {
                    await PreviewSelectedAsync();
                    if (viewerView.splitMain.Panel2Collapsed) TogglePreviewPane();
                }
            };


            // auto changed
            homeView.ThreadsAutoChanged += auto =>
            {
                SettingsService.Current.ThreadsAuto = auto;
                try { SettingsService.Save(); } catch { }
                int n = auto ? Environment.ProcessorCount : Math.Max(1, SettingsService.Current.Threads);
                try { _zip?.SetMaxConcurrency(n); } catch { }
            };

            // Allow DnD to staging list (HomeView already raises FilesDroppedForCreate)
            homeView.StagingRemoveSelectedRequested += () => RemoveSelectedFromStaging();
            homeView.StagingClearRequested += () => { _staging.Clear(); RefreshStagingList(); };


            homeView.StagingListView.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete) RemoveSelectedFromStaging(); };



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

            viewerView.mnuCopyPaths.Click += (s, e) => CopySelectedPaths();
            viewerView.mnuCopyNames.Click += (s, e) => CopySelectedNames();

            viewerView.lvArchive.KeyPress += (s, e) => OnArchiveTypeFilter(e.KeyChar);


            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.Control && e.KeyCode == Keys.F) viewerView.txtSearch.Focus(); };


            // ListView setup
            viewerView.lvArchive.SmallImageList = _icons.ImageList;
            viewerView.lvArchive.UseCompatibleStateImageBehavior = false;

            viewerView.lvArchive.RetrieveVirtualItem += LvArchive_RetrieveVirtualItem;
            viewerView.lvArchive.DoubleClick += (s, e) => OpenSelected();
            viewerView.lvArchive.KeyDown += (s, e) => { if (e.KeyCode == Keys.Back) NavigateUp(); };

            viewerView.lvArchive.ItemDrag += (s, e) => BeginDragExtract();

            mnuFileOpen.Click += (s, e) => OpenZipFileDialog();
            mnuFileExit.Click += (s, e) => Close();
            mnuToolsSettings.Click += (s, e) => OpenSettings();
            mnuHelpAbout.Click += (s, e) => { using var a = new AboutForm(); a.ShowDialog(this); };


            mnuToolsOpenAfterCreate.Checked = SettingsService.Current.OpenExplorerAfterCreate;
            mnuToolsOpenAfterExtract.Checked = SettingsService.Current.OpenExplorerAfterExtract;

            mnuToolsOpenAfterCreate.CheckedChanged += (s, e) =>
            {
                SettingsService.Current.OpenExplorerAfterCreate = mnuToolsOpenAfterCreate.Checked;
                try { SettingsService.Save(); } catch { }
            };
            mnuToolsOpenAfterExtract.CheckedChanged += (s, e) =>
            {
                SettingsService.Current.OpenExplorerAfterExtract = mnuToolsOpenAfterExtract.Checked;
                try { SettingsService.Save(); } catch { }
            };


            // Ensure session temp exists
            try { System.IO.Directory.CreateDirectory(_sessionTemp); } catch { }

            // Hook selection changed for preview
            viewerView.lvArchive.SelectedIndexChanged += async (s, e) => await PreviewSelectedAsync();

            // Toolbar extras
            viewerView.btnOpenFolder.Click += (s, e) => OpenArchiveFolderInExplorer();
            viewerView.btnTogglePreview.Click += (s, e) => TogglePreviewPane();

            // Assign ImageList for icons
            RebuildRecentMenu();


            Load += MainForm_Load;
            FormClosing += (s, e) => 
            {
                viewerView.lvArchive.RetrieveVirtualItem -= LvArchive_RetrieveVirtualItem;
                _zip?.Dispose(); 
            };
        }

        private readonly ListViewItem _emptyItem =
new ListViewItem(new[] { "", "", "", "", "", "" });

        private void LvArchive_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            // This event can fire while _rows is being swapped/cleared
            // or after VirtualListSize changed.
            try
            {
                if (_rows == null || e.ItemIndex < 0)
                {
                    e.Item = _emptyItem;
                    return;
                }

                int count = _rows.Count;
                if (e.ItemIndex >= count)
                {
                    e.Item = _emptyItem;
                    return;
                }

                var r = _rows[e.ItemIndex];
                var it = MakeItem(r);
                e.Item = it ?? _emptyItem;
            }
            catch
            {
                // As a last resort, don’t throw from the event
                e.Item = _emptyItem;
            }
        }

        private bool HasSingleRoot(out string rootName)
        {
            rootName = null;
            if (_zip?.ZipFileEntries == null || _zip.ZipFileEntries.Count == 0) return false;

            var names = _zip.ZipFileEntries
                .Select(e => (e.FilenameInZip ?? string.Empty).Replace('\\', '/').Trim('/'))
                .Where(n => n.Length > 0)
                .ToList();

            if (names.Count == 0) return false;

            var roots = names.Select(n => n.Split('/')[0]).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (roots.Count != 1) return false;

            rootName = roots[0];

            // If there is any file at root, not a single-folder archive
            bool anyFileAtRoot = _zip.ZipFileEntries.Any(e =>
            {
                var n = (e.FilenameInZip ?? "").Replace('\\', '/').Trim('/');
                return n.Length > 0 && !n.Contains("/") && !IsDirectory(e);
            });
            return !anyFileAtRoot;
        }


        private void CopySelectedPaths()
        {
            if (_zip == null) return;
            if (viewerView.lvArchive.SelectedIndices.Count == 0) return;

            var lines = new List<string>();
            foreach (int idx in viewerView.lvArchive.SelectedIndices)
            {
                var r = _rows[idx];
                if (r.Kind == RowKind.File) lines.Add(r.Entry.FilenameInZip);
                else if (r.Kind == RowKind.Dir) lines.Add(r.Name + "/");
            }
            if (lines.Count > 0) Clipboard.SetText(string.Join(Environment.NewLine, lines));
        }

        private void CopySelectedNames()
        {
            if (_zip == null) return;
            if (viewerView.lvArchive.SelectedIndices.Count == 0) return;

            var lines = new List<string>();
            foreach (int idx in viewerView.lvArchive.SelectedIndices)
            {
                var r = _rows[idx];
                if (r.Kind == RowKind.File) lines.Add(Path.GetFileName(r.Entry.FilenameInZip.Replace('\\', '/')));
                else if (r.Kind == RowKind.Dir) lines.Add(r.Name);
            }
            if (lines.Count > 0) Clipboard.SetText(string.Join(Environment.NewLine, lines));
        }


        private void RebuildRecentMenu()
        {
            if (mnuFileRecent == null) return;

            mnuFileRecent.DropDownItems.Clear();

            var recents = RecentManager.GetList();
            if (recents.Count == 0)
            {
                var none = new ToolStripMenuItem("(none)") { Enabled = false };
                mnuFileRecent.DropDownItems.Add(none);
            }
            else
            {
                // Build items (limit to max in settings)
                int max = Math.Max(1, SettingsService.Current.RecentMax);
                foreach (var path in recents.Take(max))
                {
                    string label = Path.GetFileName(path);
                    if (string.IsNullOrEmpty(label)) label = path;

                    bool exists = File.Exists(path);
                    var item = new ToolStripMenuItem(label)
                    {
                        Tag = path,
                        ToolTipText = path,
                        Enabled = true
                    };
                    if (!exists)
                    {
                        item.ForeColor = Color.Gray;
                        item.ToolTipText = path + " (missing)";
                    }

                    item.Click += (s, e) =>
                    {
                        string p = (string)((ToolStripMenuItem)s).Tag;
                        if (File.Exists(p)) OpenArchive(p);
                        else
                        {
                            var r = MessageBox.Show(this, $"File not found:\n{p}\n\nRemove from recent list?", "Missing",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (r == DialogResult.Yes)
                            {
                                RecentManager.Remove(p);
                                RebuildRecentMenu();
                            }
                        }
                    };

                    mnuFileRecent.DropDownItems.Add(item);
                }
            }

            // Separator + Clear
            mnuFileRecent.DropDownItems.Add(new ToolStripSeparator());
            var clear = new ToolStripMenuItem("Clear list");
            clear.Click += (s, e) => { RecentManager.Clear(); RebuildRecentMenu(); };
            mnuFileRecent.DropDownItems.Add(clear);
        }

        private void OnArchiveTypeFilter(char ch)
        {
            if (!char.IsControl(ch))
            {
                _typeFilter += ch;
                RefreshRows(_typeFilter);
                if (_typeFilterTimer == null)
                {
                    _typeFilterTimer = new System.Windows.Forms.Timer();
                    _typeFilterTimer.Interval = 1200;
                    _typeFilterTimer.Tick += (s, e) =>
                    {
                        _typeFilter = "";
                        _typeFilterTimer.Stop();
                    };
                }
                _typeFilterTimer.Stop();
                _typeFilterTimer.Start();
            }
        }


        private void BeginDragExtract()
        {
            if (_zip == null || viewerView.lvArchive.SelectedIndices.Count == 0) return;

            var files = new List<string>();
            foreach (int idx in viewerView.lvArchive.SelectedIndices)
            {
                var r = _rows[idx];
                if (r.Kind == RowKind.File) files.Add(r.Entry.FilenameInZip);
            }
            if (files.Count == 0) return;

            // Extract selected to session temp
            var extracted = new List<string>();
            foreach (var pathInZip in files)
            {
                var entry = _current.Files.FirstOrDefault(f => f.FilenameInZip.Equals(pathInZip, StringComparison.OrdinalIgnoreCase));
                if (entry.FilenameInZip == null) continue;
                string temp = ExtractEntryToTempAsync(entry, CancellationToken.None).GetAwaiter().GetResult();
                extracted.Add(temp);
            }

            var data = new DataObject(DataFormats.FileDrop, extracted.ToArray());
            viewerView.lvArchive.DoDragDrop(data, DragDropEffects.Copy);
        }


        private async Task BatchExtractAsync(List<string> zips, bool separateFolders)
        {
            using var pf = new ProgressForm("Extracting archives…");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                if (!separateFolders)
                {
                    using var fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog(this) != DialogResult.OK) return;
                    string destAll = fbd.SelectedPath;

                    foreach (var z in zips)
                    {
                        using var zip = new FastZipDotNet.Zip.FastZipDotNet(z, Compression.Deflate, 6, Threads);
                        await zip.ExtractArchiveAsync(destAll, progress, pf.Token);
                    }
                }
                else
                {
                    foreach (var z in zips)
                    {
                        string dest = Path.Combine(Path.GetDirectoryName(z) ?? ".", Path.GetFileNameWithoutExtension(z));
                        Directory.CreateDirectory(dest);
                        using var zip = new FastZipDotNet.Zip.FastZipDotNet(z, Compression.Deflate, 6, Threads);
                        await zip.ExtractArchiveAsync(dest, progress, pf.Token);
                    }
                }

                if (SettingsService.Current.OpenExplorerAfterExtract)
                    TryOpenExplorer(Path.GetDirectoryName(zips[0]) ?? ".");
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Extract error"); }
            finally { pf.Close(); }
        }


        private async Task DoQuickCreateAsync()
        {
            if (_staging.Count == 0) { MessageBox.Show(this, "Add files or folders first."); return; }

            // Auto-destination: next to first staged item
            var first = _staging[0];
            string baseDir;
            string baseName;

            try
            {
                if (first.IsFolder)
                {
                    var di = new DirectoryInfo(first.Path);
                    baseDir = di.Parent?.FullName ?? di.FullName;
                    baseName = di.Name;
                }
                else
                {
                    var fi = new FileInfo(first.Path);
                    baseDir = fi.DirectoryName ?? ".";
                    baseName = Path.GetFileNameWithoutExtension(fi.Name);
                }
            }
            catch
            {
                baseDir = Path.GetDirectoryName(first.Path) ?? ".";
                baseName = first.IsFolder ? new DirectoryInfo(first.Path).Name : Path.GetFileNameWithoutExtension(first.Path);
            }

            string dest = Path.Combine(baseDir, baseName + ".zip");

            homeView.CreateDestination = dest;

            // Respect current defaults (method/level come from HomeView)
            // Encryption UI is used as-is. If encryption is enabled but no password, prompt now.
            if (homeView.CreateEncryptEnabled && string.IsNullOrEmpty(_createPassword))
            {
                using var dlg = new PasswordDialog();
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                _createPassword = dlg.Password;
                homeView.SetCreatePasswordStatus(!string.IsNullOrEmpty(_createPassword));
            }

            await DoCreateAsync();
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
            if (!EnsurePasswordForEncryptedIfNeeded()) return;


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
            // Stop the list from asking for stale indices
            viewerView.lvArchive.BeginUpdate();
            viewerView.lvArchive.VirtualListSize = 0;
            viewerView.lvArchive.EndUpdate();

            _rows.Clear();

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

            // If missing, paint gray
            bool exists = item.IsFolder ? Directory.Exists(item.Path) : File.Exists(item.Path);
            if (!exists) { it.ForeColor = Color.Gray; }

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

        private async Task DoCreateAsync()
        {
            if (_staging == null || _staging.Count == 0)
            {
                MessageBox.Show(this, "Add files or folders to the staging list first.", "Nothing to do");
                return;
            }

            // Destination
            var dest = homeView.CreateDestination?.Trim();
            if (string.IsNullOrEmpty(dest))
            {
                using var sfd = new SaveFileDialog { Filter = "Zip files|*.zip" };
                if (sfd.ShowDialog(this) != DialogResult.OK) return;
                dest = homeView.CreateDestination = sfd.FileName;
            }

            // Method/level
            var method = homeView.CreateMethodIndex switch
            {
                0 => Compression.Store,
                1 => Compression.Deflate,
                2 => Compression.Zstd,
                _ => Compression.Deflate
            };
            int level = homeView.CreateLevel;

            // Encryption choices
            bool encOn = homeView.CreateEncryptEnabled;
            EncryptionAlgorithm encAlgo = (homeView.CreateEncryptAlgorithmIndex == 0)
                ? EncryptionAlgorithm.ZipCrypto
                : EncryptionAlgorithm.Aes256; // AES to be enabled in next step

            if (encOn && string.IsNullOrEmpty(_createPassword))
            {
                using var dlg = new PasswordDialog();
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                _createPassword = dlg.Password;
                homeView.SetCreatePasswordStatus(!string.IsNullOrEmpty(_createPassword));
            }

            // Build jobs: files to add and empty folders to preserve
            var jobs = new List<(string fullPath, string internalName, long size)>();
            var emptyFolderEntries = new List<string>();

            foreach (var s in _staging)
            {
                try
                {
                    if (s.IsFolder && Directory.Exists(s.Path))
                    {
                        string rootName = new DirectoryInfo(s.Path).Name;

                        // Enumerate all subdirectories relative to s.Path
                        var allDirsRel = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        try
                        {
                            foreach (var dir in Directory.EnumerateDirectories(s.Path, "*", SearchOption.AllDirectories))
                            {
                                string rel = Path.GetRelativePath(s.Path, dir).Replace('\\', '/').Trim('/');
                                if (!string.IsNullOrEmpty(rel))
                                    allDirsRel.Add(rel);
                            }
                        }
                        catch { }

                        // Track directories that contain at least one file inside
                        var nonEmptyDirsRel = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        int filesInThisRoot = 0;

                        // Enumerate files
                        try
                        {
                            foreach (var file in Directory.EnumerateFiles(s.Path, "*", SearchOption.AllDirectories))
                            {
                                string rel = Path.GetRelativePath(s.Path, file).Replace('\\', '/').Trim('/');
                                string internalName = (rootName + "/" + rel).Replace('\\', '/').Trim('/');

                                long size = 0; try { size = new FileInfo(file).Length; } catch { }
                                jobs.Add((file, internalName, size));
                                filesInThisRoot++;

                                // mark ancestors of rel
                                var dirRel = Path.GetDirectoryName(rel)?.Replace('\\', '/');
                                while (!string.IsNullOrEmpty(dirRel))
                                {
                                    nonEmptyDirsRel.Add(dirRel);
                                    dirRel = Path.GetDirectoryName(dirRel)?.Replace('\\', '/');
                                }
                            }
                        }
                        catch { }

                        // Empty directories to preserve
                        foreach (var dRel in allDirsRel)
                        {
                            if (!nonEmptyDirsRel.Contains(dRel))
                            {
                                string internalFolder = (rootName + "/" + dRel).Replace('\\', '/').Trim('/');
                                emptyFolderEntries.Add(internalFolder);
                            }
                        }

                        // If the folder is completely empty (no files, no subdirs)
                        if (filesInThisRoot == 0 && allDirsRel.Count == 0)
                        {
                            emptyFolderEntries.Add(rootName);
                        }
                    }
                    else if (!s.IsFolder && File.Exists(s.Path))
                    {
                        string internalName = Path.GetFileName(s.Path);
                        long size = 0; try { size = new FileInfo(s.Path).Length; } catch { }
                        jobs.Add((s.Path, internalName, size));
                    }
                }
                catch { /* ignore bad path */ }
            }

            if (jobs.Count == 0 && emptyFolderEntries.Count == 0)
            {
                MessageBox.Show(this, "No valid files or folders found.", "Create");
                return;
            }

            long grandBytes = 0; int grandFiles = 0;
            foreach (var j in jobs) { grandBytes += j.size; grandFiles++; }

            int maxThreads = Math.Max(1, Environment.ProcessorCount * 2);
            int curThreads = Threads;

            using var pf = new ProgressForm($"Creating archive… ({curThreads} threads)");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                using var zip = new FastZipDotNet.Zip.FastZipDotNet(dest, method, level, curThreads);

                if (encOn)
                {
                    zip.Encryption = encAlgo;         // ZipCrypto now; AES soon
                    zip.Password = _createPassword;   // required
                }
                else
                {
                    zip.Encryption = EncryptionAlgorithm.None;
                    zip.Password = null;
                }

                // Live thread control
                pf.ConfigureThreads(maxThreads, curThreads, n =>
                {
                    try
                    {
                        SettingsService.Current.ThreadsAuto = false;
                        SettingsService.Current.Threads = n;
                        SettingsService.Save();
                        zip.SetMaxConcurrency(n);
                        pf.Text = $"Creating archive… ({n} threads)";
                    }
                    catch { }
                });

                var sem = zip.ConcurrencyLimiter;

                long aggBytes = 0;
                int aggFiles = 0;
                string currentName = null;
                var sw = Stopwatch.StartNew();

                using var reportCts = new CancellationTokenSource();
                var reporter = Task.Run(async () =>
                {
                    while (!reportCts.IsCancellationRequested)
                    {
                        long pBytes = Math.Min(Interlocked.Read(ref aggBytes), grandBytes);
                        int pFiles = Math.Min(Volatile.Read(ref aggFiles), grandFiles);
                        double speed = sw.Elapsed.TotalSeconds > 0 ? pBytes / sw.Elapsed.TotalSeconds : 0.0;

                        progress.Report(new ZipProgress
                        {
                            Operation = ZipOperation.Build,
                            CurrentFile = Volatile.Read(ref currentName),
                            TotalFiles = grandFiles,
                            TotalBytesUncompressed = grandBytes,
                            FilesProcessed = pFiles,
                            BytesProcessedUncompressed = pBytes,
                            Elapsed = sw.Elapsed,
                            SpeedBytesPerSec = speed
                        });

                        try { await Task.Delay(200, reportCts.Token); } catch { break; }
                    }
                });

                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(pf.Token);
                var tasks = new List<Task>(jobs.Count);

                foreach (var job in jobs)
                {
                    linkedCts.Token.ThrowIfCancellationRequested();

                    tasks.Add(Task.Run(() =>
                    {
                        sem.WaitOne(linkedCts.Token);
                        try
                        {
                            Volatile.Write(ref currentName, job.internalName);

                            void OnUnc(long n) => Interlocked.Add(ref aggBytes, n);
                            void OnComp(long n) { /* compressed count not needed for UI here */ }

                            zip.ZipDataWriter.AddFileWithProgress(job.fullPath, job.internalName, level, "", OnUnc, OnComp);
                            Interlocked.Increment(ref aggFiles);
                        }
                        finally
                        {
                            sem.Release();
                        }
                    }, linkedCts.Token));
                }

                await Task.WhenAll(tasks);
                reportCts.Cancel();
                try { await reporter; } catch { }

                // Preserve empty folders (sequential)
                foreach (var folder in emptyFolderEntries.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    try { zip.ZipDataWriter.AddEmptyFolder(folder); } catch { }
                }

                zip.Close();

                if (SettingsService.Current.OpenExplorerAfterCreate)
                    TryOpenExplorerSelect(dest);

                _staging.Clear();
                RefreshStagingList();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Create error"); }
            finally { pf.Close(); }
        }

        private async Task DoExtractSmartAsync()
        {
            // Pick archive if none
            if (string.IsNullOrEmpty(_zipPath))
            {
                using var ofd = new OpenFileDialog { Filter = "Zip files|*.zip" };
                if (ofd.ShowDialog(this) != DialogResult.OK) return;
                _zipPath = ofd.FileName;
            }

            // Destination policy with single-root heuristic
            string outDir;
            bool singleRoot = HasSingleRoot(out var root);
            if (singleRoot)
            {
                // If Smart chosen, extract directly next to the archive and keep the single root as folder
                string baseDir = Path.GetDirectoryName(_zipPath) ?? ".";
                outDir = Path.Combine(baseDir, root);
            }
            else if (homeView.ExtractHere && !string.IsNullOrEmpty(homeView.ExtractDestination))
            {
                outDir = homeView.ExtractDestination;
            }
            else if (homeView.ExtractToArchiveName || SettingsService.Current.ExtractDefault == "Smart")
            {
                outDir = Path.Combine(Path.GetDirectoryName(_zipPath) ?? ".", Path.GetFileNameWithoutExtension(_zipPath));
            }
            else
            {
                outDir = string.IsNullOrEmpty(homeView.ExtractDestination) ? (Path.GetDirectoryName(_zipPath) ?? ".") : homeView.ExtractDestination;
            }

            Directory.CreateDirectory(outDir);

            int maxThreads = Math.Max(1, Environment.ProcessorCount * 2);
            int curThreads = Threads;

            using var zip = new FastZipDotNet.Zip.FastZipDotNet(_zipPath, Compression.Deflate, 6, curThreads);
            using var pf = new ProgressForm($"Extracting… ({curThreads} threads)");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                pf.ConfigureThreads(maxThreads, curThreads, n =>
                {
                    try
                    {
                        SettingsService.Current.ThreadsAuto = false;
                        SettingsService.Current.Threads = n;
                        SettingsService.Save();
                        zip.SetMaxConcurrency(n);
                        pf.Text = $"Extracting… ({n} threads)";
                    }
                    catch { }
                });

                await zip.ExtractArchiveAsync(outDir, progress, pf.Token);
                if (SettingsService.Current.OpenExplorerAfterExtract)
                    TryOpenExplorer(outDir);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Extract error"); }
            finally { pf.Close(); }
        }

        private void DoFindInArchive()
        {
            if (_zip == null) { MessageBox.Show(this, "Open a zip first.", "Find"); return; }

            using var dlg = new FindDialog();
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var rx = FindDialog.BuildRegex(dlg.Pattern, dlg.MatchCase);

            // Filter current directory entries; optionally search entire archive:
            var matches = new List<int>();
            for (int i = 0; i < _rows.Count; i++)
            {
                var r = _rows[i];
                if (r.Kind == RowKind.File)
                {
                    string name = Path.GetFileName(r.Entry.FilenameInZip.Replace('\\', '/'));
                    if (rx.IsMatch(name)) matches.Add(i);
                }
                else if (r.Kind == RowKind.Dir)
                {
                    if (rx.IsMatch(r.Name)) matches.Add(i);
                }
            }

            if (matches.Count == 0)
            {
                MessageBox.Show(this, "No matches.", "Find");
                return;
            }

            viewerView.lvArchive.SelectedIndices.Clear();
            foreach (var idx in matches)
                viewerView.lvArchive.SelectedIndices.Add(idx);

            viewerView.lvArchive.EnsureVisible(matches[0]);
            viewerView.lvArchive.Focus();
        }


        // Viewer logic (unchanged behavior, just uses viewerView.* controls)
        private void OpenArchive(string path)
        {
            try
            {
                // Stop the list from asking for stale indices
                viewerView.lvArchive.BeginUpdate();
                viewerView.lvArchive.VirtualListSize = 0;
                viewerView.lvArchive.EndUpdate();
                _rows.Clear();

                _zip?.Dispose();
                _zipPath = path;

                // Open in read/write to allow adding later (writer will use random access)
                _zip = new FastZipDotNet.Zip.FastZipDotNet(path, Compression.Deflate, 6, Threads);

                // We do not support multi-part yet
                if (_zip.ZipFileEntries.Any(e => e.DiskNumberStart != 0))
                {
                    MessageBox.Show(this, "Multi-part ZIP not supported.", "Not supported");
                    _zip.Dispose(); _zip = null; _zipPath = null;
                    return;
                }

                // Build tree & show viewer
                BuildTree(_zip.ZipFileEntries);
                NavigateTo(_root);
                ShowViewer();
                Text = "Brutal Zip — " + Path.GetFileName(path);

                // Recent list
                RecentManager.Add(path);
                RebuildRecentMenu();

                // Encryption defaults for adding new files
                bool anyEncrypted = _zip.ZipFileEntries.Any(e => e.IsEncrypted);
                _encryptNewAdds = anyEncrypted;
                if (viewerView.mnuEncryptNew != null)
                    viewerView.mnuEncryptNew.Checked = anyEncrypted;

                // Decide default algorithm based on entries; AES not active yet, but detect it
                _addAlgorithm = _zip.ZipFileEntries.Any(e => e.IsAes)
                    ? EncryptionAlgorithm.Aes256   // will be enabled in next AES step
                    : EncryptionAlgorithm.ZipCrypto;

                // If the archive has encrypted entries and we plan to add to it,
                // capture (or prompt for) the password now and reuse for additions.
                if (anyEncrypted)
                {
                    if (!EnsurePasswordForEncryptedIfNeeded())
                    {
                        // User cancelled; we still opened the archive for browsing
                    }
                    _addPassword = _zip.Password; // reuse for future additions
                }
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

            viewerView.lvArchive.BeginUpdate();
            viewerView.lvArchive.VirtualListSize = 0;
            viewerView.lvArchive.EndUpdate();

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

            // Sync the virtual list size with the built _rows
            viewerView.lvArchive.BeginUpdate();
            viewerView.lvArchive.VirtualListSize = _rows.Count;
            viewerView.lvArchive.EndUpdate();

            UpdateStatus();
        }

        private void UpdateStatus()
        {
            long total = _current?.Files.Sum(f => (long)f.FileSize) ?? 0;
            viewerView.SetStatus($"{_rows.Count} items    Total: {FormatBytes(total)}");
        }

        private ListViewItem MakeItem(Row r)
        {
            if (r == null) return _emptyItem;

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

            if (!EnsurePasswordForEncryptedIfNeeded()) return;

            int maxThreads = Math.Max(1, Environment.ProcessorCount * 2);
            int curThreads = Threads;

            // Precompute totals for stable progress
            int totalFiles = 0;
            long totalBytes = 0;
            try
            {
                totalFiles = _zip.ZipFileEntries?.Count ?? 0;
                totalBytes = _zip.ZipFileEntries?.Sum(e => (long)e.FileSize) ?? 0;
            }
            catch
            {
                // Totals optional; engine will still report
            }

            using var pf = new ProgressForm($"Testing archive… ({curThreads} threads)");
            var progressUI = pf.CreateProgress();
            pf.Show(this);

            try
            {
                // Live thread control
                pf.ConfigureThreads(maxThreads, curThreads, n =>
                {
                    try
                    {
                        SettingsService.Current.ThreadsAuto = false;
                        SettingsService.Current.Threads = n;
                        SettingsService.Save();

                        // Engine uses AdjustableSemaphore; SetMaxConcurrency takes effect mid-run
                        _zip.SetMaxConcurrency(n);

                        pf.Text = $"Testing archive… ({n} threads)";
                    }
                    catch { }
                });

                // Aggregator that forwards engine progress to the UI,
                // filling in totals and smoothing elapsed/speed if needed.
                var sw = Stopwatch.StartNew();
                long lastBytes = 0;
                int lastFiles = 0;

                var aggregator = new Progress<ZipProgress>(p =>
                {
                    // Compose a UI-friendly progress payload.
                    // Use engine values when present; fall back to our precomputed totals if needed.
                    long bytesProcessed = Math.Max(p.BytesProcessedUncompressed, lastBytes);
                    int filesProcessed = Math.Max(p.FilesProcessed, lastFiles);

                    var ui = new ZipProgress
                    {
                        Operation = ZipOperation.Test,
                        CurrentFile = p.CurrentFile,
                        TotalFiles = (totalFiles > 0) ? totalFiles : p.TotalFiles,
                        TotalBytesUncompressed = (totalBytes > 0) ? totalBytes : p.TotalBytesUncompressed,
                        FilesProcessed = filesProcessed,
                        BytesProcessedUncompressed = bytesProcessed,
                        // Use engine elapsed if available; else use our stopwatch
                        Elapsed = (p.Elapsed != TimeSpan.Zero) ? p.Elapsed : sw.Elapsed,
                        // Use engine speed if provided, else derive it
                        SpeedBytesPerSec = (p.SpeedBytesPerSec > 0)
                            ? p.SpeedBytesPerSec
                            : (sw.Elapsed.TotalSeconds > 0 ? bytesProcessed / sw.Elapsed.TotalSeconds : 0.0)
                    };

                    // Persist last values to ensure non-decreasing UI counters
                    lastBytes = ui.BytesProcessedUncompressed;
                    lastFiles = ui.FilesProcessed;

                    progressUI.Report(ui);
                });

                // Call the engine test; engine will do its own multi-threaded decompression/CRC.
                bool ok = await _zip.ZipDataReader.TestArchiveAsync(Threads, aggregator, pf.Token);

                MessageBox.Show(this, ok ? "Test completed successfully." : "Test failed or cancelled.", "Test");
            }
            catch (OperationCanceledException) { } // user pressed Cancel
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Test error"); }
            finally { pf.Close(); }
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

        private async Task ExtractSelectionAsync(string outDir)
        {
            if (_zip == null) return;
            if (viewerView.lvArchive.SelectedIndices.Count == 0) return;
            if (!EnsurePasswordForEncryptedIfNeeded()) return;

            var files = new List<ZipFileEntry>();
            foreach (int idx in viewerView.lvArchive.SelectedIndices)
            {
                var r = _rows[idx];
                if (r.Kind == RowKind.File) files.Add(r.Entry);
                else if (r.Kind == RowKind.Dir && _current.Dirs.TryGetValue(r.Name, out var node)) CollectAllFiles(node, files);
            }
            if (files.Count == 0) return;

            int maxThreads = Math.Max(1, Environment.ProcessorCount * 2);
            int curThreads = Threads;

            using var pf = new ProgressForm($"Extracting… ({curThreads} threads)");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                // Live engine thread control (affects decompression workers inside engine)
                pf.ConfigureThreads(maxThreads, curThreads, n =>
                {
                    try
                    {
                        SettingsService.Current.ThreadsAuto = false;
                        SettingsService.Current.Threads = n;
                        SettingsService.Save();
                        _zip.SetMaxConcurrency(n);
                        pf.Text = $"Extracting… ({n} threads)";
                    }
                    catch { }
                });

                // Our per-file concurrency (fixed at start)
                var sem = new System.Threading.SemaphoreSlim(curThreads, curThreads);
                var tasks = new List<Task>();

                int totalFiles = files.Count;
                int filesDone = 0;                        // NEW: track processed files
                long totalUnc = files.Sum(f => (long)f.FileSize);
                long processed = 0;
                string currentName = null;

                var sw = Stopwatch.StartNew();

                var reportCts = new System.Threading.CancellationTokenSource();
                var reporter = Task.Run(async () =>
                {
                    while (!reportCts.IsCancellationRequested)
                    {
                        long p = Math.Min(Interlocked.Read(ref processed), totalUnc);
                        int fd = Volatile.Read(ref filesDone);
                        double speed = sw.Elapsed.TotalSeconds > 0 ? p / sw.Elapsed.TotalSeconds : 0.0;

                        progress.Report(new ZipProgress
                        {
                            Operation = ZipOperation.Extract,
                            CurrentFile = Volatile.Read(ref currentName),
                            TotalFiles = totalFiles,
                            TotalBytesUncompressed = totalUnc,
                            BytesProcessedUncompressed = p,
                            FilesProcessed = fd,                            // NEW
                            Elapsed = sw.Elapsed,
                            SpeedBytesPerSec = speed
                        });

                        try { await Task.Delay(200, reportCts.Token); } catch { break; }
                    }
                });

                foreach (var e in files)
                {
                    await sem.WaitAsync(pf.Token);
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            string path = Path.Combine(outDir, e.FilenameInZip);
                            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                            // Update “current file” and bytes callback
                            Volatile.Write(ref currentName, e.FilenameInZip);
                            void OnBytes(long n) => Interlocked.Add(ref processed, n);

                            // Extract
                            _zip.ZipDataReader.ExtractFile(e, path, OnBytes);

                            // Count 1 file done
                            Interlocked.Increment(ref filesDone);          // NEW
                        }
                        finally { sem.Release(); }
                    }, pf.Token));
                }

                await Task.WhenAll(tasks);
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

        private async Task AddIntoArchiveAsync(IEnumerable<string> paths)
        {
            if (_zip == null) return;
            var list = paths?.ToList();
            if (list == null || list.Count == 0) return;

            // Inside-archive destination (breadcrumb path)
            string currentPrefix = GetCurrentPathPrefix().Trim('/');

            // Build jobs and record empty dirs to preserve
            var jobs = new List<(string fullPath, string internalName, long size)>();
            var emptyFolderEntries = new List<string>();

            foreach (var p in list)
            {
                try
                {
                    var full = Path.GetFullPath(p);

                    if (Directory.Exists(full))
                    {
                        var di = new DirectoryInfo(full);
                        string rootName = di.Name;
                        string basePrefix = string.IsNullOrEmpty(currentPrefix) ? rootName : (currentPrefix + "/" + rootName);

                        // All subdirectories relative to root
                        var allDirsRel = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        try
                        {
                            foreach (var dir in Directory.EnumerateDirectories(full, "*", SearchOption.AllDirectories))
                            {
                                string rel = Path.GetRelativePath(full, dir).Replace('\\', '/').Trim('/');
                                if (!string.IsNullOrEmpty(rel))
                                    allDirsRel.Add(rel);
                            }
                        }
                        catch { }

                        // Track directories that contain at least one file
                        var nonEmptyDirsRel = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        int filesInRoot = 0;

                        // Files under this folder
                        try
                        {
                            foreach (var file in Directory.EnumerateFiles(full, "*", SearchOption.AllDirectories))
                            {
                                string rel = Path.GetRelativePath(full, file).Replace('\\', '/').Trim('/');
                                string internalName = (basePrefix + "/" + rel).Replace('\\', '/').Trim('/');

                                long size = 0; try { size = new FileInfo(file).Length; } catch { }
                                jobs.Add((file, internalName, size));
                                filesInRoot++;

                                // Mark ancestors of rel
                                var dirRel = Path.GetDirectoryName(rel)?.Replace('\\', '/');
                                while (!string.IsNullOrEmpty(dirRel))
                                {
                                    nonEmptyDirsRel.Add(dirRel);
                                    dirRel = Path.GetDirectoryName(dirRel)?.Replace('\\', '/');
                                }
                            }
                        }
                        catch { }

                        // Empty directories to preserve
                        foreach (var dRel in allDirsRel)
                        {
                            if (!nonEmptyDirsRel.Contains(dRel))
                            {
                                string internalFolder = (basePrefix + "/" + dRel).Replace('\\', '/').Trim('/');
                                emptyFolderEntries.Add(internalFolder);
                            }
                        }

                        // If the folder is completely empty
                        if (filesInRoot == 0 && allDirsRel.Count == 0)
                        {
                            emptyFolderEntries.Add(basePrefix);
                        }
                    }
                    else if (File.Exists(full))
                    {
                        string name = Path.GetFileName(full);
                        string internalName = string.IsNullOrEmpty(currentPrefix)
                            ? name
                            : (currentPrefix + "/" + name).Replace('\\', '/').Trim('/');

                        long size = 0; try { size = new FileInfo(full).Length; } catch { }
                        jobs.Add((full, internalName, size));
                    }
                }
                catch
                {
                    // Ignore bad path and continue
                }
            }

            if (jobs.Count == 0 && emptyFolderEntries.Count == 0) return;

            // Apply encryption preference for additions
            if (_encryptNewAdds)
            {
                // Try reuse archive’s known password for additions
                if (string.IsNullOrEmpty(_addPassword))
                {
                    if (!string.IsNullOrEmpty(_zip.Password))
                    {
                        _addPassword = _zip.Password;
                    }
                    else
                    {
                        using var dlg = new PasswordDialog();
                        if (dlg.ShowDialog(this) != DialogResult.OK) return;
                        _addPassword = dlg.Password;
                        _zip.Password = _addPassword; // also keep on the open zip for future reads
                    }
                }

                // For now only ZipCrypto is implemented; AES will be enabled in next step.
                var effectiveAlgo = (_addAlgorithm == EncryptionAlgorithm.ZipCrypto)
                    ? EncryptionAlgorithm.ZipCrypto
                    : EncryptionAlgorithm.ZipCrypto; // clamp to ZipCrypto until AES step

                _zip.Encryption = effectiveAlgo;
                _zip.Password = _addPassword;
            }
            else
            {
                // Explicitly disable encryption for these additions
                _zip.Encryption = EncryptionAlgorithm.None;
                // Don't clear _zip.Password here; reader may still need it for other operations
            }

            long grandBytes = 0; int grandFiles = 0;
            foreach (var j in jobs) { grandBytes += j.size; grandFiles++; }

            int maxThreads = Math.Max(1, Environment.ProcessorCount * 2);
            int curThreads = Threads;

            using var pf = new ProgressForm($"Adding to archive… ({curThreads} threads)");
            var progress = pf.CreateProgress();
            pf.Show(this);

            try
            {
                // Live engine thread control for additions
                pf.ConfigureThreads(maxThreads, curThreads, n =>
                {
                    try
                    {
                        SettingsService.Current.ThreadsAuto = false;
                        SettingsService.Current.Threads = n;
                        SettingsService.Save();
                        _zip.SetMaxConcurrency(n);
                        pf.Text = $"Adding to archive… ({n} threads)";
                    }
                    catch { }
                });

                var sem = _zip.ConcurrencyLimiter;

                long aggBytes = 0;
                int aggFiles = 0;
                string currentName = null;
                var sw = Stopwatch.StartNew();

                using var reportCts = new CancellationTokenSource();
                var reporter = Task.Run(async () =>
                {
                    while (!reportCts.IsCancellationRequested)
                    {
                        long pBytes = Math.Min(Interlocked.Read(ref aggBytes), grandBytes);
                        int pFiles = Math.Min(Volatile.Read(ref aggFiles), grandFiles);
                        double speed = sw.Elapsed.TotalSeconds > 0 ? pBytes / sw.Elapsed.TotalSeconds : 0.0;

                        progress.Report(new ZipProgress
                        {
                            Operation = ZipOperation.Build,
                            CurrentFile = Volatile.Read(ref currentName),
                            TotalFiles = grandFiles,
                            TotalBytesUncompressed = grandBytes,
                            FilesProcessed = pFiles,
                            BytesProcessedUncompressed = pBytes,
                            Elapsed = sw.Elapsed,
                            SpeedBytesPerSec = speed
                        });

                        try { await Task.Delay(200, reportCts.Token); } catch { break; }
                    }
                });

                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(pf.Token);
                var tasks = new List<Task>(jobs.Count);

                foreach (var job in jobs)
                {
                    linkedCts.Token.ThrowIfCancellationRequested();

                    tasks.Add(Task.Run(() =>
                    {
                        sem.WaitOne(linkedCts.Token);
                        try
                        {
                            Volatile.Write(ref currentName, job.internalName);

                            void OnUnc(long n) => Interlocked.Add(ref aggBytes, n);
                            void OnComp(long n) { /* compressed accounting optional */ }

                            // This call uses _zip.Encryption/_zip.Password set above
                            _zip.ZipDataWriter.AddFileWithProgress(job.fullPath, job.internalName, _zip.CompressionLevelValue, "", OnUnc, OnComp);

                            Interlocked.Increment(ref aggFiles);
                        }
                        finally
                        {
                            sem.Release();
                        }
                    }, linkedCts.Token));
                }

                await Task.WhenAll(tasks);
                reportCts.Cancel();
                try { await reporter; } catch { }

                // Preserve empty folders after files are added
                foreach (var folder in emptyFolderEntries.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    try { _zip.ZipDataWriter.AddEmptyFolder(folder); } catch { }
                }

                // Commit and reload archive view
                await Task.Run(() => _zip.Close());
                OpenArchive(_zipPath); // reopens and rebuilds tree/index
                                       // NavigateTo(_current) is not necessary because OpenArchive navigates to root;
                                       // if you want to stay at the same folder, you could preserve and re-nav here.

            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Add error"); }
            finally { pf.Close(); }
        }

        private static string CombineZipPath(string prefix, string name)
        {
            prefix = (prefix ?? "").Trim('/');
            name = (name ?? "").Trim('/');
            if (string.IsNullOrEmpty(prefix)) return name.Replace('\\','/');
            if (string.IsNullOrEmpty(name)) return prefix.Replace('\\','/');
            return (prefix + "/" + name).Replace('\\','/');
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }

        private bool EnsurePasswordForEncryptedIfNeeded()
        {
            if (_zip == null) return true;
            bool anyEncrypted = _zip.ZipFileEntries.Any(e => e.IsEncrypted);
            if (!anyEncrypted) return true;

            if (!string.IsNullOrEmpty(_zip.Password)) return true;
            using var dlg = new PasswordDialog();
            if (dlg.ShowDialog(this) != DialogResult.OK) return false;
            _zip.Password = dlg.Password;
            return true;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportList();
        }

        private void ExportList()
        {
            if (_zip == null) return;
            using var sfd = new SaveFileDialog { Filter = "CSV (.csv)|.csv|JSON (.json)|.json" };
            if (sfd.ShowDialog(this) != DialogResult.OK) return;

            var entries = _zip.ZipFileEntries;
            if (Path.GetExtension(sfd.FileName).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                var arr = entries.Select(e => new
                {
                    e.FilenameInZip,
                    e.FileSize,
                    e.CompressedSize,
                    Method = e.Method.ToString(),
                    e.ModifyTime
                }).ToList();
                File.WriteAllText(sfd.FileName, System.Text.Json.JsonSerializer.Serialize(arr, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                using var sw = new StreamWriter(sfd.FileName);
                sw.WriteLine("Name,Size,Compressed,Method,Modified");
                foreach (var e in entries)
                    sw.WriteLine($"\"{e.FilenameInZip}\",{e.FileSize},{e.CompressedSize},{e.Method},{e.ModifyTime:yyyy-MM-dd HH:mm:ss}");
            }
            TryOpenExplorerSelect(sfd.FileName);
        }

        private void MainForm_Load_2(object sender, EventArgs e)
        {

        }
    }
}
