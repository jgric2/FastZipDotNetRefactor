using Brutal_Zip.Classes.Helpers;
using Microsoft.Web.WebView2.Core;
using ScintillaNET;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace Brutal_Zip.Views
{
    public partial class PreviewPane : UserControl
    {
        [DllImport("user32.dll")] 
        private static extern bool DestroyCaret();

        [DllImport("user32.dll")] 
        private static extern bool HideCaret(IntPtr hWnd);





        public PreviewPane()
        {
            InitializeComponent();


            btnFindNext.Click += (_, __) => DoFind();
            btnCopyPreview.Click += (_, __) => DoCopy();
            btnSaveAs.Click += (_, __) => DoSaveAs();
            chkHex.CheckedChanged += (_, __) => ToggleHex();

            btnOpenExternal.Click += (s, e) => OpenExternal();
            btnPlayPause.Click += (s, e) => TogglePlay();
            btnStop.Click += (s, e) => Stop();

            scintilla.Enter += (_, __) => SetScintillaFocus(true);
            scintilla.Leave += (_, __) => SetScintillaFocus(false);
            scintilla.VisibleChanged += (_, __) => { if (!scintilla.Visible) SetScintillaFocus(false); };
            scintilla.HandleDestroyed += (_, __) => { try { DestroyCaret(); } catch { } };

            ConfigureScintillaDefaults();
            
            //scintilla.Dispose();
        }

        private void SetScintillaFocus(bool hasFocus)
        {
            try
            {
                // SCI_SETFOCUS = 2380
                scintilla.DirectMessage(2380, new IntPtr(hasFocus ? 1 : 0), IntPtr.Zero); 
                if (!hasFocus) 
                { 
                    // Make sure any stray OS caret is gone.
                    try 
                    { 
                        HideCaret(scintilla.Handle); 
                    } 
                    catch { } 
                    try
                    { 
                        DestroyCaret(); 
                    } catch { } 
                }
            }
            catch
            {

            }
        }

        private string _currentFilePath;
        private long _currentSize;
        private PreviewKind _kind = PreviewKind.None;
        private bool _mediaPaused = true;

        private string _mappedHost = "localfiles";
        private string _mappedFolder;
        private RichTextBox hexBox;  // NEW hex viewer

        private enum PreviewKind { None, Image, Text, Code, Media }

        public void Clear()
        {
            _currentFilePath = null;
            _currentSize = 0;
            _kind = PreviewKind.None;
            lblFileName.Text = "-";
            lblInfo.Text = "";
            ShowStatus("No preview");
        }


        private void DoFind()
        {
            string q = txtFind.Text;
            if (string.IsNullOrEmpty(q)) return;

            if (txtPreview.Visible)
            {
                int start = txtPreview.SelectionStart + txtPreview.SelectionLength;
                int idx = txtPreview.Text.IndexOf(q, start, StringComparison.OrdinalIgnoreCase);
                if (idx < 0) idx = txtPreview.Text.IndexOf(q, 0, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0) { txtPreview.Select(idx, q.Length); txtPreview.ScrollToCaret(); }
            }
            else if (scintilla.Visible)
            {
                var text = scintilla.Text;
                int start = scintilla.CurrentPosition + 1;
                int idx = text.IndexOf(q, start >= text.Length ? 0 : start, StringComparison.OrdinalIgnoreCase);
                if (idx < 0) idx = text.IndexOf(q, 0, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    scintilla.SelectionStart = idx;
                    scintilla.SelectionEnd = idx + q.Length;
                    scintilla.ScrollCaret();
                }
            }
            else if (hexBox.Visible)
            {
                int start = hexBox.SelectionStart + hexBox.SelectionLength;
                int idx = hexBox.Text.IndexOf(q, start, StringComparison.OrdinalIgnoreCase);
                if (idx < 0) idx = hexBox.Text.IndexOf(q, 0, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0) { hexBox.Select(idx, q.Length); hexBox.ScrollToCaret(); }
            }
        }

        private void DoCopy()
        {
            try
            {
                if (txtPreview.Visible && !string.IsNullOrEmpty(txtPreview.SelectedText))
                    Clipboard.SetText(txtPreview.SelectedText);
                else if (scintilla.Visible)
                {
                    var sel = scintilla.SelectedText;
                    if (!string.IsNullOrEmpty(sel)) Clipboard.SetText(sel);
                }
                else if (hexBox.Visible && !string.IsNullOrEmpty(hexBox.SelectedText))
                    Clipboard.SetText(hexBox.SelectedText);
            }
            catch { }
        }

        private void DoSaveAs()
        {
            if (string.IsNullOrEmpty(_currentFilePath) || !File.Exists(_currentFilePath)) return;
            using var sfd = new SaveFileDialog { FileName = Path.GetFileName(_currentFilePath) };
            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                try { File.Copy(_currentFilePath, sfd.FileName, overwrite: true); }
                catch (Exception ex) { MessageBox.Show(this, ex.Message, "Save As"); }
            }
        }

        private void ToggleHex()
        {
            if (chkHex.Checked)
                _ = LoadHexAsync();
            else
                hexBox.Visible = false;
        }

      

        private async Task LoadHexAsync()
        {
            try
            {
                string p = _currentFilePath;
                if (string.IsNullOrEmpty(p) || !File.Exists(p))
                {
                    chkHex.Checked = false;
                    return;
                }

                // Limit to ~4MB to keep UI responsive; otherwise sample head+tail
                const int Max = 4 * 1024 * 1024;

                byte[] data;
                var fi = new FileInfo(p);
                if (fi.Length <= Max)
                {
                    data = await File.ReadAllBytesAsync(p);
                }
                else
                {
                    // Read first 2MB and last 2MB with gap marker
                    int half = Max / 2;
                    data = new byte[Max + 64];
                    int off = 0;
                    using var fs = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                    off += fs.Read(data, 0, half);
                    var marker = Encoding.ASCII.GetBytes("\r\n... (skipped) ...\r\n");
                    Buffer.BlockCopy(marker, 0, data, off, marker.Length); off += marker.Length;
                    fs.Seek(-half, SeekOrigin.End);
                    off += fs.Read(data, off, half);
                    Array.Resize(ref data, off);
                }

                var sb = new StringBuilder(data.Length * 3);
                int row = 0;
                for (int i = 0; i < data.Length; i += 16)
                {
                    sb.AppendFormat("{0:X8}  ", row);
                    int j;
                    for (j = 0; j < 16 && i + j < data.Length; j++)
                        sb.AppendFormat("{0:X2} ", data[i + j]);
                    for (; j < 16; j++) sb.Append("   ");
                    sb.Append(" ");
                    for (j = 0; j < 16 && i + j < data.Length; j++)
                    {
                        byte c = data[i + j];
                        sb.Append(c >= 32 && c < 127 ? (char)c : '.');
                    }
                    sb.AppendLine();
                    row += 16;
                }

                hexBox.Text = sb.ToString();
                ShowOnly(hexBox);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Hex View");
                chkHex.Checked = false;
            }
        }

        public void ShowStatus(string message, bool isError = false)
        {
            lblUnsupported.Text = message;
            lblUnsupported.ForeColor = isError ? Color.Firebrick : SystemColors.GrayText;
            ShowOnly(lblUnsupported);
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

            // NEW: Signature sniffers first so EXE/DLL/LIB always show info even if extension is odd
            if (await IsPeWinBinaryBySignatureAsync(path) || IsPeWinBinary(ext))
            {
                _kind = PreviewKind.Text;
                await ShowPeInfoAsync(path);
                return;
            }
            if (await IsLibArchiveBySignatureAsync(path) || IsLibArchive(ext))
            {
                _kind = PreviewKind.Text;
                await ShowLibInfoAsync(path);
                return;
            }

            // existing detection paths (image, code, text, media)...
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
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete,
                    1 << 20, FileOptions.SequentialScan);
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
                txtPreview.ForeColor = Color.White;
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
            // VS-like dark palette
            var back = Color.FromArgb(30, 30, 30);            // editor background
            var text = Color.FromArgb(212, 212, 212);         // default text
            var marginBack = Color.FromArgb(37, 37, 38);      // margin background
            var marginFore = Color.FromArgb(128, 128, 128);   // line number text
            var caret = Color.White;                          // caret color
            var caretLine = Color.FromArgb(45, 45, 48);       // current line background
            var selection = Color.FromArgb(38, 79, 120);      // selection blue
            var selectionText = Color.White;
            var indentGuide = Color.FromArgb(52, 52, 52);
            var edge = Color.FromArgb(60, 60, 60);

            // Reset styles to defaults, then apply default font/colors
            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.Styles[Style.Default].BackColor = back;
            scintilla.Styles[Style.Default].ForeColor = text;
            scintilla.StyleClearAll(); // apply to all styles

            // DirectWrite for better text on dark backgrounds
            scintilla.Technology = Technology.DirectWrite;

            // Selection
            scintilla.SetSelectionBackColor(true, selection);
            scintilla.SetSelectionForeColor(true, selectionText);

            // Caret + caret line
            scintilla.CaretForeColor = caret;
            scintilla.CaretLineVisible = true;
            scintilla.CaretLineBackColor = caretLine;
            scintilla.CaretStyle = CaretStyle.Line;
            scintilla.CaretWidth = 2;

            // Indentation
            scintilla.IndentWidth = 4;
            scintilla.UseTabs = false;
            scintilla.TabWidth = 4;

            // Indentation guides (when shown)
            scintilla.Styles[Style.IndentGuide].ForeColor = indentGuide;
            scintilla.Styles[Style.IndentGuide].BackColor = back;

            // Edge (column guide) — optional
            scintilla.EdgeMode = EdgeMode.Line;
            scintilla.EdgeColumn = 120;
            scintilla.EdgeColor = edge;

            // Line numbers margin
            scintilla.Margins[0].Type = MarginType.Number;
            scintilla.Margins[0].Width = 40;
            scintilla.Styles[Style.LineNumber].BackColor = marginBack;
            scintilla.Styles[Style.LineNumber].ForeColor = marginFore;

            // Marker margin (folding)
            scintilla.Margins[2].Type = MarginType.Symbol;
            scintilla.Margins[2].Mask = Marker.MaskFolders;
            scintilla.Margins[2].Sensitive = true;
            scintilla.Margins[2].Width = 16;
            scintilla.SetFoldMarginColor(true, marginBack);
            scintilla.SetFoldMarginHighlightColor(true, marginBack);

            // Folding markers in a dark style
            var foldFore = Color.FromArgb(220, 220, 220);
            var foldBack = Color.FromArgb(60, 60, 60);

            scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            for (int i = Marker.Folder; i <= Marker.FolderTail; i++)
            {
                scintilla.Markers[i].SetForeColor(foldFore);
                scintilla.Markers[i].SetBackColor(foldBack);
            }

            // No wrap in code by default
            scintilla.WrapMode = WrapMode.None;

            // You can also tweak caret/selection transparency if needed:
            // scintilla.SelectionAlpha = 255; // opaque
        }


        private const int SCI_SETLEXERLANGUAGE = 4001;
        private void SetLexerLanguage(string lang)
        {
            var prop = typeof(Scintilla).GetProperty("LexerName");
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(scintilla, lang ?? string.Empty);
                return;
            }
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.StringToHGlobalAnsi(lang ?? string.Empty);
                scintilla.DirectMessage(SCI_SETLEXERLANGUAGE, IntPtr.Zero, ptr);
            }
            finally
            {
                if (ptr != IntPtr.Zero) Marshal.FreeHGlobal(ptr);
            }
        }


        private void ApplyLexerForExtension(string ext)
        {
            ext = ext.ToLowerInvariant();

            string lexer = "null"; // default to plain text

            if (IsCStyle(ext))
            {
                lexer = "cpp";
            }
            else if (IsXml(ext))
            {
                lexer = "xml";
            }
            else if (IsJson(ext))
            {
                lexer = "json";
            }
            else if (IsIni(ext))
            {
                // INI/.cfg uses 'props' in Lexilla
                lexer = "props";
            }
            else if (IsScript(ext))
            {
                if (ext == ".py") lexer = "python";
                else if (ext == ".rb") lexer = "ruby";
                else if (ext == ".ps1") lexer = "powershell"; // if not present in your build, it will just fall back to 'null'
                else if (ext == ".sh") lexer = "bash";        // if not present, fallback applies
                else lexer = "batch";                          // .bat/.cmd
            }

            SetLexerLanguage(lexer);

            // JSON: allow comments (Lexilla property)
            if (lexer == "json")
                scintilla.SetProperty("lexer.json.allow.comments", "1");

            // Provide some base keywords for C-like languages
            if (lexer == "cpp")
                scintilla.SetKeywords(0, string.Join(" ", GetCKeywords()));


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

            // Folding settings
            scintilla.SetProperty("fold", "1");
            scintilla.SetProperty("fold.compact", "1");
            scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);


            ApplyDarkSyntaxColorsForLexer(lexer);
        }


        private void ApplyDarkSyntaxColorsForLexer(string lexer)
        {
            // VS Code–like dark colors
            var fore = Color.FromArgb(212, 212, 212);   // default text
            var kw = Color.FromArgb(86, 156, 214);      // keywords
            var str = Color.FromArgb(206, 145, 120);    // strings
            var num = Color.FromArgb(181, 206, 168);    // numbers
            var com = Color.FromArgb(106, 153, 85);     // comments (green)
            var pp = Color.FromArgb(197, 134, 192);     // preprocessor / directives
            var op = Color.FromArgb(212, 212, 212);     // operators
            var attr = Color.FromArgb(156, 220, 254);   // xml attribute / tag-ish
            var tag = Color.FromArgb(86, 156, 214);     // xml tag
            var propName = Color.FromArgb(156, 220, 254); // json/property name

            // Start from default text for “unassigned” styles
            // (StyleClearAll already pushed default to all, so set only those we care about)

            if (lexer == "cpp")
            {
                scintilla.Styles[Style.Cpp.Comment].ForeColor = com;
                scintilla.Styles[Style.Cpp.CommentLine].ForeColor = com;
                scintilla.Styles[Style.Cpp.CommentDoc].ForeColor = com;

                scintilla.Styles[Style.Cpp.Number].ForeColor = num;
                scintilla.Styles[Style.Cpp.String].ForeColor = str;
                scintilla.Styles[Style.Cpp.Character].ForeColor = str;

                scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = pp;
                scintilla.Styles[Style.Cpp.Operator].ForeColor = op;
                scintilla.Styles[Style.Cpp.Word].ForeColor = kw; // keywords
                                                                 // Optional: user types/classes as Word2 if you set keywords(1)
                                                                 // scintilla.Styles[Style.Cpp.Word2].ForeColor = Color.FromArgb(78, 201, 176); // types
            }
            else if (lexer == "xml")
            {
                scintilla.Styles[Style.Xml.Comment].ForeColor = com;
                scintilla.Styles[Style.Xml.Tag].ForeColor = tag;
                scintilla.Styles[Style.Xml.TagEnd].ForeColor = tag;
                scintilla.Styles[Style.Xml.TagUnknown].ForeColor = tag;

                scintilla.Styles[Style.Xml.Attribute].ForeColor = attr;
                scintilla.Styles[Style.Xml.AttributeUnknown].ForeColor = attr;

                scintilla.Styles[Style.Xml.DoubleString].ForeColor = str;
                scintilla.Styles[Style.Xml.SingleString].ForeColor = str;

                scintilla.Styles[Style.Xml.Entity].ForeColor = num;
                scintilla.Styles[Style.Xml.Other].ForeColor = fore;
            }
            else if (lexer == "json")
            {
                // Keys (string keys often styled different from string values)
                scintilla.Styles[Style.Json.PropertyName].ForeColor = propName; // if present
                                                                                // Fallback commonly used:
                scintilla.Styles[Style.Json.String].ForeColor = str;
                scintilla.Styles[Style.Json.StringEol].ForeColor = str;
                scintilla.Styles[Style.Json.Number].ForeColor = num;
              //  scintilla.Styles[Style.Json.bool].ForeColor = kw;   // true/false
            //    scintilla.Styles[Style.Json.nu].ForeColor = kw;      // null
                scintilla.Styles[Style.Json.Default].ForeColor = fore;
                scintilla.Styles[Style.Json.BlockComment].ForeColor = com;
                scintilla.Styles[Style.Json.LineComment].ForeColor = com;
                scintilla.Styles[Style.Json.Operator].ForeColor = op;
                // Depending on your Scintilla5.NET build, style IDs may vary slightly;
                // if a style is missing, leaving it unassigned is fine.
            }
            else if (lexer == "props") // ini/cfg
            {
                scintilla.Styles[Style.Properties.Comment].ForeColor = com;
                scintilla.Styles[Style.Properties.Section].ForeColor = kw;     // [section]
                scintilla.Styles[Style.Properties.Assignment].ForeColor = op;   // '='
                scintilla.Styles[Style.Properties.Key].ForeColor = propName;    // key
             //   scintilla.Styles[Style.Properties.Value].ForeColor = str;       // value
            }
            else if (lexer == "batch")
            {
                scintilla.Styles[Style.Batch.Comment].ForeColor = com;
                scintilla.Styles[Style.Batch.Word].ForeColor = kw;
                scintilla.Styles[Style.Batch.Operator].ForeColor = op;
                scintilla.Styles[Style.Batch.Command].ForeColor = kw;
                scintilla.Styles[Style.Batch.Label].ForeColor = pp;
               // scintilla.Styles[Style.Batch.Variable].ForeColor = propName;
            }
            else if (lexer == "python")
            {
                scintilla.Styles[Style.Python.CommentLine].ForeColor = com;
                scintilla.Styles[Style.Python.Number].ForeColor = num;
                scintilla.Styles[Style.Python.String].ForeColor = str;
                scintilla.Styles[Style.Python.Character].ForeColor = str;
                scintilla.Styles[Style.Python.Word].ForeColor = kw;
                scintilla.Styles[Style.Python.Operator].ForeColor = op;
                scintilla.Styles[Style.Python.DefName].ForeColor = propName; // function/class name
            }
            else if (lexer == "ruby")
            {
                scintilla.Styles[Style.Ruby.CommentLine].ForeColor = com;
                scintilla.Styles[Style.Ruby.Number].ForeColor = num;
                scintilla.Styles[Style.Ruby.String].ForeColor = str;
                scintilla.Styles[Style.Ruby.Word].ForeColor = kw;
                scintilla.Styles[Style.Ruby.Operator].ForeColor = op;
                scintilla.Styles[Style.Ruby.Symbol].ForeColor = propName;
            }
            else
            {
                // "null" or unknown lexers use the default colors already set
            }
        }





        // ========== Utilities ==========
        private void ShowOnly(Control c)
        {
            picture.Visible = false;
            txtPreview.Visible = false;

            // Important: hide Scintilla before showing something else
            if (scintilla.Visible)
            {
                scintilla.Visible = false;      // triggers VisibleChanged => SetScintillaFocus(false)
            }

            lblUnsupported.Visible = false;
            webView.Visible = false;
            if (hexBox != null) hexBox.Visible = false;

            btnPlayPause.Enabled = (_kind == PreviewKind.Media && webView.CoreWebView2 != null);
            btnStop.Enabled = (_kind == PreviewKind.Media && webView.CoreWebView2 != null);

            if (c == null) lblUnsupported.Visible = true;
            else c.Visible = true;
        }

        private void ShowUnsupported()
        {
            _kind = PreviewKind.None;
            ShowStatus("Preview cannot be shown", isError: true);
        }



        private static bool IsPeWinBinary(string ext)
        {
            switch (ext)
            {
                case ".exe":
                case ".dll":
                case ".sys":
                case ".drv":
                case ".ocx":
                case ".scr":
                    return true;
                default: return false;
            }
        }

        private static bool IsLibArchive(string ext)
        {
            return string.Equals(ext, ".lib", StringComparison.OrdinalIgnoreCase);
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
                case ".fx":
                case ".gml":
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

        private async Task<bool> IsPeWinBinaryBySignatureAsync(string path)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read,  FileShare.ReadWrite | FileShare.Delete, 4096, FileOptions.SequentialScan);
                    using var br = new BinaryReader(fs);

                    // MZ
                    if (fs.Length < 0x40) return false;
                    fs.Seek(0, SeekOrigin.Begin);
                    ushort mz = br.ReadUInt16();
                    if (mz != 0x5A4D) return false; // 'M','Z'

                    // e_lfanew
                    fs.Seek(0x3C, SeekOrigin.Begin);
                    int peOffset = br.ReadInt32();
                    if (peOffset <= 0 || peOffset > fs.Length - 4) return false;

                    fs.Seek(peOffset, SeekOrigin.Begin);
                    uint sig = br.ReadUInt32();
                    return sig == 0x00004550; // 'P','E',0,0
                });
            }
            catch { return false; }
        }

        private async Task<bool> IsLibArchiveBySignatureAsync(string path)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read,  FileShare.ReadWrite | FileShare.Delete, 4096, FileOptions.SequentialScan);
                    if (fs.Length < 8) return false;
                    byte[] head = new byte[8];
                    int r = fs.Read(head, 0, 8);
                    if (r < 8) return false;
                    // "!<arch>\n"
                    return head[0] == 0x21 && head[1] == 0x3C && head[2] == 0x61 && head[3] == 0x72 &&
                    head[4] == 0x63 && head[5] == 0x68 && head[6] == 0x3E && head[7] == 0x0A;
                });
            }
            catch { return false; }
        }


        private async Task ShowPeInfoAsync(string path)
        {
            try
            {
                var sb = new StringBuilder();
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 1 << 20, FileOptions.SequentialScan);
                using var br = new BinaryReader(fs);

                // DOS header
                fs.Seek(0, SeekOrigin.Begin);
                ushort mz = br.ReadUInt16();
                if (mz != 0x5A4D) // 'MZ'
                {
                    sb.AppendLine("Not a valid PE file (missing MZ header).");
                    txtPreview.Text = sb.ToString();
                    ShowOnly(txtPreview);
                    return;
                }

                fs.Seek(0x3C, SeekOrigin.Begin);
                int peOffset = br.ReadInt32();

                if (peOffset <= 0 || peOffset > fs.Length - 256)
                {
                    sb.AppendLine("Invalid PE offset.");
                    txtPreview.Text = sb.ToString();
                    ShowOnly(txtPreview);
                    return;
                }

                fs.Seek(peOffset, SeekOrigin.Begin);
                uint peSig = br.ReadUInt32();
                if (peSig != 0x00004550) // 'PE\0\0'
                {
                    sb.AppendLine("Invalid PE signature.");
                    txtPreview.Text = sb.ToString();
                    ShowOnly(txtPreview);
                    return;
                }

                // IMAGE_FILE_HEADER
                ushort machine = br.ReadUInt16();
                ushort numSections = br.ReadUInt16();
                uint timeStamp = br.ReadUInt32();
                uint ptrToSym = br.ReadUInt32();
                uint numSyms = br.ReadUInt32();
                ushort sizeOpt = br.ReadUInt16();
                ushort characteristics = br.ReadUInt16();

                string arch = MachineToString(machine);
                DateTime ts = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp).ToLocalTime();

                // Optional header
                ushort magic = br.ReadUInt16();
                bool isPE32Plus = (magic == 0x20b);
                // read minimal interesting fields from Optional Header
                byte majorLinker = br.ReadByte();
                byte minorLinker = br.ReadByte();
                uint sizeOfCode = br.ReadUInt32();
                uint sizeOfInitializedData = br.ReadUInt32();
                uint sizeOfUninitializedData = br.ReadUInt32();
                uint addressOfEntryPoint = br.ReadUInt32();
                uint baseOfCode = br.ReadUInt32();
                ulong imageBase = isPE32Plus ? br.ReadUInt64() : br.ReadUInt32();

                // skip to subsystem/dll characteristics:
                // fields: SectionAlignment, FileAlignment, OS versions, image versions, subsystem versions...
                fs.Seek(isPE32Plus ? (peOffset + 24 + 2 + 2 + 4 + 4 + 4 + 4 + 4 + 8) : (peOffset + 24 + 2 + 2 + 4 + 4 + 4 + 4 + 4 + 4), SeekOrigin.Begin);
                // Actually safer to re-seek back after reading base fields.
                fs.Seek(peOffset + 24 + 2, SeekOrigin.Begin); // after Magic (2 bytes), we read above but safer to reposition to consistent offsets.

                // We'll parse more correctly by re-reading:
                fs.Seek(peOffset + 24, SeekOrigin.Begin);
                magic = br.ReadUInt16();
                isPE32Plus = (magic == 0x20b);
                br.ReadByte(); br.ReadByte(); // linker maj/min
                br.ReadUInt32(); br.ReadUInt32(); br.ReadUInt32(); // sizeCode, sizeInit, sizeUninit
                addressOfEntryPoint = br.ReadUInt32();
                baseOfCode = br.ReadUInt32();
                if (!isPE32Plus) br.ReadUInt32(); // baseOfData
                imageBase = isPE32Plus ? br.ReadUInt64() : br.ReadUInt32();
                uint sectionAlignment = br.ReadUInt32();
                uint fileAlignment = br.ReadUInt32();
                br.ReadUInt16(); br.ReadUInt16(); // OS ver
                br.ReadUInt16(); br.ReadUInt16(); // image ver
                br.ReadUInt16(); br.ReadUInt16(); // subsystem ver
                br.ReadUInt32(); // Win32VersionValue
                br.ReadUInt32(); // SizeOfImage
                br.ReadUInt32(); // SizeOfHeaders
                br.ReadUInt32(); // Checksum
                ushort subsystem = br.ReadUInt16();
                ushort dllChar = br.ReadUInt16();

                // Data directory count (always 16)
                uint sizeOfStackReserve = isPE32Plus ? (uint)br.ReadUInt64AsUInt32Safe() : br.ReadUInt32();
                if (isPE32Plus) br.ReadUInt64AsUInt32Safe(); else br.ReadUInt32(); // SizeOfStackCommit
                if (isPE32Plus) br.ReadUInt64AsUInt32Safe(); else br.ReadUInt32(); // SizeOfHeapReserve
                if (isPE32Plus) br.ReadUInt64AsUInt32Safe(); else br.ReadUInt32(); // SizeOfHeapCommit
                br.ReadUInt32(); // LoaderFlags
                uint numDataDirs = br.ReadUInt32();

                // Read data directories quickly – CLI header is index 14
                uint cliRva = 0, cliSize = 0;
                for (int i = 0; i < 16; i++)
                {
                    uint rva = br.ReadUInt32();
                    uint size = br.ReadUInt32();
                    if (i == 14) { cliRva = rva; cliSize = size; }
                }

                // Summary
                sb.AppendLine("Portable Executable (PE)");
                sb.AppendLine($"  Architecture     : {arch} ({(isPE32Plus ? "64-bit" : "32-bit")})");
                sb.AppendLine($"  Subsystem        : {SubsystemToString(subsystem)}");
                sb.AppendLine($"  EntryPoint RVA   : 0x{addressOfEntryPoint:X8}");
                sb.AppendLine($"  ImageBase        : 0x{imageBase:X}");
                sb.AppendLine($"  Sections         : {numSections}");
                sb.AppendLine($"  Timestamp        : {ts:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"  Characteristics  : 0x{characteristics:X4} {CharacteristicsFlagsToString(characteristics)}");
                sb.AppendLine($"  DLL Char         : 0x{dllChar:X4} {DllCharFlagsToString(dllChar)}");

                bool hasClr = (cliRva != 0 && cliSize != 0);
                sb.AppendLine($"  CLR/.NET         : {(hasClr ? "Yes" : "No")}");

                // Try versions
                try
                {
                    var fvi = FileVersionInfo.GetVersionInfo(path);
                    if (!string.IsNullOrEmpty(fvi.FileVersion))
                        sb.AppendLine($"  FileVersion      : {fvi.FileVersion}");
                    if (!string.IsNullOrEmpty(fvi.ProductVersion))
                        sb.AppendLine($"  ProductVersion   : {fvi.ProductVersion}");
                }
                catch { }

                if (hasClr)
                {
                    try
                    {
                        var asmName = AssemblyName.GetAssemblyName(path);
                        sb.AppendLine($"  AssemblyName     : {asmName.Name}");
                        sb.AppendLine($"  AssemblyVersion  : {asmName.Version}");
                    }
                    catch { /* not a managed assembly or cannot load */ }
                }

                // Try digital signature
                try
                {
                    var cert = X509Certificate2.CreateFromSignedFile(path);
                    if (cert != null)
                    {
                        sb.AppendLine("  Signed           : Yes");
                        sb.AppendLine($"    Subject        : {cert.Subject}");
                        sb.AppendLine($"    Issuer         : {cert.Issuer}");
                       // sb.AppendLine($"    Valid          : {cert.b:g} - {cert.NotAfter:g}");
                    }
                }
                catch
                {
                    sb.AppendLine("  Signed           : No/Unknown");
                }

                // Sections (basic)
                try
                {
                    // Jump to first section header
                    long secHeaders = peOffset + 24 + sizeOpt;
                    fs.Seek(secHeaders, SeekOrigin.Begin);
                    sb.AppendLine();
                    sb.AppendLine("Sections:");
                    int show = Math.Min((ushort)10, numSections);
                    for (int i = 0; i < show; i++)
                    {
                        byte[] nameBytes = br.ReadBytes(8);
                        string secName = Encoding.UTF8.GetString(nameBytes).TrimEnd('\0');
                        uint virtualSize = br.ReadUInt32();
                        uint virtualAddress = br.ReadUInt32();
                        uint sizeOfRawData = br.ReadUInt32();
                        uint ptrRawData = br.ReadUInt32();
                        br.ReadUInt32(); br.ReadUInt32(); br.ReadUInt32(); br.ReadUInt32(); // skip rest of section header (16 bytes: reloc/linenumbers/num/char)

                        sb.AppendLine($"  {secName,-8} VA=0x{virtualAddress:X8} VSZ=0x{virtualSize:X8} Raw={sizeOfRawData:N0}@0x{ptrRawData:X8}");
                    }
                }
                catch { }

                // Output
                txtPreview.Font = new Font("Consolas", 10f);
                txtPreview.Text = sb.ToString();
                ShowOnly(txtPreview);
            }
            catch (Exception ex)
            {
                txtPreview.Text = "Failed to parse PE: " + ex.Message;
                ShowOnly(txtPreview);
            }
        }

        private async Task ShowLibInfoAsync(string path)
        {
            try
            {
                var sb = new StringBuilder();
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 1 << 20, FileOptions.SequentialScan);
                using var br = new BinaryReader(fs);

                // Magic: "!<arch>\n"
                byte[] magic = br.ReadBytes(8);
                string mag = Encoding.ASCII.GetString(magic);
                if (mag != "!<arch>\n")
                {
                    sb.AppendLine("Not a COFF archive (.lib).");
                    txtPreview.Text = sb.ToString();
                    ShowOnly(txtPreview);
                    return;
                }

                sb.AppendLine("COFF Archive (.lib)");
                int memberCount = 0;
                var members = new List<string>();

                // Parse members
                while (fs.Position + 60 <= fs.Length)
                {
                    long hdrPos = fs.Position;
                    byte[] header = br.ReadBytes(60);
                    if (header.Length < 60) break;

                    string name = Encoding.ASCII.GetString(header, 0, 16).Trim();
                    string sizeStr = Encoding.ASCII.GetString(header, 48, 10).Trim();
                    if (!int.TryParse(sizeStr.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int size))
                        break;
                    // skip date/uid/gid/mode, etc.

                    string memberName = DecodeArchiveMemberName(name, br, fs);

                    memberCount++;
                    if (members.Count < 20) members.Add(memberName);

                    long next = Align2(hdrPos + 60 + size);
                    fs.Seek(next, SeekOrigin.Begin);
                }

                sb.AppendLine($"  Members: {memberCount:N0}");
                if (members.Count > 0)
                {
                    sb.AppendLine("  First members:");
                    foreach (var m in members)
                        sb.AppendLine("    " + m);
                }

                txtPreview.Font = new Font("Consolas", 10f);
                txtPreview.Text = sb.ToString();
                ShowOnly(txtPreview);
            }
            catch (Exception ex)
            {
                txtPreview.Text = "Failed to parse .lib: " + ex.Message;
                ShowOnly(txtPreview);
            }
        }

        private static long Align2(long v) => (v & 1) == 1 ? v + 1 : v;

        // Decode member name, handling short names, extended names “#1/len”, and SYSV "/nnnn"
        private string DecodeArchiveMemberName(string nameField, BinaryReader br, FileStream fs)
        {
            string name = nameField.Trim();

            // GNU extended name: "#1/nnn" – name of length nnn follows the header
            if (name.StartsWith("#1/"))
            {
                string lenStr = name.Substring(3).Trim();
                if (int.TryParse(lenStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n) && n > 0)
                {
                    byte[] nb = br.ReadBytes(n);
                    string nm = Encoding.ASCII.GetString(nb).TrimEnd('\0', '/');
                    return nm;
                }
                return "(extended)";
            }

            // System V longnames table: "/nnnn"
            if (name.StartsWith("/"))
            {
                // We don't parse the longnames string table here; return placeholder
                return name;
            }

            // Trim trailing '/'
            return name.TrimEnd('/');
        }

        private static string MachineToString(ushort m) => m switch
        {
            0x014C => "x86",
            0x8664 => "x64",
            0x01C0 => "ARM",
            0xAA64 => "ARM64",
            0x0200 => "IA64",
            0x01C4 => "ARMv7",
            _ => $"0x{m:X4}"
        };

        private static string SubsystemToString(ushort s) => s switch
        {
            1 => "Native",
            2 => "Windows GUI",
            3 => "Windows CUI",
            5 => "OS/2 CUI",
            7 => "POSIX CUI",
            9 => "Windows CE GUI",
            10 => "EFI Application",
            11 => "EFI Boot Service",
            12 => "EFI Runtime Driver",
            13 => "EFI ROM",
            14 => "XBOX",
            16 => "Windows Boot Application",
            _ => $"0x{s:X4}"
        };

        private static string CharacteristicsFlagsToString(ushort c)
        {
            var parts = new List<string>();
            if ((c & 0x0001) != 0) parts.Add("Relocs Stripped");
            if ((c & 0x0002) != 0) parts.Add("Executable");
            if ((c & 0x0004) != 0) parts.Add("LineNums Stripped");
            if ((c & 0x0008) != 0) parts.Add("LocalSyms Stripped");
            if ((c & 0x0020) != 0) parts.Add("Large Address Aware");
            if ((c & 0x2000) != 0) parts.Add("DLL");
            return string.Join(", ", parts);
        }

        private static string DllCharFlagsToString(ushort c)
        {
            var parts = new List<string>();
            if ((c & 0x0040) != 0) parts.Add("DynamicBase");
            if ((c & 0x0100) != 0) parts.Add("NX Compatible");
            if ((c & 0x4000) != 0) parts.Add("Guard CF");
            if ((c & 0x0020) != 0) parts.Add("High Entropy VA");
            return string.Join(", ", parts);
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