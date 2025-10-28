using System.ComponentModel;

namespace Brutal_Zip.Views
{
    partial class HomeView
    {
        private IContainer components = null;

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
        internal Label lblCreateHint;

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

        // Staging list
        internal ListView lvStaging;
        private ColumnHeader colStName;
        private ColumnHeader colStType;
        private ColumnHeader colStSize;
        private ColumnHeader colStItems;
        private ColumnHeader colStPath;
        internal ContextMenuStrip cmsStaging;
        internal ToolStripMenuItem mnuStagingRemove;
        internal ToolStripMenuItem mnuStagingClear;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.grpCreate = new GroupBox();
            this.pnlCreateDrop = new Panel();
            this.lblCreateDrop = new Label();
            this.btnCreateAddFiles = new Button();
            this.btnCreateAddFolder = new Button();
            this.lblCreateDest = new Label();
            this.txtCreateDest = new TextBox();
            this.btnCreateBrowse = new Button();
            this.lblCreateMethod = new Label();
            this.cmbCreateMethod = new ComboBox();
            this.lblCreateLevel = new Label();
            this.numCreateLevel = new NumericUpDown();
            this.btnCreate = new Button();
            this.lvStaging = new ListView();
            this.colStName = new ColumnHeader();
            this.colStType = new ColumnHeader();
            this.colStSize = new ColumnHeader();
            this.colStItems = new ColumnHeader();
            this.colStPath = new ColumnHeader();
            this.cmsStaging = new ContextMenuStrip(this.components);
            this.mnuStagingRemove = new ToolStripMenuItem();
            this.mnuStagingClear = new ToolStripMenuItem();
            this.lblCreateHint = new Label();

            this.grpExtract = new GroupBox();
            this.pnlExtractDrop = new Panel();
            this.lblExtractDrop = new Label();
            this.btnOpenArchive = new Button();
            this.rdoExtractToFolderName = new RadioButton();
            this.rdoExtractHere = new RadioButton();
            this.lblExtractDest = new Label();
            this.txtExtractDest = new TextBox();
            this.btnExtractBrowse = new Button();
            this.btnExtract = new Button();

            ((ISupportInitialize)(this.numCreateLevel)).BeginInit();
            this.grpCreate.SuspendLayout();
            this.pnlCreateDrop.SuspendLayout();
            this.cmsStaging.SuspendLayout();
            this.grpExtract.SuspendLayout();
            this.pnlExtractDrop.SuspendLayout();
            this.SuspendLayout();

            // HomeView
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Name = "HomeView";
            this.Size = new Size(1000, 700);

            // grpCreate
            this.grpCreate.Text = "Create";
            this.grpCreate.Location = new Point(20, 20);
            this.grpCreate.Size = new Size(450, 560);

            // pnlCreateDrop
            this.pnlCreateDrop.BorderStyle = BorderStyle.FixedSingle;
            this.pnlCreateDrop.BackColor = Color.WhiteSmoke;
            this.pnlCreateDrop.Location = new Point(20, 30);
            this.pnlCreateDrop.Size = new Size(410, 200);

            // lblCreateDrop
            this.lblCreateDrop.Dock = DockStyle.Fill;
            this.lblCreateDrop.TextAlign = ContentAlignment.MiddleCenter;
            this.lblCreateDrop.Text = "Drop files/folders here to create a ZIP";
            this.pnlCreateDrop.Controls.Add(this.lblCreateDrop);

            // btnCreateAddFiles
            this.btnCreateAddFiles.Text = "Add files…";
            this.btnCreateAddFiles.Location = new Point(20, 240);
            this.btnCreateAddFiles.Size = new Size(110, 28);

            // btnCreateAddFolder
            this.btnCreateAddFolder.Text = "Add folder…";
            this.btnCreateAddFolder.Location = new Point(140, 240);
            this.btnCreateAddFolder.Size = new Size(110, 28);

