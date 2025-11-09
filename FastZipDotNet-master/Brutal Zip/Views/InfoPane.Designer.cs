using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Brutal_Zip.Views
{
    partial class InfoPane
    {
        private IContainer components = null;

        private TableLayoutPanel table;

        private Label lblHdrGeneral;
        private Label lblNameKey; private Label lblNameVal;
        private Label lblPathKey; private Label lblPathVal;
        private Label lblTypeKey; private Label lblTypeVal;
        private Label lblSizeKey; private Label lblSizeVal;
        private Label lblPackedKey; private Label lblPackedVal;
        private Label lblRatioKey; private Label lblRatioVal;
        private Label lblMethodKey; private Label lblMethodVal;
        private Label lblEncryptKey; private Label lblEncryptVal;
        private Label lblCrcKey; private Label lblCrcVal;
        private Label lblModifiedKey; private Label lblModifiedVal;
        private Label lblAttrsKey; private Label lblAttrsVal;

        private Button btnCopyInsidePath;

        private Label lblHdrContent;
        private Label lblDimKey; private Label lblDimVal;
        private Label lblTextKey; private Label lblTextVal;

        private Label lblHdrHashes;
        private FlowLayoutPanel pnlHashes;
        private Button btnComputeCrc;
        private Button btnComputeMd5;
        private Button btnComputeSha256;
        private Label lblCrcFile;
        private Label lblMd5File;
        private Label lblSha256File;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            table = new TableLayoutPanel();
            lblHdrGeneral = new Label();
            lblNameKey = new Label();
            lblNameVal = new Label();
            lblPathKey = new Label();
            lblPathVal = new Label();
            lblTypeKey = new Label();
            lblTypeVal = new Label();
            lblSizeKey = new Label();
            lblSizeVal = new Label();
            lblPackedKey = new Label();
            lblPackedVal = new Label();
            lblRatioKey = new Label();
            lblRatioVal = new Label();
            lblMethodKey = new Label();
            lblMethodVal = new Label();
            lblEncryptKey = new Label();
            lblEncryptVal = new Label();
            lblCrcKey = new Label();
            lblCrcVal = new Label();
            lblModifiedKey = new Label();
            lblModifiedVal = new Label();
            lblAttrsKey = new Label();
            lblAttrsVal = new Label();
            btnCopyInsidePath = new Button();
            spacer1 = new Label();
            lblHdrContent = new Label();
            lblDimKey = new Label();
            lblDimVal = new Label();
            lblTextKey = new Label();
            lblTextVal = new Label();
            spacer2 = new Label();
            lblHdrHashes = new Label();
            lblCrcFile = new Label();
            lblMd5File = new Label();
            lblSha256File = new Label();
            pnlHashes = new FlowLayoutPanel();
            panel1 = new Panel();
            btnComputeSha256 = new Button();
            btnComputeMd5 = new Button();
            btnComputeCrc = new Button();
            table.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // table
            // 
            table.AutoScroll = true;
            table.BackColor = Color.FromArgb(25, 25, 25);
            table.ColumnCount = 2;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            table.Controls.Add(lblHdrGeneral, 0, 0);
            table.Controls.Add(lblNameKey, 0, 1);
            table.Controls.Add(lblNameVal, 1, 1);
            table.Controls.Add(lblPathKey, 0, 2);
            table.Controls.Add(lblPathVal, 1, 2);
            table.Controls.Add(lblTypeKey, 0, 3);
            table.Controls.Add(lblTypeVal, 1, 3);
            table.Controls.Add(lblSizeKey, 0, 4);
            table.Controls.Add(lblSizeVal, 1, 4);
            table.Controls.Add(lblPackedKey, 0, 5);
            table.Controls.Add(lblPackedVal, 1, 5);
            table.Controls.Add(lblRatioKey, 0, 6);
            table.Controls.Add(lblRatioVal, 1, 6);
            table.Controls.Add(lblMethodKey, 0, 7);
            table.Controls.Add(lblMethodVal, 1, 7);
            table.Controls.Add(lblEncryptKey, 0, 8);
            table.Controls.Add(lblEncryptVal, 1, 8);
            table.Controls.Add(lblCrcKey, 0, 9);
            table.Controls.Add(lblCrcVal, 1, 9);
            table.Controls.Add(lblModifiedKey, 0, 10);
            table.Controls.Add(lblModifiedVal, 1, 10);
            table.Controls.Add(lblAttrsKey, 0, 11);
            table.Controls.Add(lblAttrsVal, 1, 11);
            table.Controls.Add(btnCopyInsidePath, 1, 12);
            table.Controls.Add(spacer1, 0, 13);
            table.Controls.Add(lblHdrContent, 0, 14);
            table.Controls.Add(lblDimKey, 0, 15);
            table.Controls.Add(lblDimVal, 1, 15);
            table.Controls.Add(lblTextKey, 0, 16);
            table.Controls.Add(lblTextVal, 1, 16);
            table.Controls.Add(spacer2, 0, 17);
            table.Controls.Add(lblHdrHashes, 0, 18);
            table.Controls.Add(lblCrcFile, 1, 20);
            table.Controls.Add(lblMd5File, 1, 21);
            table.Controls.Add(lblSha256File, 1, 22);
            table.Controls.Add(pnlHashes, 1, 28);
            table.Controls.Add(panel1, 1, 19);
            table.Dock = DockStyle.Fill;
            table.ForeColor = Color.White;
            table.Location = new Point(0, 0);
            table.Name = "table";
            table.Padding = new Padding(8);
            table.RowCount = 32;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 19F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 19F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 14F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 14F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F));
            table.Size = new Size(524, 684);
            table.TabIndex = 0;
            // 
            // lblHdrGeneral
            // 
            lblHdrGeneral.AutoSize = true;
            table.SetColumnSpan(lblHdrGeneral, 2);
            lblHdrGeneral.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHdrGeneral.Location = new Point(11, 8);
            lblHdrGeneral.Name = "lblHdrGeneral";
            lblHdrGeneral.Size = new Size(51, 15);
            lblHdrGeneral.TabIndex = 0;
            lblHdrGeneral.Text = "General";
            // 
            // lblNameKey
            // 
            lblNameKey.AutoSize = true;
            lblNameKey.ForeColor = Color.DarkGray;
            lblNameKey.Location = new Point(11, 28);
            lblNameKey.Name = "lblNameKey";
            lblNameKey.Size = new Size(42, 15);
            lblNameKey.TabIndex = 1;
            lblNameKey.Text = "Name:";
            // 
            // lblNameVal
            // 
            lblNameVal.AutoSize = true;
            lblNameVal.Location = new Point(131, 28);
            lblNameVal.MaximumSize = new Size(600, 0);
            lblNameVal.Name = "lblNameVal";
            lblNameVal.Size = new Size(12, 15);
            lblNameVal.TabIndex = 2;
            lblNameVal.Text = "-";
            // 
            // lblPathKey
            // 
            lblPathKey.AutoSize = true;
            lblPathKey.ForeColor = Color.DarkGray;
            lblPathKey.Location = new Point(11, 48);
            lblPathKey.Name = "lblPathKey";
            lblPathKey.Size = new Size(88, 15);
            lblPathKey.TabIndex = 3;
            lblPathKey.Text = "Path in archive:";
            // 
            // lblPathVal
            // 
            lblPathVal.AutoSize = true;
            lblPathVal.Location = new Point(131, 48);
            lblPathVal.MaximumSize = new Size(600, 0);
            lblPathVal.Name = "lblPathVal";
            lblPathVal.Size = new Size(12, 15);
            lblPathVal.TabIndex = 4;
            lblPathVal.Text = "-";
            // 
            // lblTypeKey
            // 
            lblTypeKey.AutoSize = true;
            lblTypeKey.ForeColor = Color.DarkGray;
            lblTypeKey.Location = new Point(11, 68);
            lblTypeKey.Name = "lblTypeKey";
            lblTypeKey.Size = new Size(35, 15);
            lblTypeKey.TabIndex = 5;
            lblTypeKey.Text = "Type:";
            // 
            // lblTypeVal
            // 
            lblTypeVal.AutoSize = true;
            lblTypeVal.Location = new Point(131, 68);
            lblTypeVal.Name = "lblTypeVal";
            lblTypeVal.Size = new Size(12, 15);
            lblTypeVal.TabIndex = 6;
            lblTypeVal.Text = "-";
            // 
            // lblSizeKey
            // 
            lblSizeKey.AutoSize = true;
            lblSizeKey.ForeColor = Color.DarkGray;
            lblSizeKey.Location = new Point(11, 88);
            lblSizeKey.Name = "lblSizeKey";
            lblSizeKey.Size = new Size(30, 15);
            lblSizeKey.TabIndex = 7;
            lblSizeKey.Text = "Size:";
            // 
            // lblSizeVal
            // 
            lblSizeVal.AutoSize = true;
            lblSizeVal.Location = new Point(131, 88);
            lblSizeVal.Name = "lblSizeVal";
            lblSizeVal.Size = new Size(12, 15);
            lblSizeVal.TabIndex = 8;
            lblSizeVal.Text = "-";
            // 
            // lblPackedKey
            // 
            lblPackedKey.AutoSize = true;
            lblPackedKey.ForeColor = Color.DarkGray;
            lblPackedKey.Location = new Point(11, 108);
            lblPackedKey.Name = "lblPackedKey";
            lblPackedKey.Size = new Size(48, 15);
            lblPackedKey.TabIndex = 9;
            lblPackedKey.Text = "Packed:";
            // 
            // lblPackedVal
            // 
            lblPackedVal.AutoSize = true;
            lblPackedVal.Location = new Point(131, 108);
            lblPackedVal.Name = "lblPackedVal";
            lblPackedVal.Size = new Size(12, 15);
            lblPackedVal.TabIndex = 10;
            lblPackedVal.Text = "-";
            // 
            // lblRatioKey
            // 
            lblRatioKey.AutoSize = true;
            lblRatioKey.ForeColor = Color.DarkGray;
            lblRatioKey.Location = new Point(11, 128);
            lblRatioKey.Name = "lblRatioKey";
            lblRatioKey.Size = new Size(37, 15);
            lblRatioKey.TabIndex = 11;
            lblRatioKey.Text = "Ratio:";
            // 
            // lblRatioVal
            // 
            lblRatioVal.AutoSize = true;
            lblRatioVal.Location = new Point(131, 128);
            lblRatioVal.Name = "lblRatioVal";
            lblRatioVal.Size = new Size(12, 15);
            lblRatioVal.TabIndex = 12;
            lblRatioVal.Text = "-";
            // 
            // lblMethodKey
            // 
            lblMethodKey.AutoSize = true;
            lblMethodKey.ForeColor = Color.DarkGray;
            lblMethodKey.Location = new Point(11, 148);
            lblMethodKey.Name = "lblMethodKey";
            lblMethodKey.Size = new Size(52, 15);
            lblMethodKey.TabIndex = 13;
            lblMethodKey.Text = "Method:";
            // 
            // lblMethodVal
            // 
            lblMethodVal.AutoSize = true;
            lblMethodVal.Location = new Point(131, 148);
            lblMethodVal.Name = "lblMethodVal";
            lblMethodVal.Size = new Size(12, 15);
            lblMethodVal.TabIndex = 14;
            lblMethodVal.Text = "-";
            // 
            // lblEncryptKey
            // 
            lblEncryptKey.AutoSize = true;
            lblEncryptKey.ForeColor = Color.DarkGray;
            lblEncryptKey.Location = new Point(11, 168);
            lblEncryptKey.Name = "lblEncryptKey";
            lblEncryptKey.Size = new Size(67, 15);
            lblEncryptKey.TabIndex = 15;
            lblEncryptKey.Text = "Encryption:";
            // 
            // lblEncryptVal
            // 
            lblEncryptVal.AutoSize = true;
            lblEncryptVal.Location = new Point(131, 168);
            lblEncryptVal.Name = "lblEncryptVal";
            lblEncryptVal.Size = new Size(12, 15);
            lblEncryptVal.TabIndex = 16;
            lblEncryptVal.Text = "-";
            // 
            // lblCrcKey
            // 
            lblCrcKey.AutoSize = true;
            lblCrcKey.ForeColor = Color.DarkGray;
            lblCrcKey.Location = new Point(11, 188);
            lblCrcKey.Name = "lblCrcKey";
            lblCrcKey.Size = new Size(45, 15);
            lblCrcKey.TabIndex = 17;
            lblCrcKey.Text = "CRC32:";
            // 
            // lblCrcVal
            // 
            lblCrcVal.AutoSize = true;
            lblCrcVal.Location = new Point(131, 188);
            lblCrcVal.Name = "lblCrcVal";
            lblCrcVal.Size = new Size(12, 15);
            lblCrcVal.TabIndex = 18;
            lblCrcVal.Text = "-";
            // 
            // lblModifiedKey
            // 
            lblModifiedKey.AutoSize = true;
            lblModifiedKey.ForeColor = Color.DarkGray;
            lblModifiedKey.Location = new Point(11, 208);
            lblModifiedKey.Name = "lblModifiedKey";
            lblModifiedKey.Size = new Size(58, 15);
            lblModifiedKey.TabIndex = 19;
            lblModifiedKey.Text = "Modified:";
            // 
            // lblModifiedVal
            // 
            lblModifiedVal.AutoSize = true;
            lblModifiedVal.Location = new Point(131, 208);
            lblModifiedVal.Name = "lblModifiedVal";
            lblModifiedVal.Size = new Size(12, 15);
            lblModifiedVal.TabIndex = 20;
            lblModifiedVal.Text = "-";
            // 
            // lblAttrsKey
            // 
            lblAttrsKey.AutoSize = true;
            lblAttrsKey.ForeColor = Color.DarkGray;
            lblAttrsKey.Location = new Point(11, 228);
            lblAttrsKey.Name = "lblAttrsKey";
            lblAttrsKey.Size = new Size(62, 15);
            lblAttrsKey.TabIndex = 21;
            lblAttrsKey.Text = "Attributes:";
            // 
            // lblAttrsVal
            // 
            lblAttrsVal.AutoSize = true;
            lblAttrsVal.Location = new Point(131, 228);
            lblAttrsVal.Name = "lblAttrsVal";
            lblAttrsVal.Size = new Size(12, 15);
            lblAttrsVal.TabIndex = 22;
            lblAttrsVal.Text = "-";
            // 
            // btnCopyInsidePath
            // 
            btnCopyInsidePath.AutoSize = true;
            btnCopyInsidePath.Dock = DockStyle.Left;
            btnCopyInsidePath.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCopyInsidePath.FlatAppearance.BorderSize = 0;
            btnCopyInsidePath.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnCopyInsidePath.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnCopyInsidePath.FlatStyle = FlatStyle.Flat;
            btnCopyInsidePath.Location = new Point(131, 251);
            btnCopyInsidePath.Name = "btnCopyInsidePath";
            btnCopyInsidePath.Size = new Size(108, 24);
            btnCopyInsidePath.TabIndex = 23;
            btnCopyInsidePath.Text = "Copy inside path";
            // 
            // spacer1
            // 
            spacer1.AutoSize = true;
            table.SetColumnSpan(spacer1, 2);
            spacer1.Location = new Point(11, 278);
            spacer1.Name = "spacer1";
            spacer1.Size = new Size(10, 10);
            spacer1.TabIndex = 24;
            spacer1.Text = " ";
            // 
            // lblHdrContent
            // 
            lblHdrContent.AutoSize = true;
            table.SetColumnSpan(lblHdrContent, 2);
            lblHdrContent.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHdrContent.Location = new Point(11, 288);
            lblHdrContent.Name = "lblHdrContent";
            lblHdrContent.Size = new Size(52, 15);
            lblHdrContent.TabIndex = 25;
            lblHdrContent.Text = "Content";
            // 
            // lblDimKey
            // 
            lblDimKey.AutoSize = true;
            lblDimKey.ForeColor = Color.DarkGray;
            lblDimKey.Location = new Point(11, 308);
            lblDimKey.Name = "lblDimKey";
            lblDimKey.Size = new Size(72, 15);
            lblDimKey.TabIndex = 26;
            lblDimKey.Text = "Dimensions:";
            // 
            // lblDimVal
            // 
            lblDimVal.AutoSize = true;
            lblDimVal.Location = new Point(131, 308);
            lblDimVal.Name = "lblDimVal";
            lblDimVal.Size = new Size(12, 15);
            lblDimVal.TabIndex = 27;
            lblDimVal.Text = "-";
            // 
            // lblTextKey
            // 
            lblTextKey.AutoSize = true;
            lblTextKey.ForeColor = Color.DarkGray;
            lblTextKey.Location = new Point(11, 328);
            lblTextKey.Name = "lblTextKey";
            lblTextKey.Size = new Size(31, 15);
            lblTextKey.TabIndex = 28;
            lblTextKey.Text = "Text:";
            // 
            // lblTextVal
            // 
            lblTextVal.AutoSize = true;
            lblTextVal.Location = new Point(131, 328);
            lblTextVal.Name = "lblTextVal";
            lblTextVal.Size = new Size(12, 15);
            lblTextVal.TabIndex = 29;
            lblTextVal.Text = "-";
            // 
            // spacer2
            // 
            spacer2.AutoSize = true;
            table.SetColumnSpan(spacer2, 2);
            spacer2.Location = new Point(11, 348);
            spacer2.Name = "spacer2";
            spacer2.Size = new Size(10, 15);
            spacer2.TabIndex = 30;
            spacer2.Text = " ";
            // 
            // lblHdrHashes
            // 
            lblHdrHashes.AutoSize = true;
            table.SetColumnSpan(lblHdrHashes, 2);
            lblHdrHashes.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHdrHashes.Location = new Point(11, 368);
            lblHdrHashes.Name = "lblHdrHashes";
            lblHdrHashes.Size = new Size(46, 15);
            lblHdrHashes.TabIndex = 31;
            lblHdrHashes.Text = "Hashes";
            // 
            // lblCrcFile
            // 
            lblCrcFile.AutoEllipsis = true;
            lblCrcFile.AutoSize = true;
            lblCrcFile.Location = new Point(131, 424);
            lblCrcFile.Name = "lblCrcFile";
            lblCrcFile.Size = new Size(53, 15);
            lblCrcFile.TabIndex = 33;
            lblCrcFile.Text = "CRC32: -";
            // 
            // lblMd5File
            // 
            lblMd5File.AutoEllipsis = true;
            lblMd5File.AutoSize = true;
            lblMd5File.Location = new Point(131, 442);
            lblMd5File.Name = "lblMd5File";
            lblMd5File.Size = new Size(43, 15);
            lblMd5File.TabIndex = 34;
            lblMd5File.Text = "MD5: -";
            // 
            // lblSha256File
            // 
            lblSha256File.AutoEllipsis = true;
            lblSha256File.AutoSize = true;
            lblSha256File.Location = new Point(131, 461);
            lblSha256File.Name = "lblSha256File";
            lblSha256File.Size = new Size(64, 15);
            lblSha256File.TabIndex = 35;
            lblSha256File.Text = "SHA-256: -";
            // 
            // pnlHashes
            // 
            pnlHashes.AutoSize = true;
            pnlHashes.Location = new Point(131, 571);
            pnlHashes.Name = "pnlHashes";
            pnlHashes.Size = new Size(0, 0);
            pnlHashes.TabIndex = 32;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnComputeSha256);
            panel1.Controls.Add(btnComputeMd5);
            panel1.Controls.Add(btnComputeCrc);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(131, 391);
            panel1.Name = "panel1";
            panel1.Size = new Size(382, 30);
            panel1.TabIndex = 36;
            // 
            // btnComputeSha256
            // 
            btnComputeSha256.AutoSize = true;
            btnComputeSha256.Dock = DockStyle.Left;
            btnComputeSha256.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnComputeSha256.FlatAppearance.BorderSize = 0;
            btnComputeSha256.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnComputeSha256.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnComputeSha256.FlatStyle = FlatStyle.Flat;
            btnComputeSha256.Location = new Point(150, 0);
            btnComputeSha256.Name = "btnComputeSha256";
            btnComputeSha256.Size = new Size(75, 30);
            btnComputeSha256.TabIndex = 2;
            btnComputeSha256.Text = "SHA-256";
            // 
            // btnComputeMd5
            // 
            btnComputeMd5.AutoSize = true;
            btnComputeMd5.Dock = DockStyle.Left;
            btnComputeMd5.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnComputeMd5.FlatAppearance.BorderSize = 0;
            btnComputeMd5.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnComputeMd5.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnComputeMd5.FlatStyle = FlatStyle.Flat;
            btnComputeMd5.Location = new Point(75, 0);
            btnComputeMd5.Name = "btnComputeMd5";
            btnComputeMd5.Size = new Size(75, 30);
            btnComputeMd5.TabIndex = 1;
            btnComputeMd5.Text = "MD5";
            // 
            // btnComputeCrc
            // 
            btnComputeCrc.AutoSize = true;
            btnComputeCrc.Dock = DockStyle.Left;
            btnComputeCrc.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnComputeCrc.FlatAppearance.BorderSize = 0;
            btnComputeCrc.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnComputeCrc.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnComputeCrc.FlatStyle = FlatStyle.Flat;
            btnComputeCrc.Location = new Point(0, 0);
            btnComputeCrc.Name = "btnComputeCrc";
            btnComputeCrc.Size = new Size(75, 30);
            btnComputeCrc.TabIndex = 0;
            btnComputeCrc.Text = "CRC32";
            // 
            // InfoPane
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            BackColor = SystemColors.Window;
            Controls.Add(table);
            Name = "InfoPane";
            Size = new Size(524, 684);
            table.ResumeLayout(false);
            table.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }
        private Label spacer1;
        private Label spacer2;
        private Panel panel1;
    }
}