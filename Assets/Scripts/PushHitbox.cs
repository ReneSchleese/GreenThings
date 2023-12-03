using UnityEngine;

public class PushHitbox : MonoBehaviour
{
    private const float PUSH_STRENGTH = 0.1f;
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

        if (other.GetComponent<PlayerCharacter>())
        {
            return;
        }
        Vector3 direction = otherHitbox.WorldPosition - WorldPosition;
        direction = Quaternion.AngleAxis(-20, Vector3.up) * direction;
        otherHitbox.Push(direction.normalized * PUSH_STRENGTH);
    }

    private void Push(Vector3 direction)
    {
        _target.position += direction;
    }

    private Vector3 WorldPosition => _target.position;
}