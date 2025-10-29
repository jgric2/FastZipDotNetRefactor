﻿
namespace Brutal_Zip
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public partial class FindDialog : Form
    {
        public string Pattern => txtPattern.Text.Trim();
        public bool MatchCase => chkCase.Checked;

        public FindDialog()
        {
            InitializeComponent();
        }

        public static Regex BuildRegex(string patterns, bool matchCase)
        {
            // Split by ; or , into multiple patterns: e.g., "*.dll;*.exe"
            var parts = patterns.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => p.Trim()).Where(p => p.Length > 0);

            string WildToRegex(string w)
            {
                var rx = "^" + Regex.Escape(w).Replace("\\*", ".*").Replace("\\?", ".") + "$";
                return rx;
            }

            string combined = string.Join("|", parts.Select(WildToRegex));
            var opts = matchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
            return new Regex(combined.Length > 0 ? combined : ".*", opts);
        }
    }
}
