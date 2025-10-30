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
using ScintillaNET;

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

            ConfigureScintillaDefaults();
        }

        private string _currentFilePath;
        private long _currentSize;
        private PreviewKind _kind = PreviewKind.None;
        private bool _mediaPaused = true;

        private string _mappedHost = "localfiles";
        private string _mappedFolder;

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

        // ========== Media via WebView2 ==========

        private async Task EnsureWebViewAsync()
        {
            if (webView.CoreWebView2 != null) return;

            await webView.EnsureCoreWebView2Async(null);
            var s = webView.CoreWebView2.Settings;
            s.AreDefaultContextMenusEnabled = false;
            s.AreDevToolsEnabled = false;
            s.IsStatusBarEnabled = false;

            webView.CoreWebView2.NavigationCompleted += (s2, e2) =>
            {
                if (_kind == PreviewKind.Media)
                {
                    btnPlayPause.Enabled = true;
                    btnStop.Enabled = true;
                }
            };
        }

        private async Task ShowMediaAsync(string path)
        {
            try
            {
                await EnsureWebViewAsync();

                string folder = Path.GetDirectoryName(path) ?? ".";
                if (!string.Equals(folder, _mappedFolder, StringComparison.OrdinalIgnoreCase))
                {
                    _mappedFolder = folder;
                    webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                        _mappedHost,
                        folder,
                        CoreWebView2HostResourceAccessKind.Allow);
                }

                string fileName = Path.GetFileName(path);
                string src = $"https://{_mappedHost}/{Uri.EscapeDataString(fileName)}";

                string html = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
<meta http-equiv='X-UA-Compatible' content='IE=edge'/>
<style>
html,body {{ margin:0; padding:0; background:#000; height:100%; overflow:hidden; }}
#v {{ width:100%; height:100%; background:#000; }}
</style>
</head>
<body>
<video id='v' src='{HttpUtility.HtmlAttributeEncode(src)}' controls style='background:#000;'></video>
<script>
    window.playPause = function() {{
        var v=document.getElementById('v'); if(!v) return 'Play';
        if (v.paused) {{ v.play(); return 'Pause'; }} else {{ v.pause(); return 'Play'; }}
    }};
    window.stopVideo = function(){{
        var v=document.getElementById('v'); if(!v) return true;
        v.pause(); v.currentTime = 0; return true;
    }};
</script>
</body>
</html>";

                btnPlayPause.Enabled = false;
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

        private async void TogglePlay()
        {
            if (_kind != PreviewKind.Media || webView.CoreWebView2 == null)
            {
                OpenExternal();
                return;
            }

            try
            {
                string result = await webView.CoreWebView2.ExecuteScriptAsync("window.playPause && window.playPause();");
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

        // ========== Images ==========
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

        // ========== Plain Text ==========
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

        // ========== Code via Scintilla ==========
        private async Task ShowCodeAsync(string path, string ext)
        {
            try
            {
                // For very large files, use plain text fallback (>4 MB)
                if (_currentSize > 4 * 1024 * 1024)
                {
                    await ShowTextAsync(path);
                    return;
                }

                string text = await Task.Run(() => File.ReadAllText(path));

                scintilla.ReadOnly = false;
                scintilla.Text = text;
                ApplyLexerForExtension(ext);
                scintilla.ReadOnly = true;

                ShowOnly(scintilla);
            }
            catch
            {
                ShowUnsupported();
            }
        }

        private void ConfigureScintillaDefaults()
        {
            // Reset styles to defaults
            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.Styles[Style.Default].BackColor = Color.White;
            scintilla.Styles[Style.Default].ForeColor = Color.Black;
            scintilla.StyleClearAll();

            // Line numbers margin
            scintilla.Margins[0].Type = MarginType.Number;
            scintilla.Margins[0].Width = 36;

            // Folding margin
            scintilla.Margins[2].Type = MarginType.Symbol;
            scintilla.Margins[2].Mask = Marker.MaskFolders;
            scintilla.Margins[2].Sensitive = true;
            scintilla.Margins[2].Width = 16;

            // Configure folding markers
            scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            var folderColor = Color.Gray;
            for (int i = Marker.Folder; i <= Marker.FolderTail; i++)
            {
                scintilla.Markers[i].SetForeColor(Color.White);
                scintilla.Markers[i].SetBackColor(folderColor);
            }

            // Indentation
            scintilla.IndentWidth = 4;
            scintilla.UseTabs = false;
            scintilla.TabWidth = 4;

            // Caret
            scintilla.CaretLineVisible = true;
            scintilla.CaretLineBackColor = Color.FromArgb(245, 245, 255);
            scintilla.WrapMode = WrapMode.None;
        }

        private void ApplyLexerForExtension(string ext)
        {
            ext = ext.ToLowerInvariant();

            if (IsCStyle(ext))
            {
                scintilla.Lexer = Lexer.Cpp;
                scintilla.SetKeywords(0, string.Join(" ",
                    GetCKeywords())); // base keywords for C/C#/C++ style
            }
            else if (IsXml(ext))
            {
                scintilla.Lexer = Lexer.Xml;
            }
            else if (IsJson(ext))
            {
                // Scintilla has a JSON lexer in newer Lexilla; otherwise JavaScript is acceptable
                try { scintilla.Lexer = Lexer.Json; }
                catch { scintilla.Lexer = Lexer.Xml; }
                scintilla.SetProperty("lexer.json.allow.comments", "1");
            }
            else if (IsIni(ext))
            {
                // Properties/Ini
                scintilla.Lexer = Lexer.Properties;
            }
            //else if (IsYaml(ext))
            //{
            //    // Not all SciLexer builds include YAML; fallback to Null if not available
            //    try { scintilla.Lexer = Lexer.Html; }
            //    catch { scintilla.Lexer = Lexer.Null; }
            //}
            else if (IsScript(ext))
            {
                if (ext == ".py") scintilla.Lexer = Lexer.Python;
                else if (ext == ".rb") scintilla.Lexer = Lexer.Ruby;
                else if (ext == ".ps1") scintilla.Lexer = Lexer.Batch; // no native PowerShell in older builds
                else if (ext == ".sh") scintilla.Lexer = Lexer.Null;   // if unavailable: try .Null
                else scintilla.Lexer = Lexer.Batch;
            }
            else
            {
                scintilla.Lexer = Lexer.Null; // plain text fallback
            }

            // Basic coloring styles (optional fine-tuning per lexer)
            scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.Green;
            scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.Green;
            scintilla.Styles[Style.Cpp.CommentDoc].ForeColor = Color.Green;
            scintilla.Styles[Style.Cpp.Number].ForeColor = Color.DarkCyan;
            scintilla.Styles[Style.Cpp.String].ForeColor = Color.Brown;
            scintilla.Styles[Style.Cpp.Character].ForeColor = Color.Brown;
            scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Gray;
            scintilla.Styles[Style.Cpp.Operator].ForeColor = Color.Black;
            scintilla.Styles[Style.Cpp.Word].ForeColor = Color.RoyalBlue; // keywords

            // Fold properties
            scintilla.SetProperty("fold", "1");
            scintilla.SetProperty("fold.compact", "1");
            scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }

        // ========== Utilities ==========
        private void ShowOnly(Control c)
        {
            picture.Visible = false;
            txtPreview.Visible = false;
            scintilla.Visible = false;
            lblUnsupported.Visible = false;
            webView.Visible = false;

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

        private static IEnumerable<string> GetCKeywords() => new[]
        {
            "using","namespace","class","struct","enum","public","private","protected","internal",
            "static","readonly","const","void","int","long","float","double","decimal","bool","string","char",
            "new","return","if","else","switch","case","break","continue","for","foreach","while","do",
            "try","catch","finally","throw","null","true","false","var","this","base"
        };

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
        private static bool IsCode(string ext)
        {
            return IsCStyle(ext) || IsXml(ext) || IsJson(ext) || IsIni(ext) || IsYaml(ext) || IsScript(ext);
        }
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
        private static bool IsXml(string ext) => ext.Equals(".xml", StringComparison.OrdinalIgnoreCase)
                                              || ext.Equals(".html", StringComparison.OrdinalIgnoreCase)
                                              || ext.Equals(".htm", StringComparison.OrdinalIgnoreCase);
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
            double s = bytes; int i = 0;
            while (s >= 1024 && i < u.Length - 1) { s /= 1024; i++; }
            return $"{s:F1} {u[i]}";
        }

        private void lblInfo_Click(object sender, EventArgs e)
        {

        }
    }
}