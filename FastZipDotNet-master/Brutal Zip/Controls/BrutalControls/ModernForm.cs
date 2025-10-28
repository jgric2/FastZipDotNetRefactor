namespace BrutalZip2025.BrutalControls
{

    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public partial class ModernForm : Form
    {
        // DWM API function declarations
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("dwmapi.dll")]
        private static extern int DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND pBlurBehind);

        // Attribute constants
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;

        // Blur Behind Struct & Enum
        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_BLURBEHIND
        {
            public DWM_BB dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        }

        [Flags]
        public enum DWM_BB : uint
        {
            DwmBbEnable = 0x00000001,
            DwmBbBlurRegion = 0x00000002,
            DwmBbTransitionOnMaximized = 0x00000004
        }

        // Enum for window corner preferences
        private enum DWMWINDOWCORNER
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        // Constructor
        public ModernForm()
        {
            // Apply the dark theme
            ApplyDarkTheme();

            // Handle necessary events
            this.Load += ModernForm_Load;
            this.HandleCreated += ModernForm_HandleCreated;
        }

        private void ModernForm_Load(object sender, EventArgs e)
        {
            // Additional initialization if needed
        }

        private void ModernForm_HandleCreated(object sender, EventArgs e)
        {
            EnableImmersiveDarkMode();
            EnableRoundedCorners(12); // Adjust radius as needed
           // EnableBlurBehind(); // Optional: Apply blur effect
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(25, 25, 25); // Dark background
            this.ForeColor = Color.White; // Light text
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);

            // Optional: Suppress default visual styles for better control
            // Note: This may affect other controls
            // Application.EnableVisualStyles(); // Enabled by default
            // Application.SetCompatibleTextRenderingDefault(false); // Set appropriately
        }

        private void EnableImmersiveDarkMode(int useDarkMode = 1)
        {
            if (Environment.OSVersion.Version.Major < 10)
                return; // Dark mode via DWM is supported on Windows 10 and above

           // int useDarkMode = 1; // 1 to enable, 0 to disable

            // Attempt to set DWMWA_USE_IMMERSIVE_DARK_MODE
            int result = DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));

            // If necessary, adjust the attribute ID based on Windows version
            // Example:
            // int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
            // DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref useDarkMode, sizeof(int));
        }

        private void EnableRoundedCorners(int radius)
        {
            if (Environment.OSVersion.Version.Build < 22000) // Windows 11 build numbers start around 22000
                return; // Rounded corners via DWM are better supported on Windows 11

            DWMWINDOWCORNER preference = DWMWINDOWCORNER.DWMWCP_ROUND;
            int prefR = (int)preference;
            DwmSetWindowAttribute(this.Handle, DWMWA_WINDOW_CORNER_PREFERENCE, ref prefR, sizeof(int));
        }

        private void EnableBlurBehind()
        {
            DWM_BLURBEHIND blur = new DWM_BLURBEHIND
            {
                dwFlags = DWM_BB.DwmBbEnable | DWM_BB.DwmBbTransitionOnMaximized,
                fEnable = true,
                hRgnBlur = IntPtr.Zero,
                fTransitionOnMaximized = true
            };

            DwmEnableBlurBehindWindow(this.Handle, ref blur);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
           // ApplyDarkThemeToControl(e.Control);
        }

        private void ApplyDarkThemeToControl(Control control)
        {
            if (control is Button button)
            {
                button.BackColor = Color.FromArgb(28, 28, 28);
                button.ForeColor = Color.White;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            }
            else if (control is Panel || control is GroupBox)
            {
                control.BackColor = Color.FromArgb(45, 45, 48);
                control.ForeColor = Color.White;
            }
            else if (control is Label label)
            {
                label.ForeColor = Color.White;
            }
            else if (control is TextBox textBox)
            {
                textBox.BackColor = Color.FromArgb(28, 28, 28);
                textBox.ForeColor = Color.White;
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is CheckBox checkBox)
            {
                checkBox.BackColor = Color.FromArgb(45, 45, 48);
                checkBox.ForeColor = Color.White;
            }
            // Handle more control types as needed

            // Recursively apply to child controls
            foreach (Control child in control.Controls)
            {
                ApplyDarkThemeToControl(child);
            }
        }
    }
    //public partial class ModernForm : Form
    //{
    //    // Constants for resizing
    //    private const int RESIZE_HANDLE_SIZE = 10;

    //    // Constants and P/Invoke for window dragging and DWM
    //    [DllImport("dwmapi.dll")]
    //    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    //    [DllImport("user32.dll")]
    //    private static extern bool ReleaseCapture();

    //    [DllImport("user32.dll")]
    //    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    //    private const int WM_NCLBUTTONDOWN = 0xA1;
    //    private const int HTCAPTION = 0x2;
    //    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20; // May vary based on Windows version

    //    // Title bar properties
    //    private int titleBarHeight = 30;
    //    private bool isHoveringClose = false;
    //    private bool isHoveringMax = false;
    //    private bool isHoveringMin = false;

    //    // Rectangle areas for window buttons
    //    private Rectangle closeRect;
    //    private Rectangle maximizeRect;
    //    private Rectangle minimizeRect;

    //    // Hover tracking
    //    private Point lastMousePosition;

    //    public ModernForm()
    //    {
    //        // Form properties
    //        this.DoubleBuffered = true;
    //        this.FormBorderStyle = FormBorderStyle.None;
    //        this.BackColor = Color.FromArgb(30, 30, 30);
    //        this.ForeColor = Color.White;
    //        this.Text = "Modern Form";
    //        this.MinimumSize = new Size(300, 200);
    //        this.Size = new Size(800, 600);

    //        // Event handlers
    //        this.Load += ModernForm_Load;
    //        this.Resize += ModernForm_Resize;
    //        this.MouseMove += ModernForm_MouseMove;
    //        this.MouseLeave += ModernForm_MouseLeave;
    //        this.MouseDown += ModernForm_MouseDown;
    //        this.Paint += ModernForm_Paint;
    //        this.MouseClick += ModernForm_MouseClick;
    //    }

    //    private void ModernForm_Load(object sender, EventArgs e)
    //    {
    //        // Apply dark title bar
    //        ApplyDarkTitleBar();

    //        // Apply rounded corners
    //        ApplyRoundedCorners(20);
    //    }

    //    private void ModernForm_Resize(object sender, EventArgs e)
    //    {
    //        // Re-apply rounded corners on resize
    //        ApplyRoundedCorners(20);
    //        this.Invalidate(); // Redraw on resize
    //    }

    //    private void ModernForm_Paint(object sender, PaintEventArgs e)
    //    {
    //        DrawTitleBar(e.Graphics);
    //    }

    //    private void ModernForm_MouseMove(object sender, MouseEventArgs e)
    //    {
    //        // Store last mouse position
    //        lastMousePosition = e.Location;

    //        // Determine if the mouse is hovering over window buttons
    //        bool previousHoverClose = isHoveringClose;
    //        bool previousHoverMax = isHoveringMax;
    //        bool previousHoverMin = isHoveringMin;

    //        isHoveringClose = closeRect.Contains(e.Location);
    //        isHoveringMax = maximizeRect.Contains(e.Location);
    //        isHoveringMin = minimizeRect.Contains(e.Location);

    //        if (previousHoverClose != isHoveringClose ||
    //            previousHoverMax != isHoveringMax ||
    //            previousHoverMin != isHoveringMin)
    //        {
    //            this.Invalidate(); // Redraw to show hover effect
    //        }
    //    }

    //    private void ModernForm_MouseLeave(object sender, EventArgs e)
    //    {
    //        // Reset hover states when mouse leaves form
    //        isHoveringClose = false;
    //        isHoveringMax = false;
    //        isHoveringMin = false;
    //        this.Invalidate();
    //    }

    //    private void ModernForm_MouseDown(object sender, MouseEventArgs e)
    //    {
    //        if (e.Button == MouseButtons.Left && e.Y <= titleBarHeight)
    //        {
    //            ReleaseCapture();
    //            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
    //        }
    //    }

    //    private void ModernForm_MouseClick(object sender, MouseEventArgs e)
    //    {
    //        // Check if click is on window buttons
    //        if (closeRect.Contains(e.Location))
    //        {
    //            this.Close();
    //        }
    //        else if (maximizeRect.Contains(e.Location))
    //        {
    //            ToggleMaximize();
    //        }
    //        else if (minimizeRect.Contains(e.Location))
    //        {
    //            this.WindowState = FormWindowState.Minimized;
    //        }
    //    }

    //    private void DrawTitleBar(Graphics g)
    //    {
    //        // Draw title bar background
    //        Rectangle titleBarRect = new Rectangle(0, 0, this.Width, titleBarHeight);
    //        using (SolidBrush brush = new SolidBrush(Color.FromArgb(45, 45, 48)))
    //        {
    //            g.FillRectangle(brush, titleBarRect);
    //        }

    //        // Draw title text
    //        using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
    //        {
    //            using (StringFormat sf = new StringFormat
    //            {
    //                Alignment = StringAlignment.Near,
    //                LineAlignment = StringAlignment.Center
    //            })
    //            {
    //                g.DrawString(this.Text, this.Font, textBrush, new RectangleF(10, 0, this.Width - 20, titleBarHeight), sf);
    //            }
    //        }

    //        // Define window control button rectangles
    //        int buttonSize = 20;
    //        int padding = 5;
    //        int y = (titleBarHeight - buttonSize) / 2;

    //        closeRect = new Rectangle(this.Width - padding - buttonSize, y, buttonSize, buttonSize);
    //        maximizeRect = new Rectangle(this.Width - 2 * (padding + buttonSize), y, buttonSize, buttonSize);
    //        minimizeRect = new Rectangle(this.Width - 3 * (padding + buttonSize), y, buttonSize, buttonSize);

    //        // Draw window control buttons
    //        DrawWindowButton(g, closeRect, isHoveringClose, Color.FromArgb(232, 17, 35), "X");
    //        DrawWindowButton(g, maximizeRect, isHoveringMax, Color.FromArgb(25, 118, 210), this.WindowState == FormWindowState.Maximized ? "🗗" : "🗖");
    //        DrawWindowButton(g, minimizeRect, isHoveringMin, Color.FromArgb(245, 127, 23), "_");
    //    }

    //    private void DrawWindowButton(Graphics g, Rectangle rect, bool isHovering, Color backColor, string text)
    //    {
    //        // Change color on hover
    //        Color fillColor = isHovering ? ControlPaint.Light(backColor) : backColor;

    //        using (SolidBrush brush = new SolidBrush(fillColor))
    //        {
    //            g.FillRectangle(brush, rect);
    //        }

    //        // Optional: Draw button border
    //        using (Pen pen = new Pen(Color.FromArgb(64, 64, 64)))
    //        {
    //            g.DrawRectangle(pen, rect);
    //        }

    //        // Draw text
    //        using (SolidBrush textBrush = new SolidBrush(Color.White))
    //        {
    //            using (StringFormat sf = new StringFormat
    //            {
    //                Alignment = StringAlignment.Center,
    //                LineAlignment = StringAlignment.Center
    //            })
    //            {
    //                g.DrawString(text, this.Font, textBrush, rect, sf);
    //            }
    //        }
    //    }

    //    private void ToggleMaximize()
    //    {
    //        if (this.WindowState == FormWindowState.Normal)
    //            this.WindowState = FormWindowState.Maximized;
    //        else
    //            this.WindowState = FormWindowState.Normal;
    //    }

    //    private void ApplyRoundedCorners(int radius)
    //    {
    //        var path = new System.Drawing.Drawing2D.GraphicsPath();
    //        path.StartFigure();
    //        path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
    //        path.AddLine(radius, 0, this.Width - radius, 0);
    //        path.AddArc(new Rectangle(this.Width - radius, 0, radius, radius), 270, 90);
    //        path.AddLine(this.Width, radius, this.Width, this.Height - radius);
    //        path.AddArc(new Rectangle(this.Width - radius, this.Height - radius, radius, radius), 0, 90);
    //        path.AddLine(this.Width - radius, this.Height, radius, this.Height);
    //        path.AddArc(new Rectangle(0, this.Height - radius, radius, radius), 90, 90);
    //        path.CloseFigure();
    //        this.Region = new Region(path);
    //    }

    //    private void ApplyDarkTitleBar()
    //    {
    //        int value = 1; // 1 to enable dark mode. Some versions may require 20.
    //        DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));

    //        // Note: Additional DWM attributes can be set here if needed
    //    }

    //    // Override WndProc to handle resizing
    //    protected override void WndProc(ref Message m)
    //    {
    //        if (m.Msg == 0x84) // WM_NCHITTEST
    //        {
    //            base.WndProc(ref m);
    //            if ((int)m.Result == 1) // HTCLIENT
    //            {
    //                Point cursor = PointToClient(Cursor.Position);

    //                if (cursor.Y <= RESIZE_HANDLE_SIZE)
    //                {
    //                    if (cursor.X <= RESIZE_HANDLE_SIZE)
    //                        m.Result = (IntPtr)13; // HTTOPLEFT
    //                    else if (cursor.X < (this.Width - RESIZE_HANDLE_SIZE))
    //                        m.Result = (IntPtr)12; // HTTOP
    //                    else
    //                        m.Result = (IntPtr)14; // HTTOPRIGHT
    //                }
    //                else if (cursor.Y <= (this.Height - RESIZE_HANDLE_SIZE))
    //                {
    //                    if (cursor.X <= RESIZE_HANDLE_SIZE)
    //                        m.Result = (IntPtr)10; // HTLEFT
    //                    else if (cursor.X >= this.Width - RESIZE_HANDLE_SIZE)
    //                        m.Result = (IntPtr)11; // HTRIGHT
    //                }
    //                else
    //                {
    //                    if (cursor.X <= RESIZE_HANDLE_SIZE)
    //                        m.Result = (IntPtr)16; // HTBOTTOMLEFT
    //                    else if (cursor.X < (this.Width - RESIZE_HANDLE_SIZE))
    //                        m.Result = (IntPtr)15; // HTBOTTOM
    //                    else
    //                        m.Result = (IntPtr)17; // HTBOTTOMRIGHT
    //                }
    //                return;
    //            }
    //            return;
    //        }

    //        base.WndProc(ref m);
    //    }

    //    // Override OnControlAdded to apply dark theme
    //    protected override void OnControlAdded(ControlEventArgs e)
    //    {
    //        base.OnControlAdded(e);
    //        ApplyDarkTheme(e.Control);
    //    }

    //    private void ApplyDarkTheme(Control control)
    //    {
    //        if (control is Panel || control is GroupBox)
    //        {
    //            control.BackColor = Color.FromArgb(45, 45, 48);
    //            control.ForeColor = Color.White;
    //        }
    //        else if (control is Label)
    //        {
    //            control.ForeColor = Color.White;
    //        }
    //        else if (control is Button)
    //        {
    //            control.BackColor = Color.FromArgb(28, 28, 28);
    //            control.ForeColor = Color.White;
    //            ((Button)control).FlatStyle = FlatStyle.Flat;
    //            ((Button)control).FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
    //        }
    //        // Handle more control types as needed

    //        // Recursively apply to child controls
    //        foreach (Control child in control.Controls)
    //        {
    //            ApplyDarkTheme(child);
    //        }
    //    }
    //}
}
