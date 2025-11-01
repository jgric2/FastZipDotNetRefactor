using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace BrutalZip2025.BrutalControls
{
    public partial class BrutalTextBox : UserControl
    {
        private System.Windows.Forms.TextBox innerTextBox;

        // Background Properties
        private Color backgroundColor = Color.White;
        [Category("Appearance"), Description("Background color of the TextBox.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundColor
        {
            get => backgroundColor;
            set { backgroundColor = value; this.Invalidate(); }
        }

        private Color backgroundGradientStart = Color.White;
        private Color backgroundGradientEnd = Color.White;
        private bool backgroundGradientEnabled = false;

        [Category("Appearance"), Description("Start color for the background gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundGradientStart
        {
            get => backgroundGradientStart;
            set { backgroundGradientStart = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("End color for the background gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundGradientEnd
        {
            get => backgroundGradientEnd;
            set { backgroundGradientEnd = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("Enable gradient for the background.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool BackgroundGradientEnabled
        {
            get => backgroundGradientEnabled;
            set { backgroundGradientEnabled = value; this.Invalidate(); }
        }

        // Border Properties
        private Color borderColor = Color.Gray;
        [Category("Appearance"), Description("Border color of the TextBox.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; this.Invalidate(); }
        }

        private int borderThickness = 1;
        [Category("Appearance"), Description("Border thickness of the TextBox.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderThickness
        {
            get => borderThickness;
            set { borderThickness = value; AdjustInnerTextBox(); this.Invalidate(); }
        }

        private int borderRadius = 0; // 0 for square corners
        [Category("Appearance"), Description("Border radius for rounded corners. Set to 0 for square corners.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; this.Invalidate(); }
        }

        // Text Properties
        private Color textColor = Color.Black;
        [Category("Appearance"), Description("Color of the text.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TextColor
        {
            get => textColor;
            set { textColor = value; if (!textGradientEnabled) innerTextBox.ForeColor = value; this.Invalidate(); }
        }

        private Color textGradientStart = Color.Black;
        private Color textGradientEnd = Color.Black;
        private bool textGradientEnabled = false;

        [Category("Appearance"), Description("Start color for the text gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TextGradientStart
        {
            get => textGradientStart;
            set { textGradientStart = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("End color for the text gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TextGradientEnd
        {
            get => textGradientEnd;
            set { textGradientEnd = value; this.Invalidate(); }
        }

        [Category("Appearance"), Description("Enable gradient for the text.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool TextGradientEnabled
        {
            get => textGradientEnabled;
            set
            {
                textGradientEnabled = value;
                innerTextBox.ForeColor = textGradientEnabled ? Color.White : textColor;
                this.Invalidate();
            }
        }

        // Constructor
        public BrutalTextBox()
        {
            InitializeComponent();
        }

       

        // Event handlers to propagate inner TextBox events
        private void InnerTextBox_LostFocus(object sender, EventArgs e)
        {
            this.Invalidate(); // Redraw borders or other focus-dependent styles
        }

        private void InnerTextBox_GotFocus(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void InnerTextBox_TextChanged(object sender, EventArgs e)
        {
            this.TextChanged?.Invoke(this, e);
        }

        // Event to expose TextChanged
        public new event EventHandler TextChanged;

        // Expose Text property
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override string Text
        {
            get { return innerTextBox.Text; }
            set { innerTextBox.Text = value; }
        }

        // Expose Multiline property
        [Category("Behavior"), Description("Determines whether this is a multiline TextBox.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Multiline
        {
            get => innerTextBox.Multiline;
            set
            {
                innerTextBox.Multiline = value;
                AdjustInnerTextBox();
                this.Invalidate();
            }
        }

        // Expose Font property
        public override Font Font
        {
            get => innerTextBox.Font;
            set
            {
                innerTextBox.Font = value;
                base.Font = value;
                this.Invalidate();
            }
        }

        // Expose ForeColor property
        public override Color ForeColor
        {
            get => innerTextBox.ForeColor;
            set
            {
                innerTextBox.ForeColor = value;
                if (!textGradientEnabled)
                    textColor = value;
                this.Invalidate();
            }
        }

        // Expose BackColor property
        public override Color BackColor
        {
            get => innerTextBox.BackColor;
            set
            {
                innerTextBox.BackColor = value;
                base.BackColor = value;
                this.Invalidate();
            }
        }

        // Expose TextAlign property
        [Category("Appearance"), Description("Text alignment within the TextBox.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public HorizontalAlignment TextAlign
        {
            get => innerTextBox.TextAlign;
            set { innerTextBox.TextAlign = value; this.Invalidate(); }
        }

        // Expose ReadOnly property
        [Category("Behavior"), Description("Determines whether the TextBox is read-only.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ReadOnly
        {
            get => innerTextBox.ReadOnly;
            set { innerTextBox.ReadOnly = value; }
        }

        // Adjust innerTextBox layout
        private void AdjustInnerTextBox()
        {
            int padding = BorderThickness + 2;
            if (innerTextBox.Multiline)
            {
                innerTextBox.Multiline = true;
                innerTextBox.Height = this.Height - (2 * padding);
            }
            else
            {
                innerTextBox.Multiline = false;
                innerTextBox.Height = TextRenderer.MeasureText(innerTextBox.Text, innerTextBox.Font).Height + 4;
            }

            innerTextBox.Location = new Point(padding, (this.Height - innerTextBox.Height) / 2);
            innerTextBox.Width = this.Width - (2 * padding);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustInnerTextBox();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Define the rectangle for the background
            Rectangle rect = this.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            // Define the path for rounded corners
            GraphicsPath path = GetRoundedRectanglePath(rect, borderRadius);

            // Fill the background
            if (backgroundGradientEnabled)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, backgroundGradientStart, backgroundGradientEnd, LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(backgroundColor))
                {
                    g.FillPath(brush, path);
                }
            }

            // Draw the border
            using (Pen pen = new Pen(borderColor, borderThickness))
            {
                g.DrawPath(pen, path);
            }

            // Draw gradient text if enabled
            //if (textGradientEnabled)
            //{
            //    using (LinearGradientBrush textBrush = new LinearGradientBrush(innerTextBox.ClientRectangle, textGradientStart, textGradientEnd, LinearGradientMode.Horizontal))
            //    {
            //        StringFormat format = new StringFormat()
            //        {
            //            Alignment = StringAlignment.Near,
            //            LineAlignment = StringAlignment.Center
            //        };

            //        Rectangle textRect = new Rectangle(innerTextBox.Location.X, innerTextBox.Location.Y, innerTextBox.Width, innerTextBox.Height);
            //        g.DrawString(innerTextBox.Text, innerTextBox.Font, textBrush, textRect, format);
            //    }
            //}

            path.Dispose();
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        // Override Focus methods to redirect focus to innerTextBox
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            innerTextBox.Focus();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            innerTextBox.Focus();
        }

        // Handle key events if necessary
        private void InnerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        private void InnerTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }

        private void InnerTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }
    }

}
