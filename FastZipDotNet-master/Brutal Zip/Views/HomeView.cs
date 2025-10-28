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

            // CREATE: staging ListView also accepts drops
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

            // EXTRACT: big drop zone (.zip)
            pnlExtractDrop.AllowDrop = true;
            pnlExtractDrop.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            };
            pnlExtractDrop.DragDrop += (s, e) =>
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (paths == null || paths.Length == 0) return;

                var zip = paths.FirstOrDefault(p =>
                    string.Equals(Path.GetExtension(p), ".zip", StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(zip) && File.Exists(zip))
                    ZipDroppedForExtract?.Invoke(zip);
            };

            // Context menu on staging list
            mnuStagingRemove.Click += (s, e) => StagingRemoveSelectedRequested?.Invoke();
            mnuStagingClear.Click += (s, e) => StagingClearRequested?.Invoke();

            // Buttons raise events (kept from earlier)
            btnCreateAddFiles.Click += (s, e) => AddFilesClicked?.Invoke();
            btnCreateAddFolder.Click += (s, e) => AddFolderClicked?.Invoke();
            btnCreateBrowse.Click += (s, e) => BrowseCreateDestinationClicked?.Invoke();
            btnCreate.Click += (s, e) => CreateClicked?.Invoke();

            btnOpenArchive.Click += (s, e) => OpenArchiveClicked?.Invoke();
            btnExtractBrowse.Click += (s, e) => BrowseExtractDestinationClicked?.Invoke();
            btnExtract.Click += (s, e) => ExtractClicked?.Invoke();
        }

        // Events consumed by MainForm
        public event Action<string[]> FilesDroppedForCreate;
        public event Action AddFilesClicked;
        public event Action AddFolderClicked;
        public event Action BrowseCreateDestinationClicked;
        public event Action CreateClicked;

        public event Action OpenArchiveClicked;
        public event Action BrowseExtractDestinationClicked;
        public event Action ExtractClicked;

        // The missing event MainForm expects
        public event Action<string> ZipDroppedForExtract;

        // Staging context menu actions
        public event Action StagingRemoveSelectedRequested;
        public event Action StagingClearRequested;

        // Accessors used by MainForm
        public ListView StagingListView => lvStaging;

        public void SetStagingInfo(string text) => lblCreateHint.Text = text;

        public string CreateDestination { get => txtCreateDest.Text; set => txtCreateDest.Text = value; }
        public int CreateMethodIndex { get => cmbCreateMethod.SelectedIndex; set => cmbCreateMethod.SelectedIndex = value; }
        public int CreateLevel { get => (int)numCreateLevel.Value; set => numCreateLevel.Value = Math.Min(Math.Max(value, 0), 22); }

        public bool ExtractToArchiveName => rdoExtractToFolderName.Checked;
        public bool ExtractHere => rdoExtractHere.Checked;
        public string ExtractDestination { get => txtExtractDest.Text; set => txtExtractDest.Text = value; }
    }
}
