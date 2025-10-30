using static FastZipDotNet.Zip.Structure.ZipEntryEnums;

namespace Brutal_Zip
{
    public partial class WizardForm : Form
    {
        private class StagedItem
        {
            public string Path;
            public bool IsFolder;
            public long SizeBytes;  // for files; for folders computed async
            public int ItemsCount;  // for folders
            public string Name => IsFolder
                ? new DirectoryInfo(Path.TrimEnd('\\', '/')).Name
                : System.IO.Path.GetFileName(Path);
            public string Type => IsFolder ? "Folder" : "File";
        }

        private readonly IconCache _icons = new IconCache();
        private readonly List<StagedItem> _staging = new List<StagedItem>();
        private string _createPassword;

        public WizardForm()
        {
            InitializeComponent();

            // Hook events (designer-friendly: no lambdas inside InitializeComponent)
            btnClose.Click += (_, __) => Close();

            btnCreateAddFiles.Click += (_, __) => AddFiles();
            btnCreateAddFolder.Click += (_, __) => AddFolder();
            btnCreateBrowse.Click += (_, __) => BrowseCreateDest();
            btnCreateSetPassword.Click += (_, __) => SetCreatePassword();
            btnCreateStart.Click += async (_, __) => await StartCreateAsync();

            btnZipBrowse.Click += (_, __) => BrowseZip();
            btnExtractBrowse.Click += (_, __) => BrowseExtractDest();
            btnExtractStart.Click += async (_, __) => await StartExtractAsync();

            tbCreateThreads.ValueChanged += (_, __) => lblCreateThreadsVal.Text = tbCreateThreads.Value.ToString();
            tbExtractThreads.ValueChanged += (_, __) => lblExtractThreadsVal.Text = tbExtractThreads.Value.ToString();

            cmbCreateMethod.SelectedIndex = 1; // Deflate
            cmbCreateAlgo.SelectedIndex = 0;   // ZipCrypto

            lvCreate.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back) RemoveSelectedLvItems(); };

            // Context menu hooks (designer-friendly: not inside InitializeComponent)
            mnuCreateAddFiles.Click += (_, __) => AddFiles();
            mnuCreateAddFolder.Click += (_, __) => AddFolder();
            mnuCreateRemoveSelected.Click += (_, __) => RemoveSelectedLvItems();
            mnuCreateRemoveMissing.Click += (_, __) => RemoveMissingLvItems();
            mnuCreateClearAll.Click += (_, __) => ClearAllLvItems();

