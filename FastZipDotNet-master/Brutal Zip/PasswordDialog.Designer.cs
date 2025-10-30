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
        private Button btnCrack; // NEW

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblPrompt = new Label();
            txtPassword = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            btnCrack = new Button();
            checkBoxShowPassword = new CheckBox();
            SuspendLayout();
            // 
            // lblPrompt
            // 
            lblPrompt.Location = new Point(12, 16);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Size = new Size(456, 20);
            lblPrompt.TabIndex = 0;
            lblPrompt.Text = "This archive is encrypted. Enter password:";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(16, 44);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(448, 23);
            txtPassword.TabIndex = 1;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(288, 100);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(84, 28);
            btnOK.TabIndex = 2;
            btnOK.Text = "OK";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(380, 100);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(84, 28);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            // 
            // btnCrack
            // 
            btnCrack.Location = new Point(16, 100);
            btnCrack.Name = "btnCrack";
            btnCrack.Size = new Size(120, 28);
            btnCrack.TabIndex = 4;
            btnCrack.Text = "Crack…";
            btnCrack.Visible = false;
            btnCrack.Click += btnCrack_Click;
            // 
            // checkBoxShowPassword
            // 
            checkBoxShowPassword.AutoSize = true;
            checkBoxShowPassword.Location = new Point(16, 75);
            checkBoxShowPassword.Name = "checkBoxShowPassword";
            checkBoxShowPassword.Size = new Size(108, 19);
            checkBoxShowPassword.TabIndex = 5;
            checkBoxShowPassword.Text = "Show password";
            checkBoxShowPassword.UseVisualStyleBackColor = true;
            checkBoxShowPassword.CheckedChanged += checkBoxShowPassword_CheckedChanged;
            // 
            // PasswordDialog
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(480, 160);
            Controls.Add(checkBoxShowPassword);
            Controls.Add(lblPrompt);
            Controls.Add(txtPassword);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Controls.Add(btnCrack);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PasswordDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Enter Password";
            ResumeLayout(false);
            PerformLayout();
        }
        private CheckBox checkBoxShowPassword;
    }
}