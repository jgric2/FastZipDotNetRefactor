using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace YourNamespace
{

    public enum DropDownMode
    {
        Whole,
        Split
    }

    public enum ArrowAlignment
    {
        Left,
        Center,
        Right
    }

    public class DropDownButton : Button
    {
        private bool _arrowPressed;
        private bool _isCleaningUp;

        // Reused dropdown and host
        private ToolStripDropDown _activeDropDown;
        private ToolStripControlHost _host;

        // Original placement of hosted control
        private Control _origParent;
        private Rectangle _origBounds;
        private int _origIndex;
        private bool _origVisible;

        // Color freezing (to avoid ambient color change when reparented)
        private bool _foreWasExplicit, _backWasExplicit;
        private Color _savedFore, _savedBack;

        public DropDownButton()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            Mode = DropDownMode.Whole;
            ArrowAreaWidth = 18;
            ShowDropDownOnClick = true;

            ArrowAlignment = ArrowAlignment.Right;
            ArrowColor = Color.Empty;
            ArrowPadding = 6;

            DropDownDirection = ToolStripDropDownDirection.BelowLeft;
            DropDownOffset = Point.Empty;
            CloseOnDropDownControlLostFocus = true;
            MatchDropDownHostColors = true;

            UseMnemonic = true;
        }

        [Category("Behavior")]
        [DefaultValue(null)]
        public ContextMenuStrip DropDownMenu { get; set; }

        [Category("Behavior")]
        [DefaultValue(typeof(DropDownMode), "Whole")]
        public DropDownMode Mode { get; set; }

        [Category("Appearance")]
        [DefaultValue(18)]
        [Description("Width (in pixels) of the arrow segment in Split mode.")]
        public int ArrowAreaWidth { get; set; }

        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("If true, automatically shows the drop-down when triggered.")]
        public bool ShowDropDownOnClick { get; set; }

        [Category("Appearance")]
        [DefaultValue(typeof(ArrowAlignment), "Right")]
        [Description("Position of the arrow glyph when Mode=Whole. Ignored in Split mode.")]
        public ArrowAlignment ArrowAlignment { get; set; }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Empty")]
        [Description("Color of the arrow glyph. Empty uses ForeColor; disabled uses GrayText.")]
        public Color ArrowColor { get; set; }

        [Category("Appearance")]
        [DefaultValue(6)]
        [Description("Padding from the left/right edge when ArrowAlignment is Left/Right.")]
        public int ArrowPadding { get; set; }

        [Category("Behavior")]
        [DefaultValue(null)]
        [Description("Control to show as the drop-down (e.g., a Panel). Will be reparented temporarily while open.")]
        [TypeConverter(typeof(ReferenceConverter))]
        public Control DropDownControl { get; set; }

        [Category("Behavior")]
        [DefaultValue(typeof(ToolStripDropDownDirection), "BelowLeft")]
        [Description("Where to show the drop-down relative to the button.")]
        public ToolStripDropDownDirection DropDownDirection { get; set; }

        [Category("Behavior")]
        [DefaultValue(typeof(Point), "0,0")]
        [Description("Pixel offset to apply to the calculated drop-down location.")]
        public Point DropDownOffset { get; set; }

        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("If true, closes the control-based drop-down when focus leaves it.")]
        public bool CloseOnDropDownControlLostFocus { get; set; }

        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Match ToolStripDropDown/host colors to the DropDownControl while open.")]
        public bool MatchDropDownHostColors { get; set; }

        public event EventHandler DropDownClicked;
        public event CancelEventHandler DropDownOpening;
        public event EventHandler DropDownOpened;
        public event ToolStripDropDownClosedEventHandler DropDownClosed;
        public event EventHandler DropDownContentLostFocus;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            var g = pevent.Graphics;

            //if (Mode == DropDownMode.Split)
            //{
            //    var arrowArea = GetArrowAreaRectangle();
            //    int sepX = RightToLeft == RightToLeft.No ? arrowArea.Left : arrowArea.Right;

            //    using (var p1 = new Pen(SystemColors.ButtonHighlight))
            //    using (var p2 = new Pen(SystemColors.ButtonShadow))
            //    {
            //        g.DrawLine(p1, sepX, 4, sepX, Height - 5);
            //        int delta = RightToLeft == RightToLeft.No ? -1 : 1;
            //        g.DrawLine(p2, sepX + delta, 4, sepX + delta, Height - 5);
            //    }
            //}

            var glyphRect = GetArrowGlyphRectangle();
            var color = Enabled ? (ArrowColor.IsEmpty ? ForeColor : ArrowColor) : SystemColors.GrayText;
            DrawArrowGlyph(g, glyphRect, color);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (!Enabled)
            {
                base.OnMouseDown(mevent);
                return;
            }

            bool inArrowArea = IsInArrowArea(mevent.Location);

            if (Mode == DropDownMode.Whole || inArrowArea)
            {
                _arrowPressed = true;
                Invalidate(GetArrowAreaRectangle());

                OnDropDownTriggered();
                return; // suppress base click for dropdown trigger
            }

            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (_arrowPressed)
            {
                _arrowPressed = false;
                Invalidate(GetArrowAreaRectangle());
                return;
            }
            base.OnMouseUp(mevent);
        }

        protected override void OnClick(EventArgs e)
        {
            if (_arrowPressed)
                return;

            // In Split mode, clicking main area behaves like a normal button
            base.OnClick(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Down || keyData == (Keys.Alt | Keys.Down) || keyData == Keys.F4)
                return true;
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!Enabled)
                return;

            if (e.Alt && e.KeyCode == Keys.Down || e.KeyCode == Keys.F4)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                OnDropDownTriggered();
            }
            else if (e.KeyCode == Keys.Down && Mode == DropDownMode.Whole)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                OnDropDownTriggered();
            }
        }

        private void OnDropDownTriggered()
        {
            DropDownClicked?.Invoke(this, EventArgs.Empty);

            if (!ShowDropDownOnClick)
                return;

            var args = new CancelEventArgs();
            DropDownOpening?.Invoke(this, args);
            if (args.Cancel)
                return;

            if (DropDownControl != null)
            {
                ShowControlDropDown();
            }
            else if (DropDownMenu != null)
            {
                var pt = GetDropDownShowPoint();
                try
                {
                    DropDownMenu.Show(this, pt, DropDownDirection);
                    DropDownOpened?.Invoke(this, EventArgs.Empty);
                    DropDownMenu.Closed += (s, e) => DropDownClosed?.Invoke(this, e);
                }
                catch { }
            }
        }

        private Point GetDropDownShowPoint()
        {
            switch (DropDownDirection)
            {
                case ToolStripDropDownDirection.AboveLeft:
                    return new Point(0, -1);
                case ToolStripDropDownDirection.AboveRight:
                    return new Point(Width, -1);
                case ToolStripDropDownDirection.BelowRight:
                    return new Point(Width, Height);
                case ToolStripDropDownDirection.BelowLeft:
                default:
                    return new Point(0, Height);
            }
        }

        private static bool IsExplicitColor(Control c, string propName)
        {
            var pd = TypeDescriptor.GetProperties(c)[propName];
            return pd != null && pd.ShouldSerializeValue(c);
        }

        private void ShowControlDropDown()
        {
            if (DropDownControl == null) return;

            if (_activeDropDown != null && !_activeDropDown.IsDisposed && _activeDropDown.Visible)
            {
                CloseDropDown();
                return;
            }

            // Create or reuse dropdown
            if (_activeDropDown == null || _activeDropDown.IsDisposed)
            {
                _activeDropDown = new ToolStripDropDown
                {
                    AutoClose = true,
                    Padding = Padding.Empty,
                    Margin = Padding.Empty
                };
                _activeDropDown.Closed += ActiveDropDown_Closed;
                _activeDropDown.Opened += (s, e) => DropDownOpened?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                _activeDropDown.Items.Clear();
            }

            // Remember original placement BEFORE hosting
            _origParent = DropDownControl.Parent;
            _origBounds = DropDownControl.Bounds;
            _origVisible = DropDownControl.Visible;
            _origIndex = _origParent != null ? _origParent.Controls.GetChildIndex(DropDownControl) : -1;

            // Freeze colors so reparenting doesn't change look
            _foreWasExplicit = IsExplicitColor(DropDownControl, "ForeColor");
            _backWasExplicit = IsExplicitColor(DropDownControl, "BackColor");

            if (_foreWasExplicit)
                _savedFore = DropDownControl.ForeColor;
            else
            {
                _savedFore = Color.Empty; // ambient
                DropDownControl.ForeColor = DropDownControl.ForeColor; // freeze effective
            }

            if (_backWasExplicit)
                _savedBack = DropDownControl.BackColor;
            else
            {
                _savedBack = Color.Empty;
                DropDownControl.BackColor = DropDownControl.BackColor;
            }

            // Create host for the control
            _host = new ToolStripControlHost(DropDownControl)
            {
                AutoSize = false,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            _host.Size = DropDownControl.Size;

            if (MatchDropDownHostColors)
            {
                _activeDropDown.ForeColor = DropDownControl.ForeColor;
                _activeDropDown.BackColor = DropDownControl.BackColor;
                _host.ForeColor = DropDownControl.ForeColor;
                _host.BackColor = DropDownControl.BackColor;
            }

            _activeDropDown.Items.Add(_host);

            if (CloseOnDropDownControlLostFocus)
            {
                DropDownControl.LostFocus += DropDownControl_LostFocus;
                DropDownControl.Leave += DropDownControl_LostFocus;
            }

            var basePoint = GetDropDownShowPoint();
            var showPoint = new Point(basePoint.X + DropDownOffset.X, basePoint.Y + DropDownOffset.Y);

            _activeDropDown.Show(this, showPoint, DropDownDirection);
            DropDownControl.Visible = true;
            DropDownControl.Focus();
        }

        private void DropDownControl_LostFocus(object sender, EventArgs e)
        {
            if (_activeDropDown == null) return;

            BeginInvoke((MethodInvoker)delegate
            {
                var dd = _activeDropDown;
                if (dd == null || dd.IsDisposed) return;

                try
                {
                    if (!dd.ContainsFocus && !ContainsFocus)
                    {
                        DropDownContentLostFocus?.Invoke(this, EventArgs.Empty);
                        dd.Close(ToolStripDropDownCloseReason.AppFocusChange);
                    }
                }
                catch (ObjectDisposedException)
                {
                    // safe to ignore
                }
            });
        }

        private void ActiveDropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            if (_isCleaningUp) return;
            _isCleaningUp = true;

            try
            {
                DropDownClosed?.Invoke(this, e);

                if (DropDownControl != null)
                {
                    DropDownControl.LostFocus -= DropDownControl_LostFocus;
                    DropDownControl.Leave -= DropDownControl_LostFocus;
                }

                CleanupDropDown(false);
            }
            finally
            {
                _isCleaningUp = false;
            }
        }

        private void CleanupDropDown(bool suppressRestore)
        {
            var dd = _activeDropDown;

            try
            {
                if (!suppressRestore && DropDownControl != null && !DropDownControl.IsDisposed)
                {
                    if (_origParent != null && !_origParent.IsDisposed)
                    {
                        DropDownControl.Visible = false;

                        _origParent.SuspendLayout();
                        try
                        {
                            _origParent.Controls.Add(DropDownControl);
                            DropDownControl.Bounds = _origBounds;
                            if (_origIndex >= 0)
                                _origParent.Controls.SetChildIndex(DropDownControl, _origIndex);

                            // Restore exact color behavior
                            DropDownControl.ForeColor = _foreWasExplicit ? _savedFore : Color.Empty;
                            DropDownControl.BackColor = _backWasExplicit ? _savedBack : Color.Empty;

                            DropDownControl.Visible = _origVisible;
                            if (dd != null) dd.Hide();
                        }
                        finally
                        {
                            _origParent.ResumeLayout();
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (dd != null)
                {
                    try { if (_host != null) dd.Items.Remove(_host); } catch { }
                    // keep dd for reuse; dispose in our Dispose() only
                }

                _host = null;
                _origParent = null;
                _origIndex = -1;

                _foreWasExplicit = _backWasExplicit = false;
                _savedFore = _savedBack = Color.Empty;
            }
        }

        public void CloseDropDown()
        {
            var dd = _activeDropDown;
            if (dd != null && !dd.IsDisposed && dd.Visible)
            {
                try { dd.Close(ToolStripDropDownCloseReason.CloseCalled); }
                catch { }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { CloseDropDown(); } catch { }
                if (DropDownControl != null)
                {
                    DropDownControl.LostFocus -= DropDownControl_LostFocus;
                    DropDownControl.Leave -= DropDownControl_LostFocus;
                }
                if (_activeDropDown != null)
                {
                    try { _activeDropDown.Closed -= ActiveDropDown_Closed; } catch { }
                    try { if (!_activeDropDown.IsDisposed) _activeDropDown.Dispose(); } catch { }
                    _activeDropDown = null;
                }
            }
            base.Dispose(disposing);
        }

        private bool IsInArrowArea(Point p)
        {
            return GetArrowAreaRectangle().Contains(p);
        }

        private Rectangle GetArrowAreaRectangle()
        {
            if (Mode == DropDownMode.Whole)
                return ClientRectangle;

            int w = Math.Max(12, ArrowAreaWidth);
            bool rtl = RightToLeft == RightToLeft.Yes;
            return rtl
                ? new Rectangle(0, 0, w, Height)
                : new Rectangle(Width - w, 0, w, Height);
        }

        private Rectangle GetArrowGlyphRectangle()
        {
            int size = Math.Min(16, Height - 8);
            int glyphW = size;
            int glyphH = size;

            if (Mode == DropDownMode.Split)
            {
                var area = GetArrowAreaRectangle();
                int x = area.Left + (area.Width - glyphW) / 2;
                int y = area.Top + (area.Height - glyphH) / 2;
                return new Rectangle(x, y, glyphW, glyphH);
            }

            int yWhole = (Height - glyphH) / 2;
            int xWhole;
            switch (ArrowAlignment)
            {
                case ArrowAlignment.Left:
                    xWhole = ArrowPadding;
                    break;
                case ArrowAlignment.Center:
                    xWhole = (Width - glyphW) / 2;
                    break;
                case ArrowAlignment.Right:
                default:
                    xWhole = Width - glyphW - ArrowPadding;
                    break;
            }

            return new Rectangle(Math.Max(0, xWhole), Math.Max(0, yWhole),
                                 Math.Min(glyphW, Width), Math.Min(glyphH, Height));
        }

        private void DrawArrowGlyph(Graphics g, Rectangle bounds, Color color)
        {
            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2;

            int half = Math.Max(3, Math.Min(bounds.Width, bounds.Height) / 4);
            Point p1 = new Point(cx - half, cy - half / 2);
            Point p2 = new Point(cx + half, cy - half / 2);
            Point p3 = new Point(cx, cy + half);

            using (var b = new SolidBrush(color))
                g.FillPolygon(b, new[] { p1, p2, p3 });

            if (_arrowPressed && Enabled)
            {
                using (var pen = new Pen(Color.FromArgb(60, Color.Black)))
                    g.DrawRectangle(pen, Rectangle.Inflate(GetArrowAreaRectangle(), -1, -1));
            }
        }
    }



    //public enum DropDownMode
    //{
    //    Whole,
    //    Split
    //}

    //public enum ArrowAlignment
    //{
    //    Left,
    //    Center,
    //    Right
    //}

    //public class DropDownButton : Button
    //{

    //    private bool _savedAmbientForeColor;
    //    private bool _savedAmbientBackColor;
    //    private Color _savedForeColorValue;
    //    private Color _savedBackColorValue;

    //    private bool _isCleaningUp;
    //    private bool _arrowPressed;

    //    // Hosting fields for Control-based dropdown
    //    private ToolStripDropDown _activeDropDown;
    //    private ToolStripControlHost _host;
    //    private Control _origParent;
    //    private Rectangle _origBounds;
    //    private int _origIndex;
    //    private bool _origVisible;

    //    public DropDownButton()
    //    {
    //        SetStyle(ControlStyles.ResizeRedraw, true);
    //        AutoSizeMode = AutoSizeMode.GrowAndShrink;

    //        Mode = DropDownMode.Whole;
    //        ArrowAreaWidth = 18;
    //        ShowDropDownOnClick = true;

    //        ArrowAlignment = ArrowAlignment.Right;
    //        ArrowColor = Color.Empty;
    //        ArrowPadding = 6;

    //        DropDownDirection = ToolStripDropDownDirection.BelowLeft;
    //        DropDownOffset = Point.Empty;
    //        CloseOnDropDownControlLostFocus = true;

    //        UseMnemonic = true;
    //    }

    //    [Category("Behavior")]
    //    [DefaultValue(null)]
    //    public ContextMenuStrip DropDownMenu { get; set; }

    //    [Category("Behavior")]
    //    [DefaultValue(typeof(DropDownMode), "Whole")]
    //    public DropDownMode Mode { get; set; }

    //    [Category("Appearance")]
    //    [DefaultValue(18)]
    //    [Description("Width (in pixels) of the arrow segment in Split mode.")]
    //    public int ArrowAreaWidth { get; set; }

    //    [Category("Behavior")]
    //    [DefaultValue(true)]
    //    [Description("If true, automatically shows the drop-down when triggered.")]
    //    public bool ShowDropDownOnClick { get; set; }

    //    [Category("Appearance")]
    //    [DefaultValue(typeof(ArrowAlignment), "Right")]
    //    [Description("Position of the arrow glyph when Mode=Whole. Ignored in Split mode.")]
    //    public ArrowAlignment ArrowAlignment { get; set; }

    //    [Category("Appearance")]
    //    [DefaultValue(typeof(Color), "Empty")]
    //    [Description("Color of the arrow glyph. Empty uses ForeColor; disabled uses GrayText.")]
    //    public Color ArrowColor { get; set; }

    //    [Category("Appearance")]
    //    [DefaultValue(6)]
    //    [Description("Padding from the left/right edge when ArrowAlignment is Left/Right.")]
    //    public int ArrowPadding { get; set; }

    //    // New: host an arbitrary control as the dropdown content
    //    [Category("Behavior")]
    //    [DefaultValue(null)]
    //    [Description("Control to show as the drop-down (e.g., a Panel). Will be reparented temporarily while open.")]
    //    [TypeConverter(typeof(ReferenceConverter))] // enables design-time selection from existing controls
    //    public Control DropDownControl { get; set; }

    //    [Category("Behavior")]
    //    [DefaultValue(typeof(ToolStripDropDownDirection), "BelowLeft")]
    //    [Description("Where to show the drop-down relative to the button.")]
    //    public ToolStripDropDownDirection DropDownDirection { get; set; }

    //    [Category("Behavior")]
    //    [DefaultValue(typeof(Point), "0,0")]
    //    [Description("Pixel offset to apply to the calculated drop-down location.")]
    //    public Point DropDownOffset { get; set; }

    //    [Category("Behavior")]
    //    [DefaultValue(true)]
    //    [Description("If true, closes the control-based drop-down when focus leaves it.")]
    //    public bool CloseOnDropDownControlLostFocus { get; set; }

    //    public event EventHandler DropDownClicked;
    //    public event CancelEventHandler DropDownOpening;
    //    public event EventHandler DropDownOpened;
    //    public event ToolStripDropDownClosedEventHandler DropDownClosed;
    //    public event EventHandler DropDownContentLostFocus;

    //    protected override void OnPaint(PaintEventArgs pevent)
    //    {
    //        base.OnPaint(pevent);

    //        var g = pevent.Graphics;

    //        //if (Mode == DropDownMode.Split)
    //        //{
    //        //    var arrowArea = GetArrowAreaRectangle();
    //        //    int sepX = RightToLeft == RightToLeft.No ? arrowArea.Left : arrowArea.Right;

    //        //    using (var p1 = new Pen(SystemColors.ButtonHighlight))
    //        //    using (var p2 = new Pen(SystemColors.ButtonShadow))
    //        //    {
    //        //        g.DrawLine(p1, sepX, 4, sepX, Height - 5);
    //        //        int delta = RightToLeft == RightToLeft.No ? -1 : 1;
    //        //        g.DrawLine(p2, sepX + delta, 4, sepX + delta, Height - 5);
    //        //    }
    //        //}

    //        var glyphRect = GetArrowGlyphRectangle();
    //        var color = Enabled ? (ArrowColor.IsEmpty ? ForeColor : ArrowColor) : SystemColors.GrayText;
    //        DrawArrowGlyph(g, glyphRect, color);
    //    }

    //    protected override void OnMouseDown(MouseEventArgs mevent)
    //    {
    //        if (!Enabled)
    //        {
    //            base.OnMouseDown(mevent);
    //            return;
    //        }

    //        bool inArrowArea = IsInArrowArea(mevent.Location);

    //        if (Mode == DropDownMode.Whole || inArrowArea)
    //        {
    //            _arrowPressed = true;
    //            Invalidate(GetArrowAreaRectangle());

    //            OnDropDownTriggered();
    //            return; // suppress base click for dropdown trigger
    //        }

    //        base.OnMouseDown(mevent);
    //    }

    //    protected override void OnMouseUp(MouseEventArgs mevent)
    //    {
    //        if (_arrowPressed)
    //        {
    //            _arrowPressed = false;
    //            Invalidate(GetArrowAreaRectangle());
    //            return;
    //        }
    //        base.OnMouseUp(mevent);
    //    }

    //    protected override void OnClick(EventArgs e)
    //    {
    //        if (_arrowPressed)
    //            return;

    //        if (Mode == DropDownMode.Split)
    //            base.OnClick(e);
    //        else
    //            base.OnClick(e);
    //    }

    //    protected override bool IsInputKey(Keys keyData)
    //    {
    //        if (keyData == Keys.Down || keyData == (Keys.Alt | Keys.Down) || keyData == Keys.F4)
    //            return true;
    //        return base.IsInputKey(keyData);
    //    }

    //    protected override void OnKeyDown(KeyEventArgs e)
    //    {
    //        base.OnKeyDown(e);

    //        if (!Enabled)
    //            return;

    //        if (e.Alt && e.KeyCode == Keys.Down || e.KeyCode == Keys.F4)
    //        {
    //            e.Handled = true;
    //            e.SuppressKeyPress = true;
    //            OnDropDownTriggered();
    //        }
    //        else if (e.KeyCode == Keys.Down && Mode == DropDownMode.Whole)
    //        {
    //            e.Handled = true;
    //            e.SuppressKeyPress = true;
    //            OnDropDownTriggered();
    //        }
    //    }

    //    private void OnDropDownTriggered()
    //    {
    //        DropDownClicked?.Invoke(this, EventArgs.Empty);

    //        if (!ShowDropDownOnClick)
    //            return;

    //        var args = new CancelEventArgs();
    //        DropDownOpening?.Invoke(this, args);
    //        if (args.Cancel)
    //            return;

    //        if (DropDownControl != null)
    //        {
    //            ShowControlDropDown();
    //        }
    //        else if (DropDownMenu != null)
    //        {
    //            var pt = GetDropDownShowPoint();
    //            try
    //            {
    //                DropDownMenu.Show(this, pt, DropDownDirection);
    //                DropDownOpened?.Invoke(this, EventArgs.Empty);
    //                DropDownMenu.Closed += (s, e) => DropDownClosed?.Invoke(this, e);
    //            }
    //            catch { }
    //        }
    //        // else: nothing to show (only event raised)
    //    }

    //    private Point GetDropDownShowPoint()
    //    {
    //        // Base point from direction; we offset later
    //        switch (DropDownDirection)
    //        {
    //            case ToolStripDropDownDirection.AboveLeft:
    //                return new Point(0, -1);
    //            case ToolStripDropDownDirection.AboveRight:
    //                return new Point(Width, -1);
    //            case ToolStripDropDownDirection.BelowRight:
    //                return new Point(Width, Height);
    //            case ToolStripDropDownDirection.BelowLeft:
    //            default:
    //                return new Point(0, Height);
    //        }
    //    }

    //    private void ShowControlDropDown()
    //    {
    //        if (DropDownControl == null) return;

    //        if (_activeDropDown != null && !_activeDropDown.IsDisposed && _activeDropDown.Visible)
    //        {
    //            CloseDropDown();
    //            return;
    //        }

    //        _activeDropDown = new ToolStripDropDown
    //        {
    //            AutoClose = true,
    //            Padding = Padding.Empty,
    //            Margin = Padding.Empty
    //        };

    //        _activeDropDown.Closed += ActiveDropDown_Closed;
    //        _activeDropDown.Opened += (s, e) => DropDownOpened?.Invoke(this, EventArgs.Empty);

    //        _host = new ToolStripControlHost(DropDownControl)
    //        {
    //            AutoSize = false,
    //            Margin = Padding.Empty,
    //            Padding = Padding.Empty
    //        };
    //        _host.Size = DropDownControl.Size;

    //        _activeDropDown.Items.Clear();
    //        _activeDropDown.Items.Add(_host);

    //        // Remember original placement
    //        _origParent = DropDownControl.Parent;
    //        _origBounds = DropDownControl.Bounds;
    //        _origVisible = DropDownControl.Visible;
    //        _origIndex = _origParent != null ? _origParent.Controls.GetChildIndex(DropDownControl) : -1;

    //        if (CloseOnDropDownControlLostFocus)
    //        {
    //            DropDownControl.LostFocus += DropDownControl_LostFocus;
    //            DropDownControl.Leave += DropDownControl_LostFocus;
    //        }

    //        // Freeze ambient colors so they don't change when reparented
    //        ////_savedAmbientForeColor = !DropDownControl.ShouldSerializeForeColor();
    //        ////_savedAmbientBackColor = !DropDownControl.ShouldSerializeBackColor();

    //        if (_savedAmbientForeColor)
    //        {
    //            // Set explicit ForeColor to current effective (freezes child labels that inherit)
    //            DropDownControl.ForeColor = DropDownControl.ForeColor;
    //        }
    //        else
    //        {
    //            // Remember explicit value to restore later
    //            _savedForeColorValue = DropDownControl.ForeColor;
    //        }

    //        if (_savedAmbientBackColor)
    //        {
    //            DropDownControl.BackColor = DropDownControl.BackColor;
    //        }
    //        else
    //        {
    //            _savedBackColorValue = DropDownControl.BackColor;
    //        }

    //        // Optional: make the ToolStripDropDown match your panel’s colors (helps its border/background look right)
    //        _activeDropDown.ForeColor = DropDownControl.ForeColor;
    //        _activeDropDown.BackColor = DropDownControl.BackColor;
    //        _host.ForeColor = DropDownControl.ForeColor;
    //        _host.BackColor = DropDownControl.BackColor;

    //        var basePoint = GetDropDownShowPoint();
    //        var showPoint = new Point(basePoint.X + DropDownOffset.X, basePoint.Y + DropDownOffset.Y);

    //        _activeDropDown.Show(this, showPoint, DropDownDirection);
    //        DropDownControl.Visible = true;
    //        DropDownControl.Focus();
    //    }

    //    private void DropDownControl_LostFocus(object sender, EventArgs e)
    //    {
    //        // Defer to end of message loop to get the final focus target
    //        if (_activeDropDown == null) return;

    //        BeginInvoke((MethodInvoker)delegate
    //        {
    //            var dd = _activeDropDown;
    //            if (dd == null || dd.IsDisposed) return;

    //            try
    //            {
    //                if (!dd.ContainsFocus && !ContainsFocus)
    //                {
    //                    DropDownContentLostFocus?.Invoke(this, EventArgs.Empty);
    //                    dd.Close(ToolStripDropDownCloseReason.AppFocusChange);
    //                    dd.Hide();
    //                    DropDownControl.Visible = false;
    //                }
    //            }
    //            catch (ObjectDisposedException)
    //            {
    //                // Already closed/disposed — safe to ignore
    //            }
    //        });
    //    }

    //    private void ActiveDropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
    //    {
    //        if (_isCleaningUp) return;
    //        _isCleaningUp = true;

    //        try
    //        {
    //            DropDownClosed?.Invoke(this, e);

    //            // Unhook focus handlers before disposing
    //            if (DropDownControl != null)
    //            {
    //                DropDownControl.LostFocus -= DropDownControl_LostFocus;
    //                DropDownControl.Leave -= DropDownControl_LostFocus;
    //            }

    //            CleanupDropDown(false);
    //        }
    //        finally
    //        {
    //            _isCleaningUp = false;
    //        }
    //    }

    //    protected override void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            try { CloseDropDown(); } catch { }
    //            if (DropDownControl != null)
    //            {
    //                DropDownControl.LostFocus -= DropDownControl_LostFocus;
    //                DropDownControl.Leave -= DropDownControl_LostFocus;
    //            }
    //        }
    //        base.Dispose(disposing);
    //    }

    //    private void CleanupDropDown(bool suppressRestore)
    //    {
    //        var dd = _activeDropDown;

    //        try
    //        {
    //            if (!suppressRestore && DropDownControl != null && !DropDownControl.IsDisposed)
    //            {
    //                if (_origParent != null && !_origParent.IsDisposed)
    //                {
    //                    DropDownControl.Visible = false;

    //                    _origParent.SuspendLayout();
    //                    try
    //                    {
    //                        _origParent.Controls.Add(DropDownControl);
    //                        DropDownControl.Bounds = _origBounds;
    //                        if (_origIndex >= 0)
    //                            _origParent.Controls.SetChildIndex(DropDownControl, _origIndex);

    //                        // Restore ambient/existing color behavior
    //                        if (_savedAmbientForeColor)
    //                            DropDownControl.ForeColor = Color.Empty;       // back to ambient
    //                        else
    //                            DropDownControl.ForeColor = _savedForeColorValue;

    //                        if (_savedAmbientBackColor)
    //                            DropDownControl.BackColor = Color.Empty;       // back to ambient
    //                        else
    //                            DropDownControl.BackColor = _savedBackColorValue;

    //                        // Hide if that's how you want it when restored
    //                        DropDownControl.Visible = false;
    //                        if (dd != null) dd.Visible = false;
    //                    }
    //                    finally
    //                    {
    //                        _origParent.ResumeLayout();
    //                    }
    //                }
    //            }
    //        }
    //        catch { /* be defensive */ }
    //        finally
    //        {
    //            if (dd != null)
    //            {
    //                try { dd.Closed -= ActiveDropDown_Closed; } catch { }
    //                dd.Visible = false;
    //                // If you prefer not to Dispose here, leave it alive.
    //                // If you want to dispose, do it here and ensure you don't touch 'dd' afterwards.
    //                // try { if (!dd.IsDisposed) dd.Dispose(); } catch { }
    //            }

    //            _activeDropDown = null;
    //            _host = null;
    //            _origParent = null;
    //            _origIndex = -1;

    //            // Reset saved flags
    //            _savedAmbientForeColor = false;
    //            _savedAmbientBackColor = false;
    //            _savedForeColorValue = Color.Empty;
    //            _savedBackColorValue = Color.Empty;
    //        }
    //    }

    //    public void CloseDropDown()
    //    {
    //        var dd = _activeDropDown;
    //        if (dd != null && !dd.IsDisposed && dd.Visible)
    //        {
    //            try { dd.Close(ToolStripDropDownCloseReason.CloseCalled);  dd.Visible = false; }
    //            catch (ObjectDisposedException) { /* already gone */ }
    //        }
    //    }

    //    private bool IsInArrowArea(Point p)
    //    {
    //        return GetArrowAreaRectangle().Contains(p);
    //    }

    //    private Rectangle GetArrowAreaRectangle()
    //    {
    //        if (Mode == DropDownMode.Whole)
    //            return ClientRectangle;

    //        int w = Math.Max(12, ArrowAreaWidth);
    //        bool rtl = RightToLeft == RightToLeft.Yes;
    //        return rtl
    //            ? new Rectangle(0, 0, w, Height)
    //            : new Rectangle(Width - w, 0, w, Height);
    //    }

    //    private Rectangle GetArrowGlyphRectangle()
    //    {
    //        int size = Math.Min(16, Height - 8);
    //        int glyphW = size;
    //        int glyphH = size;

    //        if (Mode == DropDownMode.Split)
    //        {
    //            var area = GetArrowAreaRectangle();
    //            int x = area.Left + (area.Width - glyphW) / 2;
    //            int y = area.Top + (area.Height - glyphH) / 2;
    //            return new Rectangle(x, y, glyphW, glyphH);
    //        }

    //        int yWhole = (Height - glyphH) / 2;
    //        int xWhole;
    //        switch (ArrowAlignment)
    //        {
    //            case ArrowAlignment.Left:
    //                xWhole = ArrowPadding;
    //                break;
    //            case ArrowAlignment.Center:
    //                xWhole = (Width - glyphW) / 2;
    //                break;
    //            case ArrowAlignment.Right:
    //            default:
    //                xWhole = Width - glyphW - ArrowPadding;
    //                break;
    //        }

    //        return new Rectangle(Math.Max(0, xWhole), Math.Max(0, yWhole),
    //                             Math.Min(glyphW, Width), Math.Min(glyphH, Height));
    //    }

    //    private void DrawArrowGlyph(Graphics g, Rectangle bounds, Color color)
    //    {
    //        int cx = bounds.Left + bounds.Width / 2;
    //        int cy = bounds.Top + bounds.Height / 2;

    //        int half = Math.Max(3, Math.Min(bounds.Width, bounds.Height) / 4);
    //        Point p1 = new Point(cx - half, cy - half / 2);
    //        Point p2 = new Point(cx + half, cy - half / 2);
    //        Point p3 = new Point(cx, cy + half);

    //        using (var b = new SolidBrush(color))
    //            g.FillPolygon(b, new[] { p1, p2, p3 });

    //        if (_arrowPressed && Enabled)
    //        {
    //            using (var pen = new Pen(Color.FromArgb(60, Color.Black)))
    //                g.DrawRectangle(pen, Rectangle.Inflate(GetArrowAreaRectangle(), -1, -1));
    //        }
    //    }
    //}
}