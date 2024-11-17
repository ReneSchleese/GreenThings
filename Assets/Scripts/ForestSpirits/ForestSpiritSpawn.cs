using UnityEngine;

namespace ForestSpirits
{
    public class ForestSpiritSpawn : MonoBehaviour
    {
        private void Awake()
        {
            Game.Instance.Register(this);
        }
    }
}