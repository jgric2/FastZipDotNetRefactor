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

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
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
            mnuHelp = new ToolStripMenuItem();
            mnuHelpAbout = new ToolStripMenuItem();
            menuMain.SuspendLayout();
            SuspendLayout();
            // 
            // homeView
            // 
            homeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            homeView.CreateDestination = "";
            homeView.CreateEncryptAlgorithmIndex = 0;
            homeView.CreateEncryptEnabled = false;
            homeView.CreateLevel = 6;
            homeView.CreateMethodIndex = -1;
            homeView.ExtractDestination = "";
            homeView.Location = new Point(0, 27);
            homeView.Name = "homeView";
            homeView.Size = new Size(1000, 673);
            homeView.TabIndex = 1;
            // 
            // viewerView
            // 
            viewerView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            viewerView.Location = new Point(0, 27);
            viewerView.Name = "viewerView";
            viewerView.Size = new Size(1000, 673);
            viewerView.TabIndex = 0;
            // 
            // menuMain
            // 
            menuMain.Items.AddRange(new ToolStripItem[] { mnuFile, mnuTools, mnuHelp });
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Size = new Size(1000, 24);
            menuMain.TabIndex = 0;
            // 
            // mnuFile
            // 
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuFileOpen, toolStripMenuItem1, sepFile1, mnuFileRecent, mnuFileExit });
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
            mnuTools.DropDownItems.AddRange(new ToolStripItem[] { mnuToolsSettings, sepTools1, mnuToolsOpenAfterCreate, mnuToolsOpenAfterExtract, sepTools2, mnuToolsFind });
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
            // mnuHelp
            // 
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuHelpAbout });
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
            KeyPreview = true;
            MainMenuStrip = menuMain;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Brutal Zip";
            Load += MainForm_Load;
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private ToolStripMenuItem toolStripMenuItem1;
    }
}