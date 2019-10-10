using System;
using System.Collections.Generic;
using System.Text;

using DYTA.Math;

namespace DYTA.Render
{
    public abstract class Canvas : UIComponent
    {
        private bool m_IsDirty = true;
        public bool IsDirty
        {
            get { return m_IsDirty; }
            set 
            {
                m_IsDirty = value;

                if (m_IsDirty)
                {
                    ResetBuffer();
                }
            }
        }

        // reset buffer value and color
        public abstract void ResetBuffer();

        public abstract void SetPixel(char character, Vector2Int pos);
    }
}
