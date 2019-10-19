using DYTA;
using DYTA.Audio;
using DYTA.Math;
using DYTA.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSShaft
{
    public enum CharacterDirection
    {
        Right,
        Left,
    }

    public class Character : HasCollision
    {
        // Data
        public int Id { get; private set; }

        public int Health { get; private set; }

        public bool IsDead { get; private set; }

        // Status
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

        // callback
        public event Action<int> OnHealthChanged = delegate { };
        public event Action OnDie = delegate { };

        // Const
        private string[] m_RightBitmap;
        private string[] m_LeftBitmap;
        private string[] m_DeathBitmap;

        private const float c_MoveDuration = 0.001f;
        private const float c_GravityTimePerPixel = 0.1f;

        private const int c_MaxSpeedY = 1;

        public const int c_MaxHealth = 10;

        private static readonly ConsoleColor[] c_CharacterColors = new ConsoleColor[]{ ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.DarkCyan };

        public Character() { }

        public void Initialize(int id, World2D world, Vector2Int pos, CharacterDirection dir)
        {
            Id = id;
            Health = c_MaxHealth;

            m_World = world;
            Collider = new RectInt(pos, new Vector2Int(1, 2));
            Direction = dir;

            RenderNode = UINode.Engine.Instance.CreateNode(Collider, world.CharacterNode, "Character-Node");
            var canvas = RenderNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, c_CharacterColors[id]);

            var imageNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 1, 2), RenderNode, "Character-Image");
            Image = imageNode.AddUIComponent<Bitmap>();
            m_RightBitmap = System.IO.File.ReadAllLines("./Assets/CharacterRight.txt");
            m_LeftBitmap = System.IO.File.ReadAllLines("./Assets/CharacterLeft.txt");
            m_DeathBitmap = System.IO.File.ReadAllLines("./Assets/CharacterDeath.txt");

            Image.Load((dir == CharacterDirection.Right) ? m_RightBitmap : m_LeftBitmap);
        }

        public void TurnLeft()
        {
            if (!IsActive || IsDead)
            {
                return;
            }

            Direction = CharacterDirection.Left;
            m_MoveTimer = c_MoveDuration;
            Image.Load(m_LeftBitmap);
        }

        public void TurnRight()
        {
            if (!IsActive || IsDead)
            {
                return;
            }

            Direction = CharacterDirection.Right;
            m_MoveTimer = c_MoveDuration;
            Image.Load(m_RightBitmap);
        }

        public void Update(float timeStep)
        {
            if (!IsActive || IsDead)
            {
                Velocity = Vector2Int.Zero;
                return;
            }

            m_MoveTimer += timeStep;

            if (IsOnGround)
            {
                m_GravityTimer = 0;
                Velocity = new Vector2Int(Velocity.X, 0);
            }
            else
            {
                m_GravityTimer += timeStep;
            }


            int speedY = Velocity.Y;
            if (speedY > c_MaxSpeedY) speedY = c_MaxSpeedY;
            Velocity = new Vector2Int(0, speedY);
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

            FrameLogger.Log(string.Format("{0}- HP: [{1}], OnGnd: [{2}]", Id, Health, IsOnGround));
        }

        public int AddHealth(int value)
        {
            if (IsDead)
                return 0;

            Health += value;
            if (Health >= c_MaxHealth)
            {
                Health = c_MaxHealth;
            }

            if (Health <= 0)
            {
                Health = 0;
                IsDead = true;
            }

            OnHealthChanged.Invoke(Health);

            if (IsDead)
            {
                Image.Load(m_DeathBitmap);
                OnDie.Invoke();
            }

            return Health;
        }

        

        public void SetPosition(Vector2Int position)
        {
            Collider = new RectInt(position, Collider.Size);
            RenderNode.SetPosition(position);
        }

        protected override void onIsActiveChanged(bool value)
        {
            RenderNode.IsActive = value;
        }

        public void OnLeavePlatform(Platform plat)
        {
        }

        public void OnStepOnPlatform(Platform plat)
        {
            AudioManager.Instance.BeepMusic(150 + Health * 40, 100);
        }
    }
}
