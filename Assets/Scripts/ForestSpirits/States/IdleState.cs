using UnityEngine;

namespace ForestSpirits
{
    public class IdleState : State
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Entering idle state");
        }
    }
}