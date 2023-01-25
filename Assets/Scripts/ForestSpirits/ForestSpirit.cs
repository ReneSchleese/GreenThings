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
    [SerializeField] private GameObject _viewPrefab;
    private State _currentState;
    private List<State> _states;
    private GameObject _view;

    private void Awake()
    {
        SetupStates();
        SwitchToState(typeof(IdleState));
        _pushHitbox.Init(transform);
        _view = Instantiate(_viewPrefab, transform.position, Quaternion.identity);
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

    private Vector3 _velocity;
    private void Update()
    {
        _currentState.OnUpdate();
        transform.position = new Vector3(transform.position.x, 1.17f, transform.position.z);
        _view.transform.position = Vector3.SmoothDamp(_view.transform.position, transform.position, ref _velocity, 0.15f);
    }

    public Vector3 WorldPosition => transform.position;

    public bool IsFollowing => _currentState != null && (_currentState.GetType() == typeof(FollowPlayerState) ||
                                                         _currentState.GetType() == typeof(EnqueuedState));
}