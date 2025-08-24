using System;
using System.Collections;
using System.Collections.Generic;
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
    
    private readonly List<ForestSpiritSpawn> _forestSpiritSpawns = new();

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
        
        if(!SceneManager.GetSceneByName("Game_Treasure").isLoaded)
        {
            AsyncOperation gameTreasureOperation = SceneManager.LoadSceneAsync("Game_Treasure", LoadSceneMode.Additive);
            yield return new WaitUntil(() => gameTreasureOperation.isDone);
        }
        
        SpawnForestSpirits();
        AudioManager.Instance.PlayAmbient(_ambientClip, loop: true);
    }

    private void SpawnForestSpirits()
    {
        _forestSpiritSpawner.CalculateGrid();
        
        Debug.Assert(_forestSpiritSpawns.Count > 0, "no forest-spirit spawns have been registered");
        _forestSpiritSpawner.SortIntoGrid(_forestSpiritSpawns);
        
        foreach (ForestSpiritSpawn spawn in _forestSpiritSpawner.DrawAmountWithoutReturning<ForestSpiritSpawn>(_forestSpiritAmount))
        {
            var spawnTransform = spawn.transform;
            _spawner.SpawnForestSpirit(spawnTransform.position, spawnTransform.rotation);
        }
        foreach (ForestSpiritSpawn spawn in _forestSpiritSpawns)
        {
            Destroy(spawn.gameObject);
        }
        _forestSpiritSpawns.Clear();
    }

    public void Register(ForestSpiritSpawn spawn)
    {
        _forestSpiritSpawns.Add(spawn);
    }
    
    public void Register(BuriedTreasureSpawn spawn)
    {
        Debug.Log($"registered! {spawn}");
    }

    public PlayerCharacter Player => _player;
    public Chain Chain => _chain;
}