using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SfxStub
{
    partial class ProgressForm
    {
        private IContainer components = null;

        private PictureBox picBanner;
        private Button btnCancel;

        // Detailed list
        private ListView lvFiles;
        private ColumnHeader colName;
        private ColumnHeader colStatus;
        private ColumnHeader colSize;

        public Label LabelTitle => lblTitle;
       // public ProgressBar ProgressBar => progressOverall;
        public Label LabelFile => lblFile;
      //  public Label LabelStat => lblStat;
        public Button ButtonCancel => btnCancel;
        public ListView FileList => lvFiles;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            picBanner = new PictureBox();
            btnCancel = new Button();
            lvFiles = new ListView();
            colName = new ColumnHeader();
            colStatus = new ColumnHeader();
            colSize = new ColumnHeader();
            finalBarBrutal1 = new CustomProgBarSpeed.FinalBarBrutal();
            panelDetails = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            labelFilesProcessed = new Label();
            labelElapsed = new Label();
            labelFilesRemaining = new Label();
            labelTimeRem = new Label();
            lblFile = new Label();
            button1 = new Button();
            labelPercentageBar = new Label();
            pnlTopAccent = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            lblTitle = new Label();
            ((ISupportInitialize)picBanner).BeginInit();
            panelDetails.SuspendLayout();
            pnlTopAccent.SuspendLayout();
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
            // btnCancel
            // 
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(347, 117);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 28);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // lvFiles
            // 
            lvFiles.BackColor = Color.FromArgb(32, 32, 32);
            lvFiles.BorderStyle = BorderStyle.None;
            lvFiles.Columns.AddRange(new ColumnHeader[] { colName, colStatus, colSize });
            lvFiles.ForeColor = Color.White;
            lvFiles.FullRowSelect = true;
            lvFiles.Location = new Point(0, 248);
            lvFiles.Name = "lvFiles";
            lvFiles.Size = new Size(430, 149);
            lvFiles.TabIndex = 6;
            lvFiles.UseCompatibleStateImageBehavior = false;
            lvFiles.View = View.Details;
            lvFiles.Visible = false;
            // 
            // colName
            // 
            colName.Text = "File";
            colName.Width = 220;
            // 
            // colStatus
            // 
            colStatus.Text = "Status";
            colStatus.Width = 120;
            // 
            // colSize
            // 
            colSize.Text = "Size";
            colSize.TextAlign = HorizontalAlignment.Right;
            colSize.Width = 100;
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
            finalBarBrutal1.TabIndex = 8;
            finalBarBrutal1.Text = "finalBarBrutal1";
            finalBarBrutal1.Value = 0L;
            // 
            // panelDetails
            // 
            panelDetails.Controls.Add(labelFilesProcessed);
            panelDetails.Controls.Add(labelElapsed);
            panelDetails.Controls.Add(labelFilesRemaining);
            panelDetails.Controls.Add(labelTimeRem);
            panelDetails.Controls.Add(lblFile);
            panelDetails.Controls.Add(btnCancel);
            panelDetails.EndColor = Color.FromArgb(25, 25, 25);
            panelDetails.GlowCenterMaxOpacity = 200;
            panelDetails.GlowCenterMinOpacity = 50;
            panelDetails.GlowMinSurroundOpacity = 30;
            panelDetails.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            panelDetails.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            panelDetails.Location = new Point(0, 248);
            panelDetails.MouseEvents = true;
            panelDetails.Name = "panelDetails";
            panelDetails.Size = new Size(430, 149);
            panelDetails.StartColor = Color.FromArgb(12, 12, 12);
            panelDetails.TabIndex = 9;
            // 
            // labelFilesProcessed
            // 
            labelFilesProcessed.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelFilesProcessed.BackColor = Color.Transparent;
            labelFilesProcessed.Font = new Font("Segoe UI", 9.75F);
            labelFilesProcessed.Location = new Point(22, 121);
            labelFilesProcessed.Margin = new Padding(4, 0, 4, 0);
            labelFilesProcessed.Name = "labelFilesProcessed";
            labelFilesProcessed.Size = new Size(320, 22);
            labelFilesProcessed.TabIndex = 78;
            labelFilesProcessed.Text = "Files Processed:";
            // 
            // labelElapsed
            // 
            labelElapsed.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelElapsed.BackColor = Color.Transparent;
            labelElapsed.Font = new Font("Segoe UI", 9.75F);
            labelElapsed.Location = new Point(22, 78);
            labelElapsed.Margin = new Padding(4, 0, 4, 0);
            labelElapsed.Name = "labelElapsed";
            labelElapsed.Size = new Size(320, 22);
            labelElapsed.TabIndex = 76;
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
            labelFilesRemaining.Size = new Size(320, 22);
            labelFilesRemaining.TabIndex = 77;
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
            labelTimeRem.Size = new Size(320, 22);
            labelTimeRem.TabIndex = 75;
            labelTimeRem.Text = "Time Remaining:";
            // 
            // lblFile
            // 
            lblFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblFile.BackColor = Color.Transparent;
            lblFile.Font = new Font("Segoe UI", 9.75F);
            lblFile.Location = new Point(2, 2);
            lblFile.Margin = new Padding(4, 0, 4, 0);
            lblFile.Name = "lblFile";
            lblFile.Size = new Size(427, 48);
            lblFile.TabIndex = 68;
            lblFile.Text = "Name:";
            // 
            // button1
            // 
            button1.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(314, 108);
            button1.Name = "button1";
            button1.Size = new Size(113, 28);
            button1.TabIndex = 79;
            button1.Text = "Toggle File List";
            button1.Click += button1_Click;
            // 
            // labelPercentageBar
            // 
            labelPercentageBar.BackColor = Color.Transparent;
            labelPercentageBar.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelPercentageBar.ForeColor = Color.FromArgb(29, 181, 82);
            labelPercentageBar.Location = new Point(2, 106);
            labelPercentageBar.Name = "labelPercentageBar";
            labelPercentageBar.Size = new Size(130, 33);
            labelPercentageBar.TabIndex = 65;
            labelPercentageBar.Text = "0.00%";
            labelPercentageBar.TextAlign = ContentAlignment.MiddleCenter;
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
            pnlTopAccent.TabIndex = 80;
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
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(430, 396);
            Controls.Add(button1);
            Controls.Add(labelPercentageBar);
            Controls.Add(picBanner);
            Controls.Add(finalBarBrutal1);
            Controls.Add(panelDetails);
            Controls.Add(lvFiles);
            Controls.Add(pnlTopAccent);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Extracting…";
            Load += ProgressForm_Load;
            ((ISupportInitialize)picBanner).EndInit();
            panelDetails.ResumeLayout(false);
            pnlTopAccent.ResumeLayout(false);
            ResumeLayout(false);
        }
        private CustomProgBarSpeed.FinalBarBrutal finalBarBrutal1;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel panelDetails;
        public Label lblFile;
        public Label labelFilesProcessed;
        public Label labelElapsed;
        public Label labelFilesRemaining;
        public Label labelTimeRem;
        private Button button1;
        private Label labelPercentageBar;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel pnlTopAccent;
        private Label lblTitle;
    }
}