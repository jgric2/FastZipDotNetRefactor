using BrutalZip2025.BrutalControls;

namespace Brutal_Zip
{
    public partial class BuilderPreviewForm : ModernForm
    {
        public BuilderPreviewForm(string title, string company, Image banner, Icon icon, Color theme, Color themeEnd, bool showFileList)
        {
            InitializeComponent();

            if (banner != null) picBanner.Image = banner;
            Icon = icon ?? this.Icon;
            pnlTopAccent.StartColor = theme;
            pnlTopAccent.EndColor = themeEnd;
            lblTitle.Text = string.IsNullOrWhiteSpace(company) ? title : $"{company} — {title}";

        }

        private void BuilderPreviewForm_Load(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }
    }
}
