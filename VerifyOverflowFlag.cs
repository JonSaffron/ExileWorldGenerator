using System;
using System.Diagnostics;
using static ExileWorldGenerator.InstructionsFor6502;

namespace ExileWorldGenerator
    {
    internal class VerifyOverflowFlag
        {
        internal static void TestOverflowFlagResultAfterAdd(bool carry)
            {
            byte i = 0;
            do
                {
                byte j = 0;
                do
                    {
                    var si = i.ToSigned();
                    var sj = j.ToSigned();
                    bool didCalcOverflow;
                    try
                        {
                        var sr = (sbyte) (si + sj + (carry ? 1 : 0));
                        didCalcOverflow = false;
                        }
                    catch (OverflowException)
                        {
                        didCalcOverflow = true;
                        }

                    var c6 = (((i & 0b0111_1111) + (j & 0b0111_1111) + (carry ? 1 : 0)) & 0b1000_0000) != 0; 
                    var i7 = (i & 0b1000_0000) != 0;
                    var j7 = (j & 0b1000_0000) != 0;

                    bool mathOverflowPredicted = (!i7 && !j7 && c6) || (i7 && j7 && !c6);
                    Debug.Assert(mathOverflowPredicted == didCalcOverflow);

                    byte accumulator = i;
                    var flags = new Flags {Carry = carry};
                    AddWithCarry(ref accumulator, j, ref flags);
                    Debug.Assert(mathOverflowPredicted == flags.Overflow);

                    j = unchecked((byte) (j + 1));
                    if (j == 0)
                        break;
                    } while (true);

                i = unchecked((byte) (i + 1));
                if (i == 0)
                    break;
                } while (true);
            }

        internal static void TestOverflowFlagResultAfterSubtract(bool carry)
            {
            byte i = 0;
            do
                {
                byte j = 0;
                do
                    {
                    var si = i.ToSigned();
                    var sj = j.ToSigned();
                    bool didCalcOverflow;
                    try
                        {
                        var sr = (sbyte) ((si - sj) - (!carry ? 1 : 0));
                        didCalcOverflow = false;
                        }
                    catch (OverflowException)
                        {
                        didCalcOverflow = true;
                        }

                    var c6 = (((i & 0b0111_1111) + (~j & 0b0111_1111) + (carry ? 1 : 0)) & 0b1000_0000) != 0; 
                    var i7 = (i & 0b1000_0000) != 0;
                    var j7 = (~j & 0b1000_0000) != 0;

                    bool mathOverflowPredicted = (!i7 && !j7 && c6) || (i7 && j7 && !c6);
                    Debug.Assert(mathOverflowPredicted == didCalcOverflow);

                    byte accumulator = i;
                    var flags = new Flags {Carry = carry};
                    SubtractWithBorrow(ref accumulator, j, ref flags);
                    Debug.Assert(mathOverflowPredicted == flags.Overflow);

                    j = unchecked((byte) (j + 1));
                    if (j == 0)
                        break;
                    } while (true);

                i = unchecked((byte) (i + 1));
                if (i == 0)
                    break;
                } while (true);
            }
        }
    }
