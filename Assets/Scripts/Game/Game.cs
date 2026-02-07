using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using ForestSpirits;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>, IAppState
{
    [SerializeField] private GameUI _gameUI;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Spawner _spawner;
    [SerializeField] private GridSortedPoints _forestSpiritSpawner;
    [SerializeField] private AudioClip _ambientClip;
    [SerializeField] private int _forestSpiritAmount;
    [SerializeField] private Chain _chain;
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private List<Transform> _forestSpiritSpawns;
    [SerializeField] private TreasureHint _treasureHint;
    [Space]
    [SerializeField] private bool _useDebugSpawn;
    [SerializeField] private Transform _debugSpawnPoint;
    
    [CanBeNull] private GameTreasureManager _gameTreasureManager;

    public void Awake()
    {
        App.Instance.NotifyAwakeAppState(this);
    }

    private void Update()
    {
        _chain.OnUpdate();
    }
    
    public IEnumerator TransitionIn()
    {
        Debug.Log("Game.TransitionTo");
        yield break;
    }
    
    public IEnumerator TransitionOut()
    {
        Debug.Log("Game.TransitionOff");
        yield break;
    }
    
    public IEnumerator OnLoad()
    {
        Debug.Log("Game.OnLoadComplete");
        _gameUI.Init();
        _spawner.Init();
        AudioManager.Instance.PlayAmbient(_ambientClip, loop: true);
        App.Instance.InputManager.Interacted += OnPlayerInteracted;
        
        yield return null;
        SpawnForestSpirits();
        
        if(!SceneManager.GetSceneByName("Game_Treasure").isLoaded)
        {
            AsyncOperation gameTreasureOperation = SceneManager.LoadSceneAsync("Game_Treasure", LoadSceneMode.Additive);
            Debug.Assert(gameTreasureOperation != null);
            yield return new WaitUntil(() => gameTreasureOperation.isDone);
        }
        _gameTreasureManager = FindFirstObjectByType<GameTreasureManager>();
        Debug.Assert(_gameTreasureManager is not null);
        yield return _gameTreasureManager.Setup(numberOfTreasures: 8);
        _treasureHint.SetTarget(_gameTreasureManager.GetRandomUnopenedTreasure());
        foreach (BuriedTreasure buriedTreasure in _gameTreasureManager.BuriedTreasures)
        {
            buriedTreasure.Opened += OnTreasureOpened;
        }
    }

    public void OnUnload()
    {
        Debug.Log("Game.OnUnload");
        Debug.Assert(_gameTreasureManager is not null);
        foreach (BuriedTreasure buriedTreasure in _gameTreasureManager.BuriedTreasures)
        {
            buriedTreasure.Opened -= OnTreasureOpened;
        }
        SceneManager.UnloadSceneAsync("Game_Treasure");
        _gameTreasureManager = null;
        App.Instance.InputManager.Interacted -= OnPlayerInteracted;
        _gameUI.Unload();
    }

    private void OnTreasureOpened(BuriedTreasure treasure)
    {
        Debug.Assert(_gameTreasureManager is not null);
        _gameTreasureManager.OnTreasureOpened(treasure);
        _treasureHint.SetTarget(_gameTreasureManager.GetRandomUnopenedTreasure());
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

    private void OnPlayerInteracted()
    {
        if (_player.InteractionState.InteractionVolume == null)
        {
            return;
        }
        switch (_player.InteractionState.InteractionVolume.InteractionId)
        {
            case InteractionId.Exit:
                App.Instance.TransitionToMainMenu();
                break;
            case InteractionId.TreasureHint:
                if (true)
                {
                    _treasureHint.Trigger();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public PlayerCharacter Player => _player;
    public Chain Chain => _chain;
    public Spawner Spawner => _spawner;
    public Camera MainCamera => _mainCamera;
    public AppState Id => AppState.Game;
}