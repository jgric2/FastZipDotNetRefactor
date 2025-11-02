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

namespace Brutal_Zip.Views
{
    public partial class ViewerView : UserControl
    {
        internal ToolStripMenuItem mnuRename;           // NEW
        internal ToolStripMenuItem mnuMoveToFolder;     // NEW
        internal ToolStripMenuItem mnuDelete;           // NEW

        public event Action RenameSelectedRequested;    // NEW
        public event Action MoveToFolderRequested;      // NEW
        public event Action DeleteSelectedRequested;    // NEW


        public ViewerView()
        {
            InitializeComponent();

            //this.btnExtractSplit.DropDownItems.Add("Extract selected…");
            //this.btnExtractSplit.DropDownItems.Add("Extract here");
            //this.btnExtractSplit.DropDownItems.Add("Extract to “ArchiveName”/");
            //this.btnExtractSplit.DropDownItems.Add("Choose folder…");



            mnuRename = new ToolStripMenuItem("Rename…");
            mnuMoveToFolder = new ToolStripMenuItem("Move to folder…");
            mnuDelete = new ToolStripMenuItem("Delete");

            // Insert after existing items
            if (cmsViewer != null)
            {
                cmsViewer.Items.Add(new ToolStripSeparator());
                cmsViewer.Items.Add(mnuRename);
                cmsViewer.Items.Add(mnuMoveToFolder);
                cmsViewer.Items.Add(mnuDelete);

                mnuRename.Click += (s, e) => RenameSelectedRequested?.Invoke();
                mnuMoveToFolder.Click += (s, e) => MoveToFolderRequested?.Invoke();
                mnuDelete.Click += (s, e) => DeleteSelectedRequested?.Invoke();
            }


            btnBackHome.Click += (s, e) => BackHomeClicked?.Invoke();
            btnAddFiles.Click += (s, e) => AddFilesClicked?.Invoke();
            btnAddFolder.Click += (s, e) => AddFolderClicked?.Invoke();

            btnExtractSplit.ButtonClick += (s, e) => ExtractSelectedToClicked?.Invoke();
            btnExtractSplit.DropDownItems[0].Click += (s, e) => ExtractSelectedToClicked?.Invoke();
            btnExtractSplit.DropDownItems[1].Click += (s, e) => ExtractHereClicked?.Invoke();
            btnExtractSplit.DropDownItems[2].Click += (s, e) => ExtractSmartClicked?.Invoke();
            btnExtractSplit.DropDownItems[3].Click += (s, e) => ExtractSelectedToClicked?.Invoke();

            btnInfo.Click += (s, e) => InfoClicked?.Invoke();
            btnTest.Click += (s, e) => TestClicked?.Invoke();
            btnSettings.Click += (s, e) => SettingsClicked?.Invoke();
            txtSearch.TextChanged += (s, e) => SearchTextChanged?.Invoke(txtSearch.Text);


            mnuEncryptNew.CheckedChanged += (s, e) => EncryptNewItemsChanged?.Invoke(mnuEncryptNew.Checked);
            mnuSetAddPassword.Click += (s, e) => SetAddPasswordClicked?.Invoke();

            void ClearAlgoChecks()
            {
                mnuAlgoZipCrypto.Checked = false;
                mnuAlgoAES128.Checked = false;
                mnuAlgoAES192.Checked = false;
                mnuAlgoAES256.Checked = false;
            }

            mnuAlgoZipCrypto.Click += (s, e) =>
            {
                ClearAlgoChecks();
                SetAddEncryptionSelection(EncryptionAlgorithm.ZipCrypto);
                AddEncryptionAlgorithmChanged?.Invoke(EncryptionAlgorithm.ZipCrypto);
            };
            mnuAlgoAES128.Click += (s, e) =>
            {
                ClearAlgoChecks();
                SetAddEncryptionSelection(EncryptionAlgorithm.Aes128);
                AddEncryptionAlgorithmChanged?.Invoke(EncryptionAlgorithm.Aes128);
            };
            mnuAlgoAES192.Click += (s, e) =>
            {
                ClearAlgoChecks();
                SetAddEncryptionSelection(EncryptionAlgorithm.Aes192);
                AddEncryptionAlgorithmChanged?.Invoke(EncryptionAlgorithm.Aes192);
            };
            mnuAlgoAES256.Click += (s, e) =>
            {
                ClearAlgoChecks();
                SetAddEncryptionSelection(EncryptionAlgorithm.Aes256);
                AddEncryptionAlgorithmChanged?.Invoke(EncryptionAlgorithm.Aes256);
            };


            btnComment.Click += (s, e) => CommentClicked?.Invoke();
            btnWizard.Click += (s, e) => WizardClicked?.Invoke();

            btnToggleInfo.Click += (s, e) => InfoToggleClicked?.Invoke(); // NEW
        }


