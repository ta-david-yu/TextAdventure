using DYTA;
using DYTA.Math;
using DYTA.Render;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

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

        private List<Platform> m_Platforms = new List<Platform>();

        // UINode reference
        public UINode TowerTopNode { get; private set; }

        // Tower movement
        private float m_TowerMoveTimer = 0;

        private const float c_TowerMoveDuration = 0.25f;

        public World2D(int numOfPlayers, RectInt bounds)
        {
            TowerTopNode = UINode.Engine.Instance.CreateNode(bounds);

            int offset = bounds.Width / (numOfPlayers + 1);
            for (int i = 0; i < numOfPlayers; i++)
            {
                m_Characters.Add(new Character());
                var character = Characters[i];
                character.Initialize(i, this, new Vector2Int(offset * (i + 1), 0), (i % 2 == 0) ? CharacterDirection.Right : CharacterDirection.Left);
            }
        }

        public Platform CreatePlatform(Vector2Int pos, Vector2Int size)
        {
            var platform = new Platform();
            platform.Initialize(this, new RectInt(pos, size));
            m_Platforms.Add(platform);
            return platform;
        }

        public void Update(float timeStep)
        {
            
            m_TowerMoveTimer += timeStep;

            if (m_TowerMoveTimer > c_TowerMoveDuration)
            {
                TowerTopNode.Translate(new DYTA.Math.Vector2Int(0, -1));
                m_TowerMoveTimer = 0;
            }

            // update platform
            for (int i = 0; i < m_Platforms.Count; i++)
            {
                var platform = m_Platforms[i];
                platform.Update(timeStep);
            }

            // update velocity
            for (int i = 0; i < Characters.Count; i++)
            {
                var character = Characters[i];
                character.Update(timeStep);
            }

            // update movement, collision detection
            for (int i = 0; i < Characters.Count; i++)
            {
                var character = Characters[i];
                var velocity = character.Velocity;
                DYTA.Utility.CollisionUtil.Collision collision = new DYTA.Utility.CollisionUtil.Collision();

                // check with characters
                for (int j = 0; j < Characters.Count; j++)
                {
                    if (j != i)
                    {
                        var other = Characters[j];
                        var newCol = DYTA.Utility.CollisionUtil.ClampVelocity2D(character.Collider, other.Collider, velocity, out velocity);
                        collision = DYTA.Utility.CollisionUtil.CombineCollision(collision, newCol);

                        // check bottom
                        bool standOnThis = other.Collider.Contains(character.Collider.Max + new Vector2Int(0, 1));

                        character.IsOnGround = standOnThis;
                    }
                }
                
                // check with platforms
                for (int j = 0; j < m_Platforms.Count; j++)
                {
                    var other = m_Platforms[j];
                    var newCol = DYTA.Utility.CollisionUtil.ClampVelocity2D(character.Collider, other.Collider, velocity, out velocity);
                    collision = DYTA.Utility.CollisionUtil.CombineCollision(collision, newCol);

                    // check bottom
                    bool standOnThis = other.Collider.Contains(character.Collider.Max + new Vector2Int(0, 1));
                    
                    // invoke Enter / Exit event
                    if (standOnThis)
                    {
                        // not on this platform the previous frame
                        if (!other.IsCharacterOnThisPlatform(character))
                        {
                            other.OnCharacterEnter(character);
                        }
                    }
                    else
                    {
                        // on this platform the previous frame
                        if (other.IsCharacterOnThisPlatform(character))
                        {
                            other.OnCharacterExit(character);
                        }
                    }

                    character.IsOnGround = standOnThis;
                }

                // check with wall
                int signX = (velocity.X > 0) ? 1 : -1;
                int VecX = 0;
                for (int x = 1; x <= signX * velocity.X; x++)
                {
                    var body = character.Collider;
                    int step = x * signX;
                    var stepPos = body.Position + new Vector2Int(step, 0);

                    if (stepPos.X < 0)
                    {
                        collision.Left = true;
                        break;
                    }
                    else if (stepPos.X >= TowerTopNode.Bounds.Width - 1)
                    {
                        collision.Right = true;
                        break;
                    }

                    VecX += step;
                }
                velocity.X = VecX;

                // move
                var newPos = character.Collider.Position + velocity;
                character.SetPosition(newPos);

                //FrameLogger.Log(velocity + newPos.ToString());
            }
        }
    }
}
