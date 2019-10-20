﻿using System;
using System.Collections.Generic;
using System.Text;
using DYTA;
using DYTA.Math;
using DYTA.Render;

namespace NSShaft
{
    class SnakeApplication : ApplicationBase
    {
        enum GameState
        {
            Menu,
            InGame,
            Tutorial
        }

        enum GameMode
        {
            Single,
            TwoPlayers,
            PvP
        }

        private GameState State { get; set; }

        private bool m_OptimizedMode = false;

        #region MenuVar

        private int m_CurrentMenuSelection = 0;

        #endregion

        #region MenuUI

        private List<TextBox> m_MenuOptionsTextBoxes = new List<TextBox>();

        #endregion

        #region InGameVar

        private GameMode Mode { get; set; }

        private World2D World { get; set; }

        private HashSet<int> AliveCounter = new HashSet<int>();

        private bool IsFinished { get; set; } = false;

        private bool IsPaused { get; set; } = false;

        #endregion

        #region GameUI

        private UINode m_PlayGroundNode;
        private List<TextBox> m_HpTexts;
        private List<TextBox> m_HpBarTexts;
        private TextBox m_LevelText;

        private SingleColorCanvas m_GameOverCanvas;
        private TextBox m_GameOverText;

        #endregion

        private List<Action> c_OptionDelegates
        {
            get
            {
                List<Action> ret = new List<Action>();
                ret.Add(() =>
                {
                    Mode = GameMode.Single;
                    loadScene(enterInGameScene, delegate { });
                });
                ret.Add(() =>
                {
                    Mode = GameMode.TwoPlayers;
                    loadScene(enterInGameScene, delegate { });
                });
                ret.Add(() =>
                {
                    Mode = GameMode.PvP;
                    loadScene(enterInGameScene, delegate { });
                });
                ret.Add(() =>
                {
                    loadScene(enterTutorial, delegate { });
                });
                ret.Add(() =>
                {
                    IsRunning = false;
                });
                return ret;
            }
        }

        private static readonly List<string> c_OptionTexts = new List<string>() { "   1 PLAYER", "   2 PLAYERS", "   2 PLAYERS PVP", "   TUTORIAL", "   EXIT" };

        private static readonly Vector2Int c_GameWindowSize = new Vector2Int(50, 25);

        private static readonly ConsoleKey[,] c_InputTable = new ConsoleKey[,]
        {
            { ConsoleKey.RightArrow, ConsoleKey.LeftArrow },
            { ConsoleKey.D, ConsoleKey.A },
            { ConsoleKey.NumPad6, ConsoleKey.NumPad4 }
        };

        public SnakeApplication(Vector2Int windowSize, PixelColor color) : base(windowSize, color)
        {

        }

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
            else if (keyInfo.Key == ConsoleKey.F1)
            {
                m_OptimizedMode = !m_OptimizedMode;
            }

