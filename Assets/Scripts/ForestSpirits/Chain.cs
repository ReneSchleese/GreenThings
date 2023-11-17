using System.Collections.Generic;
using UnityEngine;

namespace ForestSpirits
{
    public class Chain
    {
        private readonly List<ChainLink> _chainLinks = new();
        private ChainLink FirstSpirit => _chainLinks.Count > 0 ? _chainLinks[0] : null;

        public void Enqueue(ChainLink spiritChainLink)
        {
            _chainLinks.Add(spiritChainLink);
        }

        public IChainTarget GetTargetFor(ChainLink requester)
        {
            Debug.Assert(_chainLinks.Contains(requester));
            return requester == FirstSpirit ? Player : _chainLinks[_chainLinks.IndexOf(requester) - 1];
        }

        public void TryClear()
        {
            if (FirstSpirit == null)
            {
                return;
            }
            if ((Player.WorldPosition - FirstSpirit.WorldPosition).magnitude > 8f)
            {
                Clear();
            }
        }

        private void Clear()
        {
            foreach (ChainLink forestSpirit in _chainLinks)
            {
                forestSpirit.SwitchToState(typeof(IdleState));
            }
            _chainLinks.Clear();
        }

        private static PlayerCharacter Player => App.Instance.Player;
    }

    public interface IChainTarget
    {
        public Vector3 WorldPosition { get; }
    }
}