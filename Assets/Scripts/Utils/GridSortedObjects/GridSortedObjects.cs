using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class GridSortedObjects
{
    [SerializeField] private Vector2 _gridMin;
    [SerializeField] private Vector2 _gridMax;
    [SerializeField] private int _segmentsX, _segmentsZ;
    private readonly List<GridBucket<Transform>> _grid = new();

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
                _grid.Add(new GridBucket<Transform>(currentMin, currentMax));
            }
        }
    }

    public void SortIntoGrid(IEnumerable<Transform> spawns)
    {
        foreach (Transform spawn in spawns)
        {
            GridBucket<Transform> bucket = _grid.First(bucket => bucket.ContainsPoint(spawn.position));
            bucket.RemainingObjects.Add(spawn);
        }
    }

    public IEnumerable<Transform> DrawAmountWithoutReturning(int amount)
    {
        List<Transform> result = new();
        List<GridBucket<Transform>> remainingSegments = _grid.Where(bucket => bucket.RemainingObjects.Count > 0).ToList();
        List<GridBucket<Transform>> alreadyUsedSegments = new();
        Debug.Assert(remainingSegments.Count > 0);

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, remainingSegments.Count);
            GridBucket<Transform> randomBucket = remainingSegments[randomIndex];
            Transform randomObject = randomBucket.GetRandomObject(markAsUsed: true);
            result.Add(randomObject);

            if (randomBucket.RemainingObjects.Count == 0)
            {
                randomBucket.SetAllRemaining();
            }
            
            remainingSegments.Remove(randomBucket);
            alreadyUsedSegments.Add(randomBucket);
            if (remainingSegments.Count == 0)
            {
                remainingSegments.AddRange(alreadyUsedSegments);
                alreadyUsedSegments.Clear();
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
        foreach (GridBucket<Transform> segment in _grid)
        {
            lines.Add(new Vector3(segment.CoordinateMin.x, 0f, segment.CoordinateMin.y));
            lines.Add(new Vector3(segment.CoordinateMin.x, 0f, segment.CoordinateMax.y));
            lines.Add(new Vector3(segment.CoordinateMax.x, 0f, segment.CoordinateMin.y));
            lines.Add(new Vector3(segment.CoordinateMax.x, 0f, segment.CoordinateMax.y));
            
            lines.Add(new Vector3(segment.CoordinateMin.x, 0f, segment.CoordinateMin.y));
            lines.Add(new Vector3(segment.CoordinateMax.x, 0f, segment.CoordinateMin.y));
            lines.Add(new Vector3(segment.CoordinateMin.x, 0f, segment.CoordinateMax.y));
            lines.Add(new Vector3(segment.CoordinateMax.x, 0f, segment.CoordinateMax.y));
        }

        Gizmos.DrawLineList(new ReadOnlySpan<Vector3>(lines.ToArray()));
    }
    
    public Vector2 GridMin
    {
        get => _gridMin;
        set => _gridMin = value;
    }

    public Vector2 GridMax
    {
        get => _gridMax;
        set => _gridMax = value;
    }

    public int SegmentsX
    {
        get => _segmentsX;
        set => _segmentsX = value;
    }

    public int SegmentsZ
    {
        get => _segmentsZ;
        set => _segmentsZ = value;
    }
}