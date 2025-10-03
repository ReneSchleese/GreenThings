using UnityEngine;

public class App : MonoBehaviour
{
    private static App _instance;

    private void Init()
    {
        AppStateTransitions = new AppStateTransitions();
        Application.targetFrameRate = 60;
    }

    /// <summary>
    /// Only relevant in editor. Call this there was a "warm start", meaning starting playmode from non-app-entry-point scene.
    /// </summary>
    public void NotifyAwakeAppState(IAppState state)
    {
        if (AppStateTransitions.CurrentState == null)
        {
            StartCoroutine(AppStateTransitions.FromEntryPoint(state.Id));
        }
        else
        {
            Debug.LogWarning($"Trying to set state={state} although app is already in state={AppStateTransitions.CurrentState}");
        }
    }

    public void StartGame()
    {
        StartCoroutine(AppStateTransitions.StartGame());
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

    private AppStateTransitions AppStateTransitions { get; set; }
}
