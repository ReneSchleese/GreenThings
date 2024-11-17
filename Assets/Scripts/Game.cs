using System.Collections;
using System.Collections.Generic;
using Audio;
using ForestSpirits;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private EntityManager _entityManager;
    [SerializeField] private GridSpawner _forestSpiritSpawner;
    [SerializeField] private AudioClip _ambientClip;
    [SerializeField] private int _forestSpiritAmount;
    
    
    
    private readonly List<ForestSpiritSpawn> _forestSpiritSpawns = new();
    private static Game _instance;

    public void Awake()
    {
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        // wait for spawners to register
        yield return null;
        
        SpawnForestSpirits();
        AudioManager.Instance.PlayAmbient(_ambientClip, loop: true);
    }

    private void SpawnForestSpirits()
    {
        _forestSpiritSpawner.CalculateGrid();
        
        Debug.Assert(_forestSpiritSpawns.Count > 0, "no forest-spirit spawns have been registered");
        _forestSpiritSpawner.SortIntoGrid(_forestSpiritSpawns);
        
        foreach (ForestSpiritSpawn spawn in _forestSpiritSpawner.TakeAtRandom(_forestSpiritAmount))
        {
            var spawnTransform = spawn.transform;
            _entityManager.SpawnForestSpirit(spawnTransform.position, spawnTransform.rotation);
        }
    }

    public void Register(ForestSpiritSpawn spawn)
    {
        _forestSpiritSpawns.Add(spawn);
    }

    public static Game Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Game>();
            }
            return _instance;
        }
    }
}