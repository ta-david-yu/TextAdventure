using DYTA.Math;
using DYTA.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    public class Platform : IHasCollider
    {
        //
        public World2D World { get; protected set; }

        public RectInt Collider { get; protected set; }

        //
        protected UINode RenderNode { get; private set; }

        public Bitmap Image { get; private set; }

        //
        protected Dictionary<int, Character> m_Characters = new Dictionary<int, Character>();

        //
        protected virtual string m_ImgFilePath { get; } = "./Assets/Platform.txt";

        public Platform() { }

        public virtual void Initialize(World2D world, RectInt collider)
        {
            Collider = collider;
            World = world;

            RenderNode = UINode.Engine.Instance.CreateNode(Collider, World.TowerTopNode, "Platform-Node");
            var canvas = RenderNode.AddUIComponent<SingleColorCanvas>();
            canvas.CanvasPixelColor = new PixelColor(ConsoleColor.Gray, ConsoleColor.Blue);

            var imageNode = UINode.Engine.Instance.CreateNode(new RectInt(Vector2Int.Zero, Collider.Size), RenderNode, "Platform-Image");
            Image = imageNode.AddUIComponent<Bitmap>();
            Image.LoadFromFile(m_ImgFilePath);
        }

        public virtual void Update(float timeStep)
        {
            // ...
        }

        public virtual void OnCharacterEnter(Character ch)
        {
            m_Characters.Add(ch.Id, ch);
        }

        public virtual void OnCharacterExit(Character ch)
        {
            m_Characters.Remove(ch.Id);
        }

        public bool IsCharacterOnThisPlatform(Character ch)
        {
            return m_Characters.ContainsKey(ch.Id);
        }
    }
}
