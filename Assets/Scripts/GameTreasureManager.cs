using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTreasureManager : MonoBehaviour
{
    [SerializeField] private GridSortedObjects _gridSortedTreasures;
    [SerializeField] private BuriedTreasureSpawn[] _treasureSpawns;

    public IEnumerator Setup()
    {
        _gridSortedTreasures.CalculateGrid();
        _gridSortedTreasures.SortIntoGrid(_treasureSpawns);
        yield break;
    }

    public void SetTreasureSpawns(IEnumerable<BuriedTreasureSpawn> spawns) => _treasureSpawns = spawns.ToArray();
}