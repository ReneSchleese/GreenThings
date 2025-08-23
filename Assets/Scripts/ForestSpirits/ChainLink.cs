using UnityEngine;

namespace ForestSpirits
{
    public class ChainLink : MonoBehaviour, IChainTarget
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        private const bool DRAW_DEBUG = false;
        private const float BREAK_GRACE_SECONDS = 2f;

        private void Awake()
        {
            _meshRenderer.enabled = DRAW_DEBUG;
        }

        public bool IsAllowedToBreak => Time.realtimeSinceStartup - RealTimeSecondsWhenPooled > BREAK_GRACE_SECONDS;

        public Spirit Spirit { get; set; }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        public Vector3 DesiredPositionStraight { get; set; }
        public Vector3 DesiredPositionFollow { get; set; }
        public Vector3 DesiredPositionLerped { get; set; }
        public float RealTimeSecondsWhenPooled { get; set; }
    }
}