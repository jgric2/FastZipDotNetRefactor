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
            mnuFileRecent = new ToolStripMenuItem();
            mnuFileExit = new ToolStripMenuItem();
            mnuTools = new ToolStripMenuItem();
            mnuToolsSettings = new ToolStripMenuItem();
            mnuHelp = new ToolStripMenuItem();
            mnuHelpAbout = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            menuMain.SuspendLayout();
            SuspendLayout();
            // 
            // homeView
            // 
            homeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            homeView.CreateDestination = "";
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
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuFileOpen, toolStripMenuItem1, mnuFileRecent, mnuFileExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(37, 20);
            mnuFile.Text = "&File";
            // 
            // mnuFileOpen
            // 
            mnuFileOpen.Name = "mnuFileOpen";
            mnuFileOpen.Size = new Size(180, 22);
            mnuFileOpen.Text = "&Open…";
            // 
            // mnuFileRecent
            // 
            mnuFileRecent.Name = "mnuFileRecent";
            mnuFileRecent.Size = new Size(180, 22);
            mnuFileRecent.Text = "&Recent";
            // 
            // mnuFileExit
            // 
            mnuFileExit.Name = "mnuFileExit";
            mnuFileExit.Size = new Size(180, 22);
            mnuFileExit.Text = "E&xit";
            // 
            // mnuTools
            // 
            mnuTools.DropDownItems.AddRange(new ToolStripItem[] { mnuToolsSettings });
            mnuTools.Name = "mnuTools";
            mnuTools.Size = new Size(47, 20);
            mnuTools.Text = "&Tools";


            mnuToolsOpenAfterCreate = new ToolStripMenuItem("Open Explorer after Create") { CheckOnClick = true };
            mnuToolsOpenAfterExtract = new ToolStripMenuItem("Open Explorer after Extract") { CheckOnClick = true };
            mnuToolsFind = new ToolStripMenuItem("Find in archive…"); // used in Feature 8

            mnuTools.DropDownItems.AddRange(new ToolStripItem[]
            {
                mnuToolsSettings,
                new ToolStripSeparator(),
                mnuToolsOpenAfterCreate,
                mnuToolsOpenAfterExtract,
                new ToolStripSeparator(),
                mnuToolsFind
            });
            mnuToolsFind.Click += (s, e) => DoFindInArchive();

            // 
            // mnuToolsSettings
            // 
            mnuToolsSettings.Name = "mnuToolsSettings";
            mnuToolsSettings.Size = new Size(180, 22);
            mnuToolsSettings.Text = "&Settings";
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
            mnuHelpAbout.Size = new Size(180, 22);
            mnuHelpAbout.Text = "&About";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(180, 22);
            toolStripMenuItem1.Text = "Export list...";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
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
            Load += MainForm_Load_1;
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private ToolStripMenuItem toolStripMenuItem1;
    }
}