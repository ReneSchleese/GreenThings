using UnityEngine;

namespace ForestSpirits
{
    public class ChainLink : MonoBehaviour, IChainTarget
    {
        public void Init(Spirit spirit)
        {
            Spirit = spirit;
            gameObject.SetActive(true);
        }

        public void OnReturn()
        {
            gameObject.SetActive(false);
            Spirit = null;
        }


        public Spirit Spirit { get; private set; }
        public Vector3 WorldPosition => transform.position;
    }
}