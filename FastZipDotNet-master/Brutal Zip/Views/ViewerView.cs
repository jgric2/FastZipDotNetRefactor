using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public event Action<string> SearchTextChanged;

        public void SetStatus(string text) => lblStatus.Text = text;

        private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
