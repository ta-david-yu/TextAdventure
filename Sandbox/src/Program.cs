using System;

using DYTA.Render;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            try
            {
                Console.SetWindowSize(110, 53);
            }
            catch
            {
                Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            }

            var app = new TextAdventureApp(new DYTA.Math.RectInt(0, 0, 95, 37), new PixelColor(ConsoleColor.Black, ConsoleColor.White));

            app.Run();

            Console.CursorVisible = true;

            Console.ReadLine();
        }
    }
}
