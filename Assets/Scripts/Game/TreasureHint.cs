using System;
using UnityEngine;

public class TreasureHint : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    private const float COOLDOWN_TIME = 5.0f;
    private float _timeLastUsed;
    private BuriedTreasure _target;

    public event Action Triggered;

    public void SetTarget(BuriedTreasure treasure)
    {
        _target = treasure;
    }

    public void Trigger()
    {
        Triggered?.Invoke();
        _timeLastUsed = Time.time;
    }
    
    public bool IsOnCooldown => _timeLastUsed +  COOLDOWN_TIME > Time.time;
}