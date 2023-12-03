using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForestSpirits
{
    public class Spirit : MonoBehaviour, IPusher
    {
        [SerializeField] public CharacterController Controller;
        [SerializeField] private PushHitbox _pushHitbox;
        [SerializeField] private Actor _actor;
        private State _currentState;
        private List<State> _states;
        private Vector3 _velocity;

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
            Vector3 position = transform.position;
            Position = new Vector3(position.x, 0f, position.z);

            _actor.SmoothSetPosition(Position);
            _actor.HandleUnfold(_currentState);
            if (_currentState.GetType() != typeof(IdleState))
            {
                _actor.SmoothLookAt(App.Instance.Player.Position);
            }
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Vector3 Speed { get; }
    }
}