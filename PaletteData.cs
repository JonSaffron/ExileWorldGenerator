namespace ExileWorldGenerator
    {
    internal class PaletteData
        {
        /// <summary>
        /// The palette algorithm to apply to the background.
        /// If 6 or lower then the palette will be adjusted according to X and Y position.
        /// </summary>
        public byte BackgroundPalette;

        /// <summary>
        /// The palette that will be used to render the background sprite.
        /// </summary>
        public Palette Palette;
        }
    }
