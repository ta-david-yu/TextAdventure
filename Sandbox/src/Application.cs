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
            UINode.Engine.Instance.RootCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

            ////
            var bitmapNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(2, 2, 22, 30), null, "Bitmap");
            var bitmap = bitmapNode.AddUIComponent<Bitmap>();
            bitmap.LoadFromFile("./Assets/ShuttleScene.txt");

            //// 
            var descriptionNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(26, 2, 67, 30), null, "Descr-Canvas");
            var canvas = descriptionNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.Yellow);

            //
            var boxNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(1, 1, 9, 1), descriptionNode, "UnlitBox");
            var box = boxNode.AddUIComponent<UnlitBox>();

            //
            var textNode = UINode.Engine.Instance.CreateNode(new Math.RectInt(1, 2, 9, 1), descriptionNode, "Text");
            m_NameTxt = textNode.AddUIComponent<TextBox>();
            m_NameTxt.text = "";//string.Empty;
            m_NameTxt.horizontalAlignment = TextBox.HorizontalAlignment.Left;
        }

        void registerGlobalEvent()
        {
            Input.KeyboardListener.Instance.OnKeyPressed += handleOnKeyPressed;
        }

        void run()
        {
            long minimumStepPerFrame = 20; // TODO: 40

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            long timeStep = minimumStepPerFrame;

            long frameCounter = 0;

            m_IsRunning = true;
            while (true)
            {
                string frameInfo = string.Format("FRAME: {0, -5}- TIMESTEP: {1, -5}", ++frameCounter, timeStep);
                FrameLogger.Log(frameInfo);

                update(timeStep);

                UINode.Engine.Instance.PreRenderNodes();
                UINode.Engine.Instance.RenderNodes();

                FrameLogger.Update();

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

                /*
                Console.SetCursorPosition(0, UINode.Engine.Instance.RootNode.Bounds.Size.Y);
                foreach (var node in UINode.Engine.Instance.NodeTreeTraverse)
                {
                    var info = string.Empty;
                    info += string.Format("{0,2} : {1,-15} ", node.InstanceId, node.Name);
                    info += (node.IsActive) ? "O" : "X";
                    info += (node.Canvas != null) ? " C " : "   ";
                    info += (node.Canvas != null && node.Canvas.IsDirty) ? " D " : "   ";
                    info += string.Format("{0,2} {1,-15} ", node.ParentCanvas.Node.InstanceId, node.ParentCanvas.Node.Name);
                    Console.WriteLine(info);
                }
                */
                //Console.ReadLine();
            }

            stopwatch.Stop();
        }

        void update(long timeStep)
        {
            // TODO: logic update
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
                    {
                        m_NameTxt.text = m_NameTxt.text.Remove(m_NameTxt.text.Length - 1);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    m_NameTxt.text += '\n';
                }
                else
                {
                    m_NameTxt.text += keyInfo.KeyChar;
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
