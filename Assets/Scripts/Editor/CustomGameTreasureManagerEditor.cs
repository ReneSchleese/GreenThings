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
            BuriedTreasureSpawn[] spawns = Resources.FindObjectsOfTypeAll<BuriedTreasureSpawn>()
                .Where(spawn => spawn.gameObject.scene.IsValid()).ToArray();
            Undo.RecordObject(manager, "Set Treasure Spawns");
            manager.SetTreasureSpawns(spawns);
            EditorUtility.SetDirty(manager);
        }
    }
}