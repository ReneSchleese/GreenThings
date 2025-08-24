using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTreasureManager : MonoBehaviour
{
    [SerializeField] private GridSortedObjects _gidSortedTreasures;
    [SerializeField] private BuriedTreasureSpawn[] _treasureSpawns;

    public IEnumerator Setup()
    {
        yield break;
    }

    public void SetTreasureSpawns(IEnumerable<BuriedTreasureSpawn> spawns) => _treasureSpawns = spawns.ToArray();
}