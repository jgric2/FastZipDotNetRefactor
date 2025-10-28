using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace BrutalCopy2025.Controls.BrutalControls
{
    public class GradientMenuStrip : MenuStrip
    {
        // Top strip gradient
        [Category("Appearance")] public Color StripGradientStart { get; set; } = Color.FromArgb(40, 40, 40);
        [Category("Appearance")] public Color StripGradientEnd { get; set; } = Color.FromArgb(20, 20, 20);
        [Category("Appearance")] public LinearGradientMode StripGradientMode { get; set; } = LinearGradientMode.Vertical;

        // Drop-down gradient
        [Category("Appearance")] public Color DropDownGradientStart { get; set; } = Color.FromArgb(32, 32, 32);
        [Category("Appearance")] public Color DropDownGradientEnd { get; set; } = Color.FromArgb(12, 12, 12);
        [Category("Appearance")] public LinearGradientMode DropDownGradientMode { get; set; } = LinearGradientMode.Vertical;

        // Item highlight
        [Category("Appearance")] public int ItemCornerRadius { get; set; } = 4;
        [Category("Appearance")] public Color ItemHoverStart { get; set; } = Color.FromArgb(50, 50, 50);
        [Category("Appearance")] public Color ItemHoverEnd { get; set; } = Color.FromArgb(30, 30, 30);
        [Category("Appearance")] public Color ItemPressedStart { get; set; } = Color.FromArgb(70, 70, 70);
        [Category("Appearance")] public Color ItemPressedEnd { get; set; } = Color.FromArgb(45, 45, 45);
        [Category("Appearance")] public LinearGradientMode ItemHighlightMode { get; set; } = LinearGradientMode.Vertical;
        [Category("Appearance")] public Color ItemBorderColor { get; set; } = Color.FromArgb(90, 90, 90);

        // Text
        [Category("Appearance")] public Color ItemTextColor { get; set; } = Color.White;
        [Category("Appearance")] public Color ItemSelectedTextColor { get; set; } = Color.White;
        [Category("Appearance")] public Color ItemDisabledTextColor { get; set; } = Color.Gray;

        // Arrow
        [Category("Appearance")] public bool ArrowUseGradient { get; set; } = true;
        [Category("Appearance")] public Color ArrowColor { get; set; } = Color.Gainsboro;
        [Category("Appearance")] public Color ArrowGradientStart { get; set; } = Color.Gainsboro;
        [Category("Appearance")] public Color ArrowGradientEnd { get; set; } = Color.DimGray;
        [Category("Appearance")] public LinearGradientMode ArrowGradientMode { get; set; } = LinearGradientMode.Vertical;

        // Separators / borders / image margin
        [Category("Appearance")] public Color SeparatorColor { get; set; } = Color.FromArgb(90, 90, 90);
        [Category("Appearance")] public Color DropDownBorderColor { get; set; } = Color.FromArgb(70, 70, 70);
        [Category("Appearance")] public bool UseImageMarginGradient { get; set; } = true;
        [Category("Appearance")] public Color ImageMarginGradientStart { get; set; } = Color.FromArgb(28, 28, 28);
        [Category("Appearance")] public Color ImageMarginGradientEnd { get; set; } = Color.FromArgb(18, 18, 18);
        [Category("Appearance")] public LinearGradientMode ImageMarginGradientMode { get; set; } = LinearGradientMode.Vertical;

        private readonly GradientRenderer _renderer;

        public GradientMenuStrip()
        {
            DoubleBuffered = true;
            _renderer = new GradientRenderer(this);
            // Do not set RenderMode = Custom (designer forbids it).
            // Setting Renderer to a custom renderer is the correct way:
            Renderer = _renderer;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // If something changed the renderer (e.g., Application.RenderWithVisualStyles),
            // set ours back. Do not touch RenderMode.
            if (Renderer != _renderer) Renderer = _renderer;
        }

        protected override void OnRendererChanged(EventArgs e)
        {
            base.OnRendererChanged(e);
            // keep our renderer active, but avoid designer loops
            if (!DesignMode && Renderer != _renderer) Renderer = _renderer;
        }

        // Fallback to ensure the top strip shows a gradient even if a foreign renderer sneaks in
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using var br = new LinearGradientBrush(ClientRectangle, StripGradientStart, StripGradientEnd, StripGradientMode)
            { WrapMode = WrapMode.TileFlipXY };
            e.Graphics.FillRectangle(br, ClientRectangle);
        }

        private sealed class GradientRenderer : ToolStripRenderer
        {
            private readonly GradientMenuStrip _o;
            public GradientRenderer(GradientMenuStrip owner) { _o = owner; }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                var r = e.AffectedBounds;
                if (r.Width <= 0 || r.Height <= 0) return;

                if (e.ToolStrip is MenuStrip)
                {
                    using var br = new LinearGradientBrush(r, _o.StripGradientStart, _o.StripGradientEnd, _o.StripGradientMode)
                    { WrapMode = WrapMode.TileFlipXY };
                    e.Graphics.FillRectangle(br, r);
                }
                else if (e.ToolStrip is ToolStripDropDown)
                {
                    using var br = new LinearGradientBrush(r, _o.DropDownGradientStart, _o.DropDownGradientEnd, _o.DropDownGradientMode)
                    { WrapMode = WrapMode.TileFlipXY };
                    e.Graphics.FillRectangle(br, r);
                }
                else
                {
                    base.OnRenderToolStripBackground(e);
                }
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                if (e.ToolStrip is ToolStripDropDown)
                {
                    using var p = new Pen(_o.DropDownBorderColor);
                    var r = e.AffectedBounds; r.Width -= 1; r.Height -= 1;
                    e.Graphics.DrawRectangle(p, r);
                }
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                var g = e.Graphics;
                var it = e.Item;
                bool top = it.Owner is MenuStrip;
                bool sel = it.Selected;
                bool prs = it.Pressed;

                if (!(sel || prs)) return;

                Rectangle r = top ? it.Bounds : it.ContentRectangle;
                r = Rectangle.Inflate(r, top ? -2 : -1, top ? -2 : -1);

                using var br = new LinearGradientBrush(r,
                    prs ? _o.ItemPressedStart : _o.ItemHoverStart,
                    prs ? _o.ItemPressedEnd : _o.ItemHoverEnd,
                    _o.ItemHighlightMode)
                { WrapMode = WrapMode.TileFlipXY };

                using var path = Rounded(r, _o.ItemCornerRadius);
                using var pen = new Pen(_o.ItemBorderColor);

                var old = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(br, path);
                g.DrawPath(pen, path);
                g.SmoothingMode = old;
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                bool disabled = !e.Item.Enabled;
                bool selected = e.Item.Selected || e.Item.Pressed;
                e.TextColor = disabled ? _o.ItemDisabledTextColor
                            : selected ? _o.ItemSelectedTextColor
                            : _o.ItemTextColor;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                using var p = new Pen(_o.SeparatorColor);
                var r = new Rectangle(Point.Empty, e.Item.Size);
                if (e.Vertical)
                {
                    int x = r.Width / 2;
                    e.Graphics.DrawLine(p, x, 2, x, r.Height - 4);
                }
                else
                {
                    int y = r.Height / 2;
                    e.Graphics.DrawLine(p, 2, y, r.Width - 4, y);
                }
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                var r = e.AffectedBounds;
                if (_o.UseImageMarginGradient)
                {
                    using var br = new LinearGradientBrush(r, _o.ImageMarginGradientStart, _o.ImageMarginGradientEnd, _o.ImageMarginGradientMode)
                    { WrapMode = WrapMode.TileFlipXY };
                    e.Graphics.FillRectangle(br, r);
                }
                else
                {
                    using var b = new SolidBrush(_o.ImageMarginGradientStart);
                    e.Graphics.FillRectangle(b, r);
                }
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                var g = e.Graphics;
                var r = e.ArrowRectangle;
                var pts = ArrowPoints(r, e.Direction);

                if (_o.ArrowUseGradient)
                {
                    var bounds = TriangleBounds(pts);
                    using var br = new LinearGradientBrush(bounds, _o.ArrowGradientStart, _o.ArrowGradientEnd, _o.ArrowGradientMode)
                    { WrapMode = WrapMode.TileFlipXY };
                    g.FillPolygon(br, pts);
                }
                else
                {
                    using var b = new SolidBrush(_o.ArrowColor);
                    g.FillPolygon(b, pts);
                }
            }

            // helpers
            private static GraphicsPath Rounded(Rectangle rect, int radius)
            {
                int r = Math.Max(0, Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2));
                if (r == 0) { var p0 = new GraphicsPath(); p0.AddRectangle(rect); return p0; }
                float d = r * 2f;
                var p = new GraphicsPath();
                p.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                p.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                p.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                p.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                p.CloseFigure();
                return p;
            }

            private static Point[] ArrowPoints(Rectangle rect, ArrowDirection dir)
            {
                int cx = rect.Left + rect.Width / 2;
                int cy = rect.Top + rect.Height / 2;
                int half = Math.Max(3, Math.Min(rect.Width, rect.Height) / 4);
                return dir switch
                {
                    ArrowDirection.Left => new[] { new Point(cx + half, cy - half), new Point(cx + half, cy + half), new Point(cx - half, cy) },
                    ArrowDirection.Right => new[] { new Point(cx - half, cy - half), new Point(cx - half, cy + half), new Point(cx + half, cy) },
                    ArrowDirection.Up => new[] { new Point(cx - half, cy + half), new Point(cx + half, cy + half), new Point(cx, cy - half) },
                    _ => new[] { new Point(cx - half, cy - half), new Point(cx + half, cy - half), new Point(cx, cy + half) },
                };
            }

            private static Rectangle TriangleBounds(Point[] pts)
            {
                int minX = pts.Min(p => p.X), maxX = pts.Max(p => p.X);
                int minY = pts.Min(p => p.Y), maxY = pts.Max(p => p.Y);
                return Rectangle.FromLTRB(minX, minY, maxX, maxY);
            }
        }

        // Optional: force repaint for all menus when changing a palette at runtime
        public void InvalidateAll()
        {
            Invalidate();
            foreach (var root in Items.OfType<ToolStripMenuItem>())
                InvalidateTree(root);
        }
        private void InvalidateTree(ToolStripMenuItem mi)
        {
            mi.Invalidate();
            if (mi.HasDropDownItems)
            {
                mi.DropDown.Invalidate();
                foreach (var c in mi.DropDownItems.OfType<ToolStripMenuItem>())
                    InvalidateTree(c);
            }
        }
    }
}
