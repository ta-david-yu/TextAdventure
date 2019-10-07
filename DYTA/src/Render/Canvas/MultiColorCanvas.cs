using System;
using System.Collections.Generic;
using System.Text;

using DYTA.Math;

namespace DYTA.Render
{
    public class MultiColorCanvas : Canvas
    {
        private Utility.DenseArray<CanvasPixel> m_PixelBuffer;

        private PixelColor m_ClearPixelColor;

        public MultiColorCanvas(RectInt rect, PixelColor clearColor)
        {
            CanvasBounds = rect;
            m_PixelBuffer = new Utility.DenseArray<CanvasPixel>(rect.Width, rect.Height);
            m_ClearPixelColor = clearColor;

            ResetBuffer();
        }

        public override void Render()
        {
            var bgCol = Console.BackgroundColor;
            var frCol = Console.ForegroundColor;

            for (int y = 0; y < CanvasBounds.Height; y++)
            {
                Console.SetCursorPosition(CanvasBounds.Min.X + 0, CanvasBounds.Min.Y + y);
                for (int x = 0; x < CanvasBounds.Width; x++)
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
            foreach (var pt in CanvasBounds.AllPositionsWithin)
            {
                m_PixelBuffer[pt.X - CanvasBounds.Min.X, pt.Y - CanvasBounds.Min.Y] =
                    new CanvasPixel(' ', m_ClearPixelColor.BackgroundColor, m_ClearPixelColor.ForegroundColor);
            }
        }
    }
}
