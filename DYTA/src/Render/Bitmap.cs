using System;
using System.Collections.Generic;
using System.Text;

namespace DYTA.Render
{
    public class Bitmap : UIElement
    {
        public enum DrawType
        {
            Normal,
            Sliced
        }

        public List<StringBuilder> Data { get; private set; } 

        //public DrawType 

        public Bitmap()
        {
        }

        public void LoadFromFile(string filePath)
        {
            var lines = System.IO.File.ReadAllLines(filePath);

            Load(lines);
        }

        public void Load(string[] lines)
        {
            Data = new List<StringBuilder>();

            for (int y = 0; y < Node.Bounds.Height; y++)
            {
                if (y < lines.Length)
                {
                    var line = lines[y];
                    Data.Add(new StringBuilder(line));

                    if (line.Length < Node.Bounds.Width)
                    {
                        Data[y].Append(new string(' ', Node.Bounds.Width - line.Length));
                    }
                }
                else
                {
                    var line = new StringBuilder(new string(' ', Node.Bounds.Width));
                    Data.Add(line);
                }
            }
        }

        public override void PreRender()
        {
            foreach (var pos in Node.Bounds.AllPositionsWithin)
            {
                var dataPos = pos - Node.Bounds.Position;
                Node.ParentCanvas.SetPixel(Data[dataPos.Y][dataPos.X], pos);
            }
        }
    }
}