            // Keyboard Delete to remove selected
            lvCreate.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete) RemoveSelectedLvItems(); };

            // Allow drop into the list (optional but handy)
            lvCreate.AllowDrop = true;
            lvCreate.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            };
            lvCreate.DragDrop += (s, e) =>
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (paths == null) return;
                foreach (var p in paths) TryAddPath(p);
            };

            // Icons for listview
            lvCreate.SmallImageList = _icons.ImageList;
        }


        private void RemoveSelectedLvItems()
        {
            if (lvCreate.SelectedItems.Count == 0) return;
            // Collect items first to avoid modifying collection while iterating
            var toRemove = new List<ListViewItem>();
            foreach (ListViewItem lvi in lvCreate.SelectedItems) toRemove.Add(lvi);

            foreach (var lvi in toRemove)
            {
                if (lvi.Tag is StagedItem s)
                    _staging.Remove(s);
                lvCreate.Items.Remove(lvi);
            }
        }

        private void RemoveMissingLvItems()
        {
            var toRemove = new List<StagedItem>();
            foreach (ListViewItem lvi in lvCreate.Items)
            {
                if (lvi.Tag is StagedItem s)
                {
                    bool exists = s.IsFolder ? Directory.Exists(s.Path) : File.Exists(s.Path);
                    if (!exists) toRemove.Add(s);
                }
            }

            foreach (var s in toRemove)
            {
                // Remove from ListView
                for (int i = 0; i < lvCreate.Items.Count; i++)
                {
                    if (ReferenceEquals(lvCreate.Items[i].Tag, s))
                    {
                        lvCreate.Items.RemoveAt(i);
                        break;
                    }
                }
                // Remove from model
                _staging.Remove(s);
            }
        }

        private void ClearAllLvItems()
        {
            _staging.Clear();
            lvCreate.Items.Clear();
        }


        private void AddFiles()
        {
            using var ofd = new OpenFileDialog { Multiselect = true, Filter = "All files|*.*" };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            foreach (var p in ofd.FileNames)
                TryAddPath(p);
        }

        private void AddFolder()
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) != DialogResult.OK) return;

            TryAddPath(fbd.SelectedPath);
        }

        private void TryAddPath(string inputPath)
        {
            try
            {
                string full = Path.GetFullPath(inputPath);
                bool isDir = Directory.Exists(full);
                bool isFile = File.Exists(full);
                if (!isDir && !isFile) return;

                if (_staging.Any(s => string.Equals(s.Path, full, StringComparison.OrdinalIgnoreCase)))
                    return; // avoid duplicates

                var item = new StagedItem
                {
                    Path = full,
                    IsFolder = isDir,
                    SizeBytes = isFile ? new FileInfo(full).Length : 0,
                    ItemsCount = isFile ? 1 : 0
                };

                _staging.Add(item);
                AddListViewRow(item);

                if (item.IsFolder)
                {
                    // compute folder stats async and update UI
                    _ = Task.Run(() =>
                    {
                        ComputeFolderStats(item.Path, out long bytes, out int count);
                        item.SizeBytes = bytes;
                        item.ItemsCount = count;
                        BeginInvoke(new Action(() => UpdateListViewRow(item)));
                    });
                }
            }
            catch { /* ignore bad paths */ }
        }

        private void AddListViewRow(StagedItem s)
        {
            var it = new ListViewItem(s.Name);
            it.SubItems.Add(s.Type);
            it.SubItems.Add(s.IsFolder ? "Calculating…" : FormatBytes(s.SizeBytes));
            it.SubItems.Add(s.IsFolder ? "…" : "1");
            it.SubItems.Add(s.Path);
            it.ImageIndex = s.IsFolder ? _icons.FolderIndex : _icons.GetIndexForExtension(Path.GetExtension(s.Path));
            it.Tag = s;
            lvCreate.Items.Add(it);
        }

        private void UpdateListViewRow(StagedItem s)
        {
            foreach (ListViewItem it in lvCreate.Items)
            {
                if (ReferenceEquals(it.Tag, s))
                {
                    it.SubItems[2].Text = FormatBytes(s.SizeBytes);
                    it.SubItems[3].Text = s.ItemsCount.ToString();
                    break;
                }
            }
        }

        private static void ComputeFolderStats(string folder, out long sizeBytes, out int fileCount)
        {
            sizeBytes = 0;
            fileCount = 0;
            try
            {
                var stack = new Stack<string>();
                stack.Push(folder);
                while (stack.Count > 0)
                {
                    var d = stack.Pop();
                    try
                    {
                        foreach (var file in Directory.EnumerateFiles(d))
                        {
                            try
                            {
                                sizeBytes += new FileInfo(file).Length;
                                fileCount++;
                            }
                            catch { }
                        }
                        foreach (var sub in Directory.EnumerateDirectories(d))
                            stack.Push(sub);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void BrowseCreateDest()
        {
            using var sfd = new SaveFileDialog { Filter = "Zip files|*.zip" };
            if (sfd.ShowDialog(this) == DialogResult.OK)
                txtCreateDest.Text = sfd.FileName;
        }

        private void BrowseZip()
        {
            using var ofd = new OpenFileDialog { Filter = "Zip files|*.zip" };
            if (ofd.ShowDialog(this) == DialogResult.OK)
                txtZip.Text = ofd.FileName;
        }

        private void BrowseExtractDest()
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) == DialogResult.OK)
                txtExtractDest.Text = fbd.SelectedPath;
        }

        private void SetCreatePassword()
        {
            using var dlg = new PasswordDialog();
            if (dlg.ShowDialog(this) == DialogResult.OK)
                _createPassword = dlg.Password;
        }

        private Compression MethodFromIndex(int idx) => idx switch
        {
            0 => Compression.Store,
            2 => Compression.Zstd,
            _ => Compression.Deflate
        };

        private EncryptionAlgorithm AlgoFromIndex(int idx) => idx switch
        {
            1 => EncryptionAlgorithm.Aes128,
            2 => EncryptionAlgorithm.Aes192,
            3 => EncryptionAlgorithm.Aes256,
            _ => EncryptionAlgorithm.ZipCrypto
        };

        private async Task StartCreateAsync()
        {
            try
            {
                if (_staging.Count == 0)
                {
                    MessageBox.Show(this, "Add some files or folders first.", "Create");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtCreateDest.Text))
                {
                    MessageBox.Show(this, "Choose destination.", "Create");
                    return;
                }

                var method = MethodFromIndex(cmbCreateMethod.SelectedIndex);
                int level = (int)numCreateLevel.Value;

                using var pf = new BrutalZip.ProgressForm($"Creating archive… ({tbCreateThreads.Value} threads)");
                var progress = pf.CreateProgress();
                pf.Show(this);

                using var zip = new FastZipDotNet.Zip.FastZipDotNet(txtCreateDest.Text, method, level, tbCreateThreads.Value);

                if (chkCreateEncrypt.Checked)
                {
                    if (string.IsNullOrEmpty(_createPassword))
                    {
                        using var dlg = new PasswordDialog();
                        if (dlg.ShowDialog(this) != DialogResult.OK) return;
                        _createPassword = dlg.Password;
                    }
                    zip.Encryption = AlgoFromIndex(cmbCreateAlgo.SelectedIndex);
                    zip.Password = _createPassword;
                }
                else
                {
                    zip.Encryption = EncryptionAlgorithm.None;
                }

                pf.ConfigureThreads(Math.Max(2, Environment.ProcessorCount * 2), tbCreateThreads.Value, n =>
                {
                    try { zip.SetMaxConcurrency(n); } catch { }
                });

                // Add items
                foreach (var s in _staging)
                {
                    if (s.IsFolder && Directory.Exists(s.Path))
                    {
                        await zip.ZipDataWriter.AddFilesToArchiveAsync(s.Path, tbCreateThreads.Value, level, progress, pf.Token);
                    }
                    else if (!s.IsFolder && File.Exists(s.Path))
                    {
                        void OnUnc(long n) { }
                        void OnComp(long n) { }
                        zip.ZipDataWriter.AddFileWithProgress(s.Path, Path.GetFileName(s.Path), level, "", OnUnc, OnComp);
                    }
                }

                zip.Close();
                MessageBox.Show(this, "Archive created.", "Create");
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Create");
            }
        }

        private async Task StartExtractAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtZip.Text) || !File.Exists(txtZip.Text))
                {
                    MessageBox.Show(this, "Select a valid archive.", "Extract");
                    return;
                }

                string dest;
                if (chkExtractToFolderName.Checked)
                    dest = Path.Combine(Path.GetDirectoryName(txtZip.Text) ?? ".", Path.GetFileNameWithoutExtension(txtZip.Text));
                else
                {
                    if (string.IsNullOrWhiteSpace(txtExtractDest.Text))
                    {
                        MessageBox.Show(this, "Select destination.", "Extract");
                        return;
                    }
                    dest = txtExtractDest.Text;
                }

                Directory.CreateDirectory(dest);

                using var pf = new BrutalZip.ProgressForm($"Extracting… ({tbExtractThreads.Value} threads)");
                var progress = pf.CreateProgress();
                pf.Show(this);

                using var zip = new FastZipDotNet.Zip.FastZipDotNet(txtZip.Text, Compression.Deflate, 6, tbExtractThreads.Value);

                if (zip.ZipFileEntries.Any(e => e.IsEncrypted) && string.IsNullOrEmpty(zip.Password))
                {
                    using var dlg = new PasswordDialog();
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    zip.Password = dlg.Password;
                }

                pf.ConfigureThreads(Math.Max(2, Environment.ProcessorCount * 2), tbExtractThreads.Value, n =>
                {
                    try { zip.SetMaxConcurrency(n); } catch { }
                });

                await zip.ExtractArchiveAsync(dest, progress, pf.Token);
                MessageBox.Show(this, "Extract complete.", "Extract");
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Extract");
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] u = { "B", "KB", "MB", "GB", "TB" };
            double s = bytes; int i = 0;
            while (s >= 1024 && i < u.Length - 1) { s /= 1024; i++; }
            return $"{s:F1} {u[i]}";
        }

        private void WizardForm_Load(object sender, EventArgs e)
        {
            tbExtractThreads.Maximum = Environment.ProcessorCount * 2;
            tbCreateThreads.Maximum = Environment.ProcessorCount * 2;
        }
    }
}