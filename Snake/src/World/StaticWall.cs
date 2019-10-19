using System;
using System.Collections.Generic;
using System.Text;
using DYTA.Math;
using DYTA.Render;

namespace Snake
{
    public class StaticWall : HasCollision
    {
        public World2D World { get; protected set; }

        //
        protected UINode RenderNode { get; private set; }

        public UnlitBox Image { get; private set; }

        public StaticWall() { }

        public virtual void Initialize(World2D world, RectInt collider)
        {
            Collider = collider;
            World = world;

            RenderNode = UINode.Engine.Instance.CreateNode(Collider, World.WorldStaticNode, "Static-Node");
            var canvas = RenderNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Gray, ConsoleColor.Blue);

            var imageNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, Collider.Size), RenderNode, "Wall-Image");
            Image = imageNode.AddUIComponent<UnlitBox>();
            Image.UnlitCharacter = ' ';
        }
    }
}
