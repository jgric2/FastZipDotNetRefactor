namespace Brutal_Zip
{
    public partial class InputDialog : Form
    {
        public string Prompt { get => lblPrompt.Text; set => lblPrompt.Text = value ?? ""; }
        public string ValueText { get => txtValue.Text; set => txtValue.Text = value ?? ""; }

        public InputDialog() { InitializeComponent(); }
    }
}
