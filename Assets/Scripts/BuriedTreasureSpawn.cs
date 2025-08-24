using UnityEngine;

public class BuriedTreasureSpawn : MonoBehaviour
{
    private void Awake()
    {
        Game.Instance.Register(this);
    }
}
