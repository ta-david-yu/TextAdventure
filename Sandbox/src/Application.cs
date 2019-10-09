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

            var testBoxNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(0, 0, 5, 5), bitmapNode);
            var box = testBoxNode.AddUIComponent<UnlitBox>();

            var textNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(22, 1, 40, 40));
            canvas = textNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);
            canvas.ResetBuffer();

            #region Audio
            Audio.AudioManager.Instance.Begin();

            Audio.AudioManager.Instance.Beep(523, 400); // DO
            Audio.AudioManager.Instance.Beep(523, 200); // DO
            Audio.AudioManager.Instance.Beep(523, 600); // DO
            Audio.AudioManager.Instance.Beep(523, 200); // DO
            Audio.AudioManager.Instance.Beep(587, 600); // RE
            Audio.AudioManager.Instance.Beep(523, 200); // DO
            Audio.AudioManager.Instance.Beep(494, 600); // SI

            Audio.AudioManager.Instance.Sleep(1200);

            Audio.AudioManager.Instance.Beep(440, 800); // LA
            Audio.AudioManager.Instance.Beep(392, 800); // SO
            Audio.AudioManager.Instance.Beep(523, 800); // DO

            Audio.AudioManager.Instance.Sleep(100);

            Audio.AudioManager.Instance.Beep(523, 400); // DO
            Audio.AudioManager.Instance.Beep(523, 200); // DO
            Audio.AudioManager.Instance.Beep(523, 600); // DO 
            Audio.AudioManager.Instance.Beep(523, 200); // DO
            Audio.AudioManager.Instance.Beep(587, 600); // RE
            Audio.AudioManager.Instance.Beep(523, 200); // DO
            Audio.AudioManager.Instance.Beep(494, 600); // SI

            Audio.AudioManager.Instance.Sleep(4000);

            Audio.AudioManager.Instance.Beep(523, 400); // DO
            Audio.AudioManager.Instance.Beep(587, 400); // RE
            Audio.AudioManager.Instance.Beep(523, 400); // DO
            Audio.AudioManager.Instance.Beep(587, 400); // RE
            Audio.AudioManager.Instance.Beep(523, 400); // DO
            Audio.AudioManager.Instance.Beep(587, 400); // RE
            Audio.AudioManager.Instance.Beep(523, 400); // DO
            Audio.AudioManager.Instance.Beep(587, 400); // RE
            Audio.AudioManager.Instance.Beep(523, 400); // DO
            Audio.AudioManager.Instance.Beep(587, 200); // RE
            Audio.AudioManager.Instance.Beep(587, 1200); // RE

            Thread.Sleep(800);

            // Elevate
            int initial = 300;
            int increment = 25;
            Audio.AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            Audio.AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            Audio.AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            Audio.AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            Audio.AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            Audio.AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            Audio.AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            Audio.AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            Audio.AudioManager.Instance.Beep(initial, 3200); // RE

            Audio.AudioManager.Instance.Sleep(800);

            Audio.AudioManager.Instance.Beep(392, 200); // SO
            Audio.AudioManager.Instance.Beep(392, 600); // SO
            Audio.AudioManager.Instance.Beep(523, 400); // DO
            Audio.AudioManager.Instance.Beep(587, 400); // RE
            Audio.AudioManager.Instance.Beep(698, 400); // FA
            Audio.AudioManager.Instance.Beep(659, 400); // MI
            Audio.AudioManager.Instance.Beep(587, 400); // RE
            Audio.AudioManager.Instance.Beep(523, 400); // DO
            Audio.AudioManager.Instance.Beep(494, 600); // SI

            Audio.AudioManager.Instance.Sleep(800);

            Audio.AudioManager.Instance.Beep(494, 400); // SI
            Audio.AudioManager.Instance.Beep(659, 400); // MI
            Audio.AudioManager.Instance.Beep(587, 400); // RE
            Audio.AudioManager.Instance.Beep(523, 400); // DO
            Audio.AudioManager.Instance.Beep(494, 400); // SI
            Audio.AudioManager.Instance.Beep(523, 600); // DO
            Audio.AudioManager.Instance.Beep(587, 200); // RE
            Audio.AudioManager.Instance.Beep(440, 600); // LA

            // .. and the papers want to know whose shirts you wear

            Audio.AudioManager.Instance.Begin();
            #endregion


            while (true)
            {
                UINode.Engine.Instance.PreRenderNodes();
                UINode.Engine.Instance.RenderNodes();

                var pos = testBoxNode.Bounds.Position;
                pos += new Math.Vector2Int(1, 3);
                testBoxNode.SetPosition(pos);

                Thread.Sleep(200);
            }


            Audio.AudioManager.Instance.End();
        }
    }
}
