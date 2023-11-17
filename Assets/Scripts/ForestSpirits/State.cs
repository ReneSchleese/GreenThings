using System;
using UnityEngine;

namespace ForestSpirits
{
    public class State
    {
        protected Action<Type> SwitchToState;
        protected ForestSpirit ForestSpirit;

        public void Init(ForestSpirit spirit, Action<Type> enterStateCallback)
        {
            ForestSpirit = spirit;
            SwitchToState = enterStateCallback;
        }
        
        public virtual void OnEnter() {}
        public virtual void OnExit() {}
        public virtual void OnUpdate() {}
        protected static PlayerCharacter Player => App.Instance.Player;
    }
    
    public class IdleState : State
    {
        private const float SEEKING_DISTANCE = 3f;

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!PlayerIsInReach()) return;
            Player.ForestSpiritChain.Enqueue(ForestSpirit);
            SwitchToState(typeof(FollowPlayerState));
        }

        private bool PlayerIsInReach()
        {
            return Vector3.Distance(Player.transform.position, ForestSpirit.transform.position) <= SEEKING_DISTANCE;
        }
    }
    
    public class FollowPlayerState : State
    {
        private const float SPEED = ChainLinkState.SPEED;
        private const float DEAD_ZONE_DISTANCE = 3f;
        private const float ENQUEUEING_DISTANCE = .5f;

        public override void OnUpdate()
        {
            base.OnUpdate();
            Vector3 spiritToPlayerDir = Player.transform.position - ForestSpirit.transform.position;
            float distance = spiritToPlayerDir.magnitude;
            
            if (Player.Speed.magnitude > PlayerCharacter.MOVEMENT_SPEED * 0.5f && distance > ENQUEUEING_DISTANCE)
            {
                SwitchToState(typeof(ChainLinkState));
                return;
            }
            if (distance > DEAD_ZONE_DISTANCE)
            {
                ForestSpirit.CharacterController.Move(spiritToPlayerDir.normalized * (Time.deltaTime * SPEED));
            }
        }
    }
    
    public class ChainLinkState : State
    {
        public const float SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.95f;
        private const float DEAD_ZONE_DISTANCE = 1.5f;
        private IChainTarget _target;

        public override void OnEnter()
        {
            base.OnEnter();
            _target = Player.ForestSpiritChain.GetTargetFor(ForestSpirit);
        }

        public override void OnExit()
        {
            base.OnExit();
            _target = null;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (Mathf.Approximately(Player.Speed.magnitude, 0))
            {
                SwitchToState(typeof(FollowPlayerState));
                return;
            }

            if (Vector3.Distance(_target.WorldPosition, ForestSpirit.WorldPosition) <= DEAD_ZONE_DISTANCE)
            {
                return;
            }
            Vector3 direction = _target.WorldPosition - ForestSpirit.WorldPosition;
            ForestSpirit.CharacterController.Move(direction.normalized * (Time.deltaTime * SPEED));
        }
    }
}