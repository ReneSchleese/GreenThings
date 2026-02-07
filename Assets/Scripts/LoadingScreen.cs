using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LoadingScreen : MonoBehaviour, IAppState
{
    [SerializeField] private CanvasGroup _canvasGroup;
    private const float MIN_TIME = 2f;
    private float _startTime; 

    private void Start()
    {
        _startTime = Time.time;
    }

    public IEnumerator FadeIn()
    {
        yield return _canvasGroup.DOFade(1f, 0.5f).WaitForCompletion();
    } 
    
    public IEnumerator FadeOut()
    {
        yield return _canvasGroup.DOFade(0f, 0.5f).WaitForCompletion();
    }

    public bool EnoughTimeHasPassed => Time.time - _startTime > MIN_TIME;
    public IEnumerator TransitionOut()
    {
        Debug.Log("LoadingScreen.TransitionOff");
        yield return FadeOut();
    }
    
    public IEnumerator TransitionIn()
    {
        Debug.Log("LoadingScreen.TransitionTo");
        yield return FadeIn();
    }

    public void OnUnload()
    {
        Debug.Log("LoadingScreen.OnUnload");
    }

    public IEnumerator OnLoad(AppStateParams appStateParams = null)
    {
        Debug.Log("LoadingScreen.OnLoad");
        _canvasGroup.alpha = 0;
        yield break;
    }

    public AppState Id => AppState.LoadingScreen;
}
