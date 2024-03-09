using UnityEngine;

public class HornetAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void UpdateAnimator(Vector3 currentVelocity)
    {
        float movementSpeed = currentVelocity.magnitude;
        bool isMoving = movementSpeed > Mathf.Epsilon;
        if (isMoving)
        {
            _animator.SetFloat("MovementSpeed", movementSpeed);
            var stateIsMovement = _animator.GetCurrentAnimatorStateInfo(0).IsName("Movement");
            var isTransitioningToMovement = _animator.GetAnimatorTransitionInfo(0).IsName("IdleToMovement");
            var isName = _animator.GetNextAnimatorStateInfo(0).IsName("Movement");
            if (isName)
            {
                Debug.Log("WOULD BE RUN");
            }
            if (isTransitioningToMovement)
            {
                Debug.Log("TRANSITIONING TO IDLE");
            }
            if (!stateIsMovement && !isTransitioningToMovement && !isName)
            {
                _animator.ResetTrigger("Idle");
                _animator.ResetTrigger("Run");
                Debug.Log("Trigger Run!");
                _animator.SetTrigger("Run");
            }
        }
        else
        {
            
            var stateIsIdle = _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
            var isTransitioningToIdle = _animator.GetAnimatorTransitionInfo(0).IsName("MovementToIdle");
            if (isTransitioningToIdle)
            {
                Debug.Log("TRANSITIONING TO IDLE");
            }
            if (!stateIsIdle && !isTransitioningToIdle)
            {
                _animator.ResetTrigger("Idle");
                _animator.ResetTrigger("Run");
                Debug.Log("Trigger Idle!");
                _animator.SetTrigger("Idle");
            }
        }
    }
}