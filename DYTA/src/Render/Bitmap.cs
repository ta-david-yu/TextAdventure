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

        public void LoadFromFile(string filePath, DrawType drawType = DrawType.Normal)
        {
            var lines = System.IO.File.ReadAllLines(filePath);

            Load(lines, drawType);
        }

        public void Load(string[] lines, DrawType drawType = DrawType.Normal)
        {
            Data = new List<StringBuilder>();

            if (drawType == DrawType.Normal)
            {
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
            else if (drawType == DrawType.Sliced)
            {
                for (int y = 0; y < Node.Bounds.Height; y++)
                {
                    char head = ' ';
                    char middle = ' ';
                    char end = ' ';
                    if (y == 0)
                    {
                        head = lines[0][0];
                        middle = lines[0][1];
                        end = lines[0][2];
                    }
                    else if (y == Node.Bounds.Height - 1)
                    {
                        head = lines[2][0];
                        middle = lines[2][1];
                        end = lines[2][2];
                    }
                    else
                    {
                        head = lines[1][0];
                        middle = lines[1][1];
                        end = lines[1][2];
                    }

                    Data.Add(new StringBuilder());
                    Data[y].Append(head);
                    Data[y].Append(new string(middle, Node.Bounds.Width - 2));
                    Data[y].Append(end);
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
