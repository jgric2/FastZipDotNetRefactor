namespace Brutal_Zip
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    partial class FindDialog
    {
        private IContainer components = null;
        private Label lblPattern;
        internal TextBox txtPattern;
        private CheckBox chkCase;
        private Label lblHint;
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

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Text = "Find in archive";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(420, 160);

            lblPattern = new Label();
            lblPattern.Text = "Name pattern:";
            lblPattern.Location = new Point(12, 16);
            lblPattern.Size = new Size(100, 20);

            txtPattern = new TextBox();
            txtPattern.Location = new Point(120, 14);
            txtPattern.Size = new Size(280, 23);
            txtPattern.Text = "*"; // default

            chkCase = new CheckBox();
            chkCase.Text = "Match case";
            chkCase.Location = new Point(120, 44);
            chkCase.Size = new Size(100, 20);

            lblHint = new Label();
            lblHint.Text = "Use * and ? wildcards. Example: *.dll;*.exe";
            lblHint.Location = new Point(120, 68);
            lblHint.Size = new Size(280, 20);

            btnOK = new Button();
            btnOK.Text = "Find";
            btnOK.Location = new Point(220, 108);
            btnOK.Size = new Size(80, 28);
            btnOK.DialogResult = DialogResult.OK;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(320, 108);
            btnCancel.Size = new Size(80, 28);
            btnCancel.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblPattern);
            this.Controls.Add(txtPattern);
            this.Controls.Add(chkCase);
            this.Controls.Add(lblHint);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }
}