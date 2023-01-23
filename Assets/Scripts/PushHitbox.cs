using UnityEngine;

public class PushHitbox : MonoBehaviour
{
    [SerializeField] private CapsuleCollider _capsuleCollider;
    private Transform _target;

    public void Init(Transform target)
    {
        _target = target;
    }
        
    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out PushHitbox otherHitbox))
        {
            return;
        }
        Vector3 direction = otherHitbox.WorldPosition - WorldPosition;
        direction = Quaternion.AngleAxis(-20, Vector3.up) * direction;
        otherHitbox.Push(direction.normalized * 0.1f);
    }

    private void Push(Vector3 direction)
    {
        _target.position += direction;
    }

    private Vector3 WorldPosition => _target.position;
    private float Radius => _capsuleCollider.radius;
}