using UnityEngine;

namespace ForestSpirits
{
    public class IdleState : State
    {
        private const float SEEKING_DISTANCE = 3f;

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (PlayerIsInReach())
            {
                Player.ForestSpiritChain.Enqueue(forestSpirit);
                switchToState(typeof(FollowPlayerState));
            }
        }

        private bool PlayerIsInReach()
        {
            return Vector3.Distance(Player.transform.position, forestSpirit.transform.position) <= SEEKING_DISTANCE;
        }
    }
}