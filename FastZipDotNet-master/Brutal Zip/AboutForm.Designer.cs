namespace Brutal_Zip
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Label lblVersion;
        private Label lblCopyright;
        private LinkLabel lnkSite;
        private Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblVersion = new Label();
            lblCopyright = new Label();
            lnkSite = new LinkLabel();
            btnClose = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(16, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(420, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Brutal Zip";
            // 
            // lblVersion
            // 
            lblVersion.Location = new Point(18, 56);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(420, 20);
            lblVersion.TabIndex = 1;
            lblVersion.Text = "Version 1.0.0.0";
            // 
            // lblCopyright
            // 
            lblCopyright.Location = new Point(18, 78);
            lblCopyright.Name = "lblCopyright";
            lblCopyright.Size = new Size(420, 20);
            lblCopyright.TabIndex = 2;
            lblCopyright.Text = "© 2025 Limintel Solutions";
            // 
            // lnkSite
            // 
            lnkSite.LinkColor = Color.RoyalBlue;
            lnkSite.Location = new Point(18, 100);
            lnkSite.Name = "lnkSite";
            lnkSite.Size = new Size(420, 20);
            lnkSite.TabIndex = 3;
            lnkSite.TabStop = true;
            lnkSite.Text = "https://example.com/brutalzip";
            // 
            // btnClose
            // 
            btnClose.DialogResult = DialogResult.OK;
            btnClose.Location = new Point(356, 140);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 28);
            btnClose.TabIndex = 4;
            btnClose.Text = "Close";
            // 
            // AboutForm
            // 
            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(460, 200);
            Controls.Add(lblTitle);
            Controls.Add(lblVersion);
            Controls.Add(lblCopyright);
            Controls.Add(lnkSite);
            Controls.Add(btnClose);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "About Brutal Zip";
            ResumeLayout(false);

            // Events wired in code-behind
        }
    }
}