using System;
using System.Collections.Generic;
using System.Text;
using DYTA.Math;

namespace Snake
{
    class SlidingPlatform : Platform
    {
        public enum Direction
        {
            Right,
            Left
        }

        private Direction m_Direction = Direction.Right;

        private Dictionary<int, float> m_PlatformMoveTimer = new Dictionary<int, float>();

        private const float c_PlatformMoveDuration = 0.5f;

        public SlidingPlatform() { }

        public override void Update(float timeStep)
        {
            base.Update(timeStep);

            if (m_Direction == Direction.Left)
            {
                foreach (var character in m_Characters)
                {
                    // TODO: move character
                }
            }
            else if (m_Direction == Direction.Right)
            {

            }
        }

        public override void OnCharacterEnter(Character ch)
        {
            base.OnCharacterEnter(ch);

        }

        public override void OnCharacterExit(Character ch)
        {
            base.OnCharacterExit(ch);
        }
    }
}
