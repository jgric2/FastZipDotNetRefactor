using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brutal_Zip
{
    public partial class AboutForm : Form
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
    }
}
