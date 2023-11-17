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


        public void OnUpdate(ChainLink targetLink)
        {
            if (Vector3.Distance(WorldPosition, targetLink.WorldPosition) <= ChainLinkState.DEAD_ZONE_DISTANCE)
            {
                return;
            }
            Vector3 direction = targetLink.WorldPosition - WorldPosition;
            transform.position += direction.normalized * (Time.deltaTime * ChainLinkState.SPEED);
        }

        public Spirit Spirit { get; private set; }

        public Vector3 WorldPosition => transform.position;
    }
}