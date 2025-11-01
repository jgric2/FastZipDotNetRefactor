using System.ComponentModel;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;

namespace BrutalZip2025.BrutalControls
{
    public partial class BrutalGradientPanel : Panel
    {
        // backing fields
        private Color startColor = Color.White;
        private Color endColor = Color.Black;
        private GradientSide startSide = GradientSide.Left;
        private GradientSide endSide = GradientSide.Right;
        private bool mouseEvents = true;
        private bool mouseGlow = false;

        private int glowTimerInterval = 30;    // ms
        private float glowPulseSpeed = 0.03f; // phase increment per tick
        private float glowMinRadius = 30f;   // px
        private float glowMaxRadiusFactor = 0.75f; // fraction of max(width,height)
        private float glowMovementSpeed = 0.2f;  // smoothing 0..1

        // animation state
        private Timer glowTimer;
        private float glowPhase;
        private PointF targetLocation;
        private PointF glowLocation;
        private bool isHovered;

        public BrutalGradientPanel()
        {
            // optimize flicker
            SetStyle(
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint,
                true);
            UpdateStyles();

            // init timer
            glowTimer = new Timer();
            glowTimer.Tick += GlowTimer_Tick;
            glowTimer.Interval = glowTimerInterval;
        }

        // ========== GRADIENT PROPERTIES ==========

        private byte glowMinSurroundOpacity = 30;   // edge never fully transparent
        private byte glowCenterMinOpacity = 50;   // center minimum
        private byte glowCenterMaxOpacity = 200;  // center maximum

