using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace Brutal_Zip.Views
{
    partial class PreviewPane
    {
        private IContainer components = null;

        private Label lblFileName;
        private Label lblInfo;
        private Button btnPlayPause;
        private Button btnStop;
        private Button btnOpenExternal;

        private Panel panelContent;
        private PictureBox picture;
        private RichTextBox txtPreview;
        private Label lblUnsupported;

        // WebView2 for media (and could be used for HTML/markdown previews later)
        private WebView2 webView;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            if (picture != null && picture.Image != null)
            {
                picture.Image.Dispose();
                picture.Image = null;
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            // Root
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Name = "PreviewPane";
            this.Size = new Size(800, 240);

            // Top labels
            lblFileName = new Label();
            lblFileName.Text = "-";
            lblFileName.Location = new Point(12, 10);
            lblFileName.Size = new Size(560, 18);
            lblFileName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblInfo = new Label();
            lblInfo.Text = "";
            lblInfo.Location = new Point(12, 28);
            lblInfo.Size = new Size(560, 18);
            lblInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Buttons (top-right)
            btnOpenExternal = new Button();
            btnOpenExternal.Text = "Open";
            btnOpenExternal.Location = new Point(800 - 12 - 80, 8);
            btnOpenExternal.Size = new Size(80, 24);
            btnOpenExternal.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnStop = new Button();
            btnStop.Text = "Stop";
            btnStop.Location = new Point(800 - 12 - (80 * 2) - 8, 8);
            btnStop.Size = new Size(80, 24);
            btnStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnPlayPause = new Button();
            btnPlayPause.Text = "Play";
            btnPlayPause.Location = new Point(800 - 12 - (80 * 3) - 16, 8);
            btnPlayPause.Size = new Size(80, 24);
            btnPlayPause.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Content panel
            panelContent = new Panel();
            panelContent.Location = new Point(12, 56);
            panelContent.Size = new Size(776, 172);
            panelContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelContent.BackColor = Color.Black;

            // WebView2
            webView = new WebView2();
            webView.Location = new Point(0, 0);
            webView.Size = new Size(776, 172);
            webView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView.Visible = false; // shown when media mode

            // Picture
            picture = new PictureBox();
            picture.Location = new Point(0, 0);
            picture.Size = new Size(776, 172);
            picture.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            picture.Visible = false;
            picture.BackColor = Color.Black;

            // Text/code
            txtPreview = new RichTextBox();
            txtPreview.Location = new Point(0, 0);
            txtPreview.Size = new Size(776, 172);
            txtPreview.ReadOnly = true;
            txtPreview.BorderStyle = BorderStyle.None;
            txtPreview.Font = new Font("Consolas", 10f);
            txtPreview.BackColor = Color.White;
            txtPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtPreview.Visible = false;

            // Unsupported label
            lblUnsupported = new Label();
            lblUnsupported.Location = new Point(0, 0);
            lblUnsupported.Size = new Size(776, 172);
            lblUnsupported.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblUnsupported.TextAlign = ContentAlignment.MiddleCenter;
            lblUnsupported.ForeColor = Color.White;
            lblUnsupported.BackColor = Color.Black;
            lblUnsupported.Text = "No preview available. Use Open to view.";
            lblUnsupported.Visible = true;

            panelContent.Controls.Add(webView);
            panelContent.Controls.Add(picture);
            panelContent.Controls.Add(txtPreview);
            panelContent.Controls.Add(lblUnsupported);

            Controls.Add(lblFileName);
            Controls.Add(lblInfo);
            Controls.Add(btnPlayPause);
            Controls.Add(btnStop);
            Controls.Add(btnOpenExternal);
            Controls.Add(panelContent);
        }
    }
}