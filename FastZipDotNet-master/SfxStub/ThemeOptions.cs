using BrutalZip2025.BrutalControls;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfxStub
{
    public sealed class ThemeOptions
    {
        public bool UseGradient { get; init; }
        public Color? GradStart { get; init; }
        public Color? GradEnd { get; init; }
        public GradientSide GradStartSide { get; init; } = GradientSide.Left;
        public GradientSide GradEndSide { get; init; } = GradientSide.Right;
        public bool MouseGlow { get; init; }
        public Color? TextColor { get; init; }
        public Color? WindowBackColor { get; init; }
        public Color? AccentColor { get; init; } // fallback for small bars, etc.
    }
}
