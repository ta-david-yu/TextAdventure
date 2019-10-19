using DYTA;
using DYTA.Math;
using DYTA.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    public class Platform : HasCollision
    {
        //
        public World2D World { get; protected set; }

        //
        protected UINode RenderNode { get; private set; }

        public Bitmap Image { get; private set; }

        //
        protected virtual string m_ImgFilePath { get; } = "./Assets/Platform.txt";

        public Platform() { }

        public virtual void Initialize(World2D world, RectInt collider)
        {
            Collider = collider;
            World = world;

            RenderNode = UINode.Engine.Instance.CreateNode(Collider, World.PlatformNode, "Platform-Node");
            var canvas = RenderNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Gray, ConsoleColor.Blue);

            var imageNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, Collider.Size), RenderNode, "Platform-Image");
            Image = imageNode.AddUIComponent<Bitmap>();
            Image.LoadFromFile(m_ImgFilePath);
        }

        public void Update(float timeStep)
        {
            if (!IsActive)
            {
                onUpdate(timeStep);
            }

            // ...
        }

        protected virtual void onUpdate(float timeStep)
        {

        }

        public override void OnCharacterEnter(Character ch)
        {
        }

        public override void OnCharacterExit(Character ch)
        {
        }

        public override void OnCharacterStepOn(Character ch)
        {
            OnTopCharacters.Add(ch.Id, ch);
        }

        public override void OnCharacterLiftOff(Character ch)
        {
            OnTopCharacters.Remove(ch.Id);
        }

        protected override void onIsActiveChanged(bool value)
        {
            RenderNode.IsActive = value;
        }
    }
}
