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
            checkBoxShowPassword = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // lblPrompt
            // 
            lblPrompt.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPrompt.Location = new Point(12, 16);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Size = new Size(456, 20);
            lblPrompt.TabIndex = 0;
            lblPrompt.Text = "This archive is encrypted. Enter password:";
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.FromArgb(32, 32, 32);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.ForeColor = Color.White;
            txtPassword.Location = new Point(16, 44);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(448, 23);
            txtPassword.TabIndex = 1;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnOK.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnOK.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Location = new Point(288, 100);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(84, 28);
            btnOK.TabIndex = 2;
            btnOK.Text = "OK";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(380, 100);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(84, 28);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            // 
            // btnCrack
            // 
            btnCrack.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCrack.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCrack.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCrack.FlatStyle = FlatStyle.Flat;
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
            checkBoxShowPassword.BackColor = Color.Transparent;
            checkBoxShowPassword.BoxBackColor = Color.FromArgb(64, 64, 64);
            checkBoxShowPassword.BoxBorderColor = Color.FromArgb(29, 181, 82);
            checkBoxShowPassword.BoxGradientEnabled = true;
            checkBoxShowPassword.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            checkBoxShowPassword.BoxGradientStart = Color.FromArgb(29, 181, 82);
            checkBoxShowPassword.BoxSize = 14;
            checkBoxShowPassword.CheckBorderColor = Color.Lime;
            checkBoxShowPassword.CheckColor = Color.LawnGreen;
            checkBoxShowPassword.CheckGradientEnabled = true;
            checkBoxShowPassword.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            checkBoxShowPassword.CheckGradientStart = Color.Lime;
            checkBoxShowPassword.Location = new Point(16, 71);
            checkBoxShowPassword.Name = "checkBoxShowPassword";
            checkBoxShowPassword.Size = new Size(18, 24);
            checkBoxShowPassword.TabIndex = 31;
            checkBoxShowPassword.UseVisualStyleBackColor = false;
            checkBoxShowPassword.CheckedChanged += chkEncrypt_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(37, 77);
            label1.Name = "label1";
            label1.Size = new Size(93, 15);
            label1.TabIndex = 32;
            label1.Text = "Show password";
            // 
            // PasswordDialog
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(480, 160);
            Controls.Add(label1);
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
            Load += PasswordDialog_Load;
            ResumeLayout(false);
            PerformLayout();
        }
        private BrutalZip2025.BrutalControls.BrutalCheckBox checkBoxShowPassword;
        private Label label1;
    }
}