using UnityEngine;

namespace ForestSpirits
{
    public class IdleState : State
    {
        public const float SEEKING_DISTANCE = 3f;

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("OnEnter() IdleState");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (PlayerIsInReach())
            {
                App.Instance.Player.ForestSpiritChain.Enqueue(forestSpirit);
                switchToState(typeof(FollowPlayerState));
            }
        }

        private bool PlayerIsInReach()
        {
            return Vector3.Distance(App.Instance.Player.transform.position, forestSpirit.transform.position) <= SEEKING_DISTANCE;
        }
    }
}