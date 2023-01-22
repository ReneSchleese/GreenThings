using UnityEngine;

namespace ForestSpirits
{
    public class IdleState : State
    {
        private const float MINIMUM_DISTANCE = 3f;

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
            return Vector3.Distance(App.Instance.Player.transform.position, forestSpirit.transform.position) <= MINIMUM_DISTANCE;
        }
    }
}