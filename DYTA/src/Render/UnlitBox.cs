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
            UnlitCharacter = ' ';
        }

        public override void PreRender()
        {

        }
    }
}
