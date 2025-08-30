using DG.Tweening;
using UnityEngine;

public class BuriedTreasure : MonoBehaviour
{
    [SerializeField] private Transform _animationContainer;
    private const int INITIAL_HEALTH = 3;
    private const float DEPTH_PER_HEALTH = 0.5f;
    private int _currentHealth;
    private bool _isOpen;

    private void Awake()
    {
        _currentHealth = INITIAL_HEALTH;
        UpdateVisuals(animate: false);
    }

    public void OnBeingDug()
    {
        _currentHealth = Mathf.Clamp(_currentHealth - 1, 0, INITIAL_HEALTH);
        UpdateVisuals(animate: true);
        if (!_isOpen && _currentHealth == 0)
        {
            _isOpen = true;
            DOVirtual.DelayedCall(1f, Open);
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

    private void Open()
    {
        int count = 10;
        const float upToSidewaysWeight = 0.15f;
        for (int i = 0; i < count; i++)
        {
            Coin coin = Game.Instance.Spawner.SpawnCoin(transform.position, Quaternion.identity);
            float angle = i * Mathf.PI * 2f / count;
            Vector3 dir = (1f - upToSidewaysWeight) * Vector3.up + upToSidewaysWeight * new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            const float strength = 1.75f;
            coin.ApplyForce(dir.normalized * strength * Physics.gravity.magnitude);
            coin.GroundedCheckIsEnabled = false;
            DOVirtual.DelayedCall(0.2f, () =>
            {
                coin.GroundedCheckIsEnabled = true;
                coin.IsCollectable = true;
            });
        }
    }
}