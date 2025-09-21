using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public static class EditModeTests
{
    [Test]
    public static void AllInOneBucket_AssertNoDuplicates()
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
    public static void AllBucketsWithOnePointMax_AssertNoDuplicates()
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
