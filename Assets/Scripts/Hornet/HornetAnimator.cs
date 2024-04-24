using System.Collections;
using DG.Tweening;
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
        DOTween.Kill(this);
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
        _animator.SetLayerWeight(1, 0.0f);
        _animator.SetTrigger(Constants.StartBattlecryId);
        _battlecryRoutine = StartCoroutine(Battlecry(index));
    }

    private IEnumerator Battlecry(int index)
    {
        switch (index)
        {
            case 0:
                yield return Battlecry01();
                break;
            case 1:
                yield return Battlecry02();
                break;
            case 2:
                yield return Battlecry03();
                break;
            case 3:
                yield return Battlecry04();
                break;
            case 4:
                yield return Battlecry04();
                break;
            case 5:
                yield return Battlecry05();
                break;
        }

        StopAndClearBattlecry();
    }

    private float GetWeight() => _animator.GetLayerWeight(1);
    private void SetWeight(float value) => _animator.SetLayerWeight(1, value);

    private Sequence GetSequence()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetId(this);
        return sequence;
    }

    private IEnumerator Battlecry01()
    {
        Sequence sequence = GetSequence();
        sequence.Insert(0.1f, DOTween.To(GetWeight, SetWeight, 0.2f, 0.06f));
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.1f, 0.10f));
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.35f, 0.06f));
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.25f, 0.10f));
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.45f, 0.1f));
        sequence.AppendCallback(() => _animator.SetTrigger(Constants.StopBattlecryId));
        yield return sequence.WaitForCompletion();
    }

    private IEnumerator Battlecry02()
    {
        Sequence sequence = GetSequence();
        sequence.AppendInterval(0.05f);
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.9f, 0.15f).SetEase(Ease.InSine));
        sequence.AppendInterval(0.3f);
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.0f, 0.2f).SetEase(Ease.InQuad));
        sequence.AppendCallback(() => _animator.SetTrigger(Constants.StopBattlecryId));
        yield return sequence.WaitForCompletion();
    }
    
    private IEnumerator Battlecry03()
    {
        Sequence sequence = GetSequence();
        sequence.Insert(0.1f, DOTween.To(GetWeight, SetWeight, 0.9f, 0.15f).SetEase(Ease.InSine));
        sequence.AppendInterval(0.4f);
        sequence.AppendCallback(() => _animator.SetTrigger(Constants.StopBattlecryId));
        
        yield return sequence.WaitForCompletion();
    }
    
    private IEnumerator Battlecry04()
    {
        Sequence sequence = GetSequence();
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.5f, 0.1f));
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.3f, 0.1f));
        sequence.Append(DOTween.To(GetWeight, SetWeight, 1.0f, 0.15f));
        sequence.AppendInterval(0.1f);
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.7f, 0.1f));
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.9f, 0.1f));
        sequence.AppendInterval(0.15f);
        sequence.AppendCallback(() => _animator.SetTrigger(Constants.StopBattlecryId));
        yield return sequence.WaitForCompletion();
    }
    
    private IEnumerator Battlecry05()
    {
        Sequence sequence = GetSequence();
        sequence.Insert(0.1f, DOTween.To(GetWeight, SetWeight, 0.5f, 0.1f));
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.45f, 0.15f));
        sequence.AppendInterval(0.1f);
        sequence.Append(DOTween.To(GetWeight, SetWeight, 1.0f, 0.2f));
        sequence.AppendInterval(0.15f);
        sequence.Append(DOTween.To(GetWeight, SetWeight, 0.0f, 0.2f));
        sequence.AppendCallback(() => _animator.SetTrigger(Constants.StopBattlecryId));
        yield return sequence.WaitForCompletion();
    }

    private void StopAndClearBattlecry()
    {
        if (_battlecryRoutine != null)
        {
            StopCoroutine(_battlecryRoutine);
            DOTween.Kill(this);
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