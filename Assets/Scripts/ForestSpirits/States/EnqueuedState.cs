using UnityEngine;

namespace ForestSpirits
{
    public class EnqueuedState : State
    {
        public const float SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.95f;
        private const float DEADZONE_DISTANCE = 1.5f;
        private IFollowable _target;

        public override void OnEnter()
        {
            base.OnEnter();
            _target = Player.ForestSpiritChain.GetTarget(forestSpirit);
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
                switchToState(typeof(FollowPlayerState));
                return;
            }

            if (Vector3.Distance(_target.WorldPosition, forestSpirit.WorldPosition) <= DEADZONE_DISTANCE)
            {
                return;
            }
            Vector3 direction = _target.WorldPosition - forestSpirit.WorldPosition;
            forestSpirit.CharacterController.Move(direction.normalized * (Time.deltaTime * SPEED));
        }
    }
}