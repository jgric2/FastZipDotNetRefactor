using System.ComponentModel;

namespace BrutalCopy2025.Controls.BrutalControls
{
    public class MatchMinimap : Control
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TotalLines { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public List<int> MatchLines { get; set; } = new();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedLine { get; set; } = 0;

        // New: paddings to align with scrollbar track (top and bottom arrow buttons)
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TopPadding { get; set; } = 0;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BottomPadding { get; set; } = 0;

        public event Action<int>? LineClicked;

        public MatchMinimap()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint, true);
            BackColor = Color.FromArgb(20, 20, 20);
            ForeColor = Color.FromArgb(253, 190, 4);
            Cursor = Cursors.Hand;
            Width = 12;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(BackColor);

            if (TotalLines <= 0) return;

            int yTop = TopPadding;
            int yBottom = Height - BottomPadding;
            if (yBottom <= yTop) return;

            int trackHeight = yBottom - yTop;
            int totalSpan = Math.Max(1, TotalLines - 1);

            // Optional: draw a faint track background
            using (var trackBrush = new SolidBrush(Color.FromArgb(24, 255, 255, 255)))
            {
                g.FillRectangle(trackBrush, 0, yTop, Width, trackHeight);
            }

            // Draw match ticks
            using var tickPen = new Pen(ForeColor, 1f);
            foreach (var ln in MatchLines)
            {
                if (ln < 1 || ln > TotalLines) continue;
                float t = (ln - 1) / (float)totalSpan;
                float y = yTop + t * (trackHeight - 1);
                int iy = (int)Math.Round(y);
                g.DrawLine(tickPen, 0, iy, Width, iy);
            }

            // Draw selected line indicator (blue)
            if (SelectedLine >= 1 && SelectedLine <= TotalLines)
            {
                float t = (SelectedLine - 1) / (float)totalSpan;
                float y = yTop + t * (trackHeight - 1);
                int iy = (int)Math.Round(y);
                using var selPen = new Pen(Color.FromArgb(220, 100, 200, 255), Math.Max(1f, Width / 2f));
                g.DrawLine(selPen, 0, iy, Width, iy);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (TotalLines <= 0) return;

            int yTop = TopPadding;
            int yBottom = Height - BottomPadding;
            int trackHeight = yBottom - yTop;
            if (trackHeight <= 1) return;

            // Map click Y to [0..1] along the track
            float cy = e.Y;
            float t = (cy - yTop) / (trackHeight - 1);
            t = Math.Max(0f, Math.Min(1f, t));

            int totalSpan = Math.Max(1, TotalLines - 1);
            int line = (int)Math.Round(t * totalSpan) + 1;

            LineClicked?.Invoke(line);
        }
    }
}
