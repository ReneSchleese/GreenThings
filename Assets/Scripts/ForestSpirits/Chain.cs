using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForestSpirits
{
    public class Chain : MonoBehaviour
    {
        [SerializeField] private ChainLink _chainLinkPrefab;
        [SerializeField] private Transform _activeContainer;
        [SerializeField] private Transform _inactiveContainer;
     
        private const float BREAK_DISTANCE = 8f;
        private const float UPDATE_SPEED = 20f;
        private const float CHAIN_LINK_DISTANCE = 1.5f;
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
            Debug.Assert(_spiritToLinks.ContainsKey(requester));
            return _spiritToLinks[requester];
        }

        public void OnUpdate()
        {
            if (_chainLinks.Count == 0)
            {
                return;
            }
            if ((Player.Position - _chainLinks[0].Spirit.Position).magnitude > BREAK_DISTANCE)
            {
                Break();
                return;
            }
            for (int index = 0; index < _chainLinks.Count; index++)
            {
                ChainLink chainLink = _chainLinks[index];
                IChainTarget followTarget = index == 0 ? Player : _chainLinks[index - 1];
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

        private void Break()
        {
            foreach (Spirit forestSpirit in _spiritToLinks.Keys)
            {
                forestSpirit.SwitchToState(typeof(IdleState));
            }
            foreach (ChainLink chainLink in _chainLinks)
            {
                _chainLinkPool.Return(chainLink);
            }
            _chainLinks.Clear();
            _spiritToLinks.Clear();
        }

        private static PlayerCharacter Player => App.Instance.Player;
    }

    public interface IChainTarget
    {
        public Vector3 Position { get; }
    }
}