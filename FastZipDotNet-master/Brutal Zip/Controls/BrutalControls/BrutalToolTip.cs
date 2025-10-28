using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrutalCopy2025.Controls.BrutalControls
{
    public class BrutalToolTip : ToolTip
    {
        // Appearance props
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackGradientTop { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackGradientBottom { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TextColor { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font ToolTipFont { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int PaddingX { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int PaddingY { get; set; }

        // private state
        private string _currentText;

        public BrutalToolTip()
        {
            // owner-draw *and* turn OFF built-in fade/animation so the window is
            // a normal opaque HWND rather than a layered one.
            OwnerDraw = true;
            UseFading = false;
            UseAnimation = false;
            ShowAlways = true;

            // defaults
            BackGradientTop = Color.FromArgb(200, Color.DimGray);
            BackGradientBottom = Color.FromArgb(80, Color.Black);
            BorderColor = Color.Lime;
            TextColor = Color.White;
            ToolTipFont = new Font("Segoe UI", 9f);
            PaddingX = 6;
            PaddingY = 4;

            Popup += OnPopup;
            Draw += OnDraw;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Popup -= OnPopup;
                Draw -= OnDraw;
                ToolTipFont.Dispose();
            }
            base.Dispose(disposing);
        }

        // intercept Show so we know what text we’ll actually draw
        public new void Show(string text, IWin32Window w, int x, int y)
        {
            _currentText = text ?? "";
            base.Show(text, w, x, y);
        }
        public new void Show(string text, IWin32Window w, int x, int y, int duration)
        {
            _currentText = text ?? "";
            base.Show(text, w, x, y, duration);
        }

        // measure the text
        private void OnPopup(object s, PopupEventArgs e)
        {
            string txt = GetToolTip(e.AssociatedControl);


            string[] lines = (txt ?? _currentText).Split('\n');
            int maxW = 0;
            foreach (string line in lines)
                maxW = Math.Max(maxW,
                  TextRenderer.MeasureText(line, ToolTipFont).Width
                );

            int lineH = TextRenderer.MeasureText("Wg", ToolTipFont).Height;
            int totalH = lines.Length * lineH;

            e.ToolTipSize = new Size(
              maxW + PaddingX * 2,
              totalH + PaddingY * 2
            );
        }

        // draw first a fully opaque base, then gradient on top, then border+text
        private void OnDraw(object s, DrawToolTipEventArgs e)
        {
            //Size r2 = Size.Empty;
            if (e.ToolTipText != null && e.ToolTipText != "" && e.ToolTipText != _currentText)
            {
                //if (_currentText == null || _currentText == "")
                //{
                _currentText = e.ToolTipText;
                //}




            }


            var g = e.Graphics;
            var r = e.Bounds;

            //string[] lines = (_currentText ?? "").Split('\n');
            //int maxW = 0;
            //foreach (string line in lines)
            //    maxW = Math.Max(maxW,
            //      TextRenderer.MeasureText(line, ToolTipFont).Width
            //    );

            //int lineH = TextRenderer.MeasureText("Wg", ToolTipFont).Height;
            //int totalH = lines.Length * lineH;

            //r = new Rectangle(0,0,
            //  maxW + PaddingX * 2,
            //  totalH + PaddingY * 2
            //);



            // 1) base fill with *fully* opaque bottom-color
            using (var bg0 = new SolidBrush(WithAlpha(BackGradientBottom, 255)))
                g.FillRectangle(bg0, r);

            // 2) nice gradient overlay (you can give these any alpha you like;
            //    they sit on top of the opaque base)
            Rectangle inner = Rectangle.Inflate(r, -1, -1);
            using (var br = new LinearGradientBrush(
                     inner,
                     WithAlpha(BackGradientTop, BackGradientTop.A),
                     WithAlpha(BackGradientBottom, BackGradientBottom.A),
                     LinearGradientMode.Vertical))
            {
                g.FillRectangle(br, inner);
            }

            // 3) border
            using (var pen = new Pen(BorderColor))
                g.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);

            // 4) text
            var textR = new Rectangle(
              r.X + PaddingX,
              r.Y + PaddingY,
              r.Width - PaddingX * 2,
              r.Height - PaddingY * 2
            );

            


            TextRenderer.DrawText(
              g,
              _currentText,
              ToolTipFont,
              textR,
              TextColor,
              TextFormatFlags.WordBreak
            );
        }

        // helper to swap in any alpha you like
        private static Color WithAlpha(Color c, int a)
        {
            if (a < 0) a = 0;
            if (a > 255) a = 255;
            return Color.FromArgb(a, c.R, c.G, c.B);
        }

        // optional helper to mirror your bar’s palette
        public void SetColours(
            Color gradTop, Color gradBot, Color border, Color text)
        {
            BackGradientTop = gradTop;
            BackGradientBottom = gradBot;
            BorderColor = border;
            TextColor = text;
        }
    }
}
