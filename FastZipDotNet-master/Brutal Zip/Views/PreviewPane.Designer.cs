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


        private Panel panelTop;                 // NEW
        private TextBox txtFind;                // NEW
        private Button btnFindNext;             // NEW
        private Button btnCopyPreview;          // NEW
        private Button btnSaveAs;               // NEW


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
            lblUnsupported = new Label();
            hexBox = new RichTextBox();
            webView = new WebView2();
            picture = new PictureBox();
            txtPreview = new RichTextBox();
            scintilla = new Scintilla();
            panelTop = new Panel();
            label1 = new Label();
            chkHex = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            txtFind = new TextBox();
            btnFindNext = new Button();
            btnCopyPreview = new Button();
            btnSaveAs = new Button();
            panelContent.SuspendLayout();
            ((ISupportInitialize)webView).BeginInit();
            ((ISupportInitialize)picture).BeginInit();
            panelTop.SuspendLayout();
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
            lblInfo.Location = new Point(12, 37);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(749, 18);
            lblInfo.TabIndex = 1;
            lblInfo.Click += lblInfo_Click;
            // 
            // btnOpenExternal
            // 
            btnOpenExternal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenExternal.Location = new Point(708, 8);
            btnOpenExternal.Name = "btnOpenExternal";
            btnOpenExternal.Size = new Size(80, 24);
            btnOpenExternal.TabIndex = 4;
            btnOpenExternal.Text = "Open";
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnStop.Location = new Point(803, 5);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(80, 24);
            btnStop.TabIndex = 3;
            btnStop.Text = "Stop";
            btnStop.Visible = false;
            // 
            // btnPlayPause
            // 
            btnPlayPause.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPlayPause.Location = new Point(803, 5);
            btnPlayPause.Name = "btnPlayPause";
            btnPlayPause.Size = new Size(80, 24);
            btnPlayPause.TabIndex = 2;
            btnPlayPause.Text = "Play";
            btnPlayPause.Visible = false;
            // 
            // panelContent
            // 
            panelContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelContent.BackColor = Color.Black;
            panelContent.Controls.Add(lblUnsupported);
            panelContent.Controls.Add(hexBox);
            panelContent.Controls.Add(webView);
            panelContent.Controls.Add(picture);
            panelContent.Controls.Add(txtPreview);
            panelContent.Controls.Add(scintilla);
            panelContent.Location = new Point(12, 59);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(776, 163);
            panelContent.TabIndex = 5;
            // 
            // lblUnsupported
            // 
            lblUnsupported.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblUnsupported.BackColor = Color.Black;
            lblUnsupported.ForeColor = Color.White;
            lblUnsupported.Location = new Point(0, 0);
            lblUnsupported.Name = "lblUnsupported";
            lblUnsupported.Size = new Size(776, 163);
            lblUnsupported.TabIndex = 4;
            lblUnsupported.Text = "No preview available. Use Open to view.";
            lblUnsupported.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // hexBox
            // 
            hexBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            hexBox.BackColor = Color.FromArgb(32, 32, 32);
            hexBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            hexBox.Font = new Font("Consolas", 9F);
            hexBox.ForeColor = Color.White;
            hexBox.Location = new Point(0, 0);
            hexBox.Name = "hexBox";
            hexBox.ReadOnly = true;
            hexBox.Size = new Size(776, 163);
            hexBox.TabIndex = 0;
            hexBox.Text = "";
            hexBox.Visible = false;
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = Color.White;
            webView.Location = new Point(0, 0);
            webView.Name = "webView";
            webView.Size = new Size(776, 163);
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
            picture.Size = new Size(776, 163);
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            picture.TabIndex = 1;
            picture.TabStop = false;
            picture.Visible = false;
            // 
            // txtPreview
            // 
            txtPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtPreview.BackColor = Color.FromArgb(32, 32, 32);
            txtPreview.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtPreview.Font = new Font("Consolas", 10F);
            txtPreview.ForeColor = Color.White;
            txtPreview.Location = new Point(0, 0);
            txtPreview.Name = "txtPreview";
            txtPreview.ReadOnly = true;
            txtPreview.Size = new Size(776, 163);
            txtPreview.TabIndex = 2;
            txtPreview.Text = "";
            txtPreview.Visible = false;
            // 
            // scintilla
            // 
            scintilla.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            scintilla.AutocompleteListSelectedBackColor = Color.FromArgb(0, 120, 215);
            scintilla.CaretLineBackColor = Color.Black;
            scintilla.LexerName = null;
            scintilla.Location = new Point(0, 0);
            scintilla.Name = "scintilla";
            scintilla.Size = new Size(776, 163);
            scintilla.TabIndex = 3;
            scintilla.Visible = false;
            // 
            // panelTop
            // 
            panelTop.Controls.Add(label1);
            panelTop.Controls.Add(chkHex);
            panelTop.Controls.Add(txtFind);
            panelTop.Controls.Add(btnFindNext);
            panelTop.Controls.Add(btnCopyPreview);
            panelTop.Controls.Add(btnSaveAs);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Padding = new Padding(6);
            panelTop.Size = new Size(800, 34);
            panelTop.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(422, 11);
            label1.Name = "label1";
            label1.Size = new Size(30, 15);
            label1.TabIndex = 38;
            label1.Text = "Hex";
            // 
            // chkHex
            // 
            chkHex.BackColor = Color.Transparent;
            chkHex.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkHex.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkHex.BoxGradientEnabled = true;
            chkHex.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkHex.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkHex.BoxSize = 14;
            chkHex.CheckBorderColor = Color.Lime;
            chkHex.CheckColor = Color.LawnGreen;
            chkHex.CheckGradientEnabled = true;
            chkHex.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkHex.CheckGradientStart = Color.Lime;
            chkHex.Location = new Point(402, 6);
            chkHex.Name = "chkHex";
            chkHex.Size = new Size(18, 24);
            chkHex.TabIndex = 37;
            chkHex.UseVisualStyleBackColor = false;
            // 
            // txtFind
            // 
            txtFind.BackColor = Color.FromArgb(32, 32, 32);
            txtFind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtFind.ForeColor = Color.White;
            txtFind.Location = new Point(8, 6);
            txtFind.Name = "txtFind";
            txtFind.Size = new Size(180, 23);
            txtFind.TabIndex = 0;
            // 
            // btnFindNext
            // 
            btnFindNext.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnFindNext.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnFindNext.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnFindNext.FlatStyle = FlatStyle.Flat;
            btnFindNext.Location = new Point(194, 4);
            btnFindNext.Name = "btnFindNext";
            btnFindNext.Size = new Size(50, 26);
            btnFindNext.TabIndex = 1;
            btnFindNext.Text = "Find";
            // 
            // btnCopyPreview
            // 
            btnCopyPreview.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCopyPreview.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCopyPreview.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCopyPreview.FlatStyle = FlatStyle.Flat;
            btnCopyPreview.Location = new Point(250, 4);
            btnCopyPreview.Name = "btnCopyPreview";
            btnCopyPreview.Size = new Size(60, 26);
            btnCopyPreview.TabIndex = 2;
            btnCopyPreview.Text = "Copy";
            // 
            // btnSaveAs
            // 
            btnSaveAs.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnSaveAs.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnSaveAs.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnSaveAs.FlatStyle = FlatStyle.Flat;
            btnSaveAs.Location = new Point(316, 4);
            btnSaveAs.Name = "btnSaveAs";
            btnSaveAs.Size = new Size(80, 26);
            btnSaveAs.TabIndex = 3;
            btnSaveAs.Text = "Save As…";
            // 
            // PreviewPane
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(25, 25, 25);
            Controls.Add(panelTop);
            Controls.Add(lblFileName);
            Controls.Add(lblInfo);
            Controls.Add(btnPlayPause);
            Controls.Add(btnStop);
            Controls.Add(btnOpenExternal);
            Controls.Add(panelContent);
            ForeColor = Color.White;
            Name = "PreviewPane";
            Size = new Size(800, 240);
            panelContent.ResumeLayout(false);
            ((ISupportInitialize)webView).EndInit();
            ((ISupportInitialize)picture).EndInit();
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            ResumeLayout(false);
        }
        private Label label1;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkHex;
    }
}