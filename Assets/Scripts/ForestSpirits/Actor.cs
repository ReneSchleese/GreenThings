using UnityEngine;

namespace ForestSpirits
{
    public class Actor : MonoBehaviour
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
            transform.position = Vector3.SmoothDamp(currentPosition, position, ref _posDampVelocity, 0.1f);
            Velocity = (currentPosition - _lastPosition) / Time.deltaTime;
            Speed = Velocity.magnitude;
            _lastPosition = currentPosition;
            _animator.SetFloat(AnimationIds.WalkingSpeed, Speed);
        }

        public void HandleUnfold(State state)
        {
            if (!IsSlowEnoughToUnfold() || !FollowsPlayer())
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
            bool FollowsPlayer() => state.GetType() == typeof(FollowPlayerState);
        }

        private void Unfold()
        {
            _isInUnfoldState = true;
            _animator.SetTrigger(AnimationIds.Unfold);
        }

        public void SmoothLookAt(Vector3 position)
        {
            Vector3 direction = position - transform.position;
            Vector3 directionZeroY = new(direction.x, 0f, direction.z);
            Quaternion lookRotation = Quaternion.LookRotation(directionZeroY, Vector3.up);
            transform.rotation = Utils.SmoothDamp(transform.rotation, lookRotation, ref _rotDampVelocity, 0.2f);
        }

        private float Speed { get; set; }
        public Vector3 Velocity { get; private set; }
    }

}