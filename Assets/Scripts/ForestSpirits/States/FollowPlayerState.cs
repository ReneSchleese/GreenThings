using UnityEngine;

namespace ForestSpirits
{
    public class FollowPlayerState : State
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Entered FollowPlayerState");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            float SPEED = 2f;
            forestSpirit.CharacterController.Move(
                (App.Instance.Player.transform.position - forestSpirit.transform.position).normalized * Time.deltaTime * SPEED);
        }
    }
}