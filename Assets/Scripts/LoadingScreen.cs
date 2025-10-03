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
    public IEnumerator PrepareBeingTransitionedFrom()
    {
        Debug.Log("LoadingScreen.PrepareBeingTransitionedFrom");
        yield return FadeOut();
    }
    
    public IEnumerator PrepareBeingTransitionedTo()
    {
        Debug.Log("LoadingScreen.PrepareBeingTransitionedTo");
        yield return FadeIn();
    }

    public void OnUnload()
    {
        Debug.Log("LoadingScreen.OnUnload");
    }

    public void OnLoad()
    {
        Debug.Log("LoadingScreen.OnLoad");
        _canvasGroup.alpha = 0;
    }

    public AppState Id => AppState.LoadingScreen;
}
