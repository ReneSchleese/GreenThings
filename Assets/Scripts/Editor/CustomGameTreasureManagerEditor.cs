using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(GameTreasureManager))]
public class CustomGameTreasureManagerEditor : Editor
{
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
                .Where(spawn => spawn.name.StartsWith("BuriedTreasureSpawn")).ToArray();
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
        for (float x = gridMin.x; x < gridMax.x; x += stepSize)
        {
            for (float z = gridMin.y; z < gridMax.y; z += stepSize)
            {
                Vector3 origin = new(x, height, z);
                Physics.Raycast(origin, Vector3.down, out RaycastHit hit, height - minY, LayerMask.GetMask("Environment"));
                if (hit.collider != null)
                {
                    Debug.DrawRay(hit.point, Vector3.up * 2, Color.green, 3f);   
                }
            }
        }
            
        Transform[] spawns = manager.TreasureSpawnsParent.GetComponentsInChildren<Transform>()
            .Where(spawn => spawn.name.StartsWith("BuriedTreasureSpawn")).ToArray();
        Undo.RecordObject(manager, "Set Treasure Spawns");
        manager.TreasureSpawns = spawns.ToArray();
        EditorUtility.SetDirty(manager);
    }
}