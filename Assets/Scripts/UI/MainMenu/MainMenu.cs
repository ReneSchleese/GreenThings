using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour, IAppState
{
    [SerializeField] private MainMenuView _mainMenuView;
    
    private void Awake()
    {
        App.Instance.NotifyAwakeAppState(this);
    }

    public IEnumerator TransitionOut()
    {
        Debug.Log($"{nameof(MainMenu)}.{nameof(TransitionOut)}");
        yield break;
    }

    public IEnumerator TransitionIn()
    {
        Debug.Log($"{nameof(MainMenu)}.{nameof(TransitionIn)}");
        yield break;
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

    public AppState Id => AppState.MainMenu;
}
