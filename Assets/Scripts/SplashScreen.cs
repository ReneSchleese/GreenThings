using System.Collections;
using UnityEngine;

public class SplashScreen : MonoBehaviour, IAppState
{
    [SerializeField] private Canvas _canvas;

    private void Awake()
    {
        App.Instance.NotifyAwakeAppState(this);
    }

    public IEnumerator TransitionOut() { yield break; }

    public IEnumerator TransitionIn()
    {
        StartCoroutine(WaitThenTransitionToMainMenu());
        yield break;
        
        IEnumerator WaitThenTransitionToMainMenu()
        {
            yield return new WaitForSeconds(1f);
            App.Instance.TransitionToMainMenu();
        }
    }

    public void OnUnload() { }
    
    public IEnumerator OnLoad(AppStateParams appStateParams = null) { yield break; }

    public AppState Id => AppState.SplashScreen;
}
