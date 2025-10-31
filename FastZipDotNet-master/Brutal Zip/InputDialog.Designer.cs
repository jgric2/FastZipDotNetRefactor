using System.ComponentModel;

namespace Brutal_Zip
{
    partial class InputDialog
    {
        private IContainer components = null;
        private Label lblPrompt;
        private TextBox txtValue;
        private Button btnOK;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            lblPrompt = new Label();
            txtValue = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(520, 140);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Input";
            AcceptButton = btnOK;
            CancelButton = btnCancel;

            lblPrompt.Text = "Enter value:";
            lblPrompt.Location = new Point(12, 16);
            lblPrompt.Size = new Size(496, 20);

            txtValue.Location = new Point(12, 40);
            txtValue.Size = new Size(496, 23);

            btnOK.Text = "OK";
            btnOK.Location = new Point(340, 88);
            btnOK.Size = new Size(80, 28);
            btnOK.DialogResult = DialogResult.OK;

            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(428, 88);
            btnCancel.Size = new Size(80, 28);
            btnCancel.DialogResult = DialogResult.Cancel;

            Controls.Add(lblPrompt);
            Controls.Add(txtValue);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}