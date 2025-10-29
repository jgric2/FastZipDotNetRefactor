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
            toolStrip = new ToolStrip();
            btnBackHome = new ToolStripButton();
            btnAddFiles = new ToolStripButton();
            btnAddFolder = new ToolStripButton();
            btnExtractSplit = new ToolStripSplitButton();
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
            previewPane = new PreviewPane();
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            toolStrip.SuspendLayout();
            ((ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            toolStrip.Items.AddRange(new ToolStripItem[] { btnBackHome, btnAddFiles, btnAddFolder, btnExtractSplit, btnOpenFolder, btnTogglePreview, lblSearch, txtSearch, btnInfo, btnTest, btnSettings });
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
            btnExtractSplit.Name = "btnExtractSplit";
            btnExtractSplit.Size = new Size(58, 22);
            this.btnExtractSplit.Text = "Extract";
            this.btnExtractSplit.DropDownItems.Add("Extract selected…");
            this.btnExtractSplit.DropDownItems.Add("Extract here");
            this.btnExtractSplit.DropDownItems.Add("Extract to “ArchiveName”/");
            this.btnExtractSplit.DropDownItems.Add("Choose folder…");
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
            splitMain.Panel2Collapsed = true;
            splitMain.Size = new Size(1000, 625);
            splitMain.SplitterDistance = 71;
            splitMain.TabIndex = 0;
            // 
            // lvArchive
            // 
            lvArchive.Columns.AddRange(new ColumnHeader[] { colName, colSize, colPacked, colRatio, colMethod, colModified });
            lvArchive.Dock = DockStyle.Fill;
            lvArchive.FullRowSelect = true;
            lvArchive.Location = new Point(0, 0);
            lvArchive.Name = "lvArchive";
            lvArchive.Size = new Size(1000, 625);
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
            // previewPane
            // 
            previewPane.Dock = DockStyle.Fill;
            previewPane.Location = new Point(0, 0);
            previewPane.Name = "previewPane";
            previewPane.Size = new Size(150, 25);
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
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
