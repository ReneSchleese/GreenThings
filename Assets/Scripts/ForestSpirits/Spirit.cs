using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using UnityEngine;

namespace ForestSpirits
{
    public class Spirit : MonoBehaviour, IPushable
    {
        [SerializeField] public CharacterController Controller;
        [SerializeField] private PushHitbox _pushHitbox;
        [SerializeField] private Actor _actor;
        [SerializeField] private Transform _targetLookRotator;
        [SerializeField] private AudioClip[] _followPlayerClips;
        [SerializeField] private AudioClip[] _unfoldingClips;
        private State _currentState;
        private List<State> _states;
        private static PseudoRandomIndex _followPlayerClipIndex;
        private static PseudoRandomIndex _unfoldingClipIndex;

        private void Awake()
        {
            SetupStates();
            SwitchToState(typeof(IdleState));
            _pushHitbox.Init(this);
            _actor.transform.SetParent(transform.parent);

            _followPlayerClipIndex ??= new PseudoRandomIndex(_followPlayerClips.Length);
            _unfoldingClipIndex ??= new PseudoRandomIndex(_unfoldingClips.Length);
        }

        private void SetupStates()
        {
            _states = new List<State>
            {
                new IdleState(),
                new FollowPlayerState(),
                new ChainLinkState(),
                new FlowerState()
            };
            foreach (State state in _states)
            {
                state.Init(spirit: this, _actor, SwitchToState);
            }
        }

        public void SwitchToState(Type state)
        {
            State stateBefore = _currentState;
            _currentState?.OnExit();
            _currentState = _states.First(s => s.GetType() == state);
            _currentState.OnEnter();
            
            if (stateBefore is IdleState && state == typeof(FollowPlayerState))
            {
                AudioManager.Instance.PlayEffect(_followPlayerClips[_followPlayerClipIndex.Get()]);
            }
            if (state == typeof(FlowerState))
            {
                AudioManager.Instance.PlayEffect(_unfoldingClips[_unfoldingClipIndex.Get()]);
            }

            if (state == typeof(ChainLinkState))
            {
                Priority = 1 + App.Instance.Player.Chain.GetIndex(this);
                Debug.Log(Priority);
            }
        }

        private void Update()
        {
            Controller.Move(Controller.isGrounded ? Vector3.zero : Physics.gravity * Time.deltaTime);
            _currentState.OnUpdate();
            _actor.SmoothSetPosition(Position);
            if (_currentState.GetType() != typeof(IdleState))
            {
                _actor.SmoothLookAt(App.Instance.Player.Position);
            }
            IChainTarget chainTarget = App.Instance.Player.Chain.GetTargetFor(this);
            if (chainTarget != null)
            {
                _targetLookRotator.LookAt(chainTarget.Position, Vector3.up);
            }
        }

        public void Push(Vector3 direction)
        {
            Controller.Move(direction);
        }

        public void HandleCollision(float radius, IPushable otherPushable)
        {
            if (!otherPushable.IsPushable)
            {
                return;
            }

            Vector3 pushBackDir = otherPushable.Transform.position - transform.position;
            Vector3 pushDirection = pushBackDir;
            
            if (TargetDir.HasValue && otherPushable.TargetDir.HasValue )
            {
                var dot = Vector3.Dot(TargetDir.Value.normalized, otherPushable.TargetDir.Value.normalized);
                bool haveOpposingTargets = dot < 0;
                if (haveOpposingTargets)
                {
                    Vector3 otherPositionInLocalSpace = _targetLookRotator.InverseTransformPoint(otherPushable.Transform.position);
                    Vector3 pushToSideDir = Quaternion.AngleAxis(otherPositionInLocalSpace.x < 0f ? -90 : 90, Vector3.up) * TargetDir.Value.normalized;
                    pushDirection = pushToSideDir;
                }
            }
            
            var distance = Vector3.Distance(transform.position, otherPushable.Transform.position);
            var lerpPushStrength = Mathf.Max(0.05f, Mathf.Lerp(3f, 0f, distance / radius));
            Debug.Log($"HandleCollision, distance / radius={distance / radius}, distance={distance}, radius={radius}");


            Vector3 pushDirectionNormalized = pushDirection.normalized;
            otherPushable.Push(pushDirectionNormalized * lerpPushStrength);
            Push(-pushDirectionNormalized * lerpPushStrength);
            Debug.DrawRay(transform.position, pushDirectionNormalized * lerpPushStrength * 3f, Color.blue);
        }

        public Vector3 Position => transform.position;

        public Vector3? TargetDir
        {
            get
            {
                IChainTarget chainTarget = App.Instance.Player.Chain.GetTargetFor(this);
                return chainTarget == null ? null : chainTarget.Position - transform.position;
            }
        }

        public Transform Transform => transform;

        public bool IsPushable => true;

        public int Priority { get; private set; }

        public Vector3 Velocity => _actor.Velocity;
        public PushHitbox PushHitbox => _pushHitbox;
    }
}