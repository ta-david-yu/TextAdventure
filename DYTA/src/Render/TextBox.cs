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

        private string m_Text = string.Empty;
        public string text
        {
            get { return m_Text; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(m_Text))
                    {
                        return;
                    }
                    m_Text = "";
                    Node.SetDirty();
                }
                else if (m_Text != value)
                {
                    m_Text = value;
                    Node.SetDirty();
                }
            }
        }

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
                        var halfTextCount = (line.Length % 2 == 0)? line.Length / 2 : (line.Length - 1) / 2;
                        anchorX = halfWidth - halfTextCount;
                        break;
                    case HorizontalAlignment.Right:
                        anchorX = Node.Bounds.Width - line.Length;
                        break;
                }

                for (int j = 0; j < line.Length; j++)
                {
                    var offset = new Math.Vector2Int(anchorX + j, anchorY + i);
                    Node.ParentCanvas.SetPixel(line[j], Node.Bounds.Position + offset);
                }
            }
        }
    }
}