            // lblCreateDest
            this.lblCreateDest.Text = "Destination:";
            this.lblCreateDest.Location = new Point(20, 280);
            this.lblCreateDest.Size = new Size(80, 20);

            // txtCreateDest
            this.txtCreateDest.Location = new Point(20, 300);
            this.txtCreateDest.Size = new Size(330, 23);

            // btnCreateBrowse
            this.btnCreateBrowse.Text = "Browse…";
            this.btnCreateBrowse.Location = new Point(360, 298);
            this.btnCreateBrowse.Size = new Size(70, 26);

            // lblCreateMethod
            this.lblCreateMethod.Text = "Method:";
            this.lblCreateMethod.Location = new Point(20, 340);
            this.lblCreateMethod.Size = new Size(60, 20);

            // cmbCreateMethod
            this.cmbCreateMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCreateMethod.Items.AddRange(new object[] { "Store", "Deflate", "Zstd" });
            this.cmbCreateMethod.Location = new Point(80, 336);
            this.cmbCreateMethod.Size = new Size(110, 23);

            // lblCreateLevel
            this.lblCreateLevel.Text = "Level:";
            this.lblCreateLevel.Location = new Point(210, 340);
            this.lblCreateLevel.Size = new Size(40, 20);

            // numCreateLevel
            this.numCreateLevel.Location = new Point(260, 336);
            this.numCreateLevel.Size = new Size(60, 23);
            this.numCreateLevel.Minimum = 0;
            this.numCreateLevel.Maximum = 22;
            this.numCreateLevel.Value = 6;

            // btnCreate
            this.btnCreate.Text = "Create";
            this.btnCreate.Location = new Point(20, 368);
            this.btnCreate.Size = new Size(100, 28);

            // cmsStaging
            this.cmsStaging.Items.AddRange(new ToolStripItem[] {
            this.mnuStagingRemove,
            this.mnuStagingClear
        });

            // mnuStagingRemove
            this.mnuStagingRemove.Text = "Remove selected";

            // mnuStagingClear
            this.mnuStagingClear.Text = "Clear all";

            // lvStaging
            this.lvStaging.Location = new Point(20, 402);
            this.lvStaging.Size = new Size(410, 120);
            this.lvStaging.View = View.Details;
            this.lvStaging.FullRowSelect = true;
            this.lvStaging.HideSelection = false;
            this.lvStaging.ContextMenuStrip = this.cmsStaging;

            this.colStName.Text = "Name";
            this.colStName.Width = 150;
            this.colStType.Text = "Type";
            this.colStType.Width = 70;
            this.colStSize.Text = "Size";
            this.colStSize.Width = 80;
            this.colStSize.TextAlign = HorizontalAlignment.Right;
            this.colStItems.Text = "Items";
            this.colStItems.Width = 60;
            this.colStItems.TextAlign = HorizontalAlignment.Right;
            this.colStPath.Text = "Path";
            this.colStPath.Width = 300;

            this.lvStaging.Columns.AddRange(new ColumnHeader[] {
            this.colStName, this.colStType, this.colStSize, this.colStItems, this.colStPath
        });

            // lblCreateHint
            this.lblCreateHint.Text = "Staging: none";
            this.lblCreateHint.Location = new Point(20, 528);
            this.lblCreateHint.Size = new Size(410, 20);
            this.lblCreateHint.AutoEllipsis = true;

            // Add controls to grpCreate
            this.grpCreate.Controls.Add(this.pnlCreateDrop);
            this.grpCreate.Controls.Add(this.btnCreateAddFiles);
            this.grpCreate.Controls.Add(this.btnCreateAddFolder);
            this.grpCreate.Controls.Add(this.lblCreateDest);
            this.grpCreate.Controls.Add(this.txtCreateDest);
            this.grpCreate.Controls.Add(this.btnCreateBrowse);
            this.grpCreate.Controls.Add(this.lblCreateMethod);
            this.grpCreate.Controls.Add(this.cmbCreateMethod);
            this.grpCreate.Controls.Add(this.lblCreateLevel);
            this.grpCreate.Controls.Add(this.numCreateLevel);
            this.grpCreate.Controls.Add(this.btnCreate);
            this.grpCreate.Controls.Add(this.lvStaging);
            this.grpCreate.Controls.Add(this.lblCreateHint);

