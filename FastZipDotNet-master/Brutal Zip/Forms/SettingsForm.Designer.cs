using System.ComponentModel;
using System.Windows.Forms;

namespace BrutalZip
{
    partial class SettingsForm
    {
        private IContainer components = null;

        private Label lblMethod;
        private Label lblLevel;
        private NumericUpDown numLevel;

        private Label lblThreads;
        private Label lblDefaultThreadsValue;

        private GroupBox grpExtractDefault;
        private RadioButton rdoExtractSmart;
        private RadioButton rdoExtractHere;

        private GroupBox grpEncryptDefault;         // NEW
        private Label lblEncryptAlgo;               // NEW

        private Button btnOK;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblMethod = new Label();
            lblLevel = new Label();
            numLevel = new NumericUpDown();
            lblThreads = new Label();
            lblDefaultThreadsValue = new Label();
            grpExtractDefault = new GroupBox();
            rdoExtractSmart = new RadioButton();
            rdoExtractHere = new RadioButton();
            grpEncryptDefault = new GroupBox();
            cmbEncryptAlgo = new BrutalZip2025.BrutalControls.BrutalComboBox();
            label4 = new Label();
            lblEncryptAlgo = new Label();
            chkEncryptNewDefault = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            btnOK = new Button();
            btnCancel = new Button();
            label1 = new Label();
            chkOpenExplorerAfterCreate = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label2 = new Label();
            chkContextMenu = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label3 = new Label();
            chkOpenExplorerAfterExtract = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            cmbMethod = new BrutalZip2025.BrutalControls.BrutalComboBox();
            tbDefaultThreads = new BrutalZip2025.BrutalControls.BrutalTrackBar();
            label5 = new Label();
            chkThreadsAuto = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            ((ISupportInitialize)numLevel).BeginInit();
            grpExtractDefault.SuspendLayout();
            grpEncryptDefault.SuspendLayout();
            ((ISupportInitialize)tbDefaultThreads).BeginInit();
            SuspendLayout();
            // 
            // lblMethod
            // 
            lblMethod.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMethod.Location = new Point(16, 16);
            lblMethod.Name = "lblMethod";
            lblMethod.Size = new Size(120, 23);
            lblMethod.TabIndex = 0;
            lblMethod.Text = "Default method:";
            // 
            // lblLevel
            // 
            lblLevel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLevel.Location = new Point(16, 45);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(120, 23);
            lblLevel.TabIndex = 2;
            lblLevel.Text = "Default level:";
            // 
            // numLevel
            // 
            numLevel.BackColor = Color.FromArgb(32, 32, 32);
            numLevel.BorderStyle = BorderStyle.FixedSingle;
            numLevel.ForeColor = Color.White;
            numLevel.Location = new Point(160, 43);
            numLevel.Maximum = new decimal(new int[] { 22, 0, 0, 0 });
            numLevel.Name = "numLevel";
            numLevel.Size = new Size(80, 23);
            numLevel.TabIndex = 3;
            numLevel.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // lblThreads
            // 
            lblThreads.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblThreads.Location = new Point(16, 73);
            lblThreads.Name = "lblThreads";
            lblThreads.Size = new Size(120, 23);
            lblThreads.TabIndex = 4;
            lblThreads.Text = "Threads:";
            // 
            // lblDefaultThreadsValue
            // 
            lblDefaultThreadsValue.Location = new Point(309, 77);
            lblDefaultThreadsValue.Name = "lblDefaultThreadsValue";
            lblDefaultThreadsValue.Size = new Size(40, 20);
            lblDefaultThreadsValue.TabIndex = 6;
            lblDefaultThreadsValue.Text = "1";
            // 
            // grpExtractDefault
            // 
            grpExtractDefault.Controls.Add(rdoExtractSmart);
            grpExtractDefault.Controls.Add(rdoExtractHere);
            grpExtractDefault.ForeColor = Color.White;
            grpExtractDefault.Location = new Point(16, 99);
            grpExtractDefault.Name = "grpExtractDefault";
            grpExtractDefault.Size = new Size(488, 72);
            grpExtractDefault.TabIndex = 8;
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
            // grpEncryptDefault
            // 
            grpEncryptDefault.Controls.Add(cmbEncryptAlgo);
            grpEncryptDefault.Controls.Add(label4);
            grpEncryptDefault.Controls.Add(lblEncryptAlgo);
            grpEncryptDefault.Controls.Add(chkEncryptNewDefault);
            grpEncryptDefault.ForeColor = Color.White;
            grpEncryptDefault.Location = new Point(16, 181);
            grpEncryptDefault.Name = "grpEncryptDefault";
            grpEncryptDefault.Size = new Size(488, 74);
            grpEncryptDefault.TabIndex = 9;
            grpEncryptDefault.TabStop = false;
            grpEncryptDefault.Text = "Default encryption for new archives";
            // 
            // cmbEncryptAlgo
            // 
            cmbEncryptAlgo.BackColor = Color.FromArgb(32, 32, 32);
            cmbEncryptAlgo.ButtonColor = Color.FromArgb(48, 48, 48);
            cmbEncryptAlgo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEncryptAlgo.ForeColor = Color.White;
            cmbEncryptAlgo.FormattingEnabled = true;
            cmbEncryptAlgo.Items.AddRange(new object[] { "ZipCrypto", "AES-128", "AES-192", "AES-256" });
            cmbEncryptAlgo.Location = new Point(88, 43);
            cmbEncryptAlgo.Name = "cmbEncryptAlgo";
            cmbEncryptAlgo.Size = new Size(129, 23);
            cmbEncryptAlgo.TabIndex = 39;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(33, 22);
            label4.Name = "label4";
            label4.Size = new Size(184, 15);
            label4.TabIndex = 40;
            label4.Text = "Encrypt new archives by default";
            // 
            // lblEncryptAlgo
            // 
            lblEncryptAlgo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEncryptAlgo.Location = new Point(12, 46);
            lblEncryptAlgo.Name = "lblEncryptAlgo";
            lblEncryptAlgo.Size = new Size(70, 20);
            lblEncryptAlgo.TabIndex = 1;
            lblEncryptAlgo.Text = "Algorithm:";
            // 
            // chkEncryptNewDefault
            // 
            chkEncryptNewDefault.BackColor = Color.Transparent;
            chkEncryptNewDefault.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkEncryptNewDefault.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkEncryptNewDefault.BoxGradientEnabled = true;
            chkEncryptNewDefault.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkEncryptNewDefault.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkEncryptNewDefault.BoxSize = 14;
            chkEncryptNewDefault.CheckBorderColor = Color.Lime;
            chkEncryptNewDefault.CheckColor = Color.LawnGreen;
            chkEncryptNewDefault.Checked = true;
            chkEncryptNewDefault.CheckGradientEnabled = true;
            chkEncryptNewDefault.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkEncryptNewDefault.CheckGradientStart = Color.Lime;
            chkEncryptNewDefault.CheckState = CheckState.Checked;
            chkEncryptNewDefault.Location = new Point(12, 16);
            chkEncryptNewDefault.Name = "chkEncryptNewDefault";
            chkEncryptNewDefault.Size = new Size(18, 24);
            chkEncryptNewDefault.TabIndex = 39;
            chkEncryptNewDefault.UseVisualStyleBackColor = false;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnOK.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnOK.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Location = new Point(320, 308);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 28);
            btnOK.TabIndex = 13;
            btnOK.Text = "OK";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(416, 308);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 28);
            btnCancel.TabIndex = 14;
            btnCancel.Text = "Cancel";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(37, 267);
            label1.Name = "label1";
            label1.Size = new Size(157, 15);
            label1.TabIndex = 34;
            label1.Text = "Open Explorer after create";
            // 
            // chkOpenExplorerAfterCreate
            // 
            chkOpenExplorerAfterCreate.BackColor = Color.Transparent;
            chkOpenExplorerAfterCreate.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkOpenExplorerAfterCreate.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkOpenExplorerAfterCreate.BoxGradientEnabled = true;
            chkOpenExplorerAfterCreate.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkOpenExplorerAfterCreate.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkOpenExplorerAfterCreate.BoxSize = 14;
            chkOpenExplorerAfterCreate.CheckBorderColor = Color.Lime;
            chkOpenExplorerAfterCreate.CheckColor = Color.LawnGreen;
            chkOpenExplorerAfterCreate.Checked = true;
            chkOpenExplorerAfterCreate.CheckGradientEnabled = true;
            chkOpenExplorerAfterCreate.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkOpenExplorerAfterCreate.CheckGradientStart = Color.Lime;
            chkOpenExplorerAfterCreate.CheckState = CheckState.Checked;
            chkOpenExplorerAfterCreate.Location = new Point(16, 261);
            chkOpenExplorerAfterCreate.Name = "chkOpenExplorerAfterCreate";
            chkOpenExplorerAfterCreate.Size = new Size(18, 24);
            chkOpenExplorerAfterCreate.TabIndex = 33;
            chkOpenExplorerAfterCreate.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(37, 317);
            label2.Name = "label2";
            label2.Size = new Size(162, 15);
            label2.TabIndex = 36;
            label2.Text = "Open Explorer after extract";
            // 
            // chkContextMenu
            // 
            chkContextMenu.BackColor = Color.Transparent;
            chkContextMenu.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkContextMenu.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkContextMenu.BoxGradientEnabled = true;
            chkContextMenu.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkContextMenu.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkContextMenu.BoxSize = 14;
            chkContextMenu.CheckBorderColor = Color.Lime;
            chkContextMenu.CheckColor = Color.LawnGreen;
            chkContextMenu.Checked = true;
            chkContextMenu.CheckGradientEnabled = true;
            chkContextMenu.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkContextMenu.CheckGradientStart = Color.Lime;
            chkContextMenu.CheckState = CheckState.Checked;
            chkContextMenu.Location = new Point(16, 311);
            chkContextMenu.Name = "chkContextMenu";
            chkContextMenu.Size = new Size(18, 24);
            chkContextMenu.TabIndex = 35;
            chkContextMenu.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(37, 291);
            label3.Name = "label3";
            label3.Size = new Size(176, 15);
            label3.TabIndex = 38;
            label3.Text = "Add to Explorer context menu";
            // 
            // chkOpenExplorerAfterExtract
            // 
            chkOpenExplorerAfterExtract.BackColor = Color.Transparent;
            chkOpenExplorerAfterExtract.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkOpenExplorerAfterExtract.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkOpenExplorerAfterExtract.BoxGradientEnabled = true;
            chkOpenExplorerAfterExtract.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkOpenExplorerAfterExtract.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkOpenExplorerAfterExtract.BoxSize = 14;
            chkOpenExplorerAfterExtract.CheckBorderColor = Color.Lime;
            chkOpenExplorerAfterExtract.CheckColor = Color.LawnGreen;
            chkOpenExplorerAfterExtract.Checked = true;
            chkOpenExplorerAfterExtract.CheckGradientEnabled = true;
            chkOpenExplorerAfterExtract.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkOpenExplorerAfterExtract.CheckGradientStart = Color.Lime;
            chkOpenExplorerAfterExtract.CheckState = CheckState.Checked;
            chkOpenExplorerAfterExtract.Location = new Point(16, 285);
            chkOpenExplorerAfterExtract.Name = "chkOpenExplorerAfterExtract";
            chkOpenExplorerAfterExtract.Size = new Size(18, 24);
            chkOpenExplorerAfterExtract.TabIndex = 37;
            chkOpenExplorerAfterExtract.UseVisualStyleBackColor = false;
            // 
            // cmbMethod
            // 
            cmbMethod.BackColor = Color.FromArgb(32, 32, 32);
            cmbMethod.ButtonColor = Color.FromArgb(48, 48, 48);
            cmbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMethod.ForeColor = Color.White;
            cmbMethod.FormattingEnabled = true;
            cmbMethod.Items.AddRange(new object[] { "Store", "Deflate", "Zstd" });
            cmbMethod.Location = new Point(160, 13);
            cmbMethod.Name = "cmbMethod";
            cmbMethod.Size = new Size(220, 23);
            cmbMethod.TabIndex = 40;
            // 
            // tbDefaultThreads
            // 
            tbDefaultThreads.LeftBarColor = Color.FromArgb(29, 181, 82);
            tbDefaultThreads.Location = new Point(160, 62);
            tbDefaultThreads.Maximum = 100;
            tbDefaultThreads.Name = "tbDefaultThreads";
            tbDefaultThreads.RightBarColor = Color.FromArgb(22, 132, 99);
            tbDefaultThreads.Size = new Size(143, 45);
            tbDefaultThreads.TabIndex = 41;
            tbDefaultThreads.ThumbInnerColor = Color.FromArgb(29, 181, 82);
            tbDefaultThreads.ThumbOutlineColor = Color.FromArgb(19, 90, 42);
            tbDefaultThreads.ThumbOutlineThickness = 2;
            tbDefaultThreads.Value = 50;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(352, 79);
            label5.Name = "label5";
            label5.Size = new Size(34, 15);
            label5.TabIndex = 43;
            label5.Text = "Auto";
            // 
            // chkThreadsAuto
            // 
            chkThreadsAuto.BackColor = Color.Transparent;
            chkThreadsAuto.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkThreadsAuto.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkThreadsAuto.BoxGradientEnabled = true;
            chkThreadsAuto.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkThreadsAuto.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkThreadsAuto.BoxSize = 14;
            chkThreadsAuto.CheckBorderColor = Color.Lime;
            chkThreadsAuto.CheckColor = Color.LawnGreen;
            chkThreadsAuto.Checked = true;
            chkThreadsAuto.CheckGradientEnabled = true;
            chkThreadsAuto.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkThreadsAuto.CheckGradientStart = Color.Lime;
            chkThreadsAuto.CheckState = CheckState.Checked;
            chkThreadsAuto.Location = new Point(331, 73);
            chkThreadsAuto.Name = "chkThreadsAuto";
            chkThreadsAuto.Size = new Size(18, 24);
            chkThreadsAuto.TabIndex = 42;
            chkThreadsAuto.UseVisualStyleBackColor = false;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(520, 347);
            Controls.Add(label5);
            Controls.Add(chkThreadsAuto);
            Controls.Add(cmbMethod);
            Controls.Add(label3);
            Controls.Add(chkOpenExplorerAfterExtract);
            Controls.Add(label2);
            Controls.Add(chkContextMenu);
            Controls.Add(label1);
            Controls.Add(chkOpenExplorerAfterCreate);
            Controls.Add(lblMethod);
            Controls.Add(lblLevel);
            Controls.Add(numLevel);
            Controls.Add(lblThreads);
            Controls.Add(lblDefaultThreadsValue);
            Controls.Add(grpExtractDefault);
            Controls.Add(grpEncryptDefault);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Controls.Add(tbDefaultThreads);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            Load += SettingsForm_Load;
            ((ISupportInitialize)numLevel).EndInit();
            grpExtractDefault.ResumeLayout(false);
            grpEncryptDefault.ResumeLayout(false);
            grpEncryptDefault.PerformLayout();
            ((ISupportInitialize)tbDefaultThreads).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        private Label label1;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkOpenExplorerAfterCreate;
        private Label label2;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkContextMenu;
        private Label label3;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkOpenExplorerAfterExtract;
        private Label label4;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkEncryptNewDefault;
        private BrutalZip2025.BrutalControls.BrutalComboBox cmbEncryptAlgo;
        private BrutalZip2025.BrutalControls.BrutalComboBox cmbMethod;
        private BrutalZip2025.BrutalControls.BrutalTrackBar tbDefaultThreads;
        private Label label5;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkThreadsAuto;
    }
}