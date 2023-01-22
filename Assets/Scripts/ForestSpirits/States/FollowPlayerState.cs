using UnityEngine;

namespace ForestSpirits
{
    public class FollowPlayerState : State
    {
        private const float SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.8f;
        private const float DEADZONE_DISTANCE = 1.25f;
        private const float ENQUEUEING_DISTANCE = 1.6f;
        private float _enterTime;

        public override void OnEnter()
        {
            base.OnEnter();
            _enterTime = Time.time;
            Debug.Log("OnEnter() FollowPlayerState");
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
                return;
            }
            if (distance > DEADZONE_DISTANCE)
            {
                forestSpirit.CharacterController.Move(spiritToPlayerDir.normalized * Time.deltaTime * SPEED);
            }
        }

        private bool MayEnqueue => Time.time - _enterTime > 0.33f;
    }
}