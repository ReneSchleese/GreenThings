using DG.Tweening;
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
        [SerializeField] private SpriteBlobShadow _blobShadow;
        [SerializeField] private Transform _animationContainer;

        private Vector3 _lastPosition;
        private Vector3 _posDampVelocity;
        private Quaternion _rotDampVelocity;

        public void SmoothSetPosition(Vector3 position)
        {
            transform.position = Vector3.SmoothDamp(transform.position, position, ref _posDampVelocity, 0.1f);
            Vector3 currentPosition = transform.position;
            Velocity = (currentPosition - _lastPosition) / Time.deltaTime;
            Speed = Velocity.magnitude;
            _lastPosition = currentPosition;
            _animator.SetFloat(AnimationIds.WalkingSpeed, Speed);
        }

        public void Unfold()
        {
            _animator.SetTrigger(AnimationIds.Unfold);
        }

        public void SmoothLookAt(Vector3 position)
        {
            Vector3 direction = position - transform.position;
            Vector3 directionZeroY = new(direction.x, 0f, direction.z);
            Quaternion lookRotation = Quaternion.LookRotation(directionZeroY, Vector3.up);
            transform.rotation = Utils.SmoothDamp(transform.rotation, lookRotation, ref _rotDampVelocity, 0.2f);
        }

        public void BumpUpwards()
        {
            var relativeSpeed = Mathf.InverseLerp(0f, 7f, Speed);
            float duration = Mathf.Clamp(relativeSpeed * 0.3f, 0.18f, 0.3f);
            _animationContainer.DOPunchPosition(Vector3.up * Mathf.Clamp(relativeSpeed, 0.2f, 0.7f), duration, 2).SetId(this);
        }

        public float Speed { get; private set; }
        public Vector3 Velocity { get; private set; }
        public SpriteBlobShadow BlobShadow => _blobShadow;
    }

}