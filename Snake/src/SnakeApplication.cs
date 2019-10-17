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
        private UINode m_CharacterNode;


        #endregion

        #region GameVar

        private GameState State { get; set; }

        private List<Vector2Int> m_SnakeBodies = new List<Vector2Int>();

        private Vector2Int m_CharacterPosition = new Vector2Int(0, c_PlaygroundSize.Y - 1);

        #endregion

        private static readonly Vector2Int c_PlaygroundSize = new Vector2Int(20, 20);



        public SnakeApplication(RectInt bounds, PixelColor color) : base(bounds, color)
        {

        }

        protected override void handleOnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            if(keyInfo.Key == ConsoleKey.RightArrow)
            {
                m_CharacterPosition += new Vector2Int(1, 0);
            }
            else if(keyInfo.Key == ConsoleKey.LeftArrow)
            {
                m_CharacterPosition += new Vector2Int(-1, 0);
            }
            else if (keyInfo.Key == ConsoleKey.Spacebar)
            {
                m_CharacterPosition += new Vector2Int(0, -5);
            }

            m_CharacterNode.SetPosition(m_CharacterPosition);
        }

        protected override void loadInitialScene()
        {
            m_PlayGroundNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, c_PlaygroundSize + Vector2Int.One), null, "Playground-Node");
            var playgroundCanvas = m_PlayGroundNode.AddUIComponent<SingleColorCanvas>();
            playgroundCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.DarkGray, ConsoleColor.White);

            var playgroundLayoutNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, c_PlaygroundSize + Vector2Int.One), m_PlayGroundNode, "PlaygroundLayout-Node");
            var layoutBitmap = playgroundLayoutNode.AddUIComponent<Bitmap>();
            layoutBitmap.LoadFromFile("./Assets/Layout.txt", Bitmap.DrawType.Sliced);

            var movableAreaNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.One, c_PlaygroundSize), m_PlayGroundNode, "Movable-Area-Node");

            m_CharacterNode = UINode.Engine.Instance.CreateNode(new RectInt(m_CharacterPosition, Vector2Int.One), movableAreaNode, "Character");
            var snakeCanvas = m_CharacterNode.AddUIComponent<SingleColorCanvas>();
            snakeCanvas.CanvasPixelColor = new PixelColor(ConsoleColor.Yellow, ConsoleColor.White);

            var snakeImgNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 1, 1), m_CharacterNode);
            var snakeImg = snakeImgNode.AddUIComponent<TextBox>();
            snakeImg.text = " ";

            m_SnakeBodies = new List<Vector2Int>();
            m_SnakeBodies.Add(new Vector2Int(5, 5));
        }

        protected override void update(long timeStep)
        {

        }
    }
}
