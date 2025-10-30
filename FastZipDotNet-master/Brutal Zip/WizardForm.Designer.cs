using System.ComponentModel;

namespace Brutal_Zip
{
    partial class WizardForm
    {
        private IContainer components = null;

        private TabControl tabs;
        private TabPage tabCreate;
        private TabPage tabExtract;

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
        internal ComboBox cmbCreateMethod;
        private Label lblCreateLevel;
        internal NumericUpDown numCreateLevel;

        internal CheckBox chkCreateEncrypt;
        private Label lblCreateAlgo;
        internal ComboBox cmbCreateAlgo;
        private Button btnCreateSetPassword;

        private Label lblCreateThreads;
        internal TrackBar tbCreateThreads;
        internal Label lblCreateThreadsVal;

        internal Button btnCreateStart;

        // Extract tab controls
        private Label lblZip;
        internal TextBox txtZip;
        private Button btnZipBrowse;

        private Label lblExtractDest;
        internal TextBox txtExtractDest;
        private Button btnExtractBrowse;

        internal CheckBox chkExtractToFolderName;

        private Label lblExtractThreads;
        internal TrackBar tbExtractThreads;
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
            tabs = new TabControl();
            tabCreate = new TabPage();
            lvCreate = new ListView();
            colName = new ColumnHeader();
            colType = new ColumnHeader();
            colSize = new ColumnHeader();
            colItems = new ColumnHeader();
            colPath = new ColumnHeader();
            btnCreateAddFiles = new Button();
            btnCreateAddFolder = new Button();
            lblCreateDest = new Label();
            txtCreateDest = new TextBox();
            btnCreateBrowse = new Button();
            lblCreateMethod = new Label();
            cmbCreateMethod = new ComboBox();
            lblCreateLevel = new Label();
            numCreateLevel = new NumericUpDown();
            chkCreateEncrypt = new CheckBox();
            lblCreateAlgo = new Label();
            cmbCreateAlgo = new ComboBox();
            btnCreateSetPassword = new Button();
            lblCreateThreads = new Label();
            tbCreateThreads = new TrackBar();
            lblCreateThreadsVal = new Label();
            btnCreateStart = new Button();
            tabExtract = new TabPage();
            lblZip = new Label();
            txtZip = new TextBox();
            btnZipBrowse = new Button();
            lblExtractDest = new Label();
            txtExtractDest = new TextBox();
            btnExtractBrowse = new Button();
            chkExtractToFolderName = new CheckBox();
            lblExtractThreads = new Label();
            tbExtractThreads = new TrackBar();
            lblExtractThreadsVal = new Label();
            btnExtractStart = new Button();
            btnClose = new Button();
            tabs.SuspendLayout();
            tabCreate.SuspendLayout();
            ((ISupportInitialize)numCreateLevel).BeginInit();
            ((ISupportInitialize)tbCreateThreads).BeginInit();
            tabExtract.SuspendLayout();
            ((ISupportInitialize)tbExtractThreads).BeginInit();
            SuspendLayout();
            // 
            // tabs
            // 
            tabs.Controls.Add(tabCreate);
            tabs.Controls.Add(tabExtract);
            tabs.Location = new Point(12, 12);
            tabs.Name = "tabs";
            tabs.SelectedIndex = 0;
            tabs.Size = new Size(876, 520);
            tabs.TabIndex = 0;
            // 
            // tabCreate
            // 
            tabCreate.Controls.Add(lvCreate);
            tabCreate.Controls.Add(btnCreateAddFiles);
            tabCreate.Controls.Add(btnCreateAddFolder);
            tabCreate.Controls.Add(lblCreateDest);
            tabCreate.Controls.Add(txtCreateDest);
            tabCreate.Controls.Add(btnCreateBrowse);
            tabCreate.Controls.Add(lblCreateMethod);
            tabCreate.Controls.Add(cmbCreateMethod);
            tabCreate.Controls.Add(lblCreateLevel);
            tabCreate.Controls.Add(numCreateLevel);
            tabCreate.Controls.Add(chkCreateEncrypt);
            tabCreate.Controls.Add(lblCreateAlgo);
            tabCreate.Controls.Add(cmbCreateAlgo);
            tabCreate.Controls.Add(btnCreateSetPassword);
            tabCreate.Controls.Add(lblCreateThreads);
            tabCreate.Controls.Add(tbCreateThreads);
            tabCreate.Controls.Add(lblCreateThreadsVal);
            tabCreate.Controls.Add(btnCreateStart);
            tabCreate.Location = new Point(4, 24);
            tabCreate.Name = "tabCreate";
            tabCreate.Padding = new Padding(8);
            tabCreate.Size = new Size(868, 492);
            tabCreate.TabIndex = 0;
            tabCreate.Text = "Create";
            // 
            // lvCreate
            // 
            lvCreate.Columns.AddRange(new ColumnHeader[] { colName, colType, colSize, colItems, colPath });
            lvCreate.FullRowSelect = true;
            lvCreate.Location = new Point(16, 16);
            lvCreate.Name = "lvCreate";
            lvCreate.Size = new Size(700, 220);
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
            // btnCreateAddFiles
            // 
            btnCreateAddFiles.Location = new Point(728, 16);
            btnCreateAddFiles.Name = "btnCreateAddFiles";
            btnCreateAddFiles.Size = new Size(120, 28);
            btnCreateAddFiles.TabIndex = 1;
            btnCreateAddFiles.Text = "Add files…";
            // 
            // btnCreateAddFolder
            // 
            btnCreateAddFolder.Location = new Point(728, 50);
            btnCreateAddFolder.Name = "btnCreateAddFolder";
            btnCreateAddFolder.Size = new Size(120, 28);
            btnCreateAddFolder.TabIndex = 2;
            btnCreateAddFolder.Text = "Add folder…";
            // 
            // lblCreateDest
            // 
            lblCreateDest.AutoSize = true;
            lblCreateDest.Location = new Point(16, 252);
            lblCreateDest.Name = "lblCreateDest";
            lblCreateDest.Size = new Size(70, 15);
            lblCreateDest.TabIndex = 3;
            lblCreateDest.Text = "Destination:";
            // 
            // txtCreateDest
            // 
            txtCreateDest.Location = new Point(96, 248);
            txtCreateDest.Name = "txtCreateDest";
            txtCreateDest.Size = new Size(580, 23);
            txtCreateDest.TabIndex = 4;
            // 
            // btnCreateBrowse
            // 
            btnCreateBrowse.Location = new Point(684, 246);
            btnCreateBrowse.Name = "btnCreateBrowse";
            btnCreateBrowse.Size = new Size(100, 26);
            btnCreateBrowse.TabIndex = 5;
            btnCreateBrowse.Text = "Browse…";
            // 
            // lblCreateMethod
            // 
            lblCreateMethod.AutoSize = true;
            lblCreateMethod.Location = new Point(16, 286);
            lblCreateMethod.Name = "lblCreateMethod";
            lblCreateMethod.Size = new Size(52, 15);
            lblCreateMethod.TabIndex = 6;
            lblCreateMethod.Text = "Method:";
            // 
            // cmbCreateMethod
            // 
            cmbCreateMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCreateMethod.Items.AddRange(new object[] { "Store", "Deflate", "Zstd" });
            cmbCreateMethod.Location = new Point(96, 284);
            cmbCreateMethod.Name = "cmbCreateMethod";
            cmbCreateMethod.Size = new Size(120, 23);
            cmbCreateMethod.TabIndex = 7;
            // 
            // lblCreateLevel
            // 
            lblCreateLevel.AutoSize = true;
            lblCreateLevel.Location = new Point(232, 286);
            lblCreateLevel.Name = "lblCreateLevel";
            lblCreateLevel.Size = new Size(37, 15);
            lblCreateLevel.TabIndex = 8;
            lblCreateLevel.Text = "Level:";
            // 
            // numCreateLevel
            // 
            numCreateLevel.Location = new Point(280, 284);
            numCreateLevel.Maximum = new decimal(new int[] { 22, 0, 0, 0 });
            numCreateLevel.Name = "numCreateLevel";
            numCreateLevel.Size = new Size(60, 23);
            numCreateLevel.TabIndex = 9;
            numCreateLevel.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // chkCreateEncrypt
            // 
            chkCreateEncrypt.AutoSize = true;
            chkCreateEncrypt.Location = new Point(16, 318);
            chkCreateEncrypt.Name = "chkCreateEncrypt";
            chkCreateEncrypt.Size = new Size(66, 19);
            chkCreateEncrypt.TabIndex = 10;
            chkCreateEncrypt.Text = "Encrypt";
            // 
            // lblCreateAlgo
            // 
            lblCreateAlgo.AutoSize = true;
            lblCreateAlgo.Location = new Point(96, 318);
            lblCreateAlgo.Name = "lblCreateAlgo";
            lblCreateAlgo.Size = new Size(35, 15);
            lblCreateAlgo.TabIndex = 11;
            lblCreateAlgo.Text = "Algo:";
            // 
            // cmbCreateAlgo
            // 
            cmbCreateAlgo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCreateAlgo.Items.AddRange(new object[] { "ZipCrypto", "AES-128", "AES-192", "AES-256" });
            cmbCreateAlgo.Location = new Point(140, 316);
            cmbCreateAlgo.Name = "cmbCreateAlgo";
            cmbCreateAlgo.Size = new Size(120, 23);
            cmbCreateAlgo.TabIndex = 12;
            // 
            // btnCreateSetPassword
            // 
            btnCreateSetPassword.Location = new Point(268, 314);
            btnCreateSetPassword.Name = "btnCreateSetPassword";
            btnCreateSetPassword.Size = new Size(120, 26);
            btnCreateSetPassword.TabIndex = 13;
            btnCreateSetPassword.Text = "Set password…";
            // 
            // lblCreateThreads
            // 
            lblCreateThreads.AutoSize = true;
            lblCreateThreads.Location = new Point(16, 352);
            lblCreateThreads.Name = "lblCreateThreads";
            lblCreateThreads.Size = new Size(52, 15);
            lblCreateThreads.TabIndex = 14;
            lblCreateThreads.Text = "Threads:";
            // 
            // tbCreateThreads
            // 
            tbCreateThreads.Location = new Point(80, 348);
            tbCreateThreads.Maximum = 16;
            tbCreateThreads.Minimum = 1;
            tbCreateThreads.Name = "tbCreateThreads";
            tbCreateThreads.Size = new Size(240, 45);
            tbCreateThreads.TabIndex = 15;
            tbCreateThreads.Value = 16;
            // 
            // lblCreateThreadsVal
            // 
            lblCreateThreadsVal.Location = new Point(330, 352);
            lblCreateThreadsVal.Name = "lblCreateThreadsVal";
            lblCreateThreadsVal.Size = new Size(100, 23);
            lblCreateThreadsVal.TabIndex = 16;
            lblCreateThreadsVal.Text = "16";
            // 
            // btnCreateStart
            // 
            btnCreateStart.Location = new Point(728, 424);
            btnCreateStart.Name = "btnCreateStart";
            btnCreateStart.Size = new Size(120, 28);
            btnCreateStart.TabIndex = 17;
            btnCreateStart.Text = "Start";
            // 
            // tabExtract
            // 
            tabExtract.Controls.Add(lblZip);
            tabExtract.Controls.Add(txtZip);
            tabExtract.Controls.Add(btnZipBrowse);
            tabExtract.Controls.Add(lblExtractDest);
            tabExtract.Controls.Add(txtExtractDest);
            tabExtract.Controls.Add(btnExtractBrowse);
            tabExtract.Controls.Add(chkExtractToFolderName);
            tabExtract.Controls.Add(lblExtractThreads);
            tabExtract.Controls.Add(tbExtractThreads);
            tabExtract.Controls.Add(lblExtractThreadsVal);
            tabExtract.Controls.Add(btnExtractStart);
            tabExtract.Location = new Point(4, 24);
            tabExtract.Name = "tabExtract";
            tabExtract.Padding = new Padding(8);
            tabExtract.Size = new Size(868, 492);
            tabExtract.TabIndex = 1;
            tabExtract.Text = "Extract";
            // 
            // lblZip
            // 
            lblZip.AutoSize = true;
            lblZip.Location = new Point(16, 24);
            lblZip.Name = "lblZip";
            lblZip.Size = new Size(50, 15);
            lblZip.TabIndex = 0;
            lblZip.Text = "Archive:";
            // 
            // txtZip
            // 
            txtZip.Location = new Point(96, 20);
            txtZip.Name = "txtZip";
            txtZip.Size = new Size(574, 23);
            txtZip.TabIndex = 1;
            // 
            // btnZipBrowse
            // 
            btnZipBrowse.Location = new Point(684, 18);
            btnZipBrowse.Name = "btnZipBrowse";
            btnZipBrowse.Size = new Size(100, 26);
            btnZipBrowse.TabIndex = 2;
            btnZipBrowse.Text = "Browse…";
            // 
            // lblExtractDest
            // 
            lblExtractDest.AutoSize = true;
            lblExtractDest.Location = new Point(16, 56);
            lblExtractDest.Name = "lblExtractDest";
            lblExtractDest.Size = new Size(70, 15);
            lblExtractDest.TabIndex = 3;
            lblExtractDest.Text = "Destination:";
            // 
            // txtExtractDest
            // 
            txtExtractDest.Location = new Point(96, 52);
            txtExtractDest.Name = "txtExtractDest";
            txtExtractDest.Size = new Size(574, 23);
            txtExtractDest.TabIndex = 4;
            // 
            // btnExtractBrowse
            // 
            btnExtractBrowse.Location = new Point(684, 50);
            btnExtractBrowse.Name = "btnExtractBrowse";
            btnExtractBrowse.Size = new Size(100, 26);
            btnExtractBrowse.TabIndex = 5;
            btnExtractBrowse.Text = "Browse…";
            // 
            // chkExtractToFolderName
            // 
            chkExtractToFolderName.AutoSize = true;
            chkExtractToFolderName.Location = new Point(16, 88);
            chkExtractToFolderName.Name = "chkExtractToFolderName";
            chkExtractToFolderName.Size = new Size(165, 19);
            chkExtractToFolderName.TabIndex = 6;
            chkExtractToFolderName.Text = "Extract to “ArchiveName/”";
            // 
            // lblExtractThreads
            // 
            lblExtractThreads.AutoSize = true;
            lblExtractThreads.Location = new Point(16, 126);
            lblExtractThreads.Name = "lblExtractThreads";
            lblExtractThreads.Size = new Size(52, 15);
            lblExtractThreads.TabIndex = 7;
            lblExtractThreads.Text = "Threads:";
            // 
            // tbExtractThreads
            // 
            tbExtractThreads.Location = new Point(80, 122);
            tbExtractThreads.Maximum = 16;
            tbExtractThreads.Minimum = 1;
            tbExtractThreads.Name = "tbExtractThreads";
            tbExtractThreads.Size = new Size(240, 45);
            tbExtractThreads.TabIndex = 8;
            tbExtractThreads.Value = 16;
            // 
            // lblExtractThreadsVal
            // 
            lblExtractThreadsVal.Location = new Point(330, 126);
            lblExtractThreadsVal.Name = "lblExtractThreadsVal";
            lblExtractThreadsVal.Size = new Size(100, 23);
            lblExtractThreadsVal.TabIndex = 9;
            lblExtractThreadsVal.Text = "16";
            // 
            // btnExtractStart
            // 
            btnExtractStart.Location = new Point(684, 424);
            btnExtractStart.Name = "btnExtractStart";
            btnExtractStart.Size = new Size(100, 28);
            btnExtractStart.TabIndex = 10;
            btnExtractStart.Text = "Start";
            // 
            // btnClose
            // 
            btnClose.Location = new Point(788, 542);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 28);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";



