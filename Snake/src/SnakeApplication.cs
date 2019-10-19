using System;
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
            GameOver
        }

        #region GameVar

        private GameState State { get; set; }

        private World2D World { get; set; }



        #endregion

        #region GameUI

        private UINode m_PlayGroundNode;
        private List<TextBox> m_HpTexts;
        private List<TextBox> m_HpBarTexts;
        private TextBox m_LevelText;

        #endregion

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
            FrameLogger.Toggle();

            // frame UI
            m_PlayGroundNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, c_GameWindowSize + Vector2Int.One), null, "Playground-Node");
            var playgroundCanvas = m_PlayGroundNode.AddUIComponent<SingleColorCanvas>();
            playgroundCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

            var playgroundLayoutNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, c_GameWindowSize + Vector2Int.One), m_PlayGroundNode, "PlaygroundLayout-Node");
            var layoutBitmap = playgroundLayoutNode.AddUIComponent<Bitmap>();
            layoutBitmap.LoadFromFile("./Assets/Layout.txt", Bitmap.DrawType.Sliced);

            // game world
            World = new World2D(2, new RectInt(Vector2Int.One, c_GameWindowSize));

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
                text.text = string.Format("P{0} HP: {1, 2}/{2, 2}", i+1,  10, Character.c_MaxHealth);
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
            m_LevelText.text = "LEVEL\n0";
            m_LevelText.horizontalAlignment = TextBox.HorizontalAlignment.Center;
            m_LevelText.verticalAlignment = TextBox.VerticalAlignment.Middle;

            World.OnTotalLevelChanged += handleOnTotalLevelChanged;

        }

        protected override void handleOnKeyPressed(ConsoleKeyInfo keyInfo)
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

            if (keyInfo.Key == ConsoleKey.Escape)
            {
                loadScene(loadInitialScene, delegate { });
            }
        }

        protected override void update(long timeStep)
        {
            World.Update((float)timeStep / 1000.0f);
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
            m_LevelText.text = string.Format("LEVEL\n{0}", level);
        }
    }
}
