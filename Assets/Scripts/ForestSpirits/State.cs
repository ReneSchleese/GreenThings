using System;
using UnityEngine;

namespace ForestSpirits
{
    public class State
    {
        protected Action<Type> switchToState;
        protected Spirit spirit;
        protected Actor actor;

        public void Init(Spirit spirit, Actor actor, Action<Type> enterStateCallback)
        {
            this.spirit = spirit;
            switchToState = enterStateCallback;
            this.actor = actor;
        }
        
        public virtual void OnEnter() {}
        public virtual void OnExit() {}
        public virtual void OnUpdate() {}
        protected static PlayerCharacter Player => App.Instance.Player;
    }
    
    public class IdleState : State
    {
        private const float SEEKING_DISTANCE = 6f;

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!PlayerIsInReach()) return;
            Player.Chain.Enqueue(spirit);
            switchToState(typeof(FollowPlayerState));
        }

        private bool PlayerIsInReach()
        {
            return Vector3.Distance(Player.transform.position, spirit.transform.position) <= SEEKING_DISTANCE;
        }
    }
    
    public class FollowPlayerState : State
    {
        private const float SPEED = ChainLinkState.SPEED;
        private const float DEAD_ZONE_DISTANCE = 5.5f;
        private const float ENQUEUEING_DISTANCE = .5f;
        private float _timeStampWhereFast;

        public override void OnEnter()
        {
            base.OnEnter();
            _timeStampWhereFast = Time.time;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Vector3 spiritToPlayerDir = Player.transform.position - spirit.transform.position;
            float distance = spiritToPlayerDir.magnitude;
            
            if (Player.JoystickMagnitude >= 0.667f && distance > ENQUEUEING_DISTANCE)
            {
                switchToState(typeof(ChainLinkState));
                return;
            }
            
            Vector3 lookDir = Player.Position - spirit.Position;
            lookDir = new Vector3(lookDir.x, 0f, lookDir.z);
            spirit.transform.rotation = Quaternion.LookRotation(lookDir);
            
            if (distance > DEAD_ZONE_DISTANCE)
            {
                spirit.Controller.Move(spiritToPlayerDir.normalized * (SPEED * Time.deltaTime));
            }
            
            if (!IsSlowEnoughToUnfold())
            {
                _timeStampWhereFast = Time.time;
            }

            if (HasBeenSlowLongEnoughToUnfold())
            {
                switchToState(typeof(FlowerState));
                return;
            }

            bool IsSlowEnoughToUnfold() => actor.Speed <= 0.02f;
            bool HasBeenSlowLongEnoughToUnfold() => Time.time - _timeStampWhereFast > 1.5f;
        }
    }
    
    public class ChainLinkState : State
    {
        public const float SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.95f;
        private IChainTarget _target;

        public override void OnEnter()
        {
            base.OnEnter();
            _target = Player.Chain.GetTargetFor(spirit);
            spirit.Controller.radius = 0.25f;
            spirit.PushHitbox.Radius = 0.25f;
        }

        public override void OnExit()
        {
            base.OnExit();
            _target = null;
            spirit.Controller.radius = 0.4f;
            spirit.PushHitbox.Radius = 0.5f;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (Player.JoystickMagnitude < 0.667f)
            {
                switchToState(typeof(FollowPlayerState));
                return;
            }
            
            Vector3 lookDir = _target.Position - spirit.Position;
            lookDir = new Vector3(lookDir.x, 0f, lookDir.z);
            spirit.transform.rotation = Quaternion.LookRotation(lookDir);
            
            if ((_target.Position - spirit.Position).sqrMagnitude <= 0.1f)
            {
                return;
            }
            Vector3 direction = _target.Position - spirit.Position;
            spirit.Controller.Move(direction.normalized * (Time.deltaTime * SPEED));
        }
    }

    public class FlowerState : State
    {
        public override void OnEnter()
        {
            base.OnEnter();
            actor.Unfold();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Player.Velocity.magnitude > 0.1f)
            {
                switchToState(typeof(FollowPlayerState));
                return;
            }
        }
    }
}