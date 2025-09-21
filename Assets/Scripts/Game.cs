using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using ForestSpirits;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>
{
    [SerializeField] private Spawner _spawner;
    [SerializeField] private GridSortedObjects _forestSpiritSpawner;
    [SerializeField] private AudioClip _ambientClip;
    [SerializeField] private int _forestSpiritAmount;
    [SerializeField] private Chain _chain;
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private List<Transform> _forestSpiritSpawns;
    [Space]
    [SerializeField] private bool _useDebugSpawn;
    [SerializeField] private Transform _debugSpawnPoint;
    
    [CanBeNull] private GameTreasureManager _gameTreasureManager;

    public void Awake()
    {
        StartCoroutine(Setup());
    }

    private void Update()
    {
        _chain.OnUpdate();
    }

    private IEnumerator Setup()
    {
        yield return null;
        SpawnForestSpirits();
        AudioManager.Instance.PlayAmbient(_ambientClip, loop: true);
        
        if(!SceneManager.GetSceneByName("Game_Treasure").isLoaded)
        {
            AsyncOperation gameTreasureOperation = SceneManager.LoadSceneAsync("Game_Treasure", LoadSceneMode.Additive);
            Debug.Assert(gameTreasureOperation != null);
            yield return new WaitUntil(() => gameTreasureOperation.isDone);
        }
        _gameTreasureManager = FindFirstObjectByType<GameTreasureManager>();
        Debug.Assert(_gameTreasureManager != null);
        yield return _gameTreasureManager.Setup(numberOfTreasures: 3);
    }

    private void SpawnForestSpirits()
    {
        if (_useDebugSpawn)
        {
            for (int i = 0; i < _forestSpiritAmount; i++)
            {
                _spawner.SpawnForestSpirit(_debugSpawnPoint.position, Quaternion.identity);
            }
        }
        else
        {
            _forestSpiritSpawner.CalculateGrid();
            _forestSpiritSpawner.SortIntoGrid(_forestSpiritSpawns.Select(spawn => Point.FromTransform(spawn.transform)));
            foreach (Point point in _forestSpiritSpawner.DrawAmountWithoutReturning(_forestSpiritAmount))
            {
                _spawner.SpawnForestSpirit(point.ToVector3(), point.Rotation);
            }
            foreach (Transform spawn in _forestSpiritSpawns)
            {
                Destroy(spawn.gameObject);
            }
            _forestSpiritSpawns.Clear();
        }
    }

    public bool TryGetTreasureManager(out GameTreasureManager treasureManager)
    {
        treasureManager = _gameTreasureManager;
        return treasureManager != null;
    }

    public PlayerCharacter Player => _player;
    public Chain Chain => _chain;
    public Spawner Spawner => _spawner;
}