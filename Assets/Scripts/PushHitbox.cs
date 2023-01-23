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
        float distance = Mathf.Abs(direction.magnitude - otherHitbox.Radius - Radius);
        //Debug.Log($"direction={direction}, distance={distance}");
        otherHitbox.Push(direction.normalized * 0.05f);
    }

    private void Push(Vector3 direction)
    {
        _target.position += direction;
    }

    private Vector3 WorldPosition => _target.position;
    private float Radius => _capsuleCollider.radius;
}