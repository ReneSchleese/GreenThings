using System.Collections;
using UnityEngine;

public class App : MonoBehaviour
{
    [SerializeField] private ShopRequest _shopRequest;
    private static App _instance;

    private void Init()
    {
        Application.targetFrameRate = 60;
        AppStateTransitions = new AppStateTransitions();
        Shop = new Shop();
        Shop.Init();
        UserData = new UserData();
        UserData.Load();
    }

    private void OnApplicationQuit()
    {
        UserData.Save();
    }

    public void NotifyAwakeAppState(IAppState state)
    {
        if (AppStateTransitions.CurrentState == null)
        {
            StartCoroutine(EntryPointRoutine());
            return;

            IEnumerator EntryPointRoutine()
            {
                yield return AppStateTransitions.FromEntryPoint(state.Id);
                yield return BuildConfigLoader.LoadConfig();
                _shopRequest.Fetch(BuildConfigLoader.Config);
            }
        }
        else if(!AppStateTransitions.IsCurrentlyTransitioning)
        {
            Debug.LogWarning($"New state={state} during already active state={AppStateTransitions.CurrentState} and no transition in progress.");
        }
    }

    public void TransitionToGame()
    {
        Debug.Assert(!AppStateTransitions.IsCurrentlyTransitioning, "!AppStateTransitions.IsCurrentlyTransitioning");
        StartCoroutine(AppStateTransitions.ToGame());
    }

    public void TransitionToMainMenu()
    {
        Debug.Assert(!AppStateTransitions.IsCurrentlyTransitioning, "!AppStateTransitions.IsCurrentlyTransitioning");
        StartCoroutine(AppStateTransitions.ToMainMenu());
    }

    public static App Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject appPrefab = Resources.Load<GameObject>("App");
                GameObject appInstance = Instantiate(appPrefab);
                appInstance.name = "App";
                DontDestroyOnLoad(appInstance);
                _instance = appInstance.GetComponent<App>();
                _instance.Init();
            }

            return _instance;
        }
    }

    private AppStateTransitions AppStateTransitions { get; set; }
    public Shop Shop { get; private set; }
    public ShopRequest ShopRequest => _shopRequest;
    public UserData UserData { get; private set; }
}
