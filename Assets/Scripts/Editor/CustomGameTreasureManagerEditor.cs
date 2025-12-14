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
            Scene treasureScene = SceneManager.GetSceneByName("Game_Treasure");
            if (!treasureScene.isLoaded)
            {
                Debug.Log("Game_Treasure not loaded");
                return;
            }
            
            foreach (Transform existingSpawn in manager.TreasureSpawns)
            {
                Undo.DestroyObjectImmediate(existingSpawn.gameObject);
            }
            
            Transform[] spawns = manager.TreasureSpawnsParent.GetComponentsInChildren<Transform>()
                .Where(spawn => spawn.name.StartsWith("BuriedTreasureSpawn")).ToArray();
            Undo.RecordObject(manager, "Set Treasure Spawns");
            manager.TreasureSpawns = spawns.ToArray();
            EditorUtility.SetDirty(manager);
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
}