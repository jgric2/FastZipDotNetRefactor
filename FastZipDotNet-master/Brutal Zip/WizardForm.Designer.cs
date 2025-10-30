using System.ComponentModel;

namespace Brutal_Zip
{
    partial class WizardForm
    {
        private IContainer components = null;

        private TabControl tabs;
        private TabPage tabCreate;
        private TabPage tabExtract;

        // Create controls
        private ListBox lstCreatePaths;
        private Button btnCreateAddFiles;
        private Button btnCreateAddFolder;
        private Label lblCreateDest;
        private TextBox txtCreateDest;
        private Button btnCreateBrowse;
        private Label lblCreateMethod;
        private ComboBox cmbCreateMethod;
        private Label lblCreateLevel;
        private NumericUpDown numCreateLevel;
        private CheckBox chkCreateEncrypt;
        private Label lblCreateAlgo;
        private ComboBox cmbCreateAlgo;
        private Button btnCreateSetPassword;
        private Label lblCreateThreads;
        private TrackBar tbCreateThreads;
        private Label lblCreateThreadsVal;

        private Button btnCreateStart;

        // Extract controls
        private Label lblZip;
        private TextBox txtZip;
        private Button btnZipBrowse;
        private Label lblExtractDest;
        private TextBox txtExtractDest;
        private Button btnExtractBrowse;
        private CheckBox chkExtractToFolderName;
        private Label lblExtractThreads;
        private TrackBar tbExtractThreads;
        private Label lblExtractThreadsVal;
        private Button btnExtractStart;

