using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public static class GridSortedPointsTests
{
    [Test]
    public static void Buckets4_Items3_AllInOneBucket_AssertNoDuplicates()
    {
        for (int i = 0; i < 1000; i++)
        {
            var pointsGrid = GenerateTestGrid();
            var spawnPoints = new List<Point>
            {
                new() { X = -5, Y = 0, Z = -5},
                new() { X = -6, Y = 0, Z = -5},
                new() { X = -7, Y = 0, Z = -5},
            };
            pointsGrid.SortIntoGrid(spawnPoints);
            const int amountToDraw = 3;
            HashSet<Point> drawnPoints = pointsGrid.DrawAmountWithoutReturning(amountToDraw).ToHashSet();
            Assert.That(drawnPoints.Count, Is.EqualTo(3));   
        }
    }
    
    [Test]
    public static void Buckets4_Items3_EachBucket1ItemMax_AssertNoDuplicates()
    {
        for (int i = 0; i < 1000; i++)
        {
            var pointsGrid = GenerateTestGrid();
            var spawnPoints = new List<Point>
            {
                new() { X = -5, Y = 0, Z = -5},
                new() { X = -5, Y = 0, Z = 5},
                new() { X = 5, Y = 0, Z = -5},
                new() { X = 5, Y = 0, Z = 5},
            };
            pointsGrid.SortIntoGrid(spawnPoints);
            const int amountToDraw = 3;
            HashSet<Point> drawnPoints = pointsGrid.DrawAmountWithoutReturning(amountToDraw).ToHashSet();
            Assert.That(drawnPoints.Count, Is.EqualTo(3));   
        }
    }
    
    [Test]
    public static void Draw3ItemsFrom2Buckets_AssertNoDuplicates()
    {
        for (int i = 0; i < 1000; i++)
        {
            var pointsGrid = new GridSortedPoints
            {
                GridMax = new Vector2(43.75f, 67.5f),
                GridMin = new Vector2(-38.5f, -47.5f),
                SegmentsX = 3,
                SegmentsZ = 4
            };
            pointsGrid.CalculateGrid();
            var spawnPoints = new List<Point>
            {
                new() { X = -13.29f, Y = 0, Z = -34.12f},
                new() { X = -0.72f, Y = 0, Z = -33.1f},
                new() { X = 14.42f, Y = 0, Z = -33.41f},
            };
            pointsGrid.SortIntoGrid(spawnPoints);
            const int amountToDraw = 3;
            HashSet<Point> drawnPoints = pointsGrid.DrawAmountWithoutReturning(amountToDraw).ToHashSet();
            Assert.That(drawnPoints.Count, Is.EqualTo(3));   
        }
    }

    private static GridSortedPoints GenerateTestGrid()
    {
        GridSortedPoints pointsGrid  = new GridSortedPoints
        {
            GridMax = new Vector2(10, 10),
            GridMin = new Vector2(-10, -10),
            SegmentsX = 2,
            SegmentsZ = 2
        };
        pointsGrid.CalculateGrid();
        return pointsGrid;
    }
}
