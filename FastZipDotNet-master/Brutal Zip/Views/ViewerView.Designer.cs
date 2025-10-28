using System.ComponentModel;

namespace Brutal_Zip.Views
{
    partial class ViewerView
    {
        private IContainer components = null;

        internal ToolStrip toolStrip;
        internal ToolStripButton btnBackHome;
        internal ToolStripButton btnAddFiles;
        internal ToolStripButton btnAddFolder;
        internal ToolStripSplitButton btnExtractSplit;
        private ToolStripLabel lblSearch;
        internal ToolStripTextBox txtSearch;
        internal ToolStripButton btnInfo;
        internal ToolStripButton btnTest;
        internal ToolStripButton btnSettings;
        internal ToolStripButton btnOpenFolder;
        internal ToolStripButton btnTogglePreview;

        internal FlowLayoutPanel breadcrumb;
        internal SplitContainer splitMain;
        internal ListView lvArchive;
        private ColumnHeader colName;
        private ColumnHeader colSize;
        private ColumnHeader colPacked;
        private ColumnHeader colRatio;
        private ColumnHeader colMethod;
        private ColumnHeader colModified;

        internal StatusStrip statusStrip;
        internal ToolStripStatusLabel lblStatus;

        internal PreviewPane previewPane;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.toolStrip = new ToolStrip();
            this.btnBackHome = new ToolStripButton();
            this.btnAddFiles = new ToolStripButton();
            this.btnAddFolder = new ToolStripButton();
            this.btnExtractSplit = new ToolStripSplitButton();
            this.lblSearch = new ToolStripLabel();
            this.txtSearch = new ToolStripTextBox();
            this.btnInfo = new ToolStripButton();
            this.btnTest = new ToolStripButton();
            this.btnSettings = new ToolStripButton();
            this.btnOpenFolder = new ToolStripButton();
            this.btnTogglePreview = new ToolStripButton();

            this.breadcrumb = new FlowLayoutPanel();
            this.splitMain = new SplitContainer();
            this.lvArchive = new ListView();
            this.colName = new ColumnHeader();
            this.colSize = new ColumnHeader();
            this.colPacked = new ColumnHeader();
            this.colRatio = new ColumnHeader();
            this.colMethod = new ColumnHeader();
            this.colModified = new ColumnHeader();
            this.previewPane = new PreviewPane();

            this.statusStrip = new StatusStrip();
            this.lblStatus = new ToolStripStatusLabel();

            ((ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.SuspendLayout();

            // ViewerView
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Name = "ViewerView";
            this.Size = new Size(1000, 700);

            // toolStrip
            this.toolStrip.Dock = DockStyle.Top;

            this.btnBackHome.Text = "Home";
            this.btnAddFiles.Text = "Add files…";
            this.btnAddFolder.Text = "Add folder…";
            this.btnExtractSplit.Text = "Extract";
            this.btnExtractSplit.DropDownItems.Add("Extract selected…");
            this.btnExtractSplit.DropDownItems.Add("Extract here");
            this.btnExtractSplit.DropDownItems.Add("Extract to “ArchiveName”/");
            this.btnExtractSplit.DropDownItems.Add("Choose folder…");

            this.btnOpenFolder.Text = "Open folder";
            this.btnTogglePreview.Text = "Preview";

            this.lblSearch.Text = "Search:";

            this.btnInfo.Text = "Info";
            this.btnTest.Text = "Test";
            this.btnSettings.Text = "Settings";

            this.toolStrip.Items.AddRange(new ToolStripItem[]
            {
            this.btnBackHome, new ToolStripSeparator(),
            this.btnAddFiles, this.btnAddFolder, this.btnExtractSplit, new ToolStripSeparator(),
            this.btnOpenFolder, this.btnTogglePreview, new ToolStripSeparator(),
            this.lblSearch, this.txtSearch, new ToolStripSeparator(),
            this.btnInfo, this.btnTest, this.btnSettings
            });

            // breadcrumb
            this.breadcrumb.Dock = DockStyle.Top;
            this.breadcrumb.Height = 28;
            this.breadcrumb.Padding = new Padding(8, 4, 8, 4);

            // splitMain
            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.Orientation = Orientation.Horizontal;
            this.splitMain.SplitterDistance = 420;
            this.splitMain.Panel2Collapsed = true;

            // lvArchive
            this.lvArchive.Dock = DockStyle.Fill;
            this.lvArchive.View = View.Details;
            this.lvArchive.FullRowSelect = true;
            this.lvArchive.HideSelection = false;
            this.lvArchive.VirtualMode = true;

            this.colName.Text = "Name";
            this.colName.Width = 420;
            this.colSize.Text = "Size";
            this.colSize.Width = 120;
            this.colSize.TextAlign = HorizontalAlignment.Right;
            this.colPacked.Text = "Packed";
            this.colPacked.Width = 120;
            this.colPacked.TextAlign = HorizontalAlignment.Right;
            this.colRatio.Text = "Ratio";
            this.colRatio.Width = 80;
            this.colRatio.TextAlign = HorizontalAlignment.Right;
            this.colMethod.Text = "Method";
            this.colMethod.Width = 80;
            this.colModified.Text = "Modified";
            this.colModified.Width = 160;

            this.lvArchive.Columns.AddRange(new ColumnHeader[] {
            this.colName, this.colSize, this.colPacked, this.colRatio, this.colMethod, this.colModified
        });

            // previewPane
            this.previewPane.Dock = DockStyle.Fill;

            this.splitMain.Panel1.Controls.Add(this.lvArchive);
            this.splitMain.Panel2.Controls.Add(this.previewPane);

            // statusStrip
            this.statusStrip.Dock = DockStyle.Bottom;
            this.statusStrip.Items.Add(this.lblStatus);

            // Add controls
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.breadcrumb);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);

            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.Panel1.ResumeLayout(false);
            ((ISupportInitialize)(this.splitMain)).EndInit();

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
