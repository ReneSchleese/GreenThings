using DG.Tweening;
using UnityEngine;

public class ForestSpiritBody : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    private static readonly int WalkingSpeedAnimationId = Animator.StringToHash("WalkingSpeed");
    private const float WALKING_SPEED_FACTOR = 22500f;
    private Vector3 _lastPosition;
    private Vector3 _velocity;

    private Tween _lookTween;

    public void SmoothSetPosition(Vector3 position)
    {
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.SmoothDamp(currentPosition, position, ref _velocity, 0.15f);
        Speed = (currentPosition - _lastPosition).sqrMagnitude * Time.deltaTime * WALKING_SPEED_FACTOR;
        _animator.SetFloat(WalkingSpeedAnimationId, Speed);
        _lastPosition = currentPosition;
    }

    public void SmoothLookAt(Vector3 position)
    {
        LookAtPos = new Vector3(position.x, 0f, position.z);
        Quaternion lookRotation = Quaternion.LookRotation(LookAtPos - transform.position, Vector3.up);
        if (_lookTween == null || _lookTween.active == false)
        {
            _lookTween = transform.DORotateQuaternion(lookRotation, 0.5f).SetEase(Ease.Linear);
        }
    }

    private Vector3 LookAtPos { get; set; }
    private float Speed { get; set; }
}
