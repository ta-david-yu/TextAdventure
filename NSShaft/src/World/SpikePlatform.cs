using System;
using System.Collections.Generic;
using System.Text;
using DYTA.Math;
using DYTA.Render;

namespace NSShaft
{
    public class SpikePlatform : Platform
    {
        protected override string m_ImgFilePath => "./Assets/SpikePlatform.txt";

        public override void Initialize(World2D world, RectInt collider)
        {
            base.Initialize(world, collider);
            (RenderNode.Canvas as SingleColorCanvas).CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.Red);
        }

        protected override void handleOnCharacterStepOn(Character ch)
        {
            ch.AddHealth(-3);
        }
    }
}
