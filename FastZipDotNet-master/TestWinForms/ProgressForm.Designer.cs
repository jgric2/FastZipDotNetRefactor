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
            this.progressBar1 = new ProgressBar();
            this.labelStatus = new Label();
            this.btnCancel = new Button();
            this.SuspendLayout();

            this.progressBar1.Dock = DockStyle.Top;
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(600, 24);
            this.progressBar1.Style = ProgressBarStyle.Continuous;

            this.labelStatus.Dock = DockStyle.Fill;
            this.labelStatus.Location = new System.Drawing.Point(0, 24);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(600, 78);
            this.labelStatus.Text = "";
            this.labelStatus.Padding = new Padding(8);

            this.btnCancel.Dock = DockStyle.Bottom;
            this.btnCancel.Location = new System.Drawing.Point(0, 102);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(600, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 132);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Progress";
            this.ResumeLayout(false);
        }
    }
}