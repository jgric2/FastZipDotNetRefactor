namespace TestWinForms
{
    partial class ProgressForm
    {
        private System.ComponentModel.IContainer components = null;
        private ProgressBar progressBar1;
        private Label labelStatus;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            progressBar1 = new ProgressBar();
            labelStatus = new Label();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar1.Dock = DockStyle.Top;
            progressBar1.Location = new Point(0, 0);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(600, 24);
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 2;
            // 
            // labelStatus
            // 
            labelStatus.Dock = DockStyle.Fill;
            labelStatus.Location = new Point(0, 24);
            labelStatus.Name = "labelStatus";
            labelStatus.Padding = new Padding(8);
            labelStatus.Size = new Size(600, 78);
            labelStatus.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.Dock = DockStyle.Bottom;
            btnCancel.Location = new Point(0, 102);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(600, 30);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 132);
            Controls.Add(labelStatus);
            Controls.Add(btnCancel);
            Controls.Add(progressBar1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Progress";
            Load += ProgressForm_Load;
            ResumeLayout(false);
        }
    }
}