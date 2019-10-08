using System;
using System.Collections.Generic;
using System.Threading;
using DYTA.Render;

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
            UINode.Engine.CreateSingleton(new Math.RectInt(0, 0, 63, 42), PixelColor.DefaultColor);

            var rootNode = UINode.Engine.Instance.RootNode;
            var rootCanvas = rootNode.GetUIComponent<SingleColorCanvas>();
            rootCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkYellow, ConsoleColor.Gray);

            var bitmapNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(1, 1, 20, 40));
            var canvas = bitmapNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);
            canvas.ResetBuffer();
            
            var textNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(22, 1, 40, 40));
            canvas = textNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);
            canvas.ResetBuffer();

            UINode.Engine.Instance.PreRenderNodes();
            UINode.Engine.Instance.RenderNodes();

            /*
            #region SMB
            Console.Beep(659, 125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(523, 125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(784, 125); Thread.Sleep(375); Console.Beep(392, 125); Thread.Sleep(375); Console.Beep(523, 125); Thread.Sleep(250); Console.Beep(392, 125); Thread.Sleep(250); Console.Beep(330, 125); Thread.Sleep(250); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(494, 125); Thread.Sleep(125); Console.Beep(466, 125); Thread.Sleep(42); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(392, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(880, 125); Thread.Sleep(125); Console.Beep(698, 125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(587, 125); Console.Beep(494, 125); Thread.Sleep(125); Console.Beep(523, 125); Thread.Sleep(250); Console.Beep(392, 125); Thread.Sleep(250); Console.Beep(330, 125); Thread.Sleep(250); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(494, 125); Thread.Sleep(125); Console.Beep(466, 125); Thread.Sleep(42); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(392, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(880, 125); Thread.Sleep(125); Console.Beep(698, 125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(587, 125); Console.Beep(494, 125); Thread.Sleep(375); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(415, 125); Console.Beep(440, 125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(440, 125); Console.Beep(523, 125); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(698, 125); Thread.Sleep(125); Console.Beep(698, 125); Console.Beep(698, 125); Thread.Sleep(625); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(415, 125); Console.Beep(440, 125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(440, 125); Console.Beep(523, 125); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(622, 125); Thread.Sleep(250); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(523, 125); Thread.Sleep(1125); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(415, 125); Console.Beep(440, 125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(440, 125); Console.Beep(523, 125); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(698, 125); Thread.Sleep(125); Console.Beep(698, 125); Console.Beep(698, 125); Thread.Sleep(625); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(415, 125); Console.Beep(440, 125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(440, 125); Console.Beep(523, 125); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(622, 125); Thread.Sleep(250); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(523, 125);
            #endregion
            */
            Console.Beep(523, 400); // DO
            Console.Beep(523, 200); // DO
            Console.Beep(523, 600); // DO
            Console.Beep(523, 200); // DO
            Console.Beep(587, 600); // RE
            Console.Beep(523, 200); // DO
            Console.Beep(494, 600); // SI

            Thread.Sleep(1200);

            Console.Beep(440, 800); // LA
            Console.Beep(392, 800); // SO
            Console.Beep(523, 800); // DO

            Thread.Sleep(100);

            Console.Beep(523, 400); // DO
            Console.Beep(523, 200); // DO
            Console.Beep(523, 600); // DO 
            Console.Beep(523, 200); // DO
            Console.Beep(587, 600); // RE
            Console.Beep(523, 200); // DO
            Console.Beep(494, 600); // SI

            Thread.Sleep(3600);

            Console.Beep(523, 400); // DO
            Console.Beep(587, 400); // RE
            Console.Beep(523, 400); // DO
            Console.Beep(587, 400); // RE
            Console.Beep(523, 400); // DO
            Console.Beep(587, 400); // RE
            Console.Beep(523, 400); // DO
            Console.Beep(587, 400); // RE
            Console.Beep(523, 400); // DO
            Console.Beep(587, 200); // RE
            Console.Beep(587, 600); // RE
        }
    }
}
