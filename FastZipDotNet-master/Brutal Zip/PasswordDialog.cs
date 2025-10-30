using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brutal_Zip
{
    public partial class PasswordDialog : Form
    {
        public string Password => txtPassword.Text;
        public PasswordDialog() { InitializeComponent(); }
    }
}
