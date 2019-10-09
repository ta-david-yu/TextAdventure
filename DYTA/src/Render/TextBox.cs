using System;
using System.Collections.Generic;
using System.Text;

namespace DYTA.Render
{
    public class TextBox : UIElement
    {
        public enum HorizontalAlignment
        {
            Left,
            Center,
            Right
        }

        public enum VerticalAlignment
        {
            Top,
            Middle,
            Bottom
        }

        public StringBuilder text { get; set; } = new StringBuilder();

        public HorizontalAlignment horizontalAlignment { get; set; } = HorizontalAlignment.Left;
        public VerticalAlignment verticalAlignment { get; set; } = VerticalAlignment.Top;

        public TextBox()
        {

        }

        public override void PreRender()
        {
            var lines = text.ToString().Split('\n');

            int anchorY = 0;
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    anchorY = 0;
                    break;
                case VerticalAlignment.Middle:
                    var halfWidth = Node.Bounds.Height / 2;
                    var halfTextCount = lines.Length / 2;
                    anchorY = halfWidth - halfTextCount;
                    break;
                case VerticalAlignment.Bottom:
                    anchorY = Node.Bounds.Height - lines.Length;
                    break;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                int anchorX = 0;

                switch (horizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        anchorX = 0;
                        break;
                    case HorizontalAlignment.Center:
                        var halfWidth = Node.Bounds.Width / 2;
                        var halfTextCount = line.Length / 2;
                        anchorX = halfWidth - halfTextCount;
                        break;
                    case HorizontalAlignment.Right:
                        anchorX = Node.Bounds.Width - line.Length;
                        break;
                }

                for (int j = 0; j < line.Length; j++)
                {
                    var offset = new Math.Vector2Int(anchorX + j, anchorY + i);
                    MainCanvas.SetPixel(line[j], Node.Bounds.Position + offset);
                }
            }
        }
    }
}
