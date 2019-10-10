using System;
using System.Collections.Generic;
using System.Text;

namespace DYTA.Render
{
    public class UnlitBox : UIElement
    {
        public char UnlitCharacter { get; set; }

        public UnlitBox()
        {
            UnlitCharacter = 'O';
        }

        public override void PreRender()
        {
            foreach (var pos in Node.Bounds.AllPositionsWithin)
            {
                Node.ParentCanvas.SetPixel(UnlitCharacter, pos);
            }
        }
    }
}
