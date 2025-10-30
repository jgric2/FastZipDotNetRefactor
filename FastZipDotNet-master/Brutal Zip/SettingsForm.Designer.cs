using System.ComponentModel;
using System.Windows.Forms;

namespace BrutalZip
{
    partial class SettingsForm
    {
        private IContainer components = null;

        private Label lblMethod;
        private ComboBox cmbMethod;
        private Label lblLevel;
        private NumericUpDown numLevel;

        private Label lblThreads;
        private TrackBar tbDefaultThreads;
        private Label lblDefaultThreadsValue;
        private CheckBox chkThreadsAuto;

        private GroupBox grpExtractDefault;
        private RadioButton rdoExtractSmart;
        private RadioButton rdoExtractHere;

        private GroupBox grpEncryptDefault;         // NEW
        private CheckBox chkEncryptNewDefault;      // NEW
        private Label lblEncryptAlgo;               // NEW
        private ComboBox cmbEncryptAlgo;            // NEW

        private CheckBox chkOpenExplorerAfterCreate;
        private CheckBox chkOpenExplorerAfterExtract;
        private CheckBox chkContextMenu;

        private Button btnOK;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();

            this.lblMethod = new Label();
            this.cmbMethod = new ComboBox();
            this.lblLevel = new Label();
            this.numLevel = new NumericUpDown();

            this.lblThreads = new Label();
            this.tbDefaultThreads = new TrackBar();
            this.lblDefaultThreadsValue = new Label();
            this.chkThreadsAuto = new CheckBox();

            this.grpExtractDefault = new GroupBox();
            this.rdoExtractSmart = new RadioButton();
            this.rdoExtractHere = new RadioButton();

            this.grpEncryptDefault = new GroupBox();       // NEW
            this.chkEncryptNewDefault = new CheckBox();    // NEW
            this.lblEncryptAlgo = new Label();             // NEW
            this.cmbEncryptAlgo = new ComboBox();          // NEW

            this.chkOpenExplorerAfterCreate = new CheckBox();
            this.chkOpenExplorerAfterExtract = new CheckBox();
            this.chkContextMenu = new CheckBox();

            this.btnOK = new Button();
            this.btnCancel = new Button();

            ((ISupportInitialize)(this.numLevel)).BeginInit();
            ((ISupportInitialize)(this.tbDefaultThreads)).BeginInit();
            this.grpExtractDefault.SuspendLayout();
            this.grpEncryptDefault.SuspendLayout(); // NEW
            this.SuspendLayout();

            // SettingsForm
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Text = "Settings";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(520, 430); // increased height

            // Default method
            this.lblMethod.Text = "Default method:";
            this.lblMethod.Location = new Point(16, 16);
            this.lblMethod.Size = new Size(120, 23);

            this.cmbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbMethod.Items.AddRange(new object[] { "Store", "Deflate", "Zstd" });
            this.cmbMethod.Location = new Point(160, 14);
            this.cmbMethod.Size = new Size(220, 23);

            // Default level
            this.lblLevel.Text = "Default level:";
            this.lblLevel.Location = new Point(16, 52);
            this.lblLevel.Size = new Size(120, 23);

            this.numLevel.Location = new Point(160, 50);
            this.numLevel.Size = new Size(80, 23);
            this.numLevel.Minimum = 0;
            this.numLevel.Maximum = 22;
            this.numLevel.Value = 6;

            // Default threads slider
            this.lblThreads.Text = "Threads:";
            this.lblThreads.Location = new Point(16, 88);
            this.lblThreads.Size = new Size(120, 23);

            this.tbDefaultThreads.Location = new Point(160, 84);
            this.tbDefaultThreads.Size = new Size(220, 45);
            this.tbDefaultThreads.Minimum = 1;
            this.tbDefaultThreads.Maximum = 4; // overridden in code-behind LoadUi
            this.tbDefaultThreads.TickFrequency = 1;
            this.tbDefaultThreads.SmallChange = 1;
            this.tbDefaultThreads.LargeChange = 1;
            this.tbDefaultThreads.Value = 1;

            this.lblDefaultThreadsValue.Text = "1";
            this.lblDefaultThreadsValue.Location = new Point(160 + 220 + 8, 88);
            this.lblDefaultThreadsValue.Size = new Size(40, 20);

