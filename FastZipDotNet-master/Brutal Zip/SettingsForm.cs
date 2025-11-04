using Brutal_Zip.Classes.Helpers;
using BrutalZip2025.BrutalControls;

namespace BrutalZip
{
    public partial class SettingsForm : ModernForm
    {
        public SettingsForm()
        {
            InitializeComponent();
            WireEvents();
            LoadUi();
        }

        private void WireEvents()
        {
            tbDefaultThreads.ValueChanged += (s, e) =>
            {
                lblDefaultThreadsValue.Text = tbDefaultThreads.Value.ToString();
            };

            chkThreadsAuto.CheckedChanged += (s, e) =>
            {
                tbDefaultThreads.Enabled = !chkThreadsAuto.Checked;
            };

            // Enable/disable algo choice with master checkbox
            chkEncryptNewDefault.CheckedChanged += (s, e) =>
            {
                cmbEncryptAlgo.Enabled = chkEncryptNewDefault.Checked;
            };

            btnOK.Click += (s, e) =>
            {
                // Persist method/level
                SettingsService.Current.DefaultMethod = cmbMethod.SelectedIndex switch
                {
                    0 => "Store",
                    2 => "Zstd",
                    _ => "Deflate"
                };
                SettingsService.Current.DefaultLevel = (int)numLevel.Value;

                // Persist threads
                SettingsService.Current.ThreadsAuto = chkThreadsAuto.Checked;
                SettingsService.Current.Threads = tbDefaultThreads.Value;

                // Persist extract defaults
                SettingsService.Current.ExtractDefault = rdoExtractHere.Checked ? "Here" : "Smart";

                // Persist Explorer options
                SettingsService.Current.OpenExplorerAfterCreate = chkOpenExplorerAfterCreate.Checked;
                SettingsService.Current.OpenExplorerAfterExtract = chkOpenExplorerAfterExtract.Checked;
                SettingsService.Current.AddContextMenu = chkContextMenu.Checked;

                // Persist encryption defaults (NEW)
                SettingsService.Current.EncryptNewArchivesByDefault = chkEncryptNewDefault.Checked;
                SettingsService.Current.DefaultEncryptAlgorithm = AlgoStringFromIndex(cmbEncryptAlgo.SelectedIndex);

                try
                {
                    SettingsService.Save();
                    if (SettingsService.Current.AddContextMenu) ShellIntegration.Install();
                    else ShellIntegration.Uninstall();

                    DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Failed to save settings or update Explorer integration:\n" + ex.Message,
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                }
            };
        }

        private void LoadUi()
        {
            // Method/level
            cmbMethod.SelectedIndex = SettingsService.Current.DefaultMethod switch
            {
                "Store" => 0,
                "Zstd" => 2,
                _ => 1
            };
            numLevel.Value = Math.Max(numLevel.Minimum, Math.Min(numLevel.Maximum, SettingsService.Current.DefaultLevel));

            // Threads
            int maxThreads = Math.Max(1, Environment.ProcessorCount * 2);
            tbDefaultThreads.Minimum = 1;
            tbDefaultThreads.Maximum = maxThreads;

            int current = Math.Max(1, Math.Min(maxThreads, SettingsService.Current.Threads));
            tbDefaultThreads.Value = current;
            lblDefaultThreadsValue.Text = current.ToString();

            chkThreadsAuto.Checked = SettingsService.Current.ThreadsAuto;
            tbDefaultThreads.Enabled = !chkThreadsAuto.Checked;

            // Extract default
            if (string.Equals(SettingsService.Current.ExtractDefault, "Here", StringComparison.OrdinalIgnoreCase))
            {
                rdoExtractHere.Checked = true;
                rdoExtractSmart.Checked = false;
            }
            else
            {
                rdoExtractSmart.Checked = true;
                rdoExtractHere.Checked = false;
            }

            // Explorer options
            chkOpenExplorerAfterCreate.Checked = SettingsService.Current.OpenExplorerAfterCreate;
            chkOpenExplorerAfterExtract.Checked = SettingsService.Current.OpenExplorerAfterExtract;
            chkContextMenu.Checked = SettingsService.Current.AddContextMenu;

            // Encryption defaults (NEW)
            chkEncryptNewDefault.Checked = SettingsService.Current.EncryptNewArchivesByDefault;
            cmbEncryptAlgo.SelectedIndex = AlgoIndexFromString(SettingsService.Current.DefaultEncryptAlgorithm);
            cmbEncryptAlgo.Enabled = chkEncryptNewDefault.Checked;
        }

        private static int AlgoIndexFromString(string s)
        {
            return s?.Trim()?.ToUpperInvariant() switch
            {
                "AES128" => 1,
                "AES192" => 2,
                "AES256" => 3,
                _ => 0 // ZipCrypto
            };
        }

        private static string AlgoStringFromIndex(int index)
        {
            return index switch
            {
                1 => "AES128",
                2 => "AES192",
                3 => "AES256",
                _ => "ZipCrypto"
            };
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }
    }
}