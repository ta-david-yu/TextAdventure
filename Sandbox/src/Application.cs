using System;
using System.Collections.Generic;
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
            UINode.Engine.CreateSingleton(new Math.RectInt(0, 0, 60, 40), PixelColor.DefaultColor);

            var rootNode = UINode.Engine.Instance.RootNode;
            var rootCanvas = rootNode.GetUIComponent<SingleColorCanvas>();
            rootCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Gray, ConsoleColor.Gray);

            var bitmapNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(1, 1, 20, 38));
            var canvas = bitmapNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.White);
            canvas.ResetBuffer();
            
            var textNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(22, 1, 37, 38));
            canvas = textNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.White);
            canvas.ResetBuffer();

            UINode.Engine.Instance.PreRenderNodes();
            UINode.Engine.Instance.RenderNodes();
            while (true)
            {
                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
