using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace BrutalZip2025.BrutalControls
{
    public class BrutalCheckBox : CheckBox
    {
        // Properties for Checkbox customization

        // Checkbox box size
        private int boxSize = 16;
        [Category("Appearance"), Description("Size of the checkbox box.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BoxSize
        {
            get => boxSize;
            set { boxSize = value; this.Invalidate(); }
        }

        // Box colors
        private Color boxBackColor = Color.White;
        [Category("Appearance"), Description("Background color of the checkbox box.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BoxBackColor
        {
            get => boxBackColor;
            set { boxBackColor = value; this.Invalidate(); }
        }

        private Color boxBorderColor = Color.Black;
        [Category("Appearance"), Description("Border color of the checkbox box.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BoxBorderColor
        {
            get => boxBorderColor;
            set { boxBorderColor = value; this.Invalidate(); }
        }

        // Checkmark colors
        private Color checkColor = Color.Black;
        [Category("Appearance"), Description("Color of the checkmark.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CheckColor
        {
            get => checkColor;
            set { checkColor = value; this.Invalidate(); }
        }

        private Color checkBorderColor = Color.Black;
        [Category("Appearance"), Description("Border color of the checkmark.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CheckBorderColor
        {
            get => checkBorderColor;
            set { checkBorderColor = value; this.Invalidate(); }
        }

        // Gradient properties
        private Color boxGradientStart = Color.Transparent;
        private Color boxGradientEnd = Color.Transparent;
        private bool boxGradientEnabled = false;

        [Category("Appearance"), Description("Start color for the box gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BoxGradientStart
        {
            get => boxGradientStart;
            set { boxGradientStart = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("End color for the box gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BoxGradientEnd
        {
            get => boxGradientEnd;
            set { boxGradientEnd = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("Enable gradient for the box.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool BoxGradientEnabled
        {
            get => boxGradientEnabled;
            set { boxGradientEnabled = value; this.Invalidate(); }
        }

        private Color checkGradientStart = Color.Transparent;
        private Color checkGradientEnd = Color.Transparent;
        private bool checkGradientEnabled = false;

        [Category("Appearance"), Description("Start color for the checkmark gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CheckGradientStart
        {
            get => checkGradientStart;
            set { checkGradientStart = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("End color for the checkmark gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CheckGradientEnd
        {
            get => checkGradientEnd;
            set { checkGradientEnd = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("Enable gradient for the checkmark.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool CheckGradientEnabled
        {
            get => checkGradientEnabled;
            set { checkGradientEnabled = value; this.Invalidate(); }
        }
        // Similarly, you can add gradient properties for other elements if needed

        public BrutalCheckBox()
        {
            this.DoubleBuffered = true;
            this.AutoSize = false; // We'll handle sizing manually
            this.Size = new Size(150, boxSize + 4); // Default size

            //this.ThreeState = true;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Base class does not draw anything since we're owner drawing
            // Uncomment the line below if you want to include the base drawing
             base.OnPaint(pevent);

            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Calculate positions
            Rectangle boxRect = new Rectangle(0, (this.Height - boxSize) / 2, boxSize, boxSize);
            Rectangle textRect = new Rectangle(boxRect.Right + 4, 0, this.Width - boxRect.Right - 4, this.Height);

            // Draw the checkbox box with or without gradient
            if (boxGradientEnabled)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(boxRect, boxGradientStart, boxGradientEnd, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, boxRect);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(boxBackColor))
                {
                    g.FillRectangle(brush, boxRect);
                }
            }

            // Draw the border of the checkbox box
            using (Pen pen = new Pen(boxBorderColor))
            {
                g.DrawRectangle(pen, boxRect);
            }

            // If checked, draw the checkmark
            if (this.CheckState == CheckState.Checked)
            {
                // Define checkmark points
                PointF[] checkPoints = new PointF[]
                {
        new PointF(boxRect.Left + boxSize * 0.2f, boxRect.Top + boxSize * 0.5f),
        new PointF(boxRect.Left + boxSize * 0.45f, boxRect.Bottom - boxSize * 0.3f),
        new PointF(boxRect.Right - boxSize * 0.2f, boxRect.Top + boxSize * 0.2f)
                };

                // Draw the checkmark
                if (checkGradientEnabled)
                {
                    using (LinearGradientBrush checkBrush = new LinearGradientBrush(boxRect, checkGradientStart, checkGradientEnd, LinearGradientMode.Vertical))
                    {
                        using (Pen checkPen = new Pen(checkBrush, 2))
                        {
                            checkPen.StartCap = LineCap.Round;
                            checkPen.EndCap = LineCap.Round;
                            g.DrawLines(checkPen, checkPoints);
                        }
                    }
                }
                else
                {
                    using (Pen checkPen = new Pen(checkColor, 2))
                    {
                        checkPen.StartCap = LineCap.Round;
                        checkPen.EndCap = LineCap.Round;
                        g.DrawLines(checkPen, checkPoints);
                    }
                }

                // Draw the border of the checkmark
                if (checkBorderColor != Color.Transparent)
                {
                    using (Pen checkBorderPen = new Pen(checkBorderColor, 1))
                    {
                        checkBorderPen.StartCap = LineCap.Round;
                        checkBorderPen.EndCap = LineCap.Round;
                        g.DrawLines(checkBorderPen, checkPoints);
                    }
                }
            }
            else if (this.CheckState == CheckState.Indeterminate)
            {
                // NEW: draw a centered dash/rectangle for the indeterminate state
                int markHeight = Math.Max(2, (int)Math.Round(boxSize * 0.25f)); // 25% height (min 2px)
                int markWidth = (int)Math.Round(boxSize * 0.6f);               // 60% width
                int x = boxRect.Left + (boxRect.Width - markWidth) / 2;
                int y = boxRect.Top + (boxRect.Height - markHeight) / 2;
                Rectangle markRect = new Rectangle(x, y, markWidth, markHeight);

                if (checkGradientEnabled)
                {
                    using (LinearGradientBrush b = new LinearGradientBrush(markRect, checkGradientStart, checkGradientEnd, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(b, markRect);
                    }
                }
                else
                {
                    using (SolidBrush b = new SolidBrush(checkColor))
                    {
                        g.FillRectangle(b, markRect);
                    }
                }

                if (checkBorderColor != Color.Transparent)
                {
                    using (Pen p = new Pen(checkBorderColor, 1))
                    {
                        g.DrawRectangle(p, markRect);
                    }
                }
            }

            // Draw the text
            TextRenderer.DrawText(g, this.Text, this.Font, textRect, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.Invalidate();
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            this.Invalidate();
        }

        // NEW: also repaint when the tri-state value changes
        protected override void OnCheckStateChanged(EventArgs e)
        {
            base.OnCheckStateChanged(e);
            this.Invalidate();
        }

        // Optionally, override other events to ensure proper behavior
    }
}
