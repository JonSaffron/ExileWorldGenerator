using System;
using System.Collections.Generic;
using System.Drawing;

namespace ExileMappedBackground
    {
    class GamePalette
        {
        public GameColour Colour1;
        public GameColour Colour2;
        public GameColour Colour3;

        private static readonly Dictionary<GameColour, Color> GameColours = BuildGameColours();

        public Color this[int paletteIndex]
            {
            get
                {
                if (paletteIndex == 0)
                    return Color.Black;
                if (paletteIndex == 1)
                    return GameColours[Colour1];
                if (paletteIndex == 2)
                    return GameColours[Colour2];
                if (paletteIndex == 3)
                    return GameColours[Colour3];

                throw new ArgumentOutOfRangeException(nameof(paletteIndex));
                }
            }

        private static Dictionary<GameColour, Color> BuildGameColours()
            {
            var result = new Dictionary<GameColour, Color>
                {
                    {GameColour.BlackForeground, Color.Black},
                    {GameColour.RedForeground, Color.Red},
                    {GameColour.GreenForeground, Color.Green},
                    {GameColour.YellowForeground, Color.Yellow},
                    {GameColour.BlueForeground, Color.Blue},
                    {GameColour.MagentaForeground, Color.Magenta},
                    {GameColour.CyanForeground, Color.Cyan},
                    {GameColour.WhiteForeground, Color.White},

                    {GameColour.BlackBackground, Color.Black},
                    {GameColour.RedBackground, Color.Red},
                    {GameColour.GreenBackground, Color.Green},
                    {GameColour.YellowBackground, Color.Yellow},
                    {GameColour.BlueBackground, Color.Blue},
                    {GameColour.MagentaBackground, Color.Magenta},
                    {GameColour.CyanBackground, Color.Cyan},
                    {GameColour.WhiteBackground, Color.White}
                };
            return result;
            }
        }
    }
