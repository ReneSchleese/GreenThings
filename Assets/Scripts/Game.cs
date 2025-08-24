using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using ForestSpirits;
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
        // wait for spawners to register
        yield return null;
        SpawnForestSpirits();
        AudioManager.Instance.PlayAmbient(_ambientClip, loop: true);
        
        if(!SceneManager.GetSceneByName("Game_Treasure").isLoaded)
        {
            AsyncOperation gameTreasureOperation = SceneManager.LoadSceneAsync("Game_Treasure", LoadSceneMode.Additive);
            yield return new WaitUntil(() => gameTreasureOperation.isDone);
        }
        GameTreasureManager treasureManager = FindFirstObjectByType<GameTreasureManager>();
        yield return treasureManager.Setup();
    }

    private void SpawnForestSpirits()
    {
        _forestSpiritSpawner.CalculateGrid();
        
        Debug.Assert(_forestSpiritSpawns.Count > 0, "no forest-spirit spawns have been registered");
        _forestSpiritSpawner.SortIntoGrid(_forestSpiritSpawns.Select(spawn => spawn.transform));
        
        foreach (Transform spawn in _forestSpiritSpawner.DrawAmountWithoutReturning(_forestSpiritAmount))
        {
            _spawner.SpawnForestSpirit(spawn.position, spawn.rotation);
        }
        foreach (Transform spawn in _forestSpiritSpawns)
        {
            Destroy(spawn.gameObject);
        }
        _forestSpiritSpawns.Clear();
    }

    public PlayerCharacter Player => _player;
    public Chain Chain => _chain;
}