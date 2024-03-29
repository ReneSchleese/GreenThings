﻿using System;
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
            if (_currentState is IdleState && state == typeof(FollowPlayerState))
            {
                Debug.Log("play yay");
            }
            
            _currentState?.OnExit();
            _currentState = _states.First(s => s.GetType() == state);
            _currentState.OnEnter();
        }

        private void Update()
        {
            Controller.Move(Controller.isGrounded ? Vector3.zero : Physics.gravity * Time.deltaTime);
            _currentState.OnUpdate();
            _actor.SmoothSetPosition(Position);
            HandleUnfold();
            if (_currentState.GetType() != typeof(IdleState))
            {
                _actor.SmoothLookAt(App.Instance.Player.Position);
            }

            _positionLastFrame = Position;
        }
        
        private bool _isInUnfoldState;
        private float _timeStampWhereFast;

        private void HandleUnfold()
        {
            if (!IsSlowEnoughToUnfold() || !FollowsPlayer())
            {
                _timeStampWhereFast = Time.time;
                _isInUnfoldState = false;
            }

            if (!_isInUnfoldState && HasBeenSlowLongEnoughToUnfold())
            {
                _isInUnfoldState = true;
                _actor.Unfold();
                Debug.Log("play unfold");
            }

            bool IsSlowEnoughToUnfold() => _actor.Speed <= 0.02f;
            bool HasBeenSlowLongEnoughToUnfold() => Time.time - _timeStampWhereFast > 3.0f;
            bool FollowsPlayer() => _currentState is FollowPlayerState;
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