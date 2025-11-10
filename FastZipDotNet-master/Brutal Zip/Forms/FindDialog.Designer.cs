namespace Brutal_Zip
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    partial class FindDialog
    {
        private IContainer components = null;
        private Label lblPattern;
        internal TextBox txtPattern;
        //private CheckBox chkCase;
        private Label lblHint;
        private Button btnOK;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblPattern = new Label();
            txtPattern = new TextBox();
            lblHint = new Label();
            btnOK = new Button();
            btnCancel = new Button();
            label1 = new Label();
            chkCase = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            SuspendLayout();
            // 
            // lblPattern
            // 
            lblPattern.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPattern.Location = new Point(12, 16);
            lblPattern.Name = "lblPattern";
            lblPattern.Size = new Size(100, 20);
            lblPattern.TabIndex = 0;
            lblPattern.Text = "Name pattern:";
            // 
            // txtPattern
            // 
            txtPattern.BackColor = Color.FromArgb(32, 32, 32);
            txtPattern.BorderStyle = BorderStyle.FixedSingle;
            txtPattern.ForeColor = Color.White;
            txtPattern.Location = new Point(120, 14);
            txtPattern.Name = "txtPattern";
            txtPattern.Size = new Size(280, 23);
            txtPattern.TabIndex = 1;
            txtPattern.Text = "*";
            // 
            // lblHint
            // 
            lblHint.Location = new Point(120, 68);
            lblHint.Name = "lblHint";
            lblHint.Size = new Size(280, 20);
            lblHint.TabIndex = 3;
            lblHint.Text = "Use * and ? wildcards. Example: *.dll;*.exe";
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnOK.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnOK.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Location = new Point(220, 108);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(80, 28);
            btnOK.TabIndex = 4;
            btnOK.Text = "Find";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(320, 108);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 28);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(141, 47);
            label1.Name = "label1";
            label1.Size = new Size(69, 15);
            label1.TabIndex = 34;
            label1.Text = "Match case";
            // 
            // chkCase
            // 
            chkCase.BackColor = Color.Transparent;
            chkCase.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkCase.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkCase.BoxGradientEnabled = true;
            chkCase.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkCase.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkCase.BoxSize = 14;
            chkCase.CheckBorderColor = Color.Lime;
            chkCase.CheckColor = Color.LawnGreen;
            chkCase.Checked = true;
            chkCase.CheckGradientEnabled = true;
            chkCase.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkCase.CheckGradientStart = Color.Lime;
            chkCase.CheckState = CheckState.Checked;
            chkCase.Location = new Point(120, 41);
            chkCase.Name = "chkCase";
            chkCase.Size = new Size(18, 24);
            chkCase.TabIndex = 33;
            chkCase.UseVisualStyleBackColor = false;
            chkCase.CheckedChanged += chkCase_CheckedChanged;
            // 
            // FindDialog
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(420, 160);
            Controls.Add(label1);
            Controls.Add(chkCase);
            Controls.Add(lblPattern);
            Controls.Add(txtPattern);
            Controls.Add(lblHint);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FindDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Find in archive";
            Load += FindDialog_Load;
            ResumeLayout(false);
            PerformLayout();
        }
        private Label label1;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkCase;
    }
}