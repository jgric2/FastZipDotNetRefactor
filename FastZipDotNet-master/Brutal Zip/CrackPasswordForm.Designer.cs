using System.ComponentModel;

namespace Brutal_Zip
{
    partial class CrackPasswordForm
    {
        private IContainer components = null;

        private TabControl tabs;
        private TabPage tabBrute;
        private TabPage tabDict;

        // Brute-force controls
        private CheckBox chkLower;
        private CheckBox chkUpper;
        private CheckBox chkDigits;
        private CheckBox chkSymbols;
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
        private CheckBox chkMutateCase;
        private CheckBox chkAppendDigits;

        // Bottom controls
        private Label lblThreads;
        private TrackBar tbThreads;
        private Label lblThreadsVal;
        private Button btnStart;
        private Button btnStop;
        private Button btnClose;

        private Label lblStatus;
        private Label lblFound;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tabs = new TabControl();
            tabBrute = new TabPage();
            chkDigits = new CheckBox();
            chkLower = new CheckBox();
            chkUpper = new CheckBox();
            chkSymbols = new CheckBox();
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
            tabDict = new TabPage();
            lblDictPath = new Label();
            txtDictPath = new TextBox();
            btnBrowseDict = new Button();
            chkMutateCase = new CheckBox();
            chkAppendDigits = new CheckBox();
            lblThreads = new Label();
            tbThreads = new TrackBar();
            lblThreadsVal = new Label();
            btnStart = new Button();
            btnStop = new Button();
            btnClose = new Button();
            lblStatus = new Label();
            lblFound = new Label();
            tabs.SuspendLayout();
            tabBrute.SuspendLayout();
            ((ISupportInitialize)numMinLen).BeginInit();
            ((ISupportInitialize)numMaxLen).BeginInit();
            tabDict.SuspendLayout();
            ((ISupportInitialize)tbThreads).BeginInit();
            SuspendLayout();
            // 
            // tabs
            // 
            tabs.Controls.Add(tabBrute);
            tabs.Controls.Add(tabDict);
            tabs.Location = new Point(12, 12);
            tabs.Name = "tabs";
            tabs.SelectedIndex = 0;
            tabs.Size = new Size(696, 300);
            tabs.TabIndex = 0;
            // 
            // tabBrute
            // 
            tabBrute.Controls.Add(chkDigits);
            tabBrute.Controls.Add(chkLower);
            tabBrute.Controls.Add(chkUpper);
            tabBrute.Controls.Add(chkSymbols);
            tabBrute.Controls.Add(lblCustom);
            tabBrute.Controls.Add(txtCustom);
            tabBrute.Controls.Add(lblMinLen);
            tabBrute.Controls.Add(numMinLen);
            tabBrute.Controls.Add(numMaxLen);
            tabBrute.Controls.Add(lblPrefix);
            tabBrute.Controls.Add(txtPrefix);
            tabBrute.Controls.Add(lblKeyspace);
            tabBrute.Controls.Add(btnEstimate);
            tabBrute.Controls.Add(lblMaxLen);
            tabBrute.Location = new Point(4, 24);
            tabBrute.Name = "tabBrute";
            tabBrute.Size = new Size(688, 272);
            tabBrute.TabIndex = 0;
            tabBrute.Text = "Brute force";
            // 
            // chkDigits
            // 
            chkDigits.Checked = true;
            chkDigits.CheckState = CheckState.Checked;
            chkDigits.Location = new Point(140, 16);
            chkDigits.Name = "chkDigits";
            chkDigits.Size = new Size(54, 24);
            chkDigits.TabIndex = 2;
            chkDigits.Text = "0-9";
            // 
            // chkLower
            // 
            chkLower.Checked = true;
            chkLower.CheckState = CheckState.Checked;
            chkLower.Location = new Point(16, 16);
            chkLower.Name = "chkLower";
            chkLower.Size = new Size(46, 24);
            chkLower.TabIndex = 0;
            chkLower.Text = "a-z";
            // 
            // chkUpper
            // 
            chkUpper.Checked = true;
            chkUpper.CheckState = CheckState.Checked;
            chkUpper.Location = new Point(78, 16);
            chkUpper.Name = "chkUpper";
            chkUpper.Size = new Size(58, 24);
            chkUpper.TabIndex = 1;
            chkUpper.Text = "A-Z";
            // 
            // chkSymbols
            // 
            chkSymbols.Location = new Point(195, 16);
            chkSymbols.Name = "chkSymbols";
            chkSymbols.Size = new Size(65, 24);
            chkSymbols.TabIndex = 3;
            chkSymbols.Text = "!@#$...";
            // 
            // lblCustom
            // 
            lblCustom.Location = new Point(16, 48);
            lblCustom.Name = "lblCustom";
            lblCustom.Size = new Size(58, 23);
            lblCustom.TabIndex = 4;
            lblCustom.Text = "Custom:";
            // 
            // txtCustom
            // 
            txtCustom.Location = new Point(103, 46);
            txtCustom.Name = "txtCustom";
            txtCustom.Size = new Size(257, 23);
            txtCustom.TabIndex = 5;
            // 
            // lblMinLen
            // 
            lblMinLen.Location = new Point(16, 80);
            lblMinLen.Name = "lblMinLen";
            lblMinLen.Size = new Size(80, 23);
            lblMinLen.TabIndex = 6;
            lblMinLen.Text = "Min length:";
            // 
            // numMinLen
            // 
            numMinLen.Location = new Point(103, 78);
            numMinLen.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            numMinLen.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numMinLen.Name = "numMinLen";
            numMinLen.Size = new Size(120, 23);
            numMinLen.TabIndex = 7;
            numMinLen.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numMaxLen
            // 
            numMaxLen.Location = new Point(323, 78);
            numMaxLen.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            numMaxLen.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numMaxLen.Name = "numMaxLen";
            numMaxLen.Size = new Size(120, 23);
            numMaxLen.TabIndex = 9;
            numMaxLen.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // lblPrefix
            // 
            lblPrefix.Location = new Point(16, 112);
            lblPrefix.Name = "lblPrefix";
            lblPrefix.Size = new Size(100, 23);
            lblPrefix.TabIndex = 10;
            lblPrefix.Text = "Starts with (optional):";
            // 
            // txtPrefix
            // 
            txtPrefix.Location = new Point(111, 110);
            txtPrefix.Name = "txtPrefix";
            txtPrefix.Size = new Size(249, 23);
            txtPrefix.TabIndex = 11;
            // 
            // lblKeyspace
            // 
            lblKeyspace.Location = new Point(16, 144);
            lblKeyspace.Name = "lblKeyspace";
            lblKeyspace.Size = new Size(600, 20);
            lblKeyspace.TabIndex = 12;
            lblKeyspace.Text = "Keyspace: -";
            // 
            // btnEstimate
            // 
            btnEstimate.Location = new Point(16, 170);
            btnEstimate.Name = "btnEstimate";
            btnEstimate.Size = new Size(120, 26);
            btnEstimate.TabIndex = 13;
            btnEstimate.Text = "Estimate";
            btnEstimate.Click += btnEstimate_Click;
            // 
            // lblMaxLen
            // 
            lblMaxLen.Location = new Point(240, 80);
            lblMaxLen.Name = "lblMaxLen";
            lblMaxLen.Size = new Size(100, 23);
            lblMaxLen.TabIndex = 8;
            lblMaxLen.Text = "Max length:";
            // 
            // tabDict
            // 
            tabDict.Controls.Add(lblDictPath);
            tabDict.Controls.Add(txtDictPath);
            tabDict.Controls.Add(btnBrowseDict);
            tabDict.Controls.Add(chkMutateCase);
            tabDict.Controls.Add(chkAppendDigits);
            tabDict.Location = new Point(4, 24);
            tabDict.Name = "tabDict";
            tabDict.Size = new Size(688, 272);
            tabDict.TabIndex = 1;
            tabDict.Text = "Dictionary";
            // 
            // lblDictPath
            // 
            lblDictPath.Location = new Point(16, 16);
            lblDictPath.Name = "lblDictPath";
            lblDictPath.Size = new Size(58, 23);
            lblDictPath.TabIndex = 0;
            lblDictPath.Text = "List file:";
            // 
            // txtDictPath
            // 
            txtDictPath.Location = new Point(80, 14);
            txtDictPath.Name = "txtDictPath";
            txtDictPath.Size = new Size(460, 23);
            txtDictPath.TabIndex = 1;
            // 
            // btnBrowseDict
            // 
            btnBrowseDict.Location = new Point(550, 12);
            btnBrowseDict.Name = "btnBrowseDict";
            btnBrowseDict.Size = new Size(90, 26);
            btnBrowseDict.TabIndex = 2;
            btnBrowseDict.Text = "Browse…";
            btnBrowseDict.Click += btnBrowseDict_Click;
            // 
            // chkMutateCase
            // 
            chkMutateCase.Location = new Point(16, 52);
            chkMutateCase.Name = "chkMutateCase";
            chkMutateCase.Size = new Size(104, 24);
            chkMutateCase.TabIndex = 3;
            chkMutateCase.Text = "Try lower/UPPER/Title-case variants";
            // 
            // chkAppendDigits
            // 
            chkAppendDigits.Location = new Point(16, 74);
            chkAppendDigits.Name = "chkAppendDigits";
            chkAppendDigits.Size = new Size(104, 24);
            chkAppendDigits.TabIndex = 4;
            chkAppendDigits.Text = "Try digits suffix (0–999)";
            // 
            // lblThreads
            // 
            lblThreads.Location = new Point(12, 320);
            lblThreads.Name = "lblThreads";
            lblThreads.Size = new Size(100, 23);
            lblThreads.TabIndex = 1;
            lblThreads.Text = "Threads:";
            // 
            // tbThreads
            // 
            tbThreads.Location = new Point(72, 316);
            tbThreads.Maximum = 16;
            tbThreads.Minimum = 1;
            tbThreads.Name = "tbThreads";
            tbThreads.Size = new Size(260, 45);
            tbThreads.TabIndex = 2;
            tbThreads.Value = 16;
            tbThreads.ValueChanged += tbThreads_ValueChanged;
            // 
            // lblThreadsVal
            // 
            lblThreadsVal.Location = new Point(340, 320);
            lblThreadsVal.Name = "lblThreadsVal";
            lblThreadsVal.Size = new Size(40, 20);
            lblThreadsVal.TabIndex = 3;
            lblThreadsVal.Text = "16";
            // 
            // btnStart
            // 
            btnStart.Location = new Point(400, 316);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(80, 28);
            btnStart.TabIndex = 4;
            btnStart.Text = "Start";
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(486, 316);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(80, 28);
            btnStop.TabIndex = 5;
            btnStop.Text = "Stop";
            btnStop.Click += btnStop_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(572, 316);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(80, 28);
            btnClose.TabIndex = 6;
            btnClose.Text = "Close";
            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(12, 360);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(696, 40);
            lblStatus.TabIndex = 7;
            lblStatus.Text = "Idle.";
            // 
            // lblFound
            // 
            lblFound.Location = new Point(12, 406);
            lblFound.Name = "lblFound";
            lblFound.Size = new Size(696, 20);
            lblFound.TabIndex = 8;
            lblFound.Text = "Found: -";
            // 
            // CrackPasswordForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(720, 520);
            Controls.Add(tabs);
            Controls.Add(lblThreads);
            Controls.Add(tbThreads);
            Controls.Add(lblThreadsVal);
            Controls.Add(btnStart);
            Controls.Add(btnStop);
            Controls.Add(btnClose);
            Controls.Add(lblStatus);
            Controls.Add(lblFound);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CrackPasswordForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Crack Password";
            Load += CrackPasswordForm_Load;
            tabs.ResumeLayout(false);
            tabBrute.ResumeLayout(false);
            tabBrute.PerformLayout();
            ((ISupportInitialize)numMinLen).EndInit();
            ((ISupportInitialize)numMaxLen).EndInit();
            tabDict.ResumeLayout(false);
            tabDict.PerformLayout();
            ((ISupportInitialize)tbThreads).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}