namespace Brutal_Zip.Views
{
    partial class PreviewPane
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblFileName;
        private Label lblInfo;
        private Button btnPlayPause;
        private Button btnStop;
        private Button btnOpenExternal;
        private Panel panelContent;
        private PictureBox picture;
        private RichTextBox txtPreview;
        private Label lblUnsupported;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            // Dispose current image if any
            if (picture != null && picture.Image != null)
            {
                picture.Image.Dispose();
                picture.Image = null;
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblFileName = new Label();
            lblInfo = new Label();
            btnOpenExternal = new Button();
            btnStop = new Button();
            btnPlayPause = new Button();
            panelContent = new Panel();
            picture = new PictureBox();
            txtPreview = new RichTextBox();
            lblUnsupported = new Label();
            panelContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picture).BeginInit();
            SuspendLayout();
            // 
            // lblFileName
            // 
            lblFileName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblFileName.Location = new Point(12, 10);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(560, 18);
            lblFileName.TabIndex = 0;
            lblFileName.Text = "-";
            // 
            // lblInfo
            // 
            lblInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblInfo.Location = new Point(12, 28);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(560, 18);
            lblInfo.TabIndex = 1;
            // 
            // btnOpenExternal
            // 
            btnOpenExternal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenExternal.Location = new Point(631, 29);
            btnOpenExternal.Name = "btnOpenExternal";
            btnOpenExternal.Size = new Size(80, 24);
            btnOpenExternal.TabIndex = 4;
            btnOpenExternal.Text = "Open";
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnStop.Location = new Point(717, 29);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(80, 24);
            btnStop.TabIndex = 3;
            btnStop.Text = "Stop";
            // 
            // btnPlayPause
            // 
            btnPlayPause.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPlayPause.Location = new Point(717, 3);
            btnPlayPause.Name = "btnPlayPause";
            btnPlayPause.Size = new Size(80, 24);
            btnPlayPause.TabIndex = 2;
            btnPlayPause.Text = "Play";
            // 
            // panelContent
            // 
            panelContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelContent.BackColor = Color.Black;
            panelContent.Controls.Add(picture);
            panelContent.Controls.Add(txtPreview);
            panelContent.Controls.Add(lblUnsupported);
            panelContent.Location = new Point(12, 56);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(776, 172);
            panelContent.TabIndex = 5;
            // 
            // picture
            // 
            picture.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picture.BackColor = Color.Black;
            picture.Location = new Point(0, 0);
            picture.Name = "picture";
            picture.Size = new Size(776, 172);
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            picture.TabIndex = 0;
            picture.TabStop = false;
            picture.Visible = false;
            // 
            // txtPreview
            // 
            txtPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtPreview.BorderStyle = BorderStyle.None;
            txtPreview.Font = new Font("Consolas", 10F);
            txtPreview.Location = new Point(0, 0);
            txtPreview.Name = "txtPreview";
            txtPreview.ReadOnly = true;
            txtPreview.Size = new Size(776, 172);
            txtPreview.TabIndex = 1;
            txtPreview.Text = "";
            txtPreview.Visible = false;
            // 
            // lblUnsupported
            // 
            lblUnsupported.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblUnsupported.BackColor = Color.Black;
            lblUnsupported.ForeColor = Color.White;
            lblUnsupported.Location = new Point(0, 0);
            lblUnsupported.Name = "lblUnsupported";
            lblUnsupported.Size = new Size(776, 172);
            lblUnsupported.TabIndex = 2;
            lblUnsupported.Text = "No preview available. Use Open to view.";
            lblUnsupported.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PreviewPane
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(lblFileName);
            Controls.Add(lblInfo);
            Controls.Add(btnPlayPause);
            Controls.Add(btnStop);
            Controls.Add(btnOpenExternal);
            Controls.Add(panelContent);
            Name = "PreviewPane";
            Size = new Size(800, 240);
            panelContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picture).EndInit();
            ResumeLayout(false);

            // Events wired in code-behind
        }
    }
}