            if (State == GameState.Menu)
            {
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    IsRunning = false;
                }
                else if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    m_CurrentMenuSelection = (m_CurrentMenuSelection - 1 < 0) ? c_OptionTexts.Count - 1 : m_CurrentMenuSelection - 1;
                    selectOption(m_CurrentMenuSelection);
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    m_CurrentMenuSelection = (m_CurrentMenuSelection + 1 > c_OptionTexts.Count - 1) ? 0 : m_CurrentMenuSelection + 1;
                    selectOption(m_CurrentMenuSelection);
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    confirmOption(m_CurrentMenuSelection);
                }
            }
            else if (State == GameState.InGame)
            {
                for (int i = 0; i < World.Characters.Count; i++)
                {
                    var character = World.Characters[i];

                    if (keyInfo.Key == c_InputTable[i, 0])
                    {
                        character.TurnRight();
                    }
                    else if (keyInfo.Key == c_InputTable[i, 1])
                    {
                        character.TurnLeft();
                    }
                }

                if (keyInfo.Key == ConsoleKey.F1)
                {
                    World.BackgroundImageNode.IsActive = !m_OptimizedMode;
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    loadScene(enterMainMenu, delegate { });
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    if (IsFinished)
                    {
                        loadScene(enterMainMenu, delegate { });
                    }
                }
                else if (keyInfo.Key == ConsoleKey.F2)
                {
                    if (IsPaused)
                    {
                        Resume();
                    }
                    else
                    {
                        Pause();
                    }
                }
            }
            else if (State == GameState.Tutorial)
            {

            }
        }

        protected override void update(long timeStep)
        {
            if (State == GameState.Menu)
            {
                // TODO: maybe animation?
            }
            else if (State == GameState.InGame)
            {
                if (!IsFinished && !IsPaused)
                {
                    World.Update((float)timeStep / 1000.0f);
                }
            }
        }

        private void enterInGameScene()
        {
            State = GameState.InGame;
            IsFinished = false;
            IsPaused = false;

            // frame UI
            m_PlayGroundNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, c_GameWindowSize + Vector2Int.One), null, "Playground-Node");
            var playgroundCanvas = m_PlayGroundNode.AddUIComponent<SingleColorCanvas>();
            playgroundCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

            var playgroundLayoutNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, c_GameWindowSize + Vector2Int.One), m_PlayGroundNode, "PlaygroundLayout-Node");
            var layoutBitmap = playgroundLayoutNode.AddUIComponent<Bitmap>();
            layoutBitmap.LoadFromFile("./Assets/Layout.txt", Bitmap.DrawType.Sliced);

            // game world
            World = new World2D((Mode == GameMode.Single)? 1 : 2, new RectInt(Vector2Int.One, c_GameWindowSize));

            // register player dead event
            AliveCounter = new HashSet<int>();
            foreach (var character in World.Characters)
            {
                int id = character.Id;
                AliveCounter.Add(id);
                character.OnDie += () => handleOnPlayerDie(id);
            }

            // game UI
            RectInt gameUISize = new RectInt(new Vector2Int(0, c_GameWindowSize.Y + 1), new Vector2Int(c_GameWindowSize.X + 1, 5));

            var uiNode = UINode.Engine.Instance.CreateNode(gameUISize, null, "Game UI");
            var canvas = uiNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.White);

            RectInt panelBounds = new RectInt(new Vector2Int(0, 0), new Vector2Int(c_GameWindowSize.X + 1, 5));

            var layoutNode = UINode.Engine.Instance.CreateNode(panelBounds, uiNode, "Game UI");
            var layout = layoutNode.AddUIComponent<Bitmap>();
            layout.LoadFromFile("./Assets/Layout-Empty.txt", Bitmap.DrawType.Sliced);

            // register character hp , create hp ui
            m_HpTexts = new List<TextBox>();
            m_HpBarTexts = new List<TextBox>();
            for (int i = 0; i < World.Characters.Count; i++)
            {
                var character = World.Characters[i];
                character.OnHealthChanged += (int health) => handleOnCharacterHpChanged(character.Id, health);

                var textNode = UINode.Engine.Instance.CreateNode(new RectInt(3, 1 + i * 2, 10, 1), uiNode, "Game UI");
                var text = textNode.AddUIComponent<TextBox>();
                text.text = string.Format("P{0} HP: {1, 2}/{2, 2}", i + 1, 10, Character.c_MaxHealth);
                text.horizontalAlignment = TextBox.HorizontalAlignment.Left;
                text.verticalAlignment = TextBox.VerticalAlignment.Middle;

                m_HpTexts.Add(text);

                var hpBarCanvasNode = UINode.Engine.Instance.CreateNode(new RectInt(18, 1 + i * 2, 20, 1), uiNode, "Game UI");
                var hpBarCanvas = hpBarCanvasNode.AddUIComponent<SingleColorCanvas>();
                hpBarCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGreen, ConsoleColor.DarkGreen);

                var hpBarBackNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 20, 1), hpBarCanvasNode, "Game UI - Text");
                text = hpBarBackNode.AddUIComponent<TextBox>();
                text.text = new string("                    ");

                var hpBarInsideCanvasNode = UINode.Engine.Instance.CreateNode(new RectInt(18, 1 + i * 2, 20, 1), uiNode, "Game UI - Bar");
                var hpBarInsideCanvas = hpBarInsideCanvasNode.AddUIComponent<SingleColorCanvas>();
                hpBarInsideCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Yellow, ConsoleColor.Yellow);

                var actualInsideTextNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 20, 1), hpBarInsideCanvasNode, "Game UI - Bar -Text");
                text = actualInsideTextNode.AddUIComponent<TextBox>();
                text.text = new string("                    ");

                m_HpBarTexts.Add(text);
            }

            // add level ui
            var lvlUINode = UINode.Engine.Instance.CreateNode(new RectInt(39, 1, 10, 3), uiNode);
            m_LevelText = lvlUINode.AddUIComponent<TextBox>();
            m_LevelText.text = "LEVEL\n000";
            m_LevelText.horizontalAlignment = TextBox.HorizontalAlignment.Center;
            m_LevelText.verticalAlignment = TextBox.VerticalAlignment.Middle;

            World.OnTotalLevelChanged += handleOnTotalLevelChanged;

            // create Hint UI
            var hintNode = UINode.Engine.Instance.CreateNode(new RectInt(0, World.TowerTopNode.Bounds.Size.Y + 7, World.TowerTopNode.Bounds.Size.X, 1), null, "Hint-CanvasNode");
            canvas = hintNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkGray);

            var hintTextNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, World.TowerTopNode.Bounds.Size.X, 1), hintNode, "Hint-TextBoxNode");
            var hintTextBox = hintTextNode.AddUIComponent<TextBox>();
            hintTextBox.text = "F1: turn on/off optimized mode in game\nF2: pause/resume game";
            hintTextBox.verticalAlignment = TextBox.VerticalAlignment.Middle;
            hintTextBox.horizontalAlignment = TextBox.HorizontalAlignment.Center;

            // create GAME OVER UI
            var gameOverNode = UINode.Engine.Instance.CreateNode(new RectInt(0, World.TowerTopNode.Bounds.Size.Y / 2, World.TowerTopNode.Bounds.Size.X + 1, 5), null, "GO-CanvasNode");
            m_GameOverCanvas = gameOverNode.AddUIComponent<SingleColorCanvas>();
            m_GameOverCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Yellow, ConsoleColor.Black);

            var goTextNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, World.TowerTopNode.Bounds.Size.X + 1, 5), gameOverNode, "GO-TextBoxNode");
            var goUnlitBox = goTextNode.AddUIComponent<UnlitBox>();
            goUnlitBox.UnlitCharacter = ' ';
            m_GameOverText = goTextNode.AddUIComponent<TextBox>();
            m_GameOverText.text = "GAME OVER\n\npress enter to leave";
            m_GameOverText.verticalAlignment = TextBox.VerticalAlignment.Middle;
            m_GameOverText.horizontalAlignment = TextBox.HorizontalAlignment.Center;

            gameOverNode.IsActive = false;

            // Setup bg settings
            World.BackgroundImageNode.IsActive = !m_OptimizedMode;
        }

        private void enterMainMenu()
        {
            State = GameState.Menu;
            m_CurrentMenuSelection = 0;
            m_MenuOptionsTextBoxes = new List<TextBox>();

            // Title Image
            var titleShotNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 22, 25), null, "Tower-Node");
            var canvas = titleShotNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkRed);

            var imageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 22, 25), titleShotNode,"Tower-ImageNode");
            var image = imageNode.AddUIComponent<Bitmap>();
            image.LoadFromFile("./Assets/TitleTower.txt", Bitmap.DrawType.Normal);

            // Title Icon
            var titleIconNode = UINode.Engine.Instance.CreateNode(new RectInt(23, 2, 42, 5), null, "Icon-Node");
            canvas = titleIconNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.Red);

            imageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 42, 5), titleIconNode, "Icon-ImageNode");
            image = imageNode.AddUIComponent<Bitmap>();
            image.LoadFromFile("./Assets/Title.txt", Bitmap.DrawType.Normal);

            // create Button UI
            for (int i = 0; i < c_OptionTexts.Count; i++)
            {
                var text = c_OptionTexts[i];

                var btnNode = UINode.Engine.Instance.CreateNode(new RectInt(32, 11 + i * 2, 20, 1), null, text + "-CanvasNode");
                canvas = btnNode.AddUIComponent<SingleColorCanvas>();
                canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkYellow);

                var textNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 20, 1), btnNode, text + "-TextBoxNode");
                var textBox = textNode.AddUIComponent<TextBox>();
                textBox.text = text;

                m_MenuOptionsTextBoxes.Add(textBox);
            }

            // create Hint UI
            var hintNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 27, Console.WindowWidth, 1), null, "Hint-CanvasNode");
            canvas = hintNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkGray);

            var hintTextNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, Console.WindowWidth, 1), hintNode, "Hint-TextBoxNode");
            var hintTextBox = hintTextNode.AddUIComponent<TextBox>();
            hintTextBox.text = "ARROW KEYS: select, ENTER: confirm\nF1: turn on/off optimized mode in game\nF9: turn on/off debug mode";
            hintTextBox.verticalAlignment = TextBox.VerticalAlignment.Middle;
            hintTextBox.horizontalAlignment = TextBox.HorizontalAlignment.Center;

            selectOption(m_CurrentMenuSelection);
        }

        private void enterTutorial()
        {

        }

        #region MainMenu handler
        private void selectOption(int index)
        {
            // TODO: Update UI
            DYTA.Audio.AudioManager.Instance.BeepMusic(150, 100);
            for (int i = 0; i < m_MenuOptionsTextBoxes.Count; i++)
            {
                var textBox = m_MenuOptionsTextBoxes[i];
                if (i == index)
                {
                    var text = new StringBuilder(c_OptionTexts[i]);
                    text[0] = '-';
                    text[1] = '>';
                    textBox.text = text.ToString();

                    (textBox.Node.ParentCanvas as SingleColorCanvas).CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.Yellow);
                }
                else
                {
                    var text = new StringBuilder(c_OptionTexts[i]);
                    textBox.text = text.ToString();

                    (textBox.Node.ParentCanvas as SingleColorCanvas).CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.DarkYellow);
                }
            }
        }

        private void confirmOption(int index)
        {
            c_OptionDelegates[index].Invoke();
        }

        #endregion

        #region InGame handler

        private void Pause()
        {
            IsPaused = true;
            m_GameOverCanvas.Node.IsActive = true;
            m_GameOverText.text = string.Format("GAME PAUSED\n\npress F2 to resume");
        }

        private void Resume()
        {
            IsPaused = false;
            m_GameOverCanvas.Node.IsActive = false;
        }

        private void handleOnCharacterHpChanged(int id, int health)
        {
            if (health == 0)
            {
                m_HpTexts[id].text = string.Format("P{0} DECEASED", id+1, health, Character.c_MaxHealth);
                m_HpBarTexts[id].text = "";
            }
            else
            {
                m_HpTexts[id].text = string.Format("P{0} HP[{1, 2}/{2, 2}]", id+1, health, Character.c_MaxHealth);
                m_HpBarTexts[id].text = new string(' ', health * 2);
                m_HpBarTexts[id].Node.ParentCanvas.Node.SetSize(new Vector2Int(health * 2, 1));
                m_HpBarTexts[id].Node.SetSize(new Vector2Int(health * 2, 1));
            }
        }

        private void handleOnTotalLevelChanged(int level)
        {
            m_LevelText.text = string.Format("LEVEL\n{0:D3}", level);
        }

        private void handleOnPlayerDie(int id)
        {
            AliveCounter.Remove(id);
            switch (Mode)
            {
                case GameMode.Single:
                    if (AliveCounter.Count <= 0)
                    {
                        IsFinished = true;
                        m_GameOverCanvas.Node.IsActive = true;
                        m_GameOverText.text = string.Format("GAME OVER - SCORE: {0:D3}\n\npress enter to leave", World.TotalLevelCounter);
                    }
                    break;
                case GameMode.TwoPlayers:
                    if (AliveCounter.Count <= 0)
                    {
                        IsFinished = true;
                        m_GameOverCanvas.Node.IsActive = true;
                        m_GameOverText.text = string.Format("GAME OVER - SCORE: {0:D3}\n\npress enter to leave", World.TotalLevelCounter);
                    }
                    break;
                case GameMode.PvP:
                    if (AliveCounter.Count == 1)
                    {
                        int winnerId = -1;
                        foreach (var lastId in AliveCounter)
                        {
                            winnerId = lastId;
                        }

                        IsFinished = true;
                        m_GameOverCanvas.Node.IsActive = true;
                        m_GameOverCanvas.CanvasPixelColor = new PixelColor(Character.c_CharacterColors[winnerId], ConsoleColor.Black);
                        m_GameOverText.text = string.Format("GAME OVER - WINNER: PLAYER {0}\n\npress enter to leave", winnerId + 1);
                    }
                    break;
            }
        }

        #endregion
    }
}
