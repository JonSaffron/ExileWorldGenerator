namespace ExileWorldGenerator
    {
    internal class PaletteData
        {
        /// <summary>
        /// The palette algorithm to apply to the background.
        /// If 6 or lower than the palette will be adjusted according to X and Y position.
        /// </summary>
        public readonly byte BackgroundPalette;

        /// <summary>
        /// The palette that will be used to render the background sprite.
        /// </summary>
        public readonly Palette Palette;

        public PaletteData(byte backgroundPalette, Palette palette)
            {
            this.BackgroundPalette = backgroundPalette;
            this.Palette = palette;
            }
        }
    }
