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
        public enum Scene
        {
            MainMenu,
            InGame
        }

        private Scene m_CurrScene = Scene.MainMenu;

        #region MainMenu Var
        private Bitmap m_TitleBitmap;
        private TextBox m_NewGameText;
        private TextBox m_LoadGameText;
        private TextBox m_ExitText;

        #endregion

        private TextBox m_NameTxt;

        public TextAdventureApp(RectInt bounds, PixelColor color) : base(bounds, color)
        {
        }

        #region Override
        protected override void handleOnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                UINode.Engine.Instance.CleanUp();
                IsRunning = false;
            }
            else
            {
                if (keyInfo.Key == ConsoleKey.NumPad0)
                {
                    loadScene(loadMainMenu);
                }
                else if (keyInfo.Key == ConsoleKey.NumPad1)
                {
                    loadScene(loadInGame);
                }

                /*
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
                */
            }
        }

        protected override void loadInitialScene()
        {
            loadMainMenu();
        }

        protected override void logicUpdate(long timeStep)
        {
            float second = timeStep / 1000.0f;
            // TODO
            if (m_CurrScene == Scene.MainMenu)
            {
            }
        }

        #endregion

        #region Scene

        private void loadMainMenu()
        {
            m_CurrScene = Scene.MainMenu;

            playSpaceOddity();
            //// MainNode


            //// 
            var imgCanNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 60, 35), null, "Image-Canvas");
            var imgCan = imgCanNode.AddUIComponent<SingleColorCanvas>();
            imgCan.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

            var imgNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 60, 35), imgCanNode, "Image");
            var img = imgNode.AddUIComponent<Bitmap>();
            img.LoadFromFile("./Assets/SpaceStation.txt");

            ///
            var iconCanNode = UINode.Engine.Instance.CreateNode(new RectInt(60, 2, 35, 10), null, "Icon-Canvas");
            var canvas = iconCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

            var iconNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 33, 10), iconCanNode, "Icon");
            img = iconNode.AddUIComponent<Bitmap>();
            img.LoadFromFile("./Assets/TitleText.txt");


            int menuAnchor = 13;
            ////
            var txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(60, menuAnchor, 32, 3), null, "Text0-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.Black);
            
            var newGameNode = UINode.Engine.Instance.CreateNode(new RectInt(3, 1, 23, 1), txtCanNode, "Text");
            m_NewGameText = newGameNode.AddUIComponent<TextBox>();
            m_NewGameText.text = "NEW ADVENTURE";
            m_NewGameText.horizontalAlignment = TextBox.HorizontalAlignment.Left;
            m_NewGameText.verticalAlignment = TextBox.VerticalAlignment.Top;

            ////
            txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(60, menuAnchor + 4, 32, 3), null, "Text1-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.Black);

            var loadGameNode = UINode.Engine.Instance.CreateNode(new RectInt(3, 1, 23, 1), txtCanNode, "Text");
            m_LoadGameText = loadGameNode.AddUIComponent<TextBox>();
            m_LoadGameText.text = "LOAD ADVENTURE";
            m_LoadGameText.horizontalAlignment = TextBox.HorizontalAlignment.Left;
            m_LoadGameText.verticalAlignment = TextBox.VerticalAlignment.Top;

            ////
            txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(60, menuAnchor + 8, 32, 3), null, "Text2-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.Black);

            var exitNode = UINode.Engine.Instance.CreateNode(new RectInt(3, 1, 23, 1), txtCanNode, "Text");
            m_ExitText = exitNode.AddUIComponent<TextBox>();
            m_ExitText.text = "EXIT";
            m_ExitText.horizontalAlignment = TextBox.HorizontalAlignment.Left;
            m_ExitText.verticalAlignment = TextBox.VerticalAlignment.Top;
        }

        private void loadInGame()
        {
            m_CurrScene = Scene.InGame;

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

        #endregion

        private void playSpaceOddity()
        {
            #region Audio

            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(523, 600); // DO
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(587, 600); // RE
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(494, 600); // SI

            AudioManager.Instance.Delay(1200);

            AudioManager.Instance.Beep(440, 800); // LA
            AudioManager.Instance.Beep(392, 800); // SO
            AudioManager.Instance.Beep(523, 800); // DO

            AudioManager.Instance.Delay(100);

            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(523, 600); // DO 
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(587, 600); // RE
            AudioManager.Instance.Beep(523, 200); // DO
            AudioManager.Instance.Beep(494, 600); // SI

            AudioManager.Instance.Delay(4000);

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

            AudioManager.Instance.Delay(800);

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

            AudioManager.Instance.Delay(800);

            AudioManager.Instance.Beep(392, 200); // SO
            AudioManager.Instance.Beep(392, 600); // SO
            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(587, 400); // RE
            AudioManager.Instance.Beep(698, 400); // FA
            AudioManager.Instance.Beep(659, 400); // MI
            AudioManager.Instance.Beep(587, 400); // RE
            AudioManager.Instance.Beep(523, 400); // DO
            AudioManager.Instance.Beep(494, 600); // SI

            AudioManager.Instance.Delay(800);

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
