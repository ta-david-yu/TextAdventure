using System;
using DYTA;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new SnakeApplication(new DYTA.Math.RectInt(DYTA.Math.Vector2Int.Zero, new DYTA.Math.Vector2Int(95, 30)), DYTA.Render.PixelColor.DefaultColor);

            app.Run();
        }
    }
}
