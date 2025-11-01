using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrutalZip2025.BrutalControls
{

    public partial class BrutalButton : Button
    {
        #region Properties

        // Background colors
        private Color backgroundColor = Color.LightGray;
        [Category("Appearance"), Description("Background color of the button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundColor
        {
            get => backgroundColor;
            set { backgroundColor = value; Invalidate(); }
        }

        private Color backgroundGradientStart = Color.Transparent;
        private Color backgroundGradientEnd = Color.Transparent;
        private bool backgroundGradientEnabled = false;

        [Category("Appearance"), Description("Start color for the background gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundGradientStart
        {
            get => backgroundGradientStart;
            set { backgroundGradientStart = value; Invalidate(); }
        }

        [Category("Appearance"), Description("End color for the background gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundGradientEnd
        {
            get => backgroundGradientEnd;
            set { backgroundGradientEnd = value; Invalidate(); }
        }

        [Category("Appearance"), Description("Enable gradient for the background.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool BackgroundGradientEnabled
        {
            get => backgroundGradientEnabled;
            set { backgroundGradientEnabled = value; Invalidate(); }
        }

        // Border colors
        private Color borderColor = Color.DarkGray;
        [Category("Appearance"), Description("Border color of the button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        private Color borderGradientStart = Color.Transparent;
        private Color borderGradientEnd = Color.Transparent;
        private bool borderGradientEnabled = false;

        [Category("Appearance"), Description("Start color for the border gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderGradientStart
        {
            get => borderGradientStart;
            set { borderGradientStart = value; Invalidate(); }
        }

        [Category("Appearance"), Description("End color for the border gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderGradientEnd
        {
            get => borderGradientEnd;
            set { borderGradientEnd = value; Invalidate(); }
        }

        [Category("Appearance"), Description("Enable gradient for the border.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool BorderGradientEnabled
        {
            get => borderGradientEnabled;
            set { borderGradientEnabled = value; Invalidate(); }
        }

        // Text colors
        private Color textColor = Color.Black;
        [Category("Appearance"), Description("Color of the button text.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CustomTextColor
        {
            get => textColor;
            set { textColor = value; Invalidate(); }
        }

        // Image properties
        private Image buttonImage = null;
        [Category("Appearance"), Description("Image to display on the button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image ButtonImage
        {
            get => buttonImage;
            set { buttonImage = value; Invalidate(); }
        }

        private bool autoScaleImage = false;
        [Category("Appearance"), Description("Automatically scale the image if it exceeds the button dimensions.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool AutoScaleImage
        {
            get => autoScaleImage;
            set { autoScaleImage = value; Invalidate(); }
        }

        // Padding between image and text
        private int imageTextPadding = 5;
        [Category("Layout"), Description("Padding between the image and the text.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ImageTextPadding
        {
            get => imageTextPadding;
            set { imageTextPadding = value; Invalidate(); }
        }

        // Rounded Corners
        private int cornerRadius = 10;
        [Category("Appearance"), Description("Radius for the button's rounded corners.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = value < 0 ? 0 : value;
                Invalidate();
            }
        }

        // Hover Background Colors
        private Color hoverBackgroundColor = Color.Gray;
        [Category("Appearance"), Description("Background color when the mouse hovers over the button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverBackgroundColor
        {
            get => hoverBackgroundColor;
            set { hoverBackgroundColor = value; Invalidate(); }
        }

        private Color hoverBackgroundGradientStart = Color.Transparent;
        private Color hoverBackgroundGradientEnd = Color.Transparent;
        private bool hoverBackgroundGradientEnabled = false;

        [Category("Appearance"), Description("Start color for the hover background gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverBackgroundGradientStart
        {
            get => hoverBackgroundGradientStart;
            set { hoverBackgroundGradientStart = value; Invalidate(); }
        }

        [Category("Appearance"), Description("End color for the hover background gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverBackgroundGradientEnd
        {
            get => hoverBackgroundGradientEnd;
            set { hoverBackgroundGradientEnd = value; Invalidate(); }
        }

        [Category("Appearance"), Description("Enable gradient for the hover background.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool HoverBackgroundGradientEnabled
        {
            get => hoverBackgroundGradientEnabled;
            set { hoverBackgroundGradientEnabled = value; Invalidate(); }
        }

        // Hover Border Colors
        private Color hoverBorderColor = Color.DarkGray;
        [Category("Appearance"), Description("Border color when the mouse hovers over the button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverBorderColor
        {
            get => hoverBorderColor;
            set { hoverBorderColor = value; Invalidate(); }
        }

        // Pressed State Colors
        private Color pressedBackgroundColor = Color.DarkGray;
        [Category("Appearance"), Description("Background color when the button is pressed.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color PressedBackgroundColor
        {
            get => pressedBackgroundColor;
            set { pressedBackgroundColor = value; Invalidate(); }
        }

        // Animation Properties
        private int animationDuration = 200; // in milliseconds
        [Category("Behavior"), Description("Duration of the pressed state animation in milliseconds.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int AnimationDuration
        {
            get => animationDuration;
            set { animationDuration = value < 0 ? 0 : value; }
        }

        #endregion

        #region State Management

        private enum ButtonStateEnum
        {
            Normal,
            Hovered,
            Pressed
        }

        private ButtonStateEnum currentState = ButtonStateEnum.Normal;
        private bool isHovered = false;
        private bool isPressed = false;

        #endregion

        #region Animation

        private System.Windows.Forms.Timer animationTimer;
        private DateTime animationStartTime;
        private bool isAnimating = false;

        private Color animationStartColor;
        private Color animationEndColor;

        public BrutalButton()
        {
            this.DoubleBuffered = true;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(100, 40); // Default size
            this.ForeColor = textColor;

            // Initialize animation timer
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 15; // ~60 FPS
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            double elapsed = (DateTime.Now - animationStartTime).TotalMilliseconds;
            double progress = Math.Min(elapsed / animationDuration, 1.0); // Clamp to [0,1]

            // Interpolate colors based on progress
            Color currentBackgroundColor = InterpolateColor(animationStartColor, animationEndColor, progress);
            backgroundColor = currentBackgroundColor;

            Invalidate();

            if (progress >= 1.0)
            {
                animationTimer.Stop();
                isAnimating = false;
            }
        }

        private Color InterpolateColor(Color start, Color end, double progress)
        {
            int r = (int)(start.R + (end.R - start.R) * progress);
            int g = (int)(start.G + (end.G - start.G) * progress);
            int b = (int)(start.B + (end.B - start.B) * progress);
            int a = (int)(start.A + (end.A - start.A) * progress);
            return Color.FromArgb(a, r, g, b);
        }

        private void StartAnimation(Color start, Color end)
        {
            if (isAnimating)
            {
                // Optionally, handle ongoing animations
                animationTimer.Stop();
            }

            animationStartColor = start;
            animationEndColor = end;
            animationStartTime = DateTime.Now;
            isAnimating = true;
            animationTimer.Start();
        }

        #endregion

        #region Event Overrides

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            if (!isPressed)
            {
                currentState = ButtonStateEnum.Hovered;
                // Start hover animation if desired
                StartAnimation(backgroundColor, HoverBackgroundColor);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            if (!isPressed)
            {
                currentState = ButtonStateEnum.Normal;
                // Revert to normal background
                StartAnimation(backgroundColor, BackgroundColor);
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                currentState = ButtonStateEnum.Pressed;
                StartAnimation(backgroundColor, PressedBackgroundColor);
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = false;
                currentState = isHovered ? ButtonStateEnum.Hovered : ButtonStateEnum.Normal;
                Color targetColor = isHovered ? HoverBackgroundColor : BackgroundColor;
                StartAnimation(backgroundColor, targetColor);
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.Invalidate(); // Redraw to reflect enabled/disabled state
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Do not call base.OnPaint to prevent default drawing
            // base.OnPaint(pevent);

            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Define the button's rectangle
            RectangleF buttonRect = new RectangleF(0, 0, this.Width, this.Height);

            // Create the rounded path
            GraphicsPath roundedPath = GetRoundedRectanglePath(buttonRect, cornerRadius);

            // Clip the graphics to the rounded path
            g.SetClip(roundedPath);

            // Draw background based on state
            if (currentState == ButtonStateEnum.Pressed)
            {
                // Pressed state: Use PressedBackgroundColor
                using (SolidBrush bgBrush = new SolidBrush(pressedBackgroundColor))
                {
                    g.FillPath(bgBrush, roundedPath);
                }
            }
            else if (currentState == ButtonStateEnum.Hovered)
            {
                if (HoverBackgroundGradientEnabled)
                {
                    using (LinearGradientBrush bgBrush = new LinearGradientBrush(buttonRect, HoverBackgroundGradientStart, HoverBackgroundGradientEnd, LinearGradientMode.Vertical))
                    {
                        g.FillPath(bgBrush, roundedPath);
                    }
                }
                else
                {
                    using (SolidBrush bgBrush = new SolidBrush(hoverBackgroundColor))
                    {
                        g.FillPath(bgBrush, roundedPath);
                    }
                }
            }
            else // Normal state
            {
                if (BackgroundGradientEnabled)
                {
                    using (LinearGradientBrush bgBrush = new LinearGradientBrush(buttonRect, BackgroundGradientStart, BackgroundGradientEnd, LinearGradientMode.Vertical))
                    {
                        g.FillPath(bgBrush, roundedPath);
                    }
                }
                else
                {
                    using (SolidBrush bgBrush = new SolidBrush(backgroundColor))
                    {
                        g.FillPath(bgBrush, roundedPath);
                    }
                }
            }

            // Draw border based on state
            if (isHovered && HoverBorderColor != Color.Empty && currentState != ButtonStateEnum.Pressed)
            {
                using (Pen borderPen = new Pen(hoverBorderColor, 1))
                {
                    g.DrawPath(borderPen, roundedPath);
                }
            }
            else
            {
                if (BorderGradientEnabled)
                {
                    using (LinearGradientBrush borderBrush = new LinearGradientBrush(buttonRect, BorderGradientStart, BorderGradientEnd, LinearGradientMode.Vertical))
                    {
                        using (Pen borderPen = new Pen(borderBrush, 1))
                        {
                            g.DrawPath(borderPen, roundedPath);
                        }
                    }
                }
                else
                {
                    using (Pen borderPen = new Pen(borderColor, 1))
                    {
                        g.DrawPath(borderPen, roundedPath);
                    }
                }
            }

            // Reset the clip
            g.ResetClip();

            // Calculate image and text positions
            SizeF imageSize = GetScaledImageSize();
            SizeF textSize = string.IsNullOrEmpty(Text) ? SizeF.Empty : g.MeasureString(Text, Font);
            float totalHeight = imageSize.Height + (buttonImage != null && !string.IsNullOrEmpty(Text) ? imageTextPadding : 0) + textSize.Height;
            float startY = (Height - totalHeight) / 2;

            // Draw Image
            if (buttonImage != null)
            {
                float imageX = (Width - imageSize.Width) / 2;
                float imageY = startY;
                RectangleF imgRect = new RectangleF(imageX, imageY, imageSize.Width, imageSize.Height);
                g.DrawImage(buttonImage, imgRect);
            }

            // Draw Text
            if (!string.IsNullOrEmpty(Text))
            {
                float textX = (Width - textSize.Width) / 2;
                float textY = startY + imageSize.Height + (buttonImage != null ? imageTextPadding : 0);
                RectangleF textRect = new RectangleF(textX, textY, textSize.Width, textSize.Height);

                using (SolidBrush textBrush = new SolidBrush(textColor))
                {
                    g.DrawString(Text, Font, textBrush, textRect);
                }
            }

            // Draw focus rectangle if needed
            if (this.Focused && this.ShowFocusCues)
            {
                ControlPaint.DrawFocusRectangle(g, this.ClientRectangle);
            }
            Invalidate();

        }

        private GraphicsPath GetRoundedRectanglePath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float diameter = radius * 2;

            // Top-left arc
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            // Top edge
            path.AddLine(rect.X + radius, rect.Y, rect.Right - radius, rect.Y);
            // Top-right arc
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            // Right edge
            path.AddLine(rect.Right, rect.Y + radius, rect.Right, rect.Bottom - radius);
            // Bottom-right arc
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            // Bottom edge
            path.AddLine(rect.Right - radius, rect.Bottom, rect.X + radius, rect.Bottom);
            // Bottom-left arc
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            // Left edge
            path.AddLine(rect.X, rect.Bottom - radius, rect.X, rect.Y + radius);

            path.CloseFigure();
            return path;
        }

        private SizeF GetScaledImageSize()
        {
            if (buttonImage == null)
                return SizeF.Empty;

            if (autoScaleImage && (buttonImage.Width > this.Width - 20 || buttonImage.Height > this.Height - 20))
            {
                // Scale the image to fit within the button
                float scale = Math.Min((this.Width - 20) / (float)buttonImage.Width, (this.Height - 20) / (float)buttonImage.Height);
                return new SizeF(buttonImage.Width * scale, buttonImage.Height * scale);
            }
            else
            {
                return new SizeF(buttonImage.Width, buttonImage.Height);
            }
        }

        #endregion

        #region Overrides

        // Override BackColor to hide it and use BackgroundColor instead
        [Browsable(false)]
        public override Color BackColor
        {
            get => BackgroundColor;
            set => BackgroundColor = value;
        }

        #endregion
    }
}
