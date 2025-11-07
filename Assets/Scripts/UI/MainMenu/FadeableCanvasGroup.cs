using DG.Tweening;
using UnityEngine;

public class FadeableCanvasGroup
{
    private CanvasGroup _canvasGroup;
    private float _duration;
    
    public FadeableCanvasGroup(CanvasGroup canvasGroup, float fadeDuration)
    {
        _canvasGroup = canvasGroup;
        _duration = fadeDuration;
    }

    public Tween Fade(bool fadeIn)
    {
        return _canvasGroup.DOFade(fadeIn ? 1f : 0f, _duration)
            .SetEase(Ease.OutCubic)
            .OnStart(() =>
            {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = true;
            })
            .OnComplete(() =>
            {
                _canvasGroup.interactable = fadeIn;
                _canvasGroup.blocksRaycasts = fadeIn;
                OnFadeComplete(fadeIn);
            });
    }

    public void FadeInstantly(bool fadeIn)
    {
        _canvasGroup.alpha = fadeIn ? 1f : 0f;
        _canvasGroup.interactable = fadeIn;
        _canvasGroup.blocksRaycasts = fadeIn;
        OnFadeComplete(fadeIn);
    }

    void OnFadeComplete(bool fadeIn) {}
}