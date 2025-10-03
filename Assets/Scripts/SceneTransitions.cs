using UnityEngine;

public class SceneTransitions : MonoBehaviour
{
    private static SceneTransitions _instance;

    public void StartGame()
    {
        
    }

    public static SceneTransitions Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instanceObject = new GameObject("SceneTransitions");
                DontDestroyOnLoad(instanceObject);
                SceneTransitions instance = instanceObject.AddComponent<SceneTransitions>();
                _instance = instance;
            }

            return _instance;
        }
    }
}