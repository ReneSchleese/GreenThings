using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameTreasureManager))]
public class CustomGameTreasureManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GameTreasureManager manager = (GameTreasureManager)target;
        if (GUILayout.Button("Find and set all treasure-spawns"))
        {
            Transform[] spawns = manager.TreasureSpawnsParent.GetComponentsInChildren<Transform>()
                .Where(spawn => spawn.name.StartsWith("BuriedTreasureSpawn")).ToArray();
            Undo.RecordObject(manager, "Set Treasure Spawns");
            manager.SetTreasureSpawns(spawns);
            EditorUtility.SetDirty(manager);
        }
    }
}