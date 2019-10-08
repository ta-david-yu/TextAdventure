using System;
using System.Collections.Generic;
using System.Text;

using DYTA.Math;

namespace DYTA.Render
{
    public class SingleColorCanvas : Canvas
    {
        public PixelColor CanvasPixelColor { get; set; }

        private List<string> m_PixelBuffer;

        public SingleColorCanvas()
        {
            CanvasPixelColor = new PixelColor();
        }

        public override void Render()
        {
            var bgCol = Console.BackgroundColor;
            var frCol = Console.ForegroundColor;

            Console.BackgroundColor = CanvasPixelColor.BackgroundColor;
            Console.ForegroundColor = CanvasPixelColor.ForegroundColor;

            var worldPos = Node.WorldAnchor;

            for (int y = 0; y < Node.Bounds.Height; y++)
            {
                Console.SetCursorPosition(worldPos.X + Node.Bounds.Min.X + 0, worldPos.Y + Node.Bounds.Min.Y + y);
                var line = m_PixelBuffer[y];
                Console.WriteLine(line);
            }

            Console.BackgroundColor = bgCol;
            Console.ForegroundColor = frCol;
        }

        public override void ResetBuffer()
        {
            m_PixelBuffer = new List<string>();

            for (int y = 0; y < Node.Bounds.Height; y++)
            {
                var line = new string(' ', Node.Bounds.Width);
                m_PixelBuffer.Add(line);
            }
        }
    }
}
