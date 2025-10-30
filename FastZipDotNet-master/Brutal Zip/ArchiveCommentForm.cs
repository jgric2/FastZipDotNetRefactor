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
    public partial class ArchiveCommentForm : Form
    {
        public string Comment
        {
            get => txtComment.Text;
            set => txtComment.Text = value ?? string.Empty;
        }

        public ArchiveCommentForm()
        {
            InitializeComponent();
            txtComment.TextChanged += (_, __) => UpdateCount();
            btnOK.Click += (_, __) =>
            {
                if (!ValidateSize())
                {
                    DialogResult = DialogResult.None;
                }
            };
        }

        private void UpdateCount()
        {
            // EOCD comment is a ushort length; Close writes UTF-8 bytes.
            int bytes = Encoding.UTF8.GetByteCount(txtComment.Text ?? string.Empty);
            lblCount.Text = $"{bytes} / 65535 bytes";
        }

        private bool ValidateSize()
        {
            int bytes = Encoding.UTF8.GetByteCount(txtComment.Text ?? string.Empty);
            if (bytes > 65535)
            {
                MessageBox.Show(this, "Comment exceeds 65535 bytes in UTF-8. Please shorten.", "Too long",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
}
