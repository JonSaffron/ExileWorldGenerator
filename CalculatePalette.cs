using System;
using System.Linq;
using static ExileWorldGenerator.InstructionsFor6502;

namespace ExileWorldGenerator
    {
    internal static class CalculatePalette
        {
        private static readonly byte[] BackgroundPaletteLookup = BuildBackgroundPaletteLookup();
        private static readonly byte[] WallPaletteZeroLookup = BuildWallPaletteZeroLookup();
        private static readonly byte[] WallPaletteThreeLookup = BuildWallPaletteThreeLookup();
        private static readonly byte[] WallPaletteFourLookup = BuildWallPaletteFourLookup();

        internal static PaletteData GetPalette(ref byte background, ref byte orientation, byte squareX, byte squareY)
            {
            if (background > 0x3f)
                throw new ArgumentOutOfRangeException();
            if ((orientation & 0x3f) != 0)
                throw new ArgumentOutOfRangeException();

            // ReSharper disable InlineOutVariableDeclaration
            byte accumulator;
            // ReSharper restore InlineOutVariableDeclaration
            Flags flags = new Flags();

            Load(out accumulator, BackgroundPaletteLookup[background], ref flags);
            var backgroundPalette = accumulator;
            if (!flags.Zero) goto palette_not_zero;

// palette 0
            Load(out accumulator, squareY, ref flags);
            flags.Carry = true;
            SubtractWithBorrow(ref accumulator, 0x54, ref flags);
            LogicalShiftRight(ref accumulator, ref flags);
            LogicalShiftRight(ref accumulator, ref flags);
            LogicalShiftRight(ref accumulator, ref flags);
            LogicalShiftRight(ref accumulator, ref flags);
            byte index = accumulator;
            Load(out accumulator, WallPaletteZeroLookup[index], ref flags);
                
palette_not_zero:
            Compare(accumulator, 0x03, ref flags);
            if (flags.Carry) goto palette_not_one_or_two;

// palette 1 or 2
            AddWithCarry(ref accumulator, 0xb1, ref flags);
            BitTest(accumulator, squareY, ref flags);
            if (!flags.Negative) goto palette_not_six;
            ArithmeticShiftLeft(ref accumulator, ref flags);
            AddWithCarry(ref accumulator, 0x90, ref flags);

palette_not_one_or_two:
            Compare(accumulator, 0x03, ref flags);
            if (!flags.Zero) goto palette_not_three;

// palette 3
            Load(out accumulator, orientation, ref flags);
            RotateLeft(ref accumulator, ref flags);
            RotateLeft(ref accumulator, ref flags);
            RotateLeft(ref accumulator, ref flags);
            SubtractWithBorrow(ref accumulator, squareY, ref flags);
            RotateRight(ref accumulator, ref flags);
            flags.Carry = false;
            AddWithCarry(ref accumulator, squareX, ref flags);
            And(ref accumulator, 0x03, ref flags);
            index = accumulator;
            accumulator = WallPaletteThreeLookup[index];

palette_not_three:
            Compare(accumulator, 0x04, ref flags);
            if (!flags.Zero) goto palette_not_four;

// palette 4
            Load(out accumulator, squareY, ref flags);
            RotateLeft(ref accumulator, ref flags);
            RotateLeft(ref accumulator, ref flags);
            RotateLeft(ref accumulator, ref flags);
            RotateLeft(ref accumulator, ref flags);
            And(ref accumulator, 0x7, ref flags);
            index = accumulator;
            accumulator = WallPaletteFourLookup[index];

palette_not_four:
            Compare(accumulator, 0x05, ref flags);
            if (!flags.Zero) goto palette_not_five;

// palette 5
            Load(out accumulator, squareY, ref flags);
            RotateRight(ref accumulator, ref flags);
            RotateRight(ref accumulator, ref flags);
            Eor(ref accumulator, squareY, ref flags);
            RotateRight(ref accumulator, ref flags);
            if (flags.Carry) 
                background = 0x19;
            RotateRight(ref accumulator, ref flags);
            SubtractWithBorrow(ref accumulator, squareY, ref flags);
            And(ref accumulator, 0x40, ref flags);
            Eor(ref accumulator, orientation, ref flags);
            BitTest(accumulator,orientation, ref flags);
            orientation = accumulator;
            Load(out accumulator, 0xB1, ref flags);
            if (!flags.Overflow) goto palette_not_six;
            AddWithCarry(ref accumulator, 0x0a, ref flags);

palette_not_five:
            Compare(accumulator, 0x06, ref flags);
            if (!flags.Zero) goto palette_not_six;

// palette 6
            Load(out accumulator, 0x9c, ref flags);
            BitTest(accumulator, orientation, ref flags);
            if (!flags.Overflow) goto palette_not_six;
            Load(out accumulator, 0xcf, ref flags);

        palette_not_six:
            var displayedPalette = accumulator;
            var result = new PaletteData {BackgroundPalette = backgroundPalette, Palette = SquarePalette.FromByte(displayedPalette)};
            return result;
            }

        private static byte[] BuildBackgroundPaletteLookup()
            {
            var result = new byte[]
                {
                0x80,0x02,0x91,0x91,0x91,0x00,0x91,0xA8,0xDC,0xB8,0x8C,0x80,0xC9,0x4A,0x80,0x06, // 00
                0x88,0x05,0x04,0x00,0x02,0x02,0x02,0x02,0x02,0x91,0x03,0x03,0x02,0x02,0x00,0x00, // 10
                0x00,0xBC,0xB1,0x00,0x00,0x00,0x01,0x01,0x01,0x01,0x00,0x04,0x04,0x04,0x04,0x04, // 20
                0x04,0x04,0x02,0x01,0x01,0x01,0x02,0x02,0x02,0x00,0x00,0x00,0x82,0x02,0x64,0xEE  // 30
                };
            return result;
            }

        private static byte[] BuildWallPaletteZeroLookup()
            {
            var result = new byte[] {0x8D,0x82,0x8B,0x8F,0x84,0x89,0x8D}
                .Concat(BuildWallPaletteFourLookup())
                .Concat(new byte[] { 0x81 });
            return result.ToArray();
            }

        private static byte[] BuildWallPaletteThreeLookup()
            {
            return new byte[] {0xB1,0x97,0xFD,0xF3};
            }

        private static byte[] BuildWallPaletteFourLookup()
            {
            return new byte[] {0x81,0x82,0x81,0x85,0xB2,0xCD,0x90,0x95};
            }
        }
    }