        [Browsable(true), Category("Appearance"),
         Description("Minimum opacity (0–255) of the glow's outer edge.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public byte GlowMinSurroundOpacity
        {
            get => glowMinSurroundOpacity;
            set => glowMinSurroundOpacity = value;
        }

        [Browsable(true), Category("Appearance"),
         Description("Minimum opacity (0–255) of the glow's center.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public byte GlowCenterMinOpacity
        {
            get => glowCenterMinOpacity;
            set => glowCenterMinOpacity = value;
        }

        [Browsable(true), Category("Appearance"),
         Description("Maximum opacity (0–255) of the glow's center.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public byte GlowCenterMaxOpacity
        {
            get => glowCenterMaxOpacity;
            set => glowCenterMaxOpacity = value;
        }


        [Browsable(true), Category("Appearance"), Description("Gradient start color.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color StartColor
        {
            get => startColor;
            set { startColor = value; Invalidate(); }
        }

        [Browsable(true), Category("Appearance"), Description("Gradient end color.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color EndColor
        {
            get => endColor;
            set { endColor = value; Invalidate(); }
        }

        [Browsable(true), Category("Appearance"), Description("Side where gradient starts."), DefaultValue(GradientSide.Left)]
        public GradientSide GradientStartSide
        {
            get => startSide;
            set { startSide = value; Invalidate(); }
        }

        [Browsable(true), Category("Appearance"), Description("Side where gradient ends."), DefaultValue(GradientSide.Right)]
        public GradientSide GradientEndSide
        {
            get => endSide;
            set { endSide = value; Invalidate(); }
        }

        [Browsable(true), Category("Behavior"), Description("Allow or block mouse events on this panel.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool MouseEvents
        {
            get => mouseEvents;
            set { mouseEvents = value; Invalidate(); }
        }

        // ========== GLOW PROPERTIES ==========

        [Browsable(true), Category("Behavior"), Description("Enable pulsing glow under the mouse."), DefaultValue(false)]
        public bool MouseGlow
        {
            get => mouseGlow;
            set
            {
                mouseGlow = value;
                if (mouseGlow)
                {
                    // sync positions
                    Point c = PointToClient(Cursor.Position);
                    targetLocation = glowLocation = c;
                    glowPhase = 0;
                    glowTimer.Start();
                }
                else
                {
                    glowTimer.Stop();
                    isHovered = false;
                }
                Invalidate();
            }
        }

        [Browsable(true), Category("Behavior"), Description("Timer interval for glow animation (ms)."), DefaultValue(30)]
        public int GlowTimerInterval
        {
            get => glowTimerInterval;
            set
            {
                glowTimerInterval = value;
                if (glowTimer != null)
                    glowTimer.Interval = glowTimerInterval;
            }
        }

        [Browsable(true), Category("Behavior"), Description("Phase increment per timer tick (0–1)."), DefaultValue(0.03f)]
        public float GlowPulseSpeed
        {
            get => glowPulseSpeed;
            set => glowPulseSpeed = value;
        }

        [Browsable(true), Category("Appearance"), Description("Minimum glow radius in pixels."), DefaultValue(30f)]
        public float GlowMinRadius
        {
            get => glowMinRadius;
            set => glowMinRadius = value;
        }

        [Browsable(true), Category("Appearance"), Description("Maximum glow radius as fraction of control size."), DefaultValue(0.75f)]
        public float GlowMaxRadiusFactor
        {
            get => glowMaxRadiusFactor;
            set => glowMaxRadiusFactor = value;
        }

        [Browsable(true), Category("Behavior"), Description("Glow movement smoothing (0=snappy,1=static)."), DefaultValue(0.2f)]
        public float GlowMovementSpeed
        {
            get => glowMovementSpeed;
            set
            {
                glowMovementSpeed = Math.Max(0f, Math.Min(1f, value));
            }
        }

        // ========== ANIMATION LOOP ==========

        private void GlowTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (IsDisposed)
                {
                    glowTimer.Stop();
                    return;
                }

                // read real mouse pos in client coords
                Point raw = PointToClient(Cursor.Position);
                bool nowOver = ClientRectangle.Contains(raw);

                isHovered = nowOver;
                if (isHovered)
                    targetLocation = raw;

                // advance pulse
                glowPhase += glowPulseSpeed;
                if (glowPhase > 1f) glowPhase -= 1f;

                // chase target
                if (isHovered)
                {
                    float s = glowMovementSpeed;
                    glowLocation.X += (targetLocation.X - glowLocation.X) * s;
                    glowLocation.Y += (targetLocation.Y - glowLocation.Y) * s;
                }
            }
            catch
            {
                glowTimer.Stop();
            }
         

            Invalidate();
        }

        // ========== PAINTING ==========

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // suppress default
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;

            // draw linear gradient
            Point sp = GetSidePoint(GradientStartSide);
            Point ep = GetSidePoint(GradientEndSide);
            if (sp == ep) ep.X++;
            using (var br = new LinearGradientBrush(sp, ep, StartColor, EndColor))
                g.FillRectangle(br, ClientRectangle);

            // draw glow
            if (MouseGlow && isHovered)
                DrawGlow(g);

            base.OnPaint(e);
        }

        private void DrawGlow(Graphics g)
        {
            // oscillating t in [0..1]
            float t = (float)((Math.Sin(glowPhase * 2 * Math.PI) + 1) * 0.5);
            float maxR = Math.Max(Width, Height) * glowMaxRadiusFactor;
            float r = glowMinRadius + (maxR - glowMinRadius) * t;

            // mid-color, lightened
            Color mid = Color.FromArgb(
                (StartColor.R + EndColor.R) / 2,
                (StartColor.G + EndColor.G) / 2,
                (StartColor.B + EndColor.B) / 2);
            Color glowCol = ControlPaint.Light(mid, 0.3f);

            using (var path = new GraphicsPath())
            {
                path.AddEllipse(
                    glowLocation.X - r,
                    glowLocation.Y - r,
                    r * 2, r * 2);

                using (var pgb = new PathGradientBrush(path))
                {
                    pgb.CenterPoint = glowLocation;
                    pgb.CenterColor = Color.FromArgb((int)(150 * t) + 50, glowCol);
                    pgb.SurroundColors = new[] { Color.FromArgb(0, glowCol) };
                    g.FillEllipse(pgb,
                        glowLocation.X - r,
                        glowLocation.Y - r,
                        r * 2, r * 2);
                }
            }
        }

        // ========== HIT-TEST OVERRIDE ==========

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTTRANSPARENT = -1;

            if (!MouseEvents && m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTTRANSPARENT;
                return;
            }

            base.WndProc(ref m);
        }

        private Point GetSidePoint(GradientSide side)
        {
            return side switch
            {
                GradientSide.Left => new Point(0, Height / 2),
                GradientSide.Top => new Point(Width / 2, 0),
                GradientSide.Right => new Point(Width, Height / 2),
                GradientSide.Bottom => new Point(Width / 2, Height),
                _ => Point.Empty,
            };
        }

     /*   protected override void Dispose(bool disposing)
        {
            if (disposing)
                glowTimer?.Dispose();
            base.Dispose(disposing);
        }*/
    }

    public enum GradientSide
    {
        Left,
        Top,
        Right,
        Bottom
    }
}