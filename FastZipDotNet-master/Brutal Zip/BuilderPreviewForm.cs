using BrutalZip2025.BrutalControls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Brutal_Zip
{
    public partial class BuilderPreviewForm : ModernForm
    {
        public BuilderPreviewForm(string title, string company, Image banner, Icon icon, Color theme, bool showFileList)
        {
            InitializeComponent();

            if (banner != null) picBanner.Image = banner;
            Icon = icon ?? this.Icon;
            pnlTopAccent.BackColor = theme;

            lblTitle.Text = string.IsNullOrWhiteSpace(company) ? title : $"{company} — {title}";

            //if (!showFileList)
            //{
            //    lvFiles.Visible = false;
            //    Height = 240;
            //}

            //// Populate a couple of dummy items
            //lvFiles.Items.Add(new ListViewItem(new[] { "file1.txt", "Done", "12.3 KB" }));
            //lvFiles.Items.Add(new ListViewItem(new[] { "file2.png", "Extracting…", "4.5 MB" }));
            //lvFiles.Items.Add(new ListViewItem(new[] { "bin/app.dll", "Waiting…", "" }));
        }

        private void BuilderPreviewForm_Load(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }
    }
}
