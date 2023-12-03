using ForestSpirits;
using UnityEngine;

public class PushHitbox : MonoBehaviour
{
    [SerializeField] private bool _pushable;
    private const float PUSH_STRENGTH = 0.3f;
    private IPusher _pusher;

    public void Init(IPusher pusher)
    {
        _pusher = pusher;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out PushHitbox otherHitbox))
        {
            return;
        }

        if (!otherHitbox.GetComponentInParent<Spirit>())
        {
            return;
        }

        IPusher pusher = otherHitbox.GetComponentInParent<Spirit>();
        if (!otherHitbox.Pushable)
        {
            return;
        }

        Vector3 direction = pusher.Transform.position - Position;
        if(_pusher.Velocity.magnitude > 0.01f)
        {
            Vector3 forwardBefore = _pusher.Transform.forward;
            _pusher.Transform.forward = _pusher.Velocity;
            Vector3 otherPositionInLocalSpace = _pusher.Transform.InverseTransformPoint(pusher.Transform.position);
            _pusher.Transform.forward = forwardBefore;
            Debug.Log($"otherSpace={otherPositionInLocalSpace.x}");
            direction = Quaternion.AngleAxis(otherPositionInLocalSpace.x < 0f ? -90 : 90, Vector3.up) *
                        _pusher.Velocity;
            Debug.DrawRay(transform.position, direction, Color.red, 1f);
            if (gameObject.name.Contains("PlayerCharacter"))
            {
                Debug.Log(nameof(OnTriggerStay) + $", direction={direction}");
            }
        }
        pusher.Push(direction.normalized * PUSH_STRENGTH);
    }

    // private void Push(Vector3 direction)
    // {
        // _pusher.Transform.position += direction;
    // }

    private Vector3 Position => _pusher.Transform.position;
    private bool Pushable => _pushable;
}

public interface IPusher
{
    public Vector3 Velocity { get; }
    public Transform Transform { get; }
    public void Push(Vector3 direction);
    public bool Pushable { get; }
}