using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private TextMeshProUGUI _requestState;
    [SerializeField] private CanvasGroup _canvasGroup;

    public event Action ShopButtonPress;
    public event Action InventoryButtonPress;

    public void OnLoad()
    {
        RootGroup = new FadeableCanvasGroup(_canvasGroup, 0.5f);
        _startGameButton.onClick.AddListener(() => App.Instance.TransitionToGame());
        _shopButton.onClick.AddListener(() => ShopButtonPress?.Invoke());
        _inventoryButton.onClick.AddListener(() => InventoryButtonPress?.Invoke());
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

    public FadeableCanvasGroup RootGroup { get; private set; }
}