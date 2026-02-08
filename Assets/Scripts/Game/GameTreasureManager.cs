using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

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
        VinylId? vinylId = GetRandomUnownedVinylId();
        bool spawnVinyl = vinylId is not null && Random.Range(0f, 1f) < 0.05f;
        if(spawnVinyl)
        {
            Vinyl vinyl = Game.Instance.Spawner.SpawnVinyl(treasure.transform.position, Quaternion.identity, vinylId.Value);
            LaunchUpwards(vinyl, Random.Range(0f, 1f) * 360);
        }
        else
        {
            const int count = 10;
            for (int i = 0; i < count; i++)
            {
                Coin coin = Game.Instance.Spawner.SpawnCoin(treasure.transform.position, Quaternion.identity);
                float angle = i * Mathf.PI * 2f / count;
                LaunchUpwards(coin, angle);
            }
        }

        return;

        void LaunchUpwards(ICollectable collectable, float angle)
        {
            const float upToSidewaysWeight = 0.15f;
            Vector3 dir = (1f - upToSidewaysWeight) * Vector3.up + upToSidewaysWeight * new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            const float strength = 1.4f;
            collectable.ApplyForce(dir.normalized * strength * Physics.gravity.magnitude);
            collectable.GroundedCheckIsEnabled = false;
            DOVirtual.DelayedCall(0.2f, () =>
            {
                collectable.GroundedCheckIsEnabled = true;
                collectable.CollectionIsAllowed = true;
            });
        }
    }

    private VinylId? GetRandomUnownedVinylId()
    {
        VinylId[] allIds = (VinylId[])Enum.GetValues(typeof(VinylId));

        var unownedIds = allIds
            .Where(id => !App.Instance.UserData.OwnedVinylIds.Contains(id))
            .ToArray();

        if (unownedIds.Length == 0)
            return null;

        return unownedIds[Random.Range(0, unownedIds.Length)];
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