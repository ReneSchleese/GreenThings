﻿using System;
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