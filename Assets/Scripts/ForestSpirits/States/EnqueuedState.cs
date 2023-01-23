using UnityEngine;

namespace ForestSpirits
{
    public class EnqueuedState : State
    {
        private const float SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.8f;
        private const float DEADZONE_DISTANCE = 1.25f;
        private IFollowable _target;

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("OnEnter() EnqueuedState");
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

            Vector3 spiritToTargetDir = _target.WorldPosition - forestSpirit.WorldPosition;
            float distance = spiritToTargetDir.magnitude;
            

            if (Mathf.Approximately(Player.Speed.magnitude, 0))
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