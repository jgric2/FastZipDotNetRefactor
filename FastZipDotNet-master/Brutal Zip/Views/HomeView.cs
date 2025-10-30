using System.Data;

namespace Brutal_Zip.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

            // CREATE: big drop zone (add files/folders to staging)
            pnlCreateDrop.AllowDrop = true;
            pnlCreateDrop.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            };
            pnlCreateDrop.DragDrop += (s, e) =>
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                FilesDroppedForCreate?.Invoke(paths ?? Array.Empty<string>());
            };

            // CREATE: staging list drop
            lvStaging.AllowDrop = true;
            lvStaging.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            };
            lvStaging.DragDrop += (s, e) =>
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                FilesDroppedForCreate?.Invoke(paths ?? Array.Empty<string>());
            };

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

            // Context menu
            mnuStagingRemove.Click += (s, e) => StagingRemoveSelectedRequested?.Invoke();
            mnuStagingRemoveMissing.Click += (s, e) => StagingRemoveMissingRequested?.Invoke();
            mnuStagingClear.Click += (s, e) => StagingClearRequested?.Invoke();

            // Buttons
            btnCreateAddFiles.Click += (s, e) => AddFilesClicked?.Invoke();
            btnCreateAddFolder.Click += (s, e) => AddFolderClicked?.Invoke();
            btnCreateBrowse.Click += (s, e) => BrowseCreateDestinationClicked?.Invoke();
           // btnCreate.Click += (s, e) => CreateClicked?.Invoke();
            btnCreateQuick.Click += (s, e) => QuickCreateClicked?.Invoke();

            btnOpenArchive.Click += (s, e) => OpenArchiveClicked?.Invoke();
            btnExtractBrowse.Click += (s, e) => BrowseExtractDestinationClicked?.Invoke();
            btnExtract.Click += (s, e) => ExtractClicked?.Invoke();

            // Threads slider + Auto
            tbThreads.ValueChanged += (s, e) =>
            {
                lblThreadsValue.Text = tbThreads.Value.ToString();
                ThreadsSliderChanged?.Invoke(tbThreads.Value);
            };
            chkThreadsAutoMain.CheckedChanged += (s, e) =>
            {
                tbThreads.Enabled = !chkThreadsAutoMain.Checked;
                ThreadsAutoChanged?.Invoke(chkThreadsAutoMain.Checked);
            };
        }

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
    }
}
