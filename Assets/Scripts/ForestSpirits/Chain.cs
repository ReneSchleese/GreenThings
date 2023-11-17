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
            for (int i = 0; i < _chainLinks.Count; i++)
            {
                ChainLink chainLink = _chainLinks[i];
                if (i == 0)
                {
                    Vector3 behindPlayerPos = Player.WorldPosition - Player.transform.forward * 0.5f;
                    chainLink.transform.position = behindPlayerPos;
                }
                else if(i == 1)
                {
                    Vector3 behindPlayerPos = Player.WorldPosition - Player.transform.forward * 2f;
                    chainLink.transform.position = behindPlayerPos;
                }
                else
                {
                    ChainLink beforeLink = _chainLinks[i - 1];
                    chainLink.OnUpdate(beforeLink);
                }
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