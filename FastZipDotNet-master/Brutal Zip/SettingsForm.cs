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
            LoadUi();
            WireEvents();
        }

        private void WireEvents()
        {
            chkThreadsAuto.CheckedChanged += (s, e) => numThreads.Enabled = !chkThreadsAuto.Checked;
        }

        private void LoadUi()
        {
            cmbMethod.SelectedIndex = SettingsService.Current.DefaultMethod switch
            {
                "Store" => 0,
                "Zstd" => 2,
                _ => 1
            };
            numLevel.Value = Math.Min(Math.Max(SettingsService.Current.DefaultLevel, 0), 22);

            chkThreadsAuto.Checked = SettingsService.Current.ThreadsAuto;
            numThreads.Value = Math.Min(Math.Max(SettingsService.Current.Threads, 1), 256);
            numThreads.Enabled = !chkThreadsAuto.Checked;

            if (SettingsService.Current.ExtractDefault == "Here")
            {
                rdoExtractHere.Checked = true;
                rdoExtractSmart.Checked = false;
            }
            else
            {
                rdoExtractSmart.Checked = true;
                rdoExtractHere.Checked = false;
            }

            chkOpenExplorerAfterCreate.Checked = SettingsService.Current.OpenExplorerAfterCreate;
            chkOpenExplorerAfterExtract.Checked = SettingsService.Current.OpenExplorerAfterExtract;
            chkContextMenu.Checked = SettingsService.Current.AddContextMenu;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SettingsService.Current.DefaultMethod = cmbMethod.SelectedIndex switch
            {
                0 => "Store",
                2 => "Zstd",
                _ => "Deflate"
            };
            SettingsService.Current.DefaultLevel = (int)numLevel.Value;
            SettingsService.Current.ThreadsAuto = chkThreadsAuto.Checked;
            SettingsService.Current.Threads = (int)numThreads.Value;
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
                MessageBox.Show(this, "Failed to save settings or update Explorer integration:\n" + ex.Message, "Error");
                DialogResult = DialogResult.None;
            }
        }
    }
}