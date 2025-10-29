using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace Brutal_Zip.Views
{
    public partial class PreviewPane : UserControl
    {
        public PreviewPane()
        {
            InitializeComponent();
            btnOpenExternal.Click += (s, e) => OpenExternal();
            btnPlayPause.Click += (s, e) => TogglePlay();
            btnStop.Click += (s, e) => Stop();
        }

        private string _currentFilePath;
        private long _currentSize;
        private PreviewKind _kind = PreviewKind.None;
        private bool _mediaPaused = true;

        private string _mappedHost = "localfiles";
        private string _mappedFolder;   // to avoid remapping on every preview


        private enum PreviewKind { None, Image, Text, Code, Media }

        public void Clear()
        {
            _currentFilePath = null;
            _currentSize = 0;
            _kind = PreviewKind.None;
            lblFileName.Text = "-";
            lblInfo.Text = "";
            ShowOnly(null);
        }

        public async Task ShowFileAsync(string path)
        {
            Clear();
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) { ShowUnsupported(); return; }

            _currentFilePath = path;
            try { _currentSize = new FileInfo(path).Length; } catch { _currentSize = 0; }

            lblFileName.Text = Path.GetFileName(path);
            lblInfo.Text = $"{FormatBytes(_currentSize)}    {path}";

            string ext = Path.GetExtension(path).ToLowerInvariant();

            if (IsImage(ext))
            {
                _kind = PreviewKind.Image;
                await ShowImageAsync(path);
                return;
            }

            if (IsCode(ext))
            {
                _kind = PreviewKind.Code;
                await ShowCodeAsync(path, ext);
                return;
            }

            if (IsText(ext) && _currentSize <= 8 * 1024 * 1024)
            {
                _kind = PreviewKind.Text;
                await ShowTextAsync(path);
                return;
            }

            if (IsMedia(ext))
            {
                _kind = PreviewKind.Media;
                await ShowMediaAsync(path);
                return;
            }

            ShowUnsupported();
        }

        private async Task ShowMediaAsync(string path)
        {
            try
            {
                await EnsureWebViewAsync();

                // Map the folder of the media file to a virtual HTTPS origin
                string folder = Path.GetDirectoryName(path) ?? ".";
                if (!string.Equals(folder, _mappedFolder, StringComparison.OrdinalIgnoreCase))
                {
                    _mappedFolder = folder;
                    // Allow WebView2 to serve any file in this folder via https://localfiles/
                    webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                        _mappedHost,
                        folder,
                        CoreWebView2HostResourceAccessKind.Allow);
                }

                // Build a safe url under the mapped host
                string fileName = Path.GetFileName(path);
                string src = $"https://{_mappedHost}/{Uri.EscapeDataString(fileName)}";

                string html = $@"
<!DOCTYPE html> <html> <head> <meta charset='utf-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'/> <style> html,body {{ margin:0; padding:0; background:#000; height:100%; overflow:hidden; }} #v {{ width:100%; height:100%; background:#000; }} </style> </head> <body> <video id='v' src='{src}' controls style='background:#000;'></video> <script> window.playPause = function() {{ var v=document.getElementById('v'); if(!v) return 'Play'; if (v.paused) {{ v.play(); return 'Pause'; }} else {{ v.pause(); return 'Play'; }} }}; window.stopVideo = function(){{ var v=document.getElementById('v'); if(!v) return true; v.pause(); v.currentTime = 0; return true; }}; </script> </body> </html>";
                btnPlayPause.Enabled = false; // will enable on NavigationCompleted
                btnStop.Enabled = false;
                _mediaPaused = true;
                btnPlayPause.Text = "Play";

                webView.CoreWebView2.NavigateToString(html);
                ShowOnly(webView);
            }
            catch
            {
                ShowUnsupported();
            }
        }
        private async Task EnsureWebViewAsync()
        {
            if (webView.CoreWebView2 != null) return;

            await webView.EnsureCoreWebView2Async(null);

            var s = webView.CoreWebView2.Settings;
            s.AreDefaultContextMenusEnabled = false;
            s.AreDevToolsEnabled = false;
            s.IsStatusBarEnabled = false;

            // Enable/disable buttons when the page is ready
            webView.CoreWebView2.NavigationCompleted += (s2, e2) =>
            {
                if (_kind == PreviewKind.Media)
                {
                    btnPlayPause.Enabled = true;
                    btnStop.Enabled = true;
                }
            };
        }

        private async void TogglePlay()
        {
            if (_kind != PreviewKind.Media || webView.CoreWebView2 == null)
            {
                OpenExternal();
                return;
            }

            try
            {
                // returns "Pause" or "Play"
                string result = await webView.CoreWebView2.ExecuteScriptAsync("window.playPause && window.playPause();");
                // result contains quotes from JSON-string serialization
                if (result != null && result.IndexOf("Pause", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _mediaPaused = false;
                    btnPlayPause.Text = "Pause";
                }
                else
                {
                    _mediaPaused = true;
                    btnPlayPause.Text = "Play";
                }
            }
            catch { }
        }

        private async void Stop()
        {
            if (_kind != PreviewKind.Media || webView.CoreWebView2 == null) return;
            try
            {
                await webView.CoreWebView2.ExecuteScriptAsync("window.stopVideo && window.stopVideo();");
                _mediaPaused = true;
                btnPlayPause.Text = "Play";
            }
            catch { }
        }

        private void OpenExternal()
        {
            if (!string.IsNullOrEmpty(_currentFilePath) && File.Exists(_currentFilePath))
            {
                try { Process.Start(new ProcessStartInfo(_currentFilePath) { UseShellExecute = true }); }
                catch { }
            }
        }

        private async Task ShowImageAsync(string path)
        {
            try
            {
                var bmp = await Task.Run(() =>
                {
                    using var fs = File.OpenRead(path);
                    using var img = Image.FromStream(fs, false, false);
                    return new Bitmap(img);
                });
                if (IsDisposed) { bmp.Dispose(); return; }

                if (picture.Image != null) { var old = picture.Image; picture.Image = null; old.Dispose(); }
                picture.Image = bmp;
                ShowOnly(picture);
            }
            catch { ShowUnsupported(); }
        }

        private async Task ShowTextAsync(string path)
        {
            try
            {
                string text = await Task.Run(() => File.ReadAllText(path));
                txtPreview.Clear();
                txtPreview.Text = text;
                txtPreview.Font = new Font("Consolas", 10f);
                txtPreview.ForeColor = Color.Black;
                ShowOnly(txtPreview);
            }
            catch { ShowUnsupported(); }
        }

        private async Task ShowCodeAsync(string path, string ext)
        {
            try
            {
                string text = await Task.Run(() => File.ReadAllText(path));
                txtPreview.Clear();
                txtPreview.Text = text;
                txtPreview.Font = new Font("Consolas", 10f);
                txtPreview.ForeColor = Color.Black;

                if (text.Length <= 2_000_000) // 2MB limit for fast preview coloring
                    ApplySyntaxColoring(ext, text);

                ShowOnly(txtPreview);
            }
            catch { ShowUnsupported(); }
        }

        private void ShowOnly(Control c)
        {
            picture.Visible = false;
            txtPreview.Visible = false;
            lblUnsupported.Visible = false;
            webView.Visible = false;

            // Only enable Play/Stop in Media mode; otherwise leave them enabled if you want “Open” to work
            btnPlayPause.Enabled = (_kind == PreviewKind.Media && webView.CoreWebView2 != null);
            btnStop.Enabled = (_kind == PreviewKind.Media && webView.CoreWebView2 != null);

            if (c == null)
            {
                lblUnsupported.Visible = true;
            }
            else
            {
                c.Visible = true;
            }
        }

        private void ShowUnsupported()
        {
            _kind = PreviewKind.None;
            ShowOnly(null);
        }

        // Lightweight syntax coloring
        private void ApplySyntaxColoring(string ext, string text)
        {
            int selStart = txtPreview.SelectionStart;
            int selLen = txtPreview.SelectionLength;
            txtPreview.SuspendLayout();
            try
            {
                txtPreview.SelectAll();
                txtPreview.SelectionColor = Color.Black;

                if (IsCStyle(ext))
                {
                    Colorize(@"""(?:\\.|[^""\\])*""", Color.Brown); // strings
                    Colorize(@"//.*?$", Color.ForestGreen, RegexOptions.Multiline); // line comments
                    Colorize(@"/\*.*?\*/", Color.ForestGreen, RegexOptions.Singleline); // block comments
                    ColorizeKeywords(GetCKeywords(), Color.RoyalBlue);
                }
                else if (IsXml(ext))
                {
                    Colorize(@"</?[\w\-\:]+", Color.RoyalBlue);
                    Colorize(@"""[^""]*""", Color.Brown);
                    Colorize(@"<!--.*?-->", Color.ForestGreen, RegexOptions.Singleline);
                }
                else if (IsJson(ext))
                {
                    Colorize(@"""[^""]*""(?=\s*:)", Color.RoyalBlue);
                    Colorize(@"""(?:\\.|[^""\\])*""", Color.Brown);
                    Colorize(@"\b(true|false|null)\b", Color.MediumVioletRed);
                    Colorize(@"//.*?$", Color.ForestGreen, RegexOptions.Multiline);
                    Colorize(@"/\*.*?\*/", Color.ForestGreen, RegexOptions.Singleline);
                }
                else if (IsIni(ext) || IsYaml(ext))
                {
                    Colorize(@"^\s*#.*?$", Color.ForestGreen, RegexOptions.Multiline);
                    Colorize(@"^\s*;.*?$", Color.ForestGreen, RegexOptions.Multiline);
                    Colorize(@".*?\:", Color.RoyalBlue);
                    Colorize(@"""[^""]*""", Color.Brown);
                }
                else if (IsScript(ext))
                {
                    Colorize(@"#.*?$", Color.ForestGreen, RegexOptions.Multiline);
                    Colorize(@"//.*?$", Color.ForestGreen, RegexOptions.Multiline);
                    Colorize(@""".*?""|'.*?'", Color.Brown);
                    ColorizeKeywords(GetScriptKeywords(ext), Color.RoyalBlue);
                }

                void Colorize(string pattern, Color color, RegexOptions opts = RegexOptions.None)
                {
                    foreach (Match m in Regex.Matches(text, pattern, opts))
                    {
                        txtPreview.Select(m.Index, m.Length);
                        txtPreview.SelectionColor = color;
                    }
                }

                void ColorizeKeywords(IEnumerable<string> kws, Color color)
                {
                    foreach (var kw in kws)
                    {
                        foreach (Match m in Regex.Matches(text, $@"\b{Regex.Escape(kw)}\b"))
                        {
                            txtPreview.Select(m.Index, m.Length);
                            txtPreview.SelectionColor = color;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                txtPreview.SelectionStart = selStart;
                txtPreview.SelectionLength = selLen;
                txtPreview.ResumeLayout();
            }
        }

        private static IEnumerable<string> GetCKeywords() => new[]
        {
        "using","namespace","class","struct","enum","public","private","protected","internal",
        "static","readonly","const","void","int","long","float","double","decimal","bool","string","char",
        "new","return","if","else","switch","case","break","continue","for","foreach","while","do",
        "try","catch","finally","throw","null","true","false","var","this","base"
    };

        private static IEnumerable<string> GetScriptKeywords(string ext)
        {
            if (ext.Equals(".ps1", StringComparison.OrdinalIgnoreCase))
                return new[] { "param", "function", "return", "if", "else", "switch", "foreach", "while", "break", "continue", "trap", "try", "catch", "finally" };
            if (ext.Equals(".sh", StringComparison.OrdinalIgnoreCase))
                return new[] { "if", "then", "fi", "else", "elif", "case", "esac", "for", "while", "until", "do", "done", "function", "return" };
            if (ext.Equals(".bat", StringComparison.OrdinalIgnoreCase) || ext.Equals(".cmd", StringComparison.OrdinalIgnoreCase))
                return new[] { "echo", "set", "if", "else", "goto", "call", "for", "in", "do", "exit", "shift", "setlocal", "endlocal" };
            if (ext.Equals(".py", StringComparison.OrdinalIgnoreCase))
                return new[] { "def", "class", "return", "if", "elif", "else", "try", "except", "finally", "for", "while", "with", "import", "from", "as", "yield", "True", "False", "None" };
            if (ext.Equals(".rb", StringComparison.OrdinalIgnoreCase))
                return new[] { "def", "class", "module", "return", "if", "elsif", "else", "begin", "rescue", "ensure", "end", "while", "until", "for", "do", "yield", "true", "false", "nil" };
            return Array.Empty<string>();
        }

        private static bool IsImage(string ext)
        {
            switch (ext)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".gif":
                case ".tif":
                case ".tiff":
                case ".webp":
                case ".ico":
                    return true;
                default: return false;
            }
        }
        private static bool IsText(string ext)
        {
            switch (ext)
            {
                case ".txt":
                case ".log":
                case ".md":
                case ".csv":
                case ".json":
                case ".xml":
                case ".ini":
                case ".cfg":
                case ".yml":
                case ".yaml":
                case ".bat":
                case ".ps1":
                case ".sh":
                case ".rtf":
                    return true;
                default: return false;
            }
        }
        private static bool IsCode(string ext) => IsCStyle(ext) || IsXml(ext) || IsJson(ext) || IsIni(ext) || IsYaml(ext) || IsScript(ext);
        private static bool IsCStyle(string ext)
        {
            switch (ext)
            {
                case ".cs":
                case ".c":
                case ".h":
                case ".hpp":
                case ".hh":
                case ".cpp":
                case ".cc":
                case ".java":
                case ".js":
                case ".ts":
                case ".go":
                case ".rs":
                case ".swift":
                case ".kt":
                case ".m":
                case ".vb":
                    return true;
                default: return false;
            }
        }
        private static bool IsXml(string ext) => ext.Equals(".xml", StringComparison.OrdinalIgnoreCase) || ext.Equals(".html", StringComparison.OrdinalIgnoreCase) || ext.Equals(".htm", StringComparison.OrdinalIgnoreCase);
        private static bool IsJson(string ext) => ext.Equals(".json", StringComparison.OrdinalIgnoreCase);
        private static bool IsIni(string ext) => ext.Equals(".ini", StringComparison.OrdinalIgnoreCase) || ext.Equals(".cfg", StringComparison.OrdinalIgnoreCase);
        private static bool IsYaml(string ext) => ext.Equals(".yml", StringComparison.OrdinalIgnoreCase) || ext.Equals(".yaml", StringComparison.OrdinalIgnoreCase);
        private static bool IsScript(string ext)
        {
            switch (ext)
            {
                case ".ps1":
                case ".sh":
                case ".bat":
                case ".cmd":
                case ".py":
                case ".rb":
                    return true;
                default: return false;
            }
        }
        private static bool IsMedia(string ext)
        {
            switch (ext)
            {
                case ".mp4":
                case ".m4v":
                case ".mov":
                case ".webm":
                case ".mp3":
                case ".m4a":
                case ".wav":
                case ".ogg":
                    return true;
                default: return false;
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] u = { "B", "KB", "MB", "GB", "TB" };
            double s = bytes; int i = 0; while (s >= 1024 && i < u.Length - 1) { s /= 1024; i++; }
            return $"{s:F1} {u[i]}";
        }
    }
}