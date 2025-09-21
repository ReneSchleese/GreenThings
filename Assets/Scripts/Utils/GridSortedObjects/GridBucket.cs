using System.Collections.Generic;
using UnityEngine;

public class GridBucket<T>
{
    public readonly Vector2 CoordinateMin;
    public readonly Vector2 CoordinateMax;
    public readonly List<T> RemainingObjects = new();
    public readonly List<T> AlreadyUsedObjects = new();

    public GridBucket(Vector2 coordinateMin, Vector2 coordinateMax)
    {
        CoordinateMin = coordinateMin;
        CoordinateMax = coordinateMax;
    }

    public bool ContainsPoint(Point point)
    {
        return point.X <= CoordinateMax.x && point.X >= CoordinateMin.x 
                                          && point.Z <= CoordinateMax.y && point.Z >= CoordinateMin.y;
    }

    public T GetRandomObject(bool markAsUsed)
    {
        Debug.Assert(RemainingObjects.Count > 0, "Objects.Count > 0");
        T result = RemainingObjects[Random.Range(0, RemainingObjects.Count)];
        if (markAsUsed)
        {
            RemainingObjects.Remove(result);
            AlreadyUsedObjects.Add(result);
        }
        return result;
    }

    public void SetAllRemaining()
    {
        RemainingObjects.AddRange(AlreadyUsedObjects);
        AlreadyUsedObjects.Clear();
    }
}