using System;
using DYTA;

namespace Snake
{
    class Program
    {
        const int c_Width = 60;
        const int c_Height = 55;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            try
            {
                Console.SetWindowSize(c_Width + 10, c_Height + 15);
            }
            catch
            {
                Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            }

            var app = new SnakeApplication(new DYTA.Math.Vector2Int(c_Width, c_Height), DYTA.Render.PixelColor.DefaultColor);

            app.Run();

            Console.CursorVisible = true;
        }
    }
}
