using DYTA;
using DYTA.Math;
using DYTA.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    public enum CharacterDirection
    {
        Right,
        Left,
    }

    public class Character : IHasCollider
    {
        public int Id { get; private set; }

        // Status
        public RectInt Collider { get; private set; }

        public CharacterDirection Direction { get; private set; }

        public Vector2Int Velocity { get; set; }

        public bool UseGravity { get; set; } = true;

        public bool IsOnGround { get; set; } = false;

        // Timer
        private float m_MoveTimer = 0.0f;
        private float m_GravityTimer = 0.0f;

        //
        private World2D m_World;

        // UI Refernce
        public UINode RenderNode { get; private set; }

        public Bitmap Image { get; private set; }

        // Const
        private string[] m_RightBitmap;
        private string[] m_LeftBitmap;

        private const float c_MoveDuration = 0.015f;
        private const float c_GravityTimePerPixel = 0.02f;

        public Character() { }

        public void Initialize(int id, World2D world, Vector2Int pos, CharacterDirection dir)
        {
            Id = id;

            m_World = world;
            Collider = new RectInt(pos, new Vector2Int(1, 2));
            Direction = dir;

            RenderNode = UINode.Engine.Instance.CreateNode(Collider, world.TowerTopNode, "Character-Node");
            var canvas = RenderNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.Yellow);

            var imageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 1, 2), RenderNode, "Character-Image");
            Image = imageNode.AddUIComponent<Bitmap>();
            m_RightBitmap = System.IO.File.ReadAllLines("./Assets/CharacterRight.txt");
            m_LeftBitmap = System.IO.File.ReadAllLines("./Assets/CharacterLeft.txt");

            Image.Load((dir == CharacterDirection.Right) ? m_RightBitmap : m_LeftBitmap);
        }

        public void TurnLeft()
        {
            Direction = CharacterDirection.Left;
            m_MoveTimer = c_MoveDuration;
            Image.Load(m_LeftBitmap);
        }

        public void TurnRight()
        {
            Direction = CharacterDirection.Right;
            m_MoveTimer = c_MoveDuration;
            Image.Load(m_RightBitmap);
        }

        public void Update(float timeStep)
        {
            //FrameLogger.Log(timeStep.ToString());

            m_MoveTimer += timeStep;

            // TODO, reset Timer  on ground
            m_GravityTimer += timeStep;

            Velocity = Vector2Int.Zero;
            if (m_MoveTimer > c_MoveDuration)
            {
                Velocity += new Vector2Int(Direction == CharacterDirection.Right ? 1 : -1, 0);
                m_MoveTimer = 0;
            }

            if (m_GravityTimer > c_GravityTimePerPixel)
            {
                Velocity += new Vector2Int(0, 1);
                m_GravityTimer = 0;
            }
            //FrameLogger.Log(Velocity.ToString());
        }

        public void SetPosition(Vector2Int position)
        {
            Collider = new RectInt(position, Collider.Size);
            RenderNode.SetPosition(position);
        }
    }
}
