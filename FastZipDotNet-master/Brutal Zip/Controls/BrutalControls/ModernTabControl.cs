using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrutalCopy2025.Controls.BrutalControls
{
    [DesignerCategory("Code")]
    public class ModernTabControl : TabControl
    {
        private int _hotIndex = -1;

        // Appearance
        private Color _headerBackColor = Color.FromArgb(28, 28, 28);
        private Color _bodyBackColor = Color.FromArgb(25, 25, 25);
        private Color _selectedTabBackColor = Color.FromArgb(32, 32, 32);
        private Color _hoverBackColor = Color.FromArgb(36, 36, 36);
        private Color _borderColor = Color.FromArgb(55, 55, 55);
        private Color _inactiveTextColor = Color.FromArgb(200, 200, 200);
        private Color _selectedTextColor = Color.White;
        private Color _accentColor = Color.FromArgb(0, 120, 215);

        private bool _useSystemAccent = true;
        private bool _useAccentUnderline = true;
        private int _underlineThickness = 2;
        private Padding _headerTextPadding = new Padding(12, 0, 12, 0);

        // Behavior
        private bool _fillHeaderGaps = true;       // paint area behind/around tabs
        private bool _showHeaderSeparator = true;  // thin line between header and page
        private bool _suppressSystemBorder = true; // hide 3D border via styles and overpaint
        private bool _useExplorerTheme = true;     // flattens comctl look a bit

        public ModernTabControl()
        {
            // Optimized painting (UserPaint is not supported by TabControl)
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            DrawMode = TabDrawMode.OwnerDrawFixed;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(140, 36); // for Top/Bottom; for Left/Right set new Size(36, 140)
            Padding = new Point(16, 8);

            base.BackColor = _headerBackColor;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9f);

            if (TryGetSystemAccentColor(out var sysAccent))
                _accentColor = sysAccent;

            HandleCreated += (s, e) =>
            {
                if (_useExplorerTheme) try { SetWindowTheme(this.Handle, "Explorer", null); } catch { }
                UpdatePageColors();
            };

            ParentChanged += (s, e) => UpdatePageColors();
            ControlAdded += (s, e) => { if (e.Control is TabPage p) ConfigureTabPage(p); };

            MouseMove += (s, e) =>
            {
                int prevHot = _hotIndex;
                _hotIndex = -1;
                for (int i = 0; i < TabCount; i++)
                    if (GetTabRect(i).Contains(e.Location)) { _hotIndex = i; break; }
                if (prevHot != _hotIndex) Invalidate();
            };
            MouseLeave += (s, e) => { _hotIndex = -1; Invalidate(); };

            SystemColorsChanged += (s, e) =>
            {
                if (_useSystemAccent && TryGetSystemAccentColor(out var accent))
                    AccentColor = accent;
            };
        }

        // Flatten borders (removes WS_EX_CLIENTEDGE/WS_BORDER if the framework adds them)
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                if (_suppressSystemBorder)
                {
                    const int WS_EX_CLIENTEDGE = 0x00000200;
                    const int WS_BORDER = unchecked((int)0x00800000);
                    cp.ExStyle &= ~WS_EX_CLIENTEDGE;
                    cp.Style &= ~WS_BORDER;
                }
                return cp;
            }
        }

        // Public properties
        [Category("Appearance")]
        public Color HeaderBackColor
        {
            get => _headerBackColor;
            set { _headerBackColor = value; base.BackColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color BodyBackColor
        {
            get => _bodyBackColor;
            set { _bodyBackColor = value; UpdatePageColors(); Invalidate(); }
        }

        [Category("Appearance")]
        public Color SelectedTabBackColor
        {
            get => _selectedTabBackColor;
            set { _selectedTabBackColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color HoverBackColor
        {
            get => _hoverBackColor;
            set { _hoverBackColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color InactiveTextColor
        {
            get => _inactiveTextColor;
            set { _inactiveTextColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color SelectedTextColor
        {
            get => _selectedTextColor;
            set { _selectedTextColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool UseSystemAccentColor
        {
            get => _useSystemAccent;
            set
            {
                _useSystemAccent = value;
                if (_useSystemAccent && TryGetSystemAccentColor(out var accent))
                    AccentColor = accent;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool UseAccentUnderline
        {
            get => _useAccentUnderline;
            set { _useAccentUnderline = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(2)]
        public int UnderlineThickness
        {
            get => _underlineThickness;
            set { _underlineThickness = Math.Max(0, value); Invalidate(); }
        }

        [Category("Layout")]
        public Padding HeaderTextPadding
        {
            get => _headerTextPadding;
            set { _headerTextPadding = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool FillHeaderGaps
        {
            get => _fillHeaderGaps;
            set { _fillHeaderGaps = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowHeaderSeparator
        {
            get => _showHeaderSeparator;
            set { _showHeaderSeparator = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool SuppressSystemBorder
        {
            get => _suppressSystemBorder;
            set { _suppressSystemBorder = value; RecreateHandle(); }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool UseExplorerTheme
        {
            get => _useExplorerTheme;
            set { _useExplorerTheme = value; if (IsHandleCreated && value) try { SetWindowTheme(this.Handle, "Explorer", null); } catch { } Invalidate(); }
        }

        protected override bool ShowFocusCues => false;

        private void ConfigureTabPage(TabPage page)
        {
            page.UseVisualStyleBackColor = false;
            page.BackColor = _bodyBackColor;
            page.ForeColor = this.ForeColor;
            // Optional: tighten page padding for a modern look
            if (page.Padding.Top > 8 || page.Padding.Left > 8)
                page.Padding = new Padding(8);
        }

        private void UpdatePageColors()
        {
            foreach (TabPage page in this.TabPages)
                ConfigureTabPage(page);
        }

        // Draw tabs (text, icons, background)
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int index = e.Index;
            var page = this.TabPages[index];
            bool selected = (this.SelectedIndex == index);
            bool hot = (_hotIndex == index);

            Rectangle rect = GetTabRect(index);
            Color back = selected ? _selectedTabBackColor : (hot ? _hoverBackColor : _headerBackColor);

            using (var sb = new SolidBrush(back))
                g.FillRectangle(sb, rect);

            var textColor = selected ? _selectedTextColor : _inactiveTextColor;

            switch (Alignment)
            {
                case TabAlignment.Top:
                case TabAlignment.Bottom:
                    DrawHorizontalTab(g, rect, page, textColor, selected);
                    break;

                case TabAlignment.Left:
                case TabAlignment.Right:
                    DrawVerticalTab(g, rect, page, textColor, selected, Alignment == TabAlignment.Left);
                    break;
            }
        }

        private void DrawHorizontalTab(Graphics g, Rectangle rect, TabPage page, Color textColor, bool selected)
        {
            var content = Rectangle.Inflate(rect, -6, 0);
            content = new Rectangle(
                content.Left + _headerTextPadding.Left,
                content.Top,
                content.Width - (_headerTextPadding.Left + _headerTextPadding.Right),
                content.Height
            );

            int textLeft = content.Left;

            if (this.ImageList != null)
            {
                int imgIndex = page.ImageIndex;
                if (imgIndex >= 0 && imgIndex < this.ImageList.Images.Count)
                {
                    var img = this.ImageList.Images[imgIndex];
                    int imgY = content.Top + (content.Height - img.Height) / 2;
                    g.DrawImage(img, textLeft, imgY, img.Width, img.Height);
                    textLeft += img.Width + 6;
                }
            }

            var textBounds = new Rectangle(textLeft, rect.Top, content.Right - textLeft, rect.Height);
            TextRenderer.DrawText(
                g,
                page.Text,
                this.Font,
                textBounds,
                textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix
            );

            if (selected && _useAccentUnderline && _underlineThickness > 0)
            {
                int y = (Alignment == TabAlignment.Top) ? rect.Bottom - _underlineThickness : rect.Top;
                var underlineRect = new Rectangle(rect.Left + 10, y, rect.Width - 20, _underlineThickness);
                if (underlineRect.Width > 0)
                {
                    using (var sb = new SolidBrush(_accentColor))
                        g.FillRectangle(sb, underlineRect);
                }
            }
        }

        private void DrawVerticalTab(Graphics g, Rectangle rect, TabPage page, Color textColor, bool selected, bool isLeft)
        {
            // Accent bar on inner edge
            if (selected && _useAccentUnderline && _underlineThickness > 0)
            {
                var bar = isLeft
                    ? new Rectangle(rect.Left, rect.Top + 10, _underlineThickness, rect.Height - 20)
                    : new Rectangle(rect.Right - _underlineThickness, rect.Top + 10, _underlineThickness, rect.Height - 20);

                if (bar.Width > 0 && bar.Height > 0)
                {
                    using (var sb = new SolidBrush(_accentColor))
                        g.FillRectangle(sb, bar);
                }
            }

            // Rotate text/image so it reads horizontally
            var state = g.Save();
            if (isLeft)
            {
                g.TranslateTransform(rect.Left, rect.Bottom);
                g.RotateTransform(270);
            }
            else
            {
                g.TranslateTransform(rect.Right, rect.Top);
                g.RotateTransform(90);
            }

            var rr = new Rectangle(0, 0, rect.Height, rect.Width);
            rr = Rectangle.Inflate(rr, -6, 0);

            var content = new Rectangle(
                rr.Left + _headerTextPadding.Left,
                rr.Top,
                rr.Width - (_headerTextPadding.Left + _headerTextPadding.Right),
                rr.Height
            );

            int textLeft = content.Left;

            if (this.ImageList != null)
            {
                int imgIndex = page.ImageIndex;
                if (imgIndex >= 0 && imgIndex < this.ImageList.Images.Count)
                {
                    var img = this.ImageList.Images[imgIndex];
                    int imgY = content.Top + (content.Height - img.Height) / 2;
                    g.DrawImage(img, textLeft, imgY, img.Width, img.Height);
                    textLeft += img.Width + 6;
                }
            }

            using (var tb = new SolidBrush(textColor))
            {
                var textRect = new RectangleF(textLeft, rr.Top, content.Right - textLeft, rr.Height);
                var sf = new StringFormat(StringFormatFlags.NoWrap)
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                var oldHint = g.TextRenderingHint;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.DrawString(page.Text, this.Font, tb, textRect, sf);
                g.TextRenderingHint = oldHint;
            }

            g.Restore(state);
        }

        // Critical: stop the default 240,240,240 background erase and post-paint our header/body
        protected override void WndProc(ref Message m)
        {
            const int WM_ERASEBKGND = 0x0014;
            const int WM_PAINT = 0x000F;

            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    // Prevent Control color (240,240,240) erase; we'll paint everything in WM_PAINT overlay
                    m.Result = (IntPtr)1;
                    return;

                case WM_PAINT:
                    base.WndProc(ref m); // let the system + owner-draw run

                    // Post-paint overlays to kill white header/background and 3D borders
                    using (var g = Graphics.FromHwnd(this.Handle))
                    {
                        PaintHeaderBackgroundGaps(g); // fill header outside tabs
                        Hide3DBorderAndDrawSeparator(g);
                    }
                    return;

                default:
                    base.WndProc(ref m);
                    return;
            }
        }

        private void PaintHeaderBackgroundGaps(Graphics g)
        {
            if (!_fillHeaderGaps) return;

            Rectangle header = GetHeaderRectangle();
            if (header.Width <= 0 || header.Height <= 0) return;

            using (var rgn = new Region(header))
            {
                // Do not paint over tabs themselves
                for (int i = 0; i < this.TabCount; i++)
                    rgn.Exclude(GetTabRect(i));

                using (var sb = new SolidBrush(_headerBackColor))
                    g.FillRegion(sb, rgn);
            }
        }

        private void Hide3DBorderAndDrawSeparator(Graphics g)
        {
            if (!_suppressSystemBorder && !_showHeaderSeparator) return;

            var disp = this.DisplayRectangle;

            // Overpaint system's beveled page border with the body color (thickness 2 for safety)
            if (_suppressSystemBorder)
            {
                using (var pErase = new Pen(_bodyBackColor, 2))
                {
                    // Around display rectangle
                    // Top/Bottom/Left/Right edges right outside the page area
                    g.DrawLine(pErase, disp.Left - 1, disp.Top - 1, disp.Right + 1, disp.Top - 1);
                    g.DrawLine(pErase, disp.Left - 1, disp.Bottom, disp.Right + 1, disp.Bottom);
                    g.DrawLine(pErase, disp.Left - 1, disp.Top - 1, disp.Left - 1, disp.Bottom + 1);
                    g.DrawLine(pErase, disp.Right, disp.Top - 1, disp.Right, disp.Bottom + 1);
                }
            }

            // Draw a crisp 1px separator between header and page area
            if (_showHeaderSeparator)
            {
                using (var p = new Pen(_borderColor))
                {
                    switch (Alignment)
                    {
                        case TabAlignment.Top:
                            g.DrawLine(p, 0, disp.Top - 1, this.Width, disp.Top - 1);
                            break;
                        case TabAlignment.Bottom:
                            g.DrawLine(p, 0, disp.Bottom, this.Width, disp.Bottom);
                            break;
                        case TabAlignment.Left:
                            g.DrawLine(p, disp.Left - 1, 0, disp.Left - 1, this.Height);
                            break;
                        case TabAlignment.Right:
                            g.DrawLine(p, disp.Right, 0, disp.Right, this.Height);
                            break;
                    }
                }
            }
        }

        private Rectangle GetHeaderRectangle()
        {
            var disp = this.DisplayRectangle;

            switch (Alignment)
            {
                case TabAlignment.Top:
                    return new Rectangle(0, 0, this.Width, Math.Max(0, disp.Top));
                case TabAlignment.Bottom:
                    return new Rectangle(0, disp.Bottom, this.Width, Math.Max(0, this.Height - disp.Bottom));
                case TabAlignment.Left:
                    return new Rectangle(0, 0, Math.Max(0, disp.Left), this.Height);
                case TabAlignment.Right:
                    return new Rectangle(disp.Right, 0, Math.Max(0, this.Width - disp.Right), this.Height);
                default:
                    return Rectangle.Empty;
            }
        }

        // Accent color from OS
        private static bool TryGetSystemAccentColor(out Color color)
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6 && DwmGetColorizationColor(out uint colorization, out bool _) == 0)
                {
                    byte r = (byte)((colorization >> 16) & 0xFF);
                    byte g = (byte)((colorization >> 8) & 0xFF);
                    byte b = (byte)(colorization & 0xFF);
                    color = Color.FromArgb(255, r, g, b); // opaque
                    return true;
                }
            }
            catch { }
            color = Color.Empty;
            return false;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("dwmapi.dll")]
        private static extern int DwmGetColorizationColor(out uint pcrColorization, out bool pfOpaqueBlend);

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);
    }
}
