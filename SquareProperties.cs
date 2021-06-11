using System;

namespace ExileWorldGenerator
    {
    internal class SquareProperties
        {
        public byte X;
        public byte Y;

        public GeneratedBackground GeneratedBackground;
        public BackgroundObjectData BackgroundObjectData;
        public PaletteData PaletteData;

        public byte Background;

        public string BackgroundHandlerType;

        public string BackgroundDescription;

        public TimeSpan? NextAnimationFrame;
        public int AnimationFrame;
        }
    }