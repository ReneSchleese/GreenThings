using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ForestSpirits
{
    public class State
    {
        protected Action<Type> switchToState;
        protected Spirit spirit;
        protected Puppet Puppet;

        public void Init(Spirit spirit, Puppet puppet, Action<Type> enterStateCallback)
        {
            this.spirit = spirit;
            switchToState = enterStateCallback;
            Puppet = puppet;
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
        private const float SPEED = ChainLinkState.BASE_SPEED + ChainLinkState.DISTANCE_BASED_SPEED_BOOST;
        private const float DEAD_ZONE_DISTANCE = 5.5f;
        private const float ENQUEUEING_DISTANCE = .5f;
        private float _timeStampWhereFast;
        private float _randomUnfoldDelay;

        public override void OnEnter()
        {
            base.OnEnter();
            _timeStampWhereFast = Time.time;
            _randomUnfoldDelay = Random.Range(0f, 0.3f);
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

            bool IsSlowEnoughToUnfold() => Puppet.Speed <= 0.02f;
            bool HasBeenSlowLongEnoughToUnfold() => Time.time - (_timeStampWhereFast + _randomUnfoldDelay) > 1.5f;
        }
    }
    
    public class ChainLinkState : State
    {
        public const float BASE_SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.8f;
        public const float DISTANCE_BASED_SPEED_BOOST = PlayerCharacter.MOVEMENT_SPEED * 0.15f;
        private const float SPEED_BOOST_MAX_DISTANCE = 3f;
        private IChainTarget _target;

        public override void OnEnter()
        {
            base.OnEnter();
            _target = Player.Chain.GetTargetFor(spirit);
        }

        public override void OnExit()
        {
            base.OnExit();
            _target = null;
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
            float distanceFactor = Mathf.InverseLerp(0f, SPEED_BOOST_MAX_DISTANCE, direction.magnitude);
            float speed = BASE_SPEED + DISTANCE_BASED_SPEED_BOOST * distanceFactor;
            spirit.Controller.Move(direction.normalized * (Time.deltaTime * speed));
        }
    }

    public class FlowerState : State
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Puppet.Unfold();
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