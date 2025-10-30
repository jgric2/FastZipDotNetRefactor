using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public ViewerView()
        {
            InitializeComponent();

            //this.btnExtractSplit.DropDownItems.Add("Extract selected…");
            //this.btnExtractSplit.DropDownItems.Add("Extract here");
            //this.btnExtractSplit.DropDownItems.Add("Extract to “ArchiveName”/");
            //this.btnExtractSplit.DropDownItems.Add("Choose folder…");




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


        public event Action<string> SearchTextChanged;

        public void SetStatus(string text) => lblStatus.Text = text;

        private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
