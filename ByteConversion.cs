namespace ExileWorldGenerator
    {
    internal static class ByteConversion
        {
        internal static sbyte ToSigned(this byte value)
            {
            if (value <= 127)
                return checked((sbyte) value);

            return checked((sbyte) (value - 256));
            }

        internal static byte ToUnsigned(this sbyte value)
            {
            if (value >= 0)
                return checked((byte) value);

            return checked((byte) (value + 256));
            }
        }
    }
