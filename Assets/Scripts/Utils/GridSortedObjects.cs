using System;
using System.Collections.Generic;
using System.Linq;
using ForestSpirits;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GridSortedObjects : MonoBehaviour
{
    [SerializeField] private Vector2 _gridMin;
    [SerializeField] private Vector2 _gridMax;
    [SerializeField] private int _segmentsX, _segmentsZ;
    private readonly List<GridSegment<Transform>> _grid = new();

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
                _grid.Add(new GridSegment<Transform>(currentMin, currentMax));
            }
        }
    }

    public void SortIntoGrid(IEnumerable<Transform> spawns)
    {
        foreach (Transform spawn in spawns)
        {
            GridSegment<Transform> segment = _grid.First(segment => segment.ContainsPoint(spawn.position));
            segment.Objects.Add(spawn);
        }
    }

    public IEnumerable<Transform> DrawAmountWithoutReturning(int amount)
    {
        List<Transform> result = new();
        List<GridSegment<Transform>> availableSegments = _grid.Where(segment => segment.Objects.Count > 0).ToList();
        Debug.Assert(availableSegments.Count > 0);

        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, availableSegments.Count);
            result.Add(availableSegments[index].GetRandomObject());
            availableSegments.RemoveAt(index);
            if (availableSegments.Count == 0)
            {
                availableSegments = _grid.Where(segment => segment.Objects.Count > 0).ToList();
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
        foreach (GridSegment<Transform> segment in _grid)
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
        public readonly List<T> Objects = new();

        public GridSegment(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public bool ContainsPoint(Vector3 point)
        {
            return point.x <= Max.x && point.x >= Min.x && point.z <= Max.y && point.z >= Min.y;
        }

        public T GetRandomObject()
        {
            Debug.Assert(Objects.Count > 0, "Objects.Count > 0");
            return Objects[Random.Range(0, Objects.Count)];
        }
    }
}