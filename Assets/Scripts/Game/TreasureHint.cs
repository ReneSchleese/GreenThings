using System;
using DG.Tweening;
using UnityEngine;

public class TreasureHint : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AnimationCurve _animationCurveY;
    private const float COOLDOWN_TIME = 5.0f;
    private float _timeLastUsed;
    private BuriedTreasure _target;
    private Transform _particleSystemTransform;
    private Tweener _tweener;
    private float _peakYAddend;

    public event Action Triggered;

    public void SetTarget(BuriedTreasure treasure)
    {
        _target = treasure;
        _particleSystemTransform = _particleSystem.GetComponent<Transform>();
        _peakYAddend = 0.2f * Vector3.Distance(_target?.transform.position ?? Vector3.zero, transform.position);
    }

    public void Trigger()
    {
        if (!MayBeTriggered) return;

        Vector3 startPos = transform.position;
        Vector3 endPos = _target.transform.position;
        _particleSystem.gameObject.SetActive(false);
        _particleSystemTransform.position = startPos;
        _particleSystem.gameObject.SetActive(true);
        float duration = 0.05f * Vector3.Distance(startPos, endPos);
        _tweener = DOVirtual.Vector3(startPos, endPos, duration, OnMoveUpdate).SetEase(Ease.Linear);

        Triggered?.Invoke();
        _timeLastUsed = Time.time;
        return;

        void OnMoveUpdate(Vector3 lerp)
        {
            float addendY = _animationCurveY.Evaluate(_tweener.ElapsedPercentage()) * _peakYAddend;
            Vector3 newPos = new(lerp.x, lerp.y + addendY, lerp.z);
            _particleSystemTransform.position = newPos;
        }
    }
    
    private bool IsOnCooldown => _timeLastUsed +  COOLDOWN_TIME > Time.time;
    private bool MayBeTriggered => _target is not null && !IsOnCooldown;
}