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
            components = new Container();

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
            chkShowFileList = new CheckBox();

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
            btnPreview = new Button(); // NEW

            SuspendLayout();

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(980, 720);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Build Self-Extracting Archive";

            // Source
            grpSource.Text = "Source ZIP";
            grpSource.Location = new Point(12, 12);
            grpSource.Size = new Size(956, 90);

            rdoUseCurrent.Text = "Use currently opened archive";
            rdoUseCurrent.Location = new Point(16, 24);
            rdoUseCurrent.Size = new Size(260, 20);
            rdoUseCurrent.Checked = true;

            rdoUseFile.Text = "Use ZIP file:";
            rdoUseFile.Location = new Point(16, 54);
            rdoUseFile.Size = new Size(100, 20);

            txtZipPath.Location = new Point(110, 52);
            txtZipPath.Size = new Size(750, 23);
            txtZipPath.Enabled = false;

            btnBrowseZip.Text = "Browse…";
            btnBrowseZip.Location = new Point(868, 50);
            btnBrowseZip.Size = new Size(74, 26);
            btnBrowseZip.Enabled = false;

            grpSource.Controls.Add(rdoUseCurrent);
            grpSource.Controls.Add(rdoUseFile);
            grpSource.Controls.Add(txtZipPath);
            grpSource.Controls.Add(btnBrowseZip);

            // Stub
            grpStub.Text = "SFX Stub Executable";
            grpStub.Location = new Point(12, 108);
            grpStub.Size = new Size(956, 72);

            txtStubPath.Location = new Point(16, 30);
            txtStubPath.Size = new Size(844, 23);

            btnBrowseStub.Text = "Browse…";
            btnBrowseStub.Location = new Point(868, 28);
            btnBrowseStub.Size = new Size(74, 26);

            grpStub.Controls.Add(txtStubPath);
            grpStub.Controls.Add(btnBrowseStub);

            // Branding
            grpBrand.Text = "Branding";
            grpBrand.Location = new Point(12, 184);
            grpBrand.Size = new Size(956, 232);

            lblTitle.Text = "Title:";
            lblTitle.Location = new Point(16, 28);
            lblTitle.Size = new Size(60, 20);
            txtTitle.Location = new Point(80, 26);
            txtTitle.Size = new Size(300, 23);

            lblCompany.Text = "Company:";
            lblCompany.Location = new Point(398, 28);
            lblCompany.Size = new Size(70, 20);
            txtCompany.Location = new Point(468, 26);
            txtCompany.Size = new Size(300, 23);

            chkShowFileList.Text = "Show file list in progress";
            chkShowFileList.Location = new Point(780, 27);
            chkShowFileList.Size = new Size(160, 20);
            chkShowFileList.Checked = true;

            lblDefaultDir.Text = "Default extract dir:";
            lblDefaultDir.Location = new Point(16, 58);
            lblDefaultDir.Size = new Size(120, 20);
            txtDefaultDir.Location = new Point(140, 56);
            txtDefaultDir.Size = new Size(240, 23);
            txtDefaultDir.Text = "%TEMP%\\SFX_%NAME%";

            lblMacros.AutoSize = true;
            lblMacros.Text = "Macros: %TEMP%, %DESKTOP%, %EXEDIR%, %APPDATA%, %NAME%, %TITLE%";
            lblMacros.ForeColor = Color.DimGray;
            lblMacros.Location = new Point(390, 58);

            lblIcon.Text = "Icon (.ico):";
            lblIcon.Location = new Point(16, 92);
            lblIcon.Size = new Size(80, 20);
            txtIconPath.Location = new Point(96, 90);
            txtIconPath.Size = new Size(420, 23);
            btnBrowseIcon.Text = "Browse…";
            btnBrowseIcon.Location = new Point(522, 88);
            btnBrowseIcon.Size = new Size(74, 26);
            picIcon.Location = new Point(606, 86);
            picIcon.Size = new Size(32, 32);
            picIcon.SizeMode = PictureBoxSizeMode.StretchImage;

            lblBanner.Text = "Banner (png/jpg):";
            lblBanner.Location = new Point(16, 124);
            lblBanner.Size = new Size(120, 20);
            txtBannerPath.Location = new Point(140, 122);
            txtBannerPath.Size = new Size(376, 23);
            btnBrowseBanner.Text = "Browse…";
            btnBrowseBanner.Location = new Point(522, 120);
            btnBrowseBanner.Size = new Size(74, 26);
            picBanner.Location = new Point(606, 120);
            picBanner.Size = new Size(330, 84);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;
            picBanner.BorderStyle = BorderStyle.FixedSingle;

            lblTheme.Text = "Theme color:";
            lblTheme.Location = new Point(16, 158);
            lblTheme.Size = new Size(90, 20);
            pnlThemeColor.Location = new Point(110, 156);
            pnlThemeColor.Size = new Size(40, 24);
            pnlThemeColor.BackColor = Color.DodgerBlue;
            btnPickColor.Text = "Pick…";
            btnPickColor.Location = new Point(160, 154);
            btnPickColor.Size = new Size(60, 28);

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

            // Options
            grpOptions.Text = "Options";
            grpOptions.Location = new Point(12, 420);
            grpOptions.Size = new Size(956, 116);

            chkSilent.Text = "Silent";
            chkSilent.Location = new Point(16, 24);

            chkOverwrite.Text = "Overwrite existing files";
            chkOverwrite.Location = new Point(90, 24);
            chkOverwrite.Checked = true;

            chkRequireElevation.Text = "Require elevation (UAC)";
            chkRequireElevation.Location = new Point(270, 24);

            chkShowDone.Text = "Show 'completed' dialog";
            chkShowDone.Location = new Point(450, 24);
            chkShowDone.Checked = true;

            lblRunAfter.Text = "Run after extract (relative):";
            lblRunAfter.Location = new Point(16, 54);
            txtRunAfter.Location = new Point(218, 52);
            txtRunAfter.Size = new Size(320, 23);

            lblPassword.Text = "Password (if encrypted):";
            lblPassword.Location = new Point(550, 54);
            txtPassword.Location = new Point(706, 52);
            txtPassword.Size = new Size(230, 23);
            txtPassword.UseSystemPasswordChar = true;

            grpOptions.Controls.Add(chkSilent);
            grpOptions.Controls.Add(chkOverwrite);
            grpOptions.Controls.Add(chkRequireElevation);
            grpOptions.Controls.Add(chkShowDone);
            grpOptions.Controls.Add(lblRunAfter);
            grpOptions.Controls.Add(txtRunAfter);
            grpOptions.Controls.Add(lblPassword);
            grpOptions.Controls.Add(txtPassword);

            // License
            grpLicense.Text = "License (optional)";
            grpLicense.Location = new Point(12, 540);
            grpLicense.Size = new Size(956, 120);

            txtLicense.Location = new Point(16, 24);
            txtLicense.Size = new Size(820, 80);
            txtLicense.Multiline = true;
            txtLicense.ScrollBars = ScrollBars.Vertical;

            btnLoadLicense.Text = "Load…";
            btnLoadLicense.Location = new Point(846, 24);
            btnLoadLicense.Size = new Size(84, 26);

            chkRequireAccept.Text = "Require acceptance";
            chkRequireAccept.Location = new Point(846, 56);
            chkRequireAccept.Checked = true;

            grpLicense.Controls.Add(txtLicense);
            grpLicense.Controls.Add(btnLoadLicense);
            grpLicense.Controls.Add(chkRequireAccept);

            // Buttons
            btnPreview = new Button();
            btnPreview.Text = "Preview";
            btnPreview.Location = new Point(700, 666);
            btnPreview.Size = new Size(84, 28);

            btnBuild.Text = "Build…";
            btnBuild.Location = new Point(792, 666);
            btnBuild.Size = new Size(84, 28);

            btnClose.Text = "Close";
            btnClose.Location = new Point(884, 666);
            btnClose.Size = new Size(84, 28);

            Controls.Add(grpSource);
            Controls.Add(grpStub);
            Controls.Add(grpBrand);
            Controls.Add(grpOptions);
            Controls.Add(grpLicense);
            Controls.Add(btnPreview);
            Controls.Add(btnBuild);
            Controls.Add(btnClose);

            ResumeLayout(false);
        }
    }
}