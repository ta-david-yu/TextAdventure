using System;

namespace DYTA
{
    class Application
    {
        static void Main(string[] args)
        {
            var app = new Application();
            app.run();
        }

        void run()
        {
            Render.SingleColorCanvas canvas1 = new Render.SingleColorCanvas(new Math.RectInt(1, 1, 20, 20), new Render.PixelColor(ConsoleColor.Red, ConsoleColor.White));
            canvas1.Render();
            System.Threading.Thread.Sleep(1000);

            Render.MultiColorCanvas canvas2 = new Render.MultiColorCanvas(new Math.RectInt(1, 1, 20, 20), new Render.PixelColor(ConsoleColor.Yellow, ConsoleColor.White));
            canvas2.Render();
        }
    }
}
