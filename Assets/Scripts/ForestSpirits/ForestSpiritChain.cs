using System.Collections.Generic;
using UnityEngine;

namespace ForestSpirits
{
    public class ForestSpiritChain
    {
        private readonly List<ForestSpirit> _chain = new();
        private ForestSpirit LeadingSpirit => _chain.Count > 0 ? _chain[0] : null;

        public void Enqueue(ForestSpirit spirit)
        {
            _chain.Add(spirit);
        }

        public IChainTarget GetTargetFor(ForestSpirit requester)
        {
            Debug.Assert(_chain.Contains(requester));
            return requester == LeadingSpirit ? Player : _chain[_chain.IndexOf(requester) - 1];
        }

        public void TryClear()
        {
            if (LeadingSpirit == null)
            {
                return;
            }
            if ((Player.WorldPosition - LeadingSpirit.WorldPosition).magnitude > 8f)
            {
                Clear();
            }
        }

        private void Clear()
        {
            foreach (ForestSpirit forestSpirit in _chain)
            {
                forestSpirit.SwitchToState(typeof(IdleState));
            }
            _chain.Clear();
        }

        private static PlayerCharacter Player => App.Instance.Player;
    }

    public interface IChainTarget
    {
        public Vector3 WorldPosition { get; }
    }
}