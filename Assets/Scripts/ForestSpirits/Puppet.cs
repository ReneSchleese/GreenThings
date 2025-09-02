using DG.Tweening;
using UnityEngine;

namespace ForestSpirits
{
    public class Puppet : MonoBehaviour
    {
        private static class AnimationIds
        {
            public static readonly int WalkingSpeed = Animator.StringToHash("WalkingSpeed");
            public static readonly int Unfold = Animator.StringToHash("Unfold");
            public static readonly int WalkingOffset = Animator.StringToHash("WalkingOffset");
        }

        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteBlobShadow _blobShadow;
        [SerializeField] private Transform _animationContainer;
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;

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
            _animator.SetFloat(AnimationIds.WalkingSpeed, Speed > 0.5f? Speed : 0f);
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
            DOTween.Kill(this, complete: true);
            var relativeSpeed = Mathf.InverseLerp(0f, 7f, Speed);
            float duration = Mathf.Clamp(relativeSpeed * 0.3f, 0.18f, 0.3f);
            _animationContainer.DOPunchPosition(Vector3.up * Mathf.Clamp(relativeSpeed, 0.2f, 0.7f), duration, 2)
                .OnComplete(() => _animationContainer.localPosition = Vector3.zero)
                .SetId(this);
        }
        
        public void OnScan(BuriedTreasure treasure)
        {
            if (treasure == null)
            {
                return;
            }

            string id = $"{GetInstanceID()}_Scan";
            DOTween.Kill(id);
            
            float distance = Vector3.Distance(transform.position, treasure.transform.position);
            const float distanceMin = 2f;
            const float distanceMax = 14f;
            float inverseLerp = Mathf.InverseLerp(distanceMax, distanceMin, distance);
            Sequence sequence = DOTween.Sequence().SetId(id);
            const float duration = 1f;
            sequence.InsertCallback(0, () => _animator.SetTrigger(AnimationIds.Unfold));
            sequence.Insert(0.3f, DOVirtual.Float(NormalizedScanProgress, 1f, 0.05f, value => NormalizedScanProgress = value));
            sequence.Insert(0.4f, DOVirtual.Float(1f, 0f, duration - 0.4f, value => NormalizedScanProgress = value));
            sequence.OnUpdate(() =>
            {
                _meshRenderer.material.SetFloat(ScanNormalized, NormalizedScanProgress * inverseLerp);
            });
        }

        private static readonly int ScanNormalized = Shader.PropertyToID("_ScanNormalized");
        public float Speed { get; private set; }
        public Vector3 Velocity { get; private set; }
        public SpriteBlobShadow BlobShadow => _blobShadow;
        public float NormalizedWalkingOffset
        {
            set => _animator.SetFloat(AnimationIds.WalkingOffset, Mathf.Clamp01(value));
        }
        private float NormalizedScanProgress { get; set; }
    }

}