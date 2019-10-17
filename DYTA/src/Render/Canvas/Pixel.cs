using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DYTA.Render
{
    public class PixelColor : IEquatable<PixelColor>, IComparable<PixelColor>
    {
        public static PixelColor DefaultColor { get { return new PixelColor(ConsoleColor.Black, ConsoleColor.White); }  }

        public ConsoleColor BackgroundColor { get; private set; } = ConsoleColor.Black;
        public ConsoleColor ForegroundColor { get; private set; } = ConsoleColor.White;

        public PixelColor()
        {
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
        }

        public PixelColor(ConsoleColor bg, ConsoleColor fg)
        {
            BackgroundColor = bg;
            ForegroundColor = fg;
        }

        public static bool operator ==(PixelColor lhs, PixelColor rhs)
        {
            return (lhs.BackgroundColor == rhs.BackgroundColor) && (lhs.ForegroundColor == rhs.ForegroundColor);
        }

        public static bool operator !=(PixelColor lhs, PixelColor rhs)
        {
            return (lhs.BackgroundColor != rhs.BackgroundColor) || (lhs.ForegroundColor != rhs.ForegroundColor);
        }

        public bool Equals([AllowNull] PixelColor other)
        {
            return this == other;
        }

        public int CompareTo([AllowNull] PixelColor other)
        {
            int ret = ForegroundColor.CompareTo(other.ForegroundColor);
            if (ret == 0)
            {
                ret = BackgroundColor.CompareTo(other.BackgroundColor);
            }
            return ret;
        }

        public override bool Equals(object obj)
        {
            return ((obj is PixelColor) ? (this == (PixelColor)obj) : false);
        }


        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString() => "( " + ForegroundColor + ", " + BackgroundColor + " )";
    }

    public class Pixel : IEquatable<Pixel>, IComparable<Pixel>
    {
        public char Character { get; set; }

        public PixelColor Color { get; set; }

        public Pixel(char ch, PixelColor color)
        {
            Character = ch;
            Color = color;
        }

        public bool Equals([AllowNull] Pixel other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo([AllowNull] Pixel other)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Pixel lhs, Pixel rhs)
        {
            return (lhs.Character == rhs.Character) && (lhs.Color == rhs.Color);
        }

        public static bool operator !=(Pixel lhs, Pixel rhs)
        {
            return (lhs.Character != rhs.Character) || (lhs.Color != rhs.Color);
        }

        public override bool Equals(object obj)
        {
            return ((obj is Pixel) ? (this == (Pixel)obj) : false);
        }

        public override int GetHashCode() => ToString().GetHashCode();


        public override string ToString() => "( " + Character + ", " + Color.ToString() + " )";
    }
}
