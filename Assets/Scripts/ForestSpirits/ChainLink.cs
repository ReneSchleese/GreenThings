using System;
using UnityEngine;

namespace ForestSpirits
{
    public class ChainLink : MonoBehaviour, IChainTarget
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        private const bool DRAW_DEBUG = false;

        private void Awake()
        {
            _meshRenderer.enabled = DRAW_DEBUG;
        }

        public Spirit Spirit { get; set; }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
    }
}