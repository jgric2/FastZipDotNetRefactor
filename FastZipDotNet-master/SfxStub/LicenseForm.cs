using System;
using System.Drawing;
using System.Windows.Forms;

namespace SfxStub
{
    public partial class LicenseForm : Form
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
    }
}