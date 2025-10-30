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
        internal ComboBox cmbCreateMethod;
        private Label lblCreateLevel;
        internal NumericUpDown numCreateLevel;
        internal Button btnCreate;
        internal Button btnCreateQuick; // NEW
        internal Label lblCreateHint;

        // Threads slider (Create section)
        private Label lblThreadsText;
        internal TrackBar tbThreads;
        internal Label lblThreadsValue;
        internal CheckBox chkThreadsAutoMain;


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
            grpCreate = new GroupBox();
            pnlCreateDrop = new Panel();
            lblCreateDrop = new Label();
            btnCreateAddFiles = new Button();
            btnCreateAddFolder = new Button();
            lblCreateDest = new Label();
            txtCreateDest = new TextBox();
            btnCreateBrowse = new Button();
            lblCreateMethod = new Label();
            cmbCreateMethod = new ComboBox();
            lblCreateLevel = new Label();
            numCreateLevel = new NumericUpDown();
            lblThreadsText = new Label();
            lblThreadsValue = new Label();
            chkThreadsAutoMain = new CheckBox();
            chkEncrypt = new CheckBox();
            lblEncrypt = new Label();
            cmbEncrypt = new ComboBox();
            btnCreateSetPassword = new Button();
            lblPwdStatus = new Label();
            btnCreate = new Button();
            btnCreateQuick = new Button();
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
            lblCreateHint = new Label();
            tbThreads = new TrackBar();
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
            grpCreate.SuspendLayout();
            pnlCreateDrop.SuspendLayout();
            ((ISupportInitialize)numCreateLevel).BeginInit();
            cmsStaging.SuspendLayout();
            ((ISupportInitialize)tbThreads).BeginInit();
            grpExtract.SuspendLayout();
            pnlExtractDrop.SuspendLayout();
            SuspendLayout();
            // 
            // grpCreate
            // 
            grpCreate.Controls.Add(pnlCreateDrop);
            grpCreate.Controls.Add(btnCreateAddFiles);
            grpCreate.Controls.Add(btnCreateAddFolder);
            grpCreate.Controls.Add(lblCreateDest);
            grpCreate.Controls.Add(txtCreateDest);
            grpCreate.Controls.Add(btnCreateBrowse);
            grpCreate.Controls.Add(lblCreateMethod);
            grpCreate.Controls.Add(cmbCreateMethod);
            grpCreate.Controls.Add(lblCreateLevel);
            grpCreate.Controls.Add(numCreateLevel);
            grpCreate.Controls.Add(lblThreadsText);
            grpCreate.Controls.Add(lblThreadsValue);
            grpCreate.Controls.Add(chkThreadsAutoMain);
            grpCreate.Controls.Add(chkEncrypt);
            grpCreate.Controls.Add(lblEncrypt);
            grpCreate.Controls.Add(cmbEncrypt);
            grpCreate.Controls.Add(btnCreateSetPassword);
            grpCreate.Controls.Add(lblPwdStatus);
            grpCreate.Controls.Add(btnCreate);
            grpCreate.Controls.Add(btnCreateQuick);
            grpCreate.Controls.Add(lvStaging);
            grpCreate.Controls.Add(lblCreateHint);
            grpCreate.Controls.Add(tbThreads);
            grpCreate.Location = new Point(20, 20);
            grpCreate.Name = "grpCreate";
            grpCreate.Size = new Size(450, 663);
            grpCreate.TabIndex = 1;
            grpCreate.TabStop = false;
            grpCreate.Text = "Create";
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
            // lblCreateDest
            // 
            lblCreateDest.Location = new Point(20, 280);
            lblCreateDest.Name = "lblCreateDest";
            lblCreateDest.Size = new Size(80, 20);
            lblCreateDest.TabIndex = 3;
            lblCreateDest.Text = "Destination:";
            // 
            // txtCreateDest
            // 
            txtCreateDest.Location = new Point(20, 300);
            txtCreateDest.Name = "txtCreateDest";
            txtCreateDest.Size = new Size(330, 23);
            txtCreateDest.TabIndex = 4;
            // 
            // btnCreateBrowse
            // 
            btnCreateBrowse.Location = new Point(360, 298);
            btnCreateBrowse.Name = "btnCreateBrowse";
            btnCreateBrowse.Size = new Size(70, 26);
            btnCreateBrowse.TabIndex = 5;
            btnCreateBrowse.Text = "Browse…";
            // 
            // lblCreateMethod
            // 
            lblCreateMethod.Location = new Point(20, 340);
            lblCreateMethod.Name = "lblCreateMethod";
            lblCreateMethod.Size = new Size(60, 20);
            lblCreateMethod.TabIndex = 6;
            lblCreateMethod.Text = "Method:";
            // 
            // cmbCreateMethod
            // 
            cmbCreateMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCreateMethod.Items.AddRange(new object[] { "Store", "Deflate", "Zstd" });
            cmbCreateMethod.Location = new Point(80, 336);
            cmbCreateMethod.Name = "cmbCreateMethod";
            cmbCreateMethod.Size = new Size(110, 23);
            cmbCreateMethod.TabIndex = 7;
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
            numCreateLevel.Location = new Point(260, 336);
            numCreateLevel.Maximum = new decimal(new int[] { 22, 0, 0, 0 });
            numCreateLevel.Name = "numCreateLevel";
            numCreateLevel.Size = new Size(60, 23);
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
            // chkThreadsAutoMain
            // 
            chkThreadsAutoMain.Checked = true;
            chkThreadsAutoMain.CheckState = CheckState.Checked;
            chkThreadsAutoMain.Location = new Point(373, 364);
            chkThreadsAutoMain.Name = "chkThreadsAutoMain";
            chkThreadsAutoMain.Size = new Size(52, 20);
            chkThreadsAutoMain.TabIndex = 13;
            chkThreadsAutoMain.Text = "Auto";
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
            // lblPwdStatus
            // 
            lblPwdStatus.ForeColor = Color.DimGray;
            lblPwdStatus.Location = new Point(20, 450);
            lblPwdStatus.Name = "lblPwdStatus";
            lblPwdStatus.Size = new Size(410, 18);
            lblPwdStatus.TabIndex = 18;
            lblPwdStatus.Text = "Password: (not set)";
            // 
            // btnCreate
            // 
            btnCreate.Location = new Point(20, 470);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(100, 28);
            btnCreate.TabIndex = 14;
            btnCreate.Text = "Create";
            btnCreate.Click += btnCreate_Click;
            // 
            // btnCreateQuick
            // 
            btnCreateQuick.Location = new Point(130, 470);
            btnCreateQuick.Name = "btnCreateQuick";
            btnCreateQuick.Size = new Size(160, 28);
            btnCreateQuick.TabIndex = 15;
            btnCreateQuick.Text = "Compress to <auto>";
            // 
            // lvStaging
            // 
            lvStaging.Columns.AddRange(new ColumnHeader[] { colStName, colStType, colStSize, colStItems, colStPath });
            lvStaging.ContextMenuStrip = cmsStaging;
            lvStaging.FullRowSelect = true;
            lvStaging.Location = new Point(20, 504);
            lvStaging.Name = "lvStaging";
            lvStaging.Size = new Size(410, 120);
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
            // lblCreateHint
            // 
            lblCreateHint.AutoEllipsis = true;
            lblCreateHint.Location = new Point(20, 632);
            lblCreateHint.Name = "lblCreateHint";
            lblCreateHint.Size = new Size(410, 20);
            lblCreateHint.TabIndex = 17;
            lblCreateHint.Text = "Staging: none";
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
            grpExtract.Location = new Point(500, 20);
            grpExtract.Name = "grpExtract";
            grpExtract.Size = new Size(450, 560);
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
            // HomeView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(grpCreate);
            Controls.Add(grpExtract);
            Name = "HomeView";
            Size = new Size(1000, 700);
            grpCreate.ResumeLayout(false);
            grpCreate.PerformLayout();
            pnlCreateDrop.ResumeLayout(false);
            ((ISupportInitialize)numCreateLevel).EndInit();
            cmsStaging.ResumeLayout(false);
            ((ISupportInitialize)tbThreads).EndInit();
            grpExtract.ResumeLayout(false);
            grpExtract.PerformLayout();
            pnlExtractDrop.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
