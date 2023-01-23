using System;
using System.Collections.Generic;
using System.Linq;
using ForestSpirits;
using UnityEngine;

public class ForestSpirit : MonoBehaviour, IFollowable
{
    [SerializeField] public CharacterController CharacterController;
    [SerializeField] private PushHitbox _pushHitbox;
    private State _currentState;
    private List<State> _states;
    [SerializeField] private string _stateString;
    
    private void Awake()
    {
        SetupStates();
        SwitchToState(typeof(IdleState));
        _pushHitbox.Init(transform);
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
        transform.position = new Vector3(transform.position.x, 1.17f, transform.position.z);
        /*
         * if not following
         *      if player in range
         *          move closer to player
         *          start following player
         * else if following player
         *      if needs to catch up
         *          if is first spirit
         *              move to player position
         *          else
         *              follow last spirit
         * else if following spirit
         *      if player in range
         *          follow player
         *      if needs to catch up to spirit
         *          move to spirit position
         */
    }

    public Vector3 WorldPosition => transform.position;

    public bool IsFollowing => _currentState != null && (_currentState.GetType() == typeof(FollowPlayerState) ||
                                                         _currentState.GetType() == typeof(EnqueuedState));
}