using System.Windows.Forms;

namespace BrutalZip
{
    partial class ProgressForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private ProgressBar progressOverall;
        private Label lblMetrics;
        private Button btnDetails;
        private Button btnCancel;
        private Panel panelDetails;
        private TextBox txtLog;
        private Label lblThreads;
        internal TrackBar tbThreads;
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
            progressOverall = new ProgressBar();
            lblMetrics = new Label();
            btnDetails = new Button();
            btnCancel = new Button();
            panelDetails = new Panel();
            txtLog = new TextBox();
            lblThreads = new Label();
            tbThreads = new TrackBar();
            lblThreadsCur = new Label();
            panelDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbThreads).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Location = new Point(12, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(616, 20);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Working…";
            // 
            // progressOverall
            // 
            progressOverall.Location = new Point(12, 34);
            progressOverall.Name = "progressOverall";
            progressOverall.Size = new Size(616, 22);
            progressOverall.Style = ProgressBarStyle.Continuous;
            progressOverall.TabIndex = 1;
            // 
            // lblMetrics
            // 
            lblMetrics.Location = new Point(12, 62);
            lblMetrics.Name = "lblMetrics";
            lblMetrics.Size = new Size(616, 40);
            lblMetrics.TabIndex = 2;
            // 
            // btnDetails
            // 
            btnDetails.Location = new Point(12, 112);
            btnDetails.Name = "btnDetails";
            btnDetails.Size = new Size(90, 28);
            btnDetails.TabIndex = 3;
            btnDetails.Text = "Details ▸";
            btnDetails.Click += btnDetails_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(538, 112);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 28);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // panelDetails
            // 
            panelDetails.Controls.Add(txtLog);
            panelDetails.Location = new Point(12, 148);
            panelDetails.Name = "panelDetails";
            panelDetails.Size = new Size(616, 0);
            panelDetails.TabIndex = 5;
            panelDetails.Visible = false;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.Location = new Point(0, 0);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Both;
            txtLog.Size = new Size(616, 180);
            txtLog.TabIndex = 0;
            // 
            // lblThreads
            // 
            lblThreads.Location = new Point(112, 115);
            lblThreads.Name = "lblThreads";
            lblThreads.Size = new Size(60, 20);
            lblThreads.TabIndex = 0;
            lblThreads.Text = "Threads:";
            lblThreads.Visible = false;
            // 
            // tbThreads
            // 
            tbThreads.LargeChange = 1;
            tbThreads.Location = new Point(176, 108);
            tbThreads.Maximum = 4;
            tbThreads.Minimum = 1;
            tbThreads.Name = "tbThreads";
            tbThreads.Size = new Size(260, 45);
            tbThreads.TabIndex = 1;
            tbThreads.Value = 1;
            tbThreads.Visible = false;
            // 
            // lblThreadsCur
            // 
            lblThreadsCur.Location = new Point(444, 115);
            lblThreadsCur.Name = "lblThreadsCur";
            lblThreadsCur.Size = new Size(40, 20);
            lblThreadsCur.TabIndex = 2;
            lblThreadsCur.Text = "1";
            lblThreadsCur.Visible = false;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(640, 190);
            Controls.Add(lblThreads);
            Controls.Add(tbThreads);
            Controls.Add(lblThreadsCur);
            Controls.Add(lblTitle);
            Controls.Add(progressOverall);
            Controls.Add(lblMetrics);
            Controls.Add(btnDetails);
            Controls.Add(btnCancel);
            Controls.Add(panelDetails);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Progress";
            Load += ProgressForm_Load;
            panelDetails.ResumeLayout(false);
            panelDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tbThreads).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}