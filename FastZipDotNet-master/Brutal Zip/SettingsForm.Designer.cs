using System.Windows.Forms;

namespace BrutalZip
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblMethod;
        private ComboBox cmbMethod;
        private Label lblLevel;
        private NumericUpDown numLevel;
        private Label lblThreads;
        private CheckBox chkThreadsAuto;
        private NumericUpDown numThreads;
        private GroupBox grpExtractDefault;
        private RadioButton rdoExtractSmart;
        private RadioButton rdoExtractHere;
        private CheckBox chkOpenExplorerAfterCreate;
        private CheckBox chkOpenExplorerAfterExtract;
        private CheckBox chkContextMenu;
        private Button btnOK;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblMethod = new Label();
            cmbMethod = new ComboBox();
            lblLevel = new Label();
            numLevel = new NumericUpDown();
            lblThreads = new Label();
            chkThreadsAuto = new CheckBox();
            numThreads = new NumericUpDown();
            grpExtractDefault = new GroupBox();
            rdoExtractSmart = new RadioButton();
            rdoExtractHere = new RadioButton();
            chkOpenExplorerAfterCreate = new CheckBox();
            chkOpenExplorerAfterExtract = new CheckBox();
            chkContextMenu = new CheckBox();
            btnOK = new Button();
            btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)numLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numThreads).BeginInit();
            grpExtractDefault.SuspendLayout();
            SuspendLayout();
            // 
            // lblMethod
            // 
            lblMethod.Location = new Point(16, 16);
            lblMethod.Name = "lblMethod";
            lblMethod.Size = new Size(120, 23);
            lblMethod.TabIndex = 0;
            lblMethod.Text = "Default method:";
            // 
            // cmbMethod
            // 
            cmbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMethod.Items.AddRange(new object[] { "Store", "Deflate", "Zstd" });
            cmbMethod.Location = new Point(160, 14);
            cmbMethod.Name = "cmbMethod";
            cmbMethod.Size = new Size(220, 23);
            cmbMethod.TabIndex = 1;
            // 
            // lblLevel
            // 
            lblLevel.Location = new Point(16, 52);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(120, 23);
            lblLevel.TabIndex = 2;
            lblLevel.Text = "Default level:";
            // 
            // numLevel
            // 
            numLevel.Location = new Point(160, 50);
            numLevel.Maximum = new decimal(new int[] { 22, 0, 0, 0 });
            numLevel.Name = "numLevel";
            numLevel.Size = new Size(80, 23);
            numLevel.TabIndex = 3;
            numLevel.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // lblThreads
            // 
            lblThreads.Location = new Point(16, 88);
            lblThreads.Name = "lblThreads";
            lblThreads.Size = new Size(120, 23);
            lblThreads.TabIndex = 4;
            lblThreads.Text = "Threads:";
            // 
            // chkThreadsAuto
            // 
            chkThreadsAuto.Checked = true;
            chkThreadsAuto.CheckState = CheckState.Checked;
            chkThreadsAuto.Location = new Point(160, 86);
            chkThreadsAuto.Name = "chkThreadsAuto";
            chkThreadsAuto.Size = new Size(60, 24);
            chkThreadsAuto.TabIndex = 5;
            chkThreadsAuto.Text = "Auto";
            // 
            // numThreads
            // 
            numThreads.Enabled = false;
            numThreads.Location = new Point(230, 84);
            numThreads.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            numThreads.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numThreads.Name = "numThreads";
            numThreads.Size = new Size(80, 23);
            numThreads.TabIndex = 6;
            numThreads.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // grpExtractDefault
            // 
            grpExtractDefault.Controls.Add(rdoExtractSmart);
            grpExtractDefault.Controls.Add(rdoExtractHere);
            grpExtractDefault.Location = new Point(16, 124);
            grpExtractDefault.Name = "grpExtractDefault";
            grpExtractDefault.Size = new Size(488, 72);
            grpExtractDefault.TabIndex = 7;
            grpExtractDefault.TabStop = false;
            grpExtractDefault.Text = "Extract default";
            // 
            // rdoExtractSmart
            // 
            rdoExtractSmart.Checked = true;
            rdoExtractSmart.Location = new Point(12, 22);
            rdoExtractSmart.Name = "rdoExtractSmart";
            rdoExtractSmart.Size = new Size(280, 20);
            rdoExtractSmart.TabIndex = 0;
            rdoExtractSmart.TabStop = true;
            rdoExtractSmart.Text = "Extract to “ArchiveName/” (Smart)";
            // 
            // rdoExtractHere
            // 
            rdoExtractHere.Location = new Point(12, 44);
            rdoExtractHere.Name = "rdoExtractHere";
            rdoExtractHere.Size = new Size(220, 20);
            rdoExtractHere.TabIndex = 1;
            rdoExtractHere.Text = "Extract here (same folder)";
            // 
            // chkOpenExplorerAfterCreate
            // 
            chkOpenExplorerAfterCreate.Location = new Point(16, 206);
            chkOpenExplorerAfterCreate.Name = "chkOpenExplorerAfterCreate";
            chkOpenExplorerAfterCreate.Size = new Size(230, 24);
            chkOpenExplorerAfterCreate.TabIndex = 8;
            chkOpenExplorerAfterCreate.Text = "Open Explorer after create";
            // 
            // chkOpenExplorerAfterExtract
            // 
            chkOpenExplorerAfterExtract.Location = new Point(16, 230);
            chkOpenExplorerAfterExtract.Name = "chkOpenExplorerAfterExtract";
            chkOpenExplorerAfterExtract.Size = new Size(230, 24);
            chkOpenExplorerAfterExtract.TabIndex = 9;
            chkOpenExplorerAfterExtract.Text = "Open Explorer after extract";
            // 
            // chkContextMenu
            // 
            chkContextMenu.Location = new Point(16, 254);
            chkContextMenu.Name = "chkContextMenu";
            chkContextMenu.Size = new Size(260, 24);
            chkContextMenu.TabIndex = 10;
            chkContextMenu.Text = "Add to Explorer context menu";
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(320, 284);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 28);
            btnOK.TabIndex = 11;
            btnOK.Text = "OK";
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(416, 284);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 28);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // SettingsForm
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(520, 330);
            Controls.Add(lblMethod);
            Controls.Add(cmbMethod);
            Controls.Add(lblLevel);
            Controls.Add(numLevel);
            Controls.Add(lblThreads);
            Controls.Add(chkThreadsAuto);
            Controls.Add(numThreads);
            Controls.Add(grpExtractDefault);
            Controls.Add(chkOpenExplorerAfterCreate);
            Controls.Add(chkOpenExplorerAfterExtract);
            Controls.Add(chkContextMenu);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)numLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)numThreads).EndInit();
            grpExtractDefault.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}