using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Brutal_Zip
{
    partial class SfxBuilderForm
    {
        private IContainer components = null;
        internal RadioButton rdoUseCurrent;
        internal RadioButton rdoUseFile;
        internal TextBox txtZipPath;
        internal Button btnBrowseZip;

        private GroupBox grpStub;
        internal TextBox txtStubPath;
        internal Button btnBrowseStub;
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
       // internal CheckBox chkOverwrite;

        private Label lblRunAfter;
        internal TextBox txtRunAfter;

        private Label lblPassword;
        internal TextBox txtPassword;
        internal TextBox txtLicense;
        internal Button btnLoadLicense;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            rdoUseCurrent = new RadioButton();
            rdoUseFile = new RadioButton();
            txtZipPath = new TextBox();
            btnBrowseZip = new Button();
            grpStub = new GroupBox();
            txtStubPath = new TextBox();
            btnBrowseStub = new Button();
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
            lblRunAfter = new Label();
            txtRunAfter = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            txtLicense = new TextBox();
            btnLoadLicense = new Button();
            brutalGradientPanel1 = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            panel1 = new Panel();
            buttonBuild = new Button();
            buttonPreview = new Button();
            panel6 = new Panel();
            buttonLicence = new Button();
            buttonBranding = new Button();
            buttonMain = new Button();
            panelMain = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            label6 = new Label();
            chkShowDone = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label5 = new Label();
            chkRequireElevation = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label4 = new Label();
            chkOverwrite = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label3 = new Label();
            chkSilent = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label1 = new Label();
            panelBranding = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            gradientPanelPreview = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            pnlThemeColorEnd = new Panel();
            button1 = new Button();
            label2 = new Label();
            panelLicence = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            label7 = new Label();
            chkRequireAccept = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            grpStub.SuspendLayout();
            ((ISupportInitialize)picIcon).BeginInit();
            ((ISupportInitialize)picBanner).BeginInit();
            brutalGradientPanel1.SuspendLayout();
            panelMain.SuspendLayout();
            panelBranding.SuspendLayout();
            panelLicence.SuspendLayout();
            SuspendLayout();
            // 
            // rdoUseCurrent
            // 
            rdoUseCurrent.AutoSize = true;
            rdoUseCurrent.Checked = true;
            rdoUseCurrent.Location = new Point(12, 16);
            rdoUseCurrent.Name = "rdoUseCurrent";
            rdoUseCurrent.Size = new Size(178, 19);
            rdoUseCurrent.TabIndex = 0;
            rdoUseCurrent.TabStop = true;
            rdoUseCurrent.Text = "Use currently opened archive";
            rdoUseCurrent.CheckedChanged += rdoUseCurrent_CheckedChanged;
            // 
            // rdoUseFile
            // 
            rdoUseFile.AutoSize = true;
            rdoUseFile.Location = new Point(12, 41);
            rdoUseFile.Name = "rdoUseFile";
            rdoUseFile.Size = new Size(86, 19);
            rdoUseFile.TabIndex = 1;
            rdoUseFile.Text = "Use ZIP file:";
            rdoUseFile.CheckedChanged += rdoUseFile_CheckedChanged;
            // 
            // txtZipPath
            // 
            txtZipPath.BackColor = Color.FromArgb(32, 32, 32);
            txtZipPath.BorderStyle = BorderStyle.FixedSingle;
            txtZipPath.Enabled = false;
            txtZipPath.ForeColor = Color.White;
            txtZipPath.Location = new Point(104, 41);
            txtZipPath.Name = "txtZipPath";
            txtZipPath.Size = new Size(262, 23);
            txtZipPath.TabIndex = 2;
            // 
            // btnBrowseZip
            // 
            btnBrowseZip.Enabled = false;
            btnBrowseZip.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnBrowseZip.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnBrowseZip.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnBrowseZip.FlatStyle = FlatStyle.Flat;
            btnBrowseZip.Location = new Point(372, 41);
            btnBrowseZip.Name = "btnBrowseZip";
            btnBrowseZip.Size = new Size(74, 26);
            btnBrowseZip.TabIndex = 3;
            btnBrowseZip.Text = "Browse…";
            btnBrowseZip.Click += btnBrowseZip_Click;
            // 
            // grpStub
            // 
            grpStub.Controls.Add(txtStubPath);
            grpStub.Controls.Add(btnBrowseStub);
            grpStub.ForeColor = Color.White;
            grpStub.Location = new Point(85, 283);
            grpStub.Name = "grpStub";
            grpStub.Size = new Size(318, 72);
            grpStub.TabIndex = 1;
            grpStub.TabStop = false;
            grpStub.Text = "SFX Stub Executable";
            grpStub.Visible = false;
            // 
            // txtStubPath
            // 
            txtStubPath.Location = new Point(16, 30);
            txtStubPath.Name = "txtStubPath";
            txtStubPath.Size = new Size(150, 23);
            txtStubPath.TabIndex = 0;
            // 
            // btnBrowseStub
            // 
            btnBrowseStub.Location = new Point(186, 27);
            btnBrowseStub.Name = "btnBrowseStub";
            btnBrowseStub.Size = new Size(74, 26);
            btnBrowseStub.TabIndex = 1;
            btnBrowseStub.Text = "Browse…";
            btnBrowseStub.Click += btnBrowseStub_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(14, 30);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(33, 15);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Title:";
            // 
            // txtTitle
            // 
            txtTitle.BackColor = Color.FromArgb(32, 32, 32);
            txtTitle.BorderStyle = BorderStyle.FixedSingle;
            txtTitle.ForeColor = Color.White;
            txtTitle.Location = new Point(123, 28);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(255, 23);
            txtTitle.TabIndex = 1;
            // 
            // lblCompany
            // 
            lblCompany.AutoSize = true;
            lblCompany.Location = new Point(14, 60);
            lblCompany.Name = "lblCompany";
            lblCompany.Size = new Size(62, 15);
            lblCompany.TabIndex = 2;
            lblCompany.Text = "Company:";
            // 
            // txtCompany
            // 
            txtCompany.BackColor = Color.FromArgb(32, 32, 32);
            txtCompany.BorderStyle = BorderStyle.FixedSingle;
            txtCompany.ForeColor = Color.White;
            txtCompany.Location = new Point(123, 58);
            txtCompany.Name = "txtCompany";
            txtCompany.Size = new Size(255, 23);
            txtCompany.TabIndex = 3;
            // 
            // lblDefaultDir
            // 
            lblDefaultDir.AutoSize = true;
            lblDefaultDir.Location = new Point(14, 106);
            lblDefaultDir.Name = "lblDefaultDir";
            lblDefaultDir.Size = new Size(103, 15);
            lblDefaultDir.TabIndex = 5;
            lblDefaultDir.Text = "Default extract dir:";
            // 
            // txtDefaultDir
            // 
            txtDefaultDir.BackColor = Color.FromArgb(32, 32, 32);
            txtDefaultDir.BorderStyle = BorderStyle.FixedSingle;
            txtDefaultDir.ForeColor = Color.White;
            txtDefaultDir.Location = new Point(123, 103);
            txtDefaultDir.Name = "txtDefaultDir";
            txtDefaultDir.Size = new Size(255, 23);
            txtDefaultDir.TabIndex = 6;
            txtDefaultDir.Text = "%TEMP%\\SFX_%NAME%";
            // 
            // lblMacros
            // 
            lblMacros.AutoSize = true;
            lblMacros.ForeColor = Color.DimGray;
            lblMacros.Location = new Point(14, 130);
            lblMacros.Name = "lblMacros";
            lblMacros.Size = new Size(433, 15);
            lblMacros.TabIndex = 7;
            lblMacros.Text = "Macros: %TEMP%, %DESKTOP%, %EXEDIR%, %APPDATA%, %NAME%, %TITLE%";
            // 
            // lblIcon
            // 
            lblIcon.AutoSize = true;
            lblIcon.Location = new Point(14, 157);
            lblIcon.Name = "lblIcon";
            lblIcon.Size = new Size(63, 15);
            lblIcon.TabIndex = 8;
            lblIcon.Text = "Icon (.ico):";
            lblIcon.Visible = false;
            // 
            // txtIconPath
            // 
            txtIconPath.BackColor = Color.FromArgb(32, 32, 32);
            txtIconPath.BorderStyle = BorderStyle.FixedSingle;
            txtIconPath.ForeColor = Color.White;
            txtIconPath.Location = new Point(123, 154);
            txtIconPath.Name = "txtIconPath";
            txtIconPath.Size = new Size(255, 23);
            txtIconPath.TabIndex = 9;
            txtIconPath.Visible = false;
            // 
            // btnBrowseIcon
            // 
            btnBrowseIcon.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnBrowseIcon.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnBrowseIcon.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnBrowseIcon.FlatStyle = FlatStyle.Flat;
            btnBrowseIcon.Location = new Point(382, 153);
            btnBrowseIcon.Name = "btnBrowseIcon";
            btnBrowseIcon.Size = new Size(74, 26);
            btnBrowseIcon.TabIndex = 10;
            btnBrowseIcon.Text = "Browse…";
            btnBrowseIcon.Visible = false;
            btnBrowseIcon.Click += btnBrowseIcon_Click;
            // 
            // picIcon
            // 
            picIcon.Location = new Point(87, 150);
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
            lblBanner.Location = new Point(14, 184);
            lblBanner.Name = "lblBanner";
            lblBanner.Size = new Size(101, 15);
            lblBanner.TabIndex = 12;
            lblBanner.Text = "Banner (png/jpg):";
            // 
            // txtBannerPath
            // 
            txtBannerPath.BackColor = Color.FromArgb(32, 32, 32);
            txtBannerPath.BorderStyle = BorderStyle.FixedSingle;
            txtBannerPath.ForeColor = Color.White;
            txtBannerPath.Location = new Point(123, 184);
            txtBannerPath.Name = "txtBannerPath";
            txtBannerPath.Size = new Size(255, 23);
            txtBannerPath.TabIndex = 13;
            // 
            // btnBrowseBanner
            // 
            btnBrowseBanner.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnBrowseBanner.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnBrowseBanner.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnBrowseBanner.FlatStyle = FlatStyle.Flat;
            btnBrowseBanner.Location = new Point(382, 184);
            btnBrowseBanner.Name = "btnBrowseBanner";
            btnBrowseBanner.Size = new Size(74, 26);
            btnBrowseBanner.TabIndex = 14;
            btnBrowseBanner.Text = "Browse…";
            btnBrowseBanner.Click += btnBrowseBanner_Click;
            // 
            // picBanner
            // 
            picBanner.BorderStyle = BorderStyle.FixedSingle;
            picBanner.Location = new Point(123, 211);
            picBanner.Name = "picBanner";
            picBanner.Size = new Size(330, 84);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;
            picBanner.TabIndex = 15;
            picBanner.TabStop = false;
            // 
            // lblTheme
            // 
            lblTheme.Location = new Point(14, 305);
            lblTheme.Name = "lblTheme";
            lblTheme.Size = new Size(90, 20);
            lblTheme.TabIndex = 16;
            lblTheme.Text = "Theme color:";
            // 
            // pnlThemeColor
            // 
            pnlThemeColor.BackColor = Color.FromArgb(29, 181, 82);
            pnlThemeColor.Location = new Point(123, 301);
            pnlThemeColor.Name = "pnlThemeColor";
            pnlThemeColor.Size = new Size(13, 24);
            pnlThemeColor.TabIndex = 17;
            // 
            // btnPickColor
            // 
            btnPickColor.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnPickColor.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnPickColor.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnPickColor.FlatStyle = FlatStyle.Flat;
            btnPickColor.Location = new Point(140, 300);
            btnPickColor.Name = "btnPickColor";
            btnPickColor.Size = new Size(60, 28);
            btnPickColor.TabIndex = 18;
            btnPickColor.Text = "Pick…";
            btnPickColor.Click += btnPickColor_Click;
            // 
            // lblRunAfter
            // 
            lblRunAfter.AutoSize = true;
            lblRunAfter.Location = new Point(12, 211);
            lblRunAfter.Name = "lblRunAfter";
            lblRunAfter.Size = new Size(145, 15);
            lblRunAfter.TabIndex = 4;
            lblRunAfter.Text = "Run after extract (relative):";
            // 
            // txtRunAfter
            // 
            txtRunAfter.BackColor = Color.FromArgb(32, 32, 32);
            txtRunAfter.BorderStyle = BorderStyle.FixedSingle;
            txtRunAfter.ForeColor = Color.White;
            txtRunAfter.Location = new Point(189, 209);
            txtRunAfter.Name = "txtRunAfter";
            txtRunAfter.Size = new Size(247, 23);
            txtRunAfter.TabIndex = 5;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(12, 239);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(134, 15);
            lblPassword.TabIndex = 6;
            lblPassword.Text = "Password (if encrypted):";
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.FromArgb(32, 32, 32);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.ForeColor = Color.White;
            txtPassword.Location = new Point(189, 238);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(247, 23);
            txtPassword.TabIndex = 7;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtLicense
            // 
            txtLicense.BackColor = Color.FromArgb(32, 32, 32);
            txtLicense.BorderStyle = BorderStyle.FixedSingle;
            txtLicense.ForeColor = Color.White;
            txtLicense.Location = new Point(10, 11);
            txtLicense.Multiline = true;
            txtLicense.Name = "txtLicense";
            txtLicense.ScrollBars = ScrollBars.Vertical;
            txtLicense.Size = new Size(444, 278);
            txtLicense.TabIndex = 0;
            // 
            // btnLoadLicense
            // 
            btnLoadLicense.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnLoadLicense.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnLoadLicense.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnLoadLicense.FlatStyle = FlatStyle.Flat;
            btnLoadLicense.Location = new Point(372, 295);
            btnLoadLicense.Name = "btnLoadLicense";
            btnLoadLicense.Size = new Size(84, 26);
            btnLoadLicense.TabIndex = 1;
            btnLoadLicense.Text = "Load…";
            btnLoadLicense.Click += btnLoadLicense_Click;
            // 
            // brutalGradientPanel1
            // 
            brutalGradientPanel1.BackColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel1.Controls.Add(panel1);
            brutalGradientPanel1.Controls.Add(buttonBuild);
            brutalGradientPanel1.Controls.Add(buttonPreview);
            brutalGradientPanel1.Controls.Add(panel6);
            brutalGradientPanel1.Controls.Add(buttonLicence);
            brutalGradientPanel1.Controls.Add(buttonBranding);
            brutalGradientPanel1.Controls.Add(buttonMain);
            brutalGradientPanel1.Dock = DockStyle.Top;
            brutalGradientPanel1.EndColor = Color.FromArgb(64, 64, 64);
            brutalGradientPanel1.ForeColor = Color.White;
            brutalGradientPanel1.GlowCenterMaxOpacity = 200;
            brutalGradientPanel1.GlowCenterMinOpacity = 50;
            brutalGradientPanel1.GlowMinSurroundOpacity = 30;
            brutalGradientPanel1.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            brutalGradientPanel1.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            brutalGradientPanel1.Location = new Point(0, 0);
            brutalGradientPanel1.Margin = new Padding(0);
            brutalGradientPanel1.MouseEvents = true;
            brutalGradientPanel1.Name = "brutalGradientPanel1";
            brutalGradientPanel1.Size = new Size(462, 84);
            brutalGradientPanel1.StartColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel1.TabIndex = 8;
            brutalGradientPanel1.Paint += brutalGradientPanel1_Paint;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(32, 32, 32);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(427, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(0, 0, 0, 2);
            panel1.Size = new Size(2, 84);
            panel1.TabIndex = 36;
            // 
            // buttonBuild
            // 
            buttonBuild.BackColor = Color.Transparent;
            buttonBuild.Dock = DockStyle.Left;
            buttonBuild.FlatAppearance.BorderSize = 0;
            buttonBuild.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonBuild.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonBuild.FlatStyle = FlatStyle.Flat;
            buttonBuild.Location = new Point(342, 0);
            buttonBuild.Name = "buttonBuild";
            buttonBuild.Size = new Size(85, 84);
            buttonBuild.TabIndex = 35;
            buttonBuild.Text = "Build";
            buttonBuild.TextAlign = ContentAlignment.BottomCenter;
            buttonBuild.UseVisualStyleBackColor = false;
            buttonBuild.Click += buttonBuild_Click;
            // 
            // buttonPreview
            // 
            buttonPreview.BackColor = Color.Transparent;
            buttonPreview.Dock = DockStyle.Left;
            buttonPreview.FlatAppearance.BorderSize = 0;
            buttonPreview.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonPreview.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonPreview.FlatStyle = FlatStyle.Flat;
            buttonPreview.Location = new Point(257, 0);
            buttonPreview.Name = "buttonPreview";
            buttonPreview.Size = new Size(85, 84);
            buttonPreview.TabIndex = 34;
            buttonPreview.Text = "Preview";
            buttonPreview.TextAlign = ContentAlignment.BottomCenter;
            buttonPreview.UseVisualStyleBackColor = false;
            buttonPreview.Click += buttonPreview_Click;
            // 
            // panel6
            // 
            panel6.BackColor = Color.FromArgb(32, 32, 32);
            panel6.Dock = DockStyle.Left;
            panel6.Location = new Point(255, 0);
            panel6.Name = "panel6";
            panel6.Padding = new Padding(0, 0, 0, 2);
            panel6.Size = new Size(2, 84);
            panel6.TabIndex = 33;
            // 
            // buttonLicence
            // 
            buttonLicence.BackColor = Color.Transparent;
            buttonLicence.Dock = DockStyle.Left;
            buttonLicence.FlatAppearance.BorderSize = 0;
            buttonLicence.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonLicence.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonLicence.FlatStyle = FlatStyle.Flat;
            buttonLicence.Location = new Point(170, 0);
            buttonLicence.Name = "buttonLicence";
            buttonLicence.Size = new Size(85, 84);
            buttonLicence.TabIndex = 38;
            buttonLicence.Text = "Licence";
            buttonLicence.TextAlign = ContentAlignment.BottomCenter;
            buttonLicence.UseVisualStyleBackColor = false;
            buttonLicence.Click += buttonLicence_Click;
            // 
            // buttonBranding
            // 
            buttonBranding.BackColor = Color.Transparent;
            buttonBranding.Dock = DockStyle.Left;
            buttonBranding.FlatAppearance.BorderSize = 0;
            buttonBranding.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonBranding.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonBranding.FlatStyle = FlatStyle.Flat;
            buttonBranding.Location = new Point(85, 0);
            buttonBranding.Name = "buttonBranding";
            buttonBranding.Size = new Size(85, 84);
            buttonBranding.TabIndex = 37;
            buttonBranding.Text = "Branding";
            buttonBranding.TextAlign = ContentAlignment.BottomCenter;
            buttonBranding.UseVisualStyleBackColor = false;
            buttonBranding.Click += buttonBranding_Click;
            // 
            // buttonMain
            // 
            buttonMain.BackColor = Color.Transparent;
            buttonMain.Dock = DockStyle.Left;
            buttonMain.FlatAppearance.BorderSize = 0;
            buttonMain.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonMain.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonMain.FlatStyle = FlatStyle.Flat;
            buttonMain.Location = new Point(0, 0);
            buttonMain.Name = "buttonMain";
            buttonMain.Size = new Size(85, 84);
            buttonMain.TabIndex = 0;
            buttonMain.Text = "Main";
            buttonMain.TextAlign = ContentAlignment.BottomCenter;
            buttonMain.UseVisualStyleBackColor = false;
            buttonMain.Click += buttonMain_Click;
            // 
            // panelMain
            // 
            panelMain.Controls.Add(label6);
            panelMain.Controls.Add(chkShowDone);
            panelMain.Controls.Add(label5);
            panelMain.Controls.Add(chkRequireElevation);
            panelMain.Controls.Add(label4);
            panelMain.Controls.Add(chkOverwrite);
            panelMain.Controls.Add(label3);
            panelMain.Controls.Add(chkSilent);
            panelMain.Controls.Add(grpStub);
            panelMain.Controls.Add(label1);
            panelMain.Controls.Add(btnBrowseZip);
            panelMain.Controls.Add(txtZipPath);
            panelMain.Controls.Add(rdoUseFile);
            panelMain.Controls.Add(rdoUseCurrent);
            panelMain.Controls.Add(txtPassword);
            panelMain.Controls.Add(lblPassword);
            panelMain.Controls.Add(lblRunAfter);
            panelMain.Controls.Add(txtRunAfter);
            panelMain.EndColor = Color.FromArgb(25, 25, 25);
            panelMain.ForeColor = Color.White;
            panelMain.GlowCenterMaxOpacity = 200;
            panelMain.GlowCenterMinOpacity = 50;
            panelMain.GlowMinSurroundOpacity = 30;
            panelMain.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            panelMain.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            panelMain.Location = new Point(0, 84);
            panelMain.MouseEvents = true;
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(460, 330);
            panelMain.StartColor = Color.FromArgb(16, 16, 16);
            panelMain.TabIndex = 9;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(35, 183);
            label6.Name = "label6";
            label6.Size = new Size(145, 15);
            label6.TabIndex = 52;
            label6.Text = "Show 'completed' dialog";
            // 
            // chkShowDone
            // 
            chkShowDone.BackColor = Color.Transparent;
            chkShowDone.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkShowDone.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkShowDone.BoxGradientEnabled = true;
            chkShowDone.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkShowDone.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkShowDone.BoxSize = 14;
            chkShowDone.CheckBorderColor = Color.Lime;
            chkShowDone.CheckColor = Color.LawnGreen;
            chkShowDone.Checked = true;
            chkShowDone.CheckGradientEnabled = true;
            chkShowDone.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkShowDone.CheckGradientStart = Color.Lime;
            chkShowDone.CheckState = CheckState.Checked;
            chkShowDone.Location = new Point(14, 177);
            chkShowDone.Name = "chkShowDone";
            chkShowDone.Size = new Size(18, 24);
            chkShowDone.TabIndex = 51;
            chkShowDone.UseVisualStyleBackColor = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(35, 158);
            label5.Name = "label5";
            label5.Size = new Size(141, 15);
            label5.TabIndex = 50;
            label5.Text = "Require elevation (UAC)";
            // 
            // chkRequireElevation
            // 
            chkRequireElevation.BackColor = Color.Transparent;
            chkRequireElevation.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkRequireElevation.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkRequireElevation.BoxGradientEnabled = true;
            chkRequireElevation.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkRequireElevation.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkRequireElevation.BoxSize = 14;
            chkRequireElevation.CheckBorderColor = Color.Lime;
            chkRequireElevation.CheckColor = Color.LawnGreen;
            chkRequireElevation.CheckGradientEnabled = true;
            chkRequireElevation.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkRequireElevation.CheckGradientStart = Color.Lime;
            chkRequireElevation.Location = new Point(14, 152);
            chkRequireElevation.Name = "chkRequireElevation";
            chkRequireElevation.Size = new Size(18, 24);
            chkRequireElevation.TabIndex = 49;
            chkRequireElevation.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(35, 134);
            label4.Name = "label4";
            label4.Size = new Size(138, 15);
            label4.TabIndex = 48;
            label4.Text = "Overwrite existing files";
            // 
            // chkOverwrite
            // 
            chkOverwrite.BackColor = Color.Transparent;
            chkOverwrite.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkOverwrite.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkOverwrite.BoxGradientEnabled = true;
            chkOverwrite.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkOverwrite.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkOverwrite.BoxSize = 14;
            chkOverwrite.CheckBorderColor = Color.Lime;
            chkOverwrite.CheckColor = Color.LawnGreen;
            chkOverwrite.Checked = true;
            chkOverwrite.CheckGradientEnabled = true;
            chkOverwrite.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkOverwrite.CheckGradientStart = Color.Lime;
            chkOverwrite.CheckState = CheckState.Checked;
            chkOverwrite.Location = new Point(14, 128);
            chkOverwrite.Name = "chkOverwrite";
            chkOverwrite.Size = new Size(18, 24);
            chkOverwrite.TabIndex = 47;
            chkOverwrite.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(35, 109);
            label3.Name = "label3";
            label3.Size = new Size(39, 15);
            label3.TabIndex = 46;
            label3.Text = "Silent";
            // 
            // chkSilent
            // 
            chkSilent.BackColor = Color.Transparent;
            chkSilent.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkSilent.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkSilent.BoxGradientEnabled = true;
            chkSilent.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkSilent.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkSilent.BoxSize = 14;
            chkSilent.CheckBorderColor = Color.Lime;
            chkSilent.CheckColor = Color.LawnGreen;
            chkSilent.CheckGradientEnabled = true;
            chkSilent.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkSilent.CheckGradientStart = Color.Lime;
            chkSilent.Location = new Point(14, 103);
            chkSilent.Name = "chkSilent";
            chkSilent.Size = new Size(18, 24);
            chkSilent.TabIndex = 45;
            chkSilent.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 79);
            label1.Name = "label1";
            label1.Size = new Size(50, 15);
            label1.TabIndex = 8;
            label1.Text = "Options";
            // 
            // panelBranding
            // 
            panelBranding.Controls.Add(gradientPanelPreview);
            panelBranding.Controls.Add(pnlThemeColorEnd);
            panelBranding.Controls.Add(button1);
            panelBranding.Controls.Add(lblCompany);
            panelBranding.Controls.Add(lblTheme);
            panelBranding.Controls.Add(pnlThemeColor);
            panelBranding.Controls.Add(picBanner);
            panelBranding.Controls.Add(btnPickColor);
            panelBranding.Controls.Add(picIcon);
            panelBranding.Controls.Add(btnBrowseIcon);
            panelBranding.Controls.Add(btnBrowseBanner);
            panelBranding.Controls.Add(txtBannerPath);
            panelBranding.Controls.Add(lblBanner);
            panelBranding.Controls.Add(lblIcon);
            panelBranding.Controls.Add(txtIconPath);
            panelBranding.Controls.Add(lblMacros);
            panelBranding.Controls.Add(txtDefaultDir);
            panelBranding.Controls.Add(lblDefaultDir);
            panelBranding.Controls.Add(txtCompany);
            panelBranding.Controls.Add(lblTitle);
            panelBranding.Controls.Add(txtTitle);
            panelBranding.Controls.Add(label2);
            panelBranding.EndColor = Color.FromArgb(25, 25, 25);
            panelBranding.ForeColor = Color.White;
            panelBranding.GlowCenterMaxOpacity = 200;
            panelBranding.GlowCenterMinOpacity = 50;
            panelBranding.GlowMinSurroundOpacity = 30;
            panelBranding.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            panelBranding.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            panelBranding.Location = new Point(0, 84);
            panelBranding.MouseEvents = true;
            panelBranding.Name = "panelBranding";
            panelBranding.Size = new Size(460, 330);
            panelBranding.StartColor = Color.FromArgb(16, 16, 16);
            panelBranding.TabIndex = 10;
            panelBranding.Visible = false;
            // 
            // gradientPanelPreview
            // 
            gradientPanelPreview.EndColor = Color.Black;
            gradientPanelPreview.GlowCenterMaxOpacity = 200;
            gradientPanelPreview.GlowCenterMinOpacity = 50;
            gradientPanelPreview.GlowMinSurroundOpacity = 30;
            gradientPanelPreview.Location = new Point(206, 300);
            gradientPanelPreview.MouseEvents = true;
            gradientPanelPreview.Name = "gradientPanelPreview";
            gradientPanelPreview.Size = new Size(165, 27);
            gradientPanelPreview.StartColor = Color.FromArgb(29, 181, 82);
            gradientPanelPreview.TabIndex = 21;
            // 
            // pnlThemeColorEnd
            // 
            pnlThemeColorEnd.BackColor = Color.Black;
            pnlThemeColorEnd.Location = new Point(376, 300);
            pnlThemeColorEnd.Name = "pnlThemeColorEnd";
            pnlThemeColorEnd.Size = new Size(13, 24);
            pnlThemeColorEnd.TabIndex = 19;
            // 
            // button1
            // 
            button1.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(393, 299);
            button1.Name = "button1";
            button1.Size = new Size(60, 28);
            button1.TabIndex = 20;
            button1.Text = "Pick…";
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(3, 3);
            label2.Name = "label2";
            label2.Size = new Size(57, 15);
            label2.TabIndex = 9;
            label2.Text = "Branding";
            // 
            // panelLicence
            // 
            panelLicence.Controls.Add(label7);
            panelLicence.Controls.Add(chkRequireAccept);
            panelLicence.Controls.Add(btnLoadLicense);
            panelLicence.Controls.Add(txtLicense);
            panelLicence.EndColor = Color.FromArgb(25, 25, 25);
            panelLicence.ForeColor = Color.White;
            panelLicence.GlowCenterMaxOpacity = 200;
            panelLicence.GlowCenterMinOpacity = 50;
            panelLicence.GlowMinSurroundOpacity = 30;
            panelLicence.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            panelLicence.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            panelLicence.Location = new Point(0, 84);
            panelLicence.MouseEvents = true;
            panelLicence.Name = "panelLicence";
            panelLicence.Size = new Size(460, 330);
            panelLicence.StartColor = Color.FromArgb(16, 16, 16);
            panelLicence.TabIndex = 11;
            panelLicence.Visible = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.Transparent;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(31, 301);
            label7.Name = "label7";
            label7.Size = new Size(117, 15);
            label7.TabIndex = 54;
            label7.Text = "Require acceptance";
            // 
            // chkRequireAccept
            // 
            chkRequireAccept.BackColor = Color.Transparent;
            chkRequireAccept.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkRequireAccept.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkRequireAccept.BoxGradientEnabled = true;
            chkRequireAccept.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkRequireAccept.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkRequireAccept.BoxSize = 14;
            chkRequireAccept.CheckBorderColor = Color.Lime;
            chkRequireAccept.CheckColor = Color.LawnGreen;
            chkRequireAccept.Checked = true;
            chkRequireAccept.CheckGradientEnabled = true;
            chkRequireAccept.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkRequireAccept.CheckGradientStart = Color.Lime;
            chkRequireAccept.CheckState = CheckState.Checked;
            chkRequireAccept.Location = new Point(10, 295);
            chkRequireAccept.Name = "chkRequireAccept";
            chkRequireAccept.Size = new Size(18, 24);
            chkRequireAccept.TabIndex = 53;
            chkRequireAccept.UseVisualStyleBackColor = false;
            // 
            // SfxBuilderForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(462, 415);
            Controls.Add(brutalGradientPanel1);
            Controls.Add(panelBranding);
            Controls.Add(panelLicence);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SfxBuilderForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Build Self-Extracting Archive";
            Load += SfxBuilderForm_Load;
            grpStub.ResumeLayout(false);
            grpStub.PerformLayout();
            ((ISupportInitialize)picIcon).EndInit();
            ((ISupportInitialize)picBanner).EndInit();
            brutalGradientPanel1.ResumeLayout(false);
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            panelBranding.ResumeLayout(false);
            panelBranding.PerformLayout();
            panelLicence.ResumeLayout(false);
            panelLicence.PerformLayout();
            ResumeLayout(false);
        }
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel1;
        private Panel panel6;
        private Button buttonComment;
        public Button buttonOpenFolder;
        private Button buttonAddFolder;
        private Button buttonMain;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel panelMain;
        private Label label1;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel panelBranding;
        private Label label2;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel panelLicence;
        private Button buttonBuild;
        private Button buttonPreview;
        private Panel panel1;
        private Button buttonLicence;
        private Button buttonBranding;
        private Label label6;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkShowDone;
        private Label label5;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkRequireElevation;
        private Label label4;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkOverwrite;
        private Label label3;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkSilent;
        private Label label7;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkRequireAccept;
        internal Panel pnlThemeColorEnd;
        internal Button button1;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel gradientPanelPreview;
    }
}