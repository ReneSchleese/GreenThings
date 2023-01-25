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

        public IFollowable GetTarget(ForestSpirit spirit)
        {
            Debug.Assert(_chain.Contains(spirit));
            return spirit == LeadingSpirit ? Player : _chain[_chain.IndexOf(spirit) - 1];
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

    public interface IFollowable
    {
        public Vector3 WorldPosition { get; }
    }
}