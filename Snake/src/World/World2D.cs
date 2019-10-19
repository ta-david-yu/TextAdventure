using DYTA;
using DYTA.Audio;
using DYTA.Math;
using DYTA.Render;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static DYTA.Utility.CollisionUtil;

namespace NSShaft
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

        private List<HasCollision> m_StaticObjects = new List<HasCollision>();

        private List<Platform> m_AllPlatforms = new List<Platform>();
        private List<Platform> m_NormalPlatforms = new List<Platform>();
        private List<Platform> m_SpikePlatforms = new List<Platform>();

        private List<HasCollision> m_AllColliders = new List<HasCollision>();

        // UINode reference
        public UINode WorldStaticNode { get; private set; }

        public UINode TowerTopNode { get; private set; }

        public UINode CharacterNode { get; private set; }

        public UINode PlatformNode { get; private set; }

        // Settings
        private Vector2Int m_TowerSize;

        // Tower movement
        private int m_Difficulty = 0;
        public int Difficulty 
        {
            get
            {
                return m_Difficulty;
            }

            private set
            {
                var prev = m_Difficulty;
                m_Difficulty = value;

                if (value != prev)
                {
                    OnDifficultyChanged.Invoke(value);
                }
            }
        }

        private int m_TotalLevelCounter = 0;
        public int TotalLevelCounter
        {
            get
            {
                return m_TotalLevelCounter;
            }

            private set
            {
                var prev = m_TotalLevelCounter;
                m_TotalLevelCounter = value;

                if (value != prev)
                {
                    OnTotalLevelChanged.Invoke(value);
                }
            }
        }

        private float m_TowerMoveTimer = 0;
        private float m_TowerMoveDuration = c_SlowestTowerMoveDuration;
        private int m_LevelCounter = 0;

        private Platform m_PreviousPlatform;
        private int m_SpawnPlatformCounter = 0;

        private Random m_RandomGenerator;
        private int m_SpikeSpawnThreshold = c_SpikeSpawnInitialThreshold;

        //
        private const float c_SlowestTowerMoveDuration = 0.35f;
        private const float c_FastestTowerMoveDuration = 0.15f;
        private const float c_AddUpPer10Level = 0.025f;
        private const float c_MaxDifficulty = (c_SlowestTowerMoveDuration - c_FastestTowerMoveDuration) / c_AddUpPer10Level;

        private const int c_PlayerInitialHeight = 21;
        private const int c_PlatformInitialHeight = c_PlayerInitialHeight + 2;

        private const int c_PlatformWidth = 16;
        private const int c_PlatformOffsetY = 5;

        private const int c_SpikeSpawnInitialThreshold = 9;

        // event
        public event Action<int> OnTotalLevelChanged = delegate { };
        public event Action<int> OnDifficultyChanged = delegate { };

        public World2D(int numOfPlayers, RectInt bounds)
        {
            m_RandomGenerator = new Random(DateTime.Now.Millisecond);

            m_TowerSize = bounds.Size;

            TowerTopNode = UINode.Engine.Instance.CreateNode(bounds, null, "TowerRoot");
            WorldStaticNode = UINode.Engine.Instance.CreateNode(bounds, null, "StaticNode");
            CharacterNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 1, 1), TowerTopNode, "CharacterNode");
            PlatformNode = UINode.Engine.Instance.CreateNode(new RectInt(0, 0, 1, 1), TowerTopNode, "PlatformNode");

            // create death zone
            CreateDeathZone(new Vector2Int(0, 0), new Vector2Int(m_TowerSize.X - 1, 1), true);

            // create wall, left and right
            CreateStaticWall(new Vector2Int(0, 0), new Vector2Int(1, m_TowerSize.Y - 1));
            CreateStaticWall(new Vector2Int(m_TowerSize.X - 2, 0), new Vector2Int(1, m_TowerSize.Y - 1));

            // create character
            int offset = bounds.Width / 3;
            for (int i = numOfPlayers - 1; i >= 0; i--)
            {
                var character = CreateCharacter(new Vector2Int(offset * (i + 1),
                    c_PlayerInitialHeight),
                    (i % 2 == 0) ? CharacterDirection.Left : CharacterDirection.Right);

                character.OnDie += () => handleOnPlayerDie(character.Id);
            }

            // create initial platform
            int initialPlatWidth = 21;
            int platGap = 6;
            for (int i = 0; i < 2; i++)
            {
                var platformPos = new Vector2Int(i * (platGap + initialPlatWidth), c_PlatformInitialHeight);
                var platformSize = new Vector2Int(initialPlatWidth, 1);
                CreateNormalPlatform(platformPos, platformSize);
            }
        }

        public StaticWall CreateStaticWall(Vector2Int pos, Vector2Int size)
        {
            var wall = new StaticWall();
            wall.Initialize(this, new RectInt(pos, size));
            m_StaticObjects.Add(wall);
            m_AllColliders.Add(wall);
            return wall;
        }

        public DeathZone CreateDeathZone(Vector2Int pos, Vector2Int size, bool withGraphics)
        {
            var deathZone = new DeathZone();
            deathZone.Initialize(this, new RectInt(pos, size), withGraphics);
            m_StaticObjects.Add(deathZone);
            m_AllColliders.Add(deathZone);
            return deathZone;
        }

        public Platform CreateNormalPlatform(Vector2Int pos, Vector2Int size)
        {
            Platform platform = null;

            // find inactive platform in the list
            foreach (var plat in m_NormalPlatforms)
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

                m_NormalPlatforms.Add(platform);
                m_AllPlatforms.Add(platform);
                m_AllColliders.Add(platform);
            }

            return platform;
        }

        public Platform CreateSpikePlatform(Vector2Int pos, Vector2Int size)
        {
            Platform platform = null;

            // find inactive platform in the list
            foreach (var plat in m_SpikePlatforms)
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
                platform = new SpikePlatform();
                platform.Initialize(this, new RectInt(pos, size));

                m_SpikePlatforms.Add(platform);
                m_AllPlatforms.Add(platform);
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
            if (m_TowerMoveTimer > m_TowerMoveDuration)
            {
                TowerTopNode.Translate(new DYTA.Math.Vector2Int(0, -1));
                m_TowerMoveTimer = 0;

                // recycle
                for (int i = 0; i < m_AllPlatforms.Count; i++)
                {
                    var platform = m_AllPlatforms[i];
                    if (TowerTopNode.Bounds.Position.Y - 1 + platform.Collider.Position.Y < 0)
                    {
                        platform.IsActive = false;
                    }
                }

                m_SpawnPlatformCounter++;
                if (m_SpawnPlatformCounter >= c_PlatformOffsetY)
                {
                    m_SpawnPlatformCounter = 0;
                    if (m_PreviousPlatform == null)
                    {
                        m_PreviousPlatform =
                            CreateNormalPlatform(
                                new Vector2Int(m_TowerSize.X / 2 - c_PlatformWidth / 2 - 1, c_PlatformInitialHeight + c_PlatformOffsetY),
                                new Vector2Int(c_PlatformWidth, 1));
                    }
                    else
                    {
                        var pos = m_PreviousPlatform.Collider.Position;
                        pos.Y += c_PlatformOffsetY;
                        pos.X = m_RandomGenerator.Next(0, m_TowerSize.X - c_PlatformWidth);

                        int rndToken = m_RandomGenerator.Next(0, 10);

                        if (rndToken < m_SpikeSpawnThreshold)
                        {
                            m_PreviousPlatform = CreateNormalPlatform(pos, new Vector2Int(c_PlatformWidth, 1));
                            m_SpikeSpawnThreshold -= 2;
                        }
                        else
                        {
                            m_PreviousPlatform = CreateSpikePlatform(pos, new Vector2Int(c_PlatformWidth, 1));
                            m_SpikeSpawnThreshold += 3;
                            if (m_SpikeSpawnThreshold > c_SpikeSpawnInitialThreshold)
                            {
                                m_SpikeSpawnThreshold = c_SpikeSpawnInitialThreshold;
                            }
                        }
                    }

                    m_LevelCounter++;
                    TotalLevelCounter++;
                    if (m_LevelCounter >= 10)
                    {
                        m_LevelCounter = 0;
                        Difficulty++;
                        if (Difficulty < c_MaxDifficulty)
                        {
                            m_TowerMoveDuration -= c_AddUpPer10Level;
                        }
                    }
                }
            }

            FrameLogger.Log("Difficulty - " + Difficulty);
            FrameLogger.Log("TotalLevelCounter - " + TotalLevelCounter);
            FrameLogger.Log("SpikeThreshold - " + m_SpikeSpawnThreshold);

            // update platform
            for (int i = 0; i < m_AllPlatforms.Count; i++)
            {
                var platform = m_AllPlatforms[i];
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
                        // static object
                        if (collider is StaticWall || collider is DeathZone)
                        {
                            var bounds = new RectInt(collider.Collider.Position + new Vector2Int(0, -(TowerTopNode.Bounds.Position.Y - 1)), collider.Collider.Size);
                            var col = ClampVelocity2D(character.Collider, bounds, velocity, out velocity);

                            if (col.IsTrue)
                            {
                                FrameLogger.Log("TYPE " + collider.GetType());
                                collider.OnCharacterCollision(character);
                            }
                        }
                        // dynamic object
                        else
                        {
                            var col = ClampVelocity2D(character.Collider, collider.Collider, velocity, out velocity);

                            if (col.IsTrue)
                            {
                                FrameLogger.Log("TYPE " + collider.GetType());
                                collider.OnCharacterCollision(character);
                            }
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

                // falling out of the zone
                if (character.Collider.Position.Y + TowerTopNode.Bounds.Position.Y + 1 > m_TowerSize.Y)
                {
                    character.AddHealth(-999);
                    character.IsActive = false;
                }

                if (character.Collider.Position.Y + TowerTopNode.Bounds.Position.Y <= 0)
                {
                    character.IsActive = false;
                }

                //FrameLogger.Log(velocity + newPos.ToString());
            }
        }

        private void handleOnPlayerDie(int id)
        {
            //Characters[id].IsActive = false;
            AudioManager.Instance.BeepMusic(250, 100);
            AudioManager.Instance.BeepMusic(150, 100);
        }
    }
}
