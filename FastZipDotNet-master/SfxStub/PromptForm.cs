using BrutalZip2025.BrutalControls;

namespace SfxStub
{
    public partial class PromptForm : ModernForm
    {
        public PromptForm(Image banner = null, Icon icon = null, Color? theme = null, Color? themeEnd = null, string title = null, string company = null)
        {
            InitializeComponent();

            if (banner != null) picBanner.Image = banner;
            if (icon != null) this.Icon = icon;
            if (theme.HasValue) pnlTopAccent.StartColor = theme.Value;
            if (themeEnd.HasValue) pnlTopAccent.EndColor = themeEnd.Value;
            if (!string.IsNullOrWhiteSpace(title)) this.Text = title;
            if (!string.IsNullOrWhiteSpace(company)) lblCompany.Text = company;
        }

        public void SetFolder(string folder)
        {
            txtFolder.Text = folder ?? "";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (Directory.Exists(txtFolder.Text))
                fbd.SelectedPath = txtFolder.Text;
            if (fbd.ShowDialog(this) == DialogResult.OK)
                txtFolder.Text = fbd.SelectedPath;
        }

        private void PromptForm_Load(object sender, EventArgs e)
        {

        }
    }
}