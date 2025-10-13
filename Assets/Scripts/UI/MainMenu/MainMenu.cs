using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour, IAppState
{
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private ShopView _shopView;
    [SerializeField] private InventoryView _inventoryView;
    
    private void Awake()
    {
        App.Instance.NotifyAwakeAppState(this);
    }

    public IEnumerator OnLoad()
    {
        _mainMenuView.OnLoad();
        _shopView.OnLoad();
        _inventoryView.OnLoad();
        _mainMenuView.ShopButtonPress += SwitchToShopView;
        _mainMenuView.InventoryButtonPress += SwitchToInventoryView;
        _shopView.BackButtonPress += SwitchToMainMenuView;
        _inventoryView.BackButtonPress += SwitchToMainMenuView;
        yield return new WaitUntil(() => BuildConfigLoader.IsLoaded);
    }

    public void OnUnload()
    {
        _mainMenuView.OnUnload();
        _shopView.OnUnload();
        _inventoryView.OnUnload();
        _mainMenuView.ShopButtonPress -= SwitchToShopView;
        _mainMenuView.InventoryButtonPress -= SwitchToInventoryView;
        _shopView.BackButtonPress -= SwitchToMainMenuView;
        _inventoryView.BackButtonPress -= SwitchToMainMenuView;
    }
    
    private void SwitchToMainMenuView()
    {
        ((IFadeableCanvasGroup)_mainMenuView).Fade(fadeIn: true);
        ((IFadeableCanvasGroup)_shopView).Fade(fadeIn: false);
        ((IFadeableCanvasGroup)_inventoryView).Fade(fadeIn: false);
    }

    private void SwitchToShopView()
    {
        ((IFadeableCanvasGroup)_mainMenuView).Fade(fadeIn: false);
        ((IFadeableCanvasGroup)_shopView).Fade(fadeIn: true);
        ((IFadeableCanvasGroup)_inventoryView).Fade(fadeIn: false);
    }
    
    private void SwitchToInventoryView()
    {
        ((IFadeableCanvasGroup)_mainMenuView).Fade(fadeIn: false);
        ((IFadeableCanvasGroup)_shopView).Fade(fadeIn: false);
        ((IFadeableCanvasGroup)_inventoryView).Fade(fadeIn: true);
    }

    public IEnumerator TransitionIn()
    {
        Debug.Log($"{nameof(MainMenu)}.{nameof(TransitionIn)}");
        ((IFadeableCanvasGroup)_mainMenuView).FadeInstantly(fadeIn: true);
        ((IFadeableCanvasGroup)_shopView).FadeInstantly(fadeIn: false);
        ((IFadeableCanvasGroup)_inventoryView).FadeInstantly(fadeIn: false);
        _inventoryView.OnTransitionIn();
        yield break;
    }

    public IEnumerator TransitionOut()
    {
        Debug.Log($"{nameof(MainMenu)}.{nameof(TransitionOut)}");
        yield break;
    }

    public AppState Id => AppState.MainMenu;
}
