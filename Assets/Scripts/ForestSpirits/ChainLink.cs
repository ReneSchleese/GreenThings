using UnityEngine;

namespace ForestSpirits
{
    public class ChainLink : MonoBehaviour, IChainTarget
    {
        public Spirit Spirit;
        public Vector3 WorldPosition => transform.position;
    }
}