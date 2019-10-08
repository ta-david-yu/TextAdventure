using System;
using System.Collections.Generic;
using System.Text;

namespace DYTA.Render
{
    public abstract class UIComponent
    {
        public UINode Node { get; private set; }

        public Canvas ParentCanvas { get; private set; }

        public virtual void PreRender() { }

        // draw on the cmd line
        public virtual void Render() { }

        public virtual void OnAddedToNode(UINode node)
        {
            Node = node;
        }
    }
}
