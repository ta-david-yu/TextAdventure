using System;
using System.Collections.Generic;
using System.Text;

namespace DYTA.Render
{
    public class PixelColor
    {
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;

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
