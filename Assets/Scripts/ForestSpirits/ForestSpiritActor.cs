using UnityEngine;

public class ForestSpiritActor : MonoBehaviour
{
    private static class AnimationIds
    {
        public static readonly int WalkingSpeed = Animator.StringToHash("WalkingSpeed");
        public static readonly int Unfold = Animator.StringToHash("Unfold");   
    }

    [SerializeField] private Animator _animator;
    
    private Vector3 _lastPosition;
    private Vector3 _posDampVelocity;
    private Quaternion _rotDampVelocity;

    private bool _isInUnfoldState;
    private float _timeStampWhereFast;

    public void SmoothSetPosition(Vector3 position)
    {
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.SmoothDamp(currentPosition, position, ref _posDampVelocity, 0.15f);
        Speed = (currentPosition - _lastPosition).magnitude * (1f / Time.deltaTime);
        _lastPosition = currentPosition;
        
        _animator.SetFloat(AnimationIds.WalkingSpeed, Speed);
        HandleUnfold();
    }

    private void HandleUnfold()
    {
        if (!IsSlowEnoughToUnfold())
        {
            _timeStampWhereFast = Time.time;
            _isInUnfoldState = false;
        }

        if (!_isInUnfoldState && HasBeenSlowLongEnoughToUnfold())
        {
            Unfold();
        }
        
        bool IsSlowEnoughToUnfold() => Speed <= 0.02f;
        bool HasBeenSlowLongEnoughToUnfold() => Time.time - _timeStampWhereFast > 3.0f;
    }

    private void Unfold()
    {
        _isInUnfoldState = true;
        _animator.SetTrigger(AnimationIds.Unfold);
    }
    
    public void SmoothLookAt(Vector3 position)
    {
        Vector3 targetPos = new(position.x, 0f, position.z);
        Quaternion lookRotation = Quaternion.LookRotation(targetPos - transform.position, Vector3.up);
        transform.rotation = Utils.SmoothDamp(transform.rotation, lookRotation, ref _rotDampVelocity, 0.2f);
    }

    private float Speed { get; set; }
}
