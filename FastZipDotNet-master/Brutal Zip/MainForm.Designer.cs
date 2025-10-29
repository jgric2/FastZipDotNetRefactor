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
            this.homeView = new Brutal_Zip.Views.HomeView();
            this.viewerView = new Brutal_Zip.Views.ViewerView();
            this.menuMain = new MenuStrip();

            this.mnuFile = new ToolStripMenuItem();
            this.mnuFileOpen = new ToolStripMenuItem();
            this.toolStripMenuItem1 = new ToolStripMenuItem();  // Export list...
            this.sepFile1 = new ToolStripSeparator();
            this.mnuFileRecent = new ToolStripMenuItem();
            this.mnuFileExit = new ToolStripMenuItem();

            this.mnuTools = new ToolStripMenuItem();
            this.mnuToolsSettings = new ToolStripMenuItem();
            this.sepTools1 = new ToolStripSeparator();
            this.mnuToolsOpenAfterCreate = new ToolStripMenuItem();
            this.mnuToolsOpenAfterExtract = new ToolStripMenuItem();
            this.sepTools2 = new ToolStripSeparator();
            this.mnuToolsFind = new ToolStripMenuItem();

            this.mnuHelp = new ToolStripMenuItem();
            this.mnuHelpAbout = new ToolStripMenuItem();

            this.menuMain.SuspendLayout();
            this.SuspendLayout();

            // homeView
            this.homeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.homeView.Location = new Point(0, 27);
            this.homeView.Name = "homeView";
            this.homeView.Size = new Size(1000, 673);
            this.homeView.TabIndex = 1;

            // viewerView
            this.viewerView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.viewerView.Location = new Point(0, 27);
            this.viewerView.Name = "viewerView";
            this.viewerView.Size = new Size(1000, 673);
            this.viewerView.TabIndex = 0;

            // menuMain
            this.menuMain.Items.AddRange(new ToolStripItem[] { this.mnuFile, this.mnuTools, this.mnuHelp });
            this.menuMain.Location = new Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new Size(1000, 24);
            this.menuMain.TabIndex = 0;

            // mnuFile
            this.mnuFile.DropDownItems.AddRange(new ToolStripItem[] {
    this.mnuFileOpen,
    this.toolStripMenuItem1, // Export list...
    this.sepFile1,
    this.mnuFileRecent,
    this.mnuFileExit
});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new Size(37, 20);
            this.mnuFile.Text = "&File";

            // mnuFileOpen
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.Size = new Size(180, 22);
            this.mnuFileOpen.Text = "&Open…";

            // toolStripMenuItem1 (Export list...)
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new Size(180, 22);
            this.toolStripMenuItem1.Text = "Export list...";

            // sepFile1
            this.sepFile1.Name = "sepFile1";
            this.sepFile1.Size = new Size(177, 6);

            // mnuFileRecent
            this.mnuFileRecent.Name = "mnuFileRecent";
            this.mnuFileRecent.Size = new Size(180, 22);
            this.mnuFileRecent.Text = "&Recent";

            // mnuFileExit
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new Size(180, 22);
            this.mnuFileExit.Text = "E&xit";

            // mnuTools
            this.mnuTools.DropDownItems.AddRange(new ToolStripItem[] {
    this.mnuToolsSettings,
    this.sepTools1,
    this.mnuToolsOpenAfterCreate,
    this.mnuToolsOpenAfterExtract,
    this.sepTools2,
    this.mnuToolsFind
});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new Size(47, 20);
            this.mnuTools.Text = "&Tools";

            // mnuToolsSettings
            this.mnuToolsSettings.Name = "mnuToolsSettings";
            this.mnuToolsSettings.Size = new Size(180, 22);
            this.mnuToolsSettings.Text = "&Settings";

            // sepTools1
            this.sepTools1.Name = "sepTools1";
            this.sepTools1.Size = new Size(177, 6);

            // mnuToolsOpenAfterCreate
            this.mnuToolsOpenAfterCreate.CheckOnClick = true;
            this.mnuToolsOpenAfterCreate.Name = "mnuToolsOpenAfterCreate";
            this.mnuToolsOpenAfterCreate.Size = new Size(180, 22);
            this.mnuToolsOpenAfterCreate.Text = "Open Explorer after Create";

            // mnuToolsOpenAfterExtract
            this.mnuToolsOpenAfterExtract.CheckOnClick = true;
            this.mnuToolsOpenAfterExtract.Name = "mnuToolsOpenAfterExtract";
            this.mnuToolsOpenAfterExtract.Size = new Size(180, 22);
            this.mnuToolsOpenAfterExtract.Text = "Open Explorer after Extract";

            // sepTools2
            this.sepTools2.Name = "sepTools2";
            this.sepTools2.Size = new Size(177, 6);

            // mnuToolsFind
            this.mnuToolsFind.Name = "mnuToolsFind";
            this.mnuToolsFind.Size = new Size(180, 22);
            this.mnuToolsFind.Text = "Find in archive…";

            // mnuHelp
            this.mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { this.mnuHelpAbout });
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new Size(44, 20);
            this.mnuHelp.Text = "&Help";

            // mnuHelpAbout
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new Size(180, 22);
            this.mnuHelpAbout.Text = "&About";

            // MainForm
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(1000, 700);
            this.Controls.Add(this.menuMain);
            this.Controls.Add(this.homeView);
            this.Controls.Add(this.viewerView);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuMain;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Brutal Zip";

            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private ToolStripMenuItem toolStripMenuItem1;
    }
}