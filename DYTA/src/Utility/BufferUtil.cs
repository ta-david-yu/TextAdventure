using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DYTA.Math;
using DYTA.Render;

namespace DYTA.Utility
{
    static class BufferUtil
    {
        internal static IReadOnlyCollection<Vector2Int> CompareBuffers(Pixel[,] left, Pixel[,] right)
        {
            ConcurrentBag<Vector2Int> delta = new ConcurrentBag<Vector2Int>();
            Parallel.For(0, left.GetLength(1), (y) =>
            {
                for (int x = 0, x_size = left.GetLength(0); x < x_size; x++)
                {
                    if (left[x, y] != right[x, y])
                    {
                        delta.Add(new Vector2Int(x, y));
                    }
                }
            });
            return delta;
        }

        internal static void ClearBuffer(Pixel[,] buffer, PixelColor color)
        {
            Pixel emptyCell = new Pixel(' ', color);
            for (int y = 0; y < buffer.GetLength(1); y++)
            {
                for (int x = 0; x < buffer.GetLength(0); x++)
                {
                    buffer[x, y] = emptyCell;
                }
            }
        }
    }
}
