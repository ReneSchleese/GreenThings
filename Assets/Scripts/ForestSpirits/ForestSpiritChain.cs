using System.Collections.Generic;
using UnityEngine;

namespace ForestSpirits
{
    public class ForestSpiritChain
    {
        private readonly List<ForestSpirit> _chain = new();
        public ForestSpirit LeadingSpirit => _chain.Count > 0 ? _chain[0] : null;

        public IFollowable Enqueue(ForestSpirit spirit)
        {
            _chain.Add(spirit);
            Debug.Log($"Enqueue, count={_chain.Count}");
            return spirit == LeadingSpirit ? App.Instance.Player : _chain[^2];
        }

        public void Dequeue(ForestSpirit spirit)
        {
            _chain.Remove(spirit);
            Debug.Log($"Dequeue, count={_chain.Count}");
        }
    }

    public interface IFollowable
    {
        public Vector3 WorldPosition { get; }
        public bool IsFollowing { get; }
    }
}