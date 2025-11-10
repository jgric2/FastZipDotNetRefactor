using System.ComponentModel;

namespace Brutal_Zip
{
    partial class ArchiveCommentForm
    {
        private IContainer components = null;

        private Label lblTitle;
        private TextBox txtComment;
        private Label lblCount;
        private Button btnOK;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtComment = new TextBox();
            lblCount = new Label();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTitle.Location = new Point(12, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(616, 20);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Edit the ZIP archive comment (UTF-8, up to 65535 bytes):";
            // 
            // txtComment
            // 
            txtComment.BackColor = Color.FromArgb(32, 32, 32);
            txtComment.BorderStyle = BorderStyle.FixedSingle;
            txtComment.ForeColor = Color.White;
            txtComment.Location = new Point(12, 36);
            txtComment.Multiline = true;
            txtComment.Name = "txtComment";
            txtComment.ScrollBars = ScrollBars.Vertical;
            txtComment.Size = new Size(616, 320);
            txtComment.TabIndex = 1;
            // 
            // lblCount
            // 
            lblCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCount.Location = new Point(12, 362);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(400, 20);
            lblCount.TabIndex = 2;
            lblCount.Text = "0 / 65535 bytes";
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnOK.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnOK.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Location = new Point(452, 370);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(80, 28);
            btnOK.TabIndex = 3;
            btnOK.Text = "OK";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(548, 370);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 28);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            // 
            // ArchiveCommentForm
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(640, 420);
            Controls.Add(lblTitle);
            Controls.Add(txtComment);
            Controls.Add(lblCount);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ArchiveCommentForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Archive Comment";
            Load += ArchiveCommentForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}