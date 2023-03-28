using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ForestSpiritBody : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    private static readonly int WalkingSpeedAnimationId = Animator.StringToHash("WalkingSpeed");
    private const float WALKING_SPEED_FACTOR = 22500f;
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
        Speed = (currentPosition - _lastPosition).sqrMagnitude * Time.deltaTime * WALKING_SPEED_FACTOR;
        _animator.SetFloat(WalkingSpeedAnimationId, Speed);
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
                _animator.SetTrigger("Unfold");
                StopStationaryRoutine();
            }
        }
    }

    private bool SlowEnoughForStationary()
    {
        return Speed <= 0.2f;
    }

    public void SmoothLookAt(Vector3 position)
    {
        Vector3 targetPos = new(position.x, 0f, position.z);
        Quaternion lookRotation = Quaternion.LookRotation(targetPos - transform.position, Vector3.up);
        transform.rotation = SmoothDamp(transform.rotation, lookRotation, ref _rotDampVelocity, 0.2f);
    }
    
    // Taken from here: https://gist.github.com/maxattack/4c7b4de00f5c1b95a33b
    public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time) {
        if (Time.deltaTime < Mathf.Epsilon) return rot;
        // account for double-cover
        var Dot = Quaternion.Dot(rot, target);
        var Multi = Dot > 0f ? 1f : -1f;
        target.x *= Multi;
        target.y *= Multi;
        target.z *= Multi;
        target.w *= Multi;
        // smooth damp (nlerp approx)
        var Result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
            Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
            Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
            Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
        ).normalized;
		
        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
        deriv.x -= derivError.x;
        deriv.y -= derivError.y;
        deriv.z -= derivError.z;
        deriv.w -= derivError.w;		
		
        return new Quaternion(Result.x, Result.y, Result.z, Result.w);
    }

    private void StopStationaryRoutine()
    {
        if (_stationaryRoutine != null)
        {
            Debug.Log("Stopped waiting");
            StopCoroutine(_stationaryRoutine);
            _stationaryRoutine = null;
        }
    }

    private Vector3 LookAtPos { get; set; }
    private float Speed { get; set; }
}