        private Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tabs = new TabControl();
            tabCreate = new TabPage();
            lstCreatePaths = new ListBox();
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
            tabs.Size = new Size(696, 460);
            tabs.TabIndex = 0;
            // 
            // tabCreate
            // 
            tabCreate.Controls.Add(lstCreatePaths);
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
            tabCreate.Size = new Size(688, 432);
            tabCreate.TabIndex = 0;
            tabCreate.Text = "Create";
            // 
            // lstCreatePaths
            // 
            lstCreatePaths.ItemHeight = 15;
            lstCreatePaths.Location = new Point(16, 16);
            lstCreatePaths.Name = "lstCreatePaths";
            lstCreatePaths.Size = new Size(520, 169);
            lstCreatePaths.TabIndex = 0;
            // 
            // btnCreateAddFiles
            // 
            btnCreateAddFiles.Location = new Point(548, 16);
            btnCreateAddFiles.Name = "btnCreateAddFiles";
            btnCreateAddFiles.Size = new Size(120, 28);
            btnCreateAddFiles.TabIndex = 1;
            btnCreateAddFiles.Text = "Add files…";
            // 
            // btnCreateAddFolder
            // 
            btnCreateAddFolder.Location = new Point(548, 50);
            btnCreateAddFolder.Name = "btnCreateAddFolder";
            btnCreateAddFolder.Size = new Size(120, 28);
            btnCreateAddFolder.TabIndex = 2;
            btnCreateAddFolder.Text = "Add folder…";
            // 
            // lblCreateDest
            // 
            lblCreateDest.Location = new Point(16, 210);
            lblCreateDest.Name = "lblCreateDest";
            lblCreateDest.Size = new Size(73, 23);
            lblCreateDest.TabIndex = 3;
            lblCreateDest.Text = "Destination:";
            // 
            // txtCreateDest
            // 
            txtCreateDest.Location = new Point(96, 206);
            txtCreateDest.Name = "txtCreateDest";
            txtCreateDest.Size = new Size(440, 23);
            txtCreateDest.TabIndex = 4;
            // 
            // btnCreateBrowse
            // 
            btnCreateBrowse.Location = new Point(548, 204);
            btnCreateBrowse.Name = "btnCreateBrowse";
            btnCreateBrowse.Size = new Size(120, 26);
            btnCreateBrowse.TabIndex = 5;
            btnCreateBrowse.Text = "Browse…";
            // 
            // lblCreateMethod
            // 
            lblCreateMethod.Location = new Point(16, 244);
            lblCreateMethod.Name = "lblCreateMethod";
            lblCreateMethod.Size = new Size(73, 23);
            lblCreateMethod.TabIndex = 6;
            lblCreateMethod.Text = "Method:";
            // 
            // cmbCreateMethod
            // 
            cmbCreateMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCreateMethod.Items.AddRange(new object[] { "Store", "Deflate", "Zstd" });
            cmbCreateMethod.Location = new Point(96, 242);
            cmbCreateMethod.Name = "cmbCreateMethod";
            cmbCreateMethod.Size = new Size(120, 23);
            cmbCreateMethod.TabIndex = 7;
            // 
            // lblCreateLevel
            // 
            lblCreateLevel.Location = new Point(232, 244);
            lblCreateLevel.Name = "lblCreateLevel";
            lblCreateLevel.Size = new Size(42, 23);
            lblCreateLevel.TabIndex = 8;
            lblCreateLevel.Text = "Level:";
            // 
            // numCreateLevel
            // 
            numCreateLevel.Location = new Point(280, 242);
            numCreateLevel.Maximum = new decimal(new int[] { 22, 0, 0, 0 });
            numCreateLevel.Name = "numCreateLevel";
            numCreateLevel.Size = new Size(60, 23);
            numCreateLevel.TabIndex = 9;
            numCreateLevel.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // chkCreateEncrypt
            // 
            chkCreateEncrypt.Location = new Point(16, 276);
            chkCreateEncrypt.Name = "chkCreateEncrypt";
            chkCreateEncrypt.Size = new Size(77, 24);
            chkCreateEncrypt.TabIndex = 10;
            chkCreateEncrypt.Text = "Encrypt";
            // 
            // lblCreateAlgo
            // 
            lblCreateAlgo.Location = new Point(96, 276);
            lblCreateAlgo.Name = "lblCreateAlgo";
            lblCreateAlgo.Size = new Size(38, 23);
            lblCreateAlgo.TabIndex = 11;
            lblCreateAlgo.Text = "Algo:";
            // 
            // cmbCreateAlgo
            // 
            cmbCreateAlgo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCreateAlgo.Items.AddRange(new object[] { "ZipCrypto", "AES-128", "AES-192", "AES-256" });
            cmbCreateAlgo.Location = new Point(140, 274);
            cmbCreateAlgo.Name = "cmbCreateAlgo";
            cmbCreateAlgo.Size = new Size(120, 23);
            cmbCreateAlgo.TabIndex = 12;
            // 
            // btnCreateSetPassword
            // 
            btnCreateSetPassword.Location = new Point(268, 272);
            btnCreateSetPassword.Name = "btnCreateSetPassword";
            btnCreateSetPassword.Size = new Size(120, 26);
            btnCreateSetPassword.TabIndex = 13;
            btnCreateSetPassword.Text = "Set password…";
            // 
            // lblCreateThreads
            // 
            lblCreateThreads.Location = new Point(16, 310);
            lblCreateThreads.Name = "lblCreateThreads";
            lblCreateThreads.Size = new Size(58, 23);
            lblCreateThreads.TabIndex = 14;
            lblCreateThreads.Text = "Threads:";
            // 
            // tbCreateThreads
            // 
            tbCreateThreads.Location = new Point(80, 306);
            tbCreateThreads.Maximum = 16;
            tbCreateThreads.Minimum = 1;
            tbCreateThreads.Name = "tbCreateThreads";
            tbCreateThreads.Size = new Size(240, 45);
            tbCreateThreads.TabIndex = 15;
            tbCreateThreads.Value = 16;
            // 
            // lblCreateThreadsVal
            // 
            lblCreateThreadsVal.Location = new Point(330, 310);
            lblCreateThreadsVal.Name = "lblCreateThreadsVal";
            lblCreateThreadsVal.Size = new Size(100, 23);
            lblCreateThreadsVal.TabIndex = 16;
            lblCreateThreadsVal.Text = "16";
            // 
            // btnCreateStart
            // 
            btnCreateStart.Location = new Point(548, 390);
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
            tabExtract.Size = new Size(688, 432);
            tabExtract.TabIndex = 1;
            tabExtract.Text = "Extract";
            // 
            // lblZip
            // 
            lblZip.Location = new Point(16, 20);
            lblZip.Name = "lblZip";
            lblZip.Size = new Size(74, 23);
            lblZip.TabIndex = 0;
            lblZip.Text = "Archive:";
            // 
            // txtZip
            // 
            txtZip.Location = new Point(96, 18);
            txtZip.Name = "txtZip";
            txtZip.Size = new Size(440, 23);
            txtZip.TabIndex = 1;
            // 
            // btnZipBrowse
            // 
            btnZipBrowse.Location = new Point(548, 16);
            btnZipBrowse.Name = "btnZipBrowse";
            btnZipBrowse.Size = new Size(120, 26);
            btnZipBrowse.TabIndex = 2;
            btnZipBrowse.Text = "Browse…";
            // 
            // lblExtractDest
            // 
            lblExtractDest.Location = new Point(16, 54);
            lblExtractDest.Name = "lblExtractDest";
            lblExtractDest.Size = new Size(74, 23);
            lblExtractDest.TabIndex = 3;
            lblExtractDest.Text = "Destination:";
            // 
            // txtExtractDest
            // 
            txtExtractDest.Location = new Point(96, 52);
            txtExtractDest.Name = "txtExtractDest";
            txtExtractDest.Size = new Size(440, 23);
            txtExtractDest.TabIndex = 4;
            // 
            // btnExtractBrowse
            // 
            btnExtractBrowse.Location = new Point(548, 50);
            btnExtractBrowse.Name = "btnExtractBrowse";
            btnExtractBrowse.Size = new Size(120, 26);
            btnExtractBrowse.TabIndex = 5;
            btnExtractBrowse.Text = "Browse…";
            // 
            // chkExtractToFolderName
            // 
            chkExtractToFolderName.Location = new Point(16, 84);
            chkExtractToFolderName.Name = "chkExtractToFolderName";
            chkExtractToFolderName.Size = new Size(104, 24);
            chkExtractToFolderName.TabIndex = 6;
            chkExtractToFolderName.Text = "Extract to “ArchiveName/”";
            // 
            // lblExtractThreads
            // 
            lblExtractThreads.Location = new Point(16, 120);
            lblExtractThreads.Name = "lblExtractThreads";
            lblExtractThreads.Size = new Size(58, 23);
            lblExtractThreads.TabIndex = 7;
            lblExtractThreads.Text = "Threads:";
            // 
            // tbExtractThreads
            // 
            tbExtractThreads.Location = new Point(80, 116);
            tbExtractThreads.Maximum = 16;
            tbExtractThreads.Minimum = 1;
            tbExtractThreads.Name = "tbExtractThreads";
            tbExtractThreads.Size = new Size(240, 45);
            tbExtractThreads.TabIndex = 8;
            tbExtractThreads.Value = 16;
            // 
            // lblExtractThreadsVal
            // 
            lblExtractThreadsVal.Location = new Point(330, 120);
            lblExtractThreadsVal.Name = "lblExtractThreadsVal";
            lblExtractThreadsVal.Size = new Size(100, 23);
            lblExtractThreadsVal.TabIndex = 9;
            lblExtractThreadsVal.Text = "16";
            // 
            // btnExtractStart
            // 
            btnExtractStart.Location = new Point(548, 390);
            btnExtractStart.Name = "btnExtractStart";
            btnExtractStart.Size = new Size(120, 28);
            btnExtractStart.TabIndex = 10;
            btnExtractStart.Text = "Start";
            // 
            // btnClose
            // 
            btnClose.Location = new Point(608, 486);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 28);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";
            // 
            // WizardForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(720, 540);
            Controls.Add(tabs);
            Controls.Add(btnClose);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "WizardForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Wizard";
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