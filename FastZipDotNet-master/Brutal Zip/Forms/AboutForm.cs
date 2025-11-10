using BrutalZip2025.BrutalControls;
using System.Diagnostics;
using System.Reflection;

namespace Brutal_Zip
{
    public partial class AboutForm : ModernForm
    {
        public AboutForm()
        {
            InitializeComponent();

            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version?.ToString() ?? "1.0.0.0";
            lblVersion.Text = "Version " + ver;

            lnkSite.LinkClicked += (s, e) =>
            {
                try { Process.Start(new ProcessStartInfo(lnkSite.Text) { UseShellExecute = true }); }
                catch { }
            };
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {

        }
    }
}
