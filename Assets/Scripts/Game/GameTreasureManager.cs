using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTreasureManager : MonoBehaviour
{
    [SerializeField] private GridSortedPoints _gridSortedTreasures;
    [SerializeField] private Transform _treasureSpawnsParent;
    [SerializeField] private Transform _treasuresParent;
    [SerializeField] private Transform[] _treasureSpawns;

    private readonly List<BuriedTreasure> _buriedTreasures = new();

    public IEnumerator Setup(int numberOfTreasures)
    {
        _gridSortedTreasures.CalculateGrid();
        _gridSortedTreasures.SortIntoGrid(_treasureSpawns.Select(spawn => Point.FromTransform(spawn.transform)));
        foreach (Point point in _gridSortedTreasures.DrawAmountWithoutReturning(numberOfTreasures))
        {
            BuriedTreasure treasure = Game.Instance.Spawner.SpawnBuriedTreasure(point.ToVector3(), point.Rotation, _treasuresParent);
            _buriedTreasures.Add(treasure);
        }
        yield break;
    }

    public BuriedTreasure GetNearestUnopenedTreasure(Vector3 position)
    {
        var unopenedTreasures = _buriedTreasures.Where(treasure => !treasure.IsOpen).ToList();
        unopenedTreasures.Sort((treasure1, treasure2) => Vector3.Distance(treasure1.transform.position, position)
            .CompareTo(Vector3.Distance(treasure2.transform.position, position)));
        return unopenedTreasures[0];
    }

    public void SetTreasureSpawns(IEnumerable<Transform> spawns) => _treasureSpawns = spawns.ToArray();
    public Transform TreasureSpawnsParent => _treasureSpawnsParent;
}