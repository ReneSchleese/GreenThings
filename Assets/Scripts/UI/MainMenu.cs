using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IAppState
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _shopButton;

    private void Awake()
    {
        App.Instance.NotifyAwakeAppState(this);
    }

    public IEnumerator TransitionOut()
    {
        Debug.Log("MainMenu.TransitionOff");
        yield break;
    }

    public IEnumerator TransitionIn()
    {
        Debug.Log("MainMenu.TransitionTo");
        yield break;
    }

    public void OnUnload()
    {
        Debug.Log("MainMenu.OnUnload");
    }

    public IEnumerator OnLoad()
    {
        _startGameButton.onClick.AddListener(OnStartGamePressed);
        _shopButton.onClick.AddListener(OnShopPressed);
        yield break;

        void OnStartGamePressed()
        {
            App.Instance.TransitionToGame();
        }

        void OnShopPressed()
        {
            App.Instance.FetchShop();
        }
    }

    public AppState Id => AppState.MainMenu;
}
