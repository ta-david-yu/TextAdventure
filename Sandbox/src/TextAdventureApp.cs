using System;
using System.Collections.Generic;
using System.Text;

using DYTA;
using DYTA.Render;
using DYTA.Math;
using DYTA.Audio;
using DYTA.Dialogue;

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

        private SingleColorCanvas m_HintBanner;
        private TextBox m_HintBannerText;

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

        private bool m_IsPointerSpanned = false;
        private float m_PointerMoveTimer = 0;

        private const float c_PointerMoveDuration = 0.4f;

        private const int c_MenuOptionAnchor = 3;
        private const int c_MenuOptionOffset = 2;

        private static readonly List<string> c_OptionTexts = new List<string>() { "NEW LAUNCH", "CONTINUE", "EXIT" };

        #endregion

        #region InGame Var

        enum InGameState
        {
            ShowDescription,
            WaitingInput,

            Animation
        }

        #region Reference
        private SingleColorCanvas m_InputFieldCanvas;
        private TextBox m_LocationTxt;
        private TextBox m_DesciptionTxt;
        private TextBox m_InputFieldTxt;
        private TextBox m_PromptText;
        private UnlitBox m_CursorText;
        private TextBox m_RespondText;
        #endregion

        private InGameState m_InGameState = InGameState.ShowDescription;
        private string m_DescriptionBuffer = string.Empty;

        private float m_DescriptionTextTimer = 0;

        private StringBuilder m_CommandString = new StringBuilder("");

        private float m_CursorFlickringTimer = 0;
        private bool m_LoadSaveFile = false;

        private bool m_IsDead = false;
        private bool m_IsFinished = false;

        private const float c_DescrTextDuration = 0.02f;
        private const float c_CursorFlickringDuration = 0.4f;
        private const int c_InputFieldMaxLength = 58;
        private static readonly Vector2Int c_CursorAnchor = new Vector2Int(5, 2);

        #endregion


        public TextAdventureApp(RectInt bounds, PixelColor color) : base(bounds, color)
        {
        }

        #region Override
        protected override void loadInitialScene()
        {
            enterMainMenu();
        }

        protected override void handleOnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.F9)
            {
                FrameLogger.Toggle();
            }
            else if (keyInfo.Key == ConsoleKey.F8)
            {
                AudioManager.Instance.IsMute = !AudioManager.Instance.IsMute;
            }
            else if (m_CurrScene == Scene.MainMenu)
            {
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    IsRunning = false;
                }
                else if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    m_CurrMenuSelection = (m_CurrMenuSelection - 1 < 0) ? m_MainOptionTextes.Count - 1 : m_CurrMenuSelection - 1;
                    selectMainMenuOption(m_CurrMenuSelection);
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    m_CurrMenuSelection = (m_CurrMenuSelection + 1 > m_MainOptionTextes.Count - 1) ? 0 : m_CurrMenuSelection + 1;
                    selectMainMenuOption(m_CurrMenuSelection);
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    confirmMainMenuOption();
                }
            }
            else if (m_CurrScene == Scene.InGame)
            {
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    loadScene(enterMainMenu, exitMainMenu);
                }
                else
                {

                    switch (m_InGameState)
                    {
                        case InGameState.ShowDescription:
                            if (keyInfo.Key == ConsoleKey.Enter)
                            {
                                m_DesciptionTxt.text = m_DescriptionBuffer;
                                AudioManager.Instance.StopAllAudio();
                            }
                            break;
                        case InGameState.WaitingInput:

                            if (keyInfo.Key == ConsoleKey.Backspace)
                            {
                                if (m_CommandString.Length > 0)
                                {
                                    m_CommandString.Length--;
                                    AudioManager.Instance.BeepMusic(80, 20);
                                }
                            }
                            else if (keyInfo.Key == ConsoleKey.Enter)
                            {
                                executeCommand(m_CommandString.ToString());
                                m_CommandString.Length = 0;
                                AudioManager.Instance.BeepMusic(350, 20);

                                if (m_IsDead)
                                {
                                    m_RespondText.text = string.Empty;
                                }
                            }
                            else
                            {
                                
                                if (m_CommandString.Length == 0)
                                {
                                    if (!m_IsDead)
                                    {
                                        m_RespondText.text = string.Empty;
                                    }
                                }
                                
                                if (m_CommandString.Length < c_InputFieldMaxLength)
                                {
                                    m_CommandString.Append(keyInfo.KeyChar);
                                    AudioManager.Instance.BeepMusic(150, 20);
                                }
                            }
                            m_InputFieldTxt.text = m_CommandString.ToString();
                            m_CursorText.Node.SetPosition(c_CursorAnchor + new Vector2Int(m_CommandString.Length, 0));

                            break;
                        case InGameState.Animation:
                            break;
                    }
                }
            }
        }

        protected override void update(long timeStep)
        {
            float second = timeStep / 1000.0f;

            if (m_CurrScene == Scene.MainMenu)
            {
                m_PointerMoveTimer += second;
                if (m_PointerMoveTimer > c_PointerMoveDuration)
                {
                    m_PointerMoveTimer = 0;
                    m_IsPointerSpanned = !m_IsPointerSpanned;

                    var txt = m_MainOptionTextes[m_CurrMenuSelection];

                    if (!m_IsPointerSpanned)
                    {
                        txt.text = string.Format("< {0} >", c_OptionTexts[m_CurrMenuSelection]);
                    }
                    else
                    {
                        txt.text = string.Format("<  {0}  >", c_OptionTexts[m_CurrMenuSelection]);
                    }
                }
            }
            else if (m_CurrScene == Scene.InGame)
            {
                #region Debug

                foreach (var trans in DialogueSystem.Instance.CurrentSituation.SituationTransitions)
                {
                    StringBuilder info = new StringBuilder();
                    info.Append(string.Format("CMD: {0, -15} -> SIT: {1, -15}", trans.Key, trans.Value.TargetSituationName));
                    FrameLogger.Log(info.ToString());
                }

                foreach (var variable in DialogueSystem.Instance.GlobalVariables)
                {
                    StringBuilder info = new StringBuilder();
                    info.Append(string.Format("({0, -9} = {1, 3:D3})", variable.Key, variable.Value));
                    FrameLogger.Log(info.ToString());
                }

                FrameLogger.Log("");
                #endregion

                switch (m_InGameState)
                {
                    case InGameState.ShowDescription:
                        m_DescriptionTextTimer += second;
                        if (m_DescriptionBuffer.Length > m_DesciptionTxt.text.Length)
                        {
                            if (m_DescriptionTextTimer > c_DescrTextDuration)
                            {


                                var nextChar = m_DescriptionBuffer[m_DesciptionTxt.text.Length];

                                if (m_DesciptionTxt.text.Length == 0 || (m_DescriptionBuffer[m_DesciptionTxt.text.Length - 1] != '\n' && nextChar == '\n'))
                                {
                                    AudioManager.Instance.BeepMusic(300, 150);
                                }
                                m_DesciptionTxt.text += nextChar;
                            }
                        }
                        else
                        {
                            changeInGameState(InGameState.WaitingInput);
                        }
                        break;
                    case InGameState.WaitingInput:
                        // Cursor Update
                        if (m_CommandString.Length >= c_InputFieldMaxLength)
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

                        break;
                    case InGameState.Animation:
                        break;
                }
            }
        }

        #endregion

        #region Scene

        private void enterMainMenu()
        {
            m_CurrScene = Scene.MainMenu;
            m_CurrMenuSelection = 0;
            m_MainOptionTextes = new List<TextBox>();

            playSpaceOddity();

            // Setup UI
            {
                //// static ti
                //// 
                var imgCanNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 2, 60, 35), null, "Image-Canvas");
                var imgCan = imgCanNode.AddUIComponent<SingleColorCanvas>();
                imgCan.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

                var imgNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 60, 35), imgCanNode, "Image");
                var img = imgNode.AddUIComponent<Bitmap>();
                img.LoadFromFile("./Assets/SpaceStation.txt");

                ///
                var iconCanNode = UINode.Engine.Instance.CreateNode(new RectInt(60, 2, 31, 10), null, "Icon-Canvas");
                var canvas = iconCanNode.AddUIComponent<SingleColorCanvas>();
                canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.Magenta);

                var iconNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 31, 10), iconCanNode, "Icon");
                m_TitleBitmap = iconNode.AddUIComponent<Bitmap>();
                m_TitleBitmap.LoadFromFile("./Assets/TitleText.txt");

                var creditNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 31, 10), iconCanNode, "Icon");
                m_CreditText = creditNode.AddUIComponent<TextBox>();
                m_CreditText.text = " By Ta David Yu ";
                m_CreditText.horizontalAlignment = TextBox.HorizontalAlignment.Center;
                m_CreditText.verticalAlignment = TextBox.VerticalAlignment.Bottom;

                /// Main Menu Node
                m_MainMenuNode = UINode.Engine.Instance.CreateNode(new RectInt(60, 12, 32, 25));
                m_MainMenuNode.AddUIComponent<SingleColorCanvas>();

                ////
                var txtCanvasNode = UINode.Engine.Instance.CreateNode(new RectInt(3, c_MenuOptionAnchor, 28, 3), m_MainMenuNode, "Text0-Canvas");
                canvas = txtCanvasNode.AddUIComponent<SingleColorCanvas>();
                canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkYellow);

                var newGameNode = UINode.Engine.Instance.CreateNode(new RectInt(1, 1, 23, 1), txtCanvasNode, "NEW");
                m_NewGameText = newGameNode.AddUIComponent<TextBox>();
                m_NewGameText.text = "NEW ADVENTURE";
                m_NewGameText.horizontalAlignment = TextBox.HorizontalAlignment.Center;
                m_NewGameText.verticalAlignment = TextBox.VerticalAlignment.Top;

                m_MainOptionTextes.Add(m_NewGameText);

                ////
                txtCanvasNode = UINode.Engine.Instance.CreateNode(new RectInt(3, c_MenuOptionAnchor + c_MenuOptionOffset, 28, 3), m_MainMenuNode, "Text1-Canvas");
                canvas = txtCanvasNode.AddUIComponent<SingleColorCanvas>();
                canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkYellow);

                var loadGameNode = UINode.Engine.Instance.CreateNode(new RectInt(1, 1, 23, 1), txtCanvasNode, "CONT");
                m_LoadGameText = loadGameNode.AddUIComponent<TextBox>();
                m_LoadGameText.text = "CONTINUE";
                m_LoadGameText.horizontalAlignment = TextBox.HorizontalAlignment.Center;
                m_LoadGameText.verticalAlignment = TextBox.VerticalAlignment.Top;

                m_MainOptionTextes.Add(m_LoadGameText);

                ////
                txtCanvasNode = UINode.Engine.Instance.CreateNode(new RectInt(3, c_MenuOptionAnchor + c_MenuOptionOffset * 2, 28, 3), m_MainMenuNode, "Text2-Canvas");
                canvas = txtCanvasNode.AddUIComponent<SingleColorCanvas>();
                canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkYellow);

                var exitNode = UINode.Engine.Instance.CreateNode(new RectInt(1, 1, 23, 1), txtCanvasNode, "EXIT");
                m_ExitText = exitNode.AddUIComponent<TextBox>();
                m_ExitText.text = "EXIT";
                m_ExitText.horizontalAlignment = TextBox.HorizontalAlignment.Center;
                m_ExitText.verticalAlignment = TextBox.VerticalAlignment.Top;

                m_MainOptionTextes.Add(m_ExitText);

                // - Hint
                txtCanvasNode = UINode.Engine.Instance.CreateNode(new RectInt(2, c_MenuOptionAnchor + 9, 30, 3), m_MainMenuNode, "Hint-Canvas");
                canvas = txtCanvasNode.AddUIComponent<SingleColorCanvas>();
                canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkRed);

                var hintNode = UINode.Engine.Instance.CreateNode(new RectInt(2, 1, 23, 1), txtCanvasNode, "HINT");
                var hintTxt = hintNode.AddUIComponent<TextBox>();
                hintTxt.text = "arrows-select\nenter-confirm";
                hintTxt.horizontalAlignment = TextBox.HorizontalAlignment.Center;
                hintTxt.verticalAlignment = TextBox.VerticalAlignment.Top;

                //
                txtCanvasNode = UINode.Engine.Instance.CreateNode(new RectInt(2, c_MenuOptionAnchor + 12, 30, 3), m_MainMenuNode, "Hint-Canvas");
                canvas = txtCanvasNode.AddUIComponent<SingleColorCanvas>();
                canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkRed);

                hintNode = UINode.Engine.Instance.CreateNode(new RectInt(2, 1, 23, 1), txtCanvasNode, "MUSIC");
                hintTxt = hintNode.AddUIComponent<TextBox>();
                hintTxt.text = "MUSIC - SPACE ODDITY";
                hintTxt.horizontalAlignment = TextBox.HorizontalAlignment.Center;
                hintTxt.verticalAlignment = TextBox.VerticalAlignment.Top;
            }
            var bannerNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 32, 95, 1), null, "BannerCanvas");
            m_HintBanner = bannerNode.AddUIComponent<SingleColorCanvas>();
            m_HintBanner.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkGray);
            var bannerTextNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 1), bannerNode, "BannerText");
            m_HintBannerText = bannerTextNode.AddUIComponent<TextBox>();
            m_HintBannerText.text = "[F8] to mute/unmute audio, [F9] to turn on/off debug mode";
            m_HintBannerText.horizontalAlignment = TextBox.HorizontalAlignment.Center;

            // Final setup
            selectMainMenuOption(m_CurrMenuSelection);
        }

        private void enterInGame()
        {
            //DialogueSystem.Instance.debug();

            m_CurrScene = Scene.InGame;

            m_IsDead = false;
            m_IsFinished = false;

            m_CommandString = new StringBuilder("");

            // Setup UI
            {
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

                var locationNameNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 22, 30), locationNode, "LocationName");
                m_LocationTxt = locationNameNode.AddUIComponent<TextBox>();
                m_LocationTxt.horizontalAlignment = TextBox.HorizontalAlignment.Center;
                m_LocationTxt.verticalAlignment = TextBox.VerticalAlignment.Middle;

                //// Description
                var descriptionNode = UINode.Engine.Instance.CreateNode(new RectInt(26, 2, 67, 22), null, "Descr-Canvas");
                var descriptionCanvas = descriptionNode.AddUIComponent<SingleColorCanvas>();
                descriptionCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.White);

                // 
                var descriptionLayoutNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 67, 22), descriptionNode, "Descr-Layout-Canvas");
                var descriptionLayout = descriptionLayoutNode.AddUIComponent<Bitmap>();
                descriptionLayout.LoadFromFile("./Assets/DescrLayout.txt", Bitmap.DrawType.Sliced);

                var descriptionTitleNode = descriptionLayoutNode.AddUIComponent<TextBox>();
                descriptionTitleNode.horizontalAlignment = TextBox.HorizontalAlignment.Left;
                descriptionTitleNode.verticalAlignment = TextBox.VerticalAlignment.Top;
                descriptionTitleNode.text = "+---[ DESCRIPTION ]";

                //
                var descriptionTextNode = UINode.Engine.Instance.CreateNode(new RectInt(2, 2, 65, 1), descriptionNode, "Text");
                m_DesciptionTxt = descriptionTextNode.AddUIComponent<TextBox>();
                m_DesciptionTxt.text = "This is the situation description, do something. Describe the \nlocation and the things you could do";
                m_DesciptionTxt.horizontalAlignment = TextBox.HorizontalAlignment.Left;


                //// InputField
                var inputFieldNode = UINode.Engine.Instance.CreateNode(new RectInt(26, 24, 67, 8), null, "Input-Canvas");
                m_InputFieldCanvas = inputFieldNode.AddUIComponent<SingleColorCanvas>();
                m_InputFieldCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.DarkYellow);

                // 
                var inputLayoutNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 67, 8), inputFieldNode, "Input-Layout-Canvas");
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
                m_RespondText.horizontalAlignment = TextBox.HorizontalAlignment.Left;
            }
            var bannerNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 32, 95, 1), null, "BannerCanvas");
            m_HintBanner = bannerNode.AddUIComponent<SingleColorCanvas>();
            m_HintBanner.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkGray);
            var bannerTextNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 1), bannerNode, "BannerText");
            m_HintBannerText = bannerTextNode.AddUIComponent<TextBox>();
            m_HintBannerText.text = "[ESC] to main menu, [F1] to save, [F2] to load, [ENTER] to skip description";
            m_HintBannerText.horizontalAlignment = TextBox.HorizontalAlignment.Center;

            // Setup Dialogue Machine
            DialogueSystem.Instance.CreateDialogueTreeFromFile("./SpaceBoy.json");

            DialogueSystem.Instance.OnEnterSituation += handleOnSituationChanged;
            DialogueSystem.Instance.OnTransitionFailed += handleOnTransitionFailed;
            DialogueSystem.Instance.OnExecuteInvalidCommand += handleOnExecuteInvalidCommand;

            // load save file if there's one, or flag is set
            if (m_LoadSaveFile)
            {
                DialogueSystem.Instance.Load("FirstVessel", new Dictionary<string, int>(), new HashSet<string>());
            }
            else
            {
                DialogueSystem.Instance.Load("FirstVessel", new Dictionary<string, int>(), new HashSet<string>());
            }
        }

        private void exitMainMenu()
        {
            // nothing
        }

        private void exitInGame()
        {
            // deregister event
            DialogueSystem.Instance.OnEnterSituation -= handleOnSituationChanged;
            DialogueSystem.Instance.OnTransitionFailed -= handleOnTransitionFailed;
            DialogueSystem.Instance.OnExecuteInvalidCommand -= handleOnExecuteInvalidCommand;
        }

        #endregion

        private void playSpaceOddity()
        {
            #region Audio

            AudioManager.Instance.BeepMusic(523, 400); // DO
            AudioManager.Instance.BeepMusic(523, 200); // DO
            AudioManager.Instance.BeepMusic(523, 600); // DO
            AudioManager.Instance.BeepMusic(523, 200); // DO
            AudioManager.Instance.BeepMusic(587, 600); // RE
            AudioManager.Instance.BeepMusic(523, 200); // DO
            AudioManager.Instance.BeepMusic(494, 600); // SI

            AudioManager.Instance.DelayMusic(1200);

            AudioManager.Instance.BeepMusic(440, 800); // LA
            AudioManager.Instance.BeepMusic(392, 800); // SO
            AudioManager.Instance.BeepMusic(523, 800); // DO

            AudioManager.Instance.DelayMusic(100);

            AudioManager.Instance.BeepMusic(523, 400); // DO
            AudioManager.Instance.BeepMusic(523, 200); // DO
            AudioManager.Instance.BeepMusic(523, 600); // DO 
            AudioManager.Instance.BeepMusic(523, 200); // DO
            AudioManager.Instance.BeepMusic(587, 600); // RE
            AudioManager.Instance.BeepMusic(523, 200); // DO
            AudioManager.Instance.BeepMusic(494, 600); // SI

            AudioManager.Instance.DelayMusic(4000);

            AudioManager.Instance.BeepMusic(523, 400); // DO
            AudioManager.Instance.BeepMusic(587, 400); // RE
            AudioManager.Instance.BeepMusic(523, 400); // DO
            AudioManager.Instance.BeepMusic(587, 400); // RE
            AudioManager.Instance.BeepMusic(523, 400); // DO
            AudioManager.Instance.BeepMusic(587, 400); // RE
            AudioManager.Instance.BeepMusic(523, 400); // DO
            AudioManager.Instance.BeepMusic(587, 400); // RE
            AudioManager.Instance.BeepMusic(523, 400); // DO
            AudioManager.Instance.BeepMusic(587, 200); // RE
            AudioManager.Instance.BeepMusic(587, 1200); // RE

            AudioManager.Instance.DelayMusic(800);

            // Elevate
            int initial = 300;
            int increment = 25;
            AudioManager.Instance.BeepMusic(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.BeepMusic(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.BeepMusic(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.BeepMusic(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.BeepMusic(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.BeepMusic(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.BeepMusic(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.BeepMusic(initial, 400); // RE
            initial += increment;
            AudioManager.Instance.BeepMusic(initial, 3200); // RE

            AudioManager.Instance.DelayMusic(800);

            AudioManager.Instance.BeepMusic(392, 200); // SO
            AudioManager.Instance.BeepMusic(392, 600); // SO
            AudioManager.Instance.BeepMusic(523, 400); // DO
            AudioManager.Instance.BeepMusic(587, 400); // RE
            AudioManager.Instance.BeepMusic(698, 400); // FA
            AudioManager.Instance.BeepMusic(659, 400); // MI
            AudioManager.Instance.BeepMusic(587, 400); // RE
            AudioManager.Instance.BeepMusic(523, 400); // DO
            AudioManager.Instance.BeepMusic(494, 600); // SI

            AudioManager.Instance.DelayMusic(800);

            AudioManager.Instance.BeepMusic(494, 400); // SI
            AudioManager.Instance.BeepMusic(659, 400); // MI
            AudioManager.Instance.BeepMusic(587, 400); // RE
            AudioManager.Instance.BeepMusic(523, 400); // DO
            AudioManager.Instance.BeepMusic(494, 400); // SI
            AudioManager.Instance.BeepMusic(523, 600); // DO
            AudioManager.Instance.BeepMusic(587, 200); // RE
            AudioManager.Instance.BeepMusic(440, 600); // LA

            // .. and the papers want to know whose shirts you wear


            #endregion
        }

        #region MainMenu func
        private void selectMainMenuOption(int id)
        {
            m_IsPointerSpanned = false;
            m_PointerMoveTimer = 0;

            for (int i = 0; i < m_MainOptionTextes.Count; i++)
            {
                var txt = m_MainOptionTextes[i];
                if (i == id)
                {
                    txt.text = string.Format("< {0} >", c_OptionTexts[i]);
                    (txt.Node.ParentCanvas as SingleColorCanvas).CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.Yellow);
                }
                else
                {
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
                    m_LoadSaveFile = false;
                    loadScene(enterInGame, exitInGame);
                    break;
                case 1:
                    m_LoadSaveFile = true;
                    loadScene(enterInGame, exitInGame);
                    break;
                case 2:
                    IsRunning = false;
                    break;
            }
        }
        #endregion

        #region InGame func

        private void executeCommand(string input)
        {
            if (m_IsDead)
            {
                if (string.Compare(input, "new", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    m_LoadSaveFile = false;
                    loadScene(enterInGame, exitInGame);
                }
                else if (string.Compare(input, "load", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    m_LoadSaveFile = true;
                    loadScene(enterInGame, exitInGame);
                }
                else if (string.Compare(input, "exit", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    loadScene(enterMainMenu, exitMainMenu);
                }
            }
            else
            {
                DialogueSystem.Instance.ExecuteCommand(input);
            }
        }

        private void handleOnSituationChanged(string prevSitName, string nextSitName)
        {
            var nextSit = DialogueSystem.Instance.Tree.SituationTables[nextSitName];
            
            if (nextSit.LocationName == "Death")
            {
                m_IsDead = true;
            }

            // TODO: load image to replace
            m_LocationTxt.text = nextSit.LocationName;

            changeInGameState(InGameState.ShowDescription);

            m_DesciptionTxt.text = string.Empty;
            if (DialogueSystem.Instance.VisitedSituation.Contains(nextSitName))
            {
                m_DescriptionBuffer = nextSit.Description;
            }
            else
            {
                m_DescriptionBuffer = nextSit.FirstDescription;
            }
        }

        private void handleOnTransitionFailed(Transition trans)
        {
            if (!trans.HasOnFialSituation)
            {
                m_RespondText.text = trans.OnFailDescription;
            }
        }

        private void handleOnExecuteInvalidCommand(string cmd)
        {
            if (!m_IsDead)
            {
                m_RespondText.text = "There is no way you can [" + cmd + "] now.";
            }
        }

        private void changeInGameState(InGameState state)
        {
            m_InGameState = state;
            if (m_InGameState == InGameState.WaitingInput)
            {
                m_InputFieldCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.Yellow);
                m_PromptText.Node.IsActive = true;
                m_CursorText.Node.IsActive = true;

                if (DialogueSystem.Instance.CurrentSituation.LocationName == "Death")
                {
                    m_RespondText.text = "[NEW] to start a new game, [LOAD] to load from previous save, \n[EXIT] to main menu";
                }
                else
                {
                    m_RespondText.text = "What do you want to do?";
                }
            }
            else if (m_InGameState == InGameState.ShowDescription)
            {
                m_InputFieldCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.DarkYellow);
                m_CursorText.Node.IsActive = false;
                m_PromptText.Node.IsActive = false;
                m_RespondText.text = "";
            }
        }

        #endregion
    }
}
