using System;
using DYTA;

namespace NSShaft
{
    class Program
    {
        const int c_Width = 70;
        const int c_Height = 50;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            try
            {
                Console.SetWindowSize(c_Width, c_Height + 20);
            }
            catch
            {
                Console.SetWindowSize(Console.LargestWindowWidth < c_Width? Console.LargestWindowWidth : c_Width, Console.LargestWindowHeight);
            }

            var app = new NSShaftApplication(new DYTA.Math.Vector2Int(c_Width, c_Height), DYTA.Render.PixelColor.DefaultColor);

            app.Run();

            Console.CursorVisible = true;
        }
    }
}
