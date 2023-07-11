using System.Collections;
using DG.Tweening;
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
    private Tween _lookTween;
    private Coroutine _stationaryRoutine;
    private bool _isStationary;

    public void SmoothSetPosition(Vector3 position)
    {
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.SmoothDamp(currentPosition, position, ref _posDampVelocity, 0.15f);
        Speed = (currentPosition - _lastPosition).magnitude * (1f / Time.deltaTime);

        _animator.SetFloat(AnimationIds.WalkingSpeed, Speed);
        _lastPosition = currentPosition;

        if (SlowEnoughForStationary())
        {
            if (_isStationary)
            {
                return;
            }
            _stationaryRoutine ??= StartCoroutine(WaitThenTrySetStationary());
        }
        else
        {
            _isStationary = false;
            if (_stationaryRoutine != null)
            {
                StopStationaryRoutine();
            }
        }

        IEnumerator WaitThenTrySetStationary()
        {
            Debug.Log("Waiting...");
            yield return new WaitForSeconds(3);
            if (SlowEnoughForStationary())
            {
                Debug.Log("Unfold");
                _isStationary = true;
                _animator.SetTrigger(AnimationIds.Unfold);
                StopStationaryRoutine();
            }
        }
    }

    private bool SlowEnoughForStationary() => Speed <= 0.02f;

    public void SmoothLookAt(Vector3 position)
    {
        Vector3 targetPos = new(position.x, 0f, position.z);
        Quaternion lookRotation = Quaternion.LookRotation(targetPos - transform.position, Vector3.up);
        transform.rotation = Utils.SmoothDamp(transform.rotation, lookRotation, ref _rotDampVelocity, 0.2f);
    }
    
    private void StopStationaryRoutine()
    {
        if (_stationaryRoutine != null)
        {
            StopCoroutine(_stationaryRoutine);
        }
        _stationaryRoutine = null;
    }

    private float Speed { get; set; }
}
