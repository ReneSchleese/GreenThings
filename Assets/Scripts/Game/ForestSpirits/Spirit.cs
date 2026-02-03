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
        [SerializeField] private Puppet _puppet;
        [SerializeField] private Transform _targetLookRotator;
        [SerializeField] private AudioClip[] _followPlayerClips;
        private State _currentState;
        private List<State> _states;
        private static PseudoRandomIndex _followPlayerClipIndex;
        private static PseudoRandomIndex _unfoldingClipIndex;

        public void Init()
        {
            SetupStates();
            SwitchToState(typeof(IdleState));
            _pushHitbox.Init(this);
            _puppet.Init();
            _puppet.transform.SetParent(transform.parent);
            _followPlayerClipIndex ??= new PseudoRandomIndex(_followPlayerClips.Length);
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
                state.Init(spirit: this, _puppet, SwitchToState);
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

            int playerPriority = Game.Instance.Player.Priority;
            if (state == typeof(ChainLinkState))
            {
                Priority = playerPriority + 1 + Game.Instance.Chain.GetIndex(this);
            }
            else
            {
                Priority = playerPriority + 1;
            }
        }

        private void Update()
        {
            Controller.Move(Controller.isGrounded ? Vector3.zero : Physics.gravity * Time.deltaTime);
            _currentState.OnUpdate();
            _puppet.SmoothSetPosition(Position);
            if (_currentState.GetType() != typeof(IdleState))
            {
                _puppet.SmoothLookAt(Game.Instance.Player.Position);
            }
            IChainTarget chainTarget = Game.Instance.Chain.GetTargetFor(this);
            if (chainTarget != null)
            {
                _targetLookRotator.LookAt(chainTarget.Position, Vector3.up);
            }

            if (Velocity.sqrMagnitude > 0f)
            {
                _puppet.BlobShadow.UpdateShadow();
            }
        }

        public void Push(Vector3 direction)
        {
            Controller.Move(direction);
        }

        public void HandleCollision(float radius, IPushable otherPushable)
        {
            if (!otherPushable.IsPushable || otherPushable.Priority < Priority)
            {
                return;
            }

            const float pushStrength = 0.15f;
            Vector3 pushBackDir = Utils.CloneAndSetY(otherPushable.Transform.position - transform.position, 0f);
            if (otherPushable.Priority == Priority || !TargetDir.HasValue)
            {
                Vector3 direction = pushBackDir.normalized * pushStrength;
                otherPushable.Push(direction);
                Push(-direction);
            }
            else
            {
                Vector3 otherPositionInLocalSpace = _targetLookRotator.InverseTransformPoint(otherPushable.Transform.position);
                Vector3 pushToSideDir = Quaternion.AngleAxis(otherPositionInLocalSpace.x < 0f ? -90 : 90, Vector3.up) * TargetDir.Value.normalized;
                Vector3 direction = Utils.CloneAndSetY(pushToSideDir, 0f).normalized * pushStrength;
                otherPushable.Push(direction);
                Push(-direction * 0.5f);
            }
        }

        public void BumpUpwards() => _puppet.BumpUpwards();
        public void OnScan(BuriedTreasure treasure, int index) => _puppet.OnScan(treasure, index);

        public Vector3 Position => transform.position;

        public Vector3? TargetDir
        {
            get
            {
                IChainTarget chainTarget = Game.Instance.Chain.GetTargetFor(this);
                return chainTarget == null ? null : chainTarget.Position - transform.position;
            }
        }

        public Transform Transform => transform;

        public bool IsPushable => true;

        public int Priority { get; private set; }

        public Vector3 Velocity => _puppet.Velocity;

        public int? ChainIndex
        {
            set => _puppet.NormalizedWalkingOffset = !value.HasValue ? 0f : value.Value * 0.2f;
        }
    }
}