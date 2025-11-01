namespace BrutalCopy2025.Controls.BrutalControls
{
    partial class BrutalForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            brutalGradientPanel1 = new BrutalZip2025.BrutalControls.BrutalGradientPanel();
            panelControlBox = new Panel();
            btnMinimize = new Button();
            btnClose = new Button();
            brutalGradientPanel1.SuspendLayout();
            panelControlBox.SuspendLayout();
            SuspendLayout();
            // 
            // brutalGradientPanel1
            // 
            brutalGradientPanel1.Controls.Add(panelControlBox);
            brutalGradientPanel1.Dock = DockStyle.Top;
            brutalGradientPanel1.EndColor = Color.Black;
            brutalGradientPanel1.Location = new Point(0, 0);
            brutalGradientPanel1.Name = "brutalGradientPanel1";
            brutalGradientPanel1.Size = new Size(800, 34);
            brutalGradientPanel1.StartColor = Color.FromArgb(29, 181, 82);
            brutalGradientPanel1.TabIndex = 1;
            // 
            // panelControlBox
            // 
            panelControlBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panelControlBox.BackColor = Color.Transparent;
            panelControlBox.Controls.Add(btnMinimize);
            panelControlBox.Controls.Add(btnClose);
            panelControlBox.Location = new Point(907, -1);
            panelControlBox.Margin = new Padding(4, 3, 4, 3);
            panelControlBox.Name = "panelControlBox";
            panelControlBox.Size = new Size(122, 35);
            panelControlBox.TabIndex = 42;
            // 
            // btnMinimize
            // 
            btnMinimize.Dock = DockStyle.Right;
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatAppearance.MouseDownBackColor = Color.FromArgb(44, 44, 44, 66);
            btnMinimize.FlatAppearance.MouseOverBackColor = Color.FromArgb(66, 66, 66, 22);
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.Font = new Font("Segoe UI", 9F);
            btnMinimize.ForeColor = Color.White;
            btnMinimize.Location = new Point(0, 0);
            btnMinimize.Margin = new Padding(4, 3, 4, 3);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(61, 35);
            btnMinimize.TabIndex = 0;
            btnMinimize.Text = "─";
            btnMinimize.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            btnClose.BackgroundImageLayout = ImageLayout.Center;
            btnClose.Dock = DockStyle.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseDownBackColor = Color.Red;
            btnClose.FlatAppearance.MouseOverBackColor = Color.DarkRed;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 9F);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(61, 0);
            btnClose.Margin = new Padding(4, 3, 4, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(61, 35);
            btnClose.TabIndex = 2;
            btnClose.Text = "✕";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // BrutalForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(brutalGradientPanel1);
            Name = "BrutalForm";
            Text = "BrutalForm";
            Load += BrutalForm_Load;
            brutalGradientPanel1.ResumeLayout(false);
            panelControlBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BrutalZip2025.BrutalControls.BrutalGradientPanel brutalGradientPanel1;
        private Panel panelControlBox;
        private Button btnMinimize;
        private Button btnClose;
    }
}