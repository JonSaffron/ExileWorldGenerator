using System;

namespace ExileWorldGenerator
    {
    internal readonly struct WorldSquare : IEquatable<WorldSquare>
        {
        internal readonly byte X;
        internal readonly byte Y;

        public WorldSquare(byte x, byte y)
            {
            this.X = x;
            this.Y = y;
            }

        public static WorldSquare FromInt(int value)
            {
            byte y = (byte)(value >> 8);
            byte x = (byte)(value & 0xFF);
            return new WorldSquare(x, y);
            }

        public bool Equals(WorldSquare other)
            {
            return this.X == other.X && this.Y == other.Y;
            }

        public override bool Equals(object? obj)
            {
            return obj is WorldSquare other && Equals(other);
            }

        public override int GetHashCode()
            {
            return HashCode.Combine(this.X, this.Y);
            }

        public static bool operator ==(WorldSquare left, WorldSquare right)
            {
            return left.Equals(right);
            }

        public static bool operator !=(WorldSquare left, WorldSquare right)
            {
            return !left.Equals(right);
            }
        }
    }