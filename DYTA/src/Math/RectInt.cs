using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DYTA.Math
{
    // Top-Left as anchor
    public struct RectInt : IEquatable<RectInt>
    {
        public struct PositionEnumerator : IEnumerable<Vector2Int>
        {
            private Vector2Int m_Min;

            private Vector2Int m_Max;

            public PositionEnumerator(Vector2Int min, Vector2Int max)
            {
                m_Min = min;
                m_Max = max;
            }

            public IEnumerator<Vector2Int> GetEnumerator()
            {
                for (int y = m_Min.Y; y <= m_Max.Y; y++)
                {
                    for (int x = m_Min.X; x <= m_Max.X; x++)
                    {
                        yield return new Vector2Int(x, y);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public Vector2Int Position { get; set; }
        public Vector2Int Size { get; set; }

        public int Width { get => Size.X; }
        public int Height { get => Size.Y; }

        public Vector2Int Min
        {
            get
            {
                var min = new Vector2Int(Position);
                if (Width < 0)
                    min.X += Width;
                if (Height < 0)
                    min.Y += Height;
                return min;
            }
        }

        public Vector2Int Max
        {
            get
            {
                var min = new Vector2Int(Position);
                if (Width > 0)
                    min.X += Width;
                if (Height > 0)
                    min.Y += Height;
                return min;
            }
        }

        public Vector2Int Center
        {
            get
            {
                return Position + new Vector2Int(Width / 2, Height / 2);
            }
        }

        public RectInt(int x, int y, int width, int height)
        {
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);
        }

        public RectInt(Vector2Int pos, Vector2Int size)
        {
            Position = pos;
            Size = size;
        }

        public void SetMinMax(Vector2Int min, Vector2Int max)
        {
            Position = min;
            Size = max - min;
        }

        public bool Contains(Vector2Int point)
        {
            return point.X >= Min.X && point.X <= Max.X &&
                point.Y >= Min.Y && point.Y <= Max.Y;
        }

        public PositionEnumerator AllPositionsWithin()
        {
            return new PositionEnumerator(Min, Max);
        }

        public override bool Equals(object obj)
        {
            if (obj is RectInt)
            {
                return Equals((RectInt)obj);
            }

            return false;
        }

        public bool Equals(RectInt other)
        {
            return Max == other.Max && Min == other.Min;
        }

        public override int GetHashCode()
        {
            return (Position + Size).GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("({0}, {1}, {2}, {3})", Position.X, Position.Y, Size.X, Size.Y));
            return sb.ToString();
        }
    }
}
