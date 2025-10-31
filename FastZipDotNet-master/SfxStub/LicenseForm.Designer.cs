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
        private CheckBox chkAccept;
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
            components = new Container();

            picBanner = new PictureBox();
            lblTitle = new Label();
            txtLicense = new TextBox();
            chkAccept = new CheckBox();
            btnOK = new Button();
            btnCancel = new Button();

            SuspendLayout();

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(720, 520);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "License Agreement";
            AcceptButton = btnOK;
            CancelButton = btnCancel;

            picBanner.Location = new Point(0, 0);
            picBanner.Size = new Size(720, 80);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;

            lblTitle.Text = "Please review the license agreement:";
            lblTitle.Location = new Point(12, 88);
            lblTitle.Size = new Size(696, 20);

            txtLicense.Location = new Point(12, 112);
            txtLicense.Size = new Size(696, 340);
            txtLicense.Multiline = true;
            txtLicense.ScrollBars = ScrollBars.Vertical;
            txtLicense.ReadOnly = true;

            chkAccept.Text = "I accept the terms of the license";
            chkAccept.Location = new Point(12, 460);
            chkAccept.Size = new Size(300, 20);

            btnOK.Text = "Continue";
            btnOK.Location = new Point(528, 480);
            btnOK.Size = new Size(84, 28);
            btnOK.DialogResult = DialogResult.OK;

            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(624, 480);
            btnCancel.Size = new Size(84, 28);
            btnCancel.DialogResult = DialogResult.Cancel;

            Controls.Add(picBanner);
            Controls.Add(lblTitle);
            Controls.Add(txtLicense);
            Controls.Add(chkAccept);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);

            ResumeLayout(false);
            PerformLayout();
        }
    }
}