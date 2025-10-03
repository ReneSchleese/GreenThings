using UnityEngine;

public class App : MonoBehaviour
{
    private static App _instance;

    private void Init()
    {
        SceneTransitions = new SceneTransitions();
        Application.targetFrameRate = 60;
    }

    public void TryUpdateAppStateFromSceneEntry(IAppState state)
    {
        if (SceneTransitions.CurrentState == null)
        {
            SceneTransitions.CurrentState = state;
        }
        else
        {
            Debug.LogWarning($"Trying to set state={state} although app is already in state={SceneTransitions.CurrentState}");
        }
    }

    public void StartGame()
    {
        StartCoroutine(SceneTransitions.StartGame());
    }

    public static App Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instanceObject = new GameObject("App");
                DontDestroyOnLoad(instanceObject);
                App instance = instanceObject.AddComponent<App>();
                _instance = instance;
                _instance.Init();
            }

            return _instance;
        }
    }

    private SceneTransitions SceneTransitions { get; set; }
}
