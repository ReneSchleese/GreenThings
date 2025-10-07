using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour, IFadeableCanvasGroup
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private TextMeshProUGUI _requestState;
    [SerializeField] private CanvasGroup _canvasGroup;

    public event Action ShopButtonPress;

    public void OnLoad()
    {
        _startGameButton.onClick.AddListener(() => App.Instance.TransitionToGame());
        _shopButton.onClick.AddListener(() => ShopButtonPress?.Invoke());
        App.Instance.ShopRequest.OnStateChange += UpdateRequestState;
        _requestState.text = App.Instance.ShopRequest.State.ToString();
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