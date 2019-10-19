using DYTA;
using DYTA.Audio;
using DYTA.Math;
using DYTA.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSShaft
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

        public void SetPositionAndSize(Vector2Int pos , Vector2Int size)
        {
            Collider = new RectInt(pos, size);
            RenderNode.SetPosition(pos);
            RenderNode.SetSize(size);
            Image.Node.SetSize(size);
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

        public override void OnCharacterCollision(Character ch)
        {
        }

        public override void OnCharacterStepOn(Character ch)
        {
            OnTopCharacters.Add(ch.Id, ch);
            handleOnCharacterStepOn(ch);
            ch.OnStepOnPlatform(this);
        }

        public override void OnCharacterLiftOff(Character ch)
        {
            OnTopCharacters.Remove(ch.Id);
            handleOnCharacterLiftOff(ch);
            ch.OnLeavePlatform(this);
        }

        protected virtual void handleOnCharacterStepOn(Character ch)
        {
            ch.AddHealth(1);
        }

        protected virtual void handleOnCharacterLiftOff(Character ch)
        {

        }

        protected override void onIsActiveChanged(bool value)
        {
            RenderNode.IsActive = value;
        }
    }
}
