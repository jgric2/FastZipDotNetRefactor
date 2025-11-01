using System.ComponentModel;

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
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
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
            // Base painting
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Calculate the width for the filled and unfilled parts
            float trackWidth = this.Width - 20; // Padding for thumb
            float trackHeight = 4;
            float trackX = 10;
            float trackY = this.Height / 2 - trackHeight / 2;

            float range = this.Maximum - this.Minimum;
            float percent = (this.Value - this.Minimum) / range;
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
            float thumbSize = 16;
            float thumbX = trackX + filledWidth - thumbSize / 2;
            float thumbY = this.Height / 2 - thumbSize / 2;

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



        private void UpdateValueFromMouse(int mouseX)
        {
            float trackWidth = this.Width - 20; // Padding for thumb
            float trackX = 10;

            float relativeX = mouseX - trackX;
            relativeX = Math.Max(0, Math.Min(relativeX, trackWidth));

            float percent = relativeX / trackWidth;
            int newValue = this.Minimum + (int)(percent * (this.Maximum - this.Minimum));

            this.Value = newValue;

            this.Invalidate();
        }
    }
}
