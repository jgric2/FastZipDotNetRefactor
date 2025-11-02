using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BrutalZip2025.BrutalControls
{
    public partial class BrutalTrackBar : TrackBar
    {
        // Custom properties for bar colors
        private Color _leftBarColor = Color.Blue;
        private Color _rightBarColor = Color.Gray;

        // Custom properties for thumb
        private Color _thumbInnerColor = Color.White;
        private Color _thumbOutlineColor = Color.Black;
        private int _thumbOutlineThickness = 2;

        public BrutalTrackBar()
        {
            // Enable user painting, double buffering and transparent back color
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
        }

        // Make Transparent the default back color in designer
        [DefaultValue(typeof(Color), "Transparent")]
        public override Color BackColor
        {
            get => base.BackColor;
            set { base.BackColor = value; this.Invalidate(); }
        }

        // Ask Windows to paint the parent first (helps transparency/flicker)
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        // Paint the parent background into our client area when transparent
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.BackColor.A == 255)
            {
                base.OnPaintBackground(e);
                return;
            }

            if (this.Parent != null)
            {
                var state = e.Graphics.Save();
                try
                {
                    // Offset so that parent paints at our location
                    int offsetX = -this.Left;
                    int offsetY = -this.Top;

                    // If the parent is scrollable, honor its scroll position
                    if (this.Parent is ScrollableControl sc)
                    {
                        offsetX += sc.AutoScrollPosition.X;
                        offsetY += sc.AutoScrollPosition.Y;
                    }

                    e.Graphics.TranslateTransform(offsetX, offsetY);

                    // Ask parent to paint its background and foreground behind us
                    var pe = new PaintEventArgs(e.Graphics, new Rectangle(Point.Empty, this.Parent.ClientSize));
                    this.InvokePaintBackground(this.Parent, pe);
                    this.InvokePaint(this.Parent, pe);
                }
                finally
                {
                    e.Graphics.Restore(state);
                }
            }
            else
            {
                // No parent: just clear
                using var b = new SolidBrush(Color.Transparent);
                e.Graphics.FillRectangle(b, this.ClientRectangle);
            }
        }

        // Properties for customization
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color LeftBarColor
        {
            get { return _leftBarColor; }
            set { _leftBarColor = value; this.Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color RightBarColor
        {
            get { return _rightBarColor; }
            set { _rightBarColor = value; this.Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ThumbInnerColor
        {
            get { return _thumbInnerColor; }
            set { _thumbInnerColor = value; this.Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ThumbOutlineColor
        {
            get { return _thumbOutlineColor; }
            set { _thumbOutlineColor = value; this.Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ThumbOutlineThickness
        {
            get { return _thumbOutlineThickness; }
            set { _thumbOutlineThickness = value; this.Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Do NOT call base.OnPaint(e); we fully paint ourselves to keep transparency clean
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Track geometry
            float trackWidth = this.Width - 20f; // Padding for thumb
            if (trackWidth < 1f) trackWidth = 1f;

            float trackHeight = 4f;
            float trackX = 10f;
            float trackY = this.Height / 2f - trackHeight / 2f;

            // Value to percent (avoid divide-by-zero)
            int rangeInt = this.Maximum - this.Minimum;
            float range = Math.Max(1, rangeInt);
            float percent = (this.Value - this.Minimum) / range;
            percent = Math.Max(0f, Math.Min(1f, percent));

            float filledWidth = trackWidth * percent;

            // Draw left (filled) part
            using (SolidBrush brush = new SolidBrush(_leftBarColor))
            {
                g.FillRectangle(brush, trackX, trackY, filledWidth, trackHeight);
            }

            // Draw right (unfilled) part
            using (SolidBrush brush = new SolidBrush(_rightBarColor))
            {
                g.FillRectangle(brush, trackX + filledWidth, trackY, trackWidth - filledWidth, trackHeight);
            }

            // Draw thumb as a circle
            float thumbSize = 16f;
            float thumbX = trackX + filledWidth - thumbSize / 2f;
            float thumbY = this.Height / 2f - thumbSize / 2f;

            RectangleF thumbRect = new RectangleF(thumbX, thumbY, thumbSize, thumbSize);
            using (SolidBrush brush = new SolidBrush(_thumbInnerColor))
            {
                g.FillEllipse(brush, thumbRect);
            }

            using (Pen pen = new Pen(_thumbOutlineColor, _thumbOutlineThickness))
            {
                g.DrawEllipse(pen, thumbRect);
            }
        }

        // Optional: Improve thumb dragging behavior
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            UpdateValueFromMouse(e.X);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                UpdateValueFromMouse(e.X);
            }
        }

        protected override void OnScroll(EventArgs e)
        {
            base.OnScroll(e);
            this.Invalidate();
        }

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            this.Invalidate();
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged(e);
            this.Invalidate();
        }

        protected override void OnParentBackgroundImageChanged(EventArgs e)
        {
            base.OnParentBackgroundImageChanged(e);
            this.Invalidate();
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            this.Invalidate();
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            float trackWidth = this.Width - 20f; // Padding for thumb
            if (trackWidth <= 0f) trackWidth = 1f;

            float trackX = 10f;

            float relativeX = mouseX - trackX;
            relativeX = Math.Max(0, Math.Min(relativeX, trackWidth));

            float percent = relativeX / trackWidth;
            int newValue = this.Minimum + (int)(percent * (this.Maximum - this.Minimum));

            // Clamp in case of rounding
            newValue = Math.Max(this.Minimum, Math.Min(this.Maximum, newValue));

            this.Value = newValue;

            this.Invalidate();
        }
    }
}