using System.Collections;
using UnityEngine;

public class HornetAnimator : MonoBehaviour
{
    private static class Constants
    {
        public static readonly int MovementSpeedId = Animator.StringToHash("MovementSpeed");
        public static readonly int RunTriggerId = Animator.StringToHash("Run");
        public static readonly int IdleTriggerId = Animator.StringToHash("Idle");
        public static readonly int StartBattlecryId = Animator.StringToHash("StartBattlecry");
        public static readonly int StopBattlecryId = Animator.StringToHash("StopBattlecry");
        public const string MovementState = "Movement";
        public const string IdleState = "Idle";
        public const float IsMovingThreshold = 0.1f;
    }
    
    [SerializeField] private Animator _animator;
    private Coroutine _battlecryRoutine;

    private void OnDisable()
    {
        StopAndClearBattlecry();
    }

    public void UpdateAnimator(Vector3 currentVelocity)
    {
        float movementSpeed = currentVelocity.magnitude;
        bool isMoving = movementSpeed > Constants.IsMovingThreshold;
        if (isMoving)
        {
            _animator.SetFloat(Constants.MovementSpeedId, movementSpeed);
            if (CurrentStateIs(Constants.MovementState) || NextStateIs(Constants.MovementState)) return;
            ResetAllTriggers();
            _animator.SetTrigger(Constants.RunTriggerId);
        }
        else
        {
            if (CurrentStateIs(Constants.IdleState) || NextStateIs(Constants.IdleState)) return;
            ResetAllTriggers();
            _animator.SetTrigger(Constants.IdleTriggerId);
        }
    }

    public void PlayBattlecry(int index)
    {
        StopAndClearBattlecry();
        _battlecryRoutine = StartCoroutine(Battlecry(index));
    }

    private IEnumerator Battlecry(int index)
    {
        yield return Battlecry01();
        _battlecryRoutine = null;
    }

    private IEnumerator Battlecry01()
    {
        _animator.SetLayerWeight(1, 0.8f);
        _animator.SetTrigger(Constants.StartBattlecryId);
        yield return new WaitForSeconds(0.12f);
        _animator.SetTrigger(Constants.StopBattlecryId);
        yield return new WaitForSeconds(0.06f);
        _animator.SetLayerWeight(1, 0.6f);
        _animator.SetTrigger(Constants.StartBattlecryId);
        yield return new WaitForSeconds(0.1f);
        _animator.SetTrigger(Constants.StopBattlecryId);
        yield return new WaitForSeconds(0.06f);
        _animator.SetLayerWeight(1, 0.8f);
        _animator.SetTrigger(Constants.StartBattlecryId);
        yield return new WaitForSeconds(0.18f);
        _animator.SetTrigger(Constants.StopBattlecryId);
        yield return new WaitForSeconds(0.18f);
    }
    
    private void StopAndClearBattlecry()
    {
        if (_battlecryRoutine != null)
        {
            StopCoroutine(_battlecryRoutine);
        }
        _battlecryRoutine = null;
    }

    private bool CurrentStateIs(string state)
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(state);
    }
    
    private bool NextStateIs(string state)
    {
        return _animator.GetNextAnimatorStateInfo(0).IsName(state);
    }

    private void ResetAllTriggers()
    {
        _animator.ResetTrigger(Constants.IdleTriggerId);
        _animator.ResetTrigger(Constants.RunTriggerId);
    }
}