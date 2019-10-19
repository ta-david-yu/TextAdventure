using System;
using System.Collections.Generic;
using System.Text;
using DYTA;
using DYTA.Math;
using DYTA.Render;

namespace Snake
{
    class SnakeApplication : ApplicationBase
    {
        enum GameState
        {
            Menu,
            InGame,
            GameOver
        }

        #region UIReference

        private UINode m_PlayGroundNode;


        #endregion

        #region GameVar

        private GameState State { get; set; }

        private World2D World { get; set; }

        #endregion

        private static readonly Vector2Int c_PlaygroundSize = new Vector2Int(50, 40);

        private static readonly ConsoleKey[,] c_InputTable = new ConsoleKey[,]
        {
            { ConsoleKey.D, ConsoleKey.A },
            { ConsoleKey.RightArrow, ConsoleKey.LeftArrow },
            { ConsoleKey.NumPad6, ConsoleKey.NumPad4 }
        };

        public SnakeApplication(Vector2Int windowSize, PixelColor color) : base(windowSize, color)
        {

        }

        protected override void loadInitialScene()
        {
            FrameLogger.Toggle();

            m_PlayGroundNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, c_PlaygroundSize + Vector2Int.One), null, "Playground-Node");
            var playgroundCanvas = m_PlayGroundNode.AddUIComponent<SingleColorCanvas>();
            playgroundCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.White);

            var playgroundLayoutNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, c_PlaygroundSize + Vector2Int.One), m_PlayGroundNode, "PlaygroundLayout-Node");
            var layoutBitmap = playgroundLayoutNode.AddUIComponent<Bitmap>();
            layoutBitmap.LoadFromFile("./Assets/Layout.txt", Bitmap.DrawType.Sliced);

            World = new World2D(1, new RectInt(Vector2Int.One, c_PlaygroundSize));
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
    }
}
