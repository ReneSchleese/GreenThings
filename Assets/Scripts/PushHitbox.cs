using UnityEngine;

public class PushHitbox : MonoBehaviour
{
    [SerializeField] private bool _pushable;
    private const float PUSH_STRENGTH = 0.1f;
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
        if (!otherHitbox.Pushable)
        {
            return;
        }
        Vector3 direction = otherHitbox.Position - Position;
        direction = Quaternion.AngleAxis(-20, Vector3.up) * direction;
        otherHitbox.Push(direction.normalized * PUSH_STRENGTH);
    }

    private void Push(Vector3 direction)
    {
        _pusher.Position += direction;
    }

    private Vector3 Position => _pusher.Position;
    private bool Pushable => _pushable;
}

public interface IPusher
{
    public Vector3 Speed { get; }
    public Vector3 Position { get; set; }
}