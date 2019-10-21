using DYTA.Math;
using DYTA.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSShaft
{
    public class DeathZone : HasCollision
    {
        //
        public World2D World { get; protected set; }

        //
        protected UINode RenderNode { get; private set; }

        public UnlitBox Image { get; private set; }

        public void Initialize(World2D world, RectInt collider, bool withSpikeGraphics)
        {
            Collider = collider;
            World = world;

            RenderNode = UINode.Engine.Instance.CreateNode(Collider, World.WorldStaticNode, "DeathZone-Node");
            var canvas = RenderNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Black, ConsoleColor.Red);

            if (withSpikeGraphics)
            {
                var imageNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, Collider.Size), RenderNode, "DeathZone-Image");
                Image = imageNode.AddUIComponent<UnlitBox>();
                Image.UnlitCharacter = 'V';
            }
        }

        public override void OnCharacterCollision(Character ch)
        {
            base.OnCharacterCollision(ch);
            ch.AddHealth(-999);
        }
    }
}
