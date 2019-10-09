using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using DYTA.Render;

namespace DYTA
{
    class Application
    {
        TextBox m_DebugTxt;

        static void Main(string[] args)
        {
            var app = new Application();
            app.playBgm();
            app.setupMainMenu();
            app.run();
        }

        void setupMainMenu()
        {
            UINode.Engine.CreateSingleton(new Math.RectInt(0, 0, 65, 44), PixelColor.DefaultColor);

            var rootNode = UINode.Engine.Instance.RootNode;
            var rootCanvas = rootNode.GetUIComponent<SingleColorCanvas>();
            rootCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.Gray);

            ////
            var bitmapNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(2, 2, 22, 40));
            var canvas = bitmapNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.Black);

            //// 
            var textNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(26, 2, 37, 40));
            canvas = textNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Gray, ConsoleColor.Black);
            canvas.ResetBuffer();

            //
            var testBoxNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(1, 1, 9, 1), textNode);
            var box = testBoxNode.AddUIComponent<UnlitBox>();

            //
            var testBoxNode2 = UINode.Engine.Instance.CreateNode(new Math.RectInt(1, 2, 9, 1), textNode);

            var box2 = testBoxNode2.AddUIComponent<TextBox>();
            box2.text = new System.Text.StringBuilder("DAVID");
            box2.horizontalAlignment = TextBox.HorizontalAlignment.Right;

            //// 
            var debugNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(0, 43, 65, 1), rootNode);
            m_DebugTxt = debugNode.AddUIComponent<TextBox>();
            m_DebugTxt.horizontalAlignment = TextBox.HorizontalAlignment.Left;
        }

        void run()
        {
            long minimumStepPerFrame = 200;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            long timeStep = minimumStepPerFrame;
            while (true)
            {
                UINode.Engine.Instance.PreRenderNodes();
                UINode.Engine.Instance.RenderNodes();
                m_DebugTxt.text.Clear();
                m_DebugTxt.text.Append("FRAME TIME ELAPSE - " + timeStep);
                m_DebugTxt.Node.Translate(new Math.Vector2Int(1, 0));

                // Do Something 

                timeStep = stopwatch.ElapsedMilliseconds;

                if (timeStep < minimumStepPerFrame)
                {
                    int sleep = (int)(minimumStepPerFrame - timeStep);
                    Thread.Sleep(sleep);
                    timeStep = minimumStepPerFrame;
                }

                stopwatch.Restart();
            }
        }

        void playBgm()
        {
            #region Audio

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

            Audio.AudioManager.Instance.Sleep(800);

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
        }
    }
}