            // Context menu for lvCreate (Create tab)
            cmsCreate = new ContextMenuStrip();
            mnuCreateAddFiles = new ToolStripMenuItem();
            mnuCreateAddFolder = new ToolStripMenuItem();
            sepCreate1 = new ToolStripSeparator();
            mnuCreateRemoveSelected = new ToolStripMenuItem();
            mnuCreateRemoveMissing = new ToolStripMenuItem();
            mnuCreateClearAll = new ToolStripMenuItem();

            // Items
            mnuCreateAddFiles.Text = "Add files…";
            mnuCreateAddFolder.Text = "Add folder…";
            mnuCreateRemoveSelected.Text = "Remove selected";
            mnuCreateRemoveMissing.Text = "Remove missing";
            mnuCreateClearAll.Text = "Clear all";

            cmsCreate.Items.AddRange(new ToolStripItem[]
            {
                mnuCreateAddFiles,
                mnuCreateAddFolder,
                sepCreate1,
                mnuCreateRemoveSelected,
                mnuCreateRemoveMissing,
                mnuCreateClearAll
            });

            // Attach to listview
            lvCreate.ContextMenuStrip = cmsCreate;

            // 
            // WizardForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(900, 580);
            Controls.Add(tabs);
            Controls.Add(btnClose);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "WizardForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Wizard";
            Load += WizardForm_Load;
            tabs.ResumeLayout(false);
            tabCreate.ResumeLayout(false);
            tabCreate.PerformLayout();
            ((ISupportInitialize)numCreateLevel).EndInit();
            ((ISupportInitialize)tbCreateThreads).EndInit();
            tabExtract.ResumeLayout(false);
            tabExtract.PerformLayout();
            ((ISupportInitialize)tbExtractThreads).EndInit();
            ResumeLayout(false);
        }
    }
}
