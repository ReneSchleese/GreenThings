using System.Collections;
using UnityEngine;

public class GameTreasureManager : MonoBehaviour
{
    [SerializeField] private GridSortedObjects _gidSortedTreasures;
    [SerializeField] private BuriedTreasureSpawn[] _treasureSpawns;

    public IEnumerator Setup()
    {
        yield break;
    }
}