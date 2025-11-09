using System.ComponentModel;

namespace SfxStub
{
    partial class PromptForm
    {
        private IContainer components = null;

        private PictureBox picBanner;
        private Label lblDest;
        private TextBox txtFolder;
        private Button btnBrowse;
        private Button btnOK;
        private Button btnCancel;
        private Label lblCompany;

        public string Folder => txtFolder.Text;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(PromptForm));
            picBanner = new PictureBox();
            lblDest = new Label();
            txtFolder = new TextBox();
            btnBrowse = new Button();
            btnOK = new Button();
            btnCancel = new Button();
            lblCompany = new Label();
            pnlTopAccent = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            lblTitle = new Label();
            ((ISupportInitialize)picBanner).BeginInit();
            pnlTopAccent.SuspendLayout();
            SuspendLayout();
            // 
            // picBanner
            // 
            picBanner.Dock = DockStyle.Top;
            picBanner.Location = new Point(0, 25);
            picBanner.Name = "picBanner";
            picBanner.Size = new Size(640, 80);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;
            picBanner.TabIndex = 1;
            picBanner.TabStop = false;
            // 
            // lblDest
            // 
            lblDest.Location = new Point(12, 116);
            lblDest.Name = "lblDest";
            lblDest.Size = new Size(80, 20);
            lblDest.TabIndex = 3;
            lblDest.Text = "Extract to:";
            // 
            // txtFolder
            // 
            txtFolder.BackColor = Color.FromArgb(32, 32, 32);
            txtFolder.BorderStyle = BorderStyle.FixedSingle;
            txtFolder.ForeColor = Color.White;
            txtFolder.Location = new Point(12, 140);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(540, 23);
            txtFolder.TabIndex = 4;
            // 
            // btnBrowse
            // 
            btnBrowse.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnBrowse.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnBrowse.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.Location = new Point(558, 138);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(70, 26);
            btnBrowse.TabIndex = 5;
            btnBrowse.Text = "Browse…";
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnOK.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnOK.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Location = new Point(462, 178);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(80, 28);
            btnOK.TabIndex = 6;
            btnOK.Text = "Extract";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(548, 178);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 28);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancel";
            // 
            // lblCompany
            // 
            lblCompany.Location = new Point(12, 90);
            lblCompany.Name = "lblCompany";
            lblCompany.Size = new Size(616, 20);
            lblCompany.TabIndex = 2;
            // 
            // pnlTopAccent
            // 
            pnlTopAccent.Controls.Add(lblTitle);
            pnlTopAccent.Dock = DockStyle.Top;
            pnlTopAccent.EndColor = Color.Black;
            pnlTopAccent.GlowCenterMaxOpacity = 200;
            pnlTopAccent.GlowCenterMinOpacity = 50;
            pnlTopAccent.GlowMinSurroundOpacity = 30;
            pnlTopAccent.Location = new Point(0, 0);
            pnlTopAccent.MouseEvents = true;
            pnlTopAccent.Name = "pnlTopAccent";
            pnlTopAccent.Size = new Size(640, 25);
            pnlTopAccent.StartColor = Color.FromArgb(29, 181, 82);
            pnlTopAccent.TabIndex = 81;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(4, 4);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(417, 20);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Extract";
            // 
            // PromptForm
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(640, 220);
            Controls.Add(picBanner);
            Controls.Add(lblCompany);
            Controls.Add(lblDest);
            Controls.Add(txtFolder);
            Controls.Add(btnBrowse);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Controls.Add(pnlTopAccent);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PromptForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Extract To";
            Load += PromptForm_Load;
            ((ISupportInitialize)picBanner).EndInit();
            pnlTopAccent.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        private BrutalZip2025.BrutalControls.BrutalGradientPanel pnlTopAccent;
        private Label lblTitle;
    }
}