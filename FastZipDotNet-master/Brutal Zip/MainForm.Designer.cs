namespace Brutal_Zip
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private Views.HomeView homeView;
        private Views.ViewerView viewerView;

        private MenuStrip menuMain;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuFileOpen;
        private ToolStripMenuItem mnuFileExit;
        private ToolStripMenuItem mnuTools;
        private ToolStripMenuItem mnuToolsSettings;
        private ToolStripMenuItem mnuHelp;
        private ToolStripMenuItem mnuHelpAbout;
        private ToolStripMenuItem mnuFileRecent;

        private ToolStripMenuItem mnuToolsOpenAfterCreate;     // NEW
        private ToolStripMenuItem mnuToolsOpenAfterExtract;    // NEW
        private ToolStripMenuItem mnuToolsFind;                // NEW (used in Feature 8)

        private ToolStripSeparator sepTools1;               // NEW
        private ToolStripSeparator sepTools2;               // NEW
        private ToolStripSeparator sepFile1;                 // NEW

        private ToolStripMenuItem mnuToolsSetComment;    // NEW
        private ToolStripMenuItem mnuToolsWizard;        // NEW
        private ToolStripSeparator sepTools4;            // NEW


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            homeView = new Brutal_Zip.Views.HomeView();
            viewerView = new Brutal_Zip.Views.ViewerView();
            menuMain = new MenuStrip();
            mnuFile = new ToolStripMenuItem();
            mnuFileOpen = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            sepFile1 = new ToolStripSeparator();
            mnuFileRecent = new ToolStripMenuItem();
            mnuFileExit = new ToolStripMenuItem();
            mnuTools = new ToolStripMenuItem();
            mnuToolsSettings = new ToolStripMenuItem();
            sepTools1 = new ToolStripSeparator();
            mnuToolsOpenAfterCreate = new ToolStripMenuItem();
            mnuToolsOpenAfterExtract = new ToolStripMenuItem();
            sepTools2 = new ToolStripSeparator();
            mnuToolsFind = new ToolStripMenuItem();
            sepTools3 = new ToolStripSeparator();
            mnuToolsCrackPassword = new ToolStripMenuItem();
            sepTools4 = new ToolStripSeparator();
            mnuToolsSetComment = new ToolStripMenuItem();
            mnuToolsWizard = new ToolStripMenuItem();
            mnuHelp = new ToolStripMenuItem();
            mnuHelpAbout = new ToolStripMenuItem();
            menuMain.SuspendLayout();
            SuspendLayout();
            // 
            // homeView
            // 
            homeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            homeView.BackColor = Color.FromArgb(32, 32, 32);
            homeView.CreateDestination = "";
            homeView.CreateEncryptAlgorithmIndex = 0;
            homeView.CreateEncryptEnabled = false;
            homeView.CreateLevel = 6;
            homeView.CreateMethodIndex = -1;
            homeView.ExtractDestination = "";
            homeView.ForeColor = Color.White;
            homeView.Location = new Point(0, 24);
            homeView.Name = "homeView";
            homeView.Size = new Size(1000, 673);
            homeView.TabIndex = 1;
            // 
            // viewerView
            // 
            viewerView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            viewerView.BackColor = Color.FromArgb(32, 32, 32);
            viewerView.ForeColor = Color.White;
            viewerView.Location = new Point(0, 23);
            viewerView.Name = "viewerView";
            viewerView.Size = new Size(1000, 677);
            viewerView.TabIndex = 0;
            // 
            // menuMain
            // 
            menuMain.BackColor = Color.FromArgb(32, 32, 32);
            menuMain.Items.AddRange(new ToolStripItem[] { mnuFile, mnuTools, mnuHelp });
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Size = new Size(1000, 24);
            menuMain.TabIndex = 0;
            // 
            // mnuFile
            // 
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuFileOpen, toolStripMenuItem1, sepFile1, mnuFileRecent, mnuFileExit });
            mnuFile.ForeColor = Color.White;
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(37, 20);
            mnuFile.Text = "&File";
            // 
            // mnuFileOpen
            // 
            mnuFileOpen.Name = "mnuFileOpen";
            mnuFileOpen.Size = new Size(134, 22);
            mnuFileOpen.Text = "&Open…";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(134, 22);
            toolStripMenuItem1.Text = "Export list...";
            // 
            // sepFile1
            // 
            sepFile1.Name = "sepFile1";
            sepFile1.Size = new Size(131, 6);
            // 
            // mnuFileRecent
            // 
            mnuFileRecent.Name = "mnuFileRecent";
            mnuFileRecent.Size = new Size(134, 22);
            mnuFileRecent.Text = "&Recent";
            // 
            // mnuFileExit
            // 
            mnuFileExit.Name = "mnuFileExit";
            mnuFileExit.Size = new Size(134, 22);
            mnuFileExit.Text = "E&xit";
            // 
            // mnuTools
            // 
            mnuTools.DropDownItems.AddRange(new ToolStripItem[] { mnuToolsSettings, sepTools1, mnuToolsOpenAfterCreate, mnuToolsOpenAfterExtract, sepTools2, mnuToolsFind, sepTools3, mnuToolsCrackPassword, sepTools4, mnuToolsSetComment, mnuToolsWizard });
            mnuTools.ForeColor = Color.White;
            mnuTools.Name = "mnuTools";
            mnuTools.Size = new Size(47, 20);
            mnuTools.Text = "&Tools";
            // 
            // mnuToolsSettings
            // 
            mnuToolsSettings.Name = "mnuToolsSettings";
            mnuToolsSettings.Size = new Size(213, 22);
            mnuToolsSettings.Text = "&Settings";
            // 
            // sepTools1
            // 
            sepTools1.Name = "sepTools1";
            sepTools1.Size = new Size(210, 6);
            // 
            // mnuToolsOpenAfterCreate
            // 
            mnuToolsOpenAfterCreate.CheckOnClick = true;
            mnuToolsOpenAfterCreate.Name = "mnuToolsOpenAfterCreate";
            mnuToolsOpenAfterCreate.Size = new Size(213, 22);
            mnuToolsOpenAfterCreate.Text = "Open Explorer after Create";
            // 
            // mnuToolsOpenAfterExtract
            // 
            mnuToolsOpenAfterExtract.CheckOnClick = true;
            mnuToolsOpenAfterExtract.Name = "mnuToolsOpenAfterExtract";
            mnuToolsOpenAfterExtract.Size = new Size(213, 22);
            mnuToolsOpenAfterExtract.Text = "Open Explorer after Extract";
            // 
            // sepTools2
            // 
            sepTools2.Name = "sepTools2";
            sepTools2.Size = new Size(210, 6);
            // 
            // mnuToolsFind
            // 
            mnuToolsFind.Name = "mnuToolsFind";
            mnuToolsFind.Size = new Size(213, 22);
            mnuToolsFind.Text = "Find in archive…";
            // 
            // sepTools3
            // 
            sepTools3.Name = "sepTools3";
            sepTools3.Size = new Size(210, 6);
            // 
            // mnuToolsCrackPassword
            // 
            mnuToolsCrackPassword.Enabled = false;
            mnuToolsCrackPassword.Name = "mnuToolsCrackPassword";
            mnuToolsCrackPassword.Size = new Size(213, 22);
            mnuToolsCrackPassword.Text = "Crack Password…";
            // 
            // sepTools4
            // 
            sepTools4.Name = "sepTools4";
            sepTools4.Size = new Size(210, 6);
            // 
            // mnuToolsSetComment
            // 
            mnuToolsSetComment.Name = "mnuToolsSetComment";
            mnuToolsSetComment.Size = new Size(213, 22);
            mnuToolsSetComment.Text = "Set Comment…";
            // 
            // mnuToolsWizard
            // 
            mnuToolsWizard.Name = "mnuToolsWizard";
            mnuToolsWizard.Size = new Size(213, 22);
            mnuToolsWizard.Text = "Wizard…";
            // 
            // mnuHelp
            // 
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuHelpAbout });
            mnuHelp.ForeColor = Color.White;
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(44, 20);
            mnuHelp.Text = "&Help";
            // 
            // mnuHelpAbout
            // 
            mnuHelpAbout.Name = "mnuHelpAbout";
            mnuHelpAbout.Size = new Size(107, 22);
            mnuHelpAbout.Text = "&About";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1000, 700);
            Controls.Add(menuMain);
            Controls.Add(homeView);
            Controls.Add(viewerView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MainMenuStrip = menuMain;
            Name = "MainForm";
            Text = "Brutal Zip";
            Load += MainForm_Load;
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripSeparator sepTools3;      // NEW
        internal ToolStripMenuItem mnuToolsCrackPassword; // NEW
    }
}