using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Brutal_Zip
{
    partial class BuilderPreviewForm
    {
        private IContainer components = null;

        private Panel pnlTopAccent;
        private PictureBox picBanner;
        private Label lblTitle;
        private ProgressBar progressOverall;
        private Label lblFile;
        private Label lblStat;
        private ListView lvFiles;
        private ColumnHeader colName;
        private ColumnHeader colStatus;
        private ColumnHeader colSize;
        private Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            pnlTopAccent = new Panel();
            picBanner = new PictureBox();
            lblTitle = new Label();
            progressOverall = new ProgressBar();
            lblFile = new Label();
            lblStat = new Label();
            lvFiles = new ListView();
            colName = new ColumnHeader();
            colStatus = new ColumnHeader();
            colSize = new ColumnHeader();
            btnClose = new Button();

            SuspendLayout();

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(700, 400);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "SFX Preview";

            pnlTopAccent.Location = new Point(0, 0);
            pnlTopAccent.Size = new Size(700, 6);
            pnlTopAccent.BackColor = Color.DodgerBlue;

            picBanner.Location = new Point(0, 6);
            picBanner.Size = new Size(700, 80);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;

            lblTitle.Location = new Point(12, 92);
            lblTitle.Size = new Size(676, 20);
            lblTitle.Text = "Company — Title";

            progressOverall.Location = new Point(12, 116);
            progressOverall.Size = new Size(676, 22);
            progressOverall.Style = ProgressBarStyle.Continuous;
            progressOverall.Value = 42;

            lblFile.Location = new Point(12, 144);
            lblFile.Size = new Size(676, 18);
            lblFile.Text = "current_file.txt";

            lblStat.Location = new Point(12, 164);
            lblStat.Size = new Size(676, 18);
            lblStat.Text = "Extract: 42%   123.4 MB / 512.0 MB   45.6 MB/s   8/120 files   1.23 file/s   ETA: 00:02:34   Time: 00:00:47";

            lvFiles.Location = new Point(12, 188);
            lvFiles.Size = new Size(676, 164);
            lvFiles.View = View.Details;
            lvFiles.FullRowSelect = true;
            lvFiles.Columns.AddRange(new ColumnHeader[] { colName, colStatus, colSize });

            colName.Text = "File";
            colName.Width = 430;
            colStatus.Text = "Status";
            colStatus.Width = 120;
            colSize.Text = "Size";
            colSize.Width = 100;
            colSize.TextAlign = HorizontalAlignment.Right;

            btnClose.Text = "Close";
            btnClose.Location = new Point(608, 360);
            btnClose.Size = new Size(80, 28);
            btnClose.DialogResult = DialogResult.OK;

            Controls.Add(pnlTopAccent);
            Controls.Add(picBanner);
            Controls.Add(lblTitle);
            Controls.Add(progressOverall);
            Controls.Add(lblFile);
            Controls.Add(lblStat);
            Controls.Add(lvFiles);
            Controls.Add(btnClose);

            ResumeLayout(false);
        }
    }
}