using ForestSpirits;
using UnityEngine;

public class PushHitbox : MonoBehaviour
{
    [SerializeField] private bool _isPushable;
    private const float PUSH_STRENGTH = 0.2f;
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

        if (!otherHitbox.GetComponentInParent<Spirit>())
        {
            return;
        }

        IPushable pushable = otherHitbox.GetComponentInParent<Spirit>();
        if (!otherHitbox.IsPushable)
        {
            return;
        }

        Vector3 direction = pushable.Transform.position - Position;
        if(_pushable.Velocity.magnitude > 0.01f)
        {
            Vector3 forwardBefore = _pushable.Transform.forward;
            _pushable.Transform.forward = _pushable.Velocity;
            Vector3 otherPositionInLocalSpace = _pushable.Transform.InverseTransformPoint(pushable.Transform.position);
            _pushable.Transform.forward = forwardBefore;
            Debug.Log($"otherSpace={otherPositionInLocalSpace.x}");
            direction = Quaternion.AngleAxis(otherPositionInLocalSpace.x < 0f ? -90 : 90, Vector3.up) *
                        _pushable.Velocity;
            Debug.DrawRay(transform.position, direction, Color.red, 1f);
            if (gameObject.name.Contains("PlayerCharacter"))
            {
                Debug.Log(nameof(OnTriggerStay) + $", direction={direction}");
            }
        }
        pushable.Push(direction.normalized * PUSH_STRENGTH);
    }

    // private void Push(Vector3 direction)
    // {
        // _pushable.Transform.position += direction;
    // }

    private Vector3 Position => _pushable.Transform.position;
    private bool IsPushable => _isPushable;
}

public interface IPushable
{
    public Vector3 Velocity { get; }
    public Transform Transform { get; }
    public void Push(Vector3 direction);
    public bool Pushable { get; }
}