namespace BrutalZip2025.BrutalControls
{
    partial class BrutalTextBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.innerTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

            // 
            // innerTextBox
            // 
            this.innerTextBox.BorderStyle = BorderStyle.None;
            this.innerTextBox.Location = new Point(5, 5);
            this.innerTextBox.Width = this.Width - 10;
            this.innerTextBox.Height = this.Height - 10;
            this.innerTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.innerTextBox.Multiline = false; // Default to single-line
            this.innerTextBox.BackColor = Color.Transparent;
            this.innerTextBox.ForeColor = textGradientEnabled ? Color.White : textColor;
            this.innerTextBox.TextChanged += InnerTextBox_TextChanged;
            this.innerTextBox.GotFocus += InnerTextBox_GotFocus;
            this.innerTextBox.LostFocus += InnerTextBox_LostFocus;
            this.innerTextBox.KeyDown += InnerTextBox_KeyDown;
            this.innerTextBox.KeyPress += InnerTextBox_KeyPress;
            this.innerTextBox.KeyUp += InnerTextBox_KeyUp;

            // 
            // CustomTextBox
            // 
            this.Controls.Add(this.innerTextBox);
            this.BackColor = Color.FromArgb(25, 25, 25);
            this.Size = new Size(200, 30);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
