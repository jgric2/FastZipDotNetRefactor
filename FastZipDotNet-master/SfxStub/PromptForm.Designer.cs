using System.ComponentModel;

namespace SfxStub
{
    partial class PromptForm
    {
        private IContainer components = null;

        private PictureBox picBanner;
        private Panel pnlTopAccent;
        private Label lblDest;
        private TextBox txtFolder;
        private Button btnBrowse;
        private Button btnOK;
        private Button btnCancel;
        private Label lblCompany;

        public string Folder => txtFolder.Text;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            picBanner = new PictureBox();
            pnlTopAccent = new Panel();
            lblDest = new Label();
            txtFolder = new TextBox();
            btnBrowse = new Button();
            btnOK = new Button();
            btnCancel = new Button();
            lblCompany = new Label();

            SuspendLayout();

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(640, 220);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Extract To";
            AcceptButton = btnOK;
            CancelButton = btnCancel;

            pnlTopAccent.Location = new Point(0, 0);
            pnlTopAccent.Size = new Size(640, 6);
            pnlTopAccent.BackColor = Color.DodgerBlue;

            picBanner.Location = new Point(0, 6);
            picBanner.Size = new Size(640, 80);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;

            lblCompany.Location = new Point(12, 90);
            lblCompany.Size = new Size(616, 20);
            lblCompany.Text = "";

            lblDest.Text = "Extract to:";
            lblDest.Location = new Point(12, 116);
            lblDest.Size = new Size(80, 20);

            txtFolder.Location = new Point(12, 140);
            txtFolder.Size = new Size(540, 23);

            btnBrowse.Text = "Browse…";
            btnBrowse.Location = new Point(558, 138);
            btnBrowse.Size = new Size(70, 26);
            btnBrowse.Click += btnBrowse_Click;

            btnOK.Text = "Extract";
            btnOK.Location = new Point(462, 178);
            btnOK.Size = new Size(80, 28);
            btnOK.DialogResult = DialogResult.OK;

            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(548, 178);
            btnCancel.Size = new Size(80, 28);
            btnCancel.DialogResult = DialogResult.Cancel;

            Controls.Add(pnlTopAccent);
            Controls.Add(picBanner);
            Controls.Add(lblCompany);
            Controls.Add(lblDest);
            Controls.Add(txtFolder);
            Controls.Add(btnBrowse);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);

            ResumeLayout(false);
            PerformLayout();
        }
    }
}