        public void SetAddEncryptionSelection(EncryptionAlgorithm algo)
        {
            mnuAlgoZipCrypto.Checked = false;
            mnuAlgoAES128.Checked = false;
            mnuAlgoAES192.Checked = false;
            mnuAlgoAES256.Checked = false;

            switch (algo)
            {
                case EncryptionAlgorithm.Aes128: mnuAlgoAES128.Checked = true; break;
                case EncryptionAlgorithm.Aes192: mnuAlgoAES192.Checked = true; break;
                case EncryptionAlgorithm.Aes256: mnuAlgoAES256.Checked = true; break;
                default: mnuAlgoZipCrypto.Checked = true; break;
            }
        }


        public event Action CommentClicked;
        public event Action WizardClicked;

        public event Action BackHomeClicked;
        public event Action AddFilesClicked;
        public event Action AddFolderClicked;

        public event Action ExtractSelectedToClicked;
        public event Action ExtractHereClicked;
        public event Action ExtractSmartClicked;

        public event Action InfoClicked;
        public event Action TestClicked;
        public event Action SettingsClicked;

        public event Action<bool> EncryptNewItemsChanged;
        public event Action SetAddPasswordClicked;
        public event Action<EncryptionAlgorithm> AddEncryptionAlgorithmChanged;
        public event Action InfoToggleClicked; // NEW

        public event Action<string> SearchTextChanged;

        public void SetStatus(string text) => lblStatus.Text = text;

        private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void previewPane_Load(object sender, EventArgs e)
        {

        }

        private void dropDownButton1_DropDownOpening(object sender, CancelEventArgs e)
        {
            var tt = "";

            //PanelDropExtract.Show();

        }

        private void dropDownButton1_DropDownClicked(object sender, EventArgs e)
        {
            var tt = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BackHomeClicked?.Invoke();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddFilesClicked?.Invoke();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ExtractSelectedToClicked?.Invoke();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //   OpenArchiveFolderInExplorer();
        }

        //private void OpenArchiveFolderInExplorer()
        //{
        //    if (string.IsNullOrEmpty(_zipPath)) return;
        //    try { Process.Start("explorer.exe", $"/select,\"{_zipPath}\""); } catch { }
        //}

        private void button5_Click(object sender, EventArgs e)
        {
            CommentClicked?.Invoke();
        }


        private void txtSearch_Click(object sender, EventArgs e)
        {

        }


        private void buttonFilePreview_Click(object sender, EventArgs e)
        {
            TogglePreviewPane();
        }


        private void TogglePreviewPane()
        {
            splitMain.Panel2Collapsed = !splitMain.Panel2Collapsed;
        }

        private void brutalGradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            panelSearch.Visible = !panelSearch.Visible;
            if (panelSearch.Visible)
                panelSearch.BringToFront();
            else
                panelSearch.SendToBack();
        }

        private void buttonWizard_Click(object sender, EventArgs e)
        {
            WizardClicked?.Invoke();
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            TestClicked?.Invoke();
        }

        private void buttonInfoPane_Click(object sender, EventArgs e)
        {
            InfoToggleClicked?.Invoke();
        }

        private void buttonArchiveInfo_Click(object sender, EventArgs e)
        {
            InfoClicked?.Invoke();
        }

        private void ViewerView_Load(object sender, EventArgs e)
        {

        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            SearchTextChanged?.Invoke(txtSearch.Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ExtractHereClicked?.Invoke();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ExtractSmartClicked?.Invoke();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            ExtractSelectedToClicked?.Invoke();
        }

        private void buttonExtract_Click(object sender, EventArgs e)
        {
            ExtractSelectedToClicked?.Invoke();
        }

        private void buttonCloseSearch_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = false;
        }
    }
}
