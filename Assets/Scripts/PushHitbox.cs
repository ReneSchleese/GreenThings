using UnityEngine;

public class PushHitbox : MonoBehaviour
{
    [SerializeField] private bool _isPushable;
    private const float PUSH_STRENGTH = 0.015f;
    private IPushable _pushable;

    public void Init(IPushable pushable)
    {
        _pushable = pushable;
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
        float otherPushStrength = 1f;
        float selfPushStrength = 1f;
        // we should probably check if both targets want to go in a similar direction. If so the approach below is worse
        if(_pushable.Velocity.magnitude > 0.01f)
        {
            Vector3 forwardBefore = _pushable.Transform.forward;
            _pushable.Transform.forward = _pushable.Velocity;
            Vector3 otherPositionInLocalSpace = _pushable.Transform.InverseTransformPoint(otherPushable.Transform.position);
            _pushable.Transform.forward = forwardBefore;
            direction = Quaternion.AngleAxis(otherPositionInLocalSpace.x < 0f ? -45 : 45, Vector3.up) *
                        _pushable.Velocity;
            otherPushStrength = (Pushable.Velocity - otherPushable.Velocity).magnitude;
            selfPushStrength = (otherPushable.Velocity - Pushable.Velocity).magnitude;
        }
        
        if(otherPushable.FrameCount < Time.frameCount)
        {
            otherPushable.Push(direction.normalized * otherPushStrength * PUSH_STRENGTH);
            otherPushable.FrameCount = Time.frameCount;
        }

        if (Pushable.IsPushable && Pushable.FrameCount < Time.frameCount)
        {
            Pushable.Push(-direction.normalized * selfPushStrength * PUSH_STRENGTH);
            Pushable.FrameCount = Time.frameCount;
        }
    }

    private Vector3 Position => _pushable.Transform.position;
    private IPushable Pushable => _pushable;
}

public interface IPushable
{
    public Vector3 Velocity { get; }
    public Transform Transform { get; }
    public void Push(Vector3 direction);
    public bool IsPushable { get; }
    public int FrameCount { get; set; }
}