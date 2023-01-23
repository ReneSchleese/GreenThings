using System;

namespace ForestSpirits
{
    public class State
    {
        protected Action<Type> switchToState;
        protected ForestSpirit forestSpirit;

        public virtual void Init(ForestSpirit spirit, Action<Type> enterStateCallback)
        {
            forestSpirit = spirit;
            switchToState = enterStateCallback;
        }
        
        public virtual void OnEnter() {}
        public virtual void OnExit() {}
        public virtual void OnUpdate() {}
        protected PlayerCharacter Player => App.Instance.Player;
    }
}