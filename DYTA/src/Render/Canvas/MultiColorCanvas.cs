using System;
using System.Collections.Generic;
using System.Text;

using DYTA.Math;

namespace DYTA.Render
{
    public class MultiColorCanvas : Canvas
    {
        private Utility.DenseArray<CanvasPixel> m_PixelBuffer;

        public PixelColor ClearPixelColor { get; set; }

        public MultiColorCanvas()
        {
            ClearPixelColor = new PixelColor();
        }

        public override void Render()
        {
            var bgCol = Console.BackgroundColor;
            var frCol = Console.ForegroundColor;

            for (int y = 0; y < Node.Bounds.Height; y++)
            {
                Console.SetCursorPosition(Node.Bounds.Min.X + 0, Node.Bounds.Min.Y + y);
                for (int x = 0; x < Node.Bounds.Width; x++)
                {
                    var pixel = m_PixelBuffer[x, y];
                    Console.BackgroundColor = pixel.PixelColor.BackgroundColor;
                    Console.ForegroundColor = pixel.PixelColor.ForegroundColor;
                    Console.Write(pixel.Character);
                }
            }

            Console.BackgroundColor = bgCol;
            Console.ForegroundColor = frCol;
        }

        public override void ResetBuffer()
        {
            m_PixelBuffer = new Utility.DenseArray<CanvasPixel>(Node.Bounds.Width, Node.Bounds.Height);

            foreach (var pt in Node.Bounds.AllPositionsWithin)
            {
                m_PixelBuffer[pt.X - Node.Bounds.Min.X, pt.Y - Node.Bounds.Min.Y] =
                    new CanvasPixel(' ', ClearPixelColor.BackgroundColor, ClearPixelColor.ForegroundColor);
            }
        }
    }
}
