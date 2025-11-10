using System.ComponentModel;

namespace Brutal_Zip
{
    partial class WizardForm
    {
        private IContainer components = null;

        // Create tab controls
        internal ListView lvCreate;
        private ColumnHeader colName;
        private ColumnHeader colType;
        private ColumnHeader colSize;
        private ColumnHeader colItems;
        private ColumnHeader colPath;

        private Button btnCreateAddFiles;
        private Button btnCreateAddFolder;

        private Label lblCreateDest;
        internal TextBox txtCreateDest;
        private Button btnCreateBrowse;

        private Label lblCreateMethod;
        private Label lblCreateLevel;
        internal NumericUpDown numCreateLevel;
        private Label lblCreateAlgo;
        private Button btnCreateSetPassword;

        private Label lblCreateThreads;
        internal Label lblCreateThreadsVal;

        internal Button btnCreateStart;

        // Extract tab controls
        private Label lblZip;
        internal TextBox txtZip;
        private Button btnZipBrowse;

        private Label lblExtractDest;
        internal TextBox txtExtractDest;
        private Button btnExtractBrowse;

        private Label lblExtractThreads;
        internal Label lblExtractThreadsVal;

        internal Button btnExtractStart;

        private Button btnClose;


        internal ContextMenuStrip cmsCreate;                 // NEW
        internal ToolStripMenuItem mnuCreateAddFiles;        // NEW
        internal ToolStripMenuItem mnuCreateAddFolder;       // NEW
        internal ToolStripSeparator sepCreate1;              // NEW
        internal ToolStripMenuItem mnuCreateRemoveSelected;  // NEW
        internal ToolStripMenuItem mnuCreateRemoveMissing;   // NEW
        internal ToolStripMenuItem mnuCreateClearAll;        // NEW

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(WizardForm));
            lvCreate = new ListView();
            colName = new ColumnHeader();
            colType = new ColumnHeader();
            colSize = new ColumnHeader();
            colItems = new ColumnHeader();
            colPath = new ColumnHeader();
            cmsCreate = new ContextMenuStrip(components);
            mnuCreateAddFiles = new ToolStripMenuItem();
            mnuCreateAddFolder = new ToolStripMenuItem();
            sepCreate1 = new ToolStripSeparator();
            mnuCreateRemoveSelected = new ToolStripMenuItem();
            mnuCreateRemoveMissing = new ToolStripMenuItem();
            mnuCreateClearAll = new ToolStripMenuItem();
            btnCreateAddFiles = new Button();
            btnCreateAddFolder = new Button();
            lblCreateDest = new Label();
            txtCreateDest = new TextBox();
            btnCreateBrowse = new Button();
            lblCreateMethod = new Label();
            lblCreateLevel = new Label();
            numCreateLevel = new NumericUpDown();
            lblCreateAlgo = new Label();
            btnCreateSetPassword = new Button();
            lblCreateThreads = new Label();
            lblCreateThreadsVal = new Label();
            btnCreateStart = new Button();
            lblZip = new Label();
            txtZip = new TextBox();
            btnZipBrowse = new Button();
            lblExtractDest = new Label();
            txtExtractDest = new TextBox();
            btnExtractBrowse = new Button();
            lblExtractThreads = new Label();
            lblExtractThreadsVal = new Label();
            btnExtractStart = new Button();
            btnClose = new Button();
            brutalGradientPanel1 = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            panel6 = new Panel();
            buttonExtractTab = new Button();
            buttonCreateTab = new Button();
            panelCreate = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            cmbCreateAlgo = new BrutalZip2025.BrutalControls.BrutalComboBox();
            cmbCreateMethod = new BrutalZip2025.BrutalControls.BrutalComboBox();
            label3 = new Label();
            chkCreateEncrypt = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label1 = new Label();
            tbCreateThreads = new BrutalZip2025.BrutalControls.BrutalTrackBar();
            panelExtract = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            label4 = new Label();
            chkExtractToFolderName = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label2 = new Label();
            tbExtractThreads = new BrutalZip2025.BrutalControls.BrutalTrackBar();
            panelFooter = new Panel();
            cmsCreate.SuspendLayout();
            ((ISupportInitialize)numCreateLevel).BeginInit();
            brutalGradientPanel1.SuspendLayout();
            panelCreate.SuspendLayout();
            ((ISupportInitialize)tbCreateThreads).BeginInit();
            panelExtract.SuspendLayout();
            ((ISupportInitialize)tbExtractThreads).BeginInit();
            panelFooter.SuspendLayout();
            SuspendLayout();
            // 
            // lvCreate
            // 
            lvCreate.BackColor = Color.FromArgb(32, 32, 32);
            lvCreate.BorderStyle = BorderStyle.None;
            lvCreate.Columns.AddRange(new ColumnHeader[] { colName, colType, colSize, colItems, colPath });
            lvCreate.ContextMenuStrip = cmsCreate;
            lvCreate.ForeColor = Color.White;
            lvCreate.FullRowSelect = true;
            lvCreate.Location = new Point(3, 24);
            lvCreate.Name = "lvCreate";
            lvCreate.Size = new Size(593, 220);
            lvCreate.TabIndex = 0;
            lvCreate.UseCompatibleStateImageBehavior = false;
            lvCreate.View = View.Details;
            // 
            // colName
            // 
            colName.Text = "Name";
            colName.Width = 220;
            // 
            // colType
            // 
            colType.Text = "Type";
            colType.Width = 80;
            // 
            // colSize
            // 
            colSize.Text = "Size";
            colSize.TextAlign = HorizontalAlignment.Right;
            colSize.Width = 110;
            // 
            // colItems
            // 
            colItems.Text = "Items";
            colItems.TextAlign = HorizontalAlignment.Right;
            // 
            // colPath
            // 
            colPath.Text = "Path";
            colPath.Width = 400;
            // 
            // cmsCreate
            // 
            cmsCreate.Items.AddRange(new ToolStripItem[] { mnuCreateAddFiles, mnuCreateAddFolder, sepCreate1, mnuCreateRemoveSelected, mnuCreateRemoveMissing, mnuCreateClearAll });
            cmsCreate.Name = "cmsCreate";
            cmsCreate.Size = new Size(164, 120);
            // 
            // mnuCreateAddFiles
            // 
            mnuCreateAddFiles.Name = "mnuCreateAddFiles";
            mnuCreateAddFiles.Size = new Size(163, 22);
            mnuCreateAddFiles.Text = "Add files…";
            // 
            // mnuCreateAddFolder
            // 
            mnuCreateAddFolder.Name = "mnuCreateAddFolder";
            mnuCreateAddFolder.Size = new Size(163, 22);
            mnuCreateAddFolder.Text = "Add folder…";
            // 
            // sepCreate1
            // 
            sepCreate1.Name = "sepCreate1";
            sepCreate1.Size = new Size(160, 6);
            // 
            // mnuCreateRemoveSelected
            // 
            mnuCreateRemoveSelected.Name = "mnuCreateRemoveSelected";
            mnuCreateRemoveSelected.Size = new Size(163, 22);
            mnuCreateRemoveSelected.Text = "Remove selected";
            // 
            // mnuCreateRemoveMissing
            // 
            mnuCreateRemoveMissing.Name = "mnuCreateRemoveMissing";
            mnuCreateRemoveMissing.Size = new Size(163, 22);
            mnuCreateRemoveMissing.Text = "Remove missing";
            // 
            // mnuCreateClearAll
            // 
            mnuCreateClearAll.Name = "mnuCreateClearAll";
            mnuCreateClearAll.Size = new Size(163, 22);
            mnuCreateClearAll.Text = "Clear all";
            // 
            // btnCreateAddFiles
            // 
            btnCreateAddFiles.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCreateAddFiles.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCreateAddFiles.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCreateAddFiles.FlatStyle = FlatStyle.Flat;
            btnCreateAddFiles.Location = new Point(4, 250);
            btnCreateAddFiles.Name = "btnCreateAddFiles";
            btnCreateAddFiles.Size = new Size(120, 28);
            btnCreateAddFiles.TabIndex = 1;
            btnCreateAddFiles.Text = "Add files…";
            // 
            // btnCreateAddFolder
            // 
            btnCreateAddFolder.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCreateAddFolder.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCreateAddFolder.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCreateAddFolder.FlatStyle = FlatStyle.Flat;
            btnCreateAddFolder.Location = new Point(129, 250);
            btnCreateAddFolder.Name = "btnCreateAddFolder";
            btnCreateAddFolder.Size = new Size(120, 28);
            btnCreateAddFolder.TabIndex = 2;
            btnCreateAddFolder.Text = "Add folder…";
            // 
            // lblCreateDest
            // 
            lblCreateDest.AutoSize = true;
            lblCreateDest.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCreateDest.Location = new Point(14, 291);
            lblCreateDest.Name = "lblCreateDest";
            lblCreateDest.Size = new Size(74, 15);
            lblCreateDest.TabIndex = 3;
            lblCreateDest.Text = "Destination:";
            // 
            // txtCreateDest
            // 
            txtCreateDest.BackColor = Color.FromArgb(32, 32, 32);
            txtCreateDest.BorderStyle = BorderStyle.FixedSingle;
            txtCreateDest.ForeColor = Color.White;
            txtCreateDest.Location = new Point(94, 287);
            txtCreateDest.Name = "txtCreateDest";
            txtCreateDest.Size = new Size(372, 23);
            txtCreateDest.TabIndex = 4;
            // 
            // btnCreateBrowse
            // 
            btnCreateBrowse.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCreateBrowse.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCreateBrowse.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCreateBrowse.FlatStyle = FlatStyle.Flat;
            btnCreateBrowse.Location = new Point(472, 285);
            btnCreateBrowse.Name = "btnCreateBrowse";
            btnCreateBrowse.Size = new Size(120, 26);
            btnCreateBrowse.TabIndex = 5;
            btnCreateBrowse.Text = "Browse…";
            // 
            // lblCreateMethod
            // 
            lblCreateMethod.AutoSize = true;
            lblCreateMethod.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCreateMethod.Location = new Point(14, 321);
            lblCreateMethod.Name = "lblCreateMethod";
            lblCreateMethod.Size = new Size(54, 15);
            lblCreateMethod.TabIndex = 6;
            lblCreateMethod.Text = "Method:";
            // 
            // lblCreateLevel
            // 
            lblCreateLevel.AutoSize = true;
            lblCreateLevel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCreateLevel.Location = new Point(230, 321);
            lblCreateLevel.Name = "lblCreateLevel";
            lblCreateLevel.Size = new Size(40, 15);
            lblCreateLevel.TabIndex = 8;
            lblCreateLevel.Text = "Level:";
            // 
            // numCreateLevel
            // 
            numCreateLevel.BackColor = Color.FromArgb(32, 32, 32);
            numCreateLevel.BorderStyle = BorderStyle.FixedSingle;
            numCreateLevel.ForeColor = Color.White;
            numCreateLevel.Location = new Point(278, 319);
            numCreateLevel.Maximum = new decimal(new int[] { 22, 0, 0, 0 });
            numCreateLevel.Name = "numCreateLevel";
            numCreateLevel.Size = new Size(60, 23);
            numCreateLevel.TabIndex = 9;
            numCreateLevel.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // lblCreateAlgo
            // 
            lblCreateAlgo.AutoSize = true;
            lblCreateAlgo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCreateAlgo.Location = new Point(94, 354);
            lblCreateAlgo.Name = "lblCreateAlgo";
            lblCreateAlgo.Size = new Size(35, 15);
            lblCreateAlgo.TabIndex = 11;
            lblCreateAlgo.Text = "Algo:";
            // 
            // btnCreateSetPassword
            // 
            btnCreateSetPassword.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCreateSetPassword.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCreateSetPassword.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCreateSetPassword.FlatStyle = FlatStyle.Flat;
            btnCreateSetPassword.Location = new Point(266, 351);
            btnCreateSetPassword.Name = "btnCreateSetPassword";
            btnCreateSetPassword.Size = new Size(120, 26);
            btnCreateSetPassword.TabIndex = 13;
            btnCreateSetPassword.Text = "Set password…";
            // 
            // lblCreateThreads
            // 
            lblCreateThreads.AutoSize = true;
            lblCreateThreads.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCreateThreads.Location = new Point(14, 393);
            lblCreateThreads.Name = "lblCreateThreads";
            lblCreateThreads.Size = new Size(54, 15);
            lblCreateThreads.TabIndex = 14;
            lblCreateThreads.Text = "Threads:";
            // 
            // lblCreateThreadsVal
            // 
            lblCreateThreadsVal.BackColor = Color.Transparent;
            lblCreateThreadsVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCreateThreadsVal.Location = new Point(225, 393);
            lblCreateThreadsVal.Name = "lblCreateThreadsVal";
            lblCreateThreadsVal.Size = new Size(100, 23);
            lblCreateThreadsVal.TabIndex = 16;
            lblCreateThreadsVal.Text = "16";
            // 
            // btnCreateStart
            // 
            btnCreateStart.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCreateStart.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCreateStart.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCreateStart.FlatStyle = FlatStyle.Flat;
            btnCreateStart.Location = new Point(472, 386);
            btnCreateStart.Name = "btnCreateStart";
            btnCreateStart.Size = new Size(120, 28);
            btnCreateStart.TabIndex = 17;
            btnCreateStart.Text = "Start";
            // 
            // lblZip
            // 
            lblZip.AutoSize = true;
            lblZip.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblZip.Location = new Point(16, 35);
            lblZip.Name = "lblZip";
            lblZip.Size = new Size(53, 15);
            lblZip.TabIndex = 0;
            lblZip.Text = "Archive:";
            // 
            // txtZip
            // 
            txtZip.BackColor = Color.FromArgb(32, 32, 32);
            txtZip.BorderStyle = BorderStyle.FixedSingle;
            txtZip.ForeColor = Color.White;
            txtZip.Location = new Point(96, 31);
            txtZip.Name = "txtZip";
            txtZip.Size = new Size(382, 23);
            txtZip.TabIndex = 1;
            // 
            // btnZipBrowse
            // 
            btnZipBrowse.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnZipBrowse.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnZipBrowse.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnZipBrowse.FlatStyle = FlatStyle.Flat;
            btnZipBrowse.Location = new Point(484, 30);
            btnZipBrowse.Name = "btnZipBrowse";
            btnZipBrowse.Size = new Size(100, 26);
            btnZipBrowse.TabIndex = 2;
            btnZipBrowse.Text = "Browse…";
            // 
            // lblExtractDest
            // 
            lblExtractDest.AutoSize = true;
            lblExtractDest.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblExtractDest.Location = new Point(16, 67);
            lblExtractDest.Name = "lblExtractDest";
            lblExtractDest.Size = new Size(74, 15);
            lblExtractDest.TabIndex = 3;
            lblExtractDest.Text = "Destination:";
            // 
            // txtExtractDest
            // 
            txtExtractDest.BackColor = Color.FromArgb(32, 32, 32);
            txtExtractDest.BorderStyle = BorderStyle.FixedSingle;
            txtExtractDest.ForeColor = Color.White;
            txtExtractDest.Location = new Point(96, 63);
            txtExtractDest.Name = "txtExtractDest";
            txtExtractDest.Size = new Size(382, 23);
            txtExtractDest.TabIndex = 4;
            // 
            // btnExtractBrowse
            // 
            btnExtractBrowse.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnExtractBrowse.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnExtractBrowse.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnExtractBrowse.FlatStyle = FlatStyle.Flat;
            btnExtractBrowse.Location = new Point(484, 62);
            btnExtractBrowse.Name = "btnExtractBrowse";
            btnExtractBrowse.Size = new Size(100, 26);
            btnExtractBrowse.TabIndex = 5;
            btnExtractBrowse.Text = "Browse…";
            // 
            // lblExtractThreads
            // 
            lblExtractThreads.AutoSize = true;
            lblExtractThreads.BackColor = Color.Transparent;
            lblExtractThreads.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblExtractThreads.Location = new Point(16, 126);
            lblExtractThreads.Name = "lblExtractThreads";
            lblExtractThreads.Size = new Size(54, 15);
            lblExtractThreads.TabIndex = 7;
            lblExtractThreads.Text = "Threads:";
            // 
            // lblExtractThreadsVal
            // 
            lblExtractThreadsVal.BackColor = Color.Transparent;
            lblExtractThreadsVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblExtractThreadsVal.Location = new Point(224, 126);
            lblExtractThreadsVal.Name = "lblExtractThreadsVal";
            lblExtractThreadsVal.Size = new Size(87, 23);
            lblExtractThreadsVal.TabIndex = 9;
            lblExtractThreadsVal.Text = "16";
            // 
            // btnExtractStart
            // 
            btnExtractStart.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnExtractStart.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnExtractStart.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnExtractStart.FlatStyle = FlatStyle.Flat;
            btnExtractStart.Location = new Point(484, 119);
            btnExtractStart.Name = "btnExtractStart";
            btnExtractStart.Size = new Size(100, 28);
            btnExtractStart.TabIndex = 10;
            btnExtractStart.Text = "Start";
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnClose.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(489, 8);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 28);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;
            // 
            // brutalGradientPanel1
            // 
            brutalGradientPanel1.BackColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel1.Controls.Add(panel6);
            brutalGradientPanel1.Controls.Add(buttonExtractTab);
            brutalGradientPanel1.Controls.Add(buttonCreateTab);
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
            brutalGradientPanel1.Size = new Size(600, 84);
            brutalGradientPanel1.StartColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel1.TabIndex = 9;
            // 
            // panel6
            // 
            panel6.BackColor = Color.FromArgb(32, 32, 32);
            panel6.Dock = DockStyle.Left;
            panel6.Location = new Point(170, 0);
            panel6.Name = "panel6";
            panel6.Padding = new Padding(0, 0, 0, 2);
            panel6.Size = new Size(2, 84);
            panel6.TabIndex = 33;
            // 
            // buttonExtractTab
            // 
            buttonExtractTab.BackColor = Color.Transparent;
            buttonExtractTab.Dock = DockStyle.Left;
            buttonExtractTab.FlatAppearance.BorderSize = 0;
            buttonExtractTab.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonExtractTab.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonExtractTab.FlatStyle = FlatStyle.Flat;
            buttonExtractTab.Image = (Image)resources.GetObject("buttonExtractTab.Image");
            buttonExtractTab.Location = new Point(85, 0);
            buttonExtractTab.Name = "buttonExtractTab";
            buttonExtractTab.Size = new Size(85, 84);
            buttonExtractTab.TabIndex = 37;
            buttonExtractTab.Text = "Extract";
            buttonExtractTab.TextAlign = ContentAlignment.BottomCenter;
            buttonExtractTab.UseVisualStyleBackColor = false;
            buttonExtractTab.Click += buttonExtractTab_Click;
            // 
            // buttonCreateTab
            // 
            buttonCreateTab.BackColor = Color.Transparent;
            buttonCreateTab.Dock = DockStyle.Left;
            buttonCreateTab.FlatAppearance.BorderSize = 0;
            buttonCreateTab.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonCreateTab.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonCreateTab.FlatStyle = FlatStyle.Flat;
            buttonCreateTab.Image = (Image)resources.GetObject("buttonCreateTab.Image");
            buttonCreateTab.Location = new Point(0, 0);
            buttonCreateTab.Name = "buttonCreateTab";
            buttonCreateTab.Size = new Size(85, 84);
            buttonCreateTab.TabIndex = 0;
            buttonCreateTab.Text = "Create";
            buttonCreateTab.TextAlign = ContentAlignment.BottomCenter;
            buttonCreateTab.UseVisualStyleBackColor = false;
            buttonCreateTab.Click += buttonCreateTab_Click;
            // 
            // panelCreate
            // 
            panelCreate.Controls.Add(cmbCreateAlgo);
            panelCreate.Controls.Add(cmbCreateMethod);
            panelCreate.Controls.Add(label3);
            panelCreate.Controls.Add(chkCreateEncrypt);
            panelCreate.Controls.Add(label1);
            panelCreate.Controls.Add(lblCreateThreads);
            panelCreate.Controls.Add(lblCreateAlgo);
            panelCreate.Controls.Add(lblCreateMethod);
            panelCreate.Controls.Add(lblCreateThreadsVal);
            panelCreate.Controls.Add(btnCreateAddFolder);
            panelCreate.Controls.Add(btnCreateAddFiles);
            panelCreate.Controls.Add(btnCreateStart);
            panelCreate.Controls.Add(tbCreateThreads);
            panelCreate.Controls.Add(btnCreateSetPassword);
            panelCreate.Controls.Add(lblCreateDest);
            panelCreate.Controls.Add(lblCreateLevel);
            panelCreate.Controls.Add(txtCreateDest);
            panelCreate.Controls.Add(numCreateLevel);
            panelCreate.Controls.Add(lvCreate);
            panelCreate.Controls.Add(btnCreateBrowse);
            panelCreate.EndColor = Color.FromArgb(25, 25, 25);
            panelCreate.ForeColor = Color.White;
            panelCreate.GlowCenterMaxOpacity = 200;
            panelCreate.GlowCenterMinOpacity = 50;
            panelCreate.GlowMinSurroundOpacity = 30;
            panelCreate.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            panelCreate.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            panelCreate.Location = new Point(0, 83);
            panelCreate.MouseEvents = true;
            panelCreate.Name = "panelCreate";
            panelCreate.Size = new Size(600, 422);
            panelCreate.StartColor = Color.FromArgb(16, 16, 16);
            panelCreate.TabIndex = 10;
            // 
            // cmbCreateAlgo
            // 
            cmbCreateAlgo.BackColor = Color.FromArgb(32, 32, 32);
            cmbCreateAlgo.ButtonColor = Color.FromArgb(48, 48, 48);
            cmbCreateAlgo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCreateAlgo.ForeColor = Color.White;
            cmbCreateAlgo.FormattingEnabled = true;
            cmbCreateAlgo.Items.AddRange(new object[] { "ZipCrypto", "AES-128", "AES-192", "AES-256" });
            cmbCreateAlgo.Location = new Point(135, 352);
            cmbCreateAlgo.Name = "cmbCreateAlgo";
            cmbCreateAlgo.Size = new Size(125, 23);
            cmbCreateAlgo.TabIndex = 46;
            // 
            // cmbCreateMethod
            // 
            cmbCreateMethod.BackColor = Color.FromArgb(32, 32, 32);
            cmbCreateMethod.ButtonColor = Color.FromArgb(48, 48, 48);
            cmbCreateMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCreateMethod.ForeColor = Color.White;
            cmbCreateMethod.FormattingEnabled = true;
            cmbCreateMethod.Items.AddRange(new object[] { "Store", "Deflate", "Zstd" });
            cmbCreateMethod.Location = new Point(94, 318);
            cmbCreateMethod.Name = "cmbCreateMethod";
            cmbCreateMethod.Size = new Size(130, 23);
            cmbCreateMethod.TabIndex = 45;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(37, 357);
            label3.Name = "label3";
            label3.Size = new Size(49, 15);
            label3.TabIndex = 44;
            label3.Text = "Encrypt";
            // 
            // chkCreateEncrypt
            // 
            chkCreateEncrypt.BackColor = Color.Transparent;
            chkCreateEncrypt.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkCreateEncrypt.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkCreateEncrypt.BoxGradientEnabled = true;
            chkCreateEncrypt.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkCreateEncrypt.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkCreateEncrypt.BoxSize = 14;
            chkCreateEncrypt.CheckBorderColor = Color.Lime;
            chkCreateEncrypt.CheckColor = Color.LawnGreen;
            chkCreateEncrypt.CheckGradientEnabled = true;
            chkCreateEncrypt.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkCreateEncrypt.CheckGradientStart = Color.Lime;
            chkCreateEncrypt.Location = new Point(16, 351);
            chkCreateEncrypt.Name = "chkCreateEncrypt";
            chkCreateEncrypt.Size = new Size(18, 24);
            chkCreateEncrypt.TabIndex = 43;
            chkCreateEncrypt.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 3);
            label1.Name = "label1";
            label1.Size = new Size(44, 15);
            label1.TabIndex = 13;
            label1.Text = "Create";
            // 
            // tbCreateThreads
            // 
            tbCreateThreads.LeftBarColor = Color.FromArgb(29, 181, 82);
            tbCreateThreads.Location = new Point(71, 378);
            tbCreateThreads.Maximum = 16;
            tbCreateThreads.Name = "tbCreateThreads";
            tbCreateThreads.RightBarColor = Color.FromArgb(22, 132, 99);
            tbCreateThreads.Size = new Size(143, 45);
            tbCreateThreads.TabIndex = 42;
            tbCreateThreads.ThumbInnerColor = Color.FromArgb(29, 181, 82);
            tbCreateThreads.ThumbOutlineColor = Color.FromArgb(19, 90, 42);
            tbCreateThreads.ThumbOutlineThickness = 2;
            tbCreateThreads.Value = 16;
            // 
            // panelExtract
            // 
            panelExtract.Controls.Add(label4);
            panelExtract.Controls.Add(chkExtractToFolderName);
            panelExtract.Controls.Add(label2);
            panelExtract.Controls.Add(tbExtractThreads);
            panelExtract.Controls.Add(lblZip);
            panelExtract.Controls.Add(txtZip);
            panelExtract.Controls.Add(btnZipBrowse);
            panelExtract.Controls.Add(lblExtractThreadsVal);
            panelExtract.Controls.Add(btnExtractStart);
            panelExtract.Controls.Add(lblExtractDest);
            panelExtract.Controls.Add(txtExtractDest);
            panelExtract.Controls.Add(lblExtractThreads);
            panelExtract.Controls.Add(btnExtractBrowse);
            panelExtract.EndColor = Color.FromArgb(25, 25, 25);
            panelExtract.ForeColor = Color.White;
            panelExtract.GlowCenterMaxOpacity = 200;
            panelExtract.GlowCenterMinOpacity = 50;
            panelExtract.GlowMinSurroundOpacity = 30;
            panelExtract.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            panelExtract.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            panelExtract.Location = new Point(0, 83);
            panelExtract.MouseEvents = true;
            panelExtract.Name = "panelExtract";
            panelExtract.Size = new Size(600, 156);
            panelExtract.StartColor = Color.FromArgb(16, 16, 16);
            panelExtract.TabIndex = 11;
            panelExtract.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(429, 96);
            label4.Name = "label4";
            label4.Size = new Size(158, 15);
            label4.TabIndex = 46;
            label4.Text = "Extract to “ArchiveName/”";
            // 
            // chkExtractToFolderName
            // 
            chkExtractToFolderName.BackColor = Color.Transparent;
            chkExtractToFolderName.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkExtractToFolderName.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkExtractToFolderName.BoxGradientEnabled = true;
            chkExtractToFolderName.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkExtractToFolderName.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkExtractToFolderName.BoxSize = 14;
            chkExtractToFolderName.CheckBorderColor = Color.Lime;
            chkExtractToFolderName.CheckColor = Color.LawnGreen;
            chkExtractToFolderName.CheckGradientEnabled = true;
            chkExtractToFolderName.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkExtractToFolderName.CheckGradientStart = Color.Lime;
            chkExtractToFolderName.Location = new Point(408, 90);
            chkExtractToFolderName.Name = "chkExtractToFolderName";
            chkExtractToFolderName.Size = new Size(18, 24);
            chkExtractToFolderName.TabIndex = 45;
            chkExtractToFolderName.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(3, 3);
            label2.Name = "label2";
            label2.Size = new Size(47, 15);
            label2.TabIndex = 44;
            label2.Text = "Extract";
            // 
            // tbExtractThreads
            // 
            tbExtractThreads.LeftBarColor = Color.FromArgb(29, 181, 82);
            tbExtractThreads.Location = new Point(70, 112);
            tbExtractThreads.Maximum = 16;
            tbExtractThreads.Name = "tbExtractThreads";
            tbExtractThreads.RightBarColor = Color.FromArgb(22, 132, 99);
            tbExtractThreads.Size = new Size(143, 45);
            tbExtractThreads.TabIndex = 43;
            tbExtractThreads.ThumbInnerColor = Color.FromArgb(29, 181, 82);
            tbExtractThreads.ThumbOutlineColor = Color.FromArgb(19, 90, 42);
            tbExtractThreads.ThumbOutlineThickness = 2;
            tbExtractThreads.Value = 16;
            // 
            // panelFooter
            // 
            panelFooter.BackColor = Color.FromArgb(16, 16, 16);
            panelFooter.Controls.Add(btnClose);
            panelFooter.Dock = DockStyle.Bottom;
            panelFooter.Location = new Point(0, 504);
            panelFooter.Name = "panelFooter";
            panelFooter.Size = new Size(600, 45);
            panelFooter.TabIndex = 12;
            // 
            // WizardForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(600, 549);
            Controls.Add(panelFooter);
            Controls.Add(brutalGradientPanel1);
            Controls.Add(panelCreate);
            Controls.Add(panelExtract);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "WizardForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Wizard";
            Load += WizardForm_Load;
            cmsCreate.ResumeLayout(false);
            ((ISupportInitialize)numCreateLevel).EndInit();
            brutalGradientPanel1.ResumeLayout(false);
            panelCreate.ResumeLayout(false);
            panelCreate.PerformLayout();
            ((ISupportInitialize)tbCreateThreads).EndInit();
            panelExtract.ResumeLayout(false);
            panelExtract.PerformLayout();
            ((ISupportInitialize)tbExtractThreads).EndInit();
            panelFooter.ResumeLayout(false);
            ResumeLayout(false);
        }
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel1;
        private Panel panel6;
        private Button buttonExtractTab;
        private Button buttonCreateTab;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel panelCreate;
        private BrutalZip2025.BrutalControls.BrutalTrackBar tbCreateThreads;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel panelExtract;
        private BrutalZip2025.BrutalControls.BrutalTrackBar tbExtractThreads;
        private Label label1;
        private Panel panelFooter;
        private Label label2;
        private Label label3;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkCreateEncrypt;
        private Label label4;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkExtractToFolderName;
        internal BrutalZip2025.BrutalControls.BrutalComboBox cmbCreateMethod;
        internal BrutalZip2025.BrutalControls.BrutalComboBox cmbCreateAlgo;
    }
}
