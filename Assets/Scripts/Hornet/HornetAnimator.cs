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
    private Sequence _battlecrySequence;

    private void OnDisable()
    {
        KillActiveSequence();
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
        KillActiveSequence();
        RenewSequence();
        _animator.SetLayerWeight(1, 0.0f);
        _animator.SetTrigger(Constants.StartBattlecryId);
        
        switch (index)
        {
            case 0:
                Battlecry01();
                break;
            case 1:
                Battlecry02();
                break;
            case 2:
                Battlecry03();
                break;
            case 3:
                Battlecry04();
                break;
            case 4:
                Battlecry04();
                break;
            case 5:
                Battlecry05();
                break;
        }
    }

    private float GetWeight() => _animator.GetLayerWeight(1);
    private void SetWeight(float value) => _animator.SetLayerWeight(1, value);

    private void RenewSequence()
    {
        _battlecrySequence = DOTween.Sequence();
        _battlecrySequence.SetId(this);
    }

    private void Battlecry01()
    {
        _battlecrySequence.Insert(0.1f, DOTween.To(GetWeight, SetWeight, 0.2f, 0.06f));
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.1f, 0.10f));
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.35f, 0.06f));
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.25f, 0.10f));
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.45f, 0.1f));
        _battlecrySequence.AppendCallback(() => _animator.SetTrigger(Constants.StopBattlecryId));
    }

    private void Battlecry02()
    {
        _battlecrySequence.AppendInterval(0.05f);
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.9f, 0.15f).SetEase(Ease.InSine));
        _battlecrySequence.AppendInterval(0.3f);
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.0f, 0.2f).SetEase(Ease.InQuad));
        _battlecrySequence.AppendCallback(() => _animator.SetTrigger(Constants.StopBattlecryId));
    }
    
    private void Battlecry03()
    {
        _battlecrySequence.Insert(0.125f, DOTween.To(GetWeight, SetWeight, 1.0f, 0.15f).SetEase(Ease.InSine));
        _battlecrySequence.AppendInterval(0.3f);
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.0f, 0.2f).SetEase(Ease.InQuad));
        _battlecrySequence.AppendCallback(() => _animator.SetTrigger(Constants.StopBattlecryId));
        
    }
    
    private void Battlecry04()
    {
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.5f, 0.1f));
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.3f, 0.1f));
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 1.0f, 0.15f));
        _battlecrySequence.AppendInterval(0.1f);
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.7f, 0.1f));
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.9f, 0.1f));
        _battlecrySequence.AppendInterval(0.15f);
        _battlecrySequence.AppendCallback(() => _animator.SetTrigger(Constants.StopBattlecryId));
    }
    
    private void Battlecry05()
    {
        _battlecrySequence.Insert(0.1f, DOTween.To(GetWeight, SetWeight, 0.5f, 0.1f));
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.45f, 0.15f));
        _battlecrySequence.AppendInterval(0.1f);
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 1.0f, 0.2f));
        _battlecrySequence.AppendInterval(0.15f);
        _battlecrySequence.Append(DOTween.To(GetWeight, SetWeight, 0.0f, 0.2f));
        _battlecrySequence.AppendCallback(() => _animator.SetTrigger(Constants.StopBattlecryId));
    }

    private void KillActiveSequence()
    {
        if (_battlecrySequence is { active: true})
        {
            DOTween.Kill(_battlecrySequence);
        }
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