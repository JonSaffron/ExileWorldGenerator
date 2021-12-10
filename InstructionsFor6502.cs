using System;
using System.Diagnostics;

// The setting of the overflow flag for add and subtract was based on logic set out in http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html

namespace ExileWorldGenerator
    {
    internal static class InstructionsFor6502
        {
        internal struct Flags
            {
            public bool Carry;
            public bool Zero;
            public bool Negative;
            public bool Overflow;
            }

        internal static void Load(out byte register, byte data, ref Flags flags)
            {
            register = data;
            flags.Zero = (register == 0);
            flags.Negative = (register & 0x80) != 0;
            }

        internal static void Transfer(byte from, out byte to, ref Flags flags)
            {
            Load(out to, from, ref flags);
            }

        internal static void AddWithCarry(ref byte accumulator, byte data, ref Flags flags)
            {
            bool carryIntoBit7 = (((accumulator & 0x7f) + (data & 0x7f) + (flags.Carry ? 1 : 0)) & 0x80) != 0;

            int unsignedResult = accumulator + data + (flags.Carry ? 1 : 0);
            accumulator = (byte) (unsignedResult & 0xff);
            flags.Carry = unsignedResult > 255;
            flags.Negative = (unsignedResult & 0x80) != 0;
            flags.Overflow = flags.Carry ^ carryIntoBit7;
            flags.Zero = accumulator == 0;
            }

        internal static void SubtractWithBorrow(ref byte accumulator, byte data, ref Flags flags)
            {
            bool carryIntoBit7 = (((accumulator & 0x7f) + (~data & 0x7f) + (flags.Carry ? 1 : 0)) & 0x80) != 0;

            int result = accumulator - data - (flags.Carry ? 0 : 1);
            accumulator = (byte) (result & 0xff);
            flags.Carry = (result >= 0);
            flags.Negative = (accumulator & 0x80) != 0;
            flags.Overflow = flags.Carry ^ carryIntoBit7;
            flags.Zero = accumulator == 0;
            }

        internal static void LogicalShiftRight(ref byte accumulator, ref Flags flags)
            {
            flags.Carry = (accumulator & 1) != 0;
            accumulator = (byte) (accumulator >> 1);
            }

        internal static void ArithmeticShiftLeft(ref byte accumulator, ref Flags flags)
            {
            flags.Carry = (accumulator & 0x80) != 0;
            int result = accumulator << 1;
            accumulator = (byte) (result & 0xff);
            }    

        internal static void RotateLeft(ref byte accumulator, ref Flags flags)
            {
            bool currentCarryValue = flags.Carry;
            flags.Carry = (accumulator & 0x80) != 0;
            accumulator <<= 1;
            if (currentCarryValue)
                accumulator |= 1;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        internal static void RotateRight(ref byte data, ref Flags flags)
            {
            bool currentCarryValue = flags.Carry;
            flags.Carry = (data & 0x1) != 0;
            data >>= 1;
            if (currentCarryValue)
                data |= 0x80;
            flags.Zero = (data == 0);
            flags.Negative = (data & 0x80) != 0;
            }

        internal static void Compare(byte registerValue, byte operand, ref Flags flags)
            {
            int result = registerValue - operand;
            flags.Carry = registerValue >= operand;
            flags.Zero = registerValue == operand;
            flags.Negative = (result & 0x80) != 0;
            }

        internal static void And(ref byte accumulator, byte operand, ref Flags flags)
            {
            accumulator &= operand;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        internal static void Or(ref byte accumulator, byte operand, ref Flags flags)
            {
            accumulator |= operand;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        internal static void Eor(ref byte accumulator, byte operand, ref Flags flags)
            {
            accumulator ^= operand;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        internal static void BitTest(byte accumulator, byte operand, ref Flags flags)
            {
            byte result = (byte) (accumulator & operand);
            flags.Zero = result == 0;
            flags.Negative = (operand & 0x80) != 0;
            flags.Overflow = (operand & 0x40) != 0;
            }

        internal static void Increment(ref byte register, ref Flags flags)
            {
            int result = register + 1;
            register = (byte) (result & 0xff);
            flags.Zero = register == 0;
            flags.Negative = (register & 0x80) != 0;
            }

        internal static void Decrement(ref byte register, ref Flags flags)
            {
            int result = register - 1;
            register = (byte) (result & 0xff);
            flags.Zero = register == 0;
            flags.Negative = (register & 0x80) != 0;
            }


        }
    }
