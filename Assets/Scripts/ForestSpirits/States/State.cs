using System;

namespace ForestSpirits
{
    public class State
    {
        protected Action<Type> enterState;

        public virtual void Init(Action<Type> enterStateCallback)
        {
            enterState = enterStateCallback;
        }
        
        public virtual void OnEnter() {}
        public virtual void OnExit() {}
        public virtual void OnUpdate() {}
    }
}