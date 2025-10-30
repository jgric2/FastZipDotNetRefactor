using System.ComponentModel;

namespace Brutal_Zip
{
    partial class PasswordDialog
    {
        private IContainer components = null;
        private Label lblPrompt;
        private TextBox txtPassword;
        private Button btnOK;
        private Button btnCancel;

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
            SuspendLayout();
            // 
            // PasswordDialog
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(420, 140);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Enter Password";
            AcceptButton = btnOK;
            CancelButton = btnCancel;
            // 
            // lblPrompt
            // 
            lblPrompt.Location = new Point(12, 16);
            lblPrompt.Size = new Size(396, 20);
            lblPrompt.Text = "This archive is encrypted. Enter password:";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(16, 44);
            txtPassword.Size = new Size(388, 23);
            txtPassword.UseSystemPasswordChar = true;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(232, 88);
            btnOK.Size = new Size(80, 28);
            btnOK.Text = "OK";
            btnOK.DialogResult = DialogResult.OK;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(324, 88);
            btnCancel.Size = new Size(80, 28);
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;

            Controls.Add(lblPrompt);
            Controls.Add(txtPassword);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}