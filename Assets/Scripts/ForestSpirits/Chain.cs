using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForestSpirits
{
    public class Chain
    {
        private readonly List<ChainLink> _chainLinks = new();
        private readonly Dictionary<Spirit, ChainLink> _spiritToLinks = new();
        private Spirit FirstSpirit => _chainLinks.Count > 0 ? _chainLinks[0].Spirit : null;

        public void Enqueue(Spirit spirit)
        {
            Debug.Assert(_spiritToLinks.ContainsKey(spirit));
            ChainLink link = GetOrCreateChainLink();
            link.Spirit = spirit;
            _chainLinks.Add(link);
            _spiritToLinks.Add(spirit, link);
        }

        private ChainLink GetOrCreateChainLink()
        {
            throw new NotImplementedException();
        }

        public IChainTarget GetTargetFor(Spirit requester)
        {
            Debug.Assert(_spiritToLinks.ContainsKey(requester));
            return requester == FirstSpirit ? Player : _chainLinks[_chainLinks.IndexOf(_spiritToLinks[requester]) - 1];
        }

        public void BreakOffIfTooFar()
        {
            if (FirstSpirit == null)
            {
                return;
            }
            if ((Player.WorldPosition - FirstSpirit.WorldPosition).magnitude > 8f)
            {
                Break();
            }
        }

        private void Break()
        {
            foreach (Spirit forestSpirit in _spiritToLinks.Keys)
            {
                forestSpirit.SwitchToState(typeof(IdleState));
            }
            ReturnLinks();
            _chainLinks.Clear();
            _spiritToLinks.Clear();
        }

        private void ReturnLinks()
        {
            throw new NotImplementedException();
        }

        private static PlayerCharacter Player => App.Instance.Player;
    }

    public interface IChainTarget
    {
        public Vector3 WorldPosition { get; }
    }
}