            // grpExtract
            this.grpExtract.Text = "Extract";
            this.grpExtract.Location = new Point(500, 20);
            this.grpExtract.Size = new Size(450, 560);

            // pnlExtractDrop
            this.pnlExtractDrop.BorderStyle = BorderStyle.FixedSingle;
            this.pnlExtractDrop.BackColor = Color.WhiteSmoke;
            this.pnlExtractDrop.Location = new Point(20, 30);
            this.pnlExtractDrop.Size = new Size(410, 200);

            // lblExtractDrop
            this.lblExtractDrop.Dock = DockStyle.Fill;
            this.lblExtractDrop.TextAlign = ContentAlignment.MiddleCenter;
            this.lblExtractDrop.Text = "Drop .zip files here to extract or open";
            this.pnlExtractDrop.Controls.Add(this.lblExtractDrop);

            // btnOpenArchive
            this.btnOpenArchive.Text = "Open archive…";
            this.btnOpenArchive.Location = new Point(20, 240);
            this.btnOpenArchive.Size = new Size(110, 28);

            // rdoExtractToFolderName
            this.rdoExtractToFolderName.Text = "Extract to “ArchiveName/”";
            this.rdoExtractToFolderName.Location = new Point(20, 280);
            this.rdoExtractToFolderName.Size = new Size(250, 20);
            this.rdoExtractToFolderName.Checked = true;

            // rdoExtractHere
            this.rdoExtractHere.Text = "Extract here";
            this.rdoExtractHere.Location = new Point(20, 305);
            this.rdoExtractHere.Size = new Size(250, 20);

            // lblExtractDest
            this.lblExtractDest.Text = "Destination:";
            this.lblExtractDest.Location = new Point(20, 340);
            this.lblExtractDest.Size = new Size(80, 20);

            // txtExtractDest
            this.txtExtractDest.Location = new Point(20, 360);
            this.txtExtractDest.Size = new Size(330, 23);

            // btnExtractBrowse
            this.btnExtractBrowse.Text = "Browse…";
            this.btnExtractBrowse.Location = new Point(360, 358);
            this.btnExtractBrowse.Size = new Size(70, 26);

            // btnExtract
            this.btnExtract.Text = "Extract";
            this.btnExtract.Location = new Point(20, 400);
            this.btnExtract.Size = new Size(100, 28);

            // Add controls to grpExtract
            this.grpExtract.Controls.Add(this.pnlExtractDrop);
            this.grpExtract.Controls.Add(this.btnOpenArchive);
            this.grpExtract.Controls.Add(this.rdoExtractToFolderName);
            this.grpExtract.Controls.Add(this.rdoExtractHere);
            this.grpExtract.Controls.Add(this.lblExtractDest);
            this.grpExtract.Controls.Add(this.txtExtractDest);
            this.grpExtract.Controls.Add(this.btnExtractBrowse);
            this.grpExtract.Controls.Add(this.btnExtract);

            // Add groups to HomeView
            this.Controls.Add(this.grpCreate);
            this.Controls.Add(this.grpExtract);

            this.pnlCreateDrop.ResumeLayout(false);
            this.grpCreate.ResumeLayout(false);
            this.grpCreate.PerformLayout();
            this.cmsStaging.ResumeLayout(false);
            this.pnlExtractDrop.ResumeLayout(false);
            this.grpExtract.ResumeLayout(false);
            this.grpExtract.PerformLayout();
            ((ISupportInitialize)(this.numCreateLevel)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
