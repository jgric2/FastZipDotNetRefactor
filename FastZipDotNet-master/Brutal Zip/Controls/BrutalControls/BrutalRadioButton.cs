using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace BrutalZip2025.BrutalControls
{
    public partial class BrutalRadioButton : RadioButton
    {
        // Radio Button size
        private int radioSize = 16;
        [Category("Appearance"), Description("Size of the radio button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RadioSize
        {
            get => radioSize;
            set { radioSize = value; this.Invalidate(); }
        }

        // Outer circle colors
        private Color outerCircleColor = Color.Black;
        [Category("Appearance"), Description("Color of the outer circle.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color OuterCircleColor
        {
            get => outerCircleColor;
            set { outerCircleColor = value; this.Invalidate(); }
        }

        private Color outerCircleGradientStart = Color.Transparent;
        private Color outerCircleGradientEnd = Color.Transparent;
        private bool outerCircleGradientEnabled = false;

        [Category("Appearance"), Description("Start color for the outer circle gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color OuterCircleGradientStart
        {
            get => outerCircleGradientStart;
            set { outerCircleGradientStart = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("End color for the outer circle gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color OuterCircleGradientEnd
        {
            get => outerCircleGradientEnd;
            set { outerCircleGradientEnd = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("Enable gradient for the outer circle.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool OuterCircleGradientEnabled
        {
            get => outerCircleGradientEnabled;
            set { outerCircleGradientEnabled = value; this.Invalidate(); }
        }

        // Inner fill colors (selected state)
        private Color innerFillColor = Color.Black;
        [Category("Appearance"), Description("Color of the inner fill when selected.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color InnerFillColor
        {
            get => innerFillColor;
            set { innerFillColor = value; this.Invalidate(); }
        }

        private Color innerFillGradientStart = Color.Transparent;
        private Color innerFillGradientEnd = Color.Transparent;
        private bool innerFillGradientEnabled = false;

        [Category("Appearance"), Description("Start color for the inner fill gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color InnerFillGradientStart
        {
            get => innerFillGradientStart;
            set { innerFillGradientStart = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("End color for the inner fill gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color InnerFillGradientEnd
        {
            get => innerFillGradientEnd;
            set { innerFillGradientEnd = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("Enable gradient for the inner fill.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool InnerFillGradientEnabled
        {
            get => innerFillGradientEnabled;
            set { innerFillGradientEnabled = value; this.Invalidate(); }
        }

        // Radio Button border colors
        private Color borderColor = Color.Black;
        [Category("Appearance"), Description("Border color of the radio button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; this.Invalidate(); }
        }

        // Text colors
        private Color textColor = Color.Black;
        [Category("Appearance"), Description("Color of the radio button text.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CustomTextColor
        {
            get => textColor;
            set { textColor = value; this.Invalidate(); }
        }

        // Constructor
        public BrutalRadioButton()
        {
            this.DoubleBuffered = true;
            this.AutoSize = false; // We'll handle sizing manually
            this.Size = new Size(200, radioSize + 4); // Default size
            this.ForeColor = textColor;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Do not call base.OnPaint to prevent default drawing
            base.OnPaint(pevent);

            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Calculate positions
            Rectangle radioRect = new Rectangle(0, (this.Height - radioSize) / 2, radioSize, radioSize);
            Rectangle textRect = new Rectangle(radioRect.Right + 8, 0, this.Width - radioRect.Right - 8, this.Height);

            // Draw the outer circle with or without gradient
            if (outerCircleGradientEnabled)
            {
                using (LinearGradientBrush outerBrush = new LinearGradientBrush(radioRect, outerCircleGradientStart, outerCircleGradientEnd, LinearGradientMode.Vertical))
                {
                    g.FillEllipse(outerBrush, radioRect);
                }
            }
            else
            {
                using (SolidBrush outerBrush = new SolidBrush(outerCircleColor))
                {
                    g.FillEllipse(outerBrush, radioRect);
                }
            }

            // Draw the border of the outer circle
            using (Pen borderPen = new Pen(borderColor, 1.5f))
            {
                g.DrawEllipse(borderPen, radioRect);
            }

            // If selected, draw the inner fill
            if (this.Checked)
            {
                // Define inner circle rect (smaller than outer)
                int innerSize = (int)(radioSize * 0.6);
                Rectangle innerRect = new Rectangle(
                    radioRect.X + (radioSize - innerSize) / 2,
                    radioRect.Y + (radioSize - innerSize) / 2,
                    innerSize,
                    innerSize);

                if (innerFillGradientEnabled)
                {
                    using (LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, innerFillGradientStart, innerFillGradientEnd, LinearGradientMode.Vertical))
                    {
                        g.FillEllipse(innerBrush, innerRect);
                    }
                }
                else
                {
                    using (SolidBrush innerBrush = new SolidBrush(innerFillColor))
                    {
                        g.FillEllipse(innerBrush, innerRect);
                    }
                }

                // Optionally, draw inner border
                using (Pen innerBorderPen = new Pen(borderColor, 1))
                {
                    g.DrawEllipse(innerBorderPen, innerRect);
                }
            }

            // Draw the text
            TextRenderer.DrawText(
                g,
                this.Text,
                this.Font,
                textRect,
                this.Checked ? this.CustomTextColor : this.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }

        // Ensure the control is redrawn when checked state changes
        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            this.Invalidate();
        }

        // Redraw the control when clicked
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.Invalidate();
        }

        // Handle size changes
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Invalidate();
        }
    }
}
