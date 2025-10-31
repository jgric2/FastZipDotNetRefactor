using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SfxStub
{
    partial class ProgressForm
    {
        private IContainer components = null;

        private PictureBox picBanner;
        private Panel pnlTopAccent;
        private Label lblTitle;
        private ProgressBar progressOverall;
        private Label lblFile;
        private Label lblStat;
        private Button btnCancel;

        // Detailed list
        private ListView lvFiles;
        private ColumnHeader colName;
        private ColumnHeader colStatus;
        private ColumnHeader colSize;

        public Label LabelTitle => lblTitle;
        public ProgressBar ProgressBar => progressOverall;
        public Label LabelFile => lblFile;
        public Label LabelStat => lblStat;
        public Button ButtonCancel => btnCancel;
        public ListView FileList => lvFiles;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            picBanner = new PictureBox();
            pnlTopAccent = new Panel();
            lblTitle = new Label();
            progressOverall = new ProgressBar();
            lblFile = new Label();
            lblStat = new Label();
            btnCancel = new Button();

            lvFiles = new ListView();
            colName = new ColumnHeader();
            colStatus = new ColumnHeader();
            colSize = new ColumnHeader();

            SuspendLayout();

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(700, 400);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Extracting…";

            pnlTopAccent.Location = new Point(0, 0);
            pnlTopAccent.Size = new Size(700, 6);
            pnlTopAccent.BackColor = Color.DodgerBlue;

            picBanner.Location = new Point(0, 6);
            picBanner.Size = new Size(700, 80);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;

            lblTitle.Location = new Point(12, 92);
            lblTitle.Size = new Size(676, 20);
            lblTitle.Text = "Extracting…";

            progressOverall.Location = new Point(12, 116);
            progressOverall.Size = new Size(676, 22);
            progressOverall.Style = ProgressBarStyle.Continuous;

            lblFile.Location = new Point(12, 144);
            lblFile.Size = new Size(676, 18);
            lblFile.Text = "";

            lblStat.Location = new Point(12, 164);
            lblStat.Size = new Size(676, 18);
            lblStat.Text = "";

            btnCancel.Location = new Point(608, 360);
            btnCancel.Size = new Size(80, 28);
            btnCancel.Text = "Cancel";

            // File list
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

            Controls.Add(pnlTopAccent);
            Controls.Add(picBanner);
            Controls.Add(lblTitle);
            Controls.Add(progressOverall);
            Controls.Add(lblFile);
            Controls.Add(lblStat);
            Controls.Add(lvFiles);
            Controls.Add(btnCancel);

            ResumeLayout(false);
        }
    }
}