using System;
using System.Collections.Generic;
using System.Text;

namespace DYTA.Render
{
    public class PixelColor
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
    }

    public class CanvasPixel
    {
        public char Character { get; set; }

        public PixelColor PixelColor { get; set; }

        public CanvasPixel(char ch, ConsoleColor bg, ConsoleColor fg)
        {
            Character = ch;
            PixelColor = new PixelColor(bg, fg);
        }
    }
}
