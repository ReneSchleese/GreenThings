using System;
using System.Collections.Generic;
using System.Linq;
using ForestSpirits;
using UnityEngine;

public class ForestSpirit : MonoBehaviour, IFollowable
{
    [SerializeField] public CharacterController CharacterController;
    [SerializeField] private PushHitbox _pushHitbox;
    [SerializeField] private string _stateString;
    [SerializeField] private ForestSpiritBody _body;
    private State _currentState;
    private List<State> _states;
    private Vector3 _velocity;

    private void Awake()
    {
        SetupStates();
        SwitchToState(typeof(IdleState));
        _pushHitbox.Init(transform);
        _body.transform.SetParent(null);
    }

    private void SetupStates()
    {
        _states = new List<State>
        {
            new IdleState(),
            new FollowPlayerState(),
            new EnqueuedState()
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
        _stateString = _currentState.GetType().ToString();
    }

    private void Update()
    {
        _currentState.OnUpdate();
        Vector3 position = transform.position;
        WorldPosition = new Vector3(position.x, 0f, position.z);
        _body.SmoothSetPosition(WorldPosition);
        if (_currentState is EnqueuedState or FollowPlayerState)
        {
            _body.SmoothLookAt(App.Instance.Player.transform.position);
        }
    }

    public Vector3 WorldPosition
    {
        get => transform.position;
        private set => transform.position = value;
    }
}