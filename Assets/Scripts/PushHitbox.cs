using UnityEngine;

public class PushHitbox : MonoBehaviour
{
    [SerializeField] private CapsuleCollider _collider;
    private const float PUSH_STRENGTH = 0.15f;

    public void Init(IPushable pushable)
    {
        Pushable = pushable;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out PushHitbox otherHitbox))
        {
            return;
        }

        IPushable otherPushable = otherHitbox.Pushable;
        Vector3 otherPositionInLocalSpace = Pushable.Transform.InverseTransformPoint(otherPushable.Transform.position);
        Vector3 pushDirection;
        Vector3 pushToSideDir = Quaternion.AngleAxis(otherPositionInLocalSpace.x < 0f ? -45 : 45, Vector3.up) * Pushable.Transform.forward;
        Vector3 pushBackDir = otherPushable.Transform.position - Pushable.Transform.position;

        Debug.Log(otherPushable.Velocity.magnitude);
        if (otherPushable.IsPushable)
        {
            if (Pushable.Velocity.magnitude < 4f)
            {
                // push back
                pushDirection = pushBackDir;
                otherPushable.Push(pushDirection.normalized * PUSH_STRENGTH);
                Debug.DrawRay(transform.position, pushDirection * 3f, Color.blue);
            }
            else
            {
                // push to side
                pushDirection = pushToSideDir;
                otherPushable.Push(pushDirection.normalized * PUSH_STRENGTH);
                Debug.DrawRay(transform.position, pushDirection * 3f, Color.red);
            }
        }
        if (Pushable.IsPushable)
        {
            if (otherPushable.Velocity.magnitude < 4f)
            {
                // push back
                pushDirection = -pushBackDir;
                Pushable.Push(pushDirection.normalized * PUSH_STRENGTH * 0.5f);
                Debug.DrawRay(transform.position, pushDirection * 3f, Color.blue);
            }
            else
            {
                // push to side
                pushDirection = -pushToSideDir;
                Pushable.Push(pushDirection.normalized * PUSH_STRENGTH * 0.5f);
                Debug.DrawRay(transform.position, pushDirection * 3f, Color.red);
            }
        }
    }

    public float Radius
    {
        get => _collider.radius;
        set => _collider.radius = value;
    }

    private Vector3 Position => Pushable.Transform.position;
    private IPushable Pushable { get; set; }
}

public interface IPushable
{
    public Vector3 Velocity { get; }
    public Transform Transform { get; }
    public void Push(Vector3 direction);
    public bool IsPushable { get; }
}