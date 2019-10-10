using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using DYTA.Render;

namespace DYTA
{
    class Application
    {
        private bool m_IsRunning = false;

        TextBox m_DebugTxt;
        TextBox m_NameTxt;

        static void Main(string[] args)
        {
            var app = new Application();

            app.registerGlobalEvent();

            app.playBgm();
            app.setupMainMenu();

            app.run();

            Console.ReadKey();
        }

        void setupMainMenu()
        {
            UINode.Engine.CreateSingleton(new Math.RectInt(0, 0, 95, 34), PixelColor.DefaultColor);

            var rootNode = UINode.Engine.Instance.RootNode;
            var rootCanvas = rootNode.GetUIComponent<SingleColorCanvas>();
            rootCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

            ////
            var bitmapNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(2, 2, 22, 30));
            var bitmap = bitmapNode.AddUIComponent<Bitmap>();
            bitmap.LoadFromFile("./Assets/ShuttleScene.txt");

            //// 
            var textNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(26, 2, 67, 30));
            var canvas = textNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.Yellow);

            //
            var testBoxNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(1, 1, 9, 1), textNode);
            var box = testBoxNode.AddUIComponent<UnlitBox>();

            //
            var testBoxNode2 = UINode.Engine.Instance.CreateNode(new Math.RectInt(1, 2, 9, 1), textNode);
            m_NameTxt = testBoxNode2.AddUIComponent<TextBox>();
            m_NameTxt.text = new System.Text.StringBuilder("");
            m_NameTxt.horizontalAlignment = TextBox.HorizontalAlignment.Left;

            //// 
            var debugNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(0, 43, 65, 1), rootNode);
            m_DebugTxt = debugNode.AddUIComponent<TextBox>();
            m_DebugTxt.horizontalAlignment = TextBox.HorizontalAlignment.Center;
        }

        void registerGlobalEvent()
        {
            Input.KeyboardListener.Instance.OnKeyPressed += handleOnKeyPressed;
        }

        void run()
        {
            long minimumStepPerFrame = 40;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            long timeStep = minimumStepPerFrame;

            m_IsRunning = true;
            while (true)
            {
                update(timeStep);

                m_DebugTxt.text.Length = 0;
                m_DebugTxt.text.Append("FRAME ELAPSE - " + timeStep);

                UINode.Engine.Instance.PreRenderNodes();
                UINode.Engine.Instance.RenderNodes();

                timeStep = stopwatch.ElapsedMilliseconds;

                if (timeStep < minimumStepPerFrame)
                {
                    int sleep = (int)(minimumStepPerFrame - timeStep);
                    Thread.Sleep(sleep);
                    timeStep = minimumStepPerFrame;
                }

                stopwatch.Restart();

                if (!m_IsRunning)
                {
                    break;
                }
            }

            stopwatch.Stop();
        }

        void update(long timeStep)
        {
            // TODO
            Input.KeyboardListener.Instance.QueryInput();
        }

        void handleOnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                m_IsRunning = false;
            }
            else
            {
                // TODO: Event System
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (m_NameTxt.text.Length > 0)
                        m_NameTxt.text.Length--;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    m_NameTxt.text.Append('\n');
                }
                else
                {
                    m_NameTxt.text.Append(keyInfo.KeyChar);
                }
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
