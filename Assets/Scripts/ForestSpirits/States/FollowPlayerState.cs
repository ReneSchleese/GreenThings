using UnityEngine;

namespace ForestSpirits
{
    public class FollowPlayerState : State
    {
        private const float SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.8f;
        private const float DEADZONE_DISTANCE = IdleState.SEEKING_DISTANCE * 0.2f;
        private const float ENQUEUEING_DISTANCE = IdleState.SEEKING_DISTANCE * 0.33f;
        private float _enterTime;

        public override void OnEnter()
        {
            base.OnEnter();
            _enterTime = Time.time;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            PlayerCharacter player = App.Instance.Player;
            Vector3 spiritToPlayerDir = player.transform.position - forestSpirit.transform.position;
            float distance = spiritToPlayerDir.magnitude;
            
            if (MayEnqueue && distance > ENQUEUEING_DISTANCE)
            {
                switchToState(typeof(EnqueuedState));
            }
            if (distance > DEADZONE_DISTANCE)
            {
                forestSpirit.CharacterController.Move(spiritToPlayerDir.normalized * Time.deltaTime * SPEED);
            }
        }

        private bool MayEnqueue => Time.time - _enterTime > 0.66f;
    }
}