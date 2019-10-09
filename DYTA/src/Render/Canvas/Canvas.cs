using System;
using System.Collections.Generic;
using System.Text;

using DYTA.Math;

namespace DYTA.Render
{
    public abstract class Canvas : UIComponent
    {
        // reset buffer value and color
        public abstract void ResetBuffer();

        public abstract void SetPixel(char character, Vector2Int pos);
    }
}
