using System;
using System.Collections.Generic;
using System.Text;

using DYTA.Math;

namespace DYTA.Render
{
    public class SingleColorCanvas : Canvas
    {
        private List<string> m_PixelBuffer;
        private PixelColor m_CanvasPixelColor;

        public SingleColorCanvas(RectInt rect, PixelColor monoPixelColor)
        {
            CanvasBounds = rect;
            m_PixelBuffer = new List<string>();
            m_CanvasPixelColor = monoPixelColor;

            ResetBuffer();
        }

        public override void Render()
        {
            var bgCol = Console.BackgroundColor;
            var frCol = Console.ForegroundColor;

            Console.BackgroundColor = m_CanvasPixelColor.BackgroundColor;
            Console.ForegroundColor = m_CanvasPixelColor.ForegroundColor;

            for (int y = 0; y < CanvasBounds.Height; y++)
            {
                Console.SetCursorPosition(CanvasBounds.Min.X + 0, CanvasBounds.Min.Y + y);
                var line = m_PixelBuffer[y];
                Console.WriteLine(line);
            }

            Console.BackgroundColor = bgCol;
            Console.ForegroundColor = frCol;
        }

        public override void ResetBuffer()
        {
            for (int y = 0; y < CanvasBounds.Height; y++)
            {
                var line = new string(' ', CanvasBounds.Width);
                m_PixelBuffer.Add(line);
            }
        }
    }
}
