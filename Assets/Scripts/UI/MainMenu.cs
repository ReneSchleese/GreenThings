using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IAppState
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private TextMeshProUGUI _requestState;

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

    public IEnumerator OnLoad()
    {
        _startGameButton.onClick.AddListener(OnStartGamePressed);
        _shopButton.onClick.AddListener(OnShopPressed);
        App.Instance.ShopRequest.OnStateChange += UpdateRequestState;
        _requestState.text = "";
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

    public void OnUnload()
    {
        App.Instance.ShopRequest.OnStateChange -= UpdateRequestState;
    }

    private void UpdateRequestState(ShopRequest.RequestState state)
    {
        _requestState.text = state.ToString();
    }

    public AppState Id => AppState.MainMenu;
}
