using System;
using System.Collections.Generic;
using System.Linq;
using ForestSpirits;
using UnityEngine;

public class ForestSpirit : MonoBehaviour
{
    private State _currentState;
    private List<State> _states;
    
    private void Awake()
    {
        SetupStates();
        SwitchToState(typeof(IdleState));
    }

    private void SetupStates()
    {
        _states = new List<State>
        {
            new IdleState(),
            new FollowPlayerState(),
            new FollowSpiritState()
        };
        foreach (State state in _states)
        {
            state.Init(SwitchToState);
        }
    }

    private void SwitchToState(Type state)
    {
        _currentState?.OnExit();
        _currentState = _states.First(s => s.GetType() == state);
        _currentState.OnEnter();
    }

    private void Update()
    {
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
}