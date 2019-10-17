using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DYTA.Math;

namespace DYTA.Render
{
    public class SingleColorCanvas : Canvas
    {
        private PixelColor m_CanvasPixelColor = new PixelColor();
        public PixelColor CanvasPixelColor 
        { 
            get
            {
                return m_CanvasPixelColor;
            }

            set
            {
                m_CanvasPixelColor = value;
                IsDirty = true;
                ResetBuffer();
            }
        }

        private List<StringBuilder> m_CharacterBuffer;

        private string m_ClearLine;

        public SingleColorCanvas()
        {
        }

        public override void OnInitializedByNode(UINode node)
        {
            base.OnInitializedByNode(node);
            ResetBuffer();
        }

        public override void PreRender()
        {
        }

        public override void RenderToBuffer(Pixel[,] buffer)
        {
            var bgCol = Console.BackgroundColor;
            var frCol = Console.ForegroundColor;

            Console.BackgroundColor = CanvasPixelColor.BackgroundColor;
            Console.ForegroundColor = CanvasPixelColor.ForegroundColor;

            var worldPos = Node.WorldAnchor;

            for (int y = 0; y < Node.Bounds.Height; y++)
            {
                for (int x = 0; x < Node.Bounds.Width; x++)
                {
                    var ch = m_CharacterBuffer[y][x];

                    int worldX = worldPos.X + Node.Bounds.Min.X + x;
                    int worldY = worldPos.Y + Node.Bounds.Min.Y + y;

                    if (worldX >= 0 && worldX < buffer.GetLength(0) && worldY >= 0 && worldY < buffer.GetLength(1))
                    {
                        buffer[worldX, worldY] = new Pixel(ch, CanvasPixelColor);
                    }

                    //Console.SetCursorPosition(worldPos.X + Node.Bounds.Min.X + x, worldPos.Y + Node.Bounds.Min.Y + y);
                    //Console.Write(m_CharacterBuffer[y][x]);
                }
                /*
                Console.SetCursorPosition(worldPos.X + Node.Bounds.Min.X + 0, worldPos.Y + Node.Bounds.Min.Y + y);
                var line = m_PixelBuffer[y];
                Console.WriteLine(line);
                */
            }


            Console.BackgroundColor = bgCol;
            Console.ForegroundColor = frCol;
        }

        public override void ResetBuffer()
        {
            if (m_ClearLine == null)
            {
                m_ClearLine = new string(' ', Node.Bounds.Width);
            }

            if (m_CharacterBuffer == null)
            {
                m_CharacterBuffer = new List<StringBuilder>();
                for (int y = 0; y < Node.Bounds.Height; y++)
                {
                    m_CharacterBuffer.Add(new StringBuilder(m_ClearLine));
                }
            }
            else
            {
                for (int y = 0; y < Node.Bounds.Height; y++)
                {
                    m_CharacterBuffer[y] = new StringBuilder(m_ClearLine);
                }
            }
        }

        public override void SetPixel(char character, Vector2Int pos)
        {
            var bounds = Node.Bounds;
            var canvasPos = bounds.Position + pos;

            bool insideCanvas = bounds.Contains(canvasPos);
            if (insideCanvas)
            {
                m_CharacterBuffer[pos.Y][pos.X] = character;
            }
        }
    }
}
