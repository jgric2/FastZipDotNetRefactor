using System.Windows.Forms;

namespace BrutalZip
{
    partial class ProgressForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Button btnCancel;
        private Label lblThreads;
        internal Label lblThreadsCur;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            btnCancel = new Button();
            lblThreads = new Label();
            lblThreadsCur = new Label();
            finalBarBrutal1 = new CustomProgBarSpeed.FinalBarBrutal();
            tbThreads = new BrutalZip2025.BrutalControls.BrutalTrackBar();
            labelPercentageBar = new Label();
            labelName = new Label();
            labelElapsed = new Label();
            labelTimeRem = new Label();
            labelFilesRemaining = new Label();
            labelFilesProcessed = new Label();
            brutalGradientPanel1 = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            brutalGradientPanel2 = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            ((System.ComponentModel.ISupportInitialize)tbThreads).BeginInit();
            brutalGradientPanel1.SuspendLayout();
            brutalGradientPanel2.SuspendLayout();
            SuspendLayout();
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
            // btnCancel
            // 
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(337, 131);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 28);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // lblThreads
            // 
            lblThreads.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblThreads.Location = new Point(226, 40);
            lblThreads.Name = "lblThreads";
            lblThreads.Size = new Size(60, 20);
            lblThreads.TabIndex = 0;
            lblThreads.Text = "Threads:";
            lblThreads.Visible = false;
            // 
            // lblThreadsCur
            // 
            lblThreadsCur.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblThreadsCur.Location = new Point(391, 42);
            lblThreadsCur.Name = "lblThreadsCur";
            lblThreadsCur.Size = new Size(40, 20);
            lblThreadsCur.TabIndex = 2;
            lblThreadsCur.Text = "1";
            lblThreadsCur.Visible = false;
            // 
            // finalBarBrutal1
            // 
            finalBarBrutal1.BackColor = Color.Transparent;
            finalBarBrutal1.CurrentSpeed = 0L;
            finalBarBrutal1.FilesPerSecond = 0L;
            finalBarBrutal1.Location = new Point(0, 68);
            finalBarBrutal1.Maximum = 100L;
            finalBarBrutal1.Name = "finalBarBrutal1";
            finalBarBrutal1.Paused = false;
            finalBarBrutal1.Progress = 0D;
            finalBarBrutal1.Size = new Size(429, 106);
            finalBarBrutal1.TabIndex = 5;
            finalBarBrutal1.Text = "finalBarBrutal1";
            finalBarBrutal1.Value = 0L;
            // 
            // tbThreads
            // 
            tbThreads.LeftBarColor = Color.FromArgb(29, 181, 82);
            tbThreads.Location = new Point(282, 27);
            tbThreads.Maximum = 100;
            tbThreads.Name = "tbThreads";
            tbThreads.RightBarColor = Color.FromArgb(22, 132, 99);
            tbThreads.Size = new Size(104, 45);
            tbThreads.TabIndex = 42;
            tbThreads.ThumbInnerColor = Color.FromArgb(29, 181, 82);
            tbThreads.ThumbOutlineColor = Color.FromArgb(19, 90, 42);
            tbThreads.ThumbOutlineThickness = 2;
            tbThreads.Value = 50;
            tbThreads.ValueChanged += tbThreads_ValueChanged;
            // 
            // labelPercentageBar
            // 
            labelPercentageBar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelPercentageBar.BackColor = Color.Transparent;
            labelPercentageBar.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelPercentageBar.ForeColor = Color.FromArgb(29, 181, 82);
            labelPercentageBar.Location = new Point(7, 28);
            labelPercentageBar.Name = "labelPercentageBar";
            labelPercentageBar.Size = new Size(130, 33);
            labelPercentageBar.TabIndex = 64;
            labelPercentageBar.Text = "0.00%";
            labelPercentageBar.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelName
            // 
            labelName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelName.BackColor = Color.Transparent;
            labelName.Font = new Font("Segoe UI", 9.75F);
            labelName.Location = new Point(3, 3);
            labelName.Margin = new Padding(4, 0, 4, 0);
            labelName.Name = "labelName";
            labelName.Size = new Size(426, 61);
            labelName.TabIndex = 67;
            labelName.Text = "Name:";
            // 
            // labelElapsed
            // 
            labelElapsed.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelElapsed.BackColor = Color.Transparent;
            labelElapsed.Font = new Font("Segoe UI", 9.75F);
            labelElapsed.Location = new Point(11, 93);
            labelElapsed.Margin = new Padding(4, 0, 4, 0);
            labelElapsed.Name = "labelElapsed";
            labelElapsed.Size = new Size(319, 22);
            labelElapsed.TabIndex = 72;
            labelElapsed.Text = "Time Elapsed:";
            // 
            // labelTimeRem
            // 
            labelTimeRem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelTimeRem.BackColor = Color.Transparent;
            labelTimeRem.Font = new Font("Segoe UI", 9.75F);
            labelTimeRem.Location = new Point(11, 72);
            labelTimeRem.Margin = new Padding(4, 0, 4, 0);
            labelTimeRem.Name = "labelTimeRem";
            labelTimeRem.Size = new Size(319, 22);
            labelTimeRem.TabIndex = 71;
            labelTimeRem.Text = "Time Remaining:";
            // 
            // labelFilesRemaining
            // 
            labelFilesRemaining.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelFilesRemaining.BackColor = Color.Transparent;
            labelFilesRemaining.Font = new Font("Segoe UI", 9.75F);
            labelFilesRemaining.Location = new Point(11, 114);
            labelFilesRemaining.Margin = new Padding(4, 0, 4, 0);
            labelFilesRemaining.Name = "labelFilesRemaining";
            labelFilesRemaining.Size = new Size(319, 22);
            labelFilesRemaining.TabIndex = 73;
            labelFilesRemaining.Text = "Files Remaining:";
            // 
            // labelFilesProcessed
            // 
            labelFilesProcessed.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelFilesProcessed.BackColor = Color.Transparent;
            labelFilesProcessed.Font = new Font("Segoe UI", 9.75F);
            labelFilesProcessed.Location = new Point(11, 136);
            labelFilesProcessed.Margin = new Padding(4, 0, 4, 0);
            labelFilesProcessed.Name = "labelFilesProcessed";
            labelFilesProcessed.Size = new Size(319, 22);
            labelFilesProcessed.TabIndex = 74;
            labelFilesProcessed.Text = "Files Processed:";
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
            brutalGradientPanel1.Location = new Point(0, 174);
            brutalGradientPanel1.MouseEvents = true;
            brutalGradientPanel1.Name = "brutalGradientPanel1";
            brutalGradientPanel1.Size = new Size(430, 166);
            brutalGradientPanel1.StartColor = Color.FromArgb(16, 16, 16);
            brutalGradientPanel1.TabIndex = 75;
            // 
            // brutalGradientPanel2
            // 
            brutalGradientPanel2.Controls.Add(lblTitle);
            brutalGradientPanel2.Dock = DockStyle.Top;
            brutalGradientPanel2.EndColor = Color.Black;
            brutalGradientPanel2.GlowCenterMaxOpacity = 200;
            brutalGradientPanel2.GlowCenterMinOpacity = 50;
            brutalGradientPanel2.GlowMinSurroundOpacity = 30;
            brutalGradientPanel2.Location = new Point(0, 0);
            brutalGradientPanel2.MouseEvents = true;
            brutalGradientPanel2.Name = "brutalGradientPanel2";
            brutalGradientPanel2.Size = new Size(430, 25);
            brutalGradientPanel2.StartColor = Color.FromArgb(29, 181, 82);
            brutalGradientPanel2.TabIndex = 76;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(25, 25, 25);
            ClientSize = new Size(430, 338);
            Controls.Add(brutalGradientPanel2);
            Controls.Add(brutalGradientPanel1);
            Controls.Add(labelPercentageBar);
            Controls.Add(finalBarBrutal1);
            Controls.Add(lblThreads);
            Controls.Add(lblThreadsCur);
            Controls.Add(tbThreads);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Progress";
            FormClosing += ProgressForm_FormClosing;
            Load += ProgressForm_Load;
            ((System.ComponentModel.ISupportInitialize)tbThreads).EndInit();
            brutalGradientPanel1.ResumeLayout(false);
            brutalGradientPanel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        private CustomProgBarSpeed.FinalBarBrutal finalBarBrutal1;
        private BrutalZip2025.BrutalControls.BrutalTrackBar tbThreads;
        private Label labelPercentageBar;
        public Label labelName;
        public Label labelElapsed;
        public Label labelTimeRem;
        public Label labelFilesRemaining;
        public Label labelFilesProcessed;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel1;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel2;
    }
}