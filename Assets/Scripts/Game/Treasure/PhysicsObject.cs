using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private SphereCollider _collider;
    
    private bool _isGrounded;
    private bool _groundedCheckIsEnabled;

    private void Start()
    {
        // use custom gravity
        _rigidbody.useGravity = false;
    }
    
    private void FixedUpdate()
    {
        const float OFFSET = 0.1f;
        if(GroundedCheckIsEnabled)
        {
            float distance = _collider.radius + OFFSET;
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distance, LayerMask.GetMask("Default"));
            _isGrounded = hit.collider is not null;
            if (_isGrounded)
            {
                transform.position = hit.point + Vector3.up * _collider.radius;
                // dampen
                _rigidbody.linearVelocity = new Vector3(0.5f * _rigidbody.linearVelocity.x, _rigidbody.linearVelocity.y, 0.5f * _rigidbody.linearVelocity.z);
            }
        }

        if(!_isGrounded)
        {
            // use custom gravity
            Vector3 gravity = Physics.gravity * 3f;
            _rigidbody.AddForce(gravity, ForceMode.Acceleration);       
        }
    }
    
    public void ApplyForce(Vector3 force) => _rigidbody.AddForce(force, ForceMode.Impulse);
    
    public bool GroundedCheckIsEnabled
    {
        get => _groundedCheckIsEnabled;
        set
        {
            _groundedCheckIsEnabled = value;
            if (!value)
            {
                _isGrounded = false;
            }
        }
    }
}