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
        private TextBox m_CreditText;

        private List<TextBox> m_MainOptionTextes = new List<TextBox>();

        private TextBox m_NewGameText;
        private TextBox m_LoadGameText;
        private TextBox m_ExitText;

        private int m_CurrMenuSelection = 0;

        private const int c_MenuOptionAnchor = 3;
        private const int c_MenuOptionOffset = 2;

        private static readonly List<string> c_OptionTexts = new List<string>() { "NEW LAUNCH", "CONTINUE", "EXIT" };

        #endregion

        #region InGame Var

        private TextBox m_DesciptionTxt;
        private TextBox m_InputFieldTxt;
        private TextBox m_PromptText;
        private UnlitBox m_CursorText;
        private TextBox m_RespondText;

        private StringBuilder m_InputString = new StringBuilder("");
        private float m_CursorFlickringTimer = 0;

        private const float c_CursorFlickringDuration = 0.4f;
        private const int c_InputFieldMaxLength = 58;
        private static readonly Vector2Int c_CursorAnchor = new Vector2Int(5, 2);

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
                        m_CurrMenuSelection = (m_CurrMenuSelection - 1 < 0) ? m_MainOptionTextes.Count - 1 : m_CurrMenuSelection - 1;
                        //AudioManager.Instance.Beep(300, 100);
                        selectMainMenuOption(m_CurrMenuSelection);
                    }
                    else if (keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        m_CurrMenuSelection = (m_CurrMenuSelection + 1 > m_MainOptionTextes.Count - 1) ? 0 : m_CurrMenuSelection + 1;
                        //AudioManager.Instance.Beep(300, 100);
                        selectMainMenuOption(m_CurrMenuSelection);
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        confirmMainMenuOption();
                        //AudioManager.Instance.Beep(550, 100);
                    }
                }
                else if (m_CurrScene == Scene.InGame)
                {
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (m_InputString.Length > 0)
                        {
                            m_InputString.Length--;
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        confirmInput(m_InputString.ToString());
                    }
                    else
                    {
                        if (m_InputString.Length < c_InputFieldMaxLength)
                            m_InputString.Append(keyInfo.KeyChar);
                    }
                    m_InputFieldTxt.text = m_InputString.ToString();
                    m_CursorText.Node.SetPosition(c_CursorAnchor + new Vector2Int(m_InputString.Length, 0));
                }
            }
        }

        protected override void loadInitialScene()
        {
            loadMainMenu();
        }

        protected override void update(long timeStep)
        {
            float second = timeStep / 1000.0f;

            if (m_CurrScene == Scene.MainMenu)
            {
            }
            else if (m_CurrScene == Scene.InGame)
            {
                // Cursor Update
                if (m_InputString.Length >= c_InputFieldMaxLength)
                {
                    m_CursorText.Node.IsActive = false;
                }
                else
                {
                    m_CursorFlickringTimer += second;
                    if (m_CursorFlickringTimer > c_CursorFlickringDuration)
                    {
                        m_CursorFlickringTimer = 0;
                        m_CursorText.Node.IsActive = !m_CursorText.Node.IsActive;
                    }
                }
            }
        }

        protected override void postRenderUpdate(long timeStep)
        {
            //Console.SetCursorPosition
        }

        #endregion

        #region Scene

        private void loadMainMenu()
        {
            m_CurrScene = Scene.MainMenu;
            m_CurrMenuSelection = 0;
            m_MainOptionTextes = new List<TextBox>();

            playSpaceOddity();

            //// static ti
            //// 
            var imgCanNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 2, 60, 35), null, "Image-Canvas");
            var imgCan = imgCanNode.AddUIComponent<SingleColorCanvas>();
            imgCan.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

            var imgNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 60, 35), imgCanNode, "Image");
            var img = imgNode.AddUIComponent<Bitmap>();
            img.LoadFromFile("./Assets/SpaceStation.txt");

            ///
            var iconCanNode = UINode.Engine.Instance.CreateNode(new RectInt(60, 2, 35, 10), null, "Icon-Canvas");
            var canvas = iconCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.Magenta);

            var iconNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 33, 10), iconCanNode, "Icon");
            m_TitleBitmap = iconNode.AddUIComponent<Bitmap>();
            m_TitleBitmap.LoadFromFile("./Assets/TitleText.txt");

            var creditNode = UINode.Engine.Instance.CreateNode(new RectInt(-1, 0, 33, 10), iconCanNode, "Icon");
            m_CreditText = creditNode.AddUIComponent<TextBox>();
            m_CreditText.text = " By Ta David Yu ";
            m_CreditText.horizontalAlignment = TextBox.HorizontalAlignment.Center;
            m_CreditText.verticalAlignment = TextBox.VerticalAlignment.Bottom;

            #region MainMenuOption

            /// Main Menu Node
            m_MainMenuNode = UINode.Engine.Instance.CreateNode(new RectInt(60, 12, 32, 25));
            m_MainMenuNode.AddUIComponent<SingleColorCanvas>();

            ////
            var txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(2, c_MenuOptionAnchor, 28, 3), m_MainMenuNode, "Text0-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkYellow);

            var newGameNode = UINode.Engine.Instance.CreateNode(new RectInt(1, 1, 23, 1), txtCanNode, "NEW");
            m_NewGameText = newGameNode.AddUIComponent<TextBox>();
            m_NewGameText.text = "NEW ADVENTURE";
            m_NewGameText.horizontalAlignment = TextBox.HorizontalAlignment.Center;
            m_NewGameText.verticalAlignment = TextBox.VerticalAlignment.Top;

            m_MainOptionTextes.Add(m_NewGameText);

            ////
            txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(2, c_MenuOptionAnchor + c_MenuOptionOffset, 28, 3), m_MainMenuNode, "Text1-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkYellow);

            var loadGameNode = UINode.Engine.Instance.CreateNode(new RectInt(1, 1, 23, 1), txtCanNode, "CONT");
            m_LoadGameText = loadGameNode.AddUIComponent<TextBox>();
            m_LoadGameText.text = "CONTINUE";
            m_LoadGameText.horizontalAlignment = TextBox.HorizontalAlignment.Center;
            m_LoadGameText.verticalAlignment = TextBox.VerticalAlignment.Top;

            m_MainOptionTextes.Add(m_LoadGameText);

            ////
            txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(2, c_MenuOptionAnchor + c_MenuOptionOffset * 2, 28, 3), m_MainMenuNode, "Text2-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkYellow);

            var exitNode = UINode.Engine.Instance.CreateNode(new RectInt(1, 1, 23, 1), txtCanNode, "EXIT");
            m_ExitText = exitNode.AddUIComponent<TextBox>();
            m_ExitText.text = "EXIT";
            m_ExitText.horizontalAlignment = TextBox.HorizontalAlignment.Center;
            m_ExitText.verticalAlignment = TextBox.VerticalAlignment.Top;

            m_MainOptionTextes.Add(m_ExitText);

            // - Hint
            txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(1, c_MenuOptionAnchor + 9, 30, 3), m_MainMenuNode, "Hint-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkRed);

            var hintNode = UINode.Engine.Instance.CreateNode(new RectInt(2, 1, 23, 1), txtCanNode, "HINT");
            var hintTxt = hintNode.AddUIComponent<TextBox>();
            hintTxt.text = "arrows-select\nenter-confirm";
            hintTxt.horizontalAlignment = TextBox.HorizontalAlignment.Center;
            hintTxt.verticalAlignment = TextBox.VerticalAlignment.Top;

            //
            txtCanNode = UINode.Engine.Instance.CreateNode(new RectInt(1, c_MenuOptionAnchor + 12, 30, 3), m_MainMenuNode, "Hint-Canvas");
            canvas = txtCanNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkRed);

            hintNode = UINode.Engine.Instance.CreateNode(new RectInt(2, 1, 23, 1), txtCanNode, "MUSIC");
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
            //DYTA.Dialogue.DialogueSystem.Instance.debug();

            DYTA.Dialogue.DialogueSystem.Instance.LoadScriptsFromFile("./test.json");

            m_CurrScene = Scene.InGame;

            m_InputString = new StringBuilder("");

            //// Location
            var locationNode = UINode.Engine.Instance.CreateNode(new RectInt(2, 2, 22, 30), null, "LocationCanvas");
            var locationCanvas = locationNode.AddUIComponent<SingleColorCanvas>();
            locationCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.White);

            var locationLayoutNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 22, 30), locationNode, "Bitmap");
            var locationLayout = locationLayoutNode.AddUIComponent<Bitmap>();
            locationLayout.LoadFromFile("./Assets/LocationLayout.txt", Bitmap.DrawType.Sliced);

            var locationTitle = locationLayoutNode.AddUIComponent<TextBox>();
            locationTitle.horizontalAlignment = TextBox.HorizontalAlignment.Center;
            locationTitle.verticalAlignment = TextBox.VerticalAlignment.Top;
            locationTitle.text = "[ LOCATION ]";

            //// Description
            var descriptionNode = UINode.Engine.Instance.CreateNode(new RectInt(26, 2, 67, 24), null, "Descr-Canvas");
            var descriptionCanvas = descriptionNode.AddUIComponent<SingleColorCanvas>();
            descriptionCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.White);

            // 
            var descriptionLayoutNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 67, 25), descriptionNode, "Layout-Canvas");
            var descriptionLayout = descriptionLayoutNode.AddUIComponent<Bitmap>();
            descriptionLayout.LoadFromFile("./Assets/LocationLayout.txt", Bitmap.DrawType.Sliced);

            var descriptionTitleNode = descriptionLayoutNode.AddUIComponent<TextBox>();
            descriptionTitleNode.horizontalAlignment = TextBox.HorizontalAlignment.Left;
            descriptionTitleNode.verticalAlignment = TextBox.VerticalAlignment.Top;
            descriptionTitleNode.text = "+---[ DESCRIPTION ]";


            //
            var unlitBoxNode = UINode.Engine.Instance.CreateNode(new RectInt(1, 1, 9, 1), descriptionNode, "UnlitBox");
            var unlitBox = unlitBoxNode.AddUIComponent<UnlitBox>();

            //
            var descriptionTextNode = UINode.Engine.Instance.CreateNode(new RectInt(1, 2, 65, 1), descriptionNode, "Text");
            m_DesciptionTxt = descriptionTextNode.AddUIComponent<TextBox>();
            m_DesciptionTxt.text = string.Empty;
            m_DesciptionTxt.horizontalAlignment = TextBox.HorizontalAlignment.Center;


            //// InputField
            var inputFieldNode = UINode.Engine.Instance.CreateNode(new RectInt(26, 24, 67, 8), null, "Input-Canvas");
            var inputFieldCanvas = inputFieldNode.AddUIComponent<SingleColorCanvas>();
            inputFieldCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.Yellow);

            // 
            var inputLayoutNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 67, 8), inputFieldNode, "Layout-Canvas");
            var inputLayout = inputLayoutNode.AddUIComponent<Bitmap>();
            inputLayout.LoadFromFile("./Assets/LocationLayout.txt", Bitmap.DrawType.Sliced);

            var inpnutTitleNode = inputLayoutNode.AddUIComponent<TextBox>();
            inpnutTitleNode.horizontalAlignment = TextBox.HorizontalAlignment.Left;
            inpnutTitleNode.verticalAlignment = TextBox.VerticalAlignment.Top;
            inpnutTitleNode.text = "+---[ ACTION ]";

            // 
            var inputFieldTxtNode = UINode.Engine.Instance.CreateNode(new RectInt(5, 2, 64, 1), inputFieldNode, "Text");
            m_InputFieldTxt = inputFieldTxtNode.AddUIComponent<TextBox>();
            m_InputFieldTxt.text = string.Empty;
            m_InputFieldTxt.horizontalAlignment = TextBox.HorizontalAlignment.Left;

            var inputFieldPromptNode = UINode.Engine.Instance.CreateNode(new RectInt(3, 2, 1, 1), inputFieldNode, "Prompt");
            m_PromptText = inputFieldPromptNode.AddUIComponent<TextBox>();
            m_PromptText.text = ">";
            m_PromptText.horizontalAlignment = TextBox.HorizontalAlignment.Left;

            var inputFieldCursorNode = UINode.Engine.Instance.CreateNode(new RectInt(c_CursorAnchor, Vector2Int.One), inputFieldNode, "Cursor");
            m_CursorText = inputFieldCursorNode.AddUIComponent<UnlitBox>();
            m_CursorText.UnlitCharacter = '_';

            //// Respond
            var respondCanvasNode = UINode.Engine.Instance.CreateNode(new RectInt(3, 4, 63, 2), inputFieldNode, "Output-Canvas");
            var outputCanvas = respondCanvasNode.AddUIComponent<SingleColorCanvas>();
            outputCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.White);

            var respondTxtNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 62, 2), respondCanvasNode, "Output");
            m_RespondText = respondTxtNode.AddUIComponent<TextBox>();
            m_RespondText.text = "This is the respond of your action, be careful about it.\nIt can overflow";
            m_RespondText.horizontalAlignment = TextBox.HorizontalAlignment.Left;
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

        #region MainMenu func
        private void selectMainMenuOption(int id)
        {
            for (int i = 0; i < m_MainOptionTextes.Count; i++)
            {
                var txt = m_MainOptionTextes[i];
                if (i == id)
                {
                    //var pos = new Vector2Int(1, c_MenuOptionAnchor + c_MenuOptionOffset * i);
                    //canvas.Node.SetPosition(pos);
                    txt.text = string.Format("> {0} <", c_OptionTexts[i]);
                    (txt.Node.ParentCanvas as SingleColorCanvas).CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.Yellow);
                }
                else
                {
                    //var pos = new Vector2Int(2, c_MenuOptionAnchor + c_MenuOptionOffset * i);
                    //canvas.Node.SetPosition(pos);
                    txt.text = string.Format("{0}", c_OptionTexts[i]);
                    (txt.Node.ParentCanvas as SingleColorCanvas).CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkYellow);
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
        #endregion

        #region InGame func
        private void confirmInput(string input)
        {
            m_InputString.Length = 0;
        }


        #endregion
    }
}
