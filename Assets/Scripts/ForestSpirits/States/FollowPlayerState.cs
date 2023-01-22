using UnityEngine;

namespace ForestSpirits
{
    public class FollowPlayerState : State
    {
        private const float SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.8f;
        private const float DEADZONE_DISTANCE = IdleState.SEEKING_DISTANCE * 0.5f;
        private const float LOSING_DISTANCE = IdleState.SEEKING_DISTANCE * 1.5f;

        public override void OnUpdate()
        {
            base.OnUpdate();
            Vector3 spiritToPlayerDir = App.Instance.Player.transform.position - forestSpirit.transform.position;
            float distance = spiritToPlayerDir.magnitude;
            if (distance > LOSING_DISTANCE)
            {
                switchToState(typeof(IdleState));
                return;
            }
            if(distance > DEADZONE_DISTANCE)
            {
                forestSpirit.CharacterController.Move(spiritToPlayerDir.normalized * Time.deltaTime * SPEED);
            }
        }
    }
}