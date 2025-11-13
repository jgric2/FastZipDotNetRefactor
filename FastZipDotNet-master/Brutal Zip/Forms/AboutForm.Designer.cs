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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            lblTitle = new Label();
            lblVersion = new Label();
            lblCopyright = new Label();
            lnkSite = new LinkLabel();
            btnClose = new Button();
            brutalGradientPanel1 = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            panel1 = new Panel();
            buttonDictionary = new Button();
            buttonAbout = new Button();
            panelBranding = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            label1 = new Label();
            brutalGradientPanel1.SuspendLayout();
            panelBranding.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(16, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(420, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Brutal Zip";
            // 
            // lblVersion
            // 
            lblVersion.BackColor = Color.Transparent;
            lblVersion.Location = new Point(18, 50);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(420, 20);
            lblVersion.TabIndex = 1;
            lblVersion.Text = "Version 1.0.0.0";
            // 
            // lblCopyright
            // 
            lblCopyright.BackColor = Color.Transparent;
            lblCopyright.Location = new Point(18, 72);
            lblCopyright.Name = "lblCopyright";
            lblCopyright.Size = new Size(420, 20);
            lblCopyright.TabIndex = 2;
            lblCopyright.Text = "© 2025 Limintel Solutions";
            // 
            // lnkSite
            // 
            lnkSite.BackColor = Color.Transparent;
            lnkSite.LinkColor = Color.RoyalBlue;
            lnkSite.Location = new Point(18, 205);
            lnkSite.Name = "lnkSite";
            lnkSite.Size = new Size(420, 20);
            lnkSite.TabIndex = 3;
            lnkSite.TabStop = true;
            lnkSite.Text = "https://limintel.com/";
            // 
            // btnClose
            // 
            btnClose.DialogResult = DialogResult.OK;
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnClose.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(359, 223);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 28);
            btnClose.TabIndex = 4;
            btnClose.Text = "Close";
            // 
            // brutalGradientPanel1
            // 
            brutalGradientPanel1.BackColor = Color.Transparent;
            brutalGradientPanel1.Controls.Add(panel1);
            brutalGradientPanel1.Controls.Add(buttonDictionary);
            brutalGradientPanel1.Controls.Add(buttonAbout);
            brutalGradientPanel1.Dock = DockStyle.Top;
            brutalGradientPanel1.EndColor = Color.FromArgb(64, 64, 64);
            brutalGradientPanel1.ForeColor = Color.White;
            brutalGradientPanel1.GlowCenterMaxOpacity = 200;
            brutalGradientPanel1.GlowCenterMinOpacity = 50;
            brutalGradientPanel1.GlowMinSurroundOpacity = 30;
            brutalGradientPanel1.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            brutalGradientPanel1.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            brutalGradientPanel1.Location = new Point(0, 0);
            brutalGradientPanel1.Margin = new Padding(0);
            brutalGradientPanel1.MouseEvents = true;
            brutalGradientPanel1.Name = "brutalGradientPanel1";
            brutalGradientPanel1.Size = new Size(460, 84);
            brutalGradientPanel1.StartColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel1.TabIndex = 10;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(32, 32, 32);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(170, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(0, 0, 0, 2);
            panel1.Size = new Size(2, 84);
            panel1.TabIndex = 39;
            // 
            // buttonDictionary
            // 
            buttonDictionary.BackColor = Color.Transparent;
            buttonDictionary.Dock = DockStyle.Left;
            buttonDictionary.FlatAppearance.BorderSize = 0;
            buttonDictionary.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonDictionary.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonDictionary.FlatStyle = FlatStyle.Flat;
            buttonDictionary.Image = (Image)resources.GetObject("buttonDictionary.Image");
            buttonDictionary.Location = new Point(85, 0);
            buttonDictionary.Name = "buttonDictionary";
            buttonDictionary.Size = new Size(85, 84);
            buttonDictionary.TabIndex = 37;
            buttonDictionary.Text = "Help";
            buttonDictionary.TextAlign = ContentAlignment.BottomCenter;
            buttonDictionary.UseVisualStyleBackColor = false;
            buttonDictionary.Visible = false;
            // 
            // buttonAbout
            // 
            buttonAbout.BackColor = Color.Transparent;
            buttonAbout.Dock = DockStyle.Left;
            buttonAbout.FlatAppearance.BorderSize = 0;
            buttonAbout.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonAbout.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonAbout.FlatStyle = FlatStyle.Flat;
            buttonAbout.Image = (Image)resources.GetObject("buttonAbout.Image");
            buttonAbout.Location = new Point(0, 0);
            buttonAbout.Name = "buttonAbout";
            buttonAbout.Size = new Size(85, 84);
            buttonAbout.TabIndex = 0;
            buttonAbout.Text = "About";
            buttonAbout.TextAlign = ContentAlignment.BottomCenter;
            buttonAbout.UseVisualStyleBackColor = false;
            // 
            // panelBranding
            // 
            panelBranding.Controls.Add(label1);
            panelBranding.Controls.Add(lblTitle);
            panelBranding.Controls.Add(btnClose);
            panelBranding.Controls.Add(lblVersion);
            panelBranding.Controls.Add(lblCopyright);
            panelBranding.Controls.Add(lnkSite);
            panelBranding.Dock = DockStyle.Fill;
            panelBranding.EndColor = Color.FromArgb(25, 25, 25);
            panelBranding.ForeColor = Color.White;
            panelBranding.GlowCenterMaxOpacity = 200;
            panelBranding.GlowCenterMinOpacity = 50;
            panelBranding.GlowMinSurroundOpacity = 30;
            panelBranding.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            panelBranding.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            panelBranding.Location = new Point(0, 84);
            panelBranding.MouseEvents = true;
            panelBranding.Name = "panelBranding";
            panelBranding.Size = new Size(460, 260);
            panelBranding.StartColor = Color.FromArgb(16, 16, 16);
            panelBranding.TabIndex = 11;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Location = new Point(18, 102);
            label1.Name = "label1";
            label1.Size = new Size(420, 98);
            label1.TabIndex = 5;
            label1.Text = resources.GetString("label1.Text");
            // 
            // AboutForm
            // 
            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(460, 344);
            Controls.Add(panelBranding);
            Controls.Add(brutalGradientPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "About Brutal Zip";
            Load += AboutForm_Load;
            brutalGradientPanel1.ResumeLayout(false);
            panelBranding.ResumeLayout(false);
            ResumeLayout(false);

            // Events wired in code-behind
        }
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel1;
        private Panel panel1;
        private Button buttonDictionary;
        private Button buttonAbout;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel panelBranding;
        private Label label1;
    }
}