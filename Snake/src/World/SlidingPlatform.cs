using System;
using System.Collections.Generic;
using System.Text;
using DYTA.Math;

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

        private Dictionary<int, float> m_PlatformMoveTimer = new Dictionary<int, float>();

        private const float c_PlatformMoveDuration = 0.5f;

        public SlidingPlatform() { }

        protected override void onUpdate(float timeStep)
        {
            if (m_Direction == Direction.Left)
            {
                foreach (var character in OnTopCharacters)
                {
                    // TODO: move character
                }
            }
            else if (m_Direction == Direction.Right)
            {

            }
        }
    }
}
