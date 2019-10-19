using DYTA.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    public abstract class HasCollision
    {
        private bool m_IsActive = true;
        public bool IsActive
        {
            get { return m_IsActive; }
            set
            {
                var prev = m_IsActive;
                m_IsActive = value;
                if (prev != value)
                {
                    onIsActiveChanged(value);
                }
            }
        }

        public RectInt Collider { get; protected set; }

        public Dictionary<int, Character> OnTopCharacters { get; protected set; } = new Dictionary<int, Character>();

        public virtual void OnCharacterEnter(Character ch) { }
        public virtual void OnCharacterExit(Character ch) { }

        public virtual void OnCharacterStepOn(Character ch)
        {
            OnTopCharacters.Add(ch.Id, ch);
        }

        public virtual void OnCharacterLiftOff(Character ch)
        {
            OnTopCharacters.Remove(ch.Id);
        }

        public bool IsCharacterOnThisCollider(Character ch)
        {
            return OnTopCharacters.ContainsKey(ch.Id);
        }

        protected virtual void onIsActiveChanged(bool value)
        {

        }
    }
}
