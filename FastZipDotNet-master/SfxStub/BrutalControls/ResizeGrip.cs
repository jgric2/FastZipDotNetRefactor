using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrutalCopy2025.Controls.BrutalControls
{
    /// <summary>
    /// A little transparent control that, when hit‐tested,
    /// returns whichever HT* code you’ve configured,
    /// causing the form to resize on that edge/corner.
    /// </summary>
    [ToolboxBitmap(typeof(ResizeGrip), "ResizeGrip.bmp")]
    // optional: add a 16×16 bitmap named ResizeGrip.bmp 
    // to your project so it shows nicely in the toolbox
    public class ResizeGrip : Control
    {
        public enum HitTestLocations : int
        {
            Left = 10,   // HTLEFT
            Right = 11,   // HTRIGHT
            Top = 12,   // HTTOP
            TopLeft = 13,   // HTTOPLEFT
            TopRight = 14,   // HTTOPRIGHT
            Bottom = 15,   // HTBOTTOM
            BottomLeft = 16,   // HTBOTTOMLEFT
            BottomRight = 17    // HTBOTTOMRIGHT
        }

        HitTestLocations _hitLocation = HitTestLocations.BottomRight;

        [Category("Behavior"),
         Description("Which non‐client hit‐test code to return when the mouse is over this control."),
         DefaultValue(HitTestLocations.BottomRight)]
        public HitTestLocations HitLocation
        {
            get { return _hitLocation; }
            set
            {
                if (_hitLocation != value)
                {
                    _hitLocation = value;
                    UpdateCursor();
                }
            }
        }

        public ResizeGrip()
        {
            // make it transparent by default
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;

            // no tab stop, no focus rectangle
            TabStop = false;
            UpdateCursor();
        }

        void UpdateCursor()
        {
            // pick a sensible sizing cursor for the chosen edge/corner
            switch (_hitLocation)
            {
                case HitTestLocations.Left:
                case HitTestLocations.Right:
                    Cursor = Cursors.SizeWE;
                    break;
                case HitTestLocations.Top:
                case HitTestLocations.Bottom:
                    Cursor = Cursors.SizeNS;
                    break;
                case HitTestLocations.TopLeft:
                case HitTestLocations.BottomRight:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case HitTestLocations.TopRight:
                case HitTestLocations.BottomLeft:
                    Cursor = Cursors.SizeNESW;
                    break;
                default:
                    Cursor = Cursors.Default;
                    break;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            if (m.Msg == WM_NCHITTEST)
            {
                // tell Windows “you hit my edge/corner” 
                m.Result = (IntPtr)(int)_hitLocation;
                return;
            }
            base.WndProc(ref m);
        }

        // optional: draw a little grip in the bottom‐right corner 
        // if you want a visible handle there.  Otherwise leave it blank.
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //    if (_hitLocation == HitTestLocations.BottomRight)
        //        ControlPaint.DrawSizeGrip(e.Graphics, BackColor, 0, 0, Width, Height);
        //}
    }
}
