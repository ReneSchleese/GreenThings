using System.Collections.Generic;
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
        private const float CHAIN_LINK_DISTANCE = 1.7f;
        
        private readonly List<ChainLink> _chainLinks = new();
        private readonly Dictionary<Spirit, ChainLink> _spiritToLinks = new();
        private PrefabPool<ChainLink> _chainLinkPool;
        private readonly CircularBuffer<Vector3> _playerRoutePointBuffer = new(20);
        private ChainMode _chainMode;

        private void Awake()
        {
            _chainLinkPool = new PrefabPool<ChainLink>(_chainLinkPrefab, _activeContainer, _inactiveContainer, onBeforeGet: link =>
            {
                link.transform.position = _chainLinks.Count > 0
                    ? _chainLinks[^1].transform.position
                    : Player.Position;
                link.RealTimeSecondsWhenPooled = Time.realtimeSinceStartup;
            }, onBeforeReturn: link => { link.Spirit = null; });
            _playerRoutePointBuffer.Add(Player.Position);
            UserInterface.Instance.SpiritModeToggleInput += ToggleMode;
            UserInterface.Instance.ScanInput += Scan;
        }

        public void Enqueue(Spirit spirit)
        {
            Assert.IsFalse(_spiritToLinks.ContainsKey(spirit));
            ChainLink link = _chainLinkPool.Get();
            link.Spirit = spirit;
            _chainLinks.Add(link);
            _spiritToLinks.Add(spirit, link);
            link.FollowPlayerRoutePosition = link.Position;
            spirit.ChainIndex = GetIndex(spirit);
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
            bool createNewRoutePoint = Vector3.Distance(Player.Position, _playerRoutePointBuffer.GetYoungest()) > CHAIN_LINK_DISTANCE;
            if (createNewRoutePoint)
            {
                _playerRoutePointBuffer.Add(Player.Position);
            }
            if (_chainLinks.Count == 0)
            {
                return;
            }
            
            for (int chainLinkIndex = 0; chainLinkIndex < _chainLinks.Count; chainLinkIndex++)
            {
                ChainLink chainLink = _chainLinks[chainLinkIndex];
                IChainTarget followTarget = chainLinkIndex == 0 ? Player : _chainLinks[chainLinkIndex - 1];
                if (chainLink.IsAllowedToBreak)
                {
                    Vector3 spiritToTarget = followTarget.Position - chainLink.Spirit.Position;
                    bool isTooFarAway = Utils.CloneAndSetY(spiritToTarget, 0f).magnitude > BREAK_DISTANCE;
                    if(isTooFarAway)
                    {
                        BreakAt(chainLinkIndex);
                        return;
                    }
                }
                
                int GetRouteIndex()
                {
                    return Utils.Mod(_playerRoutePointBuffer.Index - (chainLinkIndex + 2), _playerRoutePointBuffer.Length);
                }
                
                Vector3 target = _playerRoutePointBuffer.Get(GetRouteIndex());
                Vector3 currentPos = chainLink.FollowPlayerRoutePosition;
                float distance = Vector3.Distance(target, currentPos);
                float minSpeed = _chainMode == ChainMode.Default ? PlayerCharacter.MOVEMENT_SPEED * 0.2f : 0f;
                const float maxSpeed = PlayerCharacter.MOVEMENT_SPEED * 0.95f;
                float speed = Mathf.Clamp(Player.Velocity.magnitude, minSpeed, maxSpeed);
                chainLink.FollowPlayerRoutePosition += (target - currentPos).normalized * Mathf.Min(speed * Time.deltaTime, distance);
                chainLink.Position = chainLink.FollowPlayerRoutePosition;
            }
        }

        private void BreakAt(int index)
        {
            Debug.Log($"BreakAt index={index}");
            for (int i = index; i < _chainLinks.Count; i++)
            {
                ChainLink chainLink = _chainLinks[i];
                chainLink.Spirit.SwitchToState(typeof(IdleState));
                chainLink.Spirit.ChainIndex = null;
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

        private void ToggleMode()
        {
            _chainMode = _chainMode == ChainMode.Default ? ChainMode.ForceChain : ChainMode.Default;
            foreach (Spirit spirit in _spiritToLinks.Keys)
            {
                spirit.SwitchToState(typeof(ChainLinkState));
            }
        }

        private void Scan()
        {
            Debug.Log("Scan");
            bool treasureManagerExists = Game.Instance.TryGetTreasureManager(out GameTreasureManager treasureManager);
            Debug.Assert(treasureManagerExists);
            Sequence sequence = DOTween.Sequence();
            foreach (Spirit spirit in _chainLinks.Select(chainLink => chainLink.Spirit).ToList())
            {
                BuriedTreasure nearestTreasure = treasureManager.GetNearestTreasure(spirit.Position);
                if(nearestTreasure != null)
                {
                    Debug.DrawLine(spirit.Position, nearestTreasure.transform.position, Color.green, 2f);
                }
                sequence.AppendInterval(0.1f);
                sequence.AppendCallback(() => spirit.BumpUpwards());
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            foreach (Vector3 vector3 in _playerRoutePointBuffer.Values)
            {
                Gizmos.DrawCube(vector3, Vector3.one * 0.25f);   
            }
            foreach (ChainLink chainLink in _chainLinks)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(chainLink.FollowPlayerRoutePosition + Vector3.right * 0.2f, Vector3.one * 0.25f);
            }
        }

        private static PlayerCharacter Player => Game.Instance.Player;
        public ChainMode ChainMode => _chainMode;
    }

    public interface IChainTarget
    {
        public Vector3 Position { get; }
    }
    
    public enum ChainMode
    {
        Default,
        ForceChain
    }
}