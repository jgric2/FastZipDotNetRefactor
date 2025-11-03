using BrutalZip2025.BrutalControls;

namespace Brutal_Zip
{
    public partial class PasswordDialog : ModernForm
    {
        public string Password => txtPassword.Text;

        private PasswordCrackContext _crackContext;

        public PasswordDialog()
        {
            InitializeComponent();

            checkBoxShowPassword.Checked = !txtPassword.UseSystemPasswordChar;
        }

        public void SetCrackContext(PasswordCrackContext ctx)
        {
            _crackContext = ctx;
            btnCrack.Visible = (_crackContext != null);
        }

        private void btnCrack_Click(object sender, EventArgs e)
        {
            if (_crackContext == null) return;
            using var frm = new CrackPasswordForm(_crackContext);
            var r = frm.ShowDialog(this);
            if (r == DialogResult.OK && !string.IsNullOrEmpty(frm.FoundPassword))
            {
                txtPassword.Text = frm.FoundPassword;
                txtPassword.SelectAll();
                txtPassword.Focus();
            }
        }

        private void checkBoxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
        
        }

        private void PasswordDialog_Load(object sender, EventArgs e)
        {

        }

        private void chkEncrypt_CheckedChanged(object sender, EventArgs e)
        {
             txtPassword.UseSystemPasswordChar = !checkBoxShowPassword.Checked;
        }
    }
}
