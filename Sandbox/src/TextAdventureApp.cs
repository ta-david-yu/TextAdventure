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
            InGame,
            Ending
        }

        private Scene m_CurrScene = Scene.MainMenu;

        private SingleColorCanvas m_HintBanner;
        private TextBox m_HintBannerText;

        #region MainMenu Var

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
        private Bitmap m_LocationImg;
        #endregion

        private InGameState m_InGameState = InGameState.ShowDescription;
        private string m_DescriptionBuffer = string.Empty;

        private float m_DescriptionTextTimer = 0;

        private StringBuilder m_CommandString = new StringBuilder("");

        private float m_CursorFlickringTimer = 0;

        private bool m_IsDead = false;
        private bool m_IsFinished = false;

        private const float c_DescrTextDuration = 0.02f;
        private const float c_CursorFlickringDuration = 0.4f;
        private const int c_InputFieldMaxLength = 58;
        private static readonly Vector2Int c_CursorAnchor = new Vector2Int(5, 2);

        private const string c_SaveFileName = "./save.json";

        #endregion

        #region Ending Var

        enum EndingState
        {
            WaitingShaking,
            WaitingLiftoff,

            MoveRightToLeft,

            LandingEarth,

            ShowEndingText
        }

        #region Reference

        private UINode m_WaitingNode;
        private SingleColorCanvas m_ISSCanvas;
        private UINode m_ModuleNode;

        private UINode m_RightToLeftNode;
        private UINode m_RightToLeftModuleNode;

        private UINode m_EarthCutNode;
        private UINode m_SmallModuleNode;

        private SingleColorCanvas m_EndingText;

        #endregion

        private EndingState m_EndingState = EndingState.WaitingShaking;

        private float m_Timer = 0;
        private float m_SubTimer = 0;

        private bool m_ModuleIsLeft = false;
        private float m_ShakeTimer = 0;
        private float m_LiftOffMoveTimer = 0;

        private const float c_ShakingDuration = 3.5f;
        private const float c_ShakingCycle = 0.08f;

        private const float c_LiftoffDuration = 2.7f;
        private const float c_LiftOffTimePerPixel = 0.27f;

        private const float c_MoveRightToLeftDuration = 6.5f;
        private const float c_MoveTimePerPixel = 0.1f;

        private const float c_SmallMoveDuration = 70.0f;
        private const float c_SmallMoveTimePerPixel = 1.55f;

        #endregion

        #region Global

        private PlayerProgress m_Progress;

        private bool m_LoadSaveFile = false;

        #endregion

        public TextAdventureApp(RectInt bounds, PixelColor color) : base(bounds, color)
        {
        }

        #region Override
        protected override void loadInitialScene()
        {
            if (System.IO.File.Exists(c_SaveFileName))
            {
                m_Progress = PlayerProgress.LoadFromFile(c_SaveFileName);
            }
            else
            {
                m_Progress = new PlayerProgress();
                m_Progress.HasClearedGame = false;
                m_Progress.Situation = "FirstModule";
                m_Progress.GlobalVariables = new Dictionary<string, int>();
                m_Progress.VisitedSituation = new HashSet<string>();
            }

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
                    // return to main menu
                    loadScene(enterMainMenu, exitMainMenu);
                }
                else if (keyInfo.Key == ConsoleKey.F1)
                {
                    // save to file
                    m_Progress.Situation = DialogueSystem.Instance.CurrSitName;
                    m_Progress.GlobalVariables = DialogueSystem.Instance.GlobalVariables;
                    m_Progress.VisitedSituation = DialogueSystem.Instance.VisitedSituation;

                    PlayerProgress.SaveToFile(m_Progress, c_SaveFileName);

                    AudioManager.Instance.BeepMusic(790, 50);

                    FrameLogger.LogError("SAVE COMPLETE! >>> press enter to continue <<<\n\n");
                }
                else if (keyInfo.Key == ConsoleKey.F2)
                {
                    // load from file

                    m_LoadSaveFile = true;
                    loadScene(enterInGame, exitInGame);

                    AudioManager.Instance.BeepMusic(790, 50);

                    FrameLogger.LogError("LOAD COMPLETE! >>> press enter to continue <<<\n\n");
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
            else if (m_CurrScene == Scene.Ending)
            {
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    loadScene(enterMainMenu, exitMainMenu);
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

                FrameLogger.Log("SIT: " + DialogueSystem.Instance.CurrSitName);

                foreach (var trans in DialogueSystem.Instance.CurrentSituation.SituationTransitions)
                {
                    StringBuilder info = new StringBuilder();
                    info.Append(string.Format("CMD: {0, -15} -> SIT: {1, -15}", trans.Key, trans.Value.TargetSituationName));
                    FrameLogger.Log(info.ToString());
                }

                FrameLogger.Log("");

                foreach (var variable in DialogueSystem.Instance.GlobalVariables)
                {
                    StringBuilder info = new StringBuilder();
                    info.Append(string.Format("({0, -9} = {1, 3:D3})", variable.Key, variable.Value));
                    FrameLogger.Log(info.ToString());
                }

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
            else if (m_CurrScene == Scene.Ending)
            {
                m_Timer += second;
                m_SubTimer += second;
                m_ShakeTimer += second;

                if (m_EndingState == EndingState.WaitingShaking)
                {
                    if (m_ShakeTimer > c_ShakingCycle)
                    {
                        m_ModuleNode.Translate(new Vector2Int((m_ModuleIsLeft) ? 2 : -2, 0));
                        m_ModuleIsLeft = !m_ModuleIsLeft;
                        m_ShakeTimer = 0;
                    }

                    if (m_Timer > c_ShakingDuration)
                    {
                        m_EndingState = EndingState.WaitingLiftoff;
                        m_Timer = 0;
                    }
                }
                else if (m_EndingState == EndingState.WaitingLiftoff)
                {
                    m_LiftOffMoveTimer += second;
                    if (m_ShakeTimer > c_ShakingCycle)
                    {
                        m_ModuleNode.Translate(new Vector2Int((m_ModuleIsLeft) ? 2 : -2, 0));
                        m_ModuleIsLeft = !m_ModuleIsLeft;
                        m_ShakeTimer = 0;
                    }

                    if (m_LiftOffMoveTimer > c_LiftOffTimePerPixel)
                    {
                        m_ModuleNode.Translate(new Vector2Int(0, -1));
                        m_LiftOffMoveTimer = 0;
                    }

                    if (m_Timer > c_LiftoffDuration)
                    {
                        m_EndingState = EndingState.MoveRightToLeft;
                        m_Timer = 0;
                        m_WaitingNode.IsActive = false;
                        m_RightToLeftNode.IsActive = true;
                    }
                }
                else if (m_EndingState == EndingState.MoveRightToLeft)
                {
                    if (m_SubTimer > c_MoveTimePerPixel)
                    {
                        m_RightToLeftModuleNode.Translate(new Vector2Int(-1, 0));
                        m_SubTimer = 0;
                    }

                    if (m_Timer > c_MoveRightToLeftDuration)
                    {
                        m_EndingState = EndingState.LandingEarth;
                        m_Timer = 0;
                        m_RightToLeftNode.IsActive = false;
                        m_EarthCutNode.IsActive = true;
                    }
                }
                else if (m_EndingState == EndingState.LandingEarth)
                {
                    if (m_SubTimer > c_SmallMoveTimePerPixel)
                    {
                        m_SmallModuleNode.Translate(new Vector2Int(-1, 0));
                        m_SubTimer = 0;
                    }

                    if (m_Timer > c_SmallMoveDuration)
                    {
                        m_EndingState = EndingState.ShowEndingText;
                        m_Timer = 0;
                    }
                }
                else if (m_EndingState == EndingState.ShowEndingText)
                {
                }
                else
                {

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

            AudioManager.Instance.OnMusicQueueEmptied += playSpaceOddity;

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
                locationCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.Cyan);

                var locationLayoutNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 22, 30), locationNode, "Bitmap");
                var locationLayout = locationLayoutNode.AddUIComponent<Bitmap>();
                locationLayout.LoadFromFile("./Assets/LocationLayout.txt", Bitmap.DrawType.Sliced);

                var locationTitle = locationLayoutNode.AddUIComponent<TextBox>();
                locationTitle.horizontalAlignment = TextBox.HorizontalAlignment.Center;
                locationTitle.verticalAlignment = TextBox.VerticalAlignment.Top;
                locationTitle.text = "[ LOCATION ]";

                var locationInsideCanvasNode = UINode.Engine.Instance.CreateNode(new RectInt(1, 1, 20, 28), locationNode, "InsideCanvas");
                var locationInsideCanvas = locationInsideCanvasNode.AddUIComponent<SingleColorCanvas>();
                locationInsideCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkBlue, ConsoleColor.DarkGreen);


                var locationNameNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 20, 28), locationInsideCanvasNode, "LocationName");
                m_LocationTxt = locationNameNode.AddUIComponent<TextBox>();
                m_LocationTxt.horizontalAlignment = TextBox.HorizontalAlignment.Center;
                m_LocationTxt.verticalAlignment = TextBox.VerticalAlignment.Middle;

                var locationImgNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 20, 28), locationInsideCanvasNode, "LocationImg");
                m_LocationImg = locationImgNode.AddUIComponent<Bitmap>();
                m_LocationImg.LoadFromFile("./Assets/Rocket.txt");
                
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
                m_Progress = PlayerProgress.LoadFromFile(c_SaveFileName);
            }
            else
            {
                m_Progress.Situation = "FirstModule";
                m_Progress.GlobalVariables = new Dictionary<string, int>();
                m_Progress.VisitedSituation = new HashSet<string>();
            }

            DialogueSystem.Instance.Load(m_Progress.Situation, m_Progress.GlobalVariables, m_Progress.VisitedSituation);
        }

        private void enterEnding()
        {
            m_CurrScene = Scene.Ending;
            m_EndingState = EndingState.WaitingShaking;
            m_Timer = 0;
            m_SubTimer = 0;
            m_ShakeTimer = 0;

            playSpaceOddity();

            // First cut
            {
                m_WaitingNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 35), null, "Waiting-Node");

                //
                var bgNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 22), m_WaitingNode, "bg");
                var gbCanvas = bgNode.AddUIComponent<SingleColorCanvas>();
                gbCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

                var bgImageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 22), bgNode, "bgImage");
                var bgImage = bgImageNode.AddUIComponent<Bitmap>();
                bgImage.LoadFromFile("./Assets/StarBackground.txt");

                //
                m_ModuleNode = UINode.Engine.Instance.CreateNode(new RectInt(56, 9, 15, 21), m_WaitingNode, "Module");

                var module2Node = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 15, 21), m_ModuleNode, "ModuleImage");
                var module2Canvas = module2Node.AddUIComponent<SingleColorCanvas>();
                module2Canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

                var moduleImageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 15, 21), module2Node, "ModuleImage");
                var moduleImage = moduleImageNode.AddUIComponent<Bitmap>();
                moduleImage.LoadFromFile("./Assets/ISS-Module.txt");

                //
                var iSSNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 23, 95, 14), m_WaitingNode, "ISS");
                m_ISSCanvas = iSSNode.AddUIComponent<SingleColorCanvas>();
                m_ISSCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

                var iSSImageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 14), iSSNode, "ISSImage");
                var iSSImage = iSSImageNode.AddUIComponent<Bitmap>();
                iSSImage.LoadFromFile("./Assets/ISS-Station.txt");

                
            }

            // Second Cut
            {
                m_RightToLeftNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 35), null, "RightToLeft-Node");
                m_RightToLeftNode.IsActive = false;

                //
                var bgNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 22), m_RightToLeftNode, "bg");
                var gbCanvas = bgNode.AddUIComponent<SingleColorCanvas>();
                gbCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

                var bgImageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 22), bgNode, "bgImage");
                var bgImage = bgImageNode.AddUIComponent<Bitmap>();
                bgImage.LoadFromFile("./Assets/StarBackground.txt");

                //
                m_RightToLeftModuleNode = UINode.Engine.Instance.CreateNode(new RectInt(60, 15, 46, 8), m_RightToLeftNode, "Module");

                var spaceShip2Node = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 46, 8), m_RightToLeftModuleNode, "ModuleImage");
                var spaceShip2Node2Canvas = spaceShip2Node.AddUIComponent<SingleColorCanvas>();
                spaceShip2Node2Canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

                var spaceShipImageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 46, 8), spaceShip2Node, "ModuleImage");
                var spaceShipImage = spaceShipImageNode.AddUIComponent<Bitmap>();
                spaceShipImage.LoadFromFile("./Assets/RightToleftModule.txt");
            }

            // Third Cut
            {
                m_EarthCutNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 35), null, "EarthCut-Node");
                m_EarthCutNode.IsActive = false;

                //
                var bgNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 22), m_EarthCutNode, "bg");
                var gbCanvas = bgNode.AddUIComponent<SingleColorCanvas>();
                gbCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

                var bgImageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 95, 22), bgNode, "bgImage");
                var bgImage = bgImageNode.AddUIComponent<Bitmap>();
                bgImage.LoadFromFile("./Assets/StarBackground.txt");

                //
                var earthNode = UINode.Engine.Instance.CreateNode(new RectInt(2, 5, 47, 23), m_EarthCutNode, "earth");
                var earthCanvas = earthNode.AddUIComponent<SingleColorCanvas>();
                earthCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

                var earthImageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 47, 23), earthNode, "earthImg");
                var earthImage = earthImageNode.AddUIComponent<Bitmap>();
                earthImage.LoadFromFile("./Assets/Earth.txt");

                //
                
                m_SmallModuleNode = UINode.Engine.Instance.CreateNode(new RectInt(90, 20, 8, 1), m_EarthCutNode, "Module");

                var spaceShip2Node = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 8, 1), m_SmallModuleNode, "ModuleImageNode");
                var spaceShip2Node2Canvas = spaceShip2Node.AddUIComponent<SingleColorCanvas>();
                spaceShip2Node2Canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

                var spaceShipImageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 8, 1), spaceShip2Node, "ModuleImage");
                var spaceShipImage = spaceShipImageNode.AddUIComponent<Bitmap>();
                spaceShipImage.LoadFromFile("./Assets/SmallModule.txt");
            }
        }

        private void exitMainMenu()
        {
            AudioManager.Instance.OnMusicQueueEmptied -= playSpaceOddity;
        }

        private void exitInGame()
        {
            // deregister event
            DialogueSystem.Instance.OnEnterSituation -= handleOnSituationChanged;
            DialogueSystem.Instance.OnTransitionFailed -= handleOnTransitionFailed;
            DialogueSystem.Instance.OnExecuteInvalidCommand -= handleOnExecuteInvalidCommand;
        }

        private void exitEnding()
        {

        }

        #endregion

        private void playSpaceOddity()
        {
            #region Audio

            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.Do, 600);
            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.Re, 600);
            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.SiL, 600);

            AudioManager.Instance.DelayMusic(1200);

            AudioManager.Instance.PlayNote(Note.LaL, 800);
            AudioManager.Instance.PlayNote(Note.SoL, 800);
            AudioManager.Instance.PlayNote(Note.Do, 800);

            AudioManager.Instance.DelayMusic(100);

            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.Do, 600);
            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.Re, 600);
            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.SiL, 600);

            AudioManager.Instance.DelayMusic(4000);

            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.Re, 1200);

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

            AudioManager.Instance.PlayNote(Note.SoL, 200);
            AudioManager.Instance.PlayNote(Note.SoL, 600);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Fa, 400);
            AudioManager.Instance.PlayNote(Note.Mi, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.SiL, 600);

            AudioManager.Instance.DelayMusic(800);

            AudioManager.Instance.PlayNote(Note.SiL, 400);
            AudioManager.Instance.PlayNote(Note.Mi, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.SiL, 400);
            AudioManager.Instance.PlayNote(Note.Do, 600);
            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.LaL, 600);

            AudioManager.Instance.DelayMusic(1200);
            
            // .. and the papers want to know whose shirts you wear
            AudioManager.Instance.PlayNote(Note.LaL, 400);
            AudioManager.Instance.PlayNote(Note.SiL, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.Do, 600);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.Re, 600);
            AudioManager.Instance.PlayNote(Note.Do, 600);

            AudioManager.Instance.DelayMusic(1600);
            
            // Now it's time to leave the capsule if you dare
            AudioManager.Instance.PlayNote(Note.LaL, 400);
            AudioManager.Instance.PlayNote(Note.SiL, 400);
            AudioManager.Instance.PlayNote(Note.Do, 600);
            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Re, 200);

            AudioManager.Instance.PlayNote(Note.Fa, 600);
            AudioManager.Instance.PlayNote(Note.Mi, 200);
            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.Do, 600);


            AudioManager.Instance.DelayMusic(2000);

            // Ground control to major Tom, your circuit's dead
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Re, 600);
            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.Re, 500);
            AudioManager.Instance.PlayNote(Note.Mi, 700);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);

            // there's something wrong, can you hear me major tom
            AudioManager.Instance.PlayNote(Note.LaL, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Mi, 400);

            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Mi, 200);
            AudioManager.Instance.PlayNote(Note.Re, 600);

            AudioManager.Instance.DelayMusic(800);

            // can you hear major tom
            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Mi, 200);
            AudioManager.Instance.PlayNote(Note.Re, 600);

            AudioManager.Instance.DelayMusic(800);
            
            // can you hear major tom
            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Mi, 200);
            AudioManager.Instance.PlayNote(Note.Re, 600);

            // can you, here  am I sitting in a tin can
            AudioManager.Instance.DelayMusic(800);

            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.Mi, 3000);
            AudioManager.Instance.PlayNote(Note.Re, 500);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.SiL, 200);
            AudioManager.Instance.PlayNote(Note.SiL, 200);
            AudioManager.Instance.PlayNote(Note.SiL, 200);
            AudioManager.Instance.PlayNote(Note.Do, 300);
            AudioManager.Instance.PlayNote(Note.SiL, 500);
            AudioManager.Instance.PlayNote(Note.SoL, 500);

            AudioManager.Instance.DelayMusic(2000);
            
            // far  a bove the world
            AudioManager.Instance.PlayNote(Note.Mi, 2400);
            AudioManager.Instance.PlayNote(Note.Mi, 300);
            AudioManager.Instance.PlayNote(Note.Re, 700);
            AudioManager.Instance.PlayNote(Note.Do, 200);
            AudioManager.Instance.PlayNote(Note.SiL, 800);

            AudioManager.Instance.DelayMusic(3200);

            // planet earth is blue
            AudioManager.Instance.PlayNote(Note.Re, 200);
            AudioManager.Instance.PlayNote(Note.Re, 400);
            AudioManager.Instance.PlayNote(Note.Re, 600);
            AudioManager.Instance.PlayNote(Note.Mi, 400);
            AudioManager.Instance.PlayNote(Note.Do, 600);

            AudioManager.Instance.DelayMusic(200);

            // and there is nothing I can do
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.SiL, 200);
            AudioManager.Instance.PlayNote(Note.SiL, 600);
            AudioManager.Instance.PlayNote(Note.Do, 400);
            AudioManager.Instance.PlayNote(Note.SiL, 400);
            AudioManager.Instance.PlayNote(Note.LaL, 1200);

            AudioManager.Instance.DelayMusic(3600);

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
            else if (m_IsFinished)
            {
                m_Progress.HasClearedGame = true;
                loadScene(enterEnding, exitEnding);
            }
            else
            {
                DialogueSystem.Instance.ExecuteCommand(input);
            }
        }

        private void handleOnSituationChanged(string prevSitName, string nextSitName)
        {
            try
            {
                var nextSit = DialogueSystem.Instance.Tree.SituationTables[nextSitName];

                if (nextSit.LocationName == "Death")
                {
                    m_IsDead = true;
                }

                if (nextSit.IsEnding)
                {
                    m_IsFinished = true;
                }

                // TODO: load image to replace
                var path = "./Assets/" + nextSit.LocationName + ".txt";
                if (System.IO.File.Exists(path))
                {
                    m_LocationImg.Node.IsActive = true;
                    m_LocationTxt.Node.IsActive = false;
                    m_LocationImg.LoadFromFile(path);
                }
                else
                {
                    m_LocationImg.Node.IsActive = false;
                    m_LocationTxt.Node.IsActive = true;
                    m_LocationTxt.text = nextSit.LocationName + "\n\nNO IMAGE";
                }

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
            catch (Exception exp)
            {
                FrameLogger.LogError(string.Format(exp.Message + ", SitName {0} not found", nextSitName));
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
            if (!m_IsDead || !m_IsFinished)
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

                if (m_IsDead)
                {
                    m_RespondText.text = "[NEW] to start a new game, [LOAD] to load from previous save, \n[EXIT] to main menu";
                }
                else if (m_IsFinished)
                {
                    m_RespondText.text = "[ENTER] to continue";
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
