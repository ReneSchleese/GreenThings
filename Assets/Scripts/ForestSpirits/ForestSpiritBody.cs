using UnityEngine;

public class ForestSpiritBody : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    private static readonly int WalkingSpeedAnimationId = Animator.StringToHash("WalkingSpeed");
    private const float WALKING_SPEED_FACTOR = 22500f;
    private Vector3 _lastPosition;
    private Vector3 _velocity;

    public void SmoothSetPosition(Vector3 position)
    {
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.SmoothDamp(currentPosition, position, ref _velocity, 0.15f);
        Speed = (currentPosition - _lastPosition).sqrMagnitude * Time.deltaTime * WALKING_SPEED_FACTOR;
        _animator.SetFloat(WalkingSpeedAnimationId, Speed);
        _lastPosition = currentPosition;
    }

    public void LookAt(Vector3 position)
    {
        transform.LookAt(position);
    }

    private float Speed { get; set; }
}
