using ForestSpirits;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Spirit _forestSpiritPrefab;
    [SerializeField] private Transform _forestSpiritParent;
    [SerializeField] private BuriedTreasure _buriedTreasurePrefab;
    [SerializeField] private Coin _coinPrefab;
    [SerializeField] private DiggingHole _diggingHolePrefab;
    [SerializeField] private Vinyl _vinylPrefab;
    [SerializeField] private Transform _diggingHoleParent, _activeCoinsParent, _inactiveCoinsParent;

    private PrefabPool<Coin> _coinPool;

    public void Init()
    {
        _coinPool = new PrefabPool<Coin>(_coinPrefab, 
            _activeCoinsParent, 
            _inactiveCoinsParent,
            onBeforeGet: coin => coin.OnPooled());
    }
    
    public void SpawnForestSpirit(Vector3 position, Quaternion rotation)
    {
        Spirit spirit = Instantiate(_forestSpiritPrefab, position, rotation, _forestSpiritParent);
        spirit.Init();
    }

    public BuriedTreasure SpawnBuriedTreasure(Vector3 position, Quaternion rotation, Transform parent)
    {
        return Instantiate(_buriedTreasurePrefab, position, rotation, parent);
    }
    
    public Coin SpawnCoin(Vector3 position, Quaternion rotation)
    {
        Coin coin = _coinPool.Get();
        coin.transform.position = position;
        coin.transform.rotation = rotation;
        return coin;
    }

    public void Return(Coin coin)
    {
        _coinPool.Return(coin);
    }

    public DiggingHole SpawnDiggingHole(Vector3 position)
    {
        return Instantiate(_diggingHolePrefab, position, Quaternion.identity, _diggingHoleParent);
    }

    public Vinyl SpawnVinyl(Vector3 position, Quaternion rotation, VinylId id)
    {
        Vinyl vinyl = Instantiate(_vinylPrefab, position, rotation, _activeCoinsParent);
        vinyl.Id = id;
        return vinyl;
    }
}