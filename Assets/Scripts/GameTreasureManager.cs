using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTreasureManager : MonoBehaviour
{
    [SerializeField] private GridSortedObjects _gridSortedTreasures;
    [SerializeField] private Transform _treasureSpawnsParent;
    [SerializeField] private Transform _treasuresParent;
    [SerializeField] private Transform[] _treasureSpawns;

    public IEnumerator Setup(int numberOfTreasures)
    {
        _gridSortedTreasures.CalculateGrid();
        _gridSortedTreasures.SortIntoGrid(_treasureSpawns);
        foreach (Transform spawn in _gridSortedTreasures.DrawAmountWithoutReturning(numberOfTreasures))
        {
            Game.Instance.Spawner.SpawnBuriedTreasure(spawn.position, spawn.rotation, _treasuresParent);
        }
        yield break;
    }

    public void SetTreasureSpawns(IEnumerable<Transform> spawns) => _treasureSpawns = spawns.ToArray();
    public Transform TreasureSpawnsParent => _treasureSpawnsParent;
}