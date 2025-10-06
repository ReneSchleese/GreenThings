using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour, IAppState
{
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private ShopView _shopView;
    
    private void Awake()
    {
        App.Instance.NotifyAwakeAppState(this);
    }

    public IEnumerator OnLoad()
    {
        _mainMenuView.OnLoad();
        _shopView.OnLoad();
        _mainMenuView.ShopButtonPress += SwitchToShopView;
        _shopView.BackButtonPress += SwitchToMainMenuView;
        yield break;
    }

    public void OnUnload()
    {
        _mainMenuView.OnUnload();
        _shopView.OnUnload();
        _mainMenuView.ShopButtonPress -= SwitchToShopView;
        _shopView.BackButtonPress -= SwitchToMainMenuView;
    }

    private void SwitchToShopView()
    {
        ((IFadeableCanvasGroup)_mainMenuView).Fade(fadeIn: false);
        ((IFadeableCanvasGroup)_shopView).Fade(fadeIn: true);
    }

    private void SwitchToMainMenuView()
    {
        ((IFadeableCanvasGroup)_mainMenuView).Fade(fadeIn: true);
        ((IFadeableCanvasGroup)_shopView).Fade(fadeIn: false);
    }

    public IEnumerator TransitionIn()
    {
        Debug.Log($"{nameof(MainMenu)}.{nameof(TransitionIn)}");
        ((IFadeableCanvasGroup)_mainMenuView).FadeInstantly(fadeIn: true);
        ((IFadeableCanvasGroup)_shopView).FadeInstantly(fadeIn: false);
        yield break;
    }

    public IEnumerator TransitionOut()
    {
        Debug.Log($"{nameof(MainMenu)}.{nameof(TransitionOut)}");
        yield break;
    }

    public AppState Id => AppState.MainMenu;
}
