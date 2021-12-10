using System.Diagnostics;
using static ExileWorldGenerator.InstructionsFor6502;

namespace ExileWorldGenerator
    {
    internal static class ShowAngleToVector
        {
        internal static void TestAngle()
            {
            byte angle = 0;
            do
                {
                var result = BuildVector(angle, 0x10);
                Debug.WriteLine($"{angle:X}: ({result.x}, {result.y})");
                if (angle == 255)
                    break;
                angle++;
                } while (true);
            }

        private static (sbyte x, sbyte y) BuildVector(byte angle, byte magnitude)
            {
            byte xcomp = magnitude;
            byte temp = angle;

            byte y = 5;
            byte accumulator = 0;
            Flags f = new Flags();
        loop:
            LogicalShiftRight(ref temp, ref f);
            if (f.Carry)
                {
                f.Carry = false;
                AddWithCarry(ref accumulator, xcomp, ref f);
                }
            RotateRight(ref accumulator, ref f);
            Decrement(ref y, ref f);
            if (!f.Zero)
                goto loop;

            LogicalShiftRight(ref temp, ref f);
            if (f.Carry)
                {
                y = xcomp;
                xcomp = accumulator;
                accumulator = y;

                SubtractWithBorrow(ref accumulator, xcomp, ref f);
                xcomp = accumulator;
                accumulator = y;
                }

            LogicalShiftRight(ref temp, ref f);
            if (f.Carry)
                {
                Eor(ref accumulator, 0xff, ref f);
                y = accumulator;
                Increment(ref y, ref f);

                accumulator = xcomp;
                xcomp = y;
                }

            LogicalShiftRight(ref temp, ref f);
            if (f.Carry)
                {
                Eor(ref accumulator, 0xff, ref f);
                y = accumulator;
                Increment(ref y, ref f);

                accumulator = 0;
                SubtractWithBorrow(ref accumulator, xcomp, ref f);
                xcomp = accumulator;
                accumulator = y;
                }

            return (xcomp.ToSigned(), accumulator.ToSigned());
            }
        }
    }
