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
        internal ToolStripMenuItem mnuAlgoAES128;
        internal ToolStripMenuItem mnuAlgoAES192;
        internal ToolStripMenuItem mnuAlgoAES256;


        internal ToolStripButton btnComment;   // NEW
        internal ToolStripButton btnWizard;    // NEW

        internal SplitContainer splitRight;           // NEW
        internal InfoPane infoPane;                   // NEW

        internal ToolStripButton btnToggleInfo;   // NEW


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
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            toolStripMenuItem4 = new ToolStripMenuItem();
            btnOpenFolder = new ToolStripButton();
            btnTogglePreview = new ToolStripButton();
            btnComment = new ToolStripButton();
            btnWizard = new ToolStripButton();
            btnToggleInfo = new ToolStripButton();
            lblSearch = new ToolStripLabel();
            txtSearch = new ToolStripTextBox();
            btnInfo = new ToolStripButton();
            btnTest = new ToolStripButton();
            btnSettings = new ToolStripButton();
            btnEncryptAdd = new ToolStripDropDownButton();
            mnuEncryptNew = new ToolStripMenuItem();
            mnuSetAddPassword = new ToolStripMenuItem();
            mnuAlgoZipCrypto = new ToolStripMenuItem();
            mnuAlgoAES128 = new ToolStripMenuItem();
            mnuAlgoAES192 = new ToolStripMenuItem();
            mnuAlgoAES256 = new ToolStripMenuItem();
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
            splitRight = new SplitContainer();
            infoPane = new InfoPane();
            brutalGradientPanel1 = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            panel1 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            buttonInfoPane = new Button();
            buttonFilePreview = new Button();
            buttonArchiveInfo = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            buttonSearch = new Button();
            buttonTest = new Button();
            buttonWizard = new Button();
            panel6 = new Panel();
            buttonComment = new Button();
            buttonOpenFolder = new Button();
            buttonExtract = new YourNamespace.DropDownButton();
            PanelDropExtract = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            button11 = new Button();
            button10 = new Button();
            button9 = new Button();
            button4 = new Button();
            buttonAddFolder = new Button();
            buttonAddFiles = new Button();
            buttonHome = new Button();
            panelSearch = new Panel();
            tableLayoutPanel3 = new TableLayoutPanel();
            buttonCloseSearch = new Button();
            pictureBox2 = new PictureBox();
            textBoxSearch = new TextBox();
            brutalGradientPanel2 = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            toolStrip.SuspendLayout();
            ((ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            cmsViewer.SuspendLayout();
            statusStrip.SuspendLayout();
            ((ISupportInitialize)splitRight).BeginInit();
            splitRight.Panel1.SuspendLayout();
            splitRight.Panel2.SuspendLayout();
            splitRight.SuspendLayout();
            brutalGradientPanel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            PanelDropExtract.SuspendLayout();
            panelSearch.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((ISupportInitialize)pictureBox2).BeginInit();
            brutalGradientPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            toolStrip.BackColor = Color.Transparent;
            toolStrip.Dock = DockStyle.None;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Items.AddRange(new ToolStripItem[] { btnBackHome, btnAddFiles, btnAddFolder, btnExtractSplit, btnOpenFolder, btnTogglePreview, btnComment, btnWizard, btnToggleInfo, lblSearch, txtSearch, btnInfo, btnTest, btnSettings });
            toolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip.Location = new Point(833, 17);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(858, 84);
            toolStrip.TabIndex = 2;
            toolStrip.Visible = false;
            toolStrip.ItemClicked += toolStrip_ItemClicked;
            // 
            // btnBackHome
            // 
            btnBackHome.AutoSize = false;
            btnBackHome.Name = "btnBackHome";
            btnBackHome.Size = new Size(60, 81);
            btnBackHome.Text = "Home";
            // 
            // btnAddFiles
            // 
            btnAddFiles.Name = "btnAddFiles";
            btnAddFiles.Size = new Size(66, 81);
            btnAddFiles.Text = "Add files…";
            // 
            // btnAddFolder
            // 
            btnAddFolder.BackColor = Color.Transparent;
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Size = new Size(76, 81);
            btnAddFolder.Text = "Add folder…";
            // 
            // btnExtractSplit
            // 
            btnExtractSplit.BackColor = Color.Transparent;
            btnExtractSplit.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3, toolStripMenuItem4 });
            btnExtractSplit.ForeColor = Color.White;
            btnExtractSplit.Name = "btnExtractSplit";
            btnExtractSplit.Size = new Size(58, 81);
            btnExtractSplit.Text = "Extract";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(213, 22);
            toolStripMenuItem1.Text = "Extract selected…";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(213, 22);
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
            // btnOpenFolder
            // 
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(74, 81);
            btnOpenFolder.Text = "Open folder";
            // 
            // btnTogglePreview
            // 
            btnTogglePreview.Name = "btnTogglePreview";
            btnTogglePreview.Size = new Size(52, 81);
            btnTogglePreview.Text = "Preview";
            // 
            // btnComment
            // 
            btnComment.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnComment.Name = "btnComment";
            btnComment.Size = new Size(74, 81);
            btnComment.Text = "Comment…";
            // 
            // btnWizard
            // 
            btnWizard.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnWizard.Name = "btnWizard";
            btnWizard.Size = new Size(56, 81);
            btnWizard.Text = "Wizard…";
            // 
            // btnToggleInfo
            // 
            btnToggleInfo.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnToggleInfo.Name = "btnToggleInfo";
            btnToggleInfo.Size = new Size(75, 81);
            btnToggleInfo.Text = "Archive Info";
            // 
            // lblSearch
            // 
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(45, 81);
            lblSearch.Text = "Search:";
            // 
            // txtSearch
            // 
            txtSearch.BackColor = Color.FromArgb(32, 32, 32);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.ForeColor = SystemColors.MenuHighlight;
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(100, 84);
            txtSearch.Click += txtSearch_Click;
            // 
            // btnInfo
            // 
            btnInfo.Name = "btnInfo";
            btnInfo.Size = new Size(32, 81);
            btnInfo.Text = "Info";
            // 
            // btnTest
            // 
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(32, 81);
            btnTest.Text = "Test";
            // 
            // btnSettings
            // 
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(53, 81);
            btnSettings.Text = "Settings";
            // 
            // btnEncryptAdd
            // 
            btnEncryptAdd.DropDownItems.AddRange(new ToolStripItem[] { mnuEncryptNew, mnuSetAddPassword, mnuAlgoZipCrypto, mnuAlgoAES128, mnuAlgoAES192, mnuAlgoAES256 });
            btnEncryptAdd.Name = "btnEncryptAdd";
            btnEncryptAdd.Size = new Size(23, 23);
            btnEncryptAdd.Text = "Encrypt";
            // 
            // mnuEncryptNew
            // 
            mnuEncryptNew.CheckOnClick = true;
            mnuEncryptNew.Name = "mnuEncryptNew";
            mnuEncryptNew.Size = new Size(163, 22);
            mnuEncryptNew.Text = "Encrypt new files";
            // 
            // mnuSetAddPassword
            // 
            mnuSetAddPassword.Name = "mnuSetAddPassword";
            mnuSetAddPassword.Size = new Size(163, 22);
            mnuSetAddPassword.Text = "Set password…";
            // 
            // mnuAlgoZipCrypto
            // 
            mnuAlgoZipCrypto.Checked = true;
            mnuAlgoZipCrypto.CheckState = CheckState.Checked;
            mnuAlgoZipCrypto.Name = "mnuAlgoZipCrypto";
            mnuAlgoZipCrypto.Size = new Size(163, 22);
            mnuAlgoZipCrypto.Text = "ZipCrypto";
            // 
            // mnuAlgoAES128
            // 
            mnuAlgoAES128.Name = "mnuAlgoAES128";
            mnuAlgoAES128.Size = new Size(163, 22);
            mnuAlgoAES128.Text = "AES-128";
            // 
            // mnuAlgoAES192
            // 
            mnuAlgoAES192.Name = "mnuAlgoAES192";
            mnuAlgoAES192.Size = new Size(163, 22);
            mnuAlgoAES192.Text = "AES-192";
            // 
            // mnuAlgoAES256
            // 
            mnuAlgoAES256.Name = "mnuAlgoAES256";
            mnuAlgoAES256.Size = new Size(163, 22);
            mnuAlgoAES256.Text = "AES-256";
            // 
            // breadcrumb
            // 
            breadcrumb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            breadcrumb.BackColor = Color.Transparent;
            breadcrumb.Location = new Point(0, 2);
            breadcrumb.Name = "breadcrumb";
            breadcrumb.Padding = new Padding(8, 4, 8, 4);
            breadcrumb.Size = new Size(1000, 28);
            breadcrumb.TabIndex = 1;
            // 
            // splitMain
            // 
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 0);
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
            splitMain.Size = new Size(719, 561);
            splitMain.SplitterDistance = 331;
            splitMain.TabIndex = 0;
            // 
            // lvArchive
            // 
            lvArchive.BackColor = Color.FromArgb(25, 25, 25);
            lvArchive.BorderStyle = BorderStyle.None;
            lvArchive.Columns.AddRange(new ColumnHeader[] { colName, colSize, colPacked, colRatio, colMethod, colModified });
            lvArchive.ContextMenuStrip = cmsViewer;
            lvArchive.Dock = DockStyle.Fill;
            lvArchive.ForeColor = Color.White;
            lvArchive.FullRowSelect = true;
            lvArchive.Location = new Point(0, 0);
            lvArchive.Name = "lvArchive";
            lvArchive.Size = new Size(719, 331);
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
            previewPane.BackColor = Color.FromArgb(25, 25, 25);
            previewPane.Dock = DockStyle.Fill;
            previewPane.ForeColor = Color.White;
            previewPane.Location = new Point(0, 0);
            previewPane.Name = "previewPane";
            previewPane.Size = new Size(719, 226);
            previewPane.TabIndex = 0;
            previewPane.Load += previewPane_Load;
            // 
            // statusStrip
            // 
            statusStrip.BackColor = Color.FromArgb(16, 16, 16);
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
            // splitRight
            // 
            splitRight.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitRight.Location = new Point(0, 114);
            splitRight.Name = "splitRight";
            // 
            // splitRight.Panel1
            // 
            splitRight.Panel1.Controls.Add(splitMain);
            // 
            // splitRight.Panel2
            // 
            splitRight.Panel2.Controls.Add(infoPane);
            splitRight.Size = new Size(1000, 561);
            splitRight.SplitterDistance = 719;
            splitRight.TabIndex = 1;
            // 
            // infoPane
            // 
            infoPane.AutoScroll = true;
            infoPane.BackColor = Color.FromArgb(25, 25, 25);
            infoPane.Dock = DockStyle.Fill;
            infoPane.EnsureTempProvider = null;
            infoPane.ForeColor = Color.White;
            infoPane.Location = new Point(0, 0);
            infoPane.Name = "infoPane";
            infoPane.Size = new Size(277, 561);
            infoPane.TabIndex = 0;
            // 
            // brutalGradientPanel1
            // 
            brutalGradientPanel1.BackColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel1.Controls.Add(panel1);
            brutalGradientPanel1.Controls.Add(tableLayoutPanel1);
            brutalGradientPanel1.Controls.Add(tableLayoutPanel2);
            brutalGradientPanel1.Controls.Add(panel6);
            brutalGradientPanel1.Controls.Add(toolStrip);
            brutalGradientPanel1.Controls.Add(buttonComment);
            brutalGradientPanel1.Controls.Add(buttonOpenFolder);
            brutalGradientPanel1.Controls.Add(buttonExtract);
            brutalGradientPanel1.Controls.Add(buttonAddFolder);
            brutalGradientPanel1.Controls.Add(buttonAddFiles);
            brutalGradientPanel1.Controls.Add(buttonHome);
            brutalGradientPanel1.Controls.Add(panelSearch);
            brutalGradientPanel1.Dock = DockStyle.Top;
            brutalGradientPanel1.EndColor = Color.FromArgb(64, 64, 64);
            brutalGradientPanel1.ForeColor = Color.White;
            brutalGradientPanel1.GlowCenterMaxOpacity = 200;
            brutalGradientPanel1.GlowCenterMinOpacity = 50;
            brutalGradientPanel1.GlowMinSurroundOpacity = 30;
            brutalGradientPanel1.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            brutalGradientPanel1.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            brutalGradientPanel1.Location = new Point(0, 0);
            brutalGradientPanel1.Margin = new Padding(0);
            brutalGradientPanel1.MouseEvents = true;
            brutalGradientPanel1.Name = "brutalGradientPanel1";
            brutalGradientPanel1.Size = new Size(1000, 84);
            brutalGradientPanel1.StartColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel1.TabIndex = 4;
            brutalGradientPanel1.Paint += brutalGradientPanel1_Paint;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(32, 32, 32);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(740, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(0, 0, 0, 2);
            panel1.Size = new Size(2, 84);
            panel1.TabIndex = 34;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.Transparent;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(buttonInfoPane, 0, 1);
            tableLayoutPanel1.Controls.Add(buttonFilePreview, 0, 0);
            tableLayoutPanel1.Controls.Add(buttonArchiveInfo, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Left;
            tableLayoutPanel1.Location = new Point(626, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Size = new Size(114, 84);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // buttonInfoPane
            // 
            buttonInfoPane.BackColor = Color.Transparent;
            buttonInfoPane.Dock = DockStyle.Fill;
            buttonInfoPane.FlatAppearance.BorderSize = 0;
            buttonInfoPane.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonInfoPane.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonInfoPane.FlatStyle = FlatStyle.Flat;
            buttonInfoPane.Location = new Point(3, 31);
            buttonInfoPane.Name = "buttonInfoPane";
            buttonInfoPane.Size = new Size(108, 22);
            buttonInfoPane.TabIndex = 11;
            buttonInfoPane.Text = "Info Pane";
            buttonInfoPane.TextAlign = ContentAlignment.BottomCenter;
            buttonInfoPane.UseVisualStyleBackColor = false;
            buttonInfoPane.Click += buttonInfoPane_Click;
            // 
            // buttonFilePreview
            // 
            buttonFilePreview.BackColor = Color.Transparent;
            buttonFilePreview.Dock = DockStyle.Fill;
            buttonFilePreview.FlatAppearance.BorderSize = 0;
            buttonFilePreview.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonFilePreview.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonFilePreview.FlatStyle = FlatStyle.Flat;
            buttonFilePreview.Location = new Point(3, 3);
            buttonFilePreview.Name = "buttonFilePreview";
            buttonFilePreview.Size = new Size(108, 22);
            buttonFilePreview.TabIndex = 10;
            buttonFilePreview.Text = "File Preview";
            buttonFilePreview.TextAlign = ContentAlignment.BottomCenter;
            buttonFilePreview.UseVisualStyleBackColor = false;
            buttonFilePreview.Click += buttonFilePreview_Click;
            // 
            // buttonArchiveInfo
            // 
            buttonArchiveInfo.BackColor = Color.Transparent;
            buttonArchiveInfo.Dock = DockStyle.Fill;
            buttonArchiveInfo.FlatAppearance.BorderSize = 0;
            buttonArchiveInfo.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonArchiveInfo.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonArchiveInfo.FlatStyle = FlatStyle.Flat;
            buttonArchiveInfo.Location = new Point(3, 59);
            buttonArchiveInfo.Name = "buttonArchiveInfo";
            buttonArchiveInfo.Size = new Size(108, 22);
            buttonArchiveInfo.TabIndex = 9;
            buttonArchiveInfo.Text = "Archive Info";
            buttonArchiveInfo.TextAlign = ContentAlignment.BottomCenter;
            buttonArchiveInfo.UseVisualStyleBackColor = false;
            buttonArchiveInfo.Click += buttonArchiveInfo_Click;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.BackColor = Color.Transparent;
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(buttonSearch, 0, 2);
            tableLayoutPanel2.Controls.Add(buttonTest, 0, 1);
            tableLayoutPanel2.Controls.Add(buttonWizard, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Left;
            tableLayoutPanel2.Location = new Point(512, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.Size = new Size(114, 84);
            tableLayoutPanel2.TabIndex = 12;
            // 
            // buttonSearch
            // 
            buttonSearch.BackColor = Color.Transparent;
            buttonSearch.Dock = DockStyle.Fill;
            buttonSearch.FlatAppearance.BorderSize = 0;
            buttonSearch.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonSearch.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonSearch.FlatStyle = FlatStyle.Flat;
            buttonSearch.Location = new Point(3, 59);
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Size = new Size(108, 22);
            buttonSearch.TabIndex = 11;
            buttonSearch.Text = "Search";
            buttonSearch.TextAlign = ContentAlignment.BottomCenter;
            buttonSearch.UseVisualStyleBackColor = false;
            buttonSearch.Click += button1_Click_1;
            // 
            // buttonTest
            // 
            buttonTest.BackColor = Color.Transparent;
            buttonTest.Dock = DockStyle.Fill;
            buttonTest.FlatAppearance.BorderSize = 0;
            buttonTest.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonTest.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonTest.FlatStyle = FlatStyle.Flat;
            buttonTest.Location = new Point(3, 31);
            buttonTest.Name = "buttonTest";
            buttonTest.Size = new Size(108, 22);
            buttonTest.TabIndex = 10;
            buttonTest.Text = "Test";
            buttonTest.TextAlign = ContentAlignment.BottomCenter;
            buttonTest.UseVisualStyleBackColor = false;
            buttonTest.Click += buttonTest_Click;
            // 
            // buttonWizard
            // 
            buttonWizard.BackColor = Color.Transparent;
            buttonWizard.Dock = DockStyle.Fill;
            buttonWizard.FlatAppearance.BorderSize = 0;
            buttonWizard.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonWizard.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonWizard.FlatStyle = FlatStyle.Flat;
            buttonWizard.Location = new Point(3, 3);
            buttonWizard.Name = "buttonWizard";
            buttonWizard.Size = new Size(108, 22);
            buttonWizard.TabIndex = 8;
            buttonWizard.Text = "Wizard";
            buttonWizard.TextAlign = ContentAlignment.BottomCenter;
            buttonWizard.UseVisualStyleBackColor = false;
            buttonWizard.Click += buttonWizard_Click;
            // 
            // panel6
            // 
            panel6.BackColor = Color.FromArgb(32, 32, 32);
            panel6.Dock = DockStyle.Left;
            panel6.Location = new Point(510, 0);
            panel6.Name = "panel6";
            panel6.Padding = new Padding(0, 0, 0, 2);
            panel6.Size = new Size(2, 84);
            panel6.TabIndex = 33;
            // 
            // buttonComment
            // 
            buttonComment.BackColor = Color.Transparent;
            buttonComment.Dock = DockStyle.Left;
            buttonComment.FlatAppearance.BorderSize = 0;
            buttonComment.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonComment.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonComment.FlatStyle = FlatStyle.Flat;
            buttonComment.Location = new Point(425, 0);
            buttonComment.Name = "buttonComment";
            buttonComment.Size = new Size(85, 84);
            buttonComment.TabIndex = 6;
            buttonComment.Text = "Comment";
            buttonComment.TextAlign = ContentAlignment.BottomCenter;
            buttonComment.UseVisualStyleBackColor = false;
            buttonComment.Click += button5_Click;
            // 
            // buttonOpenFolder
            // 
            buttonOpenFolder.BackColor = Color.Transparent;
            buttonOpenFolder.Dock = DockStyle.Left;
            buttonOpenFolder.FlatAppearance.BorderSize = 0;
            buttonOpenFolder.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonOpenFolder.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonOpenFolder.FlatStyle = FlatStyle.Flat;
            buttonOpenFolder.Location = new Point(340, 0);
            buttonOpenFolder.Name = "buttonOpenFolder";
            buttonOpenFolder.Size = new Size(85, 84);
            buttonOpenFolder.TabIndex = 4;
            buttonOpenFolder.Text = "Open Folder";
            buttonOpenFolder.TextAlign = ContentAlignment.BottomCenter;
            buttonOpenFolder.UseVisualStyleBackColor = false;
            buttonOpenFolder.Click += button3_Click;
            // 
            // buttonExtract
            // 
            buttonExtract.ArrowColor = Color.Empty;
            buttonExtract.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonExtract.BackColor = Color.Transparent;
            buttonExtract.Dock = DockStyle.Left;
            buttonExtract.DropDownControl = PanelDropExtract;
            buttonExtract.DropDownOffset = new Point(189, 0);
            buttonExtract.FlatAppearance.BorderSize = 0;
            buttonExtract.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonExtract.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonExtract.FlatStyle = FlatStyle.Flat;
            buttonExtract.Location = new Point(255, 0);
            buttonExtract.Mode = YourNamespace.DropDownMode.Split;
            buttonExtract.Name = "buttonExtract";
            buttonExtract.Size = new Size(85, 84);
            buttonExtract.TabIndex = 3;
            buttonExtract.Text = "Extract";
            buttonExtract.TextAlign = ContentAlignment.BottomCenter;
            buttonExtract.UseVisualStyleBackColor = false;
            buttonExtract.DropDownClicked += dropDownButton1_DropDownClicked;
            buttonExtract.DropDownOpening += dropDownButton1_DropDownOpening;
            buttonExtract.Click += buttonExtract_Click;
            // 
            // PanelDropExtract
            // 
            PanelDropExtract.Controls.Add(button11);
            PanelDropExtract.Controls.Add(button10);
            PanelDropExtract.Controls.Add(button9);
            PanelDropExtract.Controls.Add(button4);
            PanelDropExtract.EndColor = Color.FromArgb(64, 64, 64);
            PanelDropExtract.GlowCenterMaxOpacity = 200;
            PanelDropExtract.GlowCenterMinOpacity = 50;
            PanelDropExtract.GlowMinSurroundOpacity = 30;
            PanelDropExtract.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            PanelDropExtract.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            PanelDropExtract.Location = new Point(254, 84);
            PanelDropExtract.MouseEvents = true;
            PanelDropExtract.Name = "PanelDropExtract";
            PanelDropExtract.Size = new Size(189, 113);
            PanelDropExtract.StartColor = Color.FromArgb(32, 32, 32);
            PanelDropExtract.TabIndex = 6;
            PanelDropExtract.Visible = false;
            // 
            // button11
            // 
            button11.BackColor = Color.Transparent;
            button11.Dock = DockStyle.Top;
            button11.FlatAppearance.BorderSize = 0;
            button11.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            button11.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            button11.FlatStyle = FlatStyle.Flat;
            button11.Location = new Point(0, 84);
            button11.Name = "button11";
            button11.Size = new Size(189, 28);
            button11.TabIndex = 13;
            button11.Text = "Choose Folder";
            button11.TextAlign = ContentAlignment.MiddleLeft;
            button11.UseVisualStyleBackColor = false;
            button11.Click += button11_Click;
            // 
            // button10
            // 
            button10.BackColor = Color.Transparent;
            button10.Dock = DockStyle.Top;
            button10.FlatAppearance.BorderSize = 0;
            button10.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            button10.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            button10.FlatStyle = FlatStyle.Flat;
            button10.Location = new Point(0, 56);
            button10.Name = "button10";
            button10.Size = new Size(189, 28);
            button10.TabIndex = 12;
            button10.Text = "Extract to \"Archive Name\"/";
            button10.TextAlign = ContentAlignment.MiddleLeft;
            button10.UseVisualStyleBackColor = false;
            button10.Click += button10_Click;
            // 
            // button9
            // 
            button9.BackColor = Color.Transparent;
            button9.Dock = DockStyle.Top;
            button9.FlatAppearance.BorderSize = 0;
            button9.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            button9.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            button9.FlatStyle = FlatStyle.Flat;
            button9.Location = new Point(0, 28);
            button9.Name = "button9";
            button9.Size = new Size(189, 28);
            button9.TabIndex = 11;
            button9.Text = "Extract Here";
            button9.TextAlign = ContentAlignment.MiddleLeft;
            button9.UseVisualStyleBackColor = false;
            button9.Click += button9_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.Transparent;
            button4.Dock = DockStyle.Top;
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            button4.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            button4.FlatStyle = FlatStyle.Flat;
            button4.Location = new Point(0, 0);
            button4.Name = "button4";
            button4.Size = new Size(189, 28);
            button4.TabIndex = 10;
            button4.Text = "Extract selected";
            button4.TextAlign = ContentAlignment.MiddleLeft;
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // buttonAddFolder
            // 
            buttonAddFolder.BackColor = Color.Transparent;
            buttonAddFolder.Dock = DockStyle.Left;
            buttonAddFolder.FlatAppearance.BorderSize = 0;
            buttonAddFolder.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonAddFolder.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonAddFolder.FlatStyle = FlatStyle.Flat;
            buttonAddFolder.Location = new Point(170, 0);
            buttonAddFolder.Name = "buttonAddFolder";
            buttonAddFolder.Size = new Size(85, 84);
            buttonAddFolder.TabIndex = 1;
            buttonAddFolder.Text = "Add Folder";
            buttonAddFolder.TextAlign = ContentAlignment.BottomCenter;
            buttonAddFolder.UseVisualStyleBackColor = false;
            buttonAddFolder.Click += button2_Click;
            // 
            // buttonAddFiles
            // 
            buttonAddFiles.BackColor = Color.Transparent;
            buttonAddFiles.Dock = DockStyle.Left;
            buttonAddFiles.FlatAppearance.BorderSize = 0;
            buttonAddFiles.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonAddFiles.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonAddFiles.FlatStyle = FlatStyle.Flat;
            buttonAddFiles.Location = new Point(85, 0);
            buttonAddFiles.Name = "buttonAddFiles";
            buttonAddFiles.Size = new Size(85, 84);
            buttonAddFiles.TabIndex = 11;
            buttonAddFiles.Text = "Add Files";
            buttonAddFiles.TextAlign = ContentAlignment.BottomCenter;
            buttonAddFiles.UseVisualStyleBackColor = false;
            // 
            // buttonHome
            // 
            buttonHome.BackColor = Color.Transparent;
            buttonHome.Dock = DockStyle.Left;
            buttonHome.FlatAppearance.BorderSize = 0;
            buttonHome.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonHome.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonHome.FlatStyle = FlatStyle.Flat;
            buttonHome.Location = new Point(0, 0);
            buttonHome.Name = "buttonHome";
            buttonHome.Size = new Size(85, 84);
            buttonHome.TabIndex = 0;
            buttonHome.Text = "Home";
            buttonHome.TextAlign = ContentAlignment.BottomCenter;
            buttonHome.UseVisualStyleBackColor = false;
            buttonHome.Click += button1_Click;
            // 
            // panelSearch
            // 
            panelSearch.Controls.Add(tableLayoutPanel3);
            panelSearch.Location = new Point(510, 54);
            panelSearch.Name = "panelSearch";
            panelSearch.Size = new Size(275, 29);
            panelSearch.TabIndex = 7;
            panelSearch.Visible = false;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 27F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28F));
            tableLayoutPanel3.Controls.Add(buttonCloseSearch, 2, 0);
            tableLayoutPanel3.Controls.Add(pictureBox2, 0, 0);
            tableLayoutPanel3.Controls.Add(textBoxSearch, 1, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(275, 29);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // buttonCloseSearch
            // 
            buttonCloseSearch.BackColor = Color.Transparent;
            buttonCloseSearch.Dock = DockStyle.Fill;
            buttonCloseSearch.FlatAppearance.BorderSize = 0;
            buttonCloseSearch.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonCloseSearch.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonCloseSearch.FlatStyle = FlatStyle.Flat;
            buttonCloseSearch.Location = new Point(250, 3);
            buttonCloseSearch.Name = "buttonCloseSearch";
            buttonCloseSearch.Size = new Size(22, 23);
            buttonCloseSearch.TabIndex = 12;
            buttonCloseSearch.Text = "X";
            buttonCloseSearch.TextAlign = ContentAlignment.BottomCenter;
            buttonCloseSearch.UseVisualStyleBackColor = false;
            buttonCloseSearch.Click += buttonCloseSearch_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.Dock = DockStyle.Fill;
            pictureBox2.Location = new Point(3, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(21, 23);
            pictureBox2.TabIndex = 2;
            pictureBox2.TabStop = false;
            // 
            // textBoxSearch
            // 
            textBoxSearch.BackColor = Color.FromArgb(32, 32, 32);
            textBoxSearch.BorderStyle = BorderStyle.FixedSingle;
            textBoxSearch.Dock = DockStyle.Fill;
            textBoxSearch.ForeColor = Color.White;
            textBoxSearch.Location = new Point(30, 3);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.Size = new Size(214, 23);
            textBoxSearch.TabIndex = 0;
            textBoxSearch.TextChanged += textBoxSearch_TextChanged;
            // 
            // brutalGradientPanel2
            // 
            brutalGradientPanel2.BackColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel2.Controls.Add(breadcrumb);
            brutalGradientPanel2.Dock = DockStyle.Top;
            brutalGradientPanel2.EndColor = Color.FromArgb(64, 64, 64);
            brutalGradientPanel2.ForeColor = Color.White;
            brutalGradientPanel2.GlowCenterMaxOpacity = 200;
            brutalGradientPanel2.GlowCenterMinOpacity = 50;
            brutalGradientPanel2.GlowMinSurroundOpacity = 30;
            brutalGradientPanel2.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            brutalGradientPanel2.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            brutalGradientPanel2.Location = new Point(0, 84);
            brutalGradientPanel2.Margin = new Padding(0);
            brutalGradientPanel2.MouseEvents = true;
            brutalGradientPanel2.Name = "brutalGradientPanel2";
            brutalGradientPanel2.Size = new Size(1000, 30);
            brutalGradientPanel2.StartColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel2.TabIndex = 5;
            // 
            // ViewerView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(32, 32, 32);
            Controls.Add(splitRight);
            Controls.Add(statusStrip);
            Controls.Add(brutalGradientPanel2);
            Controls.Add(PanelDropExtract);
            Controls.Add(brutalGradientPanel1);
            ForeColor = Color.White;
            Name = "ViewerView";
            Size = new Size(1000, 700);
            Load += ViewerView_Load;
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            cmsViewer.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            splitRight.Panel1.ResumeLayout(false);
            splitRight.Panel2.ResumeLayout(false);
            ((ISupportInitialize)splitRight).EndInit();
            splitRight.ResumeLayout(false);
            brutalGradientPanel1.ResumeLayout(false);
            brutalGradientPanel1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            PanelDropExtract.ResumeLayout(false);
            panelSearch.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ((ISupportInitialize)pictureBox2).EndInit();
            brutalGradientPanel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel1;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel2;
        private Button buttonAddFolder;
        private Button buttonHome;
        private YourNamespace.DropDownButton buttonExtract;
        private Button buttonComment;
        private TableLayoutPanel tableLayoutPanel1;
        private Button buttonArchiveInfo;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel PanelDropExtract;
        private Button button11;
        private Button button10;
        private Button button9;
        private Button button4;
        private Button buttonAddFiles;
        private Button buttonInfoPane;
        private Button buttonFilePreview;
        private TableLayoutPanel tableLayoutPanel2;
        private Button buttonSearch;
        private Button buttonTest;
        private Button buttonWizard;
        private Panel panelSearch;
        private TableLayoutPanel tableLayoutPanel3;
        private TextBox textBoxSearch;
        public Button buttonOpenFolder;
        private Button buttonCloseSearch;
        private PictureBox pictureBox2;
        private Panel panel6;
        private Panel panel1;
    }
}
