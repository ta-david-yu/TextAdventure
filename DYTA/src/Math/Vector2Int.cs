using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DYTA.Math
{
    public struct Vector2Int : IEquatable<Vector2Int>
    {
        public static Vector2Int Zero { get { return new Vector2Int(0, 0); } }
        public static Vector2Int One { get { return new Vector2Int(1, 1); } }

        public int X { get; set; }
        public int Y { get; set; }

        public Vector2Int(Vector2Int other)
        {
            X = other.X;
            Y = other.Y;
        }

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2Int)
            {
                return Equals((Vector2Int)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public bool Equals(Vector2Int other)
        {
            return X == other.X && Y == other.Y;
        }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            a.X += b.X;
            a.Y += b.Y;
            return a;
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            a.X -= b.X;
            a.Y -= b.Y;
            return a;
        }

        public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
        {
            return lhs.X == rhs.X && lhs.Y == lhs.Y;
        }

        public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("({0}, {1})", X, Y));
            return sb.ToString();
        }
    }
}
