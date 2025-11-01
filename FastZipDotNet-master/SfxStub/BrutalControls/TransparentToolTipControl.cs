using System.ComponentModel;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;

namespace BrutalCopy2025.Controls.BrutalControls
{
    public class TransparentToolTipControl : Control
    {
        // themable appearance:
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)][Category("Appearance")] public Color GradientTop { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)][Category("Appearance")] public Color GradientBottom { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)][Category("Appearance")] public Color BorderColor { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)][Category("Appearance")] public Color TextColor { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)][Category("Appearance")] public Font ToolTipFont { get; set; }
        [Category("Layout"), DefaultValue(6)]
        public int PaddingX { get; set; }
        [Category("Layout"), DefaultValue(4)]
        public int PaddingY { get; set; }

        // private state
        string _text = "";
        Timer _autoClose;

        public TransparentToolTipControl()
        {
            // we draw our own bits & support a transparent BackColor
            SetStyle(
              ControlStyles.UserPaint
             | ControlStyles.AllPaintingInWmPaint
             | ControlStyles.OptimizedDoubleBuffer
             | ControlStyles.SupportsTransparentBackColor,
              true);
            BackColor = Color.Transparent;

            // default “BrutalBar” look
            GradientTop = Color.FromArgb(160, Color.Green);
            GradientBottom = Color.FromArgb(60, Color.Green);
            BorderColor = Color.Lime;
            TextColor = Color.White;
            ToolTipFont = new Font("Segoe UI", 9f);
            PaddingX = 6;
            PaddingY = 4;

            Visible = false;
            _autoClose = new Timer();
            _autoClose.Tick += (s, e) => { _autoClose.Stop(); Hide(); };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _autoClose.Dispose();
                ToolTipFont.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Show a semi-transparent tooltip.  It will re-parent itself onto the top‐level Form
        /// so that it floats above *all* sibling controls.
        /// </summary>
        /// <param name="text">Multi‐line text (‘\n’ separated).</param>
        /// <param name="relativeTo">Any child control; we’ll FindForm() on it.</param>
        /// <param name="offsetX">X‐offset inside that control.</param>
        /// <param name="offsetY">Y‐offset inside that control.</param>
        /// <param name="durationMs">Auto‐close after this many ms (default=2000).</param>
        public void Show(
          string text,
          Control relativeTo,
          int offsetX,
          int offsetY,
          int durationMs = 2000)
        {
            if (relativeTo == null) throw new ArgumentNullException(nameof(relativeTo));
            var top = relativeTo.FindForm();
            if (top == null) throw new InvalidOperationException("Cannot find parent Form.");

            _text = text ?? "";

            // measure text
            var lines = _text.Split('\n');
            int maxW = 0;
            int lineH = TextRenderer.MeasureText("Wg", ToolTipFont).Height;
            foreach (var line in lines)
            {
                var sz = TextRenderer.MeasureText(line, ToolTipFont);
                if (sz.Width > maxW) maxW = sz.Width;
            }
            int totalH = lines.Length * lineH;

            this.Size = new Size(
              maxW + PaddingX * 2,
              totalH + PaddingY * 2
            );

            // compute screen→form‐client location
            Point screenPt = relativeTo.PointToScreen(new Point(offsetX, offsetY));
            Point clientPt = top.PointToClient(screenPt);
            this.Location = clientPt;

            // re‐parent to the Form if needed
            if (this.Parent != top)
            {
                this.Parent?.Controls.Remove(this);
                top.Controls.Add(this);
            }

            BringToFront();
            Visible = true;

            // auto close
            _autoClose.Stop();
            _autoClose.Interval = durationMs;
            _autoClose.Start();

            Invalidate();
        }

        public new void Hide()
        {
            Visible = false;
            _autoClose.Stop();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(0, 0, Width, Height);

            // 1) semi‐transparent gradient
            using (var br = new LinearGradientBrush(
                     r,
                     GradientTop,
                     GradientBottom,
                     LinearGradientMode.Vertical))
            {
                g.FillRectangle(br, r);
            }

            // 2) border
            using (var pen = new Pen(BorderColor))
                g.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);

            // 3) text
            var textRect = new Rectangle(
              r.X + PaddingX,
              r.Y + PaddingY,
              r.Width - PaddingX * 2,
              r.Height - PaddingY * 2);
            TextRenderer.DrawText(
              g,
              _text,
              ToolTipFont,
              textRect,
              TextColor,
              TextFormatFlags.WordBreak
            );

            base.OnPaint(e);
        }

        /// <summary>
        /// Quick helper so you can mirror your bar’s SetColours(...)
        /// </summary>
        public void SetColours(
          Color gradTop,
          Color gradBottom,
          Color border,
          Color text)
        {
            GradientTop = Color.FromArgb(60,gradTop);
            GradientBottom = Color.FromArgb(60, gradBottom);
            BorderColor = border;
            TextColor = text;
        }
    }
}
