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

        public void SetActive(Spirit spirit)
        {
            Spirit = spirit;
            gameObject.SetActive(true);
        }

        public void SetInactive()
        {
            gameObject.SetActive(false);
            Spirit = null;
        }
        
        public Spirit Spirit { get; private set; }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
    }
}