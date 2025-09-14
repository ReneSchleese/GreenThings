using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private ForestSpirits.Spirit _forestSpiritPrefab;
    [SerializeField] private Transform _forestSpiritParent;
    [SerializeField] private BuriedTreasure _buriedTreasurePrefab;
    [SerializeField] private Coin _coinPrefab;
    [SerializeField] private GameObject _diggingHole;
    [SerializeField] private Transform _diggingHoleParent;
    
    public void SpawnForestSpirit(Vector3 position, Quaternion rotation)
    {
        Instantiate(_forestSpiritPrefab, position, rotation, _forestSpiritParent);
    }

    public BuriedTreasure SpawnBuriedTreasure(Vector3 position, Quaternion rotation, Transform parent)
    {
        return Instantiate(_buriedTreasurePrefab, position, rotation, parent);
    }
    
    public Coin SpawnCoin(Vector3 position, Quaternion rotation)
    {
        return Instantiate(_coinPrefab, position, rotation);
    }

    public GameObject SpawnDiggingHole(Vector3 position)
    {
        return Instantiate(_diggingHole, position, Quaternion.identity, _diggingHoleParent);
    }
}