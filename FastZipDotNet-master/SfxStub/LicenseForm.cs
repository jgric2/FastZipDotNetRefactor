using BrutalZip2025.BrutalControls;

namespace SfxStub
{
    public partial class LicenseForm : ModernForm
    {
        public LicenseForm(Image banner = null, Icon icon = null, string title = null, string company = null, string licenseText = null)
        {
            InitializeComponent();

            if (banner != null) picBanner.Image = banner;
            if (icon != null) this.Icon = icon;
            if (!string.IsNullOrWhiteSpace(title))
                this.Text = title + " — License";
            if (!string.IsNullOrWhiteSpace(company))
                lblTitle.Text = $"Please review the {company} license agreement:";

            License = licenseText ?? "";
        }

        private void LicenseForm_Load(object sender, EventArgs e)
        {

        }
    }
}