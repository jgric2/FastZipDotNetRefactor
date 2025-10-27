namespace TestWinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabControlMain;
        private TabPage tabExtract;
        private TabPage tabCreate;

        // Extract tab controls
        private Button btnOpenZip;
        private Button btnUp;
        private Label lblCurrentPath;
        private ListView lvExtract;
        private Button btnExtractSelected;
        private Button btnExtractAll;
        private Button btnAddFilesExtract;
        private Button btnAddFolderExtract;
        private Button btnDeleteSelected;
        private Button btnCompact;
        private Button btnTest;
        private Button btnRepair;
        private Button btnInfo;
        private Label lblExtractInfo;

        // Create tab controls
        private Label lblCreateInfo;
        private ListView lvCreate;
        private Button btnCreateAddFiles;
        private Button btnCreateAddFolder;
        private Label lblMethod;
        private ComboBox cmbMethod;
        private Label lblLevel;
        private NumericUpDown numLevel;
        private Button btnBuildArchive;

        // Context menus
        private ContextMenuStrip cmsExtract;
        private ToolStripMenuItem mnuExtract_Open;
        private ToolStripMenuItem mnuExtract_ExtractSelected;
        private ToolStripMenuItem mnuExtract_AddFiles;
        private ToolStripMenuItem mnuExtract_AddFolder;
        private ToolStripMenuItem mnuExtract_DeleteSelected;
        private ToolStripMenuItem mnuExtract_Info;

        private ContextMenuStrip cmsCreate;
        private ToolStripMenuItem mnuCreate_RemoveSelected;
        private ToolStripMenuItem mnuCreate_Clear;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tabControlMain = new TabControl();
            tabExtract = new TabPage();
            btnOpenZip = new Button();
            btnUp = new Button();
            lblCurrentPath = new Label();
            lvExtract = new ListView();
            cmsExtract = new ContextMenuStrip(components);
            mnuExtract_Open = new ToolStripMenuItem();
            mnuExtract_ExtractSelected = new ToolStripMenuItem();
            mnuExtract_AddFiles = new ToolStripMenuItem();
            mnuExtract_AddFolder = new ToolStripMenuItem();
            mnuExtract_DeleteSelected = new ToolStripMenuItem();
            mnuExtract_Info = new ToolStripMenuItem();
            btnExtractSelected = new Button();
            btnExtractAll = new Button();
            btnAddFilesExtract = new Button();
            btnAddFolderExtract = new Button();
            btnDeleteSelected = new Button();
            btnCompact = new Button();
            btnTest = new Button();
            btnRepair = new Button();
            btnInfo = new Button();
            lblExtractInfo = new Label();
            tabCreate = new TabPage();
            lblCreateInfo = new Label();
            lvCreate = new ListView();
            cmsCreate = new ContextMenuStrip(components);
            mnuCreate_RemoveSelected = new ToolStripMenuItem();
            mnuCreate_Clear = new ToolStripMenuItem();
            btnCreateAddFiles = new Button();
            btnCreateAddFolder = new Button();
            lblMethod = new Label();
            cmbMethod = new ComboBox();
            lblLevel = new Label();
            numLevel = new NumericUpDown();
            btnBuildArchive = new Button();
            tabControlMain.SuspendLayout();
            tabExtract.SuspendLayout();
            cmsExtract.SuspendLayout();
            tabCreate.SuspendLayout();
            cmsCreate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numLevel).BeginInit();
            SuspendLayout();
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabExtract);
            tabControlMain.Controls.Add(tabCreate);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1100, 700);
            tabControlMain.TabIndex = 0;
            // 
            // tabExtract
            // 
            tabExtract.Controls.Add(btnOpenZip);
            tabExtract.Controls.Add(btnUp);
            tabExtract.Controls.Add(lblCurrentPath);
            tabExtract.Controls.Add(lvExtract);
            tabExtract.Controls.Add(btnExtractSelected);
            tabExtract.Controls.Add(btnExtractAll);
            tabExtract.Controls.Add(btnAddFilesExtract);
            tabExtract.Controls.Add(btnAddFolderExtract);
            tabExtract.Controls.Add(btnDeleteSelected);
            tabExtract.Controls.Add(btnCompact);
            tabExtract.Controls.Add(btnTest);
            tabExtract.Controls.Add(btnRepair);
            tabExtract.Controls.Add(btnInfo);
            tabExtract.Controls.Add(lblExtractInfo);
            tabExtract.Location = new Point(4, 24);
            tabExtract.Name = "tabExtract";
            tabExtract.Padding = new Padding(8);
            tabExtract.Size = new Size(1092, 672);
            tabExtract.TabIndex = 0;
            tabExtract.Text = "Extract";
            tabExtract.UseVisualStyleBackColor = true;
            // 
            // btnOpenZip
            // 
            btnOpenZip.Location = new Point(8, 8);
            btnOpenZip.Name = "btnOpenZip";
            btnOpenZip.Size = new Size(100, 28);
            btnOpenZip.TabIndex = 0;
            btnOpenZip.Text = "Open Zip...";
            btnOpenZip.UseVisualStyleBackColor = true;
            btnOpenZip.Click += btnOpenZip_Click;
            // 
            // btnUp
            // 
            btnUp.Location = new Point(114, 8);
            btnUp.Name = "btnUp";
            btnUp.Size = new Size(60, 28);
            btnUp.TabIndex = 1;
            btnUp.Text = "Up";
            btnUp.UseVisualStyleBackColor = true;
            btnUp.Click += btnUp_Click;
            // 
            // lblCurrentPath
            // 
            lblCurrentPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblCurrentPath.Location = new Point(180, 12);
            lblCurrentPath.Name = "lblCurrentPath";
            lblCurrentPath.Size = new Size(904, 20);
            lblCurrentPath.TabIndex = 2;
            lblCurrentPath.Text = "/";
            // 
            // lvExtract
            // 
            lvExtract.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvExtract.ContextMenuStrip = cmsExtract;
            lvExtract.Location = new Point(8, 44);
            lvExtract.Name = "lvExtract";
            lvExtract.Size = new Size(1076, 560);
            lvExtract.TabIndex = 2;
            lvExtract.UseCompatibleStateImageBehavior = false;
            lvExtract.DoubleClick += lvExtract_DoubleClick;
            // 
            // cmsExtract
            // 
            cmsExtract.Items.AddRange(new ToolStripItem[] { mnuExtract_Open, mnuExtract_ExtractSelected, mnuExtract_AddFiles, mnuExtract_AddFolder, mnuExtract_DeleteSelected, mnuExtract_Info });
            cmsExtract.Name = "cmsExtract";
            cmsExtract.Size = new Size(166, 154);
            // 
            // mnuExtract_Open
            // 
            mnuExtract_Open.Name = "mnuExtract_Open";
            mnuExtract_Open.Size = new Size(165, 22);
            mnuExtract_Open.Text = "Open";
            mnuExtract_Open.Click += mnuExtract_Open_Click;
            // 
            // mnuExtract_ExtractSelected
            // 
            mnuExtract_ExtractSelected.Name = "mnuExtract_ExtractSelected";
            mnuExtract_ExtractSelected.Size = new Size(165, 22);
            mnuExtract_ExtractSelected.Text = "Extract Selected...";
            mnuExtract_ExtractSelected.Click += mnuExtract_ExtractSelected_Click;
            // 
            // mnuExtract_AddFiles
            // 
            mnuExtract_AddFiles.Name = "mnuExtract_AddFiles";
            mnuExtract_AddFiles.Size = new Size(165, 22);
            mnuExtract_AddFiles.Text = "Add Files...";
            mnuExtract_AddFiles.Click += btnAddFilesExtract_Click;
            // 
            // mnuExtract_AddFolder
            // 
            mnuExtract_AddFolder.Name = "mnuExtract_AddFolder";
            mnuExtract_AddFolder.Size = new Size(165, 22);
            mnuExtract_AddFolder.Text = "Add Folder...";
            mnuExtract_AddFolder.Click += btnAddFolderExtract_Click;
            // 
            // mnuExtract_DeleteSelected
            // 
            mnuExtract_DeleteSelected.Name = "mnuExtract_DeleteSelected";
            mnuExtract_DeleteSelected.Size = new Size(165, 22);
            mnuExtract_DeleteSelected.Text = "Delete Selected";
            mnuExtract_DeleteSelected.Click += btnDeleteSelected_Click;
            // 
            // mnuExtract_Info
            // 
            mnuExtract_Info.Name = "mnuExtract_Info";
            mnuExtract_Info.Size = new Size(165, 22);
            mnuExtract_Info.Text = "Info";
            mnuExtract_Info.Click += btnInfo_Click;
            // 
            // btnExtractSelected
            // 
            btnExtractSelected.Location = new Point(477, 610);
            btnExtractSelected.Name = "btnExtractSelected";
            btnExtractSelected.Size = new Size(130, 28);
            btnExtractSelected.TabIndex = 3;
            btnExtractSelected.Text = "Extract Selected...";
            btnExtractSelected.UseVisualStyleBackColor = true;
            btnExtractSelected.Click += btnExtractSelected_Click;
            // 
            // btnExtractAll
            // 
            btnExtractAll.Location = new Point(363, 610);
            btnExtractAll.Name = "btnExtractAll";
            btnExtractAll.Size = new Size(110, 28);
            btnExtractAll.TabIndex = 4;
            btnExtractAll.Text = "Extract All...";
            btnExtractAll.UseVisualStyleBackColor = true;
            btnExtractAll.Click += btnExtractAll_Click;
            // 
            // btnAddFilesExtract
            // 
            btnAddFilesExtract.Location = new Point(8, 610);
            btnAddFilesExtract.Name = "btnAddFilesExtract";
            btnAddFilesExtract.Size = new Size(100, 28);
            btnAddFilesExtract.TabIndex = 5;
            btnAddFilesExtract.Text = "Add Files...";
            btnAddFilesExtract.UseVisualStyleBackColor = true;
            btnAddFilesExtract.Click += btnAddFilesExtract_Click;
            // 
            // btnAddFolderExtract
            // 
            btnAddFolderExtract.Location = new Point(114, 611);
            btnAddFolderExtract.Name = "btnAddFolderExtract";
            btnAddFolderExtract.Size = new Size(110, 28);
            btnAddFolderExtract.TabIndex = 6;
            btnAddFolderExtract.Text = "Add Folder...";
            btnAddFolderExtract.UseVisualStyleBackColor = true;
            btnAddFolderExtract.Click += btnAddFolderExtract_Click;
            // 
            // btnDeleteSelected
            // 
            btnDeleteSelected.Location = new Point(226, 611);
            btnDeleteSelected.Name = "btnDeleteSelected";
            btnDeleteSelected.Size = new Size(120, 28);
            btnDeleteSelected.TabIndex = 7;
            btnDeleteSelected.Text = "Delete Selected";
            btnDeleteSelected.UseVisualStyleBackColor = true;
            btnDeleteSelected.Click += btnDeleteSelected_Click;
            // 
            // btnCompact
            // 
            btnCompact.Location = new Point(729, 610);
            btnCompact.Name = "btnCompact";
            btnCompact.Size = new Size(90, 28);
            btnCompact.TabIndex = 8;
            btnCompact.Text = "Compact";
            btnCompact.UseVisualStyleBackColor = true;
            btnCompact.Click += btnCompact_Click;
            // 
            // btnTest
            // 
            btnTest.Location = new Point(633, 610);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(90, 28);
            btnTest.TabIndex = 9;
            btnTest.Text = "Test";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            // 
            // btnRepair
            // 
            btnRepair.Location = new Point(948, 613);
            btnRepair.Name = "btnRepair";
            btnRepair.Size = new Size(90, 28);
            btnRepair.TabIndex = 10;
            btnRepair.Text = "Repair";
            btnRepair.UseVisualStyleBackColor = true;
            btnRepair.Click += btnRepair_Click;
            // 
            // btnInfo
            // 
            btnInfo.Location = new Point(852, 611);
            btnInfo.Name = "btnInfo";
            btnInfo.Size = new Size(90, 28);
            btnInfo.TabIndex = 11;
            btnInfo.Text = "Info";
            btnInfo.UseVisualStyleBackColor = true;
            btnInfo.Click += btnInfo_Click;
            // 
            // lblExtractInfo
            // 
            lblExtractInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblExtractInfo.Location = new Point(8, 644);
            lblExtractInfo.Name = "lblExtractInfo";
            lblExtractInfo.Size = new Size(1076, 20);
            lblExtractInfo.TabIndex = 12;
            lblExtractInfo.Text = "No archive loaded";
            // 
            // tabCreate
            // 
            tabCreate.Controls.Add(lblCreateInfo);
            tabCreate.Controls.Add(lvCreate);
            tabCreate.Controls.Add(btnCreateAddFiles);
            tabCreate.Controls.Add(btnCreateAddFolder);
            tabCreate.Controls.Add(lblMethod);
            tabCreate.Controls.Add(cmbMethod);
            tabCreate.Controls.Add(lblLevel);
            tabCreate.Controls.Add(numLevel);
            tabCreate.Controls.Add(btnBuildArchive);
            tabCreate.Location = new Point(4, 24);
            tabCreate.Name = "tabCreate";
            tabCreate.Padding = new Padding(8);
            tabCreate.Size = new Size(1092, 672);
            tabCreate.TabIndex = 1;
            tabCreate.Text = "Create";
            tabCreate.UseVisualStyleBackColor = true;
            // 
            // lblCreateInfo
            // 
            lblCreateInfo.Location = new Point(8, 10);
            lblCreateInfo.Name = "lblCreateInfo";
            lblCreateInfo.Size = new Size(400, 20);
            lblCreateInfo.TabIndex = 0;
            lblCreateInfo.Text = "Staging area: drag & drop files/folders below";
            // 
            // lvCreate
            // 
            lvCreate.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvCreate.ContextMenuStrip = cmsCreate;
            lvCreate.Location = new Point(8, 36);
            lvCreate.Name = "lvCreate";
            lvCreate.Size = new Size(1076, 560);
            lvCreate.TabIndex = 1;
            lvCreate.UseCompatibleStateImageBehavior = false;
            // 
            // cmsCreate
            // 
            cmsCreate.Items.AddRange(new ToolStripItem[] { mnuCreate_RemoveSelected, mnuCreate_Clear });
            cmsCreate.Name = "cmsCreate";
            cmsCreate.Size = new Size(165, 48);
            // 
            // mnuCreate_RemoveSelected
            // 
            mnuCreate_RemoveSelected.Name = "mnuCreate_RemoveSelected";
            mnuCreate_RemoveSelected.Size = new Size(164, 22);
            mnuCreate_RemoveSelected.Text = "Remove Selected";
            mnuCreate_RemoveSelected.Click += mnuCreate_RemoveSelected_Click;
            // 
            // mnuCreate_Clear
            // 
            mnuCreate_Clear.Name = "mnuCreate_Clear";
            mnuCreate_Clear.Size = new Size(164, 22);
            mnuCreate_Clear.Text = "Clear";
            mnuCreate_Clear.Click += mnuCreate_Clear_Click;
            // 
            // btnCreateAddFiles
            // 
            btnCreateAddFiles.Location = new Point(8, 604);
            btnCreateAddFiles.Name = "btnCreateAddFiles";
            btnCreateAddFiles.Size = new Size(100, 28);
            btnCreateAddFiles.TabIndex = 2;
            btnCreateAddFiles.Text = "Add Files...";
            btnCreateAddFiles.UseVisualStyleBackColor = true;
            btnCreateAddFiles.Click += btnCreateAddFiles_Click;
            // 
            // btnCreateAddFolder
            // 
            btnCreateAddFolder.Location = new Point(114, 604);
            btnCreateAddFolder.Name = "btnCreateAddFolder";
            btnCreateAddFolder.Size = new Size(110, 28);
            btnCreateAddFolder.TabIndex = 3;
            btnCreateAddFolder.Text = "Add Folder...";
            btnCreateAddFolder.UseVisualStyleBackColor = true;
            btnCreateAddFolder.Click += btnCreateAddFolder_Click;
            // 
            // lblMethod
            // 
            lblMethod.Location = new Point(8, 604);
            lblMethod.Name = "lblMethod";
            lblMethod.Size = new Size(60, 20);
            lblMethod.TabIndex = 4;
            lblMethod.Text = "Method:";
            // 
            // cmbMethod
            // 
            cmbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMethod.Items.AddRange(new object[] { "Store", "Deflate", "Zstd" });
            cmbMethod.Location = new Point(8, 604);
            cmbMethod.Name = "cmbMethod";
            cmbMethod.Size = new Size(100, 23);
            cmbMethod.TabIndex = 5;
            // 
            // lblLevel
            // 
            lblLevel.Location = new Point(8, 604);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(45, 20);
            lblLevel.TabIndex = 6;
            lblLevel.Text = "Level:";
            // 
            // numLevel
            // 
            numLevel.Location = new Point(8, 604);
            numLevel.Maximum = new decimal(new int[] { 22, 0, 0, 0 });
            numLevel.Name = "numLevel";
            numLevel.Size = new Size(60, 23);
            numLevel.TabIndex = 7;
            numLevel.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // btnBuildArchive
            // 
            btnBuildArchive.Location = new Point(230, 604);
            btnBuildArchive.Name = "btnBuildArchive";
            btnBuildArchive.Size = new Size(120, 28);
            btnBuildArchive.TabIndex = 8;
            btnBuildArchive.Text = "Build Archive...";
            btnBuildArchive.UseVisualStyleBackColor = true;
            btnBuildArchive.Click += btnBuildArchive_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 700);
            Controls.Add(tabControlMain);
            Name = "MainForm";
            Text = "ZipSpeed - Fast ZIP UI";
            tabControlMain.ResumeLayout(false);
            tabExtract.ResumeLayout(false);
            cmsExtract.ResumeLayout(false);
            tabCreate.ResumeLayout(false);
            cmsCreate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numLevel).EndInit();
            ResumeLayout(false);
        }
    }
}