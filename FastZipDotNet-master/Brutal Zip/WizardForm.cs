using FastZipDotNet.Zip;
using static FastZipDotNet.Zip.Structure.ZipEntryEnums;

namespace Brutal_Zip
{
    public partial class WizardForm : Form
    {
        private string _createPassword;
        private string _extractPassword;

        public WizardForm()
        {
            InitializeComponent();

            // Hooks
            btnClose.Click += (_, __) => Close();
            btnCreateAddFiles.Click += (_, __) => AddFiles();
            btnCreateAddFolder.Click += (_, __) => AddFolder();
            btnCreateBrowse.Click += (_, __) => BrowseCreateDest();
            btnCreateSetPassword.Click += (_, __) => SetCreatePassword();
            tbCreateThreads.ValueChanged += (_, __) => lblCreateThreadsVal.Text = tbCreateThreads.Value.ToString();
            tbExtractThreads.ValueChanged += (_, __) => lblExtractThreadsVal.Text = tbExtractThreads.Value.ToString();

            btnZipBrowse.Click += (_, __) => BrowseZip();
            btnExtractBrowse.Click += (_, __) => BrowseExtractDest();

            btnCreateStart.Click += async (_, __) => await StartCreateAsync();
            btnExtractStart.Click += async (_, __) => await StartExtractAsync();

            cmbCreateMethod.SelectedIndex = 1; // Deflate
            cmbCreateAlgo.SelectedIndex = 0;   // ZipCrypto
        }

        private void AddFiles()
        {
            using var ofd = new OpenFileDialog { Multiselect = true, Filter = "All files|*.*" };
            if (ofd.ShowDialog(this) == DialogResult.OK)
                foreach (var p in ofd.FileNames) lstCreatePaths.Items.Add(p);
        }

        private void AddFolder()
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) == DialogResult.OK) lstCreatePaths.Items.Add(fbd.SelectedPath);
        }

        private void BrowseCreateDest()
        {
            using var sfd = new SaveFileDialog { Filter = "Zip files|*.zip" };
            if (sfd.ShowDialog(this) == DialogResult.OK) txtCreateDest.Text = sfd.FileName;
        }

        private void BrowseZip()
        {
            using var ofd = new OpenFileDialog { Filter = "Zip files|*.zip" };
            if (ofd.ShowDialog(this) == DialogResult.OK) txtZip.Text = ofd.FileName;
        }

        private void BrowseExtractDest()
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) == DialogResult.OK) txtExtractDest.Text = fbd.SelectedPath;
        }

        private void SetCreatePassword()
        {
            using var dlg = new PasswordDialog();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _createPassword = dlg.Password;
            }
        }

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
                if (lstCreatePaths.Items.Count == 0) { MessageBox.Show(this, "Add some files or folders.", "Create"); return; }
                if (string.IsNullOrWhiteSpace(txtCreateDest.Text)) { MessageBox.Show(this, "Select destination.", "Create"); return; }

                var method = cmbCreateMethod.SelectedIndex switch
                {
                    0 => Compression.Store,
                    2 => Compression.Zstd,
                    _ => Compression.Deflate
                };
                int level = (int)numCreateLevel.Value;

                var toAdd = lstCreatePaths.Items.Cast<string>().ToList();

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

                // Thread slider live control
                pf.ConfigureThreads(Math.Max(2, Environment.ProcessorCount * 2), tbCreateThreads.Value, n =>
                {
                    try { zip.SetMaxConcurrency(n); } catch { }
                });

                // Flatten folder content using existing API
                foreach (var p in toAdd)
                {
                    if (Directory.Exists(p))
                        await zip.ZipDataWriter.AddFilesToArchiveAsync(p, tbCreateThreads.Value, level, progress, pf.Token);
                    else if (File.Exists(p))
                    {
                        void OnUnc(long n) { }
                        void OnComp(long n) { }
                        zip.ZipDataWriter.AddFileWithProgress(p, Path.GetFileName(p), level, "", OnUnc, OnComp);
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
                if (string.IsNullOrWhiteSpace(txtZip.Text) || !File.Exists(txtZip.Text)) { MessageBox.Show(this, "Select a valid archive.", "Extract"); return; }

                string dest;
                if (chkExtractToFolderName.Checked)
                {
                    dest = Path.Combine(Path.GetDirectoryName(txtZip.Text) ?? ".", Path.GetFileNameWithoutExtension(txtZip.Text));
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(txtExtractDest.Text)) { MessageBox.Show(this, "Select destination.", "Extract"); return; }
                    dest = txtExtractDest.Text;
                }

                Directory.CreateDirectory(dest);

                using var pf = new BrutalZip.ProgressForm($"Extracting… ({tbExtractThreads.Value} threads)");
                var progress = pf.CreateProgress();
                pf.Show(this);

                using var zip = new FastZipDotNet.Zip.FastZipDotNet(txtZip.Text, Compression.Deflate, 6, tbExtractThreads.Value);

                // If encrypted, prompt once
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
    }
}