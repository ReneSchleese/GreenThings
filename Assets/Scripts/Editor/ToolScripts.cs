using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ToolScripts
{
    [MenuItem("Tools/Reverse Mirrored BoxColliders")]
    private static void ReverseMirroredBoxColliders()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.Log("No gameObject selected");
            return;
        }

        BoxCollider[] boxColliders = selected.transform.GetComponentsInChildren<BoxCollider>();
        
        BoxCollider[] boxCollidersX = boxColliders.Where(col => col.transform.lossyScale.x < 0 ^ col.size.x < 0).ToArray();
        foreach (BoxCollider boxCollider in boxCollidersX)
        {
            Vector3 size = boxCollider.size;
            size = new Vector3(-size.x, size.y, size.z);
            boxCollider.size = size;
        }

        BoxCollider[] boxCollidersZ = boxColliders.Where(col => col.transform.lossyScale.z < 0 ^ col.size.z < 0).ToArray();
        foreach (BoxCollider boxCollider in boxCollidersZ)
        {
            Vector3 size = boxCollider.size;
            size = new Vector3(size.x, size.y, -size.z);
            boxCollider.size = size;
        }
        
        Debug.Log($"Fixed {boxCollidersX.Length} boxColliders with negative X and {boxCollidersZ.Length} ones with negative Z lossyScale.");
        if (boxCollidersX.Length > 0 || boxCollidersZ.Length > 0)
        {
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
    }
}
