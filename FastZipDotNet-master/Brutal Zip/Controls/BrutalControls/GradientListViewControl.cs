using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using static System.Windows.Forms.ListView;

namespace BrutalZip2025.BrutalControls
{
    //public class TransparentListView : ListView
    //{
    //    private const int WS_EX_TRANSPARENT = 0x20;
    //    private const int WM_ERASEBKGND = 0x0014;

    //    public TransparentListView()
    //    {
    //        // turn on double-buffering and allow BackColor = Transparent
    //        //SetStyle(
    //        //  ControlStyles.OptimizedDoubleBuffer |
    //        //  ControlStyles.AllPaintingInWmPaint |
    //        //  ControlStyles.SupportsTransparentBackColor,
    //        //  true);

    //        // force the hidden DoubleBuffered property on the native ListView
    //        typeof(Control).InvokeMember(
    //          "DoubleBuffered",
    //          BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty,
    //          null, this, new object[] { true });

    //        //BackColor = Color.Transparent;
    //    }

    //    //protected override CreateParams CreateParams
    //    //{
    //    //    get
    //    //    {
    //    //        var cp = base.CreateParams;
    //    //        // tell Win32 “paint my parent first, then me”
    //    //        cp.ExStyle |= WS_EX_TRANSPARENT;
    //    //        return cp;
    //    //    }
    //    //}

    //    //protected override void WndProc(ref Message m)
    //    //{
    //    //    if (m.Msg == WM_ERASEBKGND)
    //    //    {
    //    //        // swallow the default white background fill
    //    //        m.Result = IntPtr.Zero;
    //    //        return;
    //    //    }
    //    //    base.WndProc(ref m);
    //    //}

    //    protected override void OnPaintBackground(PaintEventArgs e)
    //    {
    //        // instead of filling with white, ask our parent to paint into our DC
    //        if (BackColor == Color.Transparent && Parent != null)
    //        {
    //            var g = e.Graphics;
    //            var state = g.Save();
    //            // shift coords so that Parent draws in the correct spot
    //            g.TranslateTransform(-Left, -Top);
    //            var pea = new PaintEventArgs(g, Parent.ClientRectangle);
    //            InvokePaintBackground(Parent, pea);
    //            InvokePaint(Parent, pea);
    //            g.Restore(state);


    //        }
    //        else
    //        {
    //            base.OnPaintBackground(e);
    //        }
    //    }
    //}


