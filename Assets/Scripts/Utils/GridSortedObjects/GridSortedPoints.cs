using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class GridSortedPoints
{
    [SerializeField] private Vector2 _gridMin;
    [SerializeField] private Vector2 _gridMax;
    [SerializeField] private int _segmentsX, _segmentsZ;
    private readonly List<GridBucket<Point>> _grid = new();

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
                _grid.Add(new GridBucket<Point>(currentMin, currentMax));
            }
        }
    }

    public void SortIntoGrid(IEnumerable<Point> points)
    {
        foreach (Point point in points)
        {
            GridBucket<Point> bucket = _grid.First(bucket => bucket.ContainsPoint(point));
            bucket.RemainingObjects.Add(point);
        }
    }

    public IEnumerable<Point> DrawAmountWithoutReturning(int amount)
    {
        List<Point> result = new();
        List<GridBucket<Point>> remainingBuckets = _grid.Where(bucket => bucket.RemainingObjects.Count > 0).ToList();
        List<GridBucket<Point>> alreadyUsedBuckets = new();
        Debug.Assert(remainingBuckets.Count > 0);

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, remainingBuckets.Count);
            GridBucket<Point> randomBucket = remainingBuckets[randomIndex];
            Point randomObject = randomBucket.GetRandomObject(markAsUsed: true);
            result.Add(randomObject);

            if (randomBucket.RemainingObjects.Count == 0)
            {
                randomBucket.SetAllRemaining();
            }
            
            remainingBuckets.Remove(randomBucket);
            alreadyUsedBuckets.Add(randomBucket);
            if (remainingBuckets.Count == 0)
            {
                remainingBuckets.AddRange(alreadyUsedBuckets);
                alreadyUsedBuckets.Clear();
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
        foreach (GridBucket<Point> bucket in _grid)
        {
            lines.Add(new Vector3(bucket.CoordinateMin.x, 0f, bucket.CoordinateMin.y));
            lines.Add(new Vector3(bucket.CoordinateMin.x, 0f, bucket.CoordinateMax.y));
            lines.Add(new Vector3(bucket.CoordinateMax.x, 0f, bucket.CoordinateMin.y));
            lines.Add(new Vector3(bucket.CoordinateMax.x, 0f, bucket.CoordinateMax.y));
            
            lines.Add(new Vector3(bucket.CoordinateMin.x, 0f, bucket.CoordinateMin.y));
            lines.Add(new Vector3(bucket.CoordinateMax.x, 0f, bucket.CoordinateMin.y));
            lines.Add(new Vector3(bucket.CoordinateMin.x, 0f, bucket.CoordinateMax.y));
            lines.Add(new Vector3(bucket.CoordinateMax.x, 0f, bucket.CoordinateMax.y));
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