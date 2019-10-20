using System;
using System.Collections.Generic;
using System.Text;
using DYTA;
using DYTA.Math;
using DYTA.Render;

namespace NSShaft
{
    class SlidingPlatform : Platform
    {
        public enum Direction
        {
            Right,
            Left
        }

        private Direction m_Direction = Direction.Right;

        private List<KeyValuePair<int, float>> m_PlatformMoveTimers = new List<KeyValuePair<int, float>>();

        private const float c_PlatformMoveDuration = 0.12f;

        public SlidingPlatform() { }

        public override void Initialize(World2D world, RectInt collider)
        {
            m_Direction = (Direction)new Random(DateTime.Now.Millisecond).Next(0, 2);
            m_PlatformMoveTimers = new List<KeyValuePair<int, float>>();

            Collider = collider;
            World = world;

            RenderNode = UINode.Engine.Instance.CreateNode(Collider, World.PlatformNode, "Platform-Node");
            var canvas = RenderNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.Blue);

            var imageNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, Collider.Size), RenderNode, "Platform-Image");
            Image = imageNode.AddUIComponent<Bitmap>();
            Image.LoadFromFile((m_Direction == Direction.Left)? "./Assets/SlidingPlatformLeft.txt" : "./Assets/SlidingPlatformRight.txt");
        }

        protected override void onUpdate(float timeStep)
        {
            if (m_Direction == Direction.Left)
            {
                for (int i = 0; i < m_PlatformMoveTimers.Count; i++)
                {
                    var pair = m_PlatformMoveTimers[i];
                    var timer = pair.Value + timeStep;

                    if (timer > c_PlatformMoveDuration)
                    {
                        FrameLogger.LogError("MOVE LEFT");
                        OnTopCharacters[pair.Key].Velocity += Vector2Int.Right * -1;
                        timer = 0;
                    }
                    m_PlatformMoveTimers[i] = new KeyValuePair<int, float>(pair.Key, timer);
                }
            }
            else if (m_Direction == Direction.Right)
            {
                for (int i = 0; i < m_PlatformMoveTimers.Count; i++)
                {
                    var pair = m_PlatformMoveTimers[i];
                    var timer = pair.Value + timeStep;

                    if (timer > c_PlatformMoveDuration)
                    {
                        FrameLogger.LogError("MOVE RIGHT");
                        OnTopCharacters[pair.Key].Velocity += Vector2Int.Right * 1;
                        timer = 0;
                    }
                    m_PlatformMoveTimers[i] = new KeyValuePair<int, float>(pair.Key, timer);
                }
            }
        }

        public override void OnCharacterStepOn(Character ch)
        {
            base.OnCharacterStepOn(ch);
            m_PlatformMoveTimers.Add(new KeyValuePair<int, float>(ch.Id, 0));
        }

        public override void OnCharacterLiftOff(Character ch)
        {
            base.OnCharacterLiftOff(ch);

            for (int i = 0; i < m_PlatformMoveTimers.Count; i++)
            {
                if (m_PlatformMoveTimers[i].Key == ch.Id)
                {
                    m_PlatformMoveTimers.RemoveAt(i);
                    break;
                }
            }
        }

        protected override void onIsActiveChanged(bool value)
        {
            base.onIsActiveChanged(value);

            if (!value)
            {
                m_PlatformMoveTimers = new List<KeyValuePair<int, float>>();

                m_Direction = (Direction)new Random(DateTime.Now.Millisecond).Next(0, 2);
                Image.LoadFromFile((m_Direction == Direction.Left) ? "./Assets/SlidingPlatformLeft.txt" : "./Assets/SlidingPlatformRight.txt");
            }
        }
    }
}
