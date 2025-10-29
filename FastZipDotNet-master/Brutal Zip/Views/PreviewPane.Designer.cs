using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using ScintillaNET;

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

        // WebView2 for media
        private WebView2 webView;

        // Scintilla for code preview
        private Scintilla scintilla;

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
            lblFileName = new Label();
            lblInfo = new Label();
            btnOpenExternal = new Button();
            btnStop = new Button();
            btnPlayPause = new Button();
            panelContent = new Panel();
            webView = new WebView2();
            picture = new PictureBox();
            txtPreview = new RichTextBox();
            scintilla = new Scintilla();
            lblUnsupported = new Label();
            panelContent.SuspendLayout();
            ((ISupportInitialize)webView).BeginInit();
            ((ISupportInitialize)picture).BeginInit();
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
            btnOpenExternal.Location = new Point(708, 31);
            btnOpenExternal.Name = "btnOpenExternal";
            btnOpenExternal.Size = new Size(80, 24);
            btnOpenExternal.TabIndex = 4;
            btnOpenExternal.Text = "Open";
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnStop.Location = new Point(708, 5);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(80, 24);
            btnStop.TabIndex = 3;
            btnStop.Text = "Stop";
            // 
            // btnPlayPause
            // 
            btnPlayPause.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPlayPause.Location = new Point(624, 5);
            btnPlayPause.Name = "btnPlayPause";
            btnPlayPause.Size = new Size(80, 24);
            btnPlayPause.TabIndex = 2;
            btnPlayPause.Text = "Play";
            // 
            // panelContent
            // 
            panelContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelContent.BackColor = Color.Black;
            panelContent.Controls.Add(webView);
            panelContent.Controls.Add(picture);
            panelContent.Controls.Add(txtPreview);
            panelContent.Controls.Add(scintilla);
            panelContent.Controls.Add(lblUnsupported);
            panelContent.Location = new Point(12, 59);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(776, 158);
            panelContent.TabIndex = 5;
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = Color.White;
            webView.Location = new Point(0, 0);
            webView.Name = "webView";
            webView.Size = new Size(776, 158);
            webView.TabIndex = 0;
            webView.Visible = false;
            webView.ZoomFactor = 1D;
            // 
            // picture
            // 
            picture.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picture.BackColor = Color.Black;
            picture.Location = new Point(0, 0);
            picture.Name = "picture";
            picture.Size = new Size(776, 158);
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            picture.TabIndex = 1;
            picture.TabStop = false;
            picture.Visible = false;
            // 
            // txtPreview
            // 
            txtPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtPreview.BackColor = Color.White;
            txtPreview.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtPreview.Font = new Font("Consolas", 10F);
            txtPreview.Location = new Point(0, 0);
            txtPreview.Name = "txtPreview";
            txtPreview.ReadOnly = true;
            txtPreview.Size = new Size(776, 158);
            txtPreview.TabIndex = 2;
            txtPreview.Text = "";
            txtPreview.Visible = false;
            // 
            // scintilla
            // 
            scintilla.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            scintilla.AutoCMaxHeight = 9;
            scintilla.BiDirectionality = BiDirectionalDisplayType.Disabled;
            scintilla.CaretLineBackColor = Color.Black;
            scintilla.CaretLineVisible = true;
            scintilla.LexerName = null;
            scintilla.Location = new Point(0, 0);
            scintilla.Name = "scintilla";
            scintilla.ScrollWidth = 1;
            scintilla.Size = new Size(776, 158);
            scintilla.TabIndents = true;
            scintilla.TabIndex = 3;
            scintilla.UseRightToLeftReadingLayout = false;
            scintilla.Visible = false;
            scintilla.WrapMode = WrapMode.None;
            // 
            // lblUnsupported
            // 
            lblUnsupported.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblUnsupported.BackColor = Color.Black;
            lblUnsupported.ForeColor = Color.White;
            lblUnsupported.Location = new Point(0, 0);
            lblUnsupported.Name = "lblUnsupported";
            lblUnsupported.Size = new Size(776, 158);
            lblUnsupported.TabIndex = 4;
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
            ((ISupportInitialize)webView).EndInit();
            ((ISupportInitialize)picture).EndInit();
            ResumeLayout(false);
        }
    }
}