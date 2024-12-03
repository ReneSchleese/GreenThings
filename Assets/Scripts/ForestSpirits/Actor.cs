using System;
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

        public void SmoothSetPosition(Vector3 position)
        {
            transform.position = Vector3.SmoothDamp(transform.position, position, ref _posDampVelocity, 0.1f);
            Vector3 currentPosition = transform.position;
            Velocity = (currentPosition - _lastPosition) / Time.deltaTime;
            Speed = Velocity.magnitude;
            _lastPosition = currentPosition;
            _animator.SetFloat(AnimationIds.WalkingSpeed, Speed);
        }

        private void OnDrawGizmosSelected()
        {
            Debug.Log($"speed={Speed}");
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

        public float Speed { get; private set; }
        public Vector3 Velocity { get; private set; }
    }

}