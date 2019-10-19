using DYTA;
using DYTA.Math;
using DYTA.Render;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static DYTA.Utility.CollisionUtil;

namespace Snake
{
    public enum GameMode
    {
        OnePlayer,
        TwoPlayer
    }

    public class World2D
    {
        public GameMode Mode { get; private set; }

        private List<Character> m_Characters = new List<Character>();
        public ReadOnlyCollection<Character> Characters { get { return m_Characters.AsReadOnly(); } }

        private List<StaticWall> m_StaticWalls = new List<StaticWall>();

        private List<Platform> m_Platforms = new List<Platform>();

        private List<HasCollision> m_AllColliders = new List<HasCollision>();

        // UINode reference
        public UINode WorldStaticNode { get; private set; }

        public UINode TowerTopNode { get; private set; }

        public UINode CharacterNode { get; private set; }

        public UINode PlatformNode { get; private set; }

        // Settings
        private Vector2Int m_TowerSize;

        // Tower movement
        private float m_TowerMoveTimer = 0;

        //
        private const float c_SlowestTowerMoveDuration = 0.35f;
        private const float c_FastestTowerMoveDuration = 0.15f;

        private const int c_PlayerInitialHeight = 15;
        private const int c_PlatformWidth = 15;

        public World2D(int numOfPlayers, RectInt bounds)
        {
            m_TowerSize = bounds.Size;

            TowerTopNode = UINode.Engine.Instance.CreateNode(bounds, null, "TowerRoot");
            WorldStaticNode = UINode.Engine.Instance.CreateNode(bounds, null, "StaticNode");
            CharacterNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 1, 1), TowerTopNode, "CharacterNode");
            PlatformNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 1, 1), TowerTopNode, "PlatformNode");

            // create wall, left and right
            CreateStaticWall(new Vector2Int(0, 0), new Vector2Int(1, m_TowerSize.Y - 1));
            CreateStaticWall(new Vector2Int(m_TowerSize.X - 2, 0), new Vector2Int(1, m_TowerSize.Y - 1));

            // create character
            int offset = bounds.Width / (numOfPlayers + 1);
            for (int i = 0; i < numOfPlayers; i++)
            {
                var character = CreateCharacter(new Vector2Int(offset * (i + 1),
                    c_PlayerInitialHeight),
                    (i % 2 == 0) ? CharacterDirection.Right : CharacterDirection.Left);
            }

            // create initial platform
            int initialPlatWidth = 12;
            int platGap = 6;
            for (int i = 0; i < 3; i++)
            {
                var platformPos = new Vector2Int(i * (platGap + initialPlatWidth), c_PlayerInitialHeight + 2);
                var platformSize = new Vector2Int(initialPlatWidth, 1);
                CreatePlatform(platformPos, platformSize);
            }
        }

        public StaticWall CreateStaticWall(Vector2Int pos, Vector2Int size)
        {
            var wall = new StaticWall();
            wall.Initialize(this, new RectInt(pos, size));
            m_StaticWalls.Add(wall);
            m_AllColliders.Add(wall);
            return wall;
        }

        public Platform CreatePlatform(Vector2Int pos, Vector2Int size)
        {
            Platform platform = null;

            // find inactive platform in the list
            foreach (var plat in m_Platforms)
            {
                if (!plat.IsActive)
                {
                    platform = plat;
                    platform.SetPositionAndSize(pos, size);
                    platform.IsActive = true;
                    break;
                }
            }

            if (platform == null)
            {

                platform = new Platform();
                platform.Initialize(this, new RectInt(pos, size));

                m_Platforms.Add(platform);
                m_AllColliders.Add(platform);
            }

            return platform;
        }

        public Character CreateCharacter(Vector2Int pos, CharacterDirection dir)
        {
            var character = new Character();
            character.Initialize(m_Characters.Count, this, pos, dir);
            m_Characters.Add(character);
            m_AllColliders.Add(character);
            return character;
        }

        public void Update(float timeStep)
        {
            m_TowerMoveTimer += timeStep;

            // move tower, recycle platforms, generate new platforms
            if (m_TowerMoveTimer > c_SlowestTowerMoveDuration)
            {
                TowerTopNode.Translate(new DYTA.Math.Vector2Int(0, -1));
                m_TowerMoveTimer = 0;

                // recycle
                for (int i = 0; i < m_Platforms.Count; i++)
                {
                    var platform = m_Platforms[i];
                    if (TowerTopNode.Bounds.Position.Y + platform.Collider.Position.Y < 0)
                    {
                        platform.IsActive = false;
                    }
                }
            }

            // update platform
            for (int i = 0; i < m_Platforms.Count; i++)
            {
                var platform = m_Platforms[i];
                platform.Update(timeStep);
            }

            // update character
            for (int i = 0; i < Characters.Count; i++)
            {
                var character = Characters[i];
                character.Update(timeStep);
            }

            // update character movement, collision detection
            for (int i = 0; i < Characters.Count; i++)
            {
                var character = Characters[i];
                var velocity = character.Velocity;

                // check velocity collision
                for (int j = 0; j < m_AllColliders.Count; j++)
                {
                    var collider = m_AllColliders[j];

                    if (!collider.IsActive)
                        continue;

                    if (collider != character)
                    {
                        if (collider is StaticWall)
                        {
                            var bounds = new RectInt(collider.Collider.Position + new Vector2Int(0, -TowerTopNode.Bounds.Position.Y), collider.Collider.Size);
                            ClampVelocity2D(character.Collider, bounds, velocity, out velocity);
                        }
                        else
                        {
                            ClampVelocity2D(character.Collider, collider.Collider, velocity, out velocity);
                        }
                    }
                }

                // move
                var newPos = character.Collider.Position + velocity;
                character.SetPosition(newPos);

                // ground checking after movement
                character.IsOnGround = false;
                var bottomBox = new RectInt(character.Collider.Min.X, character.Collider.Max.Y + 1, character.Collider.Width, 1);
                for (int j = 0; j < m_AllColliders.Count; j++)
                {
                    var collider = m_AllColliders[j];

                    if (!collider.IsActive)
                    {
                        continue;
                    }

                    // check bottom
                    bool standOnThis = collider.Collider.Overlap(bottomBox);

                    // invoke Enter / Exit event
                    if (standOnThis)
                    {
                        // not on this platform the previous frame
                        if (!collider.IsCharacterOnThisCollider(character))
                        {
                            collider.OnCharacterStepOn(character);
                        }
                    }
                    else
                    {
                        // on this platform the previous frame
                        if (collider.IsCharacterOnThisCollider(character))
                        {
                            collider.OnCharacterLiftOff(character);
                        }
                    }

                    character.IsOnGround |= standOnThis;
                }


                //FrameLogger.Log(velocity + newPos.ToString());
            }
        }
    }
}
