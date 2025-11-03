using System.ComponentModel;

namespace Brutal_Zip
{
    partial class InputDialog
    {
        private IContainer components = null;
        private Label lblPrompt;
        private TextBox txtValue;
        private Button btnOK;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblPrompt = new Label();
            txtValue = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblPrompt
            // 
            lblPrompt.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPrompt.Location = new Point(12, 16);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Size = new Size(496, 20);
            lblPrompt.TabIndex = 0;
            lblPrompt.Text = "Enter value:";
            // 
            // txtValue
            // 
            txtValue.BackColor = Color.FromArgb(32, 32, 32);
            txtValue.BorderStyle = BorderStyle.FixedSingle;
            txtValue.ForeColor = Color.White;
            txtValue.Location = new Point(12, 40);
            txtValue.Name = "txtValue";
            txtValue.Size = new Size(496, 23);
            txtValue.TabIndex = 1;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnOK.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnOK.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Location = new Point(340, 88);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(80, 28);
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
            btnCancel.Location = new Point(428, 88);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 28);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            // 
            // InputDialog
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(520, 140);
            Controls.Add(lblPrompt);
            Controls.Add(txtValue);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Input";
            Load += InputDialog_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}