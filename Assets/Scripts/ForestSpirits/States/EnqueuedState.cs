using UnityEngine;

namespace ForestSpirits
{
    public class EnqueuedState : State
    {
        private const float SPEED = PlayerCharacter.MOVEMENT_SPEED * 0.8f;
        private const float BREAKING_DISTANCE = IdleState.SEEKING_DISTANCE * 1.5f;
        private IFollowable _target;
        
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Entering enqueued state");
            _target = App.Instance.Player.ForestSpiritChain.Enqueue(forestSpirit);
        }

        public override void OnExit()
        {
            base.OnExit();
            App.Instance.Player.ForestSpiritChain.Dequeue(forestSpirit);
            _target = null;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Vector3 spiritToTargetDir = _target.WorldPosition - forestSpirit.WorldPosition;
            float distance = spiritToTargetDir.magnitude;
            forestSpirit.CharacterController.Move(spiritToTargetDir.normalized * Time.deltaTime * SPEED);

            if (_target is ForestSpirit && _target.IsFollowing == false)
            {
                switchToState(typeof(IdleState));
                return;
            }
            
            if (distance > BREAKING_DISTANCE)
            {
                switchToState(typeof(IdleState));
                return;
            }
        }
    }
}