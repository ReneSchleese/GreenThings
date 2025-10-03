using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IAppState
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _shopButton;

    private void Awake()
    {
        App.Instance.TryUpdateAppStateFromSceneEntry(this);
    }

    void Start()
    {
        _startGameButton.onClick.AddListener(OnStartGamePressed);
        _shopButton.onClick.AddListener(OnShopPressed);

        void OnStartGamePressed()
        {
            App.Instance.StartGame();
        }

        void OnShopPressed()
        {
            Debug.Log("Shop");
        }
    }

    public IEnumerator TransitionOff()
    {
        Debug.Log("MainMenu.TransitionOff");
        yield break;
    }

    public IEnumerator TransitionTo()
    {
        Debug.Log("MainMenu.TransitionTo");
        yield break;
    }

    public void OnUnload()
    {
        Debug.Log("MainMenu.OnUnload");
    }

    public void OnLoad()
    {
        Debug.Log("MainMenu.OnLoadComplete");
    }

    public AppState Id => AppState.MainMenu;
}
