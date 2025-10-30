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
            components = new Container();
            lblTitle = new Label();
            txtComment = new TextBox();
            lblCount = new Label();
            btnOK = new Button();
            btnCancel = new Button();

            SuspendLayout();

            // form
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(640, 420);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Archive Comment";
            AcceptButton = btnOK;
            CancelButton = btnCancel;

            // lblTitle
            lblTitle.Location = new Point(12, 12);
            lblTitle.Size = new Size(616, 20);
            lblTitle.Text = "Edit the ZIP archive comment (UTF-8, up to 65535 bytes):";

            // txtComment
            txtComment.Location = new Point(12, 36);
            txtComment.Size = new Size(616, 320);
            txtComment.Multiline = true;
            txtComment.ScrollBars = ScrollBars.Vertical;

            // lblCount
            lblCount.Location = new Point(12, 362);
            lblCount.Size = new Size(400, 20);
            lblCount.Text = "0 / 65535 bytes";

            // btnOK
            btnOK.Location = new Point(452, 370);
            btnOK.Size = new Size(80, 28);
            btnOK.Text = "OK";
            btnOK.DialogResult = DialogResult.OK;

            // btnCancel
            btnCancel.Location = new Point(548, 370);
            btnCancel.Size = new Size(80, 28);
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;

            Controls.Add(lblTitle);
            Controls.Add(txtComment);
            Controls.Add(lblCount);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);

            ResumeLayout(false);
            PerformLayout();
        }
    }
}