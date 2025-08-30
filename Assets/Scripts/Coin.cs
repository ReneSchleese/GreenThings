using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private Transform _rotationAnimationContainer;
    [SerializeField] private Transform _hoverAnimationContainer;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField]  private SphereCollider _collider;
    [SerializeField] private SpriteBlobShadow _blobShadow;
    private bool _isGrounded;
    
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
        const float OFFSET = 0.1f;
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _collider.radius + OFFSET);
        _isGrounded = hit.collider != null;
        if (_isGrounded)
        {
            transform.position = hit.point + Vector3.up * _collider.radius;
        }
        else
        {
            Vector3 gravity = Physics.gravity * 3f;
            _rigidbody.AddForce(gravity, ForceMode.Acceleration);       
        }
    }
    
    public void ApplyForce(Vector3 force) => _rigidbody.AddForce(force, ForceMode.Impulse);
}