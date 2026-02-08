using System.Collections;
using Audio;
using UnityEngine;

public class App : MonoBehaviour
{
    [SerializeField] private ShopRequest _shopRequest;
    [SerializeField] private DownloadableContent _downloadableContent;
    [SerializeField] private BuiltInContent _builtInContent;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private AudioManager _audioManager;
    private static App _instance;

    private void Init()
    {
        Application.targetFrameRate = 60;
        AppStateTransitions = new AppStateTransitions();
        
        InputManager.Init();
        _audioManager.Init();
        
        Shop = new Shop();
        Shop.Init();
        Shop.LoadFromCache();
        
        UserData = new UserData();
        UserData.Load();

        _builtInContent.Init();
    }

    private void OnApplicationQuit()
    {
        IsQuitting = true;
        UserData.Save();
    }

    public void NotifyAwakeAppState(IAppState state)
    {
        if (AppStateTransitions.CurrentState == null)
        {
            StartCoroutine(AppStateRoutine());
            StartCoroutine(LoadingRoutine());
            return;

            IEnumerator AppStateRoutine()
            {
                yield return AppStateTransitions.FromEntryPoint(state.Id);
            }

            IEnumerator LoadingRoutine()
            {
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

    public static bool IsQuitting;
    public static App Instance
    {
        get
        {
            if (_instance == null && !IsQuitting)
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
    public InputManager InputManager => _inputManager;
    public Shop Shop { get; private set; }
    public ShopRequest ShopRequest => _shopRequest;
    public UserData UserData { get; private set; }
    public DownloadableContent DownloadableContent => _downloadableContent;
    public BuiltInContent BuiltInContent => _builtInContent;
    public AudioManager AudioManager => _audioManager;
}
