using System;
using System.Diagnostics;

namespace ExileWorldGenerator
    {
    internal class SquareProperties
        {
        internal SquareProperties(WorldSquare worldSquare, GeneratedBackground generatedBackground, BackgroundOverride? backgroundOverride, GetPaletteResult getPaletteResult)
            {
            this.WorldSquare = worldSquare;
            this.GeneratedBackground = generatedBackground ?? throw new ArgumentNullException(nameof(generatedBackground));
            this.BackgroundOverride = backgroundOverride;
            this.GetPaletteResult = getPaletteResult ?? throw new ArgumentNullException(nameof(getPaletteResult));
            }

        public readonly WorldSquare WorldSquare;

        public readonly GeneratedBackground GeneratedBackground;
        public readonly BackgroundOverride? BackgroundOverride;
        public readonly GetPaletteResult GetPaletteResult;

        public byte FinalBackground
            {
            get
                {
                byte result;
                if (this.GetPaletteResult.Background.HasValue)
                    result = this.GetPaletteResult.Background.Value;
                else if (this.BackgroundOverride != null)
                    result = this.BackgroundOverride.Background;
                else
                    result = this.GeneratedBackground.Background;
                Debug.Assert(result <= 0x3f);
                return result;
                }
            }

        public byte FinalOrientation
            {
            get
                {
                byte result;
                if (this.GetPaletteResult.Orientation.HasValue)
                    result = this.GetPaletteResult.Orientation.Value;
                else if (this.BackgroundOverride != null)
                    result = this.BackgroundOverride.Orientation;
                else
                    result = this.GeneratedBackground.Orientation;
                Debug.Assert((result & 0x3f) == 0);
                return result;
                }
            }

        public string? BackgroundHandlerType;

        public string? BackgroundDescription;

        public TimeSpan? NextAnimationFrame;
        public int AnimationFrame;
        }
    }
