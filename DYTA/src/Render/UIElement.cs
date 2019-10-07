using System;
using System.Collections.Generic;
using System.Text;

using DYTA.Math;

namespace DYTA.Render
{
    public class UIElement
    {
        public RectInt Bounds { get; set; }

        public UIElement()
        {

        }

        public virtual void Render()
        {

        }
    }
}
