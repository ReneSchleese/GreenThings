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

    public event Action Triggered;

    public void SetTarget(BuriedTreasure treasure)
    {
        _target = treasure;
        _particleSystemTransform = _particleSystem.GetComponent<Transform>();
    }

    public void Trigger()
    {
        if (!MayBeTriggered) return;

        _particleSystem.gameObject.SetActive(false);
        _particleSystemTransform.position = transform.position;
        _particleSystem.gameObject.SetActive(true);
        _tweener = DOVirtual.Vector3(_particleSystemTransform.position, _target.transform.position, 3f, OnMoveUpdate);

        Triggered?.Invoke();
        _timeLastUsed = Time.time;
        return;

        void OnMoveUpdate(Vector3 lerp)
        {
            Vector3 newPos = new(lerp.x, lerp.y + _animationCurveY.Evaluate(_tweener.ElapsedPercentage()) * 5f, lerp.z);
            _particleSystemTransform.position = newPos;
        }
    }
    
    private bool IsOnCooldown => _timeLastUsed +  COOLDOWN_TIME > Time.time;
    public bool MayBeTriggered => _target != null && !IsOnCooldown;
}