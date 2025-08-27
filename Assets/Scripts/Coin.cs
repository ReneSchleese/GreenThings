using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private Transform _rotationAnimationContainer;
    [SerializeField] private Transform _hoverAnimationContainer;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private SpriteBlobShadow _blobShadow;
    
    private void Start()
    {
        _rigidbody.useGravity = false;
        DOTween.Sequence(this)
            .Append(_rotationAnimationContainer.DOLocalRotate(Vector3.up * 360f, 2f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear))
            .SetLoops(-1);
    }

    private void Update()
    {
        _blobShadow.UpdateShadow();
    }

    private void FixedUpdate()
    {
        if (_blobShadow.IsGrounded) return;
        Vector3 gravity = Physics.gravity * 2f;
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }
    
    public void ApplyForce(Vector3 force) => _rigidbody.AddForce(force, ForceMode.Impulse);
}