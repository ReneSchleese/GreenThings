using UnityEngine;

namespace ForestSpirits
{
    public class EnqueuedState : State
    {
        private const float SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.8f;
        private const float SWITCH_TO_PLAYER_DISTANCE = 1.5f;
        private const float DEADZONE_DISTANCE = 1f;
        private IFollowable _target;
        private float _enterTime;

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("OnEnter() EnqueuedState");
            _enterTime = Time.time;
            _target = App.Instance.Player.ForestSpiritChain.GetTarget(forestSpirit);
        }

        public override void OnExit()
        {
            base.OnExit();
            _target = null;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            Vector3 spiritToTargetDir = _target.WorldPosition - forestSpirit.WorldPosition;
            float distance = spiritToTargetDir.magnitude;

            Vector3 spiritToPlayer = App.Instance.Player.WorldPosition - forestSpirit.WorldPosition;
            if (spiritToPlayer.magnitude < SWITCH_TO_PLAYER_DISTANCE)
            {
                switchToState(typeof(FollowPlayerState));
                return;
            }

            if (Mathf.Approximately(App.Instance.Player.Speed.magnitude, 0))
            {
                switchToState(typeof(FollowPlayerState));
                return;
            }
            
            if(distance > DEADZONE_DISTANCE)
            {
                forestSpirit.CharacterController.Move(spiritToTargetDir.normalized * Time.deltaTime * SPEED);
            }
        }
    }
}