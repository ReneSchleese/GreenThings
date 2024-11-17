using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using ForestSpirits;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private EntityManager _entityManager;
    [SerializeField] private AudioClip _ambientClip;
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
        CalculateGrid();
        AudioManager.Instance.PlayAmbient(_ambientClip, loop: true);
    }

    private void SpawnForestSpirits()
    {
        Debug.Assert(_forestSpiritSpawns.Count > 0, "no forest-spirit spawns have been registered");
        List<ForestSpiritSpawn> chosenSpawns = new List<ForestSpiritSpawn>(_forestSpiritSpawns);
        foreach (ForestSpiritSpawn spawn in chosenSpawns)
        {
            var spawnTransform = spawn.transform;
            _entityManager.SpawnForestSpirit(spawnTransform.position, spawnTransform.rotation);
        }
        foreach (ForestSpiritSpawn spawn in _forestSpiritSpawns)
        {
            Destroy(spawn.gameObject);
        }
    }

    private struct RectXZ
    {
        public readonly Vector2 Min;
        public readonly Vector2 Max;

        public RectXZ(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(Vector3 point)
        {
            return point.x <= Max.x && point.x >= Min.x && point.z <= Max.y && point.z >= Min.y;
        }
    }

    private List<RectXZ> _grid = new();
    [SerializeField] private Vector2 _gridMin;
    [SerializeField] private Vector2 _gridMax;
    [SerializeField] private int _segmentsX, _segmentsZ;

    private void CalculateGrid()
    {
        float segmentSizeX = (_gridMax.x - _gridMin.x) / _segmentsX;
        float segmentSizeZ = (_gridMax.y - _gridMin.y) / _segmentsZ;
        for (int x = 0; x < _segmentsX; x++)
        {
            for (int z = 0; z < _segmentsZ; z++)
            {
                Vector2 currentMin = _gridMin + new Vector2(x * segmentSizeX, z * segmentSizeZ);;
                Vector2 currentMax = _gridMin + new Vector2((x + 1) * segmentSizeX, (z + 1) * segmentSizeZ);
                _grid.Add(new RectXZ(currentMin, currentMax));
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        List<Vector3> lines = new();
        foreach (RectXZ rect in _grid)
        {
            lines.Add(new Vector3(rect.Min.x, 0f, rect.Min.y));
            lines.Add(new Vector3(rect.Min.x, 0f, rect.Max.y));
            lines.Add(new Vector3(rect.Max.x, 0f, rect.Min.y));
            lines.Add(new Vector3(rect.Max.x, 0f, rect.Max.y));
            
            lines.Add(new Vector3(rect.Min.x, 0f, rect.Min.y));
            lines.Add(new Vector3(rect.Max.x, 0f, rect.Min.y));
            lines.Add(new Vector3(rect.Min.x, 0f, rect.Max.y));
            lines.Add(new Vector3(rect.Max.x, 0f, rect.Max.y));
        }

        Gizmos.DrawLineList(new ReadOnlySpan<Vector3>(lines.ToArray()));
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