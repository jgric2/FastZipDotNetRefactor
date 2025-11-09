using BrutalZip2025.BrutalControls;
using System.ComponentModel;

namespace Brutal_Zip.Views
{
    partial class HomeView
    {
        private IContainer components = null;
        internal BrutalComboBox cmbCreateMethod;
        internal NumericUpDown numCreateLevel;
        internal Button btnCreate;
        internal Label lblCreateHint;
        internal Label lblPwdStatus;

        // Staging
        internal ListView lvStaging;
        private ColumnHeader colStName;
        private ColumnHeader colStType;
        private ColumnHeader colStSize;
        private ColumnHeader colStItems;
        private ColumnHeader colStPath;
        internal ContextMenuStrip cmsStaging;
        internal ToolStripMenuItem mnuStagingRemove;
        internal ToolStripMenuItem mnuStagingRemoveMissing; // NEW
        internal ToolStripMenuItem mnuStagingClear;
        internal Panel pnlExtractDrop;
        private Label lblExtractDrop;
        internal RadioButton rdoExtractToFolderName;
        internal RadioButton rdoExtractHere;
        private Label lblExtractDest;
        internal Button btnExtract;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            numCreateLevel = new NumericUpDown();
            btnCreate = new Button();
            lblCreateHint = new Label();
            lblPwdStatus = new Label();
            lvStaging = new ListView();
            colStName = new ColumnHeader();
            colStType = new ColumnHeader();
            colStSize = new ColumnHeader();
            colStItems = new ColumnHeader();
            colStPath = new ColumnHeader();
            cmsStaging = new ContextMenuStrip(components);
            mnuStagingRemove = new ToolStripMenuItem();
            mnuStagingRemoveMissing = new ToolStripMenuItem();
            mnuStagingClear = new ToolStripMenuItem();
            rdoExtractToFolderName = new RadioButton();
            rdoExtractHere = new RadioButton();
            lblExtractDest = new Label();
            btnExtract = new Button();
            pnlExtractDrop = new Panel();
            lblExtractDrop = new Label();
            GradientPanelCreateRibbon = new BrutalGradientPanel();
            ribbonControlsCreate = new Panel();
            panel6 = new Panel();
            btnCreateSetPassword = new Button();
            panel5 = new Panel();
            label8 = new Label();
            cmbEncrypt = new BrutalComboBox();
            chkEncrypt = new BrutalCheckBox();
            label7 = new Label();
            panel4 = new Panel();
            panel3 = new Panel();
            label6 = new Label();
            cmbCreateMethod = new BrutalComboBox();
            label5 = new Label();
            panel2 = new Panel();
            buttonWizardCreate = new Button();
            buttonComment = new Button();
            buttonAddFiles = new Button();
            buttonAddFolder = new Button();
            brutalGradientPanel3 = new BrutalGradientPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            label9 = new Label();
            label4 = new Label();
            label1 = new Label();
            tbThreads = new BrutalTrackBar();
            brutalGradientPanel2 = new BrutalGradientPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            TabButtonExtract = new CheckBox();
            tabButtonCreate = new CheckBox();
            panel1 = new Panel();
            label2 = new Label();
            lblThreadsValue = new Label();
            chkThreadsAutoMain = new BrutalCheckBox();
            label3 = new Label();
            GradientPanelExtractRibbon = new BrutalGradientPanel();
            ribbonControlsExtract = new Panel();
            panel12 = new Panel();
            buttonWizardExtract = new Button();
            buttonTestZip = new Button();
            buttonOpenZip = new Button();
            brutalGradientPanel4 = new BrutalGradientPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            label16 = new Label();
            panelCreateTabInner = new Panel();
            splitContainerCreate = new SplitContainer();
            treeViewExplorer = new TreeViewMS.TreeViewMS();
            tableLayoutPanel5 = new TableLayoutPanel();
            panel7 = new Panel();
            btnCreateQuick = new Button();
            panelCreateZipTab = new Panel();
            panelExtractZipTab = new Panel();
            tableLayoutPanel6 = new TableLayoutPanel();
            panel9 = new Panel();
            ((ISupportInitialize)numCreateLevel).BeginInit();
            cmsStaging.SuspendLayout();
            pnlExtractDrop.SuspendLayout();
            GradientPanelCreateRibbon.SuspendLayout();
            ribbonControlsCreate.SuspendLayout();
            panel5.SuspendLayout();
            panel3.SuspendLayout();
            brutalGradientPanel3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((ISupportInitialize)tbThreads).BeginInit();
            brutalGradientPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            panel1.SuspendLayout();
            GradientPanelExtractRibbon.SuspendLayout();
            ribbonControlsExtract.SuspendLayout();
            brutalGradientPanel4.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panelCreateTabInner.SuspendLayout();
            ((ISupportInitialize)splitContainerCreate).BeginInit();
            splitContainerCreate.Panel1.SuspendLayout();
            splitContainerCreate.Panel2.SuspendLayout();
            splitContainerCreate.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            panel7.SuspendLayout();
            panelCreateZipTab.SuspendLayout();
            panelExtractZipTab.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            panel9.SuspendLayout();
            SuspendLayout();
            // 
            // numCreateLevel
            // 
            numCreateLevel.BackColor = Color.FromArgb(32, 32, 32);
            numCreateLevel.BorderStyle = BorderStyle.FixedSingle;
            numCreateLevel.ForeColor = Color.White;
            numCreateLevel.Location = new Point(134, 47);
            numCreateLevel.Maximum = new decimal(new int[] { 22, 0, 0, 0 });
            numCreateLevel.Name = "numCreateLevel";
            numCreateLevel.Size = new Size(110, 23);
            numCreateLevel.TabIndex = 9;
            numCreateLevel.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // btnCreate
            // 
            btnCreate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCreate.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCreate.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCreate.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.Location = new Point(449, 3);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(100, 28);
            btnCreate.TabIndex = 14;
            btnCreate.Text = "Create";
            btnCreate.Click += btnCreate_Click;
            // 
            // lblCreateHint
            // 
            lblCreateHint.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblCreateHint.AutoEllipsis = true;
            lblCreateHint.Location = new Point(0, 10);
            lblCreateHint.Name = "lblCreateHint";
            lblCreateHint.Size = new Size(443, 20);
            lblCreateHint.TabIndex = 17;
            lblCreateHint.Text = "Staging: none";
            // 
            // lblPwdStatus
            // 
            lblPwdStatus.ForeColor = Color.DimGray;
            lblPwdStatus.Location = new Point(7, 61);
            lblPwdStatus.Name = "lblPwdStatus";
            lblPwdStatus.Size = new Size(168, 18);
            lblPwdStatus.TabIndex = 18;
            lblPwdStatus.Text = "Password: (not set)";
            // 
            // lvStaging
            // 
            lvStaging.BackColor = Color.FromArgb(25, 25, 25);
            lvStaging.BorderStyle = BorderStyle.None;
            lvStaging.Columns.AddRange(new ColumnHeader[] { colStName, colStType, colStSize, colStItems, colStPath });
            lvStaging.ContextMenuStrip = cmsStaging;
            lvStaging.Dock = DockStyle.Fill;
            lvStaging.ForeColor = Color.White;
            lvStaging.FullRowSelect = true;
            lvStaging.Location = new Point(3, 3);
            lvStaging.Name = "lvStaging";
            lvStaging.Size = new Size(715, 528);
            lvStaging.TabIndex = 16;
            lvStaging.UseCompatibleStateImageBehavior = false;
            lvStaging.View = View.Details;
            lvStaging.DragDrop += lvStaging_DragDrop;
            lvStaging.DragEnter += lvStaging_DragEnter;
            // 
            // colStName
            // 
            colStName.Text = "Name";
            colStName.Width = 150;
            // 
            // colStType
            // 
            colStType.Text = "Type";
            colStType.Width = 70;
            // 
            // colStSize
            // 
            colStSize.Text = "Size";
            colStSize.TextAlign = HorizontalAlignment.Right;
            colStSize.Width = 80;
            // 
            // colStItems
            // 
            colStItems.Text = "Items";
            colStItems.TextAlign = HorizontalAlignment.Right;
            // 
            // colStPath
            // 
            colStPath.Text = "Path";
            colStPath.Width = 300;
            // 
            // cmsStaging
            // 
            cmsStaging.Items.AddRange(new ToolStripItem[] { mnuStagingRemove, mnuStagingRemoveMissing, mnuStagingClear });
            cmsStaging.Name = "cmsStaging";
            cmsStaging.Size = new Size(164, 70);
            // 
            // mnuStagingRemove
            // 
            mnuStagingRemove.Name = "mnuStagingRemove";
            mnuStagingRemove.Size = new Size(163, 22);
            mnuStagingRemove.Text = "Remove selected";
            mnuStagingRemove.Click += mnuStagingRemove_Click;
            // 
            // mnuStagingRemoveMissing
            // 
            mnuStagingRemoveMissing.Name = "mnuStagingRemoveMissing";
            mnuStagingRemoveMissing.Size = new Size(163, 22);
            mnuStagingRemoveMissing.Text = "Remove missing";
            mnuStagingRemoveMissing.Click += mnuStagingRemoveMissing_Click;
            // 
            // mnuStagingClear
            // 
            mnuStagingClear.Name = "mnuStagingClear";
            mnuStagingClear.Size = new Size(163, 22);
            mnuStagingClear.Text = "Clear all";
            mnuStagingClear.Click += mnuStagingClear_Click;
            // 
            // rdoExtractToFolderName
            // 
            rdoExtractToFolderName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            rdoExtractToFolderName.Checked = true;
            rdoExtractToFolderName.Location = new Point(191, 8);
            rdoExtractToFolderName.Name = "rdoExtractToFolderName";
            rdoExtractToFolderName.Size = new Size(173, 20);
            rdoExtractToFolderName.TabIndex = 2;
            rdoExtractToFolderName.TabStop = true;
            rdoExtractToFolderName.Text = "Extract to “ArchiveName/”";
            // 
            // rdoExtractHere
            // 
            rdoExtractHere.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            rdoExtractHere.Location = new Point(97, 7);
            rdoExtractHere.Name = "rdoExtractHere";
            rdoExtractHere.Size = new Size(88, 20);
            rdoExtractHere.TabIndex = 3;
            rdoExtractHere.Text = "Extract here";
            // 
            // lblExtractDest
            // 
            lblExtractDest.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblExtractDest.Location = new Point(8, 8);
            lblExtractDest.Name = "lblExtractDest";
            lblExtractDest.Size = new Size(80, 20);
            lblExtractDest.TabIndex = 4;
            lblExtractDest.Text = "Destination:";
            // 
            // btnExtract
            // 
            btnExtract.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExtract.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnExtract.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnExtract.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnExtract.FlatStyle = FlatStyle.Flat;
            btnExtract.Location = new Point(890, 2);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new Size(100, 28);
            btnExtract.TabIndex = 7;
            btnExtract.Text = "Extract";
            btnExtract.Click += btnExtract_Click;
            // 
            // pnlExtractDrop
            // 
            pnlExtractDrop.BackColor = Color.FromArgb(16, 16, 16);
            pnlExtractDrop.Controls.Add(lblExtractDrop);
            pnlExtractDrop.Dock = DockStyle.Fill;
            pnlExtractDrop.ForeColor = Color.White;
            pnlExtractDrop.Location = new Point(3, 3);
            pnlExtractDrop.Name = "pnlExtractDrop";
            pnlExtractDrop.Size = new Size(994, 530);
            pnlExtractDrop.TabIndex = 0;
            // 
            // lblExtractDrop
            // 
            lblExtractDrop.Dock = DockStyle.Fill;
            lblExtractDrop.Location = new Point(0, 0);
            lblExtractDrop.Name = "lblExtractDrop";
            lblExtractDrop.Size = new Size(994, 530);
            lblExtractDrop.TabIndex = 0;
            lblExtractDrop.Text = "Drop .zip files here to extract or open\r\n(Single .zip to open in viewer, Multiple .zips for bulk extraction)";
            lblExtractDrop.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // GradientPanelCreateRibbon
            // 
            GradientPanelCreateRibbon.BackColor = Color.FromArgb(32, 32, 32);
            GradientPanelCreateRibbon.Controls.Add(ribbonControlsCreate);
            GradientPanelCreateRibbon.Dock = DockStyle.Top;
            GradientPanelCreateRibbon.EndColor = Color.FromArgb(64, 64, 64);
            GradientPanelCreateRibbon.ForeColor = Color.White;
            GradientPanelCreateRibbon.GlowCenterMaxOpacity = 200;
            GradientPanelCreateRibbon.GlowCenterMinOpacity = 50;
            GradientPanelCreateRibbon.GlowMinSurroundOpacity = 30;
            GradientPanelCreateRibbon.GradientEndSide = GradientSide.Bottom;
            GradientPanelCreateRibbon.GradientStartSide = GradientSide.Top;
            GradientPanelCreateRibbon.Location = new Point(0, 0);
            GradientPanelCreateRibbon.Margin = new Padding(0);
            GradientPanelCreateRibbon.MouseEvents = true;
            GradientPanelCreateRibbon.Name = "GradientPanelCreateRibbon";
            GradientPanelCreateRibbon.Size = new Size(1000, 83);
            GradientPanelCreateRibbon.StartColor = Color.FromArgb(32, 32, 32);
            GradientPanelCreateRibbon.TabIndex = 5;
            // 
            // ribbonControlsCreate
            // 
            ribbonControlsCreate.BackColor = Color.Transparent;
            ribbonControlsCreate.Controls.Add(panel6);
            ribbonControlsCreate.Controls.Add(btnCreateSetPassword);
            ribbonControlsCreate.Controls.Add(panel5);
            ribbonControlsCreate.Controls.Add(panel4);
            ribbonControlsCreate.Controls.Add(panel3);
            ribbonControlsCreate.Controls.Add(panel2);
            ribbonControlsCreate.Controls.Add(buttonWizardCreate);
            ribbonControlsCreate.Controls.Add(buttonComment);
            ribbonControlsCreate.Controls.Add(buttonAddFiles);
            ribbonControlsCreate.Controls.Add(buttonAddFolder);
            ribbonControlsCreate.Location = new Point(-1, -1);
            ribbonControlsCreate.Name = "ribbonControlsCreate";
            ribbonControlsCreate.Size = new Size(874, 84);
            ribbonControlsCreate.TabIndex = 7;
            // 
            // panel6
            // 
            panel6.BackColor = Color.FromArgb(32, 32, 32);
            panel6.Dock = DockStyle.Left;
            panel6.Location = new Point(862, 0);
            panel6.Name = "panel6";
            panel6.Padding = new Padding(0, 0, 0, 2);
            panel6.Size = new Size(2, 84);
            panel6.TabIndex = 32;
            // 
            // btnCreateSetPassword
            // 
            btnCreateSetPassword.BackColor = Color.Transparent;
            btnCreateSetPassword.Dock = DockStyle.Left;
            btnCreateSetPassword.FlatAppearance.BorderSize = 0;
            btnCreateSetPassword.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCreateSetPassword.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCreateSetPassword.FlatStyle = FlatStyle.Flat;
            btnCreateSetPassword.Location = new Point(777, 0);
            btnCreateSetPassword.Name = "btnCreateSetPassword";
            btnCreateSetPassword.Size = new Size(85, 84);
            btnCreateSetPassword.TabIndex = 31;
            btnCreateSetPassword.Text = "Set Password";
            btnCreateSetPassword.TextAlign = ContentAlignment.BottomCenter;
            btnCreateSetPassword.UseVisualStyleBackColor = false;
            btnCreateSetPassword.Click += btnCreateSetPassword_Click;
            // 
            // panel5
            // 
            panel5.BackColor = Color.Transparent;
            panel5.Controls.Add(label8);
            panel5.Controls.Add(cmbEncrypt);
            panel5.Controls.Add(chkEncrypt);
            panel5.Controls.Add(label7);
            panel5.Controls.Add(lblPwdStatus);
            panel5.Dock = DockStyle.Left;
            panel5.Location = new Point(596, 0);
            panel5.Name = "panel5";
            panel5.Padding = new Padding(0, 0, 0, 2);
            panel5.Size = new Size(181, 84);
            panel5.TabIndex = 30;
            // 
            // label8
            // 
            label8.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(7, 31);
            label8.Name = "label8";
            label8.Size = new Size(57, 24);
            label8.TabIndex = 33;
            label8.Text = "Method:";
            label8.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbEncrypt
            // 
            cmbEncrypt.BackColor = Color.FromArgb(32, 32, 32);
            cmbEncrypt.ButtonColor = Color.FromArgb(48, 48, 48);
            cmbEncrypt.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEncrypt.ForeColor = Color.White;
            cmbEncrypt.FormattingEnabled = true;
            cmbEncrypt.Items.AddRange(new object[] { "ZipCrypto", "AES-128", "AES-192", "AES-256" });
            cmbEncrypt.Location = new Point(84, 32);
            cmbEncrypt.Name = "cmbEncrypt";
            cmbEncrypt.Size = new Size(80, 23);
            cmbEncrypt.TabIndex = 32;
            cmbEncrypt.SelectedIndexChanged += cmbEncrypt_SelectedIndexChanged;
            // 
            // chkEncrypt
            // 
            chkEncrypt.BackColor = Color.Transparent;
            chkEncrypt.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkEncrypt.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkEncrypt.BoxGradientEnabled = true;
            chkEncrypt.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkEncrypt.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkEncrypt.BoxSize = 14;
            chkEncrypt.CheckBorderColor = Color.Lime;
            chkEncrypt.CheckColor = Color.LawnGreen;
            chkEncrypt.Checked = true;
            chkEncrypt.CheckGradientEnabled = true;
            chkEncrypt.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkEncrypt.CheckGradientStart = Color.Lime;
            chkEncrypt.CheckState = CheckState.Checked;
            chkEncrypt.Location = new Point(84, 4);
            chkEncrypt.Name = "chkEncrypt";
            chkEncrypt.Size = new Size(18, 24);
            chkEncrypt.TabIndex = 30;
            chkEncrypt.UseVisualStyleBackColor = false;
            chkEncrypt.CheckedChanged += chkEncrypt_CheckedChanged;
            // 
            // label7
            // 
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(7, 5);
            label7.Name = "label7";
            label7.Size = new Size(57, 24);
            label7.TabIndex = 31;
            label7.Text = "Encrypt:";
            label7.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel4
            // 
            panel4.BackColor = Color.FromArgb(32, 32, 32);
            panel4.Dock = DockStyle.Left;
            panel4.Location = new Point(594, 0);
            panel4.Name = "panel4";
            panel4.Padding = new Padding(0, 0, 0, 2);
            panel4.Size = new Size(2, 84);
            panel4.TabIndex = 29;
            // 
            // panel3
            // 
            panel3.BackColor = Color.Transparent;
            panel3.Controls.Add(label6);
            panel3.Controls.Add(cmbCreateMethod);
            panel3.Controls.Add(label5);
            panel3.Controls.Add(numCreateLevel);
            panel3.Dock = DockStyle.Left;
            panel3.Location = new Point(342, 0);
            panel3.Name = "panel3";
            panel3.Padding = new Padding(0, 0, 0, 2);
            panel3.Size = new Size(252, 84);
            panel3.TabIndex = 28;
            // 
            // label6
            // 
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(13, 20);
            label6.Name = "label6";
            label6.Size = new Size(116, 24);
            label6.TabIndex = 26;
            label6.Text = "Method:";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbCreateMethod
            // 
            cmbCreateMethod.BackColor = Color.FromArgb(32, 32, 32);
            cmbCreateMethod.ButtonColor = Color.FromArgb(48, 48, 48);
            cmbCreateMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCreateMethod.ForeColor = Color.White;
            cmbCreateMethod.FormattingEnabled = true;
            cmbCreateMethod.Items.AddRange(new object[] { "Store", "Deflate", "Zstd" });
            cmbCreateMethod.Location = new Point(134, 18);
            cmbCreateMethod.Name = "cmbCreateMethod";
            cmbCreateMethod.Size = new Size(110, 23);
            cmbCreateMethod.TabIndex = 27;
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(12, 46);
            label5.Name = "label5";
            label5.Size = new Size(116, 24);
            label5.TabIndex = 24;
            label5.Text = "Compression Level:";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(32, 32, 32);
            panel2.Dock = DockStyle.Left;
            panel2.Location = new Point(340, 0);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(0, 0, 0, 2);
            panel2.Size = new Size(2, 84);
            panel2.TabIndex = 25;
            // 
            // buttonWizardCreate
            // 
            buttonWizardCreate.BackColor = Color.Transparent;
            buttonWizardCreate.Dock = DockStyle.Left;
            buttonWizardCreate.FlatAppearance.BorderSize = 0;
            buttonWizardCreate.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonWizardCreate.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonWizardCreate.FlatStyle = FlatStyle.Flat;
            buttonWizardCreate.Location = new Point(255, 0);
            buttonWizardCreate.Name = "buttonWizardCreate";
            buttonWizardCreate.Size = new Size(85, 84);
            buttonWizardCreate.TabIndex = 22;
            buttonWizardCreate.Text = "Wizard";
            buttonWizardCreate.TextAlign = ContentAlignment.BottomCenter;
            buttonWizardCreate.UseVisualStyleBackColor = false;
            buttonWizardCreate.Click += buttonWizardCreate_Click;
            // 
            // buttonComment
            // 
            buttonComment.BackColor = Color.Transparent;
            buttonComment.Dock = DockStyle.Left;
            buttonComment.FlatAppearance.BorderSize = 0;
            buttonComment.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonComment.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonComment.FlatStyle = FlatStyle.Flat;
            buttonComment.Location = new Point(170, 0);
            buttonComment.Name = "buttonComment";
            buttonComment.Size = new Size(85, 84);
            buttonComment.TabIndex = 21;
            buttonComment.Text = "Comment";
            buttonComment.TextAlign = ContentAlignment.BottomCenter;
            buttonComment.UseVisualStyleBackColor = false;
            buttonComment.Click += buttonComment_Click;
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
            buttonAddFiles.TabIndex = 19;
            buttonAddFiles.Text = "Add Files";
            buttonAddFiles.TextAlign = ContentAlignment.BottomCenter;
            buttonAddFiles.UseVisualStyleBackColor = false;
            buttonAddFiles.Click += buttonAddFiles_Click;
            // 
            // buttonAddFolder
            // 
            buttonAddFolder.BackColor = Color.Transparent;
            buttonAddFolder.Dock = DockStyle.Left;
            buttonAddFolder.FlatAppearance.BorderSize = 0;
            buttonAddFolder.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonAddFolder.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonAddFolder.FlatStyle = FlatStyle.Flat;
            buttonAddFolder.Location = new Point(0, 0);
            buttonAddFolder.Name = "buttonAddFolder";
            buttonAddFolder.Size = new Size(85, 84);
            buttonAddFolder.TabIndex = 14;
            buttonAddFolder.Text = "Add Folder";
            buttonAddFolder.TextAlign = ContentAlignment.BottomCenter;
            buttonAddFolder.UseVisualStyleBackColor = false;
            buttonAddFolder.Click += buttonAddFolder_Click;
            // 
            // brutalGradientPanel3
            // 
            brutalGradientPanel3.BackColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel3.Controls.Add(tableLayoutPanel2);
            brutalGradientPanel3.Dock = DockStyle.Top;
            brutalGradientPanel3.EndColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel3.ForeColor = Color.White;
            brutalGradientPanel3.GlowCenterMaxOpacity = 200;
            brutalGradientPanel3.GlowCenterMinOpacity = 50;
            brutalGradientPanel3.GlowMinSurroundOpacity = 30;
            brutalGradientPanel3.GradientEndSide = GradientSide.Top;
            brutalGradientPanel3.GradientStartSide = GradientSide.Bottom;
            brutalGradientPanel3.Location = new Point(0, 83);
            brutalGradientPanel3.Margin = new Padding(0);
            brutalGradientPanel3.MouseEvents = true;
            brutalGradientPanel3.Name = "brutalGradientPanel3";
            brutalGradientPanel3.Size = new Size(1000, 16);
            brutalGradientPanel3.StartColor = Color.FromArgb(64, 64, 64);
            brutalGradientPanel3.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.BackColor = Color.Transparent;
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 340F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 254F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 268F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8F));
            tableLayoutPanel2.Controls.Add(label9, 2, 0);
            tableLayoutPanel2.Controls.Add(label4, 1, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(1000, 16);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Dock = DockStyle.Fill;
            label9.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.Location = new Point(597, 0);
            label9.Name = "label9";
            label9.Size = new Size(262, 16);
            label9.TabIndex = 2;
            label9.Text = "Encryption Options";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(343, 0);
            label4.Name = "label4";
            label4.Size = new Size(248, 16);
            label4.TabIndex = 1;
            label4.Text = "Compression Options";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(334, 16);
            label1.TabIndex = 0;
            label1.Text = "Create";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbThreads
            // 
            tbThreads.Dock = DockStyle.Right;
            tbThreads.LeftBarColor = Color.FromArgb(29, 181, 82);
            tbThreads.Location = new Point(163, 0);
            tbThreads.Maximum = 100;
            tbThreads.Name = "tbThreads";
            tbThreads.RightBarColor = Color.FromArgb(22, 132, 99);
            tbThreads.Size = new Size(143, 24);
            tbThreads.TabIndex = 0;
            tbThreads.ThumbInnerColor = Color.FromArgb(29, 181, 82);
            tbThreads.ThumbOutlineColor = Color.FromArgb(19, 90, 42);
            tbThreads.ThumbOutlineThickness = 2;
            tbThreads.Value = 50;
            tbThreads.Scroll += tbThreads_Scroll;
            tbThreads.ValueChanged += tbThreads_ValueChanged;
            // 
            // brutalGradientPanel2
            // 
            brutalGradientPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            brutalGradientPanel2.BackColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel2.Controls.Add(tableLayoutPanel3);
            brutalGradientPanel2.EndColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel2.ForeColor = Color.White;
            brutalGradientPanel2.GlowCenterMaxOpacity = 200;
            brutalGradientPanel2.GlowCenterMinOpacity = 50;
            brutalGradientPanel2.GlowMinSurroundOpacity = 30;
            brutalGradientPanel2.GradientEndSide = GradientSide.Top;
            brutalGradientPanel2.GradientStartSide = GradientSide.Bottom;
            brutalGradientPanel2.Location = new Point(-1, -1);
            brutalGradientPanel2.Margin = new Padding(0);
            brutalGradientPanel2.MouseEvents = true;
            brutalGradientPanel2.Name = "brutalGradientPanel2";
            brutalGradientPanel2.Size = new Size(1003, 30);
            brutalGradientPanel2.StartColor = Color.FromArgb(64, 64, 64);
            brutalGradientPanel2.TabIndex = 6;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.BackColor = Color.Transparent;
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40.8163261F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18.36735F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40.81633F));
            tableLayoutPanel3.Controls.Add(tableLayoutPanel4, 1, 0);
            tableLayoutPanel3.Controls.Add(panel1, 2, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(1003, 30);
            tableLayoutPanel3.TabIndex = 0;
            tableLayoutPanel3.Paint += tableLayoutPanel3_Paint;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(TabButtonExtract, 1, 0);
            tableLayoutPanel4.Controls.Add(tabButtonCreate, 0, 0);
            tableLayoutPanel4.Location = new Point(409, 0);
            tableLayoutPanel4.Margin = new Padding(0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Size = new Size(184, 30);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // TabButtonExtract
            // 
            TabButtonExtract.Appearance = Appearance.Button;
            TabButtonExtract.Dock = DockStyle.Fill;
            TabButtonExtract.FlatAppearance.BorderSize = 0;
            TabButtonExtract.FlatAppearance.CheckedBackColor = Color.FromArgb(32, 64, 32);
            TabButtonExtract.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            TabButtonExtract.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            TabButtonExtract.FlatStyle = FlatStyle.Flat;
            TabButtonExtract.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            TabButtonExtract.Location = new Point(92, 0);
            TabButtonExtract.Margin = new Padding(0);
            TabButtonExtract.Name = "TabButtonExtract";
            TabButtonExtract.Size = new Size(92, 30);
            TabButtonExtract.TabIndex = 8;
            TabButtonExtract.Text = "Extract";
            TabButtonExtract.TextAlign = ContentAlignment.MiddleCenter;
            TabButtonExtract.UseVisualStyleBackColor = true;
            TabButtonExtract.CheckedChanged += TabButtonExtract_CheckedChanged;
            // 
            // tabButtonCreate
            // 
            tabButtonCreate.Appearance = Appearance.Button;
            tabButtonCreate.Checked = true;
            tabButtonCreate.CheckState = CheckState.Checked;
            tabButtonCreate.Dock = DockStyle.Fill;
            tabButtonCreate.FlatAppearance.BorderSize = 0;
            tabButtonCreate.FlatAppearance.CheckedBackColor = Color.FromArgb(32, 64, 32);
            tabButtonCreate.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            tabButtonCreate.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            tabButtonCreate.FlatStyle = FlatStyle.Flat;
            tabButtonCreate.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            tabButtonCreate.Location = new Point(0, 0);
            tabButtonCreate.Margin = new Padding(0);
            tabButtonCreate.Name = "tabButtonCreate";
            tabButtonCreate.Size = new Size(92, 30);
            tabButtonCreate.TabIndex = 7;
            tabButtonCreate.Text = "Create";
            tabButtonCreate.TextAlign = ContentAlignment.MiddleCenter;
            tabButtonCreate.UseVisualStyleBackColor = true;
            tabButtonCreate.CheckedChanged += tabButtonCreate_CheckedChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(label2);
            panel1.Controls.Add(tbThreads);
            panel1.Controls.Add(lblThreadsValue);
            panel1.Controls.Add(chkThreadsAutoMain);
            panel1.Controls.Add(label3);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(596, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(404, 24);
            panel1.TabIndex = 1;
            // 
            // label2
            // 
            label2.Dock = DockStyle.Right;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(101, 0);
            label2.Name = "label2";
            label2.Size = new Size(62, 24);
            label2.TabIndex = 1;
            label2.Text = "Threads:";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblThreadsValue
            // 
            lblThreadsValue.Dock = DockStyle.Right;
            lblThreadsValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblThreadsValue.Location = new Point(306, 0);
            lblThreadsValue.Name = "lblThreadsValue";
            lblThreadsValue.Size = new Size(40, 24);
            lblThreadsValue.TabIndex = 0;
            lblThreadsValue.Text = "100";
            lblThreadsValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // chkThreadsAutoMain
            // 
            chkThreadsAutoMain.BackColor = Color.Transparent;
            chkThreadsAutoMain.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkThreadsAutoMain.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkThreadsAutoMain.BoxGradientEnabled = true;
            chkThreadsAutoMain.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkThreadsAutoMain.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkThreadsAutoMain.BoxSize = 14;
            chkThreadsAutoMain.CheckBorderColor = Color.Lime;
            chkThreadsAutoMain.CheckColor = Color.LawnGreen;
            chkThreadsAutoMain.Checked = true;
            chkThreadsAutoMain.CheckGradientEnabled = true;
            chkThreadsAutoMain.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkThreadsAutoMain.CheckGradientStart = Color.Lime;
            chkThreadsAutoMain.CheckState = CheckState.Checked;
            chkThreadsAutoMain.Dock = DockStyle.Right;
            chkThreadsAutoMain.Location = new Point(346, 0);
            chkThreadsAutoMain.Name = "chkThreadsAutoMain";
            chkThreadsAutoMain.Size = new Size(18, 24);
            chkThreadsAutoMain.TabIndex = 9;
            chkThreadsAutoMain.UseVisualStyleBackColor = false;
            chkThreadsAutoMain.CheckedChanged += chkThreadsAutoMain_CheckedChanged;
            // 
            // label3
            // 
            label3.Dock = DockStyle.Right;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(364, 0);
            label3.Name = "label3";
            label3.Size = new Size(40, 24);
            label3.TabIndex = 10;
            label3.Text = "Auto";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // GradientPanelExtractRibbon
            // 
            GradientPanelExtractRibbon.BackColor = Color.FromArgb(32, 32, 32);
            GradientPanelExtractRibbon.Controls.Add(ribbonControlsExtract);
            GradientPanelExtractRibbon.Dock = DockStyle.Top;
            GradientPanelExtractRibbon.EndColor = Color.FromArgb(64, 64, 64);
            GradientPanelExtractRibbon.ForeColor = Color.White;
            GradientPanelExtractRibbon.GlowCenterMaxOpacity = 200;
            GradientPanelExtractRibbon.GlowCenterMinOpacity = 50;
            GradientPanelExtractRibbon.GlowMinSurroundOpacity = 30;
            GradientPanelExtractRibbon.GradientEndSide = GradientSide.Bottom;
            GradientPanelExtractRibbon.GradientStartSide = GradientSide.Top;
            GradientPanelExtractRibbon.Location = new Point(0, 0);
            GradientPanelExtractRibbon.Margin = new Padding(0);
            GradientPanelExtractRibbon.MouseEvents = true;
            GradientPanelExtractRibbon.Name = "GradientPanelExtractRibbon";
            GradientPanelExtractRibbon.Size = new Size(1000, 82);
            GradientPanelExtractRibbon.StartColor = Color.FromArgb(32, 32, 32);
            GradientPanelExtractRibbon.TabIndex = 7;
            // 
            // ribbonControlsExtract
            // 
            ribbonControlsExtract.BackColor = Color.Transparent;
            ribbonControlsExtract.Controls.Add(panel12);
            ribbonControlsExtract.Controls.Add(buttonWizardExtract);
            ribbonControlsExtract.Controls.Add(buttonTestZip);
            ribbonControlsExtract.Controls.Add(buttonOpenZip);
            ribbonControlsExtract.Location = new Point(-1, -1);
            ribbonControlsExtract.Name = "ribbonControlsExtract";
            ribbonControlsExtract.Size = new Size(593, 84);
            ribbonControlsExtract.TabIndex = 7;
            // 
            // panel12
            // 
            panel12.BackColor = Color.FromArgb(32, 32, 32);
            panel12.Dock = DockStyle.Left;
            panel12.Location = new Point(255, 0);
            panel12.Name = "panel12";
            panel12.Padding = new Padding(0, 0, 0, 2);
            panel12.Size = new Size(2, 84);
            panel12.TabIndex = 25;
            // 
            // buttonWizardExtract
            // 
            buttonWizardExtract.BackColor = Color.Transparent;
            buttonWizardExtract.Dock = DockStyle.Left;
            buttonWizardExtract.FlatAppearance.BorderSize = 0;
            buttonWizardExtract.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonWizardExtract.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonWizardExtract.FlatStyle = FlatStyle.Flat;
            buttonWizardExtract.Location = new Point(170, 0);
            buttonWizardExtract.Name = "buttonWizardExtract";
            buttonWizardExtract.Size = new Size(85, 84);
            buttonWizardExtract.TabIndex = 22;
            buttonWizardExtract.Text = "Wizard";
            buttonWizardExtract.TextAlign = ContentAlignment.BottomCenter;
            buttonWizardExtract.UseVisualStyleBackColor = false;
            buttonWizardExtract.Click += buttonWizardExtract_Click;
            // 
            // buttonTestZip
            // 
            buttonTestZip.BackColor = Color.Transparent;
            buttonTestZip.Dock = DockStyle.Left;
            buttonTestZip.FlatAppearance.BorderSize = 0;
            buttonTestZip.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonTestZip.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonTestZip.FlatStyle = FlatStyle.Flat;
            buttonTestZip.Location = new Point(85, 0);
            buttonTestZip.Name = "buttonTestZip";
            buttonTestZip.Size = new Size(85, 84);
            buttonTestZip.TabIndex = 19;
            buttonTestZip.Text = "Test Zip";
            buttonTestZip.TextAlign = ContentAlignment.BottomCenter;
            buttonTestZip.UseVisualStyleBackColor = false;
            // 
            // buttonOpenZip
            // 
            buttonOpenZip.BackColor = Color.Transparent;
            buttonOpenZip.Dock = DockStyle.Left;
            buttonOpenZip.FlatAppearance.BorderSize = 0;
            buttonOpenZip.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonOpenZip.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonOpenZip.FlatStyle = FlatStyle.Flat;
            buttonOpenZip.Location = new Point(0, 0);
            buttonOpenZip.Name = "buttonOpenZip";
            buttonOpenZip.Size = new Size(85, 84);
            buttonOpenZip.TabIndex = 14;
            buttonOpenZip.Text = "Open Zip";
            buttonOpenZip.TextAlign = ContentAlignment.BottomCenter;
            buttonOpenZip.UseVisualStyleBackColor = false;
            buttonOpenZip.Click += buttonOpenZip_Click;
            // 
            // brutalGradientPanel4
            // 
            brutalGradientPanel4.BackColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel4.Controls.Add(tableLayoutPanel1);
            brutalGradientPanel4.Dock = DockStyle.Top;
            brutalGradientPanel4.EndColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel4.ForeColor = Color.White;
            brutalGradientPanel4.GlowCenterMaxOpacity = 200;
            brutalGradientPanel4.GlowCenterMinOpacity = 50;
            brutalGradientPanel4.GlowMinSurroundOpacity = 30;
            brutalGradientPanel4.GradientEndSide = GradientSide.Top;
            brutalGradientPanel4.GradientStartSide = GradientSide.Bottom;
            brutalGradientPanel4.Location = new Point(0, 82);
            brutalGradientPanel4.Margin = new Padding(0);
            brutalGradientPanel4.MouseEvents = true;
            brutalGradientPanel4.Name = "brutalGradientPanel4";
            brutalGradientPanel4.Size = new Size(1000, 16);
            brutalGradientPanel4.StartColor = Color.FromArgb(64, 64, 64);
            brutalGradientPanel4.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.Transparent;
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 255F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 177F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 49F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8F));
            tableLayoutPanel1.Controls.Add(label16, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1000, 16);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Dock = DockStyle.Fill;
            label16.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label16.Location = new Point(3, 0);
            label16.Name = "label16";
            label16.Size = new Size(249, 16);
            label16.TabIndex = 0;
            label16.Text = "Extract";
            label16.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelCreateTabInner
            // 
            panelCreateTabInner.Controls.Add(splitContainerCreate);
            panelCreateTabInner.Dock = DockStyle.Fill;
            panelCreateTabInner.Location = new Point(0, 99);
            panelCreateTabInner.Name = "panelCreateTabInner";
            panelCreateTabInner.Size = new Size(1000, 573);
            panelCreateTabInner.TabIndex = 17;
            // 
            // splitContainerCreate
            // 
            splitContainerCreate.Dock = DockStyle.Fill;
            splitContainerCreate.Location = new Point(0, 0);
            splitContainerCreate.Name = "splitContainerCreate";
            // 
            // splitContainerCreate.Panel1
            // 
            splitContainerCreate.Panel1.Controls.Add(treeViewExplorer);
            // 
            // splitContainerCreate.Panel2
            // 
            splitContainerCreate.Panel2.Controls.Add(tableLayoutPanel5);
            splitContainerCreate.Size = new Size(1000, 573);
            splitContainerCreate.SplitterDistance = 275;
            splitContainerCreate.TabIndex = 0;
            // 
            // treeViewExplorer
            // 
            treeViewExplorer.BackColor = Color.FromArgb(25, 25, 25);
            treeViewExplorer.BorderStyle = BorderStyle.None;
            treeViewExplorer.Dock = DockStyle.Fill;
            treeViewExplorer.ForeColor = Color.FromArgb(222, 222, 223);
            treeViewExplorer.LineColor = Color.FromArgb(29, 181, 82);
            treeViewExplorer.Location = new Point(0, 0);
            treeViewExplorer.Margin = new Padding(3, 0, 3, 3);
            treeViewExplorer.Name = "treeViewExplorer";
            treeViewExplorer.SelectedNodes = null;
            treeViewExplorer.Size = new Size(275, 573);
            treeViewExplorer.TabIndex = 0;
            treeViewExplorer.BeforeExpand += treeViewExplorer_BeforeExpand;
            treeViewExplorer.ItemDrag += treeViewExplorer_ItemDrag;
            treeViewExplorer.NodeMouseClick += treeViewExplorer_NodeMouseClick;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 1;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Controls.Add(lvStaging, 0, 0);
            tableLayoutPanel5.Controls.Add(panel7, 0, 1);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(0, 0);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 2;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Absolute, 39F));
            tableLayoutPanel5.Size = new Size(721, 573);
            tableLayoutPanel5.TabIndex = 0;
            // 
            // panel7
            // 
            panel7.Controls.Add(lblCreateHint);
            panel7.Controls.Add(btnCreateQuick);
            panel7.Controls.Add(btnCreate);
            panel7.Dock = DockStyle.Fill;
            panel7.Location = new Point(3, 537);
            panel7.Name = "panel7";
            panel7.Size = new Size(715, 33);
            panel7.TabIndex = 17;
            // 
            // btnCreateQuick
            // 
            btnCreateQuick.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCreateQuick.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCreateQuick.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCreateQuick.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCreateQuick.FlatStyle = FlatStyle.Flat;
            btnCreateQuick.Location = new Point(552, 3);
            btnCreateQuick.Name = "btnCreateQuick";
            btnCreateQuick.Size = new Size(160, 28);
            btnCreateQuick.TabIndex = 15;
            btnCreateQuick.Text = "Compress to <auto>";
            btnCreateQuick.Click += btnCreateQuick_Click_1;
            // 
            // panelCreateZipTab
            // 
            panelCreateZipTab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelCreateZipTab.Controls.Add(panelCreateTabInner);
            panelCreateZipTab.Controls.Add(brutalGradientPanel3);
            panelCreateZipTab.Controls.Add(GradientPanelCreateRibbon);
            panelCreateZipTab.Location = new Point(0, 28);
            panelCreateZipTab.Name = "panelCreateZipTab";
            panelCreateZipTab.Size = new Size(1000, 672);
            panelCreateZipTab.TabIndex = 18;
            // 
            // panelExtractZipTab
            // 
            panelExtractZipTab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelExtractZipTab.Controls.Add(tableLayoutPanel6);
            panelExtractZipTab.Controls.Add(brutalGradientPanel4);
            panelExtractZipTab.Controls.Add(GradientPanelExtractRibbon);
            panelExtractZipTab.Location = new Point(0, 28);
            panelExtractZipTab.Name = "panelExtractZipTab";
            panelExtractZipTab.Size = new Size(1000, 672);
            panelExtractZipTab.TabIndex = 19;
            panelExtractZipTab.Visible = false;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 1;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.Controls.Add(pnlExtractDrop, 0, 0);
            tableLayoutPanel6.Controls.Add(panel9, 0, 1);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(0, 98);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayoutPanel6.Size = new Size(1000, 574);
            tableLayoutPanel6.TabIndex = 8;
            // 
            // panel9
            // 
            panel9.Controls.Add(rdoExtractToFolderName);
            panel9.Controls.Add(lblExtractDest);
            panel9.Controls.Add(rdoExtractHere);
            panel9.Controls.Add(btnExtract);
            panel9.Dock = DockStyle.Fill;
            panel9.Location = new Point(3, 539);
            panel9.Name = "panel9";
            panel9.Size = new Size(994, 32);
            panel9.TabIndex = 1;
            // 
            // HomeView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(32, 32, 32);
            Controls.Add(brutalGradientPanel2);
            Controls.Add(panelCreateZipTab);
            Controls.Add(panelExtractZipTab);
            ForeColor = Color.White;
            Name = "HomeView";
            Size = new Size(1000, 700);
            Load += HomeView_Load;
            ((ISupportInitialize)numCreateLevel).EndInit();
            cmsStaging.ResumeLayout(false);
            pnlExtractDrop.ResumeLayout(false);
            GradientPanelCreateRibbon.ResumeLayout(false);
            ribbonControlsCreate.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel3.ResumeLayout(false);
            brutalGradientPanel3.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((ISupportInitialize)tbThreads).EndInit();
            brutalGradientPanel2.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            GradientPanelExtractRibbon.ResumeLayout(false);
            ribbonControlsExtract.ResumeLayout(false);
            brutalGradientPanel4.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panelCreateTabInner.ResumeLayout(false);
            splitContainerCreate.Panel1.ResumeLayout(false);
            splitContainerCreate.Panel2.ResumeLayout(false);
            ((ISupportInitialize)splitContainerCreate).EndInit();
            splitContainerCreate.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            panel7.ResumeLayout(false);
            panelCreateZipTab.ResumeLayout(false);
            panelExtractZipTab.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            panel9.ResumeLayout(false);
            ResumeLayout(false);
        }
        private BrutalZip2025.BrutalControls.BrutalGradientPanel GradientPanelCreateRibbon;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private CheckBox tabButtonCreate;
        private Panel ribbonControlsCreate;
        private Button buttonAddFolder;
        private Button buttonAddFiles;
        private TableLayoutPanel tableLayoutPanel4;
        private CheckBox TabButtonExtract;
        private Button buttonComment;
        private BrutalZip2025.BrutalControls.BrutalTrackBar tbThreads;
        private Button buttonWizardCreate;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel3;
        private Panel panel1;
        private Label label2;
        private Label lblThreadsValue;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkThreadsAutoMain;
        private Label label3;
        private Panel panel2;
        private Label label5;
        private Panel panel4;
        private Panel panel3;
        private Label label6;
        private Panel panel5;
        private Label label8;
        private BrutalZip2025.BrutalControls.BrutalComboBox cmbEncrypt;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkEncrypt;
        private Label label7;
        private Panel panel6;
        private Button btnCreateSetPassword;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label9;
        private Label label4;
        private Label label1;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel GradientPanelExtractRibbon;
        private Panel ribbonControlsExtract;
        private Panel panel12;
        private Button buttonWizardExtract;
        private Button buttonTestZip;
        private Button buttonOpenZip;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel4;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label16;
        private Panel panelCreateTabInner;
        private SplitContainer splitContainerCreate;
        private TreeViewMS.TreeViewMS treeViewExplorer;
        private TableLayoutPanel tableLayoutPanel5;
        private Panel panel7;
        private Panel panelCreateZipTab;
        private Panel panelExtractZipTab;
        private TableLayoutPanel tableLayoutPanel6;
        private Panel panel9;
        internal Button btnCreateQuick;
    }
}
