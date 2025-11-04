using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Brutal_Zip
{
    partial class BuilderPreviewForm
    {
        private IContainer components = null;
        private PictureBox picBanner;
        private Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            picBanner = new PictureBox();
            btnClose = new Button();
            pnlTopAccent = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            lblTitle = new Label();
            brutalGradientPanel1 = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            btnCancel = new Button();
            labelFilesProcessed = new Label();
            labelName = new Label();
            labelElapsed = new Label();
            labelFilesRemaining = new Label();
            labelTimeRem = new Label();
            labelPercentageBar = new Label();
            finalBarBrutal1 = new CustomProgBarSpeed.FinalBarBrutal();
            ((ISupportInitialize)picBanner).BeginInit();
            pnlTopAccent.SuspendLayout();
            brutalGradientPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // picBanner
            // 
            picBanner.Dock = DockStyle.Top;
            picBanner.Location = new Point(0, 25);
            picBanner.Name = "picBanner";
            picBanner.Size = new Size(430, 80);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;
            picBanner.TabIndex = 1;
            picBanner.TabStop = false;
            // 
            // btnClose
            // 
            btnClose.DialogResult = DialogResult.OK;
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnClose.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(314, 108);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(113, 28);
            btnClose.TabIndex = 7;
            btnClose.Text = "Toggle File List";
            btnClose.Click += btnClose_Click;
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
            pnlTopAccent.Size = new Size(430, 25);
            pnlTopAccent.StartColor = Color.FromArgb(29, 181, 82);
            pnlTopAccent.TabIndex = 77;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(4, 4);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(417, 20);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Working…";
            // 
            // brutalGradientPanel1
            // 
            brutalGradientPanel1.Controls.Add(btnCancel);
            brutalGradientPanel1.Controls.Add(labelFilesProcessed);
            brutalGradientPanel1.Controls.Add(labelName);
            brutalGradientPanel1.Controls.Add(labelElapsed);
            brutalGradientPanel1.Controls.Add(labelFilesRemaining);
            brutalGradientPanel1.Controls.Add(labelTimeRem);
            brutalGradientPanel1.EndColor = Color.FromArgb(25, 25, 25);
            brutalGradientPanel1.GlowCenterMaxOpacity = 200;
            brutalGradientPanel1.GlowCenterMinOpacity = 50;
            brutalGradientPanel1.GlowMinSurroundOpacity = 30;
            brutalGradientPanel1.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            brutalGradientPanel1.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            brutalGradientPanel1.Location = new Point(0, 248);
            brutalGradientPanel1.MouseEvents = true;
            brutalGradientPanel1.Name = "brutalGradientPanel1";
            brutalGradientPanel1.Size = new Size(430, 149);
            brutalGradientPanel1.StartColor = Color.FromArgb(16, 16, 16);
            brutalGradientPanel1.TabIndex = 80;
            // 
            // btnCancel
            // 
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(347, 117);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 28);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            // 
            // labelFilesProcessed
            // 
            labelFilesProcessed.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelFilesProcessed.BackColor = Color.Transparent;
            labelFilesProcessed.Font = new Font("Segoe UI", 9.75F);
            labelFilesProcessed.Location = new Point(22, 121);
            labelFilesProcessed.Margin = new Padding(4, 0, 4, 0);
            labelFilesProcessed.Name = "labelFilesProcessed";
            labelFilesProcessed.Size = new Size(321, 22);
            labelFilesProcessed.TabIndex = 74;
            labelFilesProcessed.Text = "Files Processed:";
            // 
            // labelName
            // 
            labelName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelName.BackColor = Color.Transparent;
            labelName.Font = new Font("Segoe UI", 9.75F);
            labelName.Location = new Point(2, 2);
            labelName.Margin = new Padding(4, 0, 4, 0);
            labelName.Name = "labelName";
            labelName.Size = new Size(427, 48);
            labelName.TabIndex = 67;
            labelName.Text = "Name:";
            // 
            // labelElapsed
            // 
            labelElapsed.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelElapsed.BackColor = Color.Transparent;
            labelElapsed.Font = new Font("Segoe UI", 9.75F);
            labelElapsed.Location = new Point(22, 78);
            labelElapsed.Margin = new Padding(4, 0, 4, 0);
            labelElapsed.Name = "labelElapsed";
            labelElapsed.Size = new Size(321, 22);
            labelElapsed.TabIndex = 72;
            labelElapsed.Text = "Time Elapsed:";
            // 
            // labelFilesRemaining
            // 
            labelFilesRemaining.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelFilesRemaining.BackColor = Color.Transparent;
            labelFilesRemaining.Font = new Font("Segoe UI", 9.75F);
            labelFilesRemaining.Location = new Point(22, 99);
            labelFilesRemaining.Margin = new Padding(4, 0, 4, 0);
            labelFilesRemaining.Name = "labelFilesRemaining";
            labelFilesRemaining.Size = new Size(321, 22);
            labelFilesRemaining.TabIndex = 73;
            labelFilesRemaining.Text = "Files Remaining:";
            // 
            // labelTimeRem
            // 
            labelTimeRem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelTimeRem.BackColor = Color.Transparent;
            labelTimeRem.Font = new Font("Segoe UI", 9.75F);
            labelTimeRem.Location = new Point(22, 57);
            labelTimeRem.Margin = new Padding(4, 0, 4, 0);
            labelTimeRem.Name = "labelTimeRem";
            labelTimeRem.Size = new Size(321, 22);
            labelTimeRem.TabIndex = 71;
            labelTimeRem.Text = "Time Remaining:";
            // 
            // labelPercentageBar
            // 
            labelPercentageBar.BackColor = Color.Transparent;
            labelPercentageBar.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelPercentageBar.ForeColor = Color.FromArgb(29, 181, 82);
            labelPercentageBar.Location = new Point(2, 106);
            labelPercentageBar.Name = "labelPercentageBar";
            labelPercentageBar.Size = new Size(130, 33);
            labelPercentageBar.TabIndex = 79;
            labelPercentageBar.Text = "0.00%";
            labelPercentageBar.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // finalBarBrutal1
            // 
            finalBarBrutal1.BackColor = Color.Transparent;
            finalBarBrutal1.CurrentSpeed = 0L;
            finalBarBrutal1.FilesPerSecond = 0L;
            finalBarBrutal1.Location = new Point(0, 142);
            finalBarBrutal1.Maximum = 100L;
            finalBarBrutal1.Name = "finalBarBrutal1";
            finalBarBrutal1.Paused = false;
            finalBarBrutal1.Progress = 0D;
            finalBarBrutal1.Size = new Size(429, 106);
            finalBarBrutal1.TabIndex = 78;
            finalBarBrutal1.Text = "finalBarBrutal1";
            finalBarBrutal1.Value = 0L;
            // 
            // BuilderPreviewForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(430, 396);
            Controls.Add(brutalGradientPanel1);
            Controls.Add(labelPercentageBar);
            Controls.Add(finalBarBrutal1);
            Controls.Add(picBanner);
            Controls.Add(btnClose);
            Controls.Add(pnlTopAccent);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BuilderPreviewForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "SFX Preview";
            ((ISupportInitialize)picBanner).EndInit();
            pnlTopAccent.ResumeLayout(false);
            brutalGradientPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }
        private BrutalZip2025.BrutalControls.BrutalGradientPanel pnlTopAccent;
        private Label lblTitle;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel1;
        private Button btnCancel;
        public Label labelFilesProcessed;
        public Label labelName;
        public Label labelElapsed;
        public Label labelFilesRemaining;
        public Label labelTimeRem;
        private Label labelPercentageBar;
        private CustomProgBarSpeed.FinalBarBrutal finalBarBrutal1;
    }
}