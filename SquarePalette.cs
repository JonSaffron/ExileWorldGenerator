using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ExileWorldGenerator
    {
    class SquarePalette
        {
        public GameColour Colour1;
        public GameColour Colour2;
        public GameColour PrimaryColour;

        private static readonly Dictionary<GameColour, Color> GameColours = BuildGameColours();
        private static readonly (GameColour leftColour, GameColour rightColour)[] ColourPairs = BuildColourPairs();

        public static SquarePalette FromByte(byte palette)
            {
            var colourPair = ColourPairs[palette & 0xf];
            var result = new SquarePalette
                {
                Colour1 = colourPair.rightColour,
                Colour2 = colourPair.leftColour,
                PrimaryColour = (GameColour) (palette >> 4)
                };
            return result;
            }
        
        public Color this[int paletteIndex]
            {
            get
                {
                switch (paletteIndex)
                    {
                    case 0:
                        return Color.Black;
                    case 1:
                        return GameColours[Colour1];
                    case 2:
                        return GameColours[Colour2];
                    case 3:
                        return GameColours[PrimaryColour];
                    default:
                        throw new ArgumentOutOfRangeException(nameof(paletteIndex));
                    }
                }
            }

        private static Dictionary<GameColour, Color> BuildGameColours()
            {
            var result = new Dictionary<GameColour, Color>
                {
                    {GameColour.BlackForeground, Color.Black},
                    {GameColour.RedForeground, Color.Red},
                    {GameColour.GreenForeground, Color.Lime},
                    {GameColour.YellowForeground, Color.Yellow},
                    {GameColour.BlueForeground, Color.Blue},
                    {GameColour.MagentaForeground, Color.Magenta},
                    {GameColour.CyanForeground, Color.Cyan},
                    {GameColour.WhiteForeground, Color.White},

                    {GameColour.BlackBackground, Color.Black},
                    {GameColour.RedBackground, Color.Red},
                    {GameColour.GreenBackground, Color.Lime},
                    {GameColour.YellowBackground, Color.Yellow},
                    {GameColour.BlueBackground, Color.Blue},
                    {GameColour.MagentaBackground, Color.Magenta},
                    {GameColour.CyanBackground, Color.Cyan},
                    {GameColour.WhiteBackground, Color.White}
                };
            return result;
            }

        private static (GameColour leftColour, GameColour rightColour)[] BuildColourPairs()
            {
            var colourMap = new Dictionary<byte, GameColour>
                {
                    {0b00000000, GameColour.BlackForeground},
                    {0b00000001, GameColour.RedForeground},
                    {0b00000100, GameColour.GreenForeground},
                    {0b00000101, GameColour.YellowForeground},
                    {0b00010000, GameColour.BlueForeground},
                    {0b00010001, GameColour.MagentaForeground},
                    {0b00010100, GameColour.CyanForeground},
                    {0b00010101, GameColour.WhiteForeground},
                    {0b01000000, GameColour.BlackBackground},
                    {0b01000001, GameColour.RedBackground},
                    {0b01000100, GameColour.GreenBackground},
                    {0b01000101, GameColour.YellowBackground},
                    {0b01010000, GameColour.BlueBackground},
                    {0b01010001, GameColour.MagentaBackground},
                    {0b01010100, GameColour.CyanBackground},
                    {0b01010101, GameColour.WhiteBackground}
                };
            var result = 
            (
                from palettePair in BuildPaletteValueToPixelLookup()
                let leftPixel = (palettePair & 0b10101010) >> 1
                let rightPixel = (palettePair & 0b01010101)
                let leftColour = colourMap[(byte) leftPixel]
                let rightColour = colourMap[(byte) rightPixel]
                select (leftColour, rightColour)
            ).ToArray();
            return result;
            }

        private static byte[] BuildPaletteValueToPixelLookup()
            {
            return new byte[] {0xca, 0xc9, 0xe3, 0xe9, 0xeb, 0xce, 0xf8, 0xe6, 0xcc, 0xee, 0x30, 0xde, 0xef, 0xcb, 0xfb, 0xfe};
            }
        }
    }
