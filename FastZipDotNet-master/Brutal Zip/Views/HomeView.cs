using Brutal_Zip.Controls.BrutalControls.FileOrFolderDialog;
using System.CodeDom;
using System.Collections;
using System.Data;
using System.Xml.Linq;

namespace Brutal_Zip.Views
{
    public partial class HomeView : UserControl
    {

        FileExplorer fe = new FileExplorer();


        public HomeView()
        {
            InitializeComponent();


            chkEncrypt.Checked = false;
            cmbEncrypt.SelectedIndex = 0;
            cmbEncrypt.Enabled = false;
            btnCreateSetPassword.Enabled = false;


            // CREATE: staging list drop
            lvStaging.AllowDrop = true;


            // EXTRACT: .zip drop
            pnlExtractDrop.AllowDrop = true;
            pnlExtractDrop.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            };
            pnlExtractDrop.DragDrop += (s, e) =>
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (paths == null || paths.Length == 0) return;

                var zips = paths.Where(p => string.Equals(Path.GetExtension(p), ".zip", StringComparison.OrdinalIgnoreCase) && File.Exists(p))
                                .ToList();
                if (zips.Count == 0) return;

                if (zips.Count == 1)
                {
                    ZipDroppedForExtract?.Invoke(zips[0]);
                }
                else
                {
                    // Ask user if they want separate folders (Yes) or single destination (No)
                    var dlg = MessageBox.Show(this,
                        $"Extract {zips.Count} archives?\n\nYes = each into its own folder\nNo = choose one destination for all",
                        "Batch extract", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dlg == DialogResult.Cancel) return;
                    bool separate = (dlg == DialogResult.Yes);
                    BatchZipsDroppedForExtract?.Invoke(zips, separate);
                }
            };

        }


        public bool CreateEncryptEnabled
        {
            get => chkEncrypt.Checked;
            set => chkEncrypt.Checked = value;
        }
        public int CreateEncryptAlgorithmIndex
        {
            get => cmbEncrypt.SelectedIndex;
            set => cmbEncrypt.SelectedIndex = value;
        }
        public void SetCreatePasswordStatus(bool set)
            => lblPwdStatus.Text = set ? "Password: (set)" : "Password: (not set)";


        public event Action WizardClicked;
        public event Action CommentClicked;



        // Events consumed by MainForm
        public event Action<string[]> FilesDroppedForCreate;
        public event Action AddFilesClicked;
        public event Action AddFolderClicked;
        public event Action BrowseCreateDestinationClicked;
        public event Action CreateClicked;
        public event Action QuickCreateClicked;

        public event Action OpenArchiveClicked;
        public event Action BrowseExtractDestinationClicked;
        public event Action ExtractClicked;

        public event Action<string> ZipDroppedForExtract;
        public event Action<List<string>, bool> BatchZipsDroppedForExtract; // NEW

        public event Action StagingRemoveSelectedRequested;
        public event Action StagingRemoveMissingRequested;                 // NEW
        public event Action StagingClearRequested;

        public event Action<int> ThreadsSliderChanged;
        public event Action<bool> ThreadsAutoChanged;

        public event Action<bool> CreateEncryptChanged;
        public event Action<int> CreateEncryptAlgorithmChanged;
        public event Action CreateSetPasswordClicked;


        // Accessors used by MainForm
        public ListView StagingListView => lvStaging;

        public void SetStagingInfo(string text) => lblCreateHint.Text = text;

        public string CreateDestination { get => txtCreateDest.Text; set => txtCreateDest.Text = value; }
        public int CreateMethodIndex { get => cmbCreateMethod.SelectedIndex; set => cmbCreateMethod.SelectedIndex = value; }
        public int CreateLevel
        {
            get => (int)numCreateLevel.Value;
            set
            {
                if (value < (int)numCreateLevel.Minimum) value = (int)numCreateLevel.Minimum;
                if (value > (int)numCreateLevel.Maximum) value = (int)numCreateLevel.Maximum;
                numCreateLevel.Value = value;
            }
        }

        public bool ExtractToArchiveName => rdoExtractToFolderName.Checked;
        public bool ExtractHere => rdoExtractHere.Checked;
        public string ExtractDestination { get => txtExtractDest.Text; set => txtExtractDest.Text = value; }

        public void SetThreadSlider(int max, int value, bool auto)
        {
            if (max < 1) max = 1;
            if (value < 1) value = 1;
            if (value > max) value = max;

            tbThreads.Minimum = 1;
            tbThreads.Maximum = max;
            tbThreads.Value = value;
            lblThreadsValue.Text = value.ToString();

            chkThreadsAutoMain.Checked = auto;
            tbThreads.Enabled = !auto;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            CreateClicked?.Invoke();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {

        }

        private void HomeView_Load(object sender, EventArgs e)
        {
            fe.CreateTree(this.treeViewExplorer);
        }

        private void buttonAddFolder_Click(object sender, EventArgs e)
        {
            AddFolderClicked?.Invoke();
        }

        private void buttonAddFiles_Click(object sender, EventArgs e)
        {
            AddFilesClicked?.Invoke();
        }

        private void buttonWizardCreate_Click(object sender, EventArgs e)
        {
            WizardClicked?.Invoke();
        }

        private void grpCreate_Enter(object sender, EventArgs e)
        {

        }

        private void chkThreadsAutoMain_CheckedChanged(object sender, EventArgs e)
        {
            tbThreads.Enabled = !chkThreadsAutoMain.Checked;
            ThreadsAutoChanged?.Invoke(chkThreadsAutoMain.Checked);
        }

        private void cmbEncrypt_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateEncryptAlgorithmChanged?.Invoke(cmbEncrypt.SelectedIndex);
        }

        private void chkEncrypt_CheckedChanged(object sender, EventArgs e)
        {
            cmbEncrypt.Enabled = chkEncrypt.Checked;
            btnCreateSetPassword.Enabled = chkEncrypt.Checked;
            CreateEncryptChanged?.Invoke(chkEncrypt.Checked);
        }

        private void btnCreateSetPassword_Click(object sender, EventArgs e)
        {
            CreateSetPasswordClicked?.Invoke();
        }

        private void lvStaging_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) ||
            e.Data.GetDataPresent("TreeNodeArray"))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void lvStaging_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] paths = null;

                // Preferred: Windows/TreeViewMS FileDrop payload
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                }
                // Back-compat: your custom "TreeNodeArray" payload
                else if (e.Data.GetDataPresent("TreeNodeArray"))
                {
                    var list = new List<string>();

                    object obj = e.Data.GetData("TreeNodeArray");
                    if (obj is ArrayList al)
                    {
                        foreach (var item in al)
                        {
                            if (item is TreeNode n && n.Tag is string p && !string.IsNullOrWhiteSpace(p))
                                list.Add(p);
                        }
                    }
                    else if (obj is TreeNode[] arr)
                    {
                        foreach (var n in arr)
                        {
                            if (n?.Tag is string p && !string.IsNullOrWhiteSpace(p))
                                list.Add(p);
                        }
                    }

                    paths = list.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                }

                if (paths != null && paths.Length > 0)
                    FilesDroppedForCreate?.Invoke(paths);
            }
            catch
            {
                // keep it quiet for UI
            }
        }

        private void btnCreateQuick_Click(object sender, EventArgs e)
        {

        }

        private void buttonOpenZip_Click(object sender, EventArgs e)
        {
            OpenArchiveClicked?.Invoke();
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            ExtractClicked?.Invoke();
        }

        private void btnExtractBrowse_Click(object sender, EventArgs e)
        {
            BrowseExtractDestinationClicked?.Invoke();
        }

        private void btnCreateQuick_Click_1(object sender, EventArgs e)
        {
            QuickCreateClicked?.Invoke();
        }

        private void btnCreateBrowse_Click(object sender, EventArgs e)
        {
            BrowseCreateDestinationClicked?.Invoke();
        }

        private void mnuStagingRemove_Click(object sender, EventArgs e)
        {
            StagingRemoveSelectedRequested?.Invoke();
        }

        private void mnuStagingRemoveMissing_Click(object sender, EventArgs e)
        {
            StagingRemoveMissingRequested?.Invoke();
        }

        private void mnuStagingClear_Click(object sender, EventArgs e)
        {
            StagingClearRequested?.Invoke();
        }

        private void tbThreads_ValueChanged(object sender, EventArgs e)
        {
            lblThreadsValue.Text = tbThreads.Value.ToString();
            ThreadsSliderChanged?.Invoke(tbThreads.Value);
        }

        private void grpExtract_Enter(object sender, EventArgs e)
        {

        }

        private void tbThreads_Scroll(object sender, EventArgs e)
        {

        }

        private void tabButtonCreate_CheckedChanged(object sender, EventArgs e)
        {
            if (tabButtonCreate.Checked)
            {
                TabButtonExtract.Checked = false;

                panelCreateZipTab.Visible = true;
                panelCreateZipTab.BringToFront();

                panelExtractZipTab.Visible = false;
            }
            else
            {
                if (TabButtonExtract.Checked == false)
                    tabButtonCreate.Checked = true;
                else
                    tabButtonCreate.Checked = false;
            }
        }

        private void TabButtonExtract_CheckedChanged(object sender, EventArgs e)
        {

            if (TabButtonExtract.Checked)
            {
                tabButtonCreate.Checked = false;

                panelExtractZipTab.Visible = true;
                panelExtractZipTab.BringToFront();

                panelCreateZipTab.Visible = false;
            }
            else
            {
                if (tabButtonCreate.Checked == false)
                    TabButtonExtract.Checked = true;
                else
                    TabButtonExtract.Checked = false;
            }
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonComment_Click(object sender, EventArgs e)
        {
            CommentClicked?.Invoke();
        }

        private void buttonWizardExtract_Click(object sender, EventArgs e)
        {
            WizardClicked?.Invoke();
        }

        private void treeViewExplorer_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Let TreeViewMS.OnItemDrag produce the proper DataObject (FileDrop + TreeNodeArray).
            // No action here.
        }

        private void treeViewExplorer_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            //if (e.Node.Nodes[0].Text == "")
            //{
            //    TreeNode node = fe.EnumerateDirectory(e.Node);
            //    if (Directory.Exists(node.Tag.ToString()))
            //    {
            //        backHistory.Push(currentPath);
            //        string nextPath = node.Tag.ToString();
            //        LoadDirectory(nextPath);
            //    }
            //}
        }

        private void treeViewExplorer_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)
                return;

            if (e.Node.Nodes[0].Text == "")
            {
                // TreeNode node = fe.EnumerateDirectory(e.Node);
                if (Directory.Exists(e.Node.Tag.ToString()))
                {
                    e.Node.Expand();
                    //backHistory.Push(currentPath);
                    //string nextPath = e.Node.Tag.ToString();
                    //LoadDirectory(nextPath);
                }
            }
        }
    }
}
