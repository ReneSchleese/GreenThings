using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour, IFadeableCanvasGroup
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private TextMeshProUGUI _requestState;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void OnLoad()
    {
        _startGameButton.onClick.AddListener(OnStartGamePressed);
        _shopButton.onClick.AddListener(OnShopPressed);
        App.Instance.ShopRequest.OnStateChange += UpdateRequestState;
        _requestState.text = App.Instance.ShopRequest.State.ToString();
        return;
        
        void OnStartGamePressed()
        {
            App.Instance.TransitionToGame();
        }

        void OnShopPressed()
        {
            Debug.Log("Shop pressed");
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

    public CanvasGroup CanvasGroup => _canvasGroup;
}