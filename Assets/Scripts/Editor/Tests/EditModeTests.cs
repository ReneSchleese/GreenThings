using NUnit.Framework;
using UnityEngine;

public static class EditModeTests
{
    [Test]
    public static void Test()
    {
        Debug.Log("Test");
        GridSortedObjects objects  = new GridSortedObjects
        {
            GridMax = new Vector2(10, 10),
            GridMin = new Vector2(-10, -10),
            SegmentsX = 2,
            SegmentsZ = 2
        };
        Debug.Log(objects.GridMax);
    }
}
