using System;
using System.Collections.Generic;
using System.Text;

namespace DYTA.Render
{
    public abstract class UIElement : UIComponent
    {
        public Canvas MainCanvas { get; private set; }

        public UIElement()
        {

        }

        public override void OnInitializedByNode(UINode node)
        {
            base.OnInitializedByNode(node);

            var currNode = node;
            var canvas = currNode.GetUIComponent<Canvas>();
            while (canvas == null)
            {
                currNode = currNode.Parent;
                canvas = currNode.GetUIComponent<Canvas>();
            }

            MainCanvas = canvas;
        }
    }
}
