using BrutalZip2025.BrutalControls;

namespace Brutal_Zip
{
    public partial class InputDialog : ModernForm
    {
        public string Prompt { get => lblPrompt.Text; set => lblPrompt.Text = value ?? ""; }
        public string ValueText { get => txtValue.Text; set => txtValue.Text = value ?? ""; }

        public InputDialog() { InitializeComponent(); }

        private void InputDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
