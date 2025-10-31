using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SfxStub
{
    partial class PasswordDialog
    {
        private IContainer components = null;

        private Label lblPrompt;
        private TextBox txtPassword;
        private Button btnOK;
        private Button btnCancel;
        private Label lblInfo;
        private PictureBox picBanner;

        public string Password => txtPassword.Text;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            lblPrompt = new Label();
            txtPassword = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            lblInfo = new Label();
            picBanner = new PictureBox();

            SuspendLayout();

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(540, 200);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Enter Password";
            AcceptButton = btnOK;
            CancelButton = btnCancel;

            picBanner.Location = new Point(0, 0);
            picBanner.Size = new Size(540, 60);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;

            lblPrompt.Text = "This archive is encrypted. Enter password:";
            lblPrompt.Location = new Point(12, 70);
            lblPrompt.Size = new Size(516, 20);

            txtPassword.Location = new Point(12, 94);
            txtPassword.Size = new Size(516, 23);
            txtPassword.UseSystemPasswordChar = true;

            lblInfo.Text = "";
            lblInfo.Location = new Point(12, 124);
            lblInfo.Size = new Size(516, 20);
            lblInfo.ForeColor = Color.DimGray;

            btnOK.Text = "OK";
            btnOK.Location = new Point(360, 160);
            btnOK.Size = new Size(80, 28);
            btnOK.DialogResult = DialogResult.OK;

            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(448, 160);
            btnCancel.Size = new Size(80, 28);
            btnCancel.DialogResult = DialogResult.Cancel;

            Controls.Add(picBanner);
            Controls.Add(lblPrompt);
            Controls.Add(txtPassword);
            Controls.Add(lblInfo);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
