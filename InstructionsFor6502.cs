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

            public override string ToString()
                {
                char[] result = new string('•', 4).ToCharArray();
                if (Carry)
                    result[0] = 'C';
                if (Zero)
                    result[1] = 'Z';
                if (Negative)
                    result[2] = 'N';
                if (Overflow)
                    result[3] = 'V';
                return new string(result);
                }
            }

        /// <summary>
        /// Loads a register with a specified value.
        /// </summary>
        /// <param name="register">Specifies the register to set</param>
        /// <param name="data">The value that will be set</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <remarks>
        /// The Negative flag will be set if the value being transferred has bit 7 set. The Negative flag will be cleared otherwise.
        /// The Zero flag will be set if the value being transferred is 0. The Zero flag will be cleared otherwise.
        /// </remarks>
        internal static void Load(out byte register, byte data, ref Flags flags)
            {
            register = data;
            flags.Zero = (register == 0);
            flags.Negative = (register & 0x80) != 0;
            }

        /// <summary>
        /// Copies a value from one register to another.
        /// </summary>
        /// <param name="from">The value to copy</param>
        /// <param name="to">The value that will be set</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <remarks>
        /// The Negative flag will be set if the value being transferred has bit 7 set. The Negative flag will be cleared otherwise.
        /// The Zero flag will be set if the value being transferred is 0. The Zero flag will be cleared otherwise.
        /// </remarks>
        internal static void Transfer(byte from, out byte to, ref Flags flags)
            {
            Load(out to, from, ref flags);
            }

        /// <summary>
        /// Adds the specified amount to the accumulator
        /// </summary>
        /// <param name="accumulator">The value to start with</param>
        /// <param name="data">The value to add</param>
        /// <param name="flags">If carry is clear then an extra 1 is added to the result.</param>
        /// <returns>The result of the calculation is returned in the accumulator parameter. The Carry, Negative, Overflow, and Zero flags reflect the result.</returns>
        /// <remarks>
        /// The Carry flag is set if the result is greater than 255. Carry is cleared otherwise.
        /// The Negative flag will be set if the result has bit 7 set. The Negative flag will be cleared otherwise.
        /// The Overflow flag will be set if the result is outside the range -128 to 127 (useful when performing signed arithmetic). The Overflow flag will be cleared otherwise.
        /// The Zero flag will be set if the resulting value in the accumulator is 0. The Zero flag will be cleared otherwise.
        /// </remarks>
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

        /// <summary>
        /// Subtracts the specified amount from the accumulator
        /// </summary>
        /// <param name="accumulator">The value to start with</param>
        /// <param name="data">The value to subtract</param>
        /// <param name="flags">If carry is clear then an extra 1 is deducted from the result.</param>
        /// <returns>The result of the calculation is returned in the accumulator parameter. The Carry, Negative, Overflow, and Zero flags reflect the result.</returns>
        /// <remarks>
        /// The Carry flag is set if the result is 0 or more (so if data is less than or equal to accumulator). Carry is cleared otherwise (i.e. where the result should be treated as a negative value).
        /// The Negative flag will be set if the result has bit 7 set. The Negative flag will be cleared otherwise.
        /// The Overflow flag will be set if the result is outside the range -128 to 127 (useful when performing signed arithmetic). The Overflow flag will be cleared otherwise.
        /// The Zero flag will be set if the result is 0 (i.e. the accumulator's original value was the same as the amount subtracted). The Zero flag will be cleared otherwise.
        /// </remarks>
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

        /// <summary>
        /// Shifts the bits in the accumulator one place to the right. The lowest bit will be placed into the Carry flag, whilst bit 7 will be set to 0.
        /// </summary>
        /// <param name="accumulator">Specifies the register to affect</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <remarks>
        /// The Carry flag will be set to the original value of the lowest bit in the accumulator.
        /// The Zero flag will be set if the value in the accumulator becomes 0. The Zero flag will be cleared otherwise.
        /// The Negative flag will be cleared.
        /// </remarks>
        internal static void LogicalShiftRight(ref byte accumulator, ref Flags flags)
            {
            flags.Carry = (accumulator & 1) != 0;
            accumulator = (byte) (accumulator >> 1);
            flags.Zero = accumulator == 0;
            flags.Negative = false;
            }

        /// <summary>
        /// Shifts the bits in the accumulator one place to the left. The highest bit will be placed into the Carry flag, whilst bit 0 will be set to 0.
        /// </summary>
        /// <param name="accumulator">Specifies the register to affect</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <remarks>
        /// The Carry flag will be set to the original value of the highest bit in the accumulator.
        /// The Zero flag will be set if the value in the accumulator becomes 0. The Zero flag will be cleared otherwise.
        /// The Negative flag will be set if the resulting bit 7 of the accumulator is set. The Negative flag will be cleared otherwise.
        /// </remarks>
        internal static void ArithmeticShiftLeft(ref byte accumulator, ref Flags flags)
            {
            flags.Carry = (accumulator & 0x80) != 0;
            int result = accumulator << 1;
            accumulator = (byte) (result & 0xff);
            flags.Zero = accumulator == 0;
            flags.Negative = (accumulator & 0x80) != 0;
            }    

        /// <summary>
        /// Rotates the bits in the accumulator one place to the left. The highest bit will be placed into the Carry flag, whilst bit 0 will be set to the previous value of the Carry flag.
        /// </summary>
        /// <param name="accumulator">Specifies the register to affect</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <remarks>
        /// The Carry flag will be set to the original value of the highest bit in the accumulator.
        /// The Zero flag will be set if the value in the accumulator becomes 0. The Zero flag will be cleared otherwise.
        /// The Negative flag will be set if the resulting bit 7 of the accumulator is set. The Negative flag will be cleared otherwise.
        /// </remarks>
        internal static void RotateLeft(ref byte accumulator, ref Flags flags)
            {
            bool currentCarryValue = flags.Carry;
            flags.Carry = (accumulator & 0x80) != 0;
            accumulator = (byte) ((accumulator & 0x7f) << 1) ;
            if (currentCarryValue)
                accumulator |= 1;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        /// <summary>
        /// Rotates the bits in the accumulator one place to the right. The lowest bit will be placed into the Carry flag, whilst bit 7 will be set to the previous value of the Carry flag.
        /// </summary>
        /// <param name="accumulator">Specifies the register to affect</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <remarks>
        /// The Carry flag will be set to the original value of the lowest bit in the accumulator.
        /// The Zero flag will be set if the value in the accumulator becomes 0. The Zero flag will be cleared otherwise.
        /// The Negative flag will be set if the resulting bit 7 of the accumulator is set. The Negative flag will be cleared otherwise.
        /// </remarks>
        internal static void RotateRight(ref byte accumulator, ref Flags flags)
            {
            bool currentCarryValue = flags.Carry;
            flags.Carry = (accumulator & 0x1) != 0;
            accumulator >>= 1;
            if (currentCarryValue)
                accumulator |= 0x80;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        /// <summary>
        /// Compares one value with another and sets the flags to reflect the results
        /// </summary>
        /// <param name="registerValue">The register value</param>
        /// <param name="data">A value to subtract from the register value</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <remarks>
        /// The Carry flag is set if the result is 0 or more (so if data is less than or equal to the register value). Carry is cleared otherwise (i.e. where the result would be a negative value).
        /// The Negative flag will be set if the result has bit 7 set. The Negative flag will be cleared otherwise.
        /// The Zero flag will be set if the result is 0 (i.e. the register's value was the same as the data). The Zero flag will be cleared otherwise.
        /// </remarks>
        internal static void Compare(byte registerValue, byte data, ref Flags flags)
            {
            int result = registerValue - data;
            flags.Carry = registerValue >= data;
            flags.Zero = registerValue == data;
            flags.Negative = (result & 0x80) != 0;
            }

        /// <summary>
        /// ANDs the accumulator with a specified value and sets the flags to reflect the results
        /// </summary>
        /// <param name="accumulator">The value to AND.</param>
        /// <param name="operand">The second value</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <remarks>
        /// The Negative flag will be set if the result has bit 7 set. The Negative flag will be cleared otherwise.
        /// The Zero flag will be set if the result is 0. The Zero flag will be cleared otherwise.
        /// </remarks>
        internal static void And(ref byte accumulator, byte operand, ref Flags flags)
            {
            accumulator &= operand;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        /// <summary>
        /// ORs the accumulator with a specified value and sets the flags to reflect the results
        /// </summary>
        /// <param name="accumulator">The value to OR.</param>
        /// <param name="operand">The second value</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <remarks>
        /// The Negative flag will be set if the result has bit 7 set. The Negative flag will be cleared otherwise.
        /// The Zero flag will be set if the result is 0. The Zero flag will be cleared otherwise.
        /// </remarks>
        internal static void Or(ref byte accumulator, byte operand, ref Flags flags)
            {
            accumulator |= operand;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        /// <summary>
        /// EORs the accumulator with a specified value and sets the flags to reflect the results
        /// </summary>
        /// <param name="accumulator">The value to EOR.</param>
        /// <param name="operand">The second value</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <remarks>
        /// The Negative flag will be set if the result has bit 7 set. The Negative flag will be cleared otherwise.
        /// The Zero flag will be set if the result is 0. The Zero flag will be cleared otherwise.
        /// </remarks>
        internal static void Eor(ref byte accumulator, byte operand, ref Flags flags)
            {
            accumulator ^= operand;
            flags.Zero = (accumulator == 0);
            flags.Negative = (accumulator & 0x80) != 0;
            }

        /// <summary>
        /// Performs a bit test and sets the flags to indicate the result
        /// </summary>
        /// <param name="accumulator">The value to test</param>
        /// <param name="operand">The value to AND with the accumulator</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <remarks>
        /// The Negative flag will be set if the operand has bit 7 set. The Negative flag will be cleared otherwise.
        /// The Overflow flag will be set if the operand has bit 6 set. The Overflow flag will be cleared otherwise.
        /// The Zero flag will be set if result of ANDing the accumulator with the operand is 0. The Zero flag will be cleared otherwise.
        /// </remarks>
        internal static void BitTest(byte accumulator, byte operand, ref Flags flags)
            {
            byte result = (byte) (accumulator & operand);
            flags.Zero = result == 0;
            flags.Negative = (operand & 0x80) != 0;
            flags.Overflow = (operand & 0x40) != 0;
            }

        /// <summary>
        /// Increments the specified register.
        /// </summary>
        /// <param name="register">Specifies a reference to the value to increment.</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <returns>The incremented value will be returned in the register parameter, or 0 if the original value was 255.</returns>
        /// <remarks>
        /// The Negative flag will be set if the result has bit 7 set. The Negative flag will be cleared otherwise.
        /// The Zero flag will be set if result is 0. The Zero flag will be cleared otherwise.
        /// </remarks>
        internal static void Increment(ref byte register, ref Flags flags)
            {
            int result = register + 1;
            register = (byte) (result & 0xff);
            flags.Zero = register == 0;
            flags.Negative = (register & 0x80) != 0;
            }

        /// <summary>
        /// Decrements the specified register.
        /// </summary>
        /// <param name="register">Specifies a reference to the value to decrement.</param>
        /// <param name="flags">A reference to the flags that will be set</param>
        /// <returns>The decremented value will be returned in the register parameter, or 255 if the original value was 0.</returns>
        /// <remarks>
        /// The Negative flag will be set if the result has bit 7 set. The Negative flag will be cleared otherwise.
        /// The Zero flag will be set if result is 0. The Zero flag will be cleared otherwise.
        /// </remarks>
        internal static void Decrement(ref byte register, ref Flags flags)
            {
            int result = register - 1;
            register = (byte) (result & 0xff);
            flags.Zero = register == 0;
            flags.Negative = (register & 0x80) != 0;
            }
        }
    }
