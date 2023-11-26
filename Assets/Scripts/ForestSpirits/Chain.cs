using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForestSpirits
{
    public class Chain : MonoBehaviour
    {
        [SerializeField] private ChainLink _chainLinkPrefab;
        
        private readonly List<ChainLink> _chainLinks = new();
        private readonly Dictionary<Spirit, ChainLink> _spiritToLinks = new();
        private readonly Stack<ChainLink> _inactiveChainLinks = new();
        private Spirit FirstSpirit => _chainLinks.Count > 0 ? _chainLinks[0].Spirit : null;

        public void Enqueue(Spirit spirit)
        {
            Assert.IsFalse(_spiritToLinks.ContainsKey(spirit));
            ChainLink link = GetOrCreateChainLink();
            link.Init(spirit);
            _chainLinks.Add(link);
            _spiritToLinks.Add(spirit, link);
        }

        private ChainLink GetOrCreateChainLink()
        {
            if (_inactiveChainLinks.Count > 0)
            {
                return _inactiveChainLinks.Pop();
            }

            ChainLink chainLink = Instantiate(_chainLinkPrefab);
            chainLink.OnReturn();
            return chainLink;
        }

        public IChainTarget GetTargetFor(Spirit requester)
        {
            Debug.Assert(_spiritToLinks.ContainsKey(requester));
            return _spiritToLinks[requester];
        }

        public void OnUpdate()
        {
            if (_chainLinks.Count == 0)
            {
                return;
            }
            if ((Player.WorldPosition - FirstSpirit.WorldPosition).magnitude > 8f)
            {
                Break();
                return;
            }
            for (int index = 0; index < _chainLinks.Count; index++)
            {
                const float speed = 20f;
                const float chainLinkDistance = 1.5f;
                ChainLink chainLink = _chainLinks[index];
                IChainTarget followTarget = index == 0 ? Player : _chainLinks[index - 1];
                
                Vector3 currentPos = chainLink.WorldPosition;
                Vector3 straightPos = Player.WorldPosition - Player.transform.forward * ((index + 1) * chainLinkDistance);

                Vector3 stepTowardsStraight = (straightPos - currentPos).normalized * (speed * Time.deltaTime);
                bool stepWouldOvershootTarget = stepTowardsStraight.magnitude > Vector3.Distance(currentPos, straightPos);
                Vector3 straightTargetPos = stepWouldOvershootTarget
                    ? straightPos
                    : currentPos + stepTowardsStraight;

                Vector3 stepTowardsFollow = (followTarget.WorldPosition - currentPos).normalized * (speed * Time.deltaTime);
                Vector3 followTargetPos = currentPos + stepTowardsFollow;
                if (Vector3.Distance(followTarget.WorldPosition, followTargetPos) < chainLinkDistance)
                {
                    followTargetPos = followTarget.WorldPosition - stepTowardsFollow.normalized * chainLinkDistance;
                }

                float weight = 1f / (index + 3);
                chainLink.WorldPosition = Vector3.Lerp(followTargetPos, straightTargetPos, weight);
            }
        }

        private void Break()
        {
            foreach (Spirit forestSpirit in _spiritToLinks.Keys)
            {
                forestSpirit.SwitchToState(typeof(IdleState));
            }
            foreach (ChainLink chainLink in _chainLinks)
            {
                chainLink.OnReturn();
                _inactiveChainLinks.Push(chainLink);
            }
            _chainLinks.Clear();
            _spiritToLinks.Clear();
        }

        private static PlayerCharacter Player => App.Instance.Player;
    }

    public interface IChainTarget
    {
        public Vector3 WorldPosition { get; }
    }
}