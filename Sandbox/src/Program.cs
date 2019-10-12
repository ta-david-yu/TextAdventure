using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using DYTA;
using DYTA.Render;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(100, 60);

            var app = new TextAdventureApp(new DYTA.Math.RectInt(0, 0, 95, 37), new PixelColor(ConsoleColor.Black, ConsoleColor.White));

            app.Run();

            Console.CursorVisible = true;
        }
    }
}
