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
        [SerializeField] [Range(0, 1f)] private float _stiffness = 0.15f;
        [SerializeField] [Range(0, 1f)] private float _fallOff = 0.15f;
     
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
            link.MimicRoutePosition = link.Position;
        }

        public IChainTarget GetTargetFor(Spirit requester)
        {
            return !_spiritToLinks.ContainsKey(requester) ? null : _spiritToLinks[requester];
        }

        public int GetIndex(Spirit spirit)
        {
            return _chainLinks.IndexOf(_spiritToLinks[spirit]);
        }

        private readonly Vector3[] _playerPositionsBuffer = new Vector3[20];
        private int _bufferIndex = 0;
        private float _timeLastSnapshot;

        public void OnUpdate()
        {
            if (_playerPositionsBuffer[_bufferIndex] == default)
            {
                _playerPositionsBuffer[_bufferIndex] = Player.Position;
            }
            if (Vector3.Distance(Player.Position, _playerPositionsBuffer[_bufferIndex]) > CHAIN_LINK_DISTANCE)
            {
                _bufferIndex = (_bufferIndex + 1) % _playerPositionsBuffer.Length;
                _playerPositionsBuffer[_bufferIndex] = Player.Position;
            }
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
                    Vector3 spiritToTarget = followTarget.Position - chainLink.Spirit.Position;
                    bool isTooFarAway = Utils.CloneAndSetY(spiritToTarget, 0f).magnitude > BREAK_DISTANCE;
                    if(isTooFarAway)
                    {
                        BreakAt(index);
                        return;
                    }
                }
                
                int GetRouteIndex()
                {
                    return Utils.Mod(_bufferIndex - (index + 1), _playerPositionsBuffer.Length);
                }
                
                Vector3 target = _playerPositionsBuffer[GetRouteIndex()];
                Vector3 currentPos = chainLink.MimicRoutePosition;
                float distance = Vector3.Distance(target, currentPos);
                float speed = Mathf.Clamp(Player.Velocity.magnitude, PlayerCharacter.MOVEMENT_SPEED * 0.2f, PlayerCharacter.MOVEMENT_SPEED * 0.95f);
                chainLink.MimicRoutePosition += (target - currentPos).normalized * Mathf.Min(speed * Time.deltaTime, distance);
                chainLink.Position = chainLink.MimicRoutePosition;

                /*
                
                float requiredDistance = index == 0 ? FIRST_CHAIN_LINK_DISTANCE : CHAIN_LINK_DISTANCE;
                Vector3 currentPos = chainLink.Position;
                var forward = Player.transform.forward;
                Vector3 straightPos = Player.Position - forward * FIRST_CHAIN_LINK_DISTANCE - forward * (index * requiredDistance);

                Vector3 stepTowardsFollow = (followTarget.Position - currentPos).normalized;
                Vector3 followTargetPos = currentPos + stepTowardsFollow;
                if (Vector3.Distance(followTarget.Position, followTargetPos) < requiredDistance)
                {
                    followTargetPos = followTarget.Position - stepTowardsFollow.normalized * requiredDistance;
                }
                
                float weightInChain = _stiffness * (1f / (1 * _fallOff * index + Mathf.Epsilon));
                chainLink.DesiredPositionFollow = followTargetPos;
                chainLink.DesiredPositionStraight = straightPos;
                chainLink.DesiredPositionLerped = Vector3.Lerp(chainLink.DesiredPositionFollow, chainLink.DesiredPositionStraight, App.Instance.Player.IsMoving ? weightInChain : 0f);
                
                Vector3 stepTowardsLerped = (chainLink.DesiredPositionLerped - currentPos).normalized * (PlayerCharacter.MOVEMENT_SPEED * 0.95f * Time.deltaTime);
                Vector3 lerpedTargetPos = currentPos + stepTowardsLerped;
                // if (Vector3.Distance(followTarget.Position, lerpedTargetPos) < requiredDistance)
                // {
                //     lerpedTargetPos = followTarget.Position - stepTowardsFollow.normalized * requiredDistance;
                // }
                
                //chainLink.Position = Vector3.Lerp(followTargetPos, straightTargetPos, App.Instance.Player.IsMoving ? weightInChain : 0f);
                chainLink.Position = lerpedTargetPos;*/
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            foreach (Vector3 vector3 in _playerPositionsBuffer)
            {
                Gizmos.DrawCube(vector3, Vector3.one * 0.25f);   
            }
            foreach (ChainLink chainLink in _chainLinks)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(chainLink.DesiredPositionFollow, Vector3.one * 0.25f);
                Gizmos.color = Color.red;
                Gizmos.DrawCube(chainLink.DesiredPositionStraight, Vector3.one * 0.25f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(chainLink.DesiredPositionLerped, Vector3.one * 0.25f);
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(chainLink.MimicRoutePosition + Vector3.right * 0.2f, Vector3.one * 0.25f);
            }
        }

        private static PlayerCharacter Player => App.Instance.Player;
    }

    public interface IChainTarget
    {
        public Vector3 Position { get; }
    }
}