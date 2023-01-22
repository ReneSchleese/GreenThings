using UnityEngine;

namespace ForestSpirits
{
    public class IdleState : State
    {
        public const float SEEKING_DISTANCE = 3f;

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Entering idle state");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (PlayerIsInReach())
            {
                switchToState(typeof(FollowPlayerState));
            }
        }

        private bool PlayerIsInReach()
        {
            return Vector3.Distance(App.Instance.Player.transform.position, forestSpirit.transform.position) <= SEEKING_DISTANCE;
        }
    }
}