using System;

namespace ExileWorldGenerator
    {
    internal class GetPaletteResult
        {
        public readonly PaletteData PaletteData;
        public readonly byte? Background;
        public readonly byte? Orientation;

        public GetPaletteResult(PaletteData paletteData, byte? background, byte? orientation)
            {
            if (background.HasValue && background > 0x3f) throw new ArgumentOutOfRangeException(nameof(background));
            if (orientation.HasValue && (orientation & 0x3f) != 0) throw new ArgumentOutOfRangeException(nameof(orientation));
            if (background.HasValue && !orientation.HasValue) throw new InvalidOperationException("When Background is set, Orientation should also be set.");
            this.Orientation = orientation;
            this.Background = background;
            this.PaletteData = paletteData;
            }
        }
    }