    internal sealed class GradientListViewControlDesigner : ControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            var ctl = component as GradientListViewControl;
            if (ctl?.InnerListView != null)
            {
                // The second parameter must match the PUBLIC property name on the control.
                // This is how the designer will reference it in generated code:
                // this.gradientListViewControl1.InnerListView.Columns.AddRange(...);
                EnableDesignMode(ctl.InnerListView, nameof(GradientListViewControl.InnerListView));
            }
        }
    }

    [Designer(typeof(GradientListViewControlDesigner))]
    public partial class GradientListViewControl : UserControl
    {

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ListView listView;

        private Panel headerFillPanel;
        public ImageList imageList; // Added ImageList

        // Gradient properties
        private Color headerStartColor = Color.FromArgb(29, 181, 82);
        private Color headerEndColor = Color.FromArgb(12, 12, 12);
        private GradientSide headerGradientStartSide = GradientSide.Left;
        private GradientSide headerGradientEndSide = GradientSide.Right;
        private Color hoverColor = Color.FromArgb(22, 132, 99);
        private Color selectColor = Color.FromArgb(29, 181, 82);
        private Color selectTextColor = Color.White;//Color.FromArgb(29, 181, 82);
        // Cached gradient bitmap
        private Bitmap gradientBitmap;

        private int hoveredItemIndex = -1; // Track hovered item


        // Expose it via a public read-only property.
        // The name here must match the name you use in EnableDesignMode (below).

        public ListView InnerListView => listView;

        #region Inner ListView events (pass-through)

        [Category("Inner ListView events")]
        [Description("Occurs when a column header is clicked.")]
        public event ColumnClickEventHandler ColumnClick
        {
            add { if (listView != null) listView.ColumnClick += value; }
            remove { if (listView != null) listView.ColumnClick -= value; }
        }

        [Category("Inner ListView events")]
        [Description("Occurs when a column width is being changed.")]
        public event ColumnWidthChangingEventHandler ColumnWidthChanging
        {
            add { if (listView != null) listView.ColumnWidthChanging += value; }
            remove { if (listView != null) listView.ColumnWidthChanging -= value; }
        }

        [Category("Inner ListView events")]
        [Description("Occurs when a column width has changed.")]
        public event ColumnWidthChangedEventHandler ColumnWidthChanged
        {
            add { if (listView != null) listView.ColumnWidthChanged += value; }
            remove { if (listView != null) listView.ColumnWidthChanged -= value; }
        }

        [Category("Inner ListView events")]
        [Description("Owner-draw: draw the column header.")]
        public event DrawListViewColumnHeaderEventHandler DrawColumnHeader
        {
            add { if (listView != null) listView.DrawColumnHeader += value; }
            remove { if (listView != null) listView.DrawColumnHeader -= value; }
        }

        [Category("Inner ListView events")]
        [Description("Owner-draw: draw an item.")]
        public event DrawListViewItemEventHandler DrawItem
        {
            add { if (listView != null) listView.DrawItem += value; }
            remove { if (listView != null) listView.DrawItem -= value; }
        }

        [Category("Inner ListView events")]
        [Description("Owner-draw: draw a subitem.")]
        public event DrawListViewSubItemEventHandler DrawSubItem
        {
            add { if (listView != null) listView.DrawSubItem += value; }
            remove { if (listView != null) listView.DrawSubItem -= value; }
        }

        [Category("Inner ListView events")]
        [Description("Occurs when the selected index changes.")]
        public event EventHandler SelectedIndexChanged
        {
            add { if (listView != null) listView.SelectedIndexChanged += value; }
            remove { if (listView != null) listView.SelectedIndexChanged -= value; }
        }

        [Category("Inner ListView events")]
        [Description("Occurs when an item's selection state changes.")]
        public event ListViewItemSelectionChangedEventHandler ItemSelectionChanged
        {
            add { if (listView != null) listView.ItemSelectionChanged += value; }
            remove { if (listView != null) listView.ItemSelectionChanged -= value; }
        }

        [Category("Inner ListView events")]
        [Description("Occurs when an item is activated (e.g., double-click or Enter).")]
        public event EventHandler ItemActivate
        {
            add { if (listView != null) listView.ItemActivate += value; }
            remove { if (listView != null) listView.ItemActivate -= value; }
        }

        [Category("Inner ListView events")]
        [Description("Occurs when the mouse hovers over an item.")]
        public event ListViewItemMouseHoverEventHandler ItemMouseHover
        {
            add { if (listView != null) listView.ItemMouseHover += value; }
            remove { if (listView != null) listView.ItemMouseHover -= value; }
        }

        // Key events (avoid shadowing base events by using ListView-prefixed names)
        [Category("Inner ListView events")]
        [Description("KeyDown on inner ListView.")]
        public event KeyEventHandler ListViewKeyDown
        {
            add { if (listView != null) listView.KeyDown += value; }
            remove { if (listView != null) listView.KeyDown -= value; }
        }

        [Category("Inner ListView events")]
        [Description("KeyPress on inner ListView.")]
        public event KeyPressEventHandler ListViewKeyPress
        {
            add { if (listView != null) listView.KeyPress += value; }
            remove { if (listView != null) listView.KeyPress -= value; }
        }

        [Category("Inner ListView events")]
        [Description("KeyUp on inner ListView.")]
        public event KeyEventHandler ListViewKeyUp
        {
            add { if (listView != null) listView.KeyUp += value; }
            remove { if (listView != null) listView.KeyUp -= value; }
        }

        // Common mouse events (prefixed to avoid shadowing the UserControl's own mouse events)
        [Category("Inner ListView events")]
        [Description("MouseDown on inner ListView.")]
        public event MouseEventHandler ListViewMouseDown
        {
            add { if (listView != null) listView.MouseDown += value; }
            remove { if (listView != null) listView.MouseDown -= value; }
        }

        [Category("Inner ListView events")]
        [Description("MouseUp on inner ListView.")]
        public event MouseEventHandler ListViewMouseUp
        {
            add { if (listView != null) listView.MouseUp += value; }
            remove { if (listView != null) listView.MouseUp -= value; }
        }

        [Category("Inner ListView events")]
        [Description("MouseClick on inner ListView.")]
        public event MouseEventHandler ListViewMouseClick
        {
            add { if (listView != null) listView.MouseClick += value; }
            remove { if (listView != null) listView.MouseClick -= value; }
        }

        [Category("Inner ListView events")]
        [Description("MouseDoubleClick on inner ListView.")]
        public event MouseEventHandler ListViewMouseDoubleClick
        {
            add { if (listView != null) listView.MouseDoubleClick += value; }
            remove { if (listView != null) listView.MouseDoubleClick -= value; }
        }

        #endregion

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DROPFILES)
            {
                IntPtr hDrop = m.WParam;
                int n = DragQueryFile(hDrop, 0xFFFFFFFF, null, 0);
                var sb = new System.Text.StringBuilder(260);
                for (uint i = 0; i < n; i++)
                {
                    DragQueryFile(hDrop, i, sb, sb.Capacity);
                    var item = new ListViewItem(sb.ToString());
                    listView.Items.Add(item);
                }
                DragFinish(hDrop);
                return;
            }
            base.WndProc(ref m);
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The start color of the header gradient.")]
        public Color HeaderStartColor
        {
            get => headerStartColor;
            set
            {
                if (headerStartColor != value)
                {
                    headerStartColor = value;
                    UpdateGradientBitmap();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The end color of the header gradient.")]
        public Color HeaderEndColor
        {
            get => headerEndColor;
            set
            {
                if (headerEndColor != value)
                {
                    headerEndColor = value;
                    UpdateGradientBitmap();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The side where the header gradient starts.")]
        public GradientSide HeaderGradientStartSide
        {
            get => headerGradientStartSide;
            set
            {
                if (headerGradientStartSide != value)
                {
                    headerGradientStartSide = value;
                    UpdateGradientBitmap();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The side where the header gradient ends.")]
        public GradientSide HeaderGradientEndSide
        {
            get => headerGradientEndSide;
            set
            {
                if (headerGradientEndSide != value)
                {
                    headerGradientEndSide = value;
                    UpdateGradientBitmap();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("BackColor")]
        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                if (listView != null)
                    listView.BackColor = value;
            }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("HoverColor")]
        public Color HoverColor
        {
            get => hoverColor;
            set
            {
                hoverColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("SelectColor")]
        public Color SelectColor
        {
            get => selectColor;
            set
            {
                selectColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("SelectTextColor")]
        public Color SelectTextColor
        {
            get => selectTextColor;
            set
            {
                selectTextColor = value;
            }
        }


        // Enable Designer support for Columns
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("ListView")]
        [Description("The columns in the ListView.")]
        public ColumnHeaderCollection Columns => listView.Columns;

        // Expose Items for programmatic access
        [Browsable(true)]
        public ListViewItemCollection Items => listView.Items;

        // Method to add items programmatically
        public void AddItem(ListViewItem item)
        {
            listView.Items.Add(item);
        }

        public GradientListViewControl()
        {
            InitializeComponent();

    //        SetStyle(
    //// ControlStyles.SupportsTransparentBackColor |
    // ControlStyles.OptimizedDoubleBuffer |
    // ControlStyles.ResizeRedraw,
    // true);
          //  BackColor = Color.Transparent;

            InitializeListView();
            InitializeHeaderFillPanel();
            UpdateGradientBitmap();

            this.DoubleBuffered = true;
        

            // Subscribe to mouse events for hover tracking
            listView.MouseMove += ListView_MouseMove;
            listView.MouseLeave += ListView_MouseLeave;


            // <<< HERE: enable drag & drop >>>
            //this.AllowDrop = true;            // if you ever want to drop on the blank panel too
            //listView.AllowDrop = true;        // enable drop onto the ListView itself
            //listView.DragEnter += ListView_DragEnter;
            //listView.DragDrop += ListView_DragDrop;

            //listView.KeyDown += ListView_KeyDown;
            //listView.KeyPress += ListView_KeyPress;
            //listView.KeyUp += ListView_KeyUp;
        }

        //private void ListView_DragEnter(object sender, DragEventArgs e)
        //{
        //    // we only accept file-drops
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //        e.Effect = DragDropEffects.Copy;
        //    else
        //        e.Effect = DragDropEffects.None;
        //}

        //private void ListView_DragDrop(object sender, DragEventArgs e)
        //{
        //    if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        //        return;

        //    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
        //    foreach (var f in files)
        //    {
        //        // For demo purposes we just show the file name.
        //        // You could raise an event, or store the full path in item.Tag, etc.
        //        var item = new ListViewItem(Path.GetFileName(f))
        //        {
        //            Tag = f
        //        };
        //        listView.Items.Add(item);
        //    }
        //}

        const int WM_DROPFILES = 0x0233;
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("shell32.dll")]
        static extern void DragAcceptFiles(IntPtr hWnd, bool fAccept);
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int DragQueryFile(IntPtr hDrop, uint iFile,
                                        System.Text.StringBuilder lpszFile, int cch);
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("shell32.dll")]
        static extern void DragFinish(IntPtr hDrop);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            DragAcceptFiles(this.listView.Handle, true);
            DragDropUnblocker.UnblockOLEMimeMessages(this.listView.Handle);
        }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle KeyDown event
            Console.WriteLine($"KeyDown: {e.KeyCode}");
        }

        private void ListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Handle KeyPress event
            Console.WriteLine($"KeyPress: {e.KeyChar}");
        }

        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            // Handle KeyUp event
            Console.WriteLine($"KeyUp: {e.KeyCode}");
        }

        public new event KeyEventHandler KeyDown
        {
            add { listView.KeyDown += value; }
            remove { listView.KeyDown -= value; }
        }

        public new event KeyPressEventHandler KeyPress
        {
            add { listView.KeyPress += value; }
            remove { listView.KeyPress -= value; }
        }

        public new event KeyEventHandler KeyUp
        {
            add { listView.KeyUp += value; }
            remove { listView.KeyUp -= value; }
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // paint your gradient in the entire client area
            using (var brush = new LinearGradientBrush(
                     ClientRectangle,
                     headerStartColor,
                     headerEndColor,
                     LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
        }


        private void InitializeListView()
        {
            // Initialize ImageList
            imageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(16, 16) // Adjust size as needed
            };


            // Initialize ListView
            //listView = new ListView
            //{
            //    OwnerDraw = true,
            //    View = View.Details,
            //    Dock = DockStyle.Fill,
            //    BorderStyle = BorderStyle.None,
            //    FullRowSelect = true,
            //    SmallImageList = imageList // Assign the ImageList
            //};

            listView = new ListView
            {
                OwnerDraw = true,
                View = View.Details,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                FullRowSelect = true,
                SmallImageList = imageList,
               // BackColor = Color.Transparent
            };

            listView.DrawColumnHeader += ListView_DrawColumnHeader;
            listView.DrawItem += ListView_DrawItem;
            listView.DrawSubItem += ListView_DrawSubItem;
            listView.Resize += ListView_Resize;
            listView.ColumnWidthChanged += ListView_ColumnWidthChanged;
            listView.ColumnWidthChanging += ListView_ColumnWidthChanging;
            listView.ColumnClick += ListView_ColumnClick;
            listView.ItemMouseHover += ListView_ItemMouseHover;


             


            // Enable double buffering to reduce flicker
            typeof(ListView).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                listView,
                new object[] { true }
            );

            // Add ListView to the UserControl
            this.Controls.Add(listView);
        }


    
        private void ListView_ColumnClick(object? sender, ColumnClickEventArgs e)
        {
            try
            {
                var column = listView.Columns[e.Column];
                if (column != null)
                {
                    int maxWidth = 64;
                    for (int i = 0; i < listView.VirtualListSize; i++)
                    {
                        // this.DoSomething(listView.Items[i]);
                        var itemtext = listView.Items[i].SubItems[e.Column].Text;
                        var itemLen = MeasureStringSize(itemtext, listView.Items[i].Font);
                        if (itemLen.Width > maxWidth)
                        {
                            maxWidth = itemLen.Width;
                        }
                    }


                    listView.Columns[e.Column].Width = maxWidth;
                }
            }
            catch
            {

            }
           

        }

        public static Size MeasureStringSize(string text, Font font)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (font == null)
                throw new ArgumentNullException(nameof(font));

            // Use TextRenderer.MeasureText for accurate measurements in WinForms
            return TextRenderer.MeasureText(text, font);
        }

        private void ListView_ItemMouseHover(object? sender, ListViewItemMouseHoverEventArgs e)
        {
           // e.Item.BackColor = HeaderStartColor;
           // throw new NotImplementedException();
        }

        private void InitializeHeaderFillPanel()
        {
            try
            {
                // Initialize Panel to cover the extra header space
                headerFillPanel = new Panel
                {
                    Height = TextRenderer.MeasureText("Dummy", listView.Font).Height + 10, // Adjust as needed
                    BackColor = Color.Transparent,
                    Location = new Point(0, 0),
                    Width = 0, // Will be set in ReadjustHeaders
                               // Dock = DockStyle.Top,
                    TabStop = false // Prevent the panel from receiving focus
                };
                headerFillPanel.Paint += HeaderFillPanel_Paint;

                // Add the panel over the ListView's headers
                this.Controls.Add(headerFillPanel);
                headerFillPanel.BringToFront();
            }
            catch { }
        }

        /// <summary>
        /// Updates the gradient bitmap based on current properties and ListView size.
        /// </summary>
        private void UpdateGradientBitmap()
        {
            try
            {
                // Dispose the old bitmap if it exists
                gradientBitmap?.Dispose();

                if (listView.ClientSize.Width <= 0 || listView.Font == null)
                    return;

                // Calculate header height
                int headerHeight = TextRenderer.MeasureText("Dummy", listView.Font).Height + 10;

                // Create a bitmap spanning the entire ListView width
                gradientBitmap = new Bitmap(listView.ClientSize.Width, headerHeight);
                using (Graphics g = Graphics.FromImage(gradientBitmap))
                {
                    using (LinearGradientBrush brush = CreateLinearGradientBrush(listView.ClientSize.Width, headerHeight))
                    {
                        g.FillRectangle(brush, 0, 0, listView.ClientSize.Width, headerHeight);
                    }
                }

                // Invalidate to trigger redraw
                listView.Invalidate();
                headerFillPanel?.Invalidate();
            }
            catch { }
        }

        /// <summary>
        /// Creates a LinearGradientBrush based on current properties.
        /// </summary>
        private LinearGradientBrush CreateLinearGradientBrush(int width, int height)
        {
            // Determine gradient direction
            LinearGradientMode mode = GetLinearGradientMode();

            return new LinearGradientBrush(
                new Point(0, 0),
                new Point(width, 0),
                headerStartColor,
                headerEndColor)
            {
                // Set Blend to ensure smooth transition
                Blend = new Blend
                {
                    Positions = new float[] { 0f, 1f },
                    Factors = new float[] { 0f, 1f }
                }
            };
        }

        /// <summary>
        /// Determines the LinearGradientMode based on GradientSide properties.
        /// </summary>
        private LinearGradientMode GetLinearGradientMode()
        {
            if ((headerGradientStartSide == GradientSide.Left && headerGradientEndSide == GradientSide.Right) ||
                (headerGradientStartSide == GradientSide.Right && headerGradientEndSide == GradientSide.Left))
            {
                return LinearGradientMode.Horizontal;
            }
            if ((headerGradientStartSide == GradientSide.Top && headerGradientEndSide == GradientSide.Bottom) ||
                (headerGradientStartSide == GradientSide.Bottom && headerGradientEndSide == GradientSide.Top))
            {
                return LinearGradientMode.Vertical;
            }
            // Default to horizontal
            return LinearGradientMode.Horizontal;
        }

        public Color colorSep = Color.FromArgb(100, 100, 100);

        // Handle drawing of each column header
        private void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            try
            {
                if (gradientBitmap != null)
                {
                    // Define the rectangle to draw from the gradient bitmap
                    Rectangle headerBounds = e.Bounds;

                    // Calculate corresponding source rectangle from the gradient bitmap
                    Rectangle sourceRect = new Rectangle(headerBounds.X, 0, headerBounds.Width, headerBounds.Height);

                    // Ensure sourceRect does not exceed the bitmap
                    if (sourceRect.Right > gradientBitmap.Width)
                        sourceRect.Width = gradientBitmap.Width - sourceRect.X;

                    // Destination rectangle in the header
                    Rectangle destRect = headerBounds;

                    // Draw the corresponding portion of the gradient bitmap onto the header
                    e.Graphics.DrawImage(gradientBitmap, destRect, sourceRect, GraphicsUnit.Pixel);
                }
                else
                {
                    // Fallback to solid color if gradient bitmap is not available
                    using (SolidBrush brush = new SolidBrush(headerStartColor))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                }

                // Draw the border
                using (Pen pen = new Pen(colorSep))
                {
                    e.Graphics.DrawLine(pen, e.Bounds.Right - 1, 0, e.Bounds.Right - 1, e.Bounds.Bottom);
                    //e.Graphics.DrawRectangle(pen, e.Bounds);
                }

                // Draw the header text
                TextRenderer.DrawText(
                    e.Graphics,
                    e.Header.Text,
                    e.Font,
                    e.Bounds,
                    selectTextColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
                );
            }
            catch
            {

            }
        }

        // Handle additional painting for the remaining panel
        private void HeaderFillPanel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (gradientBitmap != null)
                {
                    // Calculate total width of all columns
                    int totalWidth = 0;
                    foreach (ColumnHeader column in listView.Columns)
                    {
                        totalWidth += column.Width;
                    }

                    // Define the rectangle corresponding to the panel's position in the gradient bitmap
                    Rectangle sourceRect = new Rectangle(totalWidth, 0, headerFillPanel.Width, headerFillPanel.Height);

                    // Adjust if sourceRect exceeds the bitmap
                    if (sourceRect.Right > gradientBitmap.Width)
                        sourceRect.Width = gradientBitmap.Width - sourceRect.X;

                    // Define the destination rectangle in the panel
                    Rectangle destRect = new Rectangle(0, 0, headerFillPanel.Width, headerFillPanel.Height);

                    // Draw the corresponding part of the gradient bitmap onto the panel
                    e.Graphics.DrawImage(gradientBitmap, destRect, sourceRect, GraphicsUnit.Pixel);
                }
                else
                {
                    // Fallback to solid color if gradient bitmap is not available
                    using (SolidBrush brush = new SolidBrush(headerStartColor))
                    {
                        e.Graphics.FillRectangle(brush, headerFillPanel.Bounds);
                    }
                }
            }
            catch
            {

            }
        }

        public void ReadjustHeaders()
        {
            try
            {
                // Calculate total width of all columns
                int totalWidth = 0;
                foreach (ColumnHeader column in listView.Columns)
                {
                    totalWidth += column.Width;
                }

                // Calculate remaining width after the last column
                int remainingWidth = listView.ClientSize.Width - totalWidth;
                if (headerFillPanel != null)
                {

                    if (remainingWidth > 0)
                    {
                        // Set the panel's size to cover the remaining space
                        headerFillPanel.Width = remainingWidth;
                        headerFillPanel.Left = totalWidth;
                        headerFillPanel.Height = TextRenderer.MeasureText("Dummy", listView.Font).Height + 9;
                    }
                    else
                    {
                        // No remaining space
                        headerFillPanel.Width = 0;
                    }
                }
                

             

                // Update the gradient bitmap
                UpdateGradientBitmap();
            }
            catch { }
        }


        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                ListViewItem item = listView.GetItemAt(e.X, e.Y);
                int newHoveredIndex = item?.Index ?? -1;

                if (hoveredItemIndex != newHoveredIndex)
                {
                    // Invalidate the previous hovered item to remove hover effect
                    if (hoveredItemIndex != -1 && listView.Items.Count > hoveredItemIndex)
                    {

                        try
                        {
                            listView.Invalidate(listView.Items[hoveredItemIndex].Bounds);
                        }
                        catch
                        {

                        }

                    }

                    hoveredItemIndex = newHoveredIndex;

                    // Invalidate the new hovered item to apply hover effect
                    if (hoveredItemIndex != -1 && listView.Items.Count > hoveredItemIndex)
                    {

                        try
                        {
                            listView.Invalidate(listView.Items[hoveredItemIndex].Bounds);
                        }
                        catch
                        {

                        }


                    }
                }
            }
            catch
            {

            }
        }

        private void ListView_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                if (hoveredItemIndex != -1)
                {
                    try
                    {
                        if (listView.Items.Count >= hoveredItemIndex)
                           listView.Invalidate(listView.Items[hoveredItemIndex].Bounds);

                    }
                    catch
                    {

                    }

                    hoveredItemIndex = -1;
                }
            }
            catch { }
        }

        private void ListView_Resize(object sender, EventArgs e)
        {
            ReadjustHeaders();
        }

        private bool finalAdjust = false;

        private void ListView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            finalAdjust = true;
            ReadjustHeaders();
            finalAdjust = false;
        }

        private void ListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (!finalAdjust)
                ReadjustHeaders();
        }

        // Handle drawing of the background for items (not used in Details view)
        private void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // In Details view, the DrawSubItem event handles the drawing of items
            // This method can remain empty or handle other view modes if necessary
            if (listView.View != View.Details)
            {
                // For other views, handle the drawing here
                e.DrawBackground();
                e.DrawText();
            }
        }

        // Updated DrawSubItem method with hover handling
        private void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            try
            {
                //e.DrawDefault = true;


                bool isSelected = e.Item.Selected;
                bool isHovered = e.Item.Index == hoveredItemIndex;
                Color backColor;
                Color foreColor;

                if (isSelected)
                {
                    backColor = selectColor;
                    foreColor = selectTextColor;
                }
                else if (isHovered)
                {
                    backColor = hoverColor; // Choose a suitable hover color
                    foreColor = selectTextColor;
                }
                else
                {
                    backColor = e.Item.BackColor;
                    foreColor = e.Item.ForeColor;
                }

                // Fill the background
                using (SolidBrush backgroundBrush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
                }

                // Initialize variables for icon and text
                int iconWidth = 0;
                int iconHeight = 0;
                int iconSpacing = 4; // Space between icon and text

                // Check if this is the first column (typically where the icon is displayed)
                if (e.ColumnIndex == 0 && e.Item.ImageIndex != -1 && imageList != null)
                {
                    // Retrieve the image from the ImageList
                    //if (imageList.Images.Count > e.Item.ImageIndex)
                    {

                        Image icon = e.Item.ImageList.Images[e.Item.ImageIndex];//imageList.Images[e.Item.ImageIndex];
                        if (icon != null)
                        {
                            // Calculate icon position
                            int iconX = e.Bounds.Left + 2; // Slight padding from the left
                            int iconY = e.Bounds.Top + (e.Bounds.Height - icon.Height) / 2;

                            // Draw the icon
                            e.Graphics.DrawImage(icon, iconX, iconY, icon.Width, icon.Height);

                            // Update icon dimensions and spacing for text positioning
                            iconWidth = icon.Width;
                            iconHeight = icon.Height;
                        }
                    }

                }

                // Define the rectangle for the text, offset by icon width and spacing
                Rectangle textBounds = e.Bounds;
                if (iconWidth > 0)
                {
                    textBounds.X += iconWidth + iconSpacing;
                    textBounds.Width -= iconWidth + iconSpacing;
                }

                // Draw the text
                TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
                TextRenderer.DrawText(
                    e.Graphics,
                    e.SubItem.Text,
                    e.SubItem.Font,
                    textBounds,
                    foreColor,
                    flags
                );

                // Optionally, draw a grid line or separator
                using (Pen pen = new Pen(colorSep))
                {
                    e.Graphics.DrawLine(pen, e.Bounds.Right - 1, e.Bounds.Top, e.Bounds.Right - 1, e.Bounds.Bottom);
                }
            }
            catch { }
        }
    }


}