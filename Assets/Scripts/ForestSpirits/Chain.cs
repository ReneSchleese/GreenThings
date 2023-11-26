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
                const float speed = 0.5f;
                const float chainLinkDistance = 1.5f;
                ChainLink chainLink = _chainLinks[index];
                IChainTarget followTarget = index == 0 ? Player : _chainLinks[index - 1];
                Vector3 straightPos = Player.WorldPosition - Player.transform.forward * ((index + 1) * chainLinkDistance);
                float weight = 1f / (index + 1);
                if (weight < 0.01f) weight = 0.01f;
                Vector3 chainLinkPos = chainLink.WorldPosition;
                Vector3 straightDir = straightPos - chainLinkPos;
                Vector3 straightPosStep = straightPos;
                if (Vector3.Distance(chainLinkPos, straightPos) > 0.2f)
                {
                    straightPosStep = chainLinkPos + straightDir.normalized * speed;
                }
                
                
                Vector3 followStep = chainLinkPos + (followTarget.WorldPosition - chainLinkPos).normalized * speed;
                if (Vector3.Distance(followTarget.WorldPosition, followStep) < chainLinkDistance)
                {
                    followStep = followTarget.WorldPosition + (chainLinkPos-followTarget.WorldPosition).normalized * chainLinkDistance;
                }

                chainLink.WorldPosition = Vector3.Lerp(followStep, straightPosStep, weight);
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