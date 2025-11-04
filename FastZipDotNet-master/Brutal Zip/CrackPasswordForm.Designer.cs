using System.ComponentModel;

namespace Brutal_Zip
{
    partial class CrackPasswordForm
    {
        private IContainer components = null;
        private Label lblCustom;
        private TextBox txtCustom;
        private Label lblMinLen;
        private NumericUpDown numMinLen;
        private Label lblMaxLen;
        private NumericUpDown numMaxLen;
        private Label lblPrefix;
        private TextBox txtPrefix;
        private Label lblKeyspace;
        private Button btnEstimate;

        // Dictionary controls
        private Label lblDictPath;
        private TextBox txtDictPath;
        private Button btnBrowseDict;

        // Bottom controls
        private Label lblThreads;
        private Label lblThreadsVal;
        private Button btnStart;
        private Button btnStop;
        private Button btnClose;
        private Label lblFound;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblCustom = new Label();
            txtCustom = new TextBox();
            lblMinLen = new Label();
            numMinLen = new NumericUpDown();
            numMaxLen = new NumericUpDown();
            lblPrefix = new Label();
            txtPrefix = new TextBox();
            lblKeyspace = new Label();
            btnEstimate = new Button();
            lblMaxLen = new Label();
            lblDictPath = new Label();
            txtDictPath = new TextBox();
            btnBrowseDict = new Button();
            lblThreads = new Label();
            lblThreadsVal = new Label();
            btnStart = new Button();
            btnStop = new Button();
            btnClose = new Button();
            lblFound = new Label();
            brutalGradientPanel1 = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            panel1 = new Panel();
            buttonJob = new Button();
            panel6 = new Panel();
            buttonDictionary = new Button();
            buttonBruteForce = new Button();
            panelBruteForce = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            label4 = new Label();
            chkSymbols = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label3 = new Label();
            chkDigits = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label2 = new Label();
            chkUpper = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label1 = new Label();
            chkLower = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            panelDictionary = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            label6 = new Label();
            chkAppendDigits = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            label5 = new Label();
            chkMutateCase = new BrutalZip2025.BrutalControls.BrutalCheckBox();
            panelJob = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            labelElapsed = new Label();
            labelSpeed = new Label();
            labelAttempts = new Label();
            tbThreads = new BrutalZip2025.BrutalControls.BrutalTrackBar();
            statusStrip = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            lblStatus = new ToolStripStatusLabel();
            ((ISupportInitialize)numMinLen).BeginInit();
            ((ISupportInitialize)numMaxLen).BeginInit();
            brutalGradientPanel1.SuspendLayout();
            panelBruteForce.SuspendLayout();
            panelDictionary.SuspendLayout();
            panelJob.SuspendLayout();
            ((ISupportInitialize)tbThreads).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // lblCustom
            // 
            lblCustom.BackColor = Color.Transparent;
            lblCustom.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCustom.Location = new Point(20, 49);
            lblCustom.Name = "lblCustom";
            lblCustom.Size = new Size(58, 23);
            lblCustom.TabIndex = 4;
            lblCustom.Text = "Custom:";
            // 
            // txtCustom
            // 
            txtCustom.BackColor = Color.FromArgb(32, 32, 32);
            txtCustom.BorderStyle = BorderStyle.FixedSingle;
            txtCustom.ForeColor = Color.White;
            txtCustom.Location = new Point(107, 47);
            txtCustom.Name = "txtCustom";
            txtCustom.Size = new Size(279, 23);
            txtCustom.TabIndex = 5;
            // 
            // lblMinLen
            // 
            lblMinLen.BackColor = Color.Transparent;
            lblMinLen.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMinLen.Location = new Point(20, 79);
            lblMinLen.Name = "lblMinLen";
            lblMinLen.Size = new Size(80, 23);
            lblMinLen.TabIndex = 6;
            lblMinLen.Text = "Min length:";
            // 
            // numMinLen
            // 
            numMinLen.BackColor = Color.FromArgb(32, 32, 32);
            numMinLen.BorderStyle = BorderStyle.FixedSingle;
            numMinLen.ForeColor = Color.White;
            numMinLen.Location = new Point(107, 77);
            numMinLen.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            numMinLen.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numMinLen.Name = "numMinLen";
            numMinLen.Size = new Size(95, 23);
            numMinLen.TabIndex = 7;
            numMinLen.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numMaxLen
            // 
            numMaxLen.BackColor = Color.FromArgb(32, 32, 32);
            numMaxLen.BorderStyle = BorderStyle.FixedSingle;
            numMaxLen.ForeColor = Color.White;
            numMaxLen.Location = new Point(291, 77);
            numMaxLen.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            numMaxLen.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numMaxLen.Name = "numMaxLen";
            numMaxLen.Size = new Size(95, 23);
            numMaxLen.TabIndex = 9;
            numMaxLen.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // lblPrefix
            // 
            lblPrefix.BackColor = Color.Transparent;
            lblPrefix.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPrefix.Location = new Point(16, 109);
            lblPrefix.Name = "lblPrefix";
            lblPrefix.Size = new Size(76, 40);
            lblPrefix.TabIndex = 10;
            lblPrefix.Text = "Starts with (optional):";
            // 
            // txtPrefix
            // 
            txtPrefix.BackColor = Color.FromArgb(32, 32, 32);
            txtPrefix.BorderStyle = BorderStyle.FixedSingle;
            txtPrefix.ForeColor = Color.White;
            txtPrefix.Location = new Point(107, 109);
            txtPrefix.Name = "txtPrefix";
            txtPrefix.Size = new Size(279, 23);
            txtPrefix.TabIndex = 11;
            // 
            // lblKeyspace
            // 
            lblKeyspace.BackColor = Color.Transparent;
            lblKeyspace.Location = new Point(20, 158);
            lblKeyspace.Name = "lblKeyspace";
            lblKeyspace.Size = new Size(366, 20);
            lblKeyspace.TabIndex = 12;
            lblKeyspace.Text = "Keyspace: -";
            // 
            // btnEstimate
            // 
            btnEstimate.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnEstimate.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnEstimate.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnEstimate.FlatStyle = FlatStyle.Flat;
            btnEstimate.Location = new Point(20, 184);
            btnEstimate.Name = "btnEstimate";
            btnEstimate.Size = new Size(120, 26);
            btnEstimate.TabIndex = 13;
            btnEstimate.Text = "Estimate";
            btnEstimate.Click += btnEstimate_Click;
            // 
            // lblMaxLen
            // 
            lblMaxLen.BackColor = Color.Transparent;
            lblMaxLen.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMaxLen.Location = new Point(215, 79);
            lblMaxLen.Name = "lblMaxLen";
            lblMaxLen.Size = new Size(79, 23);
            lblMaxLen.TabIndex = 8;
            lblMaxLen.Text = "Max length:";
            // 
            // lblDictPath
            // 
            lblDictPath.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDictPath.Location = new Point(14, 17);
            lblDictPath.Name = "lblDictPath";
            lblDictPath.Size = new Size(58, 23);
            lblDictPath.TabIndex = 0;
            lblDictPath.Text = "List file:";
            // 
            // txtDictPath
            // 
            txtDictPath.BackColor = Color.FromArgb(32, 32, 32);
            txtDictPath.BorderStyle = BorderStyle.FixedSingle;
            txtDictPath.ForeColor = Color.White;
            txtDictPath.Location = new Point(78, 15);
            txtDictPath.Name = "txtDictPath";
            txtDictPath.Size = new Size(218, 23);
            txtDictPath.TabIndex = 1;
            // 
            // btnBrowseDict
            // 
            btnBrowseDict.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnBrowseDict.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnBrowseDict.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnBrowseDict.FlatStyle = FlatStyle.Flat;
            btnBrowseDict.Location = new Point(306, 12);
            btnBrowseDict.Name = "btnBrowseDict";
            btnBrowseDict.Size = new Size(90, 26);
            btnBrowseDict.TabIndex = 2;
            btnBrowseDict.Text = "Browse…";
            btnBrowseDict.Click += btnBrowseDict_Click;
            // 
            // lblThreads
            // 
            lblThreads.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblThreads.Location = new Point(16, 16);
            lblThreads.Name = "lblThreads";
            lblThreads.Size = new Size(62, 23);
            lblThreads.TabIndex = 1;
            lblThreads.Text = "Threads:";
            // 
            // lblThreadsVal
            // 
            lblThreadsVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblThreadsVal.Location = new Point(232, 16);
            lblThreadsVal.Name = "lblThreadsVal";
            lblThreadsVal.Size = new Size(40, 20);
            lblThreadsVal.TabIndex = 3;
            lblThreadsVal.Text = "16";
            // 
            // btnStart
            // 
            btnStart.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnStart.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnStart.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.Location = new Point(150, 192);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(80, 28);
            btnStart.TabIndex = 4;
            btnStart.Text = "Start";
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Location = new Point(236, 192);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(80, 28);
            btnStop.TabIndex = 5;
            btnStop.Text = "Stop";
            btnStop.Click += btnStop_Click;
            // 
            // btnClose
            // 
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnClose.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(322, 192);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(80, 28);
            btnClose.TabIndex = 6;
            btnClose.Text = "Close";
            // 
            // lblFound
            // 
            lblFound.BackColor = Color.Transparent;
            lblFound.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblFound.Location = new Point(14, 168);
            lblFound.Name = "lblFound";
            lblFound.Size = new Size(388, 20);
            lblFound.TabIndex = 8;
            lblFound.Text = "Found: -";
            // 
            // brutalGradientPanel1
            // 
            brutalGradientPanel1.BackColor = Color.Transparent;
            brutalGradientPanel1.Controls.Add(panel1);
            brutalGradientPanel1.Controls.Add(buttonJob);
            brutalGradientPanel1.Controls.Add(panel6);
            brutalGradientPanel1.Controls.Add(buttonDictionary);
            brutalGradientPanel1.Controls.Add(buttonBruteForce);
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
            brutalGradientPanel1.Size = new Size(406, 84);
            brutalGradientPanel1.StartColor = Color.FromArgb(32, 32, 32);
            brutalGradientPanel1.TabIndex = 9;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(32, 32, 32);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(257, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(0, 0, 0, 2);
            panel1.Size = new Size(2, 84);
            panel1.TabIndex = 39;
            // 
            // buttonJob
            // 
            buttonJob.BackColor = Color.Transparent;
            buttonJob.Dock = DockStyle.Left;
            buttonJob.FlatAppearance.BorderSize = 0;
            buttonJob.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonJob.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonJob.FlatStyle = FlatStyle.Flat;
            buttonJob.Location = new Point(172, 0);
            buttonJob.Name = "buttonJob";
            buttonJob.Size = new Size(85, 84);
            buttonJob.TabIndex = 38;
            buttonJob.Text = "Job";
            buttonJob.TextAlign = ContentAlignment.BottomCenter;
            buttonJob.UseVisualStyleBackColor = false;
            buttonJob.Click += buttonJob_Click;
            // 
            // panel6
            // 
            panel6.BackColor = Color.FromArgb(32, 32, 32);
            panel6.Dock = DockStyle.Left;
            panel6.Location = new Point(170, 0);
            panel6.Name = "panel6";
            panel6.Padding = new Padding(0, 0, 0, 2);
            panel6.Size = new Size(2, 84);
            panel6.TabIndex = 33;
            // 
            // buttonDictionary
            // 
            buttonDictionary.BackColor = Color.Transparent;
            buttonDictionary.Dock = DockStyle.Left;
            buttonDictionary.FlatAppearance.BorderSize = 0;
            buttonDictionary.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonDictionary.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonDictionary.FlatStyle = FlatStyle.Flat;
            buttonDictionary.Location = new Point(85, 0);
            buttonDictionary.Name = "buttonDictionary";
            buttonDictionary.Size = new Size(85, 84);
            buttonDictionary.TabIndex = 37;
            buttonDictionary.Text = "Dictionary";
            buttonDictionary.TextAlign = ContentAlignment.BottomCenter;
            buttonDictionary.UseVisualStyleBackColor = false;
            buttonDictionary.Click += buttonDictionary_Click;
            // 
            // buttonBruteForce
            // 
            buttonBruteForce.BackColor = Color.Transparent;
            buttonBruteForce.Dock = DockStyle.Left;
            buttonBruteForce.FlatAppearance.BorderSize = 0;
            buttonBruteForce.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 32, 32);
            buttonBruteForce.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            buttonBruteForce.FlatStyle = FlatStyle.Flat;
            buttonBruteForce.Location = new Point(0, 0);
            buttonBruteForce.Name = "buttonBruteForce";
            buttonBruteForce.Size = new Size(85, 84);
            buttonBruteForce.TabIndex = 0;
            buttonBruteForce.Text = "Brute Force";
            buttonBruteForce.TextAlign = ContentAlignment.BottomCenter;
            buttonBruteForce.UseVisualStyleBackColor = false;
            buttonBruteForce.Click += buttonBruteForce_Click;
            // 
            // panelBruteForce
            // 
            panelBruteForce.Controls.Add(label4);
            panelBruteForce.Controls.Add(chkSymbols);
            panelBruteForce.Controls.Add(label3);
            panelBruteForce.Controls.Add(chkDigits);
            panelBruteForce.Controls.Add(label2);
            panelBruteForce.Controls.Add(chkUpper);
            panelBruteForce.Controls.Add(label1);
            panelBruteForce.Controls.Add(chkLower);
            panelBruteForce.Controls.Add(lblKeyspace);
            panelBruteForce.Controls.Add(btnEstimate);
            panelBruteForce.Controls.Add(lblPrefix);
            panelBruteForce.Controls.Add(txtPrefix);
            panelBruteForce.Controls.Add(numMaxLen);
            panelBruteForce.Controls.Add(lblMinLen);
            panelBruteForce.Controls.Add(lblCustom);
            panelBruteForce.Controls.Add(numMinLen);
            panelBruteForce.Controls.Add(txtCustom);
            panelBruteForce.Controls.Add(lblMaxLen);
            panelBruteForce.EndColor = Color.FromArgb(25, 25, 25);
            panelBruteForce.ForeColor = Color.White;
            panelBruteForce.GlowCenterMaxOpacity = 200;
            panelBruteForce.GlowCenterMinOpacity = 50;
            panelBruteForce.GlowMinSurroundOpacity = 30;
            panelBruteForce.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            panelBruteForce.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            panelBruteForce.Location = new Point(0, 84);
            panelBruteForce.MouseEvents = true;
            panelBruteForce.Name = "panelBruteForce";
            panelBruteForce.Size = new Size(406, 224);
            panelBruteForce.StartColor = Color.FromArgb(16, 16, 16);
            panelBruteForce.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(205, 20);
            label4.Name = "label4";
            label4.Size = new Size(45, 15);
            label4.TabIndex = 42;
            label4.Text = "!@#$...";
            // 
            // chkSymbols
            // 
            chkSymbols.BackColor = Color.Transparent;
            chkSymbols.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkSymbols.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkSymbols.BoxGradientEnabled = true;
            chkSymbols.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkSymbols.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkSymbols.BoxSize = 14;
            chkSymbols.CheckBorderColor = Color.Lime;
            chkSymbols.CheckColor = Color.LawnGreen;
            chkSymbols.Checked = true;
            chkSymbols.CheckGradientEnabled = true;
            chkSymbols.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkSymbols.CheckGradientStart = Color.Lime;
            chkSymbols.CheckState = CheckState.Checked;
            chkSymbols.Location = new Point(185, 15);
            chkSymbols.Name = "chkSymbols";
            chkSymbols.Size = new Size(18, 24);
            chkSymbols.TabIndex = 41;
            chkSymbols.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(153, 20);
            label3.Name = "label3";
            label3.Size = new Size(26, 15);
            label3.TabIndex = 40;
            label3.Text = "0-9";
            // 
            // chkDigits
            // 
            chkDigits.BackColor = Color.Transparent;
            chkDigits.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkDigits.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkDigits.BoxGradientEnabled = true;
            chkDigits.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkDigits.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkDigits.BoxSize = 14;
            chkDigits.CheckBorderColor = Color.Lime;
            chkDigits.CheckColor = Color.LawnGreen;
            chkDigits.Checked = true;
            chkDigits.CheckGradientEnabled = true;
            chkDigits.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkDigits.CheckGradientStart = Color.Lime;
            chkDigits.CheckState = CheckState.Checked;
            chkDigits.Location = new Point(133, 15);
            chkDigits.Name = "chkDigits";
            chkDigits.Size = new Size(18, 24);
            chkDigits.TabIndex = 39;
            chkDigits.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(97, 20);
            label2.Name = "label2";
            label2.Size = new Size(27, 15);
            label2.TabIndex = 38;
            label2.Text = "A-Z";
            // 
            // chkUpper
            // 
            chkUpper.BackColor = Color.Transparent;
            chkUpper.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkUpper.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkUpper.BoxGradientEnabled = true;
            chkUpper.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkUpper.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkUpper.BoxSize = 14;
            chkUpper.CheckBorderColor = Color.Lime;
            chkUpper.CheckColor = Color.LawnGreen;
            chkUpper.Checked = true;
            chkUpper.CheckGradientEnabled = true;
            chkUpper.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkUpper.CheckGradientStart = Color.Lime;
            chkUpper.CheckState = CheckState.Checked;
            chkUpper.Location = new Point(77, 15);
            chkUpper.Name = "chkUpper";
            chkUpper.Size = new Size(18, 24);
            chkUpper.TabIndex = 37;
            chkUpper.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(40, 20);
            label1.Name = "label1";
            label1.Size = new Size(24, 15);
            label1.TabIndex = 36;
            label1.Text = "a-z";
            // 
            // chkLower
            // 
            chkLower.BackColor = Color.Transparent;
            chkLower.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkLower.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkLower.BoxGradientEnabled = true;
            chkLower.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkLower.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkLower.BoxSize = 14;
            chkLower.CheckBorderColor = Color.Lime;
            chkLower.CheckColor = Color.LawnGreen;
            chkLower.Checked = true;
            chkLower.CheckGradientEnabled = true;
            chkLower.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkLower.CheckGradientStart = Color.Lime;
            chkLower.CheckState = CheckState.Checked;
            chkLower.Location = new Point(20, 15);
            chkLower.Name = "chkLower";
            chkLower.Size = new Size(18, 24);
            chkLower.TabIndex = 35;
            chkLower.UseVisualStyleBackColor = false;
            // 
            // panelDictionary
            // 
            panelDictionary.Controls.Add(label6);
            panelDictionary.Controls.Add(chkAppendDigits);
            panelDictionary.Controls.Add(label5);
            panelDictionary.Controls.Add(chkMutateCase);
            panelDictionary.Controls.Add(lblDictPath);
            panelDictionary.Controls.Add(txtDictPath);
            panelDictionary.Controls.Add(btnBrowseDict);
            panelDictionary.EndColor = Color.FromArgb(25, 25, 25);
            panelDictionary.ForeColor = Color.White;
            panelDictionary.GlowCenterMaxOpacity = 200;
            panelDictionary.GlowCenterMinOpacity = 50;
            panelDictionary.GlowMinSurroundOpacity = 30;
            panelDictionary.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            panelDictionary.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            panelDictionary.Location = new Point(0, 84);
            panelDictionary.MouseEvents = true;
            panelDictionary.Name = "panelDictionary";
            panelDictionary.Size = new Size(406, 224);
            panelDictionary.StartColor = Color.FromArgb(16, 16, 16);
            panelDictionary.TabIndex = 11;
            panelDictionary.Visible = false;
            panelDictionary.Paint += panelDictionary_Paint;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.Transparent;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(36, 84);
            label6.Name = "label6";
            label6.Size = new Size(137, 15);
            label6.TabIndex = 44;
            label6.Text = "Try digits suffix (0–999)";
            // 
            // chkAppendDigits
            // 
            chkAppendDigits.BackColor = Color.Transparent;
            chkAppendDigits.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkAppendDigits.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkAppendDigits.BoxGradientEnabled = true;
            chkAppendDigits.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkAppendDigits.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkAppendDigits.BoxSize = 14;
            chkAppendDigits.CheckBorderColor = Color.Lime;
            chkAppendDigits.CheckColor = Color.LawnGreen;
            chkAppendDigits.Checked = true;
            chkAppendDigits.CheckGradientEnabled = true;
            chkAppendDigits.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkAppendDigits.CheckGradientStart = Color.Lime;
            chkAppendDigits.CheckState = CheckState.Checked;
            chkAppendDigits.Location = new Point(16, 79);
            chkAppendDigits.Name = "chkAppendDigits";
            chkAppendDigits.Size = new Size(18, 24);
            chkAppendDigits.TabIndex = 43;
            chkAppendDigits.UseVisualStyleBackColor = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(36, 57);
            label5.Name = "label5";
            label5.Size = new Size(207, 15);
            label5.TabIndex = 42;
            label5.Text = "Try lower/UPPER/Title-case variants";
            // 
            // chkMutateCase
            // 
            chkMutateCase.BackColor = Color.Transparent;
            chkMutateCase.BoxBackColor = Color.FromArgb(64, 64, 64);
            chkMutateCase.BoxBorderColor = Color.FromArgb(29, 181, 82);
            chkMutateCase.BoxGradientEnabled = true;
            chkMutateCase.BoxGradientEnd = Color.FromArgb(16, 16, 16);
            chkMutateCase.BoxGradientStart = Color.FromArgb(29, 181, 82);
            chkMutateCase.BoxSize = 14;
            chkMutateCase.CheckBorderColor = Color.Lime;
            chkMutateCase.CheckColor = Color.LawnGreen;
            chkMutateCase.Checked = true;
            chkMutateCase.CheckGradientEnabled = true;
            chkMutateCase.CheckGradientEnd = Color.FromArgb(29, 181, 82);
            chkMutateCase.CheckGradientStart = Color.Lime;
            chkMutateCase.CheckState = CheckState.Checked;
            chkMutateCase.Location = new Point(16, 52);
            chkMutateCase.Name = "chkMutateCase";
            chkMutateCase.Size = new Size(18, 24);
            chkMutateCase.TabIndex = 41;
            chkMutateCase.UseVisualStyleBackColor = false;
            // 
            // panelJob
            // 
            panelJob.Controls.Add(labelElapsed);
            panelJob.Controls.Add(labelSpeed);
            panelJob.Controls.Add(labelAttempts);
            panelJob.Controls.Add(tbThreads);
            panelJob.Controls.Add(lblThreads);
            panelJob.Controls.Add(lblThreadsVal);
            panelJob.Controls.Add(lblFound);
            panelJob.Controls.Add(btnStart);
            panelJob.Controls.Add(btnStop);
            panelJob.Controls.Add(btnClose);
            panelJob.EndColor = Color.FromArgb(25, 25, 25);
            panelJob.ForeColor = Color.White;
            panelJob.GlowCenterMaxOpacity = 200;
            panelJob.GlowCenterMinOpacity = 50;
            panelJob.GlowMinSurroundOpacity = 30;
            panelJob.GradientEndSide = BrutalZip2025.BrutalControls.GradientSide.Top;
            panelJob.GradientStartSide = BrutalZip2025.BrutalControls.GradientSide.Bottom;
            panelJob.Location = new Point(0, 84);
            panelJob.MouseEvents = true;
            panelJob.Name = "panelJob";
            panelJob.Size = new Size(406, 224);
            panelJob.StartColor = Color.FromArgb(16, 16, 16);
            panelJob.TabIndex = 12;
            panelJob.Visible = false;
            // 
            // labelElapsed
            // 
            labelElapsed.BackColor = Color.Transparent;
            labelElapsed.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelElapsed.Location = new Point(16, 104);
            labelElapsed.Name = "labelElapsed";
            labelElapsed.Size = new Size(369, 18);
            labelElapsed.TabIndex = 45;
            labelElapsed.Text = "Elapsed:";
            // 
            // labelSpeed
            // 
            labelSpeed.BackColor = Color.Transparent;
            labelSpeed.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelSpeed.Location = new Point(16, 82);
            labelSpeed.Name = "labelSpeed";
            labelSpeed.Size = new Size(369, 18);
            labelSpeed.TabIndex = 44;
            labelSpeed.Text = "Speed:";
            // 
            // labelAttempts
            // 
            labelAttempts.BackColor = Color.Transparent;
            labelAttempts.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelAttempts.Location = new Point(16, 60);
            labelAttempts.Name = "labelAttempts";
            labelAttempts.Size = new Size(369, 18);
            labelAttempts.TabIndex = 43;
            labelAttempts.Text = "Attempts:";
            // 
            // tbThreads
            // 
            tbThreads.LeftBarColor = Color.FromArgb(29, 181, 82);
            tbThreads.Location = new Point(78, 3);
            tbThreads.Maximum = 16;
            tbThreads.Name = "tbThreads";
            tbThreads.RightBarColor = Color.FromArgb(22, 132, 99);
            tbThreads.Size = new Size(143, 45);
            tbThreads.TabIndex = 42;
            tbThreads.ThumbInnerColor = Color.FromArgb(29, 181, 82);
            tbThreads.ThumbOutlineColor = Color.FromArgb(19, 90, 42);
            tbThreads.ThumbOutlineThickness = 2;
            tbThreads.Value = 16;
            tbThreads.ValueChanged += tbThreads_ValueChanged;
            // 
            // statusStrip
            // 
            statusStrip.BackColor = Color.FromArgb(16, 16, 16);
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, lblStatus });
            statusStrip.Location = new Point(0, 307);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(406, 22);
            statusStrip.TabIndex = 13;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(0, 17);
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(12, 17);
            lblStatus.Text = "_";
            // 
            // CrackPasswordForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(406, 329);
            Controls.Add(statusStrip);
            Controls.Add(brutalGradientPanel1);
            Controls.Add(panelJob);
            Controls.Add(panelBruteForce);
            Controls.Add(panelDictionary);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CrackPasswordForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Crack Password";
            Load += CrackPasswordForm_Load;
            ((ISupportInitialize)numMinLen).EndInit();
            ((ISupportInitialize)numMaxLen).EndInit();
            brutalGradientPanel1.ResumeLayout(false);
            panelBruteForce.ResumeLayout(false);
            panelBruteForce.PerformLayout();
            panelDictionary.ResumeLayout(false);
            panelDictionary.PerformLayout();
            panelJob.ResumeLayout(false);
            panelJob.PerformLayout();
            ((ISupportInitialize)tbThreads).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel1;
        private Panel panel6;
        private Button buttonDictionary;
        private Button buttonBruteForce;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel panelBruteForce;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel panelDictionary;
        private Panel panel1;
        private Button buttonJob;
        private BrutalZip2025.BrutalControls.BrutalGradientPanel panelJob;
        private BrutalZip2025.BrutalControls.BrutalTrackBar tbThreads;
        internal StatusStrip statusStrip;
        internal ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel lblStatus;
        private Label labelSpeed;
        private Label labelAttempts;
        private Label labelElapsed;
        private Label label1;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkLower;
        private Label label2;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkUpper;
        private Label label3;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkDigits;
        private Label label4;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkSymbols;
        private Label label5;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkMutateCase;
        private Label label6;
        private BrutalZip2025.BrutalControls.BrutalCheckBox chkAppendDigits;
    }
}