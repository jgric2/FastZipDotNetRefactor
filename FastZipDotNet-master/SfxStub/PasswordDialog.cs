using BrutalZip2025.BrutalControls;

namespace SfxStub
{
    public partial class PasswordDialog : ModernForm
    {
        public PasswordDialog(Image banner = null, Icon icon = null, string title = null, string info = null)
        {
            InitializeComponent();
            if (banner != null) picBanner.Image = banner;
            if (icon != null) this.Icon = icon;
            if (!string.IsNullOrWhiteSpace(title)) this.Text = title + " — Password";
            if (!string.IsNullOrWhiteSpace(info)) lblInfo.Text = info;
        }

        private void PasswordDialog_Load(object sender, EventArgs e)
        {

        }
    }
}