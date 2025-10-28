using Brutal_Zip.Classes.Helpers;
using System;
using System.Windows.Forms;

namespace BrutalZip
{
    public partial class SettingsForm : Form
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

            btnOK.Click += (s, e) =>
            {
                // Persist
                SettingsService.Current.DefaultMethod = cmbMethod.SelectedIndex switch
                {
                    0 => "Store",
                    2 => "Zstd",
                    _ => "Deflate"
                };
                SettingsService.Current.DefaultLevel = (int)numLevel.Value;

                SettingsService.Current.ThreadsAuto = chkThreadsAuto.Checked;
                SettingsService.Current.Threads = tbDefaultThreads.Value;

                SettingsService.Current.ExtractDefault = rdoExtractHere.Checked ? "Here" : "Smart";
                SettingsService.Current.OpenExplorerAfterCreate = chkOpenExplorerAfterCreate.Checked;
                SettingsService.Current.OpenExplorerAfterExtract = chkOpenExplorerAfterExtract.Checked;
                SettingsService.Current.AddContextMenu = chkContextMenu.Checked;

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

            // Extract defaults
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
        }
    }
}