using System;
using System.Collections.Generic;
using System.Text;

namespace DYTA.Render
{
    public abstract class UIComponent
    {
        public UINode Node { get; private set; }


        public virtual void PreRender() { }

        // draw on the cmd line
        public virtual void Render() { }

        public virtual void OnNodeSizeChanged(Math.Vector2Int size) { }

        public virtual void OnNodePositionChanged(Math.Vector2Int pos) { }

        public virtual void OnInitializedByNode(UINode node)
        {
            Node = node;
        }
    }
}
