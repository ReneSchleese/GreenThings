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
            _actor.transform.SetParent(null);

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
        }

        public Vector3 Position => transform.position;

        public Transform Transform => transform;
        public void Push(Vector3 direction)
        {
            Controller.Move(direction);
        }

        public bool IsPushable => true;
        public void HandleCollision(IPushable otherPushable)
        {
            Vector3 otherPositionInLocalSpace = Transform.InverseTransformPoint(otherPushable.Transform.position);
            Vector3 pushDirection;
            Vector3 pushToSideDir = Quaternion.AngleAxis(otherPositionInLocalSpace.x < 0f ? -30 : 30, Vector3.up) * Transform.forward;
            Vector3 pushBackDir = otherPushable.Transform.position - Transform.position;
            const float pushStrength = 0.075f;

            if (otherPushable.IsPushable)
            {
                bool pushBack = Velocity.magnitude < 5f;
                pushDirection = pushBack ? pushBackDir : pushToSideDir;
                otherPushable.Push(pushDirection.normalized * pushStrength);
                //Debug.DrawRay(transform.position, pushDirection * 3f, pushBack ? Color.blue : Color.red);
            }
            if (IsPushable)
            {
                bool pushBack = otherPushable.Velocity.magnitude < 5f;
                pushDirection = pushBack ? -pushBackDir : -pushToSideDir;
                Push(pushDirection.normalized * pushStrength * 0.5f);
                //Debug.DrawRay(transform.position, pushDirection * 3f, pushBack ? Color.blue : Color.red);
            }
        }

        public Vector3 Velocity => _actor.Velocity;
        public PushHitbox PushHitbox => _pushHitbox;
    }
}