            this.chkThreadsAuto.Text = "Auto";
            this.chkThreadsAuto.Location = new Point(160 + 220 + 8 + 48, 86);
            this.chkThreadsAuto.Size = new Size(60, 24);
            this.chkThreadsAuto.Checked = true;

            // Extract default group
            this.grpExtractDefault.Text = "Extract default";
            this.grpExtractDefault.Location = new Point(16, 124);
            this.grpExtractDefault.Size = new Size(488, 72);

            this.rdoExtractSmart.Text = "Extract to “ArchiveName/” (Smart)";
            this.rdoExtractSmart.Location = new Point(12, 22);
            this.rdoExtractSmart.Size = new Size(280, 20);
            this.rdoExtractSmart.Checked = true;

            this.rdoExtractHere.Text = "Extract here (same folder)";
            this.rdoExtractHere.Location = new Point(12, 44);
            this.rdoExtractHere.Size = new Size(220, 20);

            this.grpExtractDefault.Controls.Add(this.rdoExtractSmart);
            this.grpExtractDefault.Controls.Add(this.rdoExtractHere);

            // NEW: Default encryption group
            this.grpEncryptDefault.Text = "Default encryption for new archives";
            this.grpEncryptDefault.Location = new Point(16, 206);
            this.grpEncryptDefault.Size = new Size(488, 72);

            this.chkEncryptNewDefault.Text = "Encrypt new archives by default";
            this.chkEncryptNewDefault.Location = new Point(12, 22);
            this.chkEncryptNewDefault.Size = new Size(240, 20);

            this.lblEncryptAlgo.Text = "Algorithm:";
            this.lblEncryptAlgo.Location = new Point(12, 46);
            this.lblEncryptAlgo.Size = new Size(70, 20);

            this.cmbEncryptAlgo.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbEncryptAlgo.Items.AddRange(new object[] { "ZipCrypto", "AES-128", "AES-192", "AES-256" });
            this.cmbEncryptAlgo.Location = new Point(90, 44);
            this.cmbEncryptAlgo.Size = new Size(160, 23);

            this.grpEncryptDefault.Controls.Add(this.chkEncryptNewDefault);
            this.grpEncryptDefault.Controls.Add(this.lblEncryptAlgo);
            this.grpEncryptDefault.Controls.Add(this.cmbEncryptAlgo);

            // Explorer options
            this.chkOpenExplorerAfterCreate.Text = "Open Explorer after create";
            this.chkOpenExplorerAfterCreate.Location = new Point(16, 286);
            this.chkOpenExplorerAfterCreate.Size = new Size(230, 24);

            this.chkOpenExplorerAfterExtract.Text = "Open Explorer after extract";
            this.chkOpenExplorerAfterExtract.Location = new Point(16, 310);
            this.chkOpenExplorerAfterExtract.Size = new Size(230, 24);

            this.chkContextMenu.Text = "Add to Explorer context menu";
            this.chkContextMenu.Location = new Point(16, 334);
            this.chkContextMenu.Size = new Size(260, 24);

            // Buttons
            this.btnOK.Text = "OK";
            this.btnOK.Location = new Point(320, 378);
            this.btnOK.Size = new Size(88, 28);
            this.btnOK.DialogResult = DialogResult.OK;

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new Point(416, 378);
            this.btnCancel.Size = new Size(88, 28);
            this.btnCancel.DialogResult = DialogResult.Cancel;

            // Add controls
            this.Controls.Add(this.lblMethod);
            this.Controls.Add(this.cmbMethod);
            this.Controls.Add(this.lblLevel);
            this.Controls.Add(this.numLevel);

            this.Controls.Add(this.lblThreads);
            this.Controls.Add(this.tbDefaultThreads);
            this.Controls.Add(this.lblDefaultThreadsValue);
            this.Controls.Add(this.chkThreadsAuto);

            this.Controls.Add(this.grpExtractDefault);
            this.Controls.Add(this.grpEncryptDefault); // NEW group
            this.Controls.Add(this.chkOpenExplorerAfterCreate);
            this.Controls.Add(this.chkOpenExplorerAfterExtract);
            this.Controls.Add(this.chkContextMenu);

            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);

            ((ISupportInitialize)(this.numLevel)).EndInit();
            ((ISupportInitialize)(this.tbDefaultThreads)).EndInit();
            this.grpExtractDefault.ResumeLayout(false);
            this.grpEncryptDefault.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}