using BrutalZip2025.BrutalControls;
using System.ComponentModel;

namespace Brutal_Zip.Views
{
    partial class HomeView
    {
        private IContainer components = null;

        // Create group
        private GroupBox grpCreate;
        internal Panel pnlCreateDrop;
        private Label lblCreateDrop;
        internal Button btnCreateAddFiles;
        internal Button btnCreateAddFolder;
        private Label lblCreateDest;
        internal TextBox txtCreateDest;
        internal Button btnCreateBrowse;
        private Label lblCreateMethod;
        internal BrutalComboBox cmbCreateMethod;
        private Label lblCreateLevel;
        internal NumericUpDown numCreateLevel;
        internal Button btnCreate;
        internal Button btnCreateQuick; // NEW
        internal Label lblCreateHint;

        // Threads slider (Create section)
        private Label lblThreadsText;
        internal TrackBar tbThreads;
        internal Label lblThreadsValue;


        // NEW encryption UI
        internal CheckBox chkEncrypt;
        private Label lblEncrypt;
        internal ComboBox cmbEncrypt;
        internal Button btnCreateSetPassword;
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

        // Extract group
        private GroupBox grpExtract;
        internal Panel pnlExtractDrop;
        private Label lblExtractDrop;
        internal Button btnOpenArchive;
        internal RadioButton rdoExtractToFolderName;
        internal RadioButton rdoExtractHere;
        private Label lblExtractDest;
        internal TextBox txtExtractDest;
        internal Button btnExtractBrowse;
        internal Button btnExtract;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(HomeView));
            grpCreate = new GroupBox();
            pnlCreateDrop = new Panel();
            lblCreateDrop = new Label();
            btnCreateAddFiles = new Button();
            btnCreateAddFolder = new Button();
            lblCreateMethod = new Label();
            lblCreateLevel = new Label();
            numCreateLevel = new NumericUpDown();
            lblThreadsText = new Label();
            lblThreadsValue = new Label();
            chkEncrypt = new CheckBox();
            lblEncrypt = new Label();
            cmbEncrypt = new ComboBox();
            btnCreateSetPassword = new Button();
            tbThreads = new TrackBar();
            lblCreateDest = new Label();
            txtCreateDest = new TextBox();
            btnCreateBrowse = new Button();
            btnCreate = new Button();
            btnCreateQuick = new Button();
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
            grpExtract = new GroupBox();
            pnlExtractDrop = new Panel();
            lblExtractDrop = new Label();
            btnOpenArchive = new Button();
            rdoExtractToFolderName = new RadioButton();
            rdoExtractHere = new RadioButton();
            lblExtractDest = new Label();
            txtExtractDest = new TextBox();
            btnExtractBrowse = new Button();
            btnExtract = new Button();
            GradientPanelCreateRibbon = new BrutalGradientPanel();
            ribbonControlsCreate = new Panel();
            panel6 = new Panel();
            buttonSetPassword = new Button();
            panel5 = new Panel();
            label8 = new Label();
            comboBoxEncryptionMethod = new BrutalComboBox();
            checkboxEncrypt = new BrutalCheckBox();
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
            trackBarThreads = new BrutalTrackBar();
            brutalGradientPanel2 = new BrutalGradientPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            TabButtonExtract = new CheckBox();
            tabButtonCreate = new CheckBox();
            panel1 = new Panel();
            label2 = new Label();
            labelThreadsMax = new Label();
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
            panelCreateTab = new Panel();
            splitContainerCreate = new SplitContainer();
            treeViewExplorer = new TreeViewMS.TreeViewMS();
            tableLayoutPanel5 = new TableLayoutPanel();
            panel7 = new Panel();
            grpCreate.SuspendLayout();
            pnlCreateDrop.SuspendLayout();
            ((ISupportInitialize)numCreateLevel).BeginInit();
            ((ISupportInitialize)tbThreads).BeginInit();
            cmsStaging.SuspendLayout();
            grpExtract.SuspendLayout();
            pnlExtractDrop.SuspendLayout();
            GradientPanelCreateRibbon.SuspendLayout();
            ribbonControlsCreate.SuspendLayout();
            panel5.SuspendLayout();
            panel3.SuspendLayout();
            brutalGradientPanel3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((ISupportInitialize)trackBarThreads).BeginInit();
            brutalGradientPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            panel1.SuspendLayout();
            GradientPanelExtractRibbon.SuspendLayout();
            ribbonControlsExtract.SuspendLayout();
            brutalGradientPanel4.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panelCreateTab.SuspendLayout();
            ((ISupportInitialize)splitContainerCreate).BeginInit();
            splitContainerCreate.Panel1.SuspendLayout();
            splitContainerCreate.Panel2.SuspendLayout();
            splitContainerCreate.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            panel7.SuspendLayout();
            SuspendLayout();
            // 
            // grpCreate
            // 
            grpCreate.Controls.Add(pnlCreateDrop);
            grpCreate.Controls.Add(btnCreateAddFiles);
            grpCreate.Controls.Add(btnCreateAddFolder);
            grpCreate.Controls.Add(lblCreateMethod);
            grpCreate.Controls.Add(lblCreateLevel);
            grpCreate.Controls.Add(lblThreadsText);
            grpCreate.Controls.Add(lblThreadsValue);
            grpCreate.Controls.Add(chkEncrypt);
            grpCreate.Controls.Add(lblEncrypt);
            grpCreate.Controls.Add(cmbEncrypt);
            grpCreate.Controls.Add(btnCreateSetPassword);
            grpCreate.Controls.Add(tbThreads);
            grpCreate.ForeColor = Color.White;
            grpCreate.Location = new Point(566, 136);
            grpCreate.Name = "grpCreate";
            grpCreate.Size = new Size(450, 517);
            grpCreate.TabIndex = 1;
            grpCreate.TabStop = false;
            grpCreate.Text = "Create";
            grpCreate.Enter += grpCreate_Enter;
            // 
            // pnlCreateDrop
            // 
            pnlCreateDrop.BackColor = Color.WhiteSmoke;
            pnlCreateDrop.BorderStyle = BorderStyle.FixedSingle;
            pnlCreateDrop.Controls.Add(lblCreateDrop);
            pnlCreateDrop.Location = new Point(20, 30);
            pnlCreateDrop.Name = "pnlCreateDrop";
            pnlCreateDrop.Size = new Size(410, 200);
            pnlCreateDrop.TabIndex = 0;
            // 
            // lblCreateDrop
            // 
            lblCreateDrop.Dock = DockStyle.Fill;
            lblCreateDrop.Location = new Point(0, 0);
            lblCreateDrop.Name = "lblCreateDrop";
            lblCreateDrop.Size = new Size(408, 198);
            lblCreateDrop.TabIndex = 0;
            lblCreateDrop.Text = "Drop files/folders here to create a ZIP";
            lblCreateDrop.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnCreateAddFiles
            // 
            btnCreateAddFiles.Location = new Point(20, 240);
            btnCreateAddFiles.Name = "btnCreateAddFiles";
            btnCreateAddFiles.Size = new Size(110, 28);
            btnCreateAddFiles.TabIndex = 1;
            btnCreateAddFiles.Text = "Add files…";
            // 
            // btnCreateAddFolder
            // 
            btnCreateAddFolder.Location = new Point(140, 240);
            btnCreateAddFolder.Name = "btnCreateAddFolder";
            btnCreateAddFolder.Size = new Size(110, 28);
            btnCreateAddFolder.TabIndex = 2;
            btnCreateAddFolder.Text = "Add folder…";
            // 
            // lblCreateMethod
            // 
            lblCreateMethod.Location = new Point(20, 340);
            lblCreateMethod.Name = "lblCreateMethod";
            lblCreateMethod.Size = new Size(60, 20);
            lblCreateMethod.TabIndex = 6;
            lblCreateMethod.Text = "Method:";
            // 
            // lblCreateLevel
            // 
            lblCreateLevel.Location = new Point(210, 340);
            lblCreateLevel.Name = "lblCreateLevel";
            lblCreateLevel.Size = new Size(40, 20);
            lblCreateLevel.TabIndex = 8;
            lblCreateLevel.Text = "Level:";
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
            // lblThreadsText
            // 
            lblThreadsText.Location = new Point(20, 368);
            lblThreadsText.Name = "lblThreadsText";
            lblThreadsText.Size = new Size(60, 20);
            lblThreadsText.TabIndex = 10;
            lblThreadsText.Text = "Threads:";
            // 
            // lblThreadsValue
            // 
            lblThreadsValue.Location = new Point(340, 364);
            lblThreadsValue.Name = "lblThreadsValue";
            lblThreadsValue.Size = new Size(32, 20);
            lblThreadsValue.TabIndex = 12;
            lblThreadsValue.Text = "1";
            // 
            // chkEncrypt
            // 
            chkEncrypt.Location = new Point(20, 400);
            chkEncrypt.Name = "chkEncrypt";
            chkEncrypt.Size = new Size(200, 20);
            chkEncrypt.TabIndex = 14;
            chkEncrypt.Text = "Encrypt archive";
            // 
            // lblEncrypt
            // 
            lblEncrypt.Location = new Point(20, 426);
            lblEncrypt.Name = "lblEncrypt";
            lblEncrypt.Size = new Size(60, 20);
            lblEncrypt.TabIndex = 15;
            lblEncrypt.Text = "Algo:";
            // 
            // cmbEncrypt
            // 
            cmbEncrypt.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEncrypt.Items.AddRange(new object[] { "ZipCrypto", "AES-128", "AES-192", "AES-256" });
            cmbEncrypt.Location = new Point(80, 423);
            cmbEncrypt.Name = "cmbEncrypt";
            cmbEncrypt.Size = new Size(150, 23);
            cmbEncrypt.TabIndex = 16;
            // 
            // btnCreateSetPassword
            // 
            btnCreateSetPassword.Location = new Point(240, 421);
            btnCreateSetPassword.Name = "btnCreateSetPassword";
            btnCreateSetPassword.Size = new Size(100, 26);
            btnCreateSetPassword.TabIndex = 17;
            btnCreateSetPassword.Text = "Set password…";
            // 
            // tbThreads
            // 
            tbThreads.LargeChange = 1;
            tbThreads.Location = new Point(80, 363);
            tbThreads.Maximum = 8;
            tbThreads.Minimum = 1;
            tbThreads.Name = "tbThreads";
            tbThreads.Size = new Size(250, 45);
            tbThreads.TabIndex = 11;
            tbThreads.Value = 1;
            // 
            // lblCreateDest
            // 
            lblCreateDest.Location = new Point(3, 4);
            lblCreateDest.Name = "lblCreateDest";
            lblCreateDest.Size = new Size(80, 21);
            lblCreateDest.TabIndex = 3;
            lblCreateDest.Text = "Destination:";
            // 
            // txtCreateDest
            // 
            txtCreateDest.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtCreateDest.BackColor = Color.FromArgb(32, 32, 32);
            txtCreateDest.BorderStyle = BorderStyle.FixedSingle;
            txtCreateDest.ForeColor = Color.White;
            txtCreateDest.Location = new Point(89, 2);
            txtCreateDest.Name = "txtCreateDest";
            txtCreateDest.Size = new Size(231, 23);
            txtCreateDest.TabIndex = 4;
            // 
            // btnCreateBrowse
            // 
            btnCreateBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCreateBrowse.FlatAppearance.BorderSize = 0;
            btnCreateBrowse.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCreateBrowse.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCreateBrowse.FlatStyle = FlatStyle.Flat;
            btnCreateBrowse.Location = new Point(326, 1);
            btnCreateBrowse.Name = "btnCreateBrowse";
            btnCreateBrowse.Size = new Size(33, 26);
            btnCreateBrowse.TabIndex = 5;
            btnCreateBrowse.Text = "...";
            // 
            // btnCreate
            // 
            btnCreate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCreate.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCreate.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.Location = new Point(89, 30);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(100, 28);
            btnCreate.TabIndex = 14;
            btnCreate.Text = "Create";
            btnCreate.Click += btnCreate_Click;
            // 
            // btnCreateQuick
            // 
            btnCreateQuick.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCreateQuick.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCreateQuick.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCreateQuick.FlatStyle = FlatStyle.Flat;
            btnCreateQuick.Location = new Point(199, 30);
            btnCreateQuick.Name = "btnCreateQuick";
            btnCreateQuick.Size = new Size(160, 28);
            btnCreateQuick.TabIndex = 15;
            btnCreateQuick.Text = "Compress to <auto>";
            // 
            // lblCreateHint
            // 
            lblCreateHint.AutoEllipsis = true;
            lblCreateHint.Dock = DockStyle.Bottom;
            lblCreateHint.Location = new Point(0, 62);
            lblCreateHint.Name = "lblCreateHint";
            lblCreateHint.Size = new Size(362, 20);
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
            lvStaging.BorderStyle = BorderStyle.None;
            lvStaging.Columns.AddRange(new ColumnHeader[] { colStName, colStType, colStSize, colStItems, colStPath });
            lvStaging.ContextMenuStrip = cmsStaging;
            lvStaging.Dock = DockStyle.Fill;
            lvStaging.FullRowSelect = true;
            lvStaging.Location = new Point(3, 3);
            lvStaging.Name = "lvStaging";
            lvStaging.Size = new Size(362, 272);
            lvStaging.TabIndex = 16;
            lvStaging.UseCompatibleStateImageBehavior = false;
            lvStaging.View = View.Details;
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
            // 
            // mnuStagingRemoveMissing
            // 
            mnuStagingRemoveMissing.Name = "mnuStagingRemoveMissing";
            mnuStagingRemoveMissing.Size = new Size(163, 22);
            mnuStagingRemoveMissing.Text = "Remove missing";
            // 
            // mnuStagingClear
            // 
            mnuStagingClear.Name = "mnuStagingClear";
            mnuStagingClear.Size = new Size(163, 22);
            mnuStagingClear.Text = "Clear all";
            // 
            // grpExtract
            // 
            grpExtract.Controls.Add(pnlExtractDrop);
            grpExtract.Controls.Add(btnOpenArchive);
            grpExtract.Controls.Add(rdoExtractToFolderName);
            grpExtract.Controls.Add(rdoExtractHere);
            grpExtract.Controls.Add(lblExtractDest);
            grpExtract.Controls.Add(txtExtractDest);
            grpExtract.Controls.Add(btnExtractBrowse);
            grpExtract.Controls.Add(btnExtract);
            grpExtract.ForeColor = Color.White;
            grpExtract.Location = new Point(959, 150);
            grpExtract.Name = "grpExtract";
            grpExtract.Size = new Size(450, 466);
            grpExtract.TabIndex = 2;
            grpExtract.TabStop = false;
            grpExtract.Text = "Extract";
            // 
            // pnlExtractDrop
            // 
            pnlExtractDrop.BackColor = Color.WhiteSmoke;
            pnlExtractDrop.BorderStyle = BorderStyle.FixedSingle;
            pnlExtractDrop.Controls.Add(lblExtractDrop);
            pnlExtractDrop.Location = new Point(20, 30);
            pnlExtractDrop.Name = "pnlExtractDrop";
            pnlExtractDrop.Size = new Size(410, 200);
            pnlExtractDrop.TabIndex = 0;
            // 
            // lblExtractDrop
            // 
            lblExtractDrop.Dock = DockStyle.Fill;
            lblExtractDrop.Location = new Point(0, 0);
            lblExtractDrop.Name = "lblExtractDrop";
            lblExtractDrop.Size = new Size(408, 198);
            lblExtractDrop.TabIndex = 0;
            lblExtractDrop.Text = "Drop .zip files here to extract or open";
            lblExtractDrop.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnOpenArchive
            // 
            btnOpenArchive.Location = new Point(20, 240);
            btnOpenArchive.Name = "btnOpenArchive";
            btnOpenArchive.Size = new Size(110, 28);
            btnOpenArchive.TabIndex = 1;
            btnOpenArchive.Text = "Open archive…";
            // 
            // rdoExtractToFolderName
            // 
            rdoExtractToFolderName.Checked = true;
            rdoExtractToFolderName.Location = new Point(20, 280);
            rdoExtractToFolderName.Name = "rdoExtractToFolderName";
            rdoExtractToFolderName.Size = new Size(250, 20);
            rdoExtractToFolderName.TabIndex = 2;
            rdoExtractToFolderName.TabStop = true;
            rdoExtractToFolderName.Text = "Extract to “ArchiveName/”";
            // 
            // rdoExtractHere
            // 
            rdoExtractHere.Location = new Point(20, 305);
            rdoExtractHere.Name = "rdoExtractHere";
            rdoExtractHere.Size = new Size(250, 20);
            rdoExtractHere.TabIndex = 3;
            rdoExtractHere.Text = "Extract here";
            // 
            // lblExtractDest
            // 
            lblExtractDest.Location = new Point(20, 340);
            lblExtractDest.Name = "lblExtractDest";
            lblExtractDest.Size = new Size(80, 20);
            lblExtractDest.TabIndex = 4;
            lblExtractDest.Text = "Destination:";
            // 
            // txtExtractDest
            // 
            txtExtractDest.Location = new Point(20, 360);
            txtExtractDest.Name = "txtExtractDest";
            txtExtractDest.Size = new Size(330, 23);
            txtExtractDest.TabIndex = 5;
            // 
            // btnExtractBrowse
            // 
            btnExtractBrowse.Location = new Point(360, 358);
            btnExtractBrowse.Name = "btnExtractBrowse";
            btnExtractBrowse.Size = new Size(70, 26);
            btnExtractBrowse.TabIndex = 6;
            btnExtractBrowse.Text = "Browse…";
            // 
            // btnExtract
            // 
            btnExtract.Location = new Point(20, 400);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new Size(100, 28);
            btnExtract.TabIndex = 7;
            btnExtract.Text = "Extract";
            // 
            // GradientPanelCreateRibbon
            // 
            GradientPanelCreateRibbon.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GradientPanelCreateRibbon.BackColor = Color.FromArgb(32, 32, 32);
            GradientPanelCreateRibbon.Controls.Add(ribbonControlsCreate);
            GradientPanelCreateRibbon.Controls.Add(brutalGradientPanel3);
            GradientPanelCreateRibbon.EndColor = Color.FromArgb(64, 64, 64);
            GradientPanelCreateRibbon.ForeColor = Color.White;
            GradientPanelCreateRibbon.GlowCenterMaxOpacity = 200;
            GradientPanelCreateRibbon.GlowCenterMinOpacity = 50;
            GradientPanelCreateRibbon.GlowMinSurroundOpacity = 30;
            GradientPanelCreateRibbon.GradientEndSide = GradientSide.Bottom;
            GradientPanelCreateRibbon.GradientStartSide = GradientSide.Top;
            GradientPanelCreateRibbon.Location = new Point(-1, 28);
            GradientPanelCreateRibbon.Margin = new Padding(0);
            GradientPanelCreateRibbon.MouseEvents = true;
            GradientPanelCreateRibbon.Name = "GradientPanelCreateRibbon";
            GradientPanelCreateRibbon.Size = new Size(1003, 99);
            GradientPanelCreateRibbon.StartColor = Color.FromArgb(32, 32, 32);
            GradientPanelCreateRibbon.TabIndex = 5;
            // 
            // ribbonControlsCreate
            // 
            ribbonControlsCreate.BackColor = Color.Transparent;
            ribbonControlsCreate.Controls.Add(panel6);
            ribbonControlsCreate.Controls.Add(buttonSetPassword);
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
            // buttonSetPassword
            // 
            buttonSetPassword.BackColor = Color.Transparent;
            buttonSetPassword.Dock = DockStyle.Left;
            buttonSetPassword.FlatAppearance.BorderSize = 0;
            buttonSetPassword.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonSetPassword.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonSetPassword.FlatStyle = FlatStyle.Flat;
            buttonSetPassword.Location = new Point(777, 0);
            buttonSetPassword.Name = "buttonSetPassword";
            buttonSetPassword.Size = new Size(85, 84);
            buttonSetPassword.TabIndex = 31;
            buttonSetPassword.Text = "Set Password";
            buttonSetPassword.TextAlign = ContentAlignment.BottomCenter;
            buttonSetPassword.UseVisualStyleBackColor = false;
            // 
            // panel5
            // 
            panel5.BackColor = Color.Transparent;
            panel5.Controls.Add(label8);
            panel5.Controls.Add(comboBoxEncryptionMethod);
            panel5.Controls.Add(checkboxEncrypt);
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
            // comboBoxEncryptionMethod
            // 
            comboBoxEncryptionMethod.BackColor = Color.FromArgb(32, 32, 32);
            comboBoxEncryptionMethod.ButtonColor = Color.FromArgb(48, 48, 48);
            comboBoxEncryptionMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxEncryptionMethod.FormattingEnabled = true;
            comboBoxEncryptionMethod.Items.AddRange(new object[] { "ZipCrypto", "AES-128", "AES-192", "AES-256" });
            comboBoxEncryptionMethod.Location = new Point(84, 32);
            comboBoxEncryptionMethod.Name = "comboBoxEncryptionMethod";
            comboBoxEncryptionMethod.Size = new Size(80, 23);
            comboBoxEncryptionMethod.TabIndex = 32;
            // 
            // checkboxEncrypt
            // 
            checkboxEncrypt.BackColor = Color.Transparent;
            checkboxEncrypt.BoxBackColor = Color.FromArgb(64, 64, 64);
            checkboxEncrypt.BoxBorderColor = Color.FromArgb(29, 181, 82);
            checkboxEncrypt.BoxGradientEnabled = true;
            checkboxEncrypt.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            checkboxEncrypt.BoxGradientStart = Color.FromArgb(29, 181, 82);
            checkboxEncrypt.BoxSize = 14;
            checkboxEncrypt.CheckBorderColor = Color.Lime;
            checkboxEncrypt.CheckColor = Color.LawnGreen;
            checkboxEncrypt.Checked = true;
            checkboxEncrypt.CheckGradientEnabled = true;
            checkboxEncrypt.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            checkboxEncrypt.CheckGradientStart = Color.Lime;
            checkboxEncrypt.CheckState = CheckState.Checked;
            checkboxEncrypt.Location = new Point(84, 4);
            checkboxEncrypt.Name = "checkboxEncrypt";
            checkboxEncrypt.Size = new Size(18, 24);
            checkboxEncrypt.TabIndex = 30;
            checkboxEncrypt.UseVisualStyleBackColor = false;
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
            brutalGradientPanel3.Dock = DockStyle.Bottom;
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
            brutalGradientPanel3.Size = new Size(1003, 16);
            brutalGradientPanel3.StartColor = Color.FromArgb(64, 64, 64);
            brutalGradientPanel3.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.BackColor = Color.Transparent;
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.898304F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.2243271F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26.8195419F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.057827F));
            tableLayoutPanel2.Controls.Add(label9, 2, 0);
            tableLayoutPanel2.Controls.Add(label4, 1, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(1003, 16);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Dock = DockStyle.Fill;
            label9.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.Location = new Point(596, 0);
            label9.Name = "label9";
            label9.Size = new Size(263, 16);
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
            label4.Size = new Size(247, 16);
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
            // trackBarThreads
            // 
            trackBarThreads.Dock = DockStyle.Right;
            trackBarThreads.LeftBarColor = Color.FromArgb(29, 181, 82);
            trackBarThreads.Location = new Point(163, 0);
            trackBarThreads.Maximum = 100;
            trackBarThreads.Name = "trackBarThreads";
            trackBarThreads.RightBarColor = Color.FromArgb(22, 132, 99);
            trackBarThreads.Size = new Size(143, 24);
            trackBarThreads.TabIndex = 0;
            trackBarThreads.ThumbInnerColor = Color.FromArgb(29, 181, 82);
            trackBarThreads.ThumbOutlineColor = Color.FromArgb(19, 90, 42);
            trackBarThreads.ThumbOutlineThickness = 2;
            trackBarThreads.Value = 50;
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
            // 
            // panel1
            // 
            panel1.Controls.Add(label2);
            panel1.Controls.Add(trackBarThreads);
            panel1.Controls.Add(labelThreadsMax);
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
            // labelThreadsMax
            // 
            labelThreadsMax.Dock = DockStyle.Right;
            labelThreadsMax.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelThreadsMax.Location = new Point(306, 0);
            labelThreadsMax.Name = "labelThreadsMax";
            labelThreadsMax.Size = new Size(40, 24);
            labelThreadsMax.TabIndex = 0;
            labelThreadsMax.Text = "100";
            labelThreadsMax.TextAlign = ContentAlignment.MiddleLeft;
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
            GradientPanelExtractRibbon.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GradientPanelExtractRibbon.BackColor = Color.FromArgb(32, 32, 32);
            GradientPanelExtractRibbon.Controls.Add(ribbonControlsExtract);
            GradientPanelExtractRibbon.Controls.Add(brutalGradientPanel4);
            GradientPanelExtractRibbon.EndColor = Color.FromArgb(64, 64, 64);
            GradientPanelExtractRibbon.ForeColor = Color.White;
            GradientPanelExtractRibbon.GlowCenterMaxOpacity = 200;
            GradientPanelExtractRibbon.GlowCenterMinOpacity = 50;
            GradientPanelExtractRibbon.GlowMinSurroundOpacity = 30;
            GradientPanelExtractRibbon.GradientEndSide = GradientSide.Bottom;
            GradientPanelExtractRibbon.GradientStartSide = GradientSide.Top;
            GradientPanelExtractRibbon.Location = new Point(-1, 28);
            GradientPanelExtractRibbon.Margin = new Padding(0);
            GradientPanelExtractRibbon.MouseEvents = true;
            GradientPanelExtractRibbon.Name = "GradientPanelExtractRibbon";
            GradientPanelExtractRibbon.Size = new Size(1003, 99);
            GradientPanelExtractRibbon.StartColor = Color.FromArgb(32, 32, 32);
            GradientPanelExtractRibbon.TabIndex = 7;
            GradientPanelExtractRibbon.Visible = false;
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
            // 
            // brutalGradientPanel4
            // 
            brutalGradientPanel4.BackColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel4.Controls.Add(tableLayoutPanel1);
            brutalGradientPanel4.Dock = DockStyle.Bottom;
            brutalGradientPanel4.EndColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel4.ForeColor = Color.White;
            brutalGradientPanel4.GlowCenterMaxOpacity = 200;
            brutalGradientPanel4.GlowCenterMinOpacity = 50;
            brutalGradientPanel4.GlowMinSurroundOpacity = 30;
            brutalGradientPanel4.GradientEndSide = GradientSide.Top;
            brutalGradientPanel4.GradientStartSide = GradientSide.Bottom;
            brutalGradientPanel4.Location = new Point(0, 83);
            brutalGradientPanel4.Margin = new Padding(0);
            brutalGradientPanel4.MouseEvents = true;
            brutalGradientPanel4.Name = "brutalGradientPanel4";
            brutalGradientPanel4.Size = new Size(1003, 16);
            brutalGradientPanel4.StartColor = Color.FromArgb(64, 64, 64);
            brutalGradientPanel4.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.Transparent;
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.4237289F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.5992F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26.8195419F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.057827F));
            tableLayoutPanel1.Controls.Add(label16, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1003, 16);
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
            // panelCreateTab
            // 
            panelCreateTab.Controls.Add(splitContainerCreate);
            panelCreateTab.Location = new Point(3, 133);
            panelCreateTab.Name = "panelCreateTab";
            panelCreateTab.Size = new Size(557, 366);
            panelCreateTab.TabIndex = 17;
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
            splitContainerCreate.Size = new Size(557, 366);
            splitContainerCreate.SplitterDistance = 185;
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
            treeViewExplorer.SelectedNodes = (System.Collections.ArrayList)resources.GetObject("treeViewExplorer.SelectedNodes");
            treeViewExplorer.Size = new Size(185, 366);
            treeViewExplorer.TabIndex = 0;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 1;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Controls.Add(lvStaging, 0, 0);
            tableLayoutPanel5.Controls.Add(panel7, 0, 1);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(0, 0);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 2;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 75.95628F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 24.0437164F));
            tableLayoutPanel5.Size = new Size(368, 366);
            tableLayoutPanel5.TabIndex = 0;
            // 
            // panel7
            // 
            panel7.Controls.Add(lblCreateHint);
            panel7.Controls.Add(lblCreateDest);
            panel7.Controls.Add(btnCreateBrowse);
            panel7.Controls.Add(txtCreateDest);
            panel7.Controls.Add(btnCreateQuick);
            panel7.Controls.Add(btnCreate);
            panel7.Dock = DockStyle.Fill;
            panel7.Location = new Point(3, 281);
            panel7.Name = "panel7";
            panel7.Size = new Size(362, 82);
            panel7.TabIndex = 17;
            // 
            // HomeView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(32, 32, 32);
            Controls.Add(panelCreateTab);
            Controls.Add(brutalGradientPanel2);
            Controls.Add(GradientPanelCreateRibbon);
            Controls.Add(grpCreate);
            Controls.Add(grpExtract);
            Controls.Add(GradientPanelExtractRibbon);
            ForeColor = Color.White;
            Name = "HomeView";
            Size = new Size(1000, 700);
            Load += HomeView_Load;
            grpCreate.ResumeLayout(false);
            grpCreate.PerformLayout();
            pnlCreateDrop.ResumeLayout(false);
            ((ISupportInitialize)numCreateLevel).EndInit();
            ((ISupportInitialize)tbThreads).EndInit();
            cmsStaging.ResumeLayout(false);
            grpExtract.ResumeLayout(false);
            grpExtract.PerformLayout();
            pnlExtractDrop.ResumeLayout(false);
            GradientPanelCreateRibbon.ResumeLayout(false);
            ribbonControlsCreate.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel3.ResumeLayout(false);
            brutalGradientPanel3.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((ISupportInitialize)trackBarThreads).EndInit();
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
            panelCreateTab.ResumeLayout(false);
            splitContainerCreate.Panel1.ResumeLayout(false);
            splitContainerCreate.Panel2.ResumeLayout(false);
            ((ISupportInitialize)splitContainerCreate).EndInit();
            splitContainerCreate.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
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
        private BrutalZip2025.BrutalControls.BrutalTrackBar trackBarThreads;
        private Button buttonWizardCreate;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel3;
        private Panel panel1;
        private Label label2;
        private Label labelThreadsMax;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkThreadsAutoMain;
        private Label label3;
        private Panel panel2;
        private Label label5;
        private Panel panel4;
        private Panel panel3;
        private Label label6;
      //  private BrutalZip2025.BrutalControls.BrutalComboBox cmbCreateMethod;
        private Panel panel5;
        private Label label8;
        private BrutalZip2025.BrutalControls.BrutalComboBox comboBoxEncryptionMethod;
        private BrutalZip2025.BrutalControls.BrutalCheckBox checkboxEncrypt;
        private Label label7;
        private Panel panel6;
        private Button buttonSetPassword;
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
        private Panel panelCreateTab;
        private SplitContainer splitContainerCreate;
        private TreeViewMS.TreeViewMS treeViewExplorer;
        private TableLayoutPanel tableLayoutPanel5;
        private Panel panel7;
    }
}
