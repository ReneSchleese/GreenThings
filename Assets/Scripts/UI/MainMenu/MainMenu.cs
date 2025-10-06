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
        yield break;
    }

    public void OnUnload()
    {
        _mainMenuView.OnUnload();
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
