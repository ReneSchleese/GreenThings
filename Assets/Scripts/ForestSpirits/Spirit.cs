using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForestSpirits
{
    public class Spirit : MonoBehaviour, IPushable
    {
        [SerializeField] public CharacterController Controller;
        [SerializeField] private PushHitbox _pushHitbox;
        [SerializeField] private Actor _actor;
        private State _currentState;
        private List<State> _states;
        private Vector3 _positionLastFrame;

        private void Awake()
        {
            SetupStates();
            SwitchToState(typeof(IdleState));
            _pushHitbox.Init(this);
            _actor.transform.SetParent(null);
        }

        private void SetupStates()
        {
            _states = new List<State>
            {
                new IdleState(),
                new FollowPlayerState(),
                new ChainLinkState()
            };
            foreach (State state in _states)
            {
                state.Init(spirit: this, SwitchToState);
            }
        }

        public void SwitchToState(Type state)
        {
            _currentState?.OnExit();
            _currentState = _states.First(s => s.GetType() == state);
            _currentState.OnEnter();
        }

        private void Update()
        {
            _currentState.OnUpdate();
            _actor.SmoothSetPosition(Position);
            _actor.HandleUnfold(_currentState);
            if (_currentState.GetType() != typeof(IdleState))
            {
                _actor.SmoothLookAt(App.Instance.Player.Position);
            }

            Velocity = (Position - _positionLastFrame) / Time.deltaTime;
            _positionLastFrame = Position;
        }

        public Vector3 Position => transform.position;

        public Transform Transform => transform;
        public void Push(Vector3 direction)
        {
            Controller.Move(direction);
        }

        public bool IsPushable => true;

        public Vector3 Velocity { get; private set; }
        public PushHitbox PushHitbox => _pushHitbox;
    }
}