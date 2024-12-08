using System;
using System.Collections.Generic;
using System.Linq;
using ForestSpirits;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] private Vector2 _gridMin;
    [SerializeField] private Vector2 _gridMax;
    [SerializeField] private int _segmentsX, _segmentsZ;
    private readonly List<GridSegment<ForestSpiritSpawn>> _grid = new();

    public void CalculateGrid()
    {
        float segmentSizeX = (_gridMax.x - _gridMin.x) / _segmentsX;
        float segmentSizeZ = (_gridMax.y - _gridMin.y) / _segmentsZ;
        for (int x = 0; x < _segmentsX; x++)
        {
            for (int z = 0; z < _segmentsZ; z++)
            {
                Vector2 currentMin = _gridMin + new Vector2(x * segmentSizeX, z * segmentSizeZ);;
                Vector2 currentMax = _gridMin + new Vector2((x + 1) * segmentSizeX, (z + 1) * segmentSizeZ);
                _grid.Add(new GridSegment<ForestSpiritSpawn>(currentMin, currentMax));
            }
        }
    }

    public void SortIntoGrid(IEnumerable<ForestSpiritSpawn> spawns)
    {
        foreach (ForestSpiritSpawn spawn in spawns)
        {
            GridSegment<ForestSpiritSpawn> segment = _grid.First(segment => segment.ContainsPoint(spawn.transform.position));
            segment.Spawns.Add(spawn);
        }
    }

    public IEnumerable<ForestSpiritSpawn> TakeAtRandom(int amount)
    {
        List<ForestSpiritSpawn> result = new();
        List<GridSegment<ForestSpiritSpawn>> availableSegments = _grid.Where(segment => segment.Spawns.Count > 0).ToList();
        Debug.Assert(availableSegments.Count > 0);

        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, availableSegments.Count);
            result.Add(availableSegments[index].GetRandomSpawn());
            availableSegments.RemoveAt(index);
            if (availableSegments.Count == 0)
            {
                availableSegments = _grid.Where(segment => segment.Spawns.Count > 0).ToList();
            }
        }

        return result;
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    void OnDrawGizmosSelected()
    {
        List<Vector3> lines = new();
        foreach (GridSegment<ForestSpiritSpawn> segment in _grid)
        {
            lines.Add(new Vector3(segment.Min.x, 0f, segment.Min.y));
            lines.Add(new Vector3(segment.Min.x, 0f, segment.Max.y));
            lines.Add(new Vector3(segment.Max.x, 0f, segment.Min.y));
            lines.Add(new Vector3(segment.Max.x, 0f, segment.Max.y));
            
            lines.Add(new Vector3(segment.Min.x, 0f, segment.Min.y));
            lines.Add(new Vector3(segment.Max.x, 0f, segment.Min.y));
            lines.Add(new Vector3(segment.Min.x, 0f, segment.Max.y));
            lines.Add(new Vector3(segment.Max.x, 0f, segment.Max.y));
        }

        Gizmos.DrawLineList(new ReadOnlySpan<Vector3>(lines.ToArray()));
    }

    private class GridSegment<T>
    {
        public readonly Vector2 Min;
        public readonly Vector2 Max;
        public readonly List<T> Spawns = new();

        public GridSegment(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public bool ContainsPoint(Vector3 point)
        {
            return point.x <= Max.x && point.x >= Min.x && point.z <= Max.y && point.z >= Min.y;
        }

        public T GetRandomSpawn()
        {
            Debug.Assert(Spawns.Count > 0, "Spawns.Count > 0");
            return Spawns[Random.Range(0, Spawns.Count)];
        }
    }
}