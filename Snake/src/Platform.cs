using DYTA.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    public class Platform : IHasCollider
    {
        public World2D World { get; protected set; }

        public RectInt Collider { get; protected set; }

        protected Dictionary<int, Character> m_Characters = new Dictionary<int, Character>();

        public Platform(World2D world, RectInt collider)
        {
            Collider = collider;
            World = world;
        }

        public void Initialize()
        {
            // TODO, create UINode and Image
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
