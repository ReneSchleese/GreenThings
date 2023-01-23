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
        Debug.Log($"TRIGGERED, other={other.name}");
        if (other.TryGetComponent(out PushHitbox otherHitbox))
        {
            Vector3 direction = otherHitbox.WorldPosition - WorldPosition;
            float distance = Mathf.Abs(direction.magnitude - otherHitbox.Radius - Radius);
            Debug.Log($"direction={direction}, distance={distance}");
            //Debug.Assert(distance < 0);
            otherHitbox.Push(direction.normalized * 0.01f);
            //Push(-direction.normalized * 0.01f);
        }
    }

    public void Push(Vector3 direction)
    {
        _target.position += direction;
    }

    public Vector3 WorldPosition => _target.position;
    public float Radius => _capsuleCollider.radius;
}