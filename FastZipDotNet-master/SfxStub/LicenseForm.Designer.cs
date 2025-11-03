using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SfxStub
{
    partial class LicenseForm
    {
        private IContainer components = null;

        private PictureBox picBanner;
        private Label lblTitle;
        private TextBox txtLicense;
        private Button btnOK;
        private Button btnCancel;

        public string License
        {
            get => txtLicense.Text;
            set => txtLicense.Text = value ?? "";
        }

        public bool Accepted => chkAccept.Checked;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            picBanner = new PictureBox();
            lblTitle = new Label();
            txtLicense = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            chkAccept = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label1 = new Label();
            ((ISupportInitialize)picBanner).BeginInit();
            SuspendLayout();
            // 
            // picBanner
            // 
            picBanner.Location = new Point(0, 0);
            picBanner.Name = "picBanner";
            picBanner.Size = new Size(720, 80);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;
            picBanner.TabIndex = 0;
            picBanner.TabStop = false;
            // 
            // lblTitle
            // 
            lblTitle.Location = new Point(12, 88);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(696, 20);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Please review the license agreement:";
            // 
            // txtLicense
            // 
            txtLicense.BackColor = Color.FromArgb(32, 32, 32);
            txtLicense.BorderStyle = BorderStyle.FixedSingle;
            txtLicense.ForeColor = Color.White;
            txtLicense.Location = new Point(12, 112);
            txtLicense.Multiline = true;
            txtLicense.Name = "txtLicense";
            txtLicense.ReadOnly = true;
            txtLicense.ScrollBars = ScrollBars.Vertical;
            txtLicense.Size = new Size(696, 340);
            txtLicense.TabIndex = 2;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnOK.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnOK.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Location = new Point(528, 480);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(84, 28);
            btnOK.TabIndex = 4;
            btnOK.Text = "Continue";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(624, 480);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(84, 28);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            // 
            // chkAccept
            // 
            chkAccept.BackColor = Color.Transparent;
            chkAccept.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkAccept.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkAccept.BoxGradientEnabled = true;
            chkAccept.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkAccept.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkAccept.BoxSize = 14;
            chkAccept.CheckBorderColor = Color.Lime;
            chkAccept.CheckColor = Color.LawnGreen;
            chkAccept.CheckGradientEnabled = true;
            chkAccept.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkAccept.CheckGradientStart = Color.Lime;
            chkAccept.Location = new Point(12, 458);
            chkAccept.Name = "chkAccept";
            chkAccept.Size = new Size(26, 26);
            chkAccept.TabIndex = 6;
            chkAccept.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(32, 464);
            label1.Name = "label1";
            label1.Size = new Size(174, 15);
            label1.TabIndex = 7;
            label1.Text = "I accept the terms of the license";
            // 
            // LicenseForm
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(720, 520);
            Controls.Add(label1);
            Controls.Add(chkAccept);
            Controls.Add(picBanner);
            Controls.Add(lblTitle);
            Controls.Add(txtLicense);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LicenseForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "License Agreement";
            Load += LicenseForm_Load;
            ((ISupportInitialize)picBanner).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkAccept;
        private Label label1;
    }
}