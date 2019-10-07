using System;
using System.Collections.Generic;
using System.Text;

using DYTA.Math;

namespace DYTA.Render
{
    public abstract class Canvas
    {
        public RectInt CanvasBounds { get; set; }

        // draw on the cmd line
        public abstract void Render();

        // reset buffer value and color
        public abstract void ResetBuffer();
    }
}
