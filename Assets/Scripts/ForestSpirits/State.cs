using System;
using UnityEngine;

namespace ForestSpirits
{
    public class State
    {
        protected Action<Type> SwitchToState;
        protected Spirit Spirit;

        public void Init(Spirit spirit, Action<Type> enterStateCallback)
        {
            Spirit = spirit;
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
            Player.Chain.Enqueue(Spirit);
            SwitchToState(typeof(FollowPlayerState));
        }

        private bool PlayerIsInReach()
        {
            return Vector3.Distance(Player.transform.position, Spirit.transform.position) <= SEEKING_DISTANCE;
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
            Vector3 spiritToPlayerDir = Player.transform.position - Spirit.transform.position;
            float distance = spiritToPlayerDir.magnitude;
            
            if (Player.Speed.magnitude > PlayerCharacter.MOVEMENT_SPEED * 0.5f && distance > ENQUEUEING_DISTANCE)
            {
                SwitchToState(typeof(ChainLinkState));
                return;
            }
            if (distance > DEAD_ZONE_DISTANCE)
            {
                Spirit.Controller.Move(spiritToPlayerDir.normalized * (Time.deltaTime * SPEED));
            }
        }
    }
    
    public class ChainLinkState : State
    {
        public const float SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.95f;
        public const float DEAD_ZONE_DISTANCE = 1.5f;
        private IChainTarget _target;

        public override void OnEnter()
        {
            base.OnEnter();
            _target = Player.Chain.GetTargetFor(Spirit);
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

            if (Vector3.Distance(_target.Position, Spirit.Position) <= 0.01f)
            {
                return;
            }
            Vector3 direction = _target.Position - Spirit.Position;
            Spirit.Controller.Move(direction.normalized * (Time.deltaTime * SPEED));
        }
    }
}