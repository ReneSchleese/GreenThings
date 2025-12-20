using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(GameTreasureManager))]
public class CustomGameTreasureManagerEditor : Editor
{
    private const string BURIED_TREASURE_SPAWN = "BuriedTreasureSpawn";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GameTreasureManager manager = (GameTreasureManager)target;
        if (GUILayout.Button("Generate treasure-spawns"))
        {
            GenerateTreasureSpawns(manager);
        }
        if (GUILayout.Button("Find and set all treasure-spawns"))
        {
            Transform[] spawns = manager.TreasureSpawnsParent.GetComponentsInChildren<Transform>()
                .Where(spawn => spawn.name.StartsWith(BURIED_TREASURE_SPAWN)).ToArray();
            Undo.RecordObject(manager, "Set Treasure Spawns");
            manager.TreasureSpawns = spawns.ToArray();
            EditorUtility.SetDirty(manager);
        }
    }

    private void GenerateTreasureSpawns(GameTreasureManager manager)
    {
        if (!SceneManager.GetSceneByName("Game_Treasure").isLoaded)
        {
            Debug.Log("Game_Treasure not loaded");
            return;
        }
        
        foreach (Transform existingSpawn in manager.TreasureSpawns)
        {
            Undo.DestroyObjectImmediate(existingSpawn.gameObject);
        }

        Vector2 gridMax = manager.GridSortedTreasures.GridMax;
        Vector2 gridMin = manager.GridSortedTreasures.GridMin;
        const float stepSize = 2f;
        const float height = 100f;
        const float minY = -5f;
        List<Transform> treasureSpawns = new List<Transform>();
        for (float x = gridMin.x; x < gridMax.x; x += stepSize)
        {
            for (float z = gridMin.y; z < gridMax.y; z += stepSize)
            {
                Vector3 origin = new(x, height, z);
                Physics.Raycast(origin, Vector3.down, out RaycastHit hit, height - minY, LayerMask.GetMask("Environment"));
                if (hit.collider == null || !hit.collider.TryGetComponent(out EnvironmentObject envObj) || !envObj.AllowsTreasureSpawn)
                {
                    continue;
                }
                GameObject spawn = new(BURIED_TREASURE_SPAWN);
                spawn.transform.position = hit.point;
                spawn.transform.parent = manager.TreasureSpawnsParent;
                treasureSpawns.Add(spawn.transform);
                Debug.DrawRay(hit.point, Vector3.up * 2, Color.green, 3f);
            }
        }
        
        Undo.RecordObject(manager, "Set Treasure Spawns");
        manager.TreasureSpawns = treasureSpawns.ToArray();
        EditorUtility.SetDirty(manager);
    }
}