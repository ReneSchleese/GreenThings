using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GameTreasureManager : MonoBehaviour
{
    [SerializeField] private GridSortedPoints _gridSortedTreasures;
    [SerializeField] private Transform _treasureSpawnsParent;
    [SerializeField] private Transform _treasuresParent;
    [SerializeField] private Transform[] _treasureSpawns;

    public IEnumerator Setup(int numberOfTreasures)
    {
        _gridSortedTreasures.CalculateGrid();
        _gridSortedTreasures.SortIntoGrid(_treasureSpawns.Select(spawn => Point.FromTransform(spawn.transform)));
        foreach (Point point in _gridSortedTreasures.DrawAmountWithoutReturning(numberOfTreasures))
        {
            BuriedTreasure treasure = Game.Instance.Spawner.SpawnBuriedTreasure(point.ToVector3(), point.Rotation, _treasuresParent);
            BuriedTreasures.Add(treasure);
        }
        yield break;
    }
    
    public void OnTreasureOpened(BuriedTreasure treasure)
    {
        bool useVinyl = false;
        if(useVinyl)
        {
            Game.Instance.Spawner.SpawnVinyl(treasure.transform.position, Quaternion.identity);
        }
        
        int count = 10;
        const float upToSidewaysWeight = 0.15f;
        for (int i = 0; i < count; i++)
        {
            Coin coin = Game.Instance.Spawner.SpawnCoin(treasure.transform.position, Quaternion.identity);
            float angle = i * Mathf.PI * 2f / count;
            Vector3 dir = (1f - upToSidewaysWeight) * Vector3.up + upToSidewaysWeight * new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            const float strength = 1.4f;
            coin.ApplyForce(dir.normalized * strength * Physics.gravity.magnitude);
            coin.GroundedCheckIsEnabled = false;
            DOVirtual.DelayedCall(0.2f, () =>
            {
                coin.GroundedCheckIsEnabled = true;
                coin.CollectionIsAllowed = true;
            });
        }
    }

    public BuriedTreasure GetNearestUnopenedTreasure(Vector3 position)
    {
        var unopenedTreasures = BuriedTreasures.Where(treasure => !treasure.IsOpen).ToList();
        if (unopenedTreasures.Count == 0) return null;
        unopenedTreasures.Sort((treasure1, treasure2) => Vector3.Distance(treasure1.transform.position, position)
            .CompareTo(Vector3.Distance(treasure2.transform.position, position)));
        return unopenedTreasures[0];
    }

    public BuriedTreasure GetRandomUnopenedTreasure()
    {
        var unopenedTreasures = BuriedTreasures.Where(treasure => !treasure.IsOpen).ToList();
        if (unopenedTreasures.Count == 0) return null;
        return unopenedTreasures[Random.Range(0, unopenedTreasures.Count)];
    }

    public Transform[] TreasureSpawns
    {
        get => _treasureSpawns;
        set => _treasureSpawns = value;
    }

    public Transform TreasureSpawnsParent => _treasureSpawnsParent;
    public GridSortedPoints GridSortedTreasures => _gridSortedTreasures;
    public List<BuriedTreasure> BuriedTreasures { get; } = new();
}