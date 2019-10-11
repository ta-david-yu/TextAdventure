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
            var app = new TextAdventureApp(new DYTA.Math.RectInt(0, 0, 95, 34), new PixelColor(ConsoleColor.DarkCyan, ConsoleColor.White));

            app.Run();
        }
    }
}
