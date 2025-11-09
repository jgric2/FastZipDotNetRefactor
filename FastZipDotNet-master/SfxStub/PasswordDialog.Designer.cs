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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(PasswordDialog));
            lblPrompt = new Label();
            txtPassword = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            lblInfo = new Label();
            picBanner = new PictureBox();
            ((ISupportInitialize)picBanner).BeginInit();
            SuspendLayout();
            // 
            // lblPrompt
            // 
            lblPrompt.Location = new Point(12, 70);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Size = new Size(516, 20);
            lblPrompt.TabIndex = 1;
            lblPrompt.Text = "This archive is encrypted. Enter password:";
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.FromArgb(32, 32, 32);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.ForeColor = Color.White;
            txtPassword.Location = new Point(12, 94);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(516, 23);
            txtPassword.TabIndex = 2;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnOK.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnOK.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Location = new Point(360, 160);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(80, 28);
            btnOK.TabIndex = 4;
            btnOK.Text = "OK";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(448, 160);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 28);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            // 
            // lblInfo
            // 
            lblInfo.ForeColor = Color.DimGray;
            lblInfo.Location = new Point(12, 124);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(516, 20);
            lblInfo.TabIndex = 3;
            // 
            // picBanner
            // 
            picBanner.Location = new Point(0, 0);
            picBanner.Name = "picBanner";
            picBanner.Size = new Size(540, 60);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;
            picBanner.TabIndex = 0;
            picBanner.TabStop = false;
            // 
            // PasswordDialog
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(540, 200);
            Controls.Add(picBanner);
            Controls.Add(lblPrompt);
            Controls.Add(txtPassword);
            Controls.Add(lblInfo);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PasswordDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Enter Password";
            Load += PasswordDialog_Load;
            ((ISupportInitialize)picBanner).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
