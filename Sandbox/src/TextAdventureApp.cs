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
        enum MenuState
        {
            Main,
            Save
        }

        private UINode m_MainMenuNode;

        private Bitmap m_TitleBitmap;
        private List<SingleColorCanvas> m_MainOptionCanvases = new List<SingleColorCanvas>();

        private TextBox m_NewGameText;
        private TextBox m_LoadGameText;
        private TextBox m_ExitText;

        private int m_CurrMenuSelection = 0;

        private int m_MenuOptionAnchor = 2;
        private int m_MenuOptionOffset = 4;

        #endregion

        #region InGame Var
        private TextBox m_NameText;

        #endregion

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

                if (m_CurrScene == Scene.MainMenu)
                {
                    if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        m_CurrMenuSelection = (m_CurrMenuSelection - 1 < 0) ? m_MainOptionCanvases.Count - 1 : m_CurrMenuSelection - 1;
                        //AudioManager.Instance.Beep(300, 100);
                        selectMainMenuOption(m_CurrMenuSelection);
                    }
                    else if (keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        m_CurrMenuSelection = (m_CurrMenuSelection + 1 > m_MainOptionCanvases.Count - 1) ? 0 : m_CurrMenuSelection + 1;
                        //AudioManager.Instance.Beep(300, 100);
                        selectMainMenuOption(m_CurrMenuSelection);
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        confirmMainMenuOption();
                        //AudioManager.Instance.Beep(550, 100);
                    }
                }
            }
        }

        protected override void loadInitialScene()
        {
            loadMainMenu();
        }

        protected override void logicUpdate(long timeStep)
        {
            float second = timeStep / 1000.0f;

            if (m_CurrScene == Scene.MainMenu)
            {
            }
        }

        #endregion

        #region Scene

        private void loadMainMenu()
        {
            m_CurrScene = Scene.MainMenu;
            m_CurrMenuSelection = 0;
            m_MainOptionCanvases = new List<SingleColorCanvas>();

            playSpaceOddity();

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
            m_TitleBitmap = iconNode.AddUIComponent<Bitmap>();
            m_TitleBitmap.LoadFromFile("./Assets/TitleText.txt");

            #region MainMenuOption

            /// Main Menu Node
            m_MainMenuNode = UINode.Engine.Instance.CreateNode(new RectInt(60, 12, 32, 25));
            m_MainMenuNode.AddUIComponent<SingleColorCanvas>();

            ////
            var txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(2, m_MenuOptionAnchor, 28, 3), m_MainMenuNode, "Text0-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.Black);
            m_MainOptionCanvases.Add(canvas);

            var newGameNode = UINode.Engine.Instance.CreateNode(new RectInt(3, 1, 23, 1), txtCanNode, "Text");
            m_NewGameText = newGameNode.AddUIComponent<TextBox>();
            m_NewGameText.text = "NEW ADVENTURE";
            m_NewGameText.horizontalAlignment = TextBox.HorizontalAlignment.Left;
            m_NewGameText.verticalAlignment = TextBox.VerticalAlignment.Top;

            ////
            txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(2, m_MenuOptionAnchor + m_MenuOptionOffset, 28, 3), m_MainMenuNode, "Text1-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.Black);
            m_MainOptionCanvases.Add(canvas);

            var loadGameNode = UINode.Engine.Instance.CreateNode(new RectInt(3, 1, 23, 1), txtCanNode, "Text");
            m_LoadGameText = loadGameNode.AddUIComponent<TextBox>();
            m_LoadGameText.text = "LOAD ADVENTURE";
            m_LoadGameText.horizontalAlignment = TextBox.HorizontalAlignment.Left;
            m_LoadGameText.verticalAlignment = TextBox.VerticalAlignment.Top;

            ////
            txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(2, m_MenuOptionAnchor + m_MenuOptionOffset * 2, 28, 3), m_MainMenuNode, "Text2-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.Black);
            m_MainOptionCanvases.Add(canvas);

            var exitNode = UINode.Engine.Instance.CreateNode(new RectInt(3, 1, 23, 1), txtCanNode, "Text");
            m_ExitText = exitNode.AddUIComponent<TextBox>();
            m_ExitText.text = "EXIT";
            m_ExitText.horizontalAlignment = TextBox.HorizontalAlignment.Left;
            m_ExitText.verticalAlignment = TextBox.VerticalAlignment.Top;

            //
            txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(1, m_MenuOptionAnchor + 11, 30, 3), m_MainMenuNode, "Hint-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();

            var hintNode = UINode.Engine.Instance.CreateNode(new RectInt(3, 1, 23, 1), txtCanNode, "Text");
            var hintTxt = hintNode.AddUIComponent<TextBox>();
            hintTxt.text = "arrows: select\nenter: confirm";
            hintTxt.horizontalAlignment = TextBox.HorizontalAlignment.Center;
            hintTxt.verticalAlignment = TextBox.VerticalAlignment.Top;

            //
            txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(1, m_MenuOptionAnchor + 14, 30, 3), m_MainMenuNode, "Hint-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkRed);

            hintNode = UINode.Engine.Instance.CreateNode(new RectInt(3, 1, 23, 1), txtCanNode, "Text");
            hintTxt = hintNode.AddUIComponent<TextBox>();
            hintTxt.text = "MUSIC - SPACE ODDITY";
            hintTxt.horizontalAlignment = TextBox.HorizontalAlignment.Center;
            hintTxt.verticalAlignment = TextBox.VerticalAlignment.Top;
            #endregion



            // Final setup
            selectMainMenuOption(m_CurrMenuSelection);
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
            m_NameText = textNode.AddUIComponent<TextBox>();
            m_NameText.text = string.Empty;
            m_NameText.horizontalAlignment = TextBox.HorizontalAlignment.Left;
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


            #endregion

            AudioManager.Instance.Begin();
        }

        private void selectMainMenuOption(int id)
        {
            for (int i = 0; i < m_MainOptionCanvases.Count; i++)
            {
                var canvas = m_MainOptionCanvases[i];
                if (i == id)
                {
                    var pos = new Vector2Int(1, m_MenuOptionAnchor + m_MenuOptionOffset * i);
                    canvas.Node.SetPosition(pos);
                    canvas.CanvasPixelColor = new PixelColor(ConsoleColor.White, ConsoleColor.Black);
                }
                else
                {
                    var pos = new Vector2Int(2, m_MenuOptionAnchor + m_MenuOptionOffset * i);
                    canvas.Node.SetPosition(pos);
                    canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.Black);
                }
            }
        }

        private void confirmMainMenuOption()
        {
            switch (m_CurrMenuSelection)
            {
                case 0:
                    loadScene(loadInGame);
                    break;
                case 1:
                    loadScene(loadInGame);
                    break;
                case 2:
                    IsRunning = false;
                    break;
            }
        }
    }
}
