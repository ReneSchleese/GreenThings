using UnityEngine;

public class PushHitbox : MonoBehaviour
{
    private const float PUSH_STRENGTH = 0.2f;

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

        Vector3 direction = otherPushable.Transform.position - Position;
        float dot = Vector3.Dot(Pushable.Transform.forward, otherPushable.Transform.forward);
        if(Pushable.Velocity.sqrMagnitude > 0.1f && dot < 0f)
        {
            Vector3 otherPositionInLocalSpace = Pushable.Transform.InverseTransformPoint(otherPushable.Transform.position);
            direction = Quaternion.AngleAxis(otherPositionInLocalSpace.x < 0f ? -90 : 90, Vector3.up) * Pushable.Transform.forward;
        }
        otherPushable.Push(direction.normalized * PUSH_STRENGTH);
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