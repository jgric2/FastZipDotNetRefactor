using BrutalCopy2025.Controls.BrutalControls;
using GroBuf;
using GroBuf.DataMembersExtracters;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Text.Json;
using Timer = System.Windows.Forms.Timer;

namespace CustomProgBarSpeed
{
    public partial class FinalBarBrutal : Control
    {
        //—— Public API —————————————————————————————————————————————

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public long Maximum
        {
            get => _maximum;
            set
            {
                _maximum = Math.Max(1, value);
                if (_value > _maximum) _value = _maximum;
                Invalidate();
            }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public long Value
        {
            get => _value;
            set
            {
                long v = Math.Max(0, Math.Min(_maximum, value));
                if (v == _value) return;
                _value = v;
                UpdateHistory();
                Invalidate();
            }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double Progress
        {
            get => _progress;
            set
            {
                _progress = Math.Max(0, Math.Min(100, value));
                Value = (long)(_maximum * _progress / 100.0);
            }
        }

        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public long CurrentSpeed
        {
            get => _currentSpeed;
            set => _currentSpeed = value;
        }

        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public long FilesPerSecond
        {
            get => _filesPerSecond;
            set
            {
                _filesPerSecond = value;
              //  _filesPerSecList.Add(value);
            }
        }

        List<long> _fileSpeeds = new List<long>();  // parallel to _speeds/_speedTimes

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Paused
        {
            get => _paused;
            set { _paused = value; Invalidate(); }
        }

        // Private backing
        private int _maxSegmentCount;

        // If 0, we auto-compute as Width/5.  Otherwise you can set it yourself.
        [Category("Behavior")]
        [Description("Maximum segments before automatic merging. 0 = auto (Width/5).")]
        [DefaultValue(0)]
        public int MaxSegmentCount
        {
            get => _maxSegmentCount;
            set
            {
                _maxSegmentCount = Math.Max(0, value);
                Invalidate();
            }
        }


        // --- time‐stamps for each sample ---
        private List<DateTime> _speedTimes = new List<DateTime>();

        // --- segment definition with duration ---
        private struct Segment
        {
            public int Start;    // pixel index
            public int Length;   // pixels
            public float Speed;    // avg speed in this run
            public int FileCount;
            public TimeSpan Duration; // time from first→last sample
        }
        // which segment (if any) is hovered
        private int _hoverSegment = -1;
        // shared tooltip
        private BrutalToolTip _toolTip;

        public void SetColours(
            Color backGradientTop,
            Color backGradientBot,
            Color lineGradientTop,
            Color lineGradientBot,
            Color hatchForeColor,
            Color hatchBackColor,
            Color barSpeedColor,
            Color barSpeedTextColor,
            HatchStyle hatchStyle = HatchStyle.BackwardDiagonal)
        {
            _backTop = backGradientTop;
            _backBot = backGradientBot;
            _lineTop = lineGradientTop;
            _lineBot = lineGradientBot;
            _hatchFore = hatchForeColor;
            _hatchBack = hatchBackColor;
            _speedColor = barSpeedColor;
            _textColor = barSpeedTextColor;
            _hatchStyle = hatchStyle;
            Invalidate();
        }

        //—— ctor / Dispose ——————————————————————————————————————————

        public FinalBarBrutal()
        {
            // enable double‐buffer, user‐paint, redraw‐on‐resize, transparent
            SetStyle(
                ControlStyles.UserPaint
              | ControlStyles.AllPaintingInWmPaint
              | ControlStyles.OptimizedDoubleBuffer
              | ControlStyles.ResizeRedraw
              | ControlStyles.SupportsTransparentBackColor,
              true);

            BackColor = Color.Transparent;
            FontStyle fontStyle = FontStyle.Bold;
            _font = new Font("Segoe UI", 10f, fontStyle);
            _sf = new StringFormat(StringFormatFlags.MeasureTrailingSpaces)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };

            // pulse timer (orb)
    /*        _pulseTimer = new Timer { Interval = 30 };
            _pulseTimer.Tick += (s, e) =>
            {
                if (_pulseForward) _pulse += 0.04f; else _pulse -= 0.04f;
                if (_pulse >= 1f) { _pulse = 1f; _pulseForward = false; }
                else if (_pulse <= 0f) { _pulse = 0f; _pulseForward = true; }
                InvalidateOrb();
            };
            _pulseTimer.Start();*/

            // shimmer timer
            _shimmerTimer = new Timer { Interval = 40 };
            _shimmerTimer.Tick += (s, e) =>
            {
                if (!_shimmerActive)
                    return;            // nothing to do

                _shimmerPos += 0.02f;
                if (_shimmerPos >= 1f)
                {
                    // we’ve run one full sweep, shut it off
                    _shimmerActive = false;
                    _shimmerPos = 0f;
                }
                Invalidate();        // only redraw if shimmer is active
            };
            _shimmerTimer.Start();
            /*      _shimmerTimer.Tick += (s, e) =>
                  {
                      _shimmerPos += 0.02f;
                      if (_shimmerPos > 1f) _shimmerPos = 0f;
                      Invalidate();
                  };
                  _shimmerTimer.Start();*/

            // ripple timer
            _rippleTimer = new Timer { Interval = 40 };
            _rippleTimer.Tick += (s, e) =>
            {
                if (!_rippleActive) return;

                // advance phase & decay
                _ripplePhase += 0.3f;
                _rippleAmp *= 0.90f;    // decay factor

                // stop once too small
                if (_rippleAmp < 0.1f)
                    _rippleActive = false;

                Invalidate();
            };
            _rippleTimer.Start();

            // pause‐icon pulse timer
            _pauseTimer = new Timer { Interval = 50 };
            _pauseTimer.Tick += (s, e) =>
            {
                if (_paused)
                {
                    _pausePulse += 0.05f;
                    if (_pausePulse >= 1f) _pausePulse = 0f;
                    Invalidate();   // redraw so the icon pulses
                }
            };
            _pauseTimer.Start();

            // in your control’s constructor (or initialization)…
            _toolTip = new BrutalToolTip();

            // match your bar’s palette
            _toolTip.SetColours(
               _backTop,     // same as your bar’s back‐gradient top
               _backBot,     // bar’s back‐gradient bottom
               _lineTop,     // use your line‐color as border
               _textColor    // bar’s text color
            );

            // later in OnMouseMove, call
            
            //_toolTip = new ToolTip
            //{
            //    ShowAlways = true,
            //    AutoPopDelay = 3000,
            //    InitialDelay = 200,
            //    ReshowDelay = 100
            //};

        }

        private List<Segment> ComputeSegments()
        {
            // 1) build RAW runs of roughly constant speed (±5% tol)
            var raw = new List<Segment>();
            int n = _speeds.Count;
            if (n == 0) return raw;
            float tol = Math.Max(1f, _maxSpeed * 0.05f);

            int s0 = 0;
            float v0 = _speeds[0];
            DateTime t0 = _speedTimes[0];
            for (int i = 1; i < n; i++)
            {
                if (Math.Abs(_speeds[i] - v0) > tol)
                {
                    DateTime t1 = _speedTimes[i - 1];
                    raw.Add(new Segment
                    {
                        Start = s0,
                        Length = i - s0,
                        Speed = v0,
                        Duration = t1 - t0
                    });
                    s0 = i;
                    v0 = _speeds[i];
                    t0 = _speedTimes[i];
                }
            }
            // last run
            DateTime tend = _speedTimes[n - 1];
            raw.Add(new Segment
            {
                Start = s0,
                Length = n - s0,
                Speed = v0,
                Duration = tend - t0
            });

            // 2) if too many, merge into groups
            int maxSegs = _maxSegmentCount > 0
                          ? _maxSegmentCount
                          : Math.Max(1, Width / 16);
            if (raw.Count <= maxSegs)
                return raw;

            var merged = new List<Segment>();
            int groupSize = (int)Math.Ceiling(raw.Count / (double)maxSegs);
            for (int i = 0; i < raw.Count; i += groupSize)
            {
                int end = Math.Min(i + groupSize, raw.Count);
                var slice = raw.GetRange(i, end - i);

                // start & length
                int start = slice[0].Start;
                int len = slice.Sum(x => x.Length);

                // total duration & weighted average speed
                double totSec = slice.Sum(x => x.Duration.TotalSeconds);
                var dur = TimeSpan.FromSeconds(totSec);
                float avgSpd;
                if (totSec > 0)
                    avgSpd = (float)(slice.Sum(x => x.Speed * x.Duration.TotalSeconds) / totSec);
                else
                    avgSpd = slice.Average(x => x.Speed);

                merged.Add(new Segment
                {
                    Start = start,
                    Length = len,
                    Speed = avgSpd,
                    Duration = dur
                });
            }

            return merged;
        }


        static string FormatBytes(double b)
        {
            if (b >= 1 << 30) return (b / (1 << 30)).ToString("0.##") + " GB";
            if (b >= 1 << 20) return (b / (1 << 20)).ToString("0.##") + " MB";
            if (b >= 1 << 10) return (b / (1 << 10)).ToString("0.##") + " KB";
            return b.ToString("0") + " B";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _font.Dispose();
                _sf.Dispose();

               // _pulseTimer.Stop();
                _shimmerTimer.Stop();
                _rippleTimer.Stop();
                //_pulseTimer.Dispose();
                _shimmerTimer.Dispose();
                _rippleTimer.Dispose();

                _pauseTimer.Stop();
                _pauseTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        //—— Ensure parent is painted first ———————————————————————————

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x20;  // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (BackColor == Color.Transparent && Parent != null)
            {
                e.Graphics.TranslateTransform(-Left, -Top);
                var pe = new PaintEventArgs(e.Graphics,
                    new Rectangle(Point.Empty, Parent.ClientSize));
                InvokePaintBackground(Parent, pe);
                InvokePaint(Parent, pe);
                e.Graphics.TranslateTransform(Left, Top);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }


        private static double SafeRatio(long current, float max)
        {
            // Avoid zero/very small denominator and clamp to non-negative.
            double denom = Math.Max(1e-6, (double)max);
            double r = (double)current / denom;
            if (double.IsNaN(r) || double.IsInfinity(r)) r = 0.0;
            if (r < 0) r = 0.0;
            return r;
        }

        private static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Reset history (one sample per pixel), but do not leave _maxSpeed = 1
            _speeds.Clear();
            _fileSpeeds.Clear();
            _speedTimes.Clear();
            _lastPixel = 0;

            // Keep vertical scale sane relative to current speed so ratio can’t explode
            _maxSpeed = Math.Max(1f, (float)Math.Max(1, _currentSpeed));

            Invalidate();
        }

        //—— Mouse → orb hover —————————————————————————————————————————

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // (1) existing orb‐hover logic…
            /*bool now = _orbRect.Contains(e.Location);
            if (now != _hoverOrb)
            {
                _hoverOrb = now;
                InvalidateOrb();
            }*/

            // (2) segment‐hover + rich tooltip
            int hit = -1;
            var segs = ComputeSegments();
            if (segs.Count > 0 && e.X >= 0 && e.X < segs.Sum(s => s.Length))
            {
                // 2a) find which segment
                for (int i = 0; i < segs.Count; i++)
                    if (e.X >= segs[i].Start && e.X < segs[i].Start + segs[i].Length)
                    {
                        hit = i;
                        break;
                    }
            }

            if (hit != _hoverSegment)
            {
                _hoverSegment = hit;
                _toolTip.Hide(this);

                if (hit >= 0)
                {
                    var seg = segs[hit];
                    double totalT = segs.Sum(x => x.Duration.TotalSeconds);
                    double totalB = segs.Sum(x => x.Speed * x.Duration.TotalSeconds);

                    // 1) % time
                    double pctTime = totalT > 0
                                   ? seg.Duration.TotalSeconds / totalT * 100
                                   : 0;

                    // 3) % size (bytes)
                    double segBytes = seg.Speed * seg.Duration.TotalSeconds;
                    double pctSize = totalB > 0
                                     ? segBytes / totalB * 100
                                     : 0;

                    // 2) speed category via 20/40/60/80 percentiles
                    var speeds = segs.Select(x => x.Speed).OrderBy(x => x).ToList();
                    int M = speeds.Count;
                    float q20 = speeds[(int)Math.Floor(0.2 * (M - 1))];
                    float q40 = speeds[(int)Math.Floor(0.4 * (M - 1))];
                    float q60 = speeds[(int)Math.Floor(0.6 * (M - 1))];
                    float q80 = speeds[(int)Math.Floor(0.8 * (M - 1))];
                    string cat;
                    if (seg.Speed <= q20) cat = "Very Slow";
                    else if (seg.Speed <= q40) cat = "Slow";
                    else if (seg.Speed <= q60) cat = "Average";
                    else if (seg.Speed <= q80) cat = "Fast";
                    else cat = "Brutally Fast";


                    // NEW: files/s average & approx file count
                    double avgFilesPS = 0, approxFiles = 0;
                    if (_fileSpeeds.Count >= seg.Start + seg.Length)
                    {
                        var slice = _fileSpeeds.GetRange(seg.Start, seg.Length);
                        avgFilesPS = slice.Average();
                        approxFiles = avgFilesPS * seg.Duration.TotalSeconds;
                    }

                    // build the tooltip text
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine($"{FormatSpeed(seg.Speed)} ({cat})");
                    sb.AppendLine($"{FormatBytes(seg.Speed * seg.Duration.TotalSeconds)} ({pctSize:0.##}% of total)");
                    sb.AppendLine($"{seg.Duration.TotalSeconds:0.##} s ({pctTime:0.##}% of time)");
                    sb.AppendLine($"{avgFilesPS:0.##} avg files/s  (≈{approxFiles:0} approx files)");



                    _toolTip.Show(
                      sb.ToString(),
                      this,
                      e.Location.X + 15,
                      e.Location.Y + 15
                    );

                    Invalidate();
                    //Refresh();
                }
            }

            base.OnMouseMove(e);
        }


        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
           /* if (_hoverOrb)
            {
                _hoverOrb = false;
                InvalidateOrb();
            }*/
            if (_hoverSegment != -1)
            {
                _hoverSegment = -1;
                _toolTip.Hide(this);
            }
        }

      /*  void InvalidateOrb()
        {
            var r = Rectangle.Ceiling(
                RectangleF.Inflate(_orbRect, 10, 10)
            );
            Invalidate(r);
        }*/

        //—— Private state ——————————————————————————————————————————

        int _lastPixel;
        float _maxSpeed = 1f;
        List<float> _speeds = new List<float>();

        long _maximum = 100;
        long _value;
        double _progress;
        long _currentSpeed;
        long _filesPerSecond;
        bool _paused;

        // base colours
        Color _backTop = Color.FromArgb(192, Color.Green),
              _backBot = Color.FromArgb(32, Color.Green),
              _lineTop = Color.FromArgb(255, Color.Lime),
              _lineBot = Color.FromArgb(32, Color.Lime),
              _hatchFore = Color.FromArgb(128, Color.Lime),
              _hatchBack = Color.Transparent,
              _speedColor = Color.White,
              _textColor = Color.White;
        HatchStyle _hatchStyle = HatchStyle.BackwardDiagonal;

        readonly Font _font;
        readonly StringFormat _sf;

        //— new fields for orb/shimmer/ripple/LED ——————————————————————————

        private RectangleF _orbRect;
        private bool _hoverOrb;
        private Timer  _shimmerTimer, _rippleTimer; //_pulseTimer,
        private float _pulse, _shimmerPos;
        private bool _pulseForward = false;
        private float _rippleAmp, _ripplePhase;
        private long _lastSpeed;

        private bool _shimmerActive;
        private bool _rippleActive;

        // —— pause pulse for icon
        private float _pausePulse;
        private Timer _pauseTimer;

        // stores the bounding box of the two‐bar pause icon
        private RectangleF _pauseIconRect;

        // raised when user clicks the pause‐icon
        [Category("Action")]
        public event EventHandler PauseIconClicked;
        //—— Keep one speed‐sample per filled‐pixel + ripple trigger ————————


        protected override void OnMouseClick(MouseEventArgs e)
        {
            // if paused and click landed inside the icon, fire event
            if (_paused && _pauseIconRect.Contains(e.Location))
            {
                PauseIconClicked?.Invoke(this, EventArgs.Empty);
                return;    // swallow it so it doesn't turn into a normal Click
            }
            base.OnMouseClick(e);
        }


        void UpdateHistory()
        {
            int w = Width;
            int px = (int)((_value * w) / (float)_maximum);


            if (px > _lastPixel)
            {
                for (int i = 0; i < px - _lastPixel; i++)
                {
                    _fileSpeeds.Add(_filesPerSecond);    // << add this
                    _speeds.Add(_currentSpeed);
                    _speedTimes.Add(DateTime.Now);
                }
            }
            else if (px < _lastPixel)
            {
                int remove = _lastPixel - px;
                _speeds.RemoveRange(px, remove);
                _fileSpeeds.RemoveRange(px, remove);  // << and this
                _speedTimes.RemoveRange(px, remove);
            }

            _lastPixel = px;

            // BEFORE updating _maxSpeed, remember the old
            // remember old max
            // float oldMax = _maxSpeed;

            // if we need to grow the vertical scale…
            // if we need to grow the vertical scale…
            if (_currentSpeed > _maxSpeed)
            {
                _maxSpeed = _currentSpeed;

                // trigger shimmer
               // _shimmerActive = true;
                _shimmerPos = 0f;

                // trigger ripple
                _rippleActive = true;
                _ripplePhase = 0f;
                _rippleAmp = 8f;
            }

            // if speed jumped >5% of max, create a ripple
            //long diff = Math.Abs(_currentSpeed - _lastSpeed);
            //if (diff > Math.Max(1, _maxSpeed * 0.05f))
            //{
            //    _rippleAmp = 5f;
            //    _ripplePhase = 0f;
            //}
            _lastSpeed = _currentSpeed;
        }

        //—— Paint everything —————————————————————————————————————————

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            int W = ClientSize.Width, H = ClientSize.Height;
            if (W <= 0 || H <= 0) return;

            // progress width (never more than control width)
            float px = Math.Max(0, Math.Min(W, (float)_speeds.Count));

            // ONLY paint the background gradient up to current progress (px)
            // Leave the rest transparent so the parent shows through.
            if (px > 0.5f)
            {
                using (var bg = new LinearGradientBrush(
                           new RectangleF(0f, 0f, px, H),
                           _backBot, _backTop,
                           LinearGradientMode.Vertical))
                {
                    g.FillRectangle(bg, 0f, 0f, px, H);
                }
            }

            // Compute a safe yLine
            double ratio = SafeRatio(_currentSpeed, Math.Max(1f, _maxSpeed));
            double capped = Math.Min(1.0, ratio);
            float yLine = (float)(H * (1.0 - capped));
            if (float.IsNaN(yLine) || float.IsInfinity(yLine)) yLine = 0f;
            yLine = Clamp(yLine, 0f, H);

            // Draw the curve if we have enough samples
            GraphicsPath curvePath = null;
            if (_speeds.Count >= 2)
            {
                int n = _speeds.Count;

                var pts = new PointF[n + 2];
                pts[0] = new PointF(0, H);
                for (int i = 0; i < n; i++)
                {
                    float baseY = H - (_speeds[i] / Math.Max(1e-6f, _maxSpeed)) * H;
                    float rip = _rippleActive ? _rippleAmp * (float)Math.Sin(i * 0.3f + _ripplePhase) : 0f;
                    float y = Clamp(baseY + rip, 0f, H);
                    pts[i + 1] = new PointF(i, y);
                }
                pts[n + 1] = new PointF(n - 1, H);

                curvePath = new GraphicsPath();
                curvePath.AddPolygon(pts);

                // Overlays ONLY in the curve (line gradient + hatch).
                // We no longer fill a full rect in 'bounds' here, because base bg up to px is already painted.
                var bounds = curvePath.GetBounds();
                if (bounds.Width > 0 && bounds.Height > 0)
                {
                    using (var lg = new LinearGradientBrush(bounds, _lineBot, _lineTop, LinearGradientMode.Vertical))
                        g.FillPath(lg, curvePath);

                    using (var hb = new HatchBrush(_hatchStyle, _hatchFore, _hatchBack))
                        g.FillPath(hb, curvePath);
                }

                // Segment tint
                var segs = ComputeSegments();
                g.SetClip(curvePath);
                Color segDarkBase = LerpColor(_lineTop, Color.Black, 0.9999f);
                Color darkBaseCol = LerpColor(segDarkBase, _lineTop, 0.35f);

                for (int i = 0; i < segs.Count; i++)
                {
                    var seg = segs[i];
                    float t = Clamp(seg.Speed / Math.Max(1e-6f, _maxSpeed), 0f, 1f);
                    Color baseCol = LerpColor(segDarkBase, _lineTop, t);
                    Color fillCol = (_hoverSegment == -1)
                        ? Color.FromArgb(100, baseCol)
                        : (i == _hoverSegment ? darkBaseCol : Color.FromArgb(100, baseCol));

                    g.FillRectangle(new SolidBrush(fillCol), seg.Start, 0, seg.Length, H);
                }
                g.ResetClip();
            }
            else
            {
                // Not enough samples for a curved path yet.
                // Paint a simple overlay up to px so it looks alive immediately.
                if (px > 0.5f)
                {
                    using (var lg = new LinearGradientBrush(
                               new RectangleF(0, 0, px, H),
                               _lineBot, _lineTop,
                               LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(lg, 0, 0, px, H);
                    }
                    using (var hb = new HatchBrush(_hatchStyle, _hatchFore, _hatchBack))
                        g.FillRectangle(hb, 0, 0, px, H);
                }
            }

            // Current-speed horizontal line
            try
            {
                float yDraw = Clamp(yLine, 0.5f, Math.Max(0.5f, H - 0.5f));
                using (var pen = new Pen(_speedColor, 2f))
                    g.DrawLine(pen, 0f, yDraw, W, yDraw);
            }
            catch { }

            // Speed/files label
            string txt = FormatSpeed(_currentSpeed) + "\n" + _filesPerSecond + " files/s";
            SizeF sz = g.MeasureString(txt, _font);
            float tx = px - sz.Width;
            if (tx < 4) tx = 4;
            if (tx + sz.Width > W) tx = W - sz.Width;

            float ty = (yLine < sz.Height + 4) ? yLine + 2 : yLine - sz.Height - 2;
            ty = Clamp(ty, 0, Math.Max(0, H - sz.Height));

            using (var tb = new SolidBrush(_textColor))
                g.DrawString(txt, _font, tb, tx, ty, _sf);

            // Small orb
            {
                float r = 3f;
                float cx = Clamp(px, 0, W);
                float cy = Clamp(yLine, 0, H);
                _orbRect = new RectangleF(cx - r, cy - r, 2 * r, 2 * r);

                using (var gp = new GraphicsPath())
                {
                    gp.AddEllipse(_orbRect);
                    using (var pg = new PathGradientBrush(gp))
                    {
                        var c = _lineTop;
                        pg.CenterColor = Color.FromArgb(100, c);
                        pg.SurroundColors = new[] { Color.FromArgb(0, c) };
                        g.FillPath(pg, gp);
                    }
                }
                using (var ofb = new SolidBrush(_lineTop))
                    g.FillEllipse(ofb, _orbRect);
                using (var op = new Pen(_lineBot, 1f))
                    g.DrawEllipse(op, _orbRect);
            }

            // Pause overlay kept as-is (your existing code) — it uses yLine safely now.
            if (_paused)
            {
                using (var fade = new SolidBrush(Color.FromArgb(32, Color.Black)))
                    g.FillRectangle(fade, ClientRectangle);

                float p = 0.5f + 0.5f * (float)Math.Sin(_pausePulse * 2 * Math.PI);
                var clr = Color.FromArgb((int)(p * 200), Color.White);

                int iconSize = Math.Min(W, H) / 4;
                float barW = iconSize * 0.2f;
                float barH = iconSize;
                float gap = barW;
                float cx = W / 2f;
                float cy = H / 2f;
                float x1 = cx - gap / 2f - barW;
                float y1 = cy - barH / 2f;
                var r1 = new RectangleF(x1, y1, barW, barH);
                float x2 = cx + gap / 2f;
                var r2 = new RectangleF(x2, y1, barW, barH);

                _pauseIconRect = new RectangleF(x1, y1, (x2 + barW) - x1, barH);

                using (var br = new SolidBrush(clr))
                {
                    g.FillRectangle(br, r1);
                    g.FillRectangle(br, r2);
                }
            }
        }

        //—— Helpers ————————————————————————————————————————————————

        static string FormatSpeed(double bps)
        {
            if (bps >= 1 << 20) return (bps / (1 << 20)).ToString("0.##") + " MB/s";
            if (bps >= 1 << 10) return (bps / (1 << 10)).ToString("0.##") + " KB/s";
            return bps.ToString("0") + " B/s";
        }

        static Color LerpColor(Color a, Color b, float t)
        {
            t = Math.Max(0, Math.Min(1, t));
            int A = a.A + (int)((b.A - a.A) * t);
            int R = a.R + (int)((b.R - a.R) * t);
            int G = a.G + (int)((b.G - a.G) * t);
            int B = a.B + (int)((b.B - a.B) * t);
            return Color.FromArgb(A, R, G, B);
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        public BrutalBarSnapshot ExportSnapshot()
        {
            // Safe defaults
            var times = _speedTimes.Count == _speeds.Count
                      ? _speedTimes
                      : new List<DateTime>(Enumerable.Repeat(DateTime.Now, _speeds.Count));

            return new BrutalBarSnapshot
            {
                Version = 1,
                Width = this.Width,
                Height = this.Height,

                Maximum = _maximum,
                Value = _value,
                CurrentSpeed = _currentSpeed,
                FilesPerSecond = _filesPerSecond,
                Paused = _paused,
                MaxSegmentCount = _maxSegmentCount,
                MaxSpeed = _maxSpeed <= 0 ? Math.Max(1f, _speeds.DefaultIfEmpty(0f).Max()) : _maxSpeed,

                Speeds = _speeds.ToArray(),
                FileSpeeds = _fileSpeeds.ToArray(),
                SpeedTimeTicks = times.Select(t => t.Ticks).ToArray(),

                BackTopArgb = _backTop.ToArgb(),
                BackBotArgb = _backBot.ToArgb(),
                LineTopArgb = _lineTop.ToArgb(),
                LineBotArgb = _lineBot.ToArgb(),
                HatchForeArgb = _hatchFore.ToArgb(),
                HatchBackArgb = _hatchBack.ToArgb(),
                SpeedColorArgb = _speedColor.ToArgb(),
                TextColorArgb = _textColor.ToArgb(),
                HatchStyle = (int)_hatchStyle
            };
        }

        public string ExportJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(ExportSnapshot(), options);
        }



        public void ImportSnapshot(BrutalBarSnapshot s, ImportMode mode = ImportMode.ResizeControlToSnapshot, bool suppressAnimations = true)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            // Optionally match the saved size for pixel-perfect restore
            if (mode == ImportMode.ResizeControlToSnapshot)
            {
                // Set Size first (OnResize will clear history), then we repopulate it below
                this.Size = new Size(s.Width, s.Height);
            }

            // 1) Restore palette & style
            _backTop = Color.FromArgb(s.BackTopArgb);
            _backBot = Color.FromArgb(s.BackBotArgb);
            _lineTop = Color.FromArgb(s.LineTopArgb);
            _lineBot = Color.FromArgb(s.LineBotArgb);
            _hatchFore = Color.FromArgb(s.HatchForeArgb);
            _hatchBack = Color.FromArgb(s.HatchBackArgb);
            _speedColor = Color.FromArgb(s.SpeedColorArgb);
            _textColor = Color.FromArgb(s.TextColorArgb);
            _hatchStyle = (HatchStyle)s.HatchStyle;

            // 2) Restore basic fields (avoid Value property to not rebuild history)
            _maximum = Math.Max(1, s.Maximum);
            _currentSpeed = s.CurrentSpeed;
            _filesPerSecond = s.FilesPerSecond;
            _paused = s.Paused;
            _maxSegmentCount = Math.Max(0, s.MaxSegmentCount);

            // 3) Stop or suppress ongoing animations if requested (you can re-enable later)
            if (suppressAnimations)
            {
                _shimmerActive = false;
                _rippleActive = false;
                _rippleAmp = 0;
                _ripplePhase = 0;
                _pausePulse = 0;
            }

            // 4) Rebuild history lists
            var speeds = s.Speeds?.ToList() ?? new List<float>();
            var fileSpeeds = s.FileSpeeds?.ToList() ?? new List<long>();
            var times = (s.SpeedTimeTicks?.Select(t => new DateTime(t)).ToList()) ?? new List<DateTime>();

            // Ensure parallel arrays are the same length as speeds
            SyncParallelHistoryLengths(speeds, fileSpeeds, times);

            // Resample if asked and width differs
            if (mode == ImportMode.ResampleToCurrentWidth && speeds.Count > 0 && this.Width > 0 && speeds.Count != this.Width)
            {
                var resampledSpeeds = ResampleFloatSeries(speeds, this.Width);
                var resampledFileSpeeds = ResampleLongSeries(speeds, fileSpeeds, this.Width);
                var resampledTimes = InterpolateTimes(times, this.Width);

                speeds = resampledSpeeds;
                fileSpeeds = resampledFileSpeeds;
                times = resampledTimes;
            }

            // Assign lists to fields
            _speeds.Clear();
            _speeds.AddRange(speeds);

            _fileSpeeds.Clear();
            _fileSpeeds.AddRange(fileSpeeds);

            _speedTimes.Clear();
            _speedTimes.AddRange(times);

            // 5) Vertical scale
            _maxSpeed = s.MaxSpeed > 0 ? s.MaxSpeed : Math.Max(1f, _speeds.DefaultIfEmpty(0f).Max());

            // 6) Set _value so that px roughly matches history length, unless keeping the saved one
            if (mode == ImportMode.ResizeControlToSnapshot)
            {
                // Use saved Value (pixel mapping equals snapshot Width)
                _value = Math.Max(0, Math.Min(_maximum, s.Value));
            }
            else if (mode == ImportMode.KeepSizeUseSavedValue)
            {
                // Use saved Value, may look different if widths differ
                _value = Math.Max(0, Math.Min(_maximum, s.Value));
            }
            else // Resample mode already matched width, so compute value from speeds count
            {
                if (this.Width > 0)
                {
                    long vFromPx = (long)Math.Round((double)_maximum * _speeds.Count / this.Width);
                    _value = Math.Max(0, Math.Min(_maximum, vFromPx));
                }
                else
                {
                    _value = Math.Max(0, Math.Min(_maximum, s.Value));
                }
            }

            // 7) LastPixel should reflect the history
            _lastPixel = _speeds.Count;

            // 8) Derived progress
            _progress = (_maximum > 0) ? (_value * 100.0 / _maximum) : 0.0;

            // 9) Clean up UI artifacts
            _hoverSegment = -1;
            _toolTip?.Hide(this);

            Invalidate();
        }

        public void ImportJson(string json, ImportMode mode = ImportMode.ResizeControlToSnapshot, bool suppressAnimations = true)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            var snapshot = JsonSerializer.Deserialize<BrutalBarSnapshot>(json);
            if (snapshot == null) throw new InvalidOperationException("Invalid snapshot JSON.");
            ImportSnapshot(snapshot, mode, suppressAnimations);
        }

       
        //public void ImportGroBuf(string json, ImportMode mode = ImportMode.ResizeControlToSnapshot, bool suppressAnimations = true)
        //{
        //    if (json == null) throw new ArgumentNullException(nameof(json));
        //    var snapshot = JsonSerializer.Deserialize<BrutalBarSnapshot>(json);
        //    if (snapshot == null) throw new InvalidOperationException("Invalid snapshot JSON.");
        //    ImportSnapshot(snapshot, mode, suppressAnimations);
        //}


        // Make all parallel lists the same length as speeds (truncate/pad)
        private static void SyncParallelHistoryLengths(List<float> speeds, List<long> fileSpeeds, List<DateTime> times)
        {
            int n = speeds.Count;

            void AdjustList<T>(List<T> list, T pad)
            {
                if (list.Count > n) list.RemoveRange(n, list.Count - n);
                else if (list.Count < n) list.AddRange(Enumerable.Repeat(pad, n - list.Count));
            }

            long lastFile = fileSpeeds.Count > 0 ? fileSpeeds[fileSpeeds.Count - 1] : 0;
            DateTime lastTime = times.Count > 0 ? times[times.Count - 1] : DateTime.Now;

            AdjustList(fileSpeeds, lastFile);
            AdjustList(times, lastTime);
        }

        private static List<float> ResampleFloatSeries(IList<float> src, int targetCount)
        {
            var dst = new List<float>(targetCount);
            if (src.Count == 0 || targetCount <= 0) return dst;
            if (src.Count == targetCount) return new List<float>(src);

            if (targetCount == 1) { dst.Add(src[0]); return dst; }

            double scale = (src.Count - 1) / (double)(targetCount - 1);
            for (int i = 0; i < targetCount; i++)
            {
                double x = i * scale;
                int i0 = (int)Math.Floor(x);
                int i1 = Math.Min(i0 + 1, src.Count - 1);
                double t = x - i0;
                double y = src[i0] * (1 - t) + src[i1] * t;
                dst.Add((float)y);
            }
            return dst;
        }

        // Resample file-speeds using the same x mapping as the speeds, then round
        private static List<long> ResampleLongSeries(IList<float> speedsRef, IList<long> fileSpeeds, int targetCount)
        {
            if (fileSpeeds.Count == 0) return new List<long>(Enumerable.Repeat(0L, targetCount));
            var src = fileSpeeds.Select(v => (double)v).ToList();

            var dst = new List<long>(targetCount);
            if (src.Count == targetCount) return src.Select(d => (long)Math.Round(d)).ToList();

            if (targetCount == 1) { dst.Add((long)Math.Round(src[0])); return dst; }

            double scale = (src.Count - 1) / (double)(targetCount - 1);
            for (int i = 0; i < targetCount; i++)
            {
                double x = i * scale;
                int i0 = (int)Math.Floor(x);
                int i1 = Math.Min(i0 + 1, src.Count - 1);
                double t = x - i0;
                double y = src[i0] * (1 - t) + src[i1] * t;
                dst.Add((long)Math.Round(y));
            }
            return dst;
        }

        // If we resample, we won’t have per-pixel real timestamps; synthesize evenly spread timestamps over the original span.
        private static List<DateTime> InterpolateTimes(IList<DateTime> original, int targetCount)
        {
            var dst = new List<DateTime>(targetCount);
            if (original.Count == 0 || targetCount <= 0) return dst;

            var t0 = original.First();
            var t1 = original.Last();
            if (targetCount == 1) { dst.Add(t0); return dst; }

            var totalTicks = Math.Max(0L, t1.Ticks - t0.Ticks);
            for (int i = 0; i < targetCount; i++)
            {
                long ticks = t0.Ticks + (long)Math.Round(totalTicks * (i / (double)(targetCount - 1)));
                dst.Add(new DateTime(ticks));
            }
            return dst;
        }

    }

    public enum ImportMode
    {
        // Keep current Size; use saved Value to compute px (may look different if widths differ).
        KeepSizeUseSavedValue,

        // Change this control's Size to match the snapshot for a pixel-perfect restore.
        ResizeControlToSnapshot,

        // Keep current Size and resample the saved history to the current width.
        ResampleToCurrentWidth
    }

    [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    [Serializable]
    public sealed class BrutalBarSnapshot
    {
        public int Version { get; set; } = 1;

        // Geometry
        public int Width { get; set; }
        public int Height { get; set; }

        // Core values
        public long Maximum { get; set; }
        public long Value { get; set; }
        public long CurrentSpeed { get; set; }
        public long FilesPerSecond { get; set; }
        public bool Paused { get; set; }
        public int MaxSegmentCount { get; set; }

        // Vertical scale
        public float MaxSpeed { get; set; }

        // History (one sample per pixel)
        public float[] Speeds { get; set; } = Array.Empty<float>();
        public long[] FileSpeeds { get; set; } = Array.Empty<long>();
        public long[] SpeedTimeTicks { get; set; } = Array.Empty<long>(); // DateTime.Ticks

        // Colors and style
        public int BackTopArgb { get; set; }
        public int BackBotArgb { get; set; }
        public int LineTopArgb { get; set; }
        public int LineBotArgb { get; set; }
        public int HatchForeArgb { get; set; }
        public int HatchBackArgb { get; set; }
        public int SpeedColorArgb { get; set; }
        public int TextColorArgb { get; set; }
        public int HatchStyle { get; set; }
    }


}
