using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
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
}
