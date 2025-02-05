﻿using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForestSpirits
{
    public class Chain : MonoBehaviour
    {
        [SerializeField] private ChainLink _chainLinkPrefab;
        [SerializeField] private Transform _activeContainer;
        [SerializeField] private Transform _inactiveContainer;
        [SerializeField] private ChainSounds _sounds;
     
        private const float BREAK_DISTANCE = 8f;
        private const float BREAK_DISTANCE_SQR = BREAK_DISTANCE * BREAK_DISTANCE;
        private const float UPDATE_SPEED = 20f;
        private const float CHAIN_LINK_DISTANCE = 1.7f;
        private const float FIRST_CHAIN_LINK_DISTANCE = 2.5f;
        
        private readonly List<ChainLink> _chainLinks = new();
        private readonly Dictionary<Spirit, ChainLink> _spiritToLinks = new();
        private PrefabPool<ChainLink> _chainLinkPool;

        private void Awake()
        {
            _chainLinkPool = new PrefabPool<ChainLink>(_chainLinkPrefab, _activeContainer, _inactiveContainer, onBeforeGet: link =>
            {
                link.transform.position = _chainLinks.Count > 0
                    ? _chainLinks[^1].transform.position
                    : Player.Position;
                link.RealTimeSecondsWhenPooled = Time.realtimeSinceStartup;
            }, onBeforeReturn: link => { link.Spirit = null; });
        }

        public void Enqueue(Spirit spirit)
        {
            Assert.IsFalse(_spiritToLinks.ContainsKey(spirit));
            ChainLink link = _chainLinkPool.Get();
            link.Spirit = spirit;
            _chainLinks.Add(link);
            _spiritToLinks.Add(spirit, link);
        }

        public IChainTarget GetTargetFor(Spirit requester)
        {
            return !_spiritToLinks.ContainsKey(requester) ? null : _spiritToLinks[requester];
        }

        public int GetIndex(Spirit spirit)
        {
            return _chainLinks.IndexOf(_spiritToLinks[spirit]);
        }

        public void OnUpdate()
        {
            if (_chainLinks.Count == 0)
            {
                return;
            }
            
            for (int index = 0; index < _chainLinks.Count; index++)
            {
                ChainLink chainLink = _chainLinks[index];
                IChainTarget followTarget = index == 0 ? Player : _chainLinks[index - 1];
                
                if (chainLink.IsAllowedToBreak)
                {
                    Vector3 spiritToTarget = followTarget.BreakPosition - chainLink.Spirit.Position;
                    bool isTooFarAway = Utils.CloneAndSetY(spiritToTarget, 0f).magnitude > BREAK_DISTANCE;
                    if(isTooFarAway)
                    {
                        BreakAt(index);
                        return;
                    }
                }
                
                float requiredDistance = index == 0 ? FIRST_CHAIN_LINK_DISTANCE : CHAIN_LINK_DISTANCE;
                Vector3 currentPos = chainLink.Position;
                Vector3 straightPos = Player.Position - Player.transform.forward * ((index + 1) * requiredDistance);

                Vector3 stepTowardsStraight = (straightPos - currentPos).normalized * (UPDATE_SPEED * Time.deltaTime);
                bool stepWouldOvershootTarget = stepTowardsStraight.magnitude > Vector3.Distance(currentPos, straightPos);
                Vector3 straightTargetPos = stepWouldOvershootTarget
                    ? straightPos
                    : currentPos + stepTowardsStraight;

                Vector3 stepTowardsFollow = (followTarget.Position - currentPos).normalized * (UPDATE_SPEED * Time.deltaTime);
                Vector3 followTargetPos = currentPos + stepTowardsFollow;
                if (Vector3.Distance(followTarget.Position, followTargetPos) < requiredDistance)
                {
                    followTargetPos = followTarget.Position - stepTowardsFollow.normalized * requiredDistance;
                }

                float weight = 1f / (index + 3);
                chainLink.Position = Vector3.Lerp(followTargetPos, straightTargetPos, weight);
            }
        }

        private void BreakAt(int index)
        {
            Debug.Log($"BreakAt index={index}");
            for (int i = index; i < _chainLinks.Count; i++)
            {
                ChainLink chainLink = _chainLinks[i];
                chainLink.Spirit.SwitchToState(typeof(IdleState));
                _spiritToLinks.Remove(chainLink.Spirit);
                _chainLinkPool.Return(chainLink);
            }

            _chainLinks.RemoveAll(link => _chainLinks.IndexOf(link) >= index);
        }

        public void PlayEchoed(int index, float clipLength)
        {
            int repetitions = 0;
            const int repetitionsMax = 2;
            for (int i = 0; i < _chainLinks.Count - 1; i++)
            {
                if (repetitions == repetitionsMax) break;
                if (Random.Range(0f, 1f) < 0.5f) repetitions++;
            }
            
            if(_chainLinks.Count > 0)
            {
                _sounds.PlayEchoed(index, clipLength, repetitions);
            }

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(clipLength);
            foreach (Spirit t in _chainLinks.Select(chainLink => chainLink.Spirit).ToList())
            {
                sequence.AppendInterval(0.1f);
                sequence.AppendCallback(() => t.BumpUpwards());
            }
        }

        private static PlayerCharacter Player => App.Instance.Player;
    }

    public interface IChainTarget
    {
        public Vector3 Position { get; }
        public Vector3 BreakPosition { get; }
    }
}