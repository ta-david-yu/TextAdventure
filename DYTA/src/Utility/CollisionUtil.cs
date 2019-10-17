using System;
using DYTA.Math;

namespace DYTA.Utility
{
    public static class CollisionUtil
    {
        public class Collision
        {
            public bool Left { get; set; } = false;
            public bool Right { get; set; } = false;
            public bool Top { get; set; } = false;
            public bool Bottom { get; set; } = false;
        }

        public static Collision ClampVelocity2D(RectInt body, RectInt otherBody, Vector2Int velocity, out Vector2Int clampedVelocity)
        {
            Collision collision = new Collision();

            // horizontal detection, raycast stepping
            Vector2Int outputVelocity = Vector2Int.Zero;
            int signX = (velocity.X > 0)? 1 : -1; 
            for (int x = 1; x <= signX * velocity.X; x++)
            {
                int step = x * signX;
                var stepPos = body.Position + new Vector2Int(step, 0);
                var stepBB = new RectInt(stepPos, body.Size);

                // has collision on side
                if (stepBB.Overlap(otherBody))
                {
                    if (signX == 1)
                    {
                        collision.Right = true;
                    }
                    else if (signX == -1)
                    {
                        collision.Left = true;
                    }
                    break;
                }
                outputVelocity.X += step;
            }

            // vertical detection, raycast stepping
            int signY = (velocity.Y > 0) ? 1 : -1;
            for (int y = 1; y <= signY * velocity.Y; y++)
            {
                int step = y * signY;
                var stepPos = body.Position + new Vector2Int(0, step);
                var stepBB = new RectInt(stepPos, body.Size);

                // has collision on side
                if (stepBB.Overlap(otherBody))
                {
                    if (signY == 1)
                    {
                        collision.Bottom = true;
                    }
                    else if (signY == -1)
                    {
                        collision.Top = true;
                    }
                    break;
                }
                outputVelocity.Y += step;
            }

            clampedVelocity = outputVelocity;

            return collision;
        }

        public static Collision CombineCollision(Collision col1, Collision col2)
        {
            col1.Left |= col2.Left;
            col1.Right |= col2.Right;
            col1.Top |= col2.Top;
            col1.Bottom |= col2.Bottom;

            return col1;
        }
    }
}
