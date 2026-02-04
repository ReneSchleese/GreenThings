using System;
using DG.Tweening;
using UnityEngine;

public class BuriedTreasure : MonoBehaviour
{
    [SerializeField] private Transform _animationContainer;
    public event Action<BuriedTreasure> Opened;
    private const int INITIAL_HEALTH = 3;
    private const float DEPTH_PER_HEALTH = 0.5f;
    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = INITIAL_HEALTH;
        UpdateVisuals(animate: false);
    }

    public void OnBeingDug()
    {
        _currentHealth = Mathf.Clamp(_currentHealth - 1, 0, INITIAL_HEALTH);
        UpdateVisuals(animate: true);
        if (!IsOpen && _currentHealth == 0)
        {
            IsOpen = true;
            DOVirtual.DelayedCall(1f, () => Opened?.Invoke(this));
        }
    }

    private void UpdateVisuals(bool animate)
    {
        if (animate)
        {
            DOTween.Kill(this);
            _animationContainer
                .DOLocalMove(Vector3.down * _currentHealth * DEPTH_PER_HEALTH, 0.5f)
                .SetId(this)
                .SetEase(Ease.OutBack);
        }
        else
        {
            _animationContainer.localPosition = Vector3.down * _currentHealth * DEPTH_PER_HEALTH;   
        }
    }

    public bool IsFullHealth =>  _currentHealth == INITIAL_HEALTH;
    public bool IsOpen { get; private set; }
}