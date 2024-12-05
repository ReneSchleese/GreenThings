using UnityEngine;

public class PushHitbox : MonoBehaviour
{
    [SerializeField] private CapsuleCollider _collider;

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
        Pushable.HandleCollision(_collider.radius, otherPushable);
    }

    public float Radius
    {
        get => _collider.radius;
        set => _collider.radius = value;
    }

    private IPushable Pushable { get; set; }
}

public interface IPushable
{
    public Vector3 Velocity { get; }
    public Vector3? TargetDir { get; }
    public Transform Transform { get; }
    public void Push(Vector3 direction);
    public bool IsPushable { get; }
    public int Priority { get; }
    public void HandleCollision(float radius, IPushable otherPushable);
}