using UnityEngine;

public class PushHitbox : MonoBehaviour
{
    [SerializeField] private CapsuleCollider _collider;
    private const float PUSH_STRENGTH = 0.05f;

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
        if (!otherPushable.IsPushable)
        {
            return;
        }

        Vector3 otherPositionInLocalSpace = Pushable.Transform.InverseTransformPoint(otherPushable.Transform.position);
        Vector3 direction = Quaternion.AngleAxis(otherPositionInLocalSpace.x < 0f ? -90 : 90, Vector3.up) * Pushable.Transform.forward;
        Debug.DrawRay(transform.position, direction * 3f, Color.red);
        otherPushable.Push(direction.normalized * PUSH_STRENGTH);
        if (Pushable.IsPushable)
        {
            Pushable.Push(-direction.normalized * PUSH_STRENGTH);
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