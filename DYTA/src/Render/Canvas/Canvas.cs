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

        public override void OnAddedToNode(UINode node)
        {
            base.OnAddedToNode(node);
        }
    }
}
