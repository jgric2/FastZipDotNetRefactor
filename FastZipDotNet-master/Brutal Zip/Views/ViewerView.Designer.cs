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

        internal ContextMenuStrip cmsViewer;               // NEW
        internal ToolStripMenuItem mnuCopyPaths;           // NEW
        internal ToolStripMenuItem mnuCopyNames;           // NEW

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


        internal ToolStripDropDownButton btnEncryptAdd;
        internal ToolStripMenuItem mnuEncryptNew;
        internal ToolStripMenuItem mnuSetAddPassword;
        internal ToolStripMenuItem mnuAlgoZipCrypto;
        internal ToolStripMenuItem mnuAlgoAES256;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            toolStrip = new ToolStrip();
            btnBackHome = new ToolStripButton();
            btnAddFiles = new ToolStripButton();
            btnAddFolder = new ToolStripButton();
            btnExtractSplit = new ToolStripSplitButton();
            // NEW encryption dropdown
            btnEncryptAdd = new ToolStripDropDownButton();
            mnuEncryptNew = new ToolStripMenuItem();
            mnuSetAddPassword = new ToolStripMenuItem();
            mnuAlgoZipCrypto = new ToolStripMenuItem();
            mnuAlgoAES256 = new ToolStripMenuItem();

            btnOpenFolder = new ToolStripButton();
            btnTogglePreview = new ToolStripButton();
            lblSearch = new ToolStripLabel();
            txtSearch = new ToolStripTextBox();
            btnInfo = new ToolStripButton();
            btnTest = new ToolStripButton();
            btnSettings = new ToolStripButton();
            breadcrumb = new FlowLayoutPanel();
            splitMain = new SplitContainer();
            lvArchive = new ListView();
            colName = new ColumnHeader();
            colSize = new ColumnHeader();
            colPacked = new ColumnHeader();
            colRatio = new ColumnHeader();
            colMethod = new ColumnHeader();
            colModified = new ColumnHeader();
            cmsViewer = new ContextMenuStrip(components);
            mnuCopyPaths = new ToolStripMenuItem();
            mnuCopyNames = new ToolStripMenuItem();
            previewPane = new PreviewPane();
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            toolStripMenuItem4 = new ToolStripMenuItem();
            toolStrip.SuspendLayout();
            ((ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            cmsViewer.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            // toolStrip
            toolStrip.Items.AddRange(new ToolStripItem[] 
            {
                btnBackHome, btnAddFiles, btnAddFolder, btnExtractSplit,
                btnEncryptAdd,   // NEW here
                btnOpenFolder, btnTogglePreview,
                lblSearch, txtSearch, btnInfo, btnTest, btnSettings
            });

            // NEW: btnEncryptAdd dropdown and items
            btnEncryptAdd.Text = "Encrypt";
            btnEncryptAdd.DropDownItems.AddRange(new ToolStripItem[] 
            {
                mnuEncryptNew,
                mnuSetAddPassword,
                mnuAlgoZipCrypto,
                mnuAlgoAES256
            });

            mnuEncryptNew.Text = "Encrypt new files";
            mnuEncryptNew.CheckOnClick = true;

            mnuSetAddPassword.Text = "Set password…";

            mnuAlgoZipCrypto.Text = "ZipCrypto";
            mnuAlgoZipCrypto.Checked = true;

            mnuAlgoAES256.Text = "AES-256 (soon)";
            mnuAlgoAES256.Enabled = false;



            toolStrip.Location = new Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(1000, 25);
            toolStrip.TabIndex = 2;
            toolStrip.ItemClicked += toolStrip_ItemClicked;
            // 
            // btnBackHome
            // 
            btnBackHome.Name = "btnBackHome";
            btnBackHome.Size = new Size(44, 22);
            btnBackHome.Text = "Home";
            // 
            // btnAddFiles
            // 
            btnAddFiles.Name = "btnAddFiles";
            btnAddFiles.Size = new Size(66, 22);
            btnAddFiles.Text = "Add files…";
            // 
            // btnAddFolder
            // 
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Size = new Size(76, 22);
            btnAddFolder.Text = "Add folder…";
            // 
            // btnExtractSplit
            // 
            btnExtractSplit.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3, toolStripMenuItem4 });
            btnExtractSplit.Name = "btnExtractSplit";
            btnExtractSplit.Size = new Size(58, 22);
            btnExtractSplit.Text = "Extract";
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(74, 22);
            btnOpenFolder.Text = "Open folder";
            // 
            // btnTogglePreview
            // 
            btnTogglePreview.Name = "btnTogglePreview";
            btnTogglePreview.Size = new Size(52, 22);
            btnTogglePreview.Text = "Preview";
            // 
            // lblSearch
            // 
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(45, 22);
            lblSearch.Text = "Search:";
            // 
            // txtSearch
            // 
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(100, 25);
            // 
            // btnInfo
            // 
            btnInfo.Name = "btnInfo";
            btnInfo.Size = new Size(32, 22);
            btnInfo.Text = "Info";
            // 
            // btnTest
            // 
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(32, 22);
            btnTest.Text = "Test";
            // 
            // btnSettings
            // 
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(53, 22);
            btnSettings.Text = "Settings";
            // 
            // breadcrumb
            // 
            breadcrumb.Dock = DockStyle.Top;
            breadcrumb.Location = new Point(0, 25);
            breadcrumb.Name = "breadcrumb";
            breadcrumb.Padding = new Padding(8, 4, 8, 4);
            breadcrumb.Size = new Size(1000, 28);
            breadcrumb.TabIndex = 1;
            // 
            // splitMain
            // 
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 53);
            splitMain.Name = "splitMain";
            splitMain.Orientation = Orientation.Horizontal;
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(lvArchive);
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(previewPane);
            splitMain.Size = new Size(1000, 625);
            splitMain.SplitterDistance = 370;
            splitMain.TabIndex = 0;
            // 
            // lvArchive
            // 
            lvArchive.Columns.AddRange(new ColumnHeader[] { colName, colSize, colPacked, colRatio, colMethod, colModified });
            lvArchive.ContextMenuStrip = cmsViewer;
            lvArchive.Dock = DockStyle.Fill;
            lvArchive.FullRowSelect = true;
            lvArchive.Location = new Point(0, 0);
            lvArchive.Name = "lvArchive";
            lvArchive.Size = new Size(1000, 370);
            lvArchive.TabIndex = 0;
            lvArchive.UseCompatibleStateImageBehavior = false;
            lvArchive.View = View.Details;
            lvArchive.VirtualMode = true;
            // 
            // colName
            // 
            colName.Text = "Name";
            colName.Width = 420;
            // 
            // colSize
            // 
            colSize.Text = "Size";
            colSize.TextAlign = HorizontalAlignment.Right;
            colSize.Width = 120;
            // 
            // colPacked
            // 
            colPacked.Text = "Packed";
            colPacked.TextAlign = HorizontalAlignment.Right;
            colPacked.Width = 120;
            // 
            // colRatio
            // 
            colRatio.Text = "Ratio";
            colRatio.TextAlign = HorizontalAlignment.Right;
            colRatio.Width = 80;
            // 
            // colMethod
            // 
            colMethod.Text = "Method";
            colMethod.Width = 80;
            // 
            // colModified
            // 
            colModified.Text = "Modified";
            colModified.Width = 160;
            // 
            // cmsViewer
            // 
            cmsViewer.Items.AddRange(new ToolStripItem[] { mnuCopyPaths, mnuCopyNames });
            cmsViewer.Name = "cmsViewer";
            cmsViewer.Size = new Size(220, 48);
            // 
            // mnuCopyPaths
            // 
            mnuCopyPaths.Name = "mnuCopyPaths";
            mnuCopyPaths.Size = new Size(219, 22);
            mnuCopyPaths.Text = "Copy inside-archive path(s)";
            // 
            // mnuCopyNames
            // 
            mnuCopyNames.Name = "mnuCopyNames";
            mnuCopyNames.Size = new Size(219, 22);
            mnuCopyNames.Text = "Copy name(s)";
            // 
            // previewPane
            // 
            previewPane.Dock = DockStyle.Fill;
            previewPane.Location = new Point(0, 0);
            previewPane.Name = "previewPane";
            previewPane.Size = new Size(1000, 251);
            previewPane.TabIndex = 0;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusStrip.Location = new Point(0, 678);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1000, 22);
            statusStrip.TabIndex = 3;
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 17);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(180, 22);
            toolStripMenuItem1.Text = "Extract selected…";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(180, 22);
            toolStripMenuItem2.Text = "Extract here";
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(213, 22);
            toolStripMenuItem3.Text = "Extract to “ArchiveName”/";
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new Size(213, 22);
            toolStripMenuItem4.Text = "Choose folder…";
            // 
            // ViewerView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(splitMain);
            Controls.Add(breadcrumb);
            Controls.Add(toolStrip);
            Controls.Add(statusStrip);
            Name = "ViewerView";
            Size = new Size(1000, 700);
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            cmsViewer.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
    }
}
