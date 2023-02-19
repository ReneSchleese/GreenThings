using UnityEngine;

public class ForestSpiritBody : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    private const float WALKING_SPEED_FACTOR = 25000f;
    private Vector3 _lastPosition;
    private static readonly int WalkingSpeedAnimationId = Animator.StringToHash("WalkingSpeed");

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        Speed = (currentPosition - _lastPosition).sqrMagnitude * Time.deltaTime * WALKING_SPEED_FACTOR;
        _animator.SetFloat(WalkingSpeedAnimationId, Speed);
        _lastPosition = currentPosition;
    }

    private float Speed { get; set; }
}
