using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Brutal_Zip
{
    partial class SfxBuilderForm
    {
        private IContainer components = null;

        private GroupBox grpSource;
        internal RadioButton rdoUseCurrent;
        internal RadioButton rdoUseFile;
        internal TextBox txtZipPath;
        internal Button btnBrowseZip;

        private GroupBox grpStub;
        internal TextBox txtStubPath;
        internal Button btnBrowseStub;

        private GroupBox grpBrand;
        private Label lblTitle;
        internal TextBox txtTitle;
        private Label lblCompany;
        internal TextBox txtCompany;
        private Label lblDefaultDir;
        internal TextBox txtDefaultDir;
        private Label lblMacros;

        private Label lblIcon;
        internal TextBox txtIconPath;
        internal Button btnBrowseIcon;
        internal PictureBox picIcon;

        private Label lblBanner;
        internal TextBox txtBannerPath;
        internal Button btnBrowseBanner;
        internal PictureBox picBanner;

        private Label lblTheme;
        internal Panel pnlThemeColor;
        internal Button btnPickColor;

        internal CheckBox chkShowFileList;

        private GroupBox grpOptions;
        internal CheckBox chkSilent;
        internal CheckBox chkOverwrite;
        internal CheckBox chkRequireElevation;
        internal CheckBox chkShowDone;

        private Label lblRunAfter;
        internal TextBox txtRunAfter;

        private Label lblPassword;
        internal TextBox txtPassword;

        private GroupBox grpLicense;
        internal TextBox txtLicense;
        internal Button btnLoadLicense;
        internal CheckBox chkRequireAccept;

        internal Button btnBuild;
        internal Button btnClose;
        internal Button btnPreview; // NEW

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            grpSource = new GroupBox();
            rdoUseCurrent = new RadioButton();
            rdoUseFile = new RadioButton();
            txtZipPath = new TextBox();
            btnBrowseZip = new Button();
            grpStub = new GroupBox();
            txtStubPath = new TextBox();
            btnBrowseStub = new Button();
            grpBrand = new GroupBox();
            lblTitle = new Label();
            txtTitle = new TextBox();
            lblCompany = new Label();
            txtCompany = new TextBox();
            chkShowFileList = new CheckBox();
            lblDefaultDir = new Label();
            txtDefaultDir = new TextBox();
            lblMacros = new Label();
            lblIcon = new Label();
            txtIconPath = new TextBox();
            btnBrowseIcon = new Button();
            picIcon = new PictureBox();
            lblBanner = new Label();
            txtBannerPath = new TextBox();
            btnBrowseBanner = new Button();
            picBanner = new PictureBox();
            lblTheme = new Label();
            pnlThemeColor = new Panel();
            btnPickColor = new Button();
            grpOptions = new GroupBox();
            chkSilent = new CheckBox();
            chkOverwrite = new CheckBox();
            chkRequireElevation = new CheckBox();
            chkShowDone = new CheckBox();
            lblRunAfter = new Label();
            txtRunAfter = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            grpLicense = new GroupBox();
            txtLicense = new TextBox();
            btnLoadLicense = new Button();
            chkRequireAccept = new CheckBox();
            btnBuild = new Button();
            btnClose = new Button();
            btnPreview = new Button();
            grpSource.SuspendLayout();
            grpStub.SuspendLayout();
            grpBrand.SuspendLayout();
            ((ISupportInitialize)picIcon).BeginInit();
            ((ISupportInitialize)picBanner).BeginInit();
            grpOptions.SuspendLayout();
            grpLicense.SuspendLayout();
            SuspendLayout();
            // 
            // grpSource
            // 
            grpSource.Controls.Add(rdoUseCurrent);
            grpSource.Controls.Add(rdoUseFile);
            grpSource.Controls.Add(txtZipPath);
            grpSource.Controls.Add(btnBrowseZip);
            grpSource.Location = new Point(12, 12);
            grpSource.Name = "grpSource";
            grpSource.Size = new Size(956, 90);
            grpSource.TabIndex = 0;
            grpSource.TabStop = false;
            grpSource.Text = "Source ZIP";
            // 
            // rdoUseCurrent
            // 
            rdoUseCurrent.AutoSize = true;
            rdoUseCurrent.Checked = true;
            rdoUseCurrent.Location = new Point(16, 24);
            rdoUseCurrent.Name = "rdoUseCurrent";
            rdoUseCurrent.Size = new Size(178, 19);
            rdoUseCurrent.TabIndex = 0;
            rdoUseCurrent.TabStop = true;
            rdoUseCurrent.Text = "Use currently opened archive";
            // 
            // rdoUseFile
            // 
            rdoUseFile.AutoSize = true;
            rdoUseFile.Location = new Point(16, 54);
            rdoUseFile.Name = "rdoUseFile";
            rdoUseFile.Size = new Size(86, 19);
            rdoUseFile.TabIndex = 1;
            rdoUseFile.Text = "Use ZIP file:";
            // 
            // txtZipPath
            // 
            txtZipPath.Enabled = false;
            txtZipPath.Location = new Point(110, 52);
            txtZipPath.Name = "txtZipPath";
            txtZipPath.Size = new Size(750, 23);
            txtZipPath.TabIndex = 2;
            // 
            // btnBrowseZip
            // 
            btnBrowseZip.Enabled = false;
            btnBrowseZip.Location = new Point(868, 50);
            btnBrowseZip.Name = "btnBrowseZip";
            btnBrowseZip.Size = new Size(74, 26);
            btnBrowseZip.TabIndex = 3;
            btnBrowseZip.Text = "Browse…";
            // 
            // grpStub
            // 
            grpStub.Controls.Add(txtStubPath);
            grpStub.Controls.Add(btnBrowseStub);
            grpStub.Location = new Point(12, 108);
            grpStub.Name = "grpStub";
            grpStub.Size = new Size(956, 72);
            grpStub.TabIndex = 1;
            grpStub.TabStop = false;
            grpStub.Text = "SFX Stub Executable";
            // 
            // txtStubPath
            // 
            txtStubPath.Location = new Point(16, 30);
            txtStubPath.Name = "txtStubPath";
            txtStubPath.Size = new Size(844, 23);
            txtStubPath.TabIndex = 0;
            // 
            // btnBrowseStub
            // 
            btnBrowseStub.Location = new Point(868, 28);
            btnBrowseStub.Name = "btnBrowseStub";
            btnBrowseStub.Size = new Size(74, 26);
            btnBrowseStub.TabIndex = 1;
            btnBrowseStub.Text = "Browse…";
            // 
            // grpBrand
            // 
            grpBrand.Controls.Add(lblTitle);
            grpBrand.Controls.Add(txtTitle);
            grpBrand.Controls.Add(lblCompany);
            grpBrand.Controls.Add(txtCompany);
            grpBrand.Controls.Add(chkShowFileList);
            grpBrand.Controls.Add(lblDefaultDir);
            grpBrand.Controls.Add(txtDefaultDir);
            grpBrand.Controls.Add(lblMacros);
            grpBrand.Controls.Add(lblIcon);
            grpBrand.Controls.Add(txtIconPath);
            grpBrand.Controls.Add(btnBrowseIcon);
            grpBrand.Controls.Add(picIcon);
            grpBrand.Controls.Add(lblBanner);
            grpBrand.Controls.Add(txtBannerPath);
            grpBrand.Controls.Add(btnBrowseBanner);
            grpBrand.Controls.Add(picBanner);
            grpBrand.Controls.Add(lblTheme);
            grpBrand.Controls.Add(pnlThemeColor);
            grpBrand.Controls.Add(btnPickColor);
            grpBrand.Location = new Point(12, 184);
            grpBrand.Name = "grpBrand";
            grpBrand.Size = new Size(956, 232);
            grpBrand.TabIndex = 2;
            grpBrand.TabStop = false;
            grpBrand.Text = "Branding";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(16, 28);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(33, 15);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Title:";
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(80, 26);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(300, 23);
            txtTitle.TabIndex = 1;
            // 
            // lblCompany
            // 
            lblCompany.AutoSize = true;
            lblCompany.Location = new Point(398, 28);
            lblCompany.Name = "lblCompany";
            lblCompany.Size = new Size(62, 15);
            lblCompany.TabIndex = 2;
            lblCompany.Text = "Company:";
            // 
            // txtCompany
            // 
            txtCompany.Location = new Point(468, 26);
            txtCompany.Name = "txtCompany";
            txtCompany.Size = new Size(300, 23);
            txtCompany.TabIndex = 3;
            // 
            // chkShowFileList
            // 
            chkShowFileList.Checked = true;
            chkShowFileList.CheckState = CheckState.Checked;
            chkShowFileList.Location = new Point(780, 27);
            chkShowFileList.Name = "chkShowFileList";
            chkShowFileList.Size = new Size(160, 20);
            chkShowFileList.TabIndex = 4;
            chkShowFileList.Text = "Show file list in progress";
            // 
            // lblDefaultDir
            // 
            lblDefaultDir.AutoSize = true;
            lblDefaultDir.Location = new Point(16, 58);
            lblDefaultDir.Name = "lblDefaultDir";
            lblDefaultDir.Size = new Size(103, 15);
            lblDefaultDir.TabIndex = 5;
            lblDefaultDir.Text = "Default extract dir:";
            // 
            // txtDefaultDir
            // 
            txtDefaultDir.Location = new Point(140, 56);
            txtDefaultDir.Name = "txtDefaultDir";
            txtDefaultDir.Size = new Size(240, 23);
            txtDefaultDir.TabIndex = 6;
            txtDefaultDir.Text = "%TEMP%\\SFX_%NAME%";
            // 
            // lblMacros
            // 
            lblMacros.AutoSize = true;
            lblMacros.ForeColor = Color.DimGray;
            lblMacros.Location = new Point(390, 58);
            lblMacros.Name = "lblMacros";
            lblMacros.Size = new Size(433, 15);
            lblMacros.TabIndex = 7;
            lblMacros.Text = "Macros: %TEMP%, %DESKTOP%, %EXEDIR%, %APPDATA%, %NAME%, %TITLE%";
            // 
            // lblIcon
            // 
            lblIcon.AutoSize = true;
            lblIcon.Location = new Point(16, 92);
            lblIcon.Name = "lblIcon";
            lblIcon.Size = new Size(63, 15);
            lblIcon.TabIndex = 8;
            lblIcon.Text = "Icon (.ico):";
            lblIcon.Visible = false;
            // 
            // txtIconPath
            // 
            txtIconPath.Location = new Point(96, 90);
            txtIconPath.Name = "txtIconPath";
            txtIconPath.Size = new Size(420, 23);
            txtIconPath.TabIndex = 9;
            txtIconPath.Visible = false;
            // 
            // btnBrowseIcon
            // 
            btnBrowseIcon.Location = new Point(522, 88);
            btnBrowseIcon.Name = "btnBrowseIcon";
            btnBrowseIcon.Size = new Size(74, 26);
            btnBrowseIcon.TabIndex = 10;
            btnBrowseIcon.Text = "Browse…";
            btnBrowseIcon.Visible = false;
            // 
            // picIcon
            // 
            picIcon.Location = new Point(606, 86);
            picIcon.Name = "picIcon";
            picIcon.Size = new Size(32, 32);
            picIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            picIcon.TabIndex = 11;
            picIcon.TabStop = false;
            picIcon.Visible = false;
            // 
            // lblBanner
            // 
            lblBanner.AutoSize = true;
            lblBanner.Location = new Point(16, 124);
            lblBanner.Name = "lblBanner";
            lblBanner.Size = new Size(101, 15);
            lblBanner.TabIndex = 12;
            lblBanner.Text = "Banner (png/jpg):";
            // 
            // txtBannerPath
            // 
            txtBannerPath.Location = new Point(140, 122);
            txtBannerPath.Name = "txtBannerPath";
            txtBannerPath.Size = new Size(376, 23);
            txtBannerPath.TabIndex = 13;
            // 
            // btnBrowseBanner
            // 
            btnBrowseBanner.Location = new Point(522, 120);
            btnBrowseBanner.Name = "btnBrowseBanner";
            btnBrowseBanner.Size = new Size(74, 26);
            btnBrowseBanner.TabIndex = 14;
            btnBrowseBanner.Text = "Browse…";
            // 
            // picBanner
            // 
            picBanner.BorderStyle = BorderStyle.FixedSingle;
            picBanner.Location = new Point(606, 120);
            picBanner.Name = "picBanner";
            picBanner.Size = new Size(330, 84);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;
            picBanner.TabIndex = 15;
            picBanner.TabStop = false;
            // 
            // lblTheme
            // 
            lblTheme.Location = new Point(16, 158);
            lblTheme.Name = "lblTheme";
            lblTheme.Size = new Size(90, 20);
            lblTheme.TabIndex = 16;
            lblTheme.Text = "Theme color:";
            // 
            // pnlThemeColor
            // 
            pnlThemeColor.BackColor = Color.DodgerBlue;
            pnlThemeColor.Location = new Point(110, 156);
            pnlThemeColor.Name = "pnlThemeColor";
            pnlThemeColor.Size = new Size(40, 24);
            pnlThemeColor.TabIndex = 17;
            // 
            // btnPickColor
            // 
            btnPickColor.Location = new Point(160, 154);
            btnPickColor.Name = "btnPickColor";
            btnPickColor.Size = new Size(60, 28);
            btnPickColor.TabIndex = 18;
            btnPickColor.Text = "Pick…";
            // 
            // grpOptions
            // 
            grpOptions.Controls.Add(chkSilent);
            grpOptions.Controls.Add(chkOverwrite);
            grpOptions.Controls.Add(chkRequireElevation);
            grpOptions.Controls.Add(chkShowDone);
            grpOptions.Controls.Add(lblRunAfter);
            grpOptions.Controls.Add(txtRunAfter);
            grpOptions.Controls.Add(lblPassword);
            grpOptions.Controls.Add(txtPassword);
            grpOptions.Location = new Point(12, 420);
            grpOptions.Name = "grpOptions";
            grpOptions.Size = new Size(956, 116);
            grpOptions.TabIndex = 3;
            grpOptions.TabStop = false;
            grpOptions.Text = "Options";
            // 
            // chkSilent
            // 
            chkSilent.AutoSize = true;
            chkSilent.Location = new Point(16, 24);
            chkSilent.Name = "chkSilent";
            chkSilent.Size = new Size(55, 19);
            chkSilent.TabIndex = 0;
            chkSilent.Text = "Silent";
            // 
            // chkOverwrite
            // 
            chkOverwrite.AutoSize = true;
            chkOverwrite.Checked = true;
            chkOverwrite.CheckState = CheckState.Checked;
            chkOverwrite.Location = new Point(90, 24);
            chkOverwrite.Name = "chkOverwrite";
            chkOverwrite.Size = new Size(144, 19);
            chkOverwrite.TabIndex = 1;
            chkOverwrite.Text = "Overwrite existing files";
            // 
            // chkRequireElevation
            // 
            chkRequireElevation.AutoSize = true;
            chkRequireElevation.Location = new Point(270, 24);
            chkRequireElevation.Name = "chkRequireElevation";
            chkRequireElevation.Size = new Size(152, 19);
            chkRequireElevation.TabIndex = 2;
            chkRequireElevation.Text = "Require elevation (UAC)";
            // 
            // chkShowDone
            // 
            chkShowDone.AutoSize = true;
            chkShowDone.Checked = true;
            chkShowDone.CheckState = CheckState.Checked;
            chkShowDone.Location = new Point(450, 24);
            chkShowDone.Name = "chkShowDone";
            chkShowDone.Size = new Size(157, 19);
            chkShowDone.TabIndex = 3;
            chkShowDone.Text = "Show 'completed' dialog";
            // 
            // lblRunAfter
            // 
            lblRunAfter.AutoSize = true;
            lblRunAfter.Location = new Point(16, 54);
            lblRunAfter.Name = "lblRunAfter";
            lblRunAfter.Size = new Size(145, 15);
            lblRunAfter.TabIndex = 4;
            lblRunAfter.Text = "Run after extract (relative):";
            // 
            // txtRunAfter
            // 
            txtRunAfter.Location = new Point(218, 52);
            txtRunAfter.Name = "txtRunAfter";
            txtRunAfter.Size = new Size(320, 23);
            txtRunAfter.TabIndex = 5;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(550, 54);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(134, 15);
            lblPassword.TabIndex = 6;
            lblPassword.Text = "Password (if encrypted):";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(706, 52);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(230, 23);
            txtPassword.TabIndex = 7;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // grpLicense
            // 
            grpLicense.Controls.Add(txtLicense);
            grpLicense.Controls.Add(btnLoadLicense);
            grpLicense.Controls.Add(chkRequireAccept);
            grpLicense.Location = new Point(12, 540);
            grpLicense.Name = "grpLicense";
            grpLicense.Size = new Size(956, 120);
            grpLicense.TabIndex = 4;
            grpLicense.TabStop = false;
            grpLicense.Text = "License (optional)";
            // 
            // txtLicense
            // 
            txtLicense.Location = new Point(16, 24);
            txtLicense.Multiline = true;
            txtLicense.Name = "txtLicense";
            txtLicense.ScrollBars = ScrollBars.Vertical;
            txtLicense.Size = new Size(820, 80);
            txtLicense.TabIndex = 0;
            // 
            // btnLoadLicense
            // 
            btnLoadLicense.Location = new Point(846, 24);
            btnLoadLicense.Name = "btnLoadLicense";
            btnLoadLicense.Size = new Size(84, 26);
            btnLoadLicense.TabIndex = 1;
            btnLoadLicense.Text = "Load…";
            // 
            // chkRequireAccept
            // 
            chkRequireAccept.Checked = true;
            chkRequireAccept.CheckState = CheckState.Checked;
            chkRequireAccept.Location = new Point(846, 56);
            chkRequireAccept.Name = "chkRequireAccept";
            chkRequireAccept.Size = new Size(104, 48);
            chkRequireAccept.TabIndex = 2;
            chkRequireAccept.Text = "Require acceptance";
            // 
            // btnBuild
            // 
            btnBuild.Location = new Point(792, 666);
            btnBuild.Name = "btnBuild";
            btnBuild.Size = new Size(84, 28);
            btnBuild.TabIndex = 6;
            btnBuild.Text = "Build…";
            // 
            // btnClose
            // 
            btnClose.Location = new Point(884, 666);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(84, 28);
            btnClose.TabIndex = 7;
            btnClose.Text = "Close";
            // 
            // btnPreview
            // 
            btnPreview.Location = new Point(700, 666);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new Size(84, 28);
            btnPreview.TabIndex = 5;
            btnPreview.Text = "Preview";
            // 
            // SfxBuilderForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(980, 720);
            Controls.Add(grpSource);
            Controls.Add(grpStub);
            Controls.Add(grpBrand);
            Controls.Add(grpOptions);
            Controls.Add(grpLicense);
            Controls.Add(btnPreview);
            Controls.Add(btnBuild);
            Controls.Add(btnClose);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SfxBuilderForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Build Self-Extracting Archive";
            grpSource.ResumeLayout(false);
            grpSource.PerformLayout();
            grpStub.ResumeLayout(false);
            grpStub.PerformLayout();
            grpBrand.ResumeLayout(false);
            grpBrand.PerformLayout();
            ((ISupportInitialize)picIcon).EndInit();
            ((ISupportInitialize)picBanner).EndInit();
            grpOptions.ResumeLayout(false);
            grpOptions.PerformLayout();
            grpLicense.ResumeLayout(false);
            grpLicense.PerformLayout();
            ResumeLayout(false);
        }
    }
}