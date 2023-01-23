using UnityEngine;

namespace ForestSpirits
{
    public class FollowPlayerState : State
    {
        private const float SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.8f;
        private const float DEADZONE_DISTANCE = 3f;
        private const float ENQUEUEING_DISTANCE = .5f;

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("OnEnter() FollowPlayerState");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Vector3 spiritToPlayerDir = Player.transform.position - forestSpirit.transform.position;
            float distance = spiritToPlayerDir.magnitude;
            
            if (Player.Speed.magnitude > PlayerCharacter.MOVEMENT_SPEED * 0.5f && distance > ENQUEUEING_DISTANCE)
            {
                switchToState(typeof(EnqueuedState));
                return;
            }
            if (distance > DEADZONE_DISTANCE)
            {
                forestSpirit.CharacterController.Move(spiritToPlayerDir.normalized * Time.deltaTime * SPEED);
            }
        }
    }
}