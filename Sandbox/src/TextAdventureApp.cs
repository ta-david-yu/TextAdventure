using System;
using System.Collections.Generic;
using System.Text;

using DYTA;
using DYTA.Render;
using DYTA.Math;
using DYTA.Audio;

namespace Sandbox
{
    public class TextAdventureApp : ApplicationBase
    {
        TextBox m_NameTxt;

        protected override void handleOnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                IsRunning = false;
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

        protected override void initialSetup()
        {
            playBgm();

            UINode.Engine.CreateSingleton(new RectInt(0, 0, 95, 34), PixelColor.DefaultColor);
            UINode.Engine.Instance.RootCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

            ////
            var bitmapNode = UINode.Engine.Instance.CreateNode(new RectInt(2, 2, 22, 30), null, "Bitmap");
            var bitmap = bitmapNode.AddUIComponent<Bitmap>();
            bitmap.LoadFromFile("./Assets/ShuttleScene.txt");

            //// 
            var descriptionNode = UINode.Engine.Instance.CreateNode(new RectInt(26, 2, 67, 30), null, "Descr-Canvas");
            var canvas = descriptionNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.Yellow);

            //
            var boxNode = UINode.Engine.Instance.CreateNode(new RectInt(1, 1, 9, 1), descriptionNode, "UnlitBox");
            var box = boxNode.AddUIComponent<UnlitBox>();

            //
            var textNode = UINode.Engine.Instance.CreateNode(new RectInt(1, 2, 9, 1), descriptionNode, "Text");
            m_NameTxt = textNode.AddUIComponent<TextBox>();
            m_NameTxt.text = string.Empty;
            m_NameTxt.horizontalAlignment = TextBox.HorizontalAlignment.Left;
        }

        protected override void logicUpdate(long timeStep)
        {

        }

        private void playBgm()
        {
            #region Audio

            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(523, 600); // DO
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(587, 600); // RE
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(494, 600); // SI

            AudioManager.Instance.Sleep(1200);

            AudioManager.Instance.Beep(440, 800); // LA
            AudioManager.Instance.Beep(392, 800); // SO
            AudioManager.Instance.Beep(523, 800); // DO

            AudioManager.Instance.Sleep(100);

            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(523, 600); // DO 
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(587, 600); // RE
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(494, 600); // SI

            AudioManager.Instance.Sleep(4000);

            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(587, 400); // RE
            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(587, 400); // RE
            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(587, 400); // RE
            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(587, 400); // RE
            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(587, 200); // RE
            AudioManager.Instance.Beep(587, 1200); // RE

            AudioManager.Instance.Sleep(800);

            // Elevate
            int initial = 300;
            int increment = 25;
            AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.Beep(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.Beep(initial, 3200); // RE

            AudioManager.Instance.Sleep(800);

            AudioManager.Instance.Beep(392, 200); // SO
            AudioManager.Instance.Beep(392, 600); // SO
            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(587, 400); // RE
            AudioManager.Instance.Beep(698, 400); // FA
            AudioManager.Instance.Beep(659, 400); // MI
            AudioManager.Instance.Beep(587, 400); // RE
            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(494, 600); // SI

            AudioManager.Instance.Sleep(800);

            AudioManager.Instance.Beep(494, 400); // SI
            AudioManager.Instance.Beep(659, 400); // MI
            AudioManager.Instance.Beep(587, 400); // RE
            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(494, 400); // SI
            AudioManager.Instance.Beep(523, 600); // DO
            AudioManager.Instance.Beep(587, 200); // RE
            AudioManager.Instance.Beep(440, 600); // LA

            // .. and the papers want to know whose shirts you wear

            AudioManager.Instance.Begin();
            #endregion
        }
    }
}
