namespace TestWinForms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            progressBar1 = new ProgressBar();
            labelCurrentFile = new Label();
            txtOutputZip = new TextBox();
            btnCancel = new Button();
            btnBuild = new Button();
            btnUpdate = new Button();
            btnExtract = new Button();
            label1 = new Label();
            label2 = new Label();
            txtSourceFolder = new TextBox();
            labelOperation = new Label();
            label3 = new Label();
            txtExtractTo = new TextBox();
            numCompressionLevel = new NumericUpDown();
            btnTest = new Button();
            btnRepair = new Button();
            label4 = new Label();
            txtInputZip = new TextBox();
            labelStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)numCompressionLevel).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(92, 47);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(92, 98);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 1;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(92, 153);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 2;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(708, 244);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 3;
            button4.Text = "button4";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(223, 277);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(351, 23);
            progressBar1.TabIndex = 4;
            // 
            // labelCurrentFile
            // 
            labelCurrentFile.Location = new Point(223, 303);
            labelCurrentFile.Name = "labelCurrentFile";
            labelCurrentFile.Size = new Size(351, 67);
            labelCurrentFile.TabIndex = 5;
            labelCurrentFile.Text = "label1";
            labelCurrentFile.Click += labelStatus_Click;
            // 
            // txtOutputZip
            // 
            txtOutputZip.Location = new Point(223, 248);
            txtOutputZip.Name = "txtOutputZip";
            txtOutputZip.Size = new Size(351, 23);
            txtOutputZip.TabIndex = 6;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(708, 375);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnBuild
            // 
            btnBuild.Location = new Point(583, 155);
            btnBuild.Name = "btnBuild";
            btnBuild.Size = new Size(75, 23);
            btnBuild.TabIndex = 8;
            btnBuild.Text = "Build";
            btnBuild.UseVisualStyleBackColor = true;
            btnBuild.Click += btnBuild_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(583, 184);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(75, 23);
            btnUpdate.TabIndex = 9;
            btnUpdate.Text = "update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnExtract
            // 
            btnExtract.Location = new Point(595, 303);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new Size(75, 23);
            btnExtract.TabIndex = 10;
            btnExtract.Text = "Extract";
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += btnExtract_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(141, 252);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 11;
            label1.Text = "output zip";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(141, 222);
            label2.Name = "label2";
            label2.Size = new Size(76, 15);
            label2.TabIndex = 13;
            label2.Text = "source folder";
            // 
            // txtSourceFolder
            // 
            txtSourceFolder.Location = new Point(223, 219);
            txtSourceFolder.Name = "txtSourceFolder";
            txtSourceFolder.Size = new Size(351, 23);
            txtSourceFolder.TabIndex = 12;
            // 
            // labelOperation
            // 
            labelOperation.AutoSize = true;
            labelOperation.Location = new Point(223, 128);
            labelOperation.Name = "labelOperation";
            labelOperation.Size = new Size(12, 15);
            labelOperation.TabIndex = 14;
            labelOperation.Text = "_";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(141, 392);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 16;
            label3.Text = "extract to";
            // 
            // txtExtractTo
            // 
            txtExtractTo.Location = new Point(223, 388);
            txtExtractTo.Name = "txtExtractTo";
            txtExtractTo.Size = new Size(351, 23);
            txtExtractTo.TabIndex = 15;
            // 
            // numCompressionLevel
            // 
            numCompressionLevel.Location = new Point(583, 126);
            numCompressionLevel.Maximum = new decimal(new int[] { 12, 0, 0, 0 });
            numCompressionLevel.Name = "numCompressionLevel";
            numCompressionLevel.Size = new Size(75, 23);
            numCompressionLevel.TabIndex = 17;
            numCompressionLevel.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // btnTest
            // 
            btnTest.Location = new Point(687, 153);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(75, 23);
            btnTest.TabIndex = 18;
            btnTest.Text = "Test";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            // 
            // btnRepair
            // 
            btnRepair.Location = new Point(687, 184);
            btnRepair.Name = "btnRepair";
            btnRepair.Size = new Size(75, 23);
            btnRepair.TabIndex = 19;
            btnRepair.Text = "Repair";
            btnRepair.UseVisualStyleBackColor = true;
            btnRepair.Click += btnRepair_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(141, 362);
            label4.Name = "label4";
            label4.Size = new Size(53, 15);
            label4.TabIndex = 21;
            label4.Text = "input zip";
            // 
            // txtInputZip
            // 
            txtInputZip.Location = new Point(223, 359);
            txtInputZip.Name = "txtInputZip";
            txtInputZip.Size = new Size(351, 23);
            txtInputZip.TabIndex = 20;
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(141, 426);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(12, 15);
            labelStatus.TabIndex = 22;
            labelStatus.Text = "_";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(labelStatus);
            Controls.Add(label4);
            Controls.Add(txtInputZip);
            Controls.Add(btnRepair);
            Controls.Add(btnTest);
            Controls.Add(numCompressionLevel);
            Controls.Add(label3);
            Controls.Add(txtExtractTo);
            Controls.Add(labelOperation);
            Controls.Add(label2);
            Controls.Add(txtSourceFolder);
            Controls.Add(label1);
            Controls.Add(btnExtract);
            Controls.Add(btnUpdate);
            Controls.Add(btnBuild);
            Controls.Add(btnCancel);
            Controls.Add(txtOutputZip);
            Controls.Add(labelCurrentFile);
            Controls.Add(progressBar1);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)numCompressionLevel).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private ProgressBar progressBar1;
        private Label labelCurrentFile;
        private TextBox txtOutputZip;
        private Button btnCancel;
        private Button btnBuild;
        private Button btnUpdate;
        private Button btnExtract;
        private Label label1;
        private Label label2;
        private TextBox txtSourceFolder;
        private Label labelOperation;
        private Label label3;
        private TextBox txtExtractTo;
        private NumericUpDown numCompressionLevel;
        private Button btnTest;
        private Button btnRepair;
        private Label label4;
        private TextBox txtInputZip;
        private Label labelStatus;
    }
}
