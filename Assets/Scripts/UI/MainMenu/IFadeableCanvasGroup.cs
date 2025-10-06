using DG.Tweening;
using UnityEngine;

public interface IFadeableCanvasGroup
{
    CanvasGroup CanvasGroup { get; }

    public Tween Fade(bool fadeIn)
    {
        return CanvasGroup.DOFade(fadeIn ? 1f : 0f, 1f)
            .OnStart(() => CanvasGroup.interactable = false)
            .OnComplete(() => CanvasGroup.interactable = fadeIn);
    }

    public void FadeInstantly(bool fadeIn)
    {
        CanvasGroup.alpha = fadeIn ? 1f : 0f;
        CanvasGroup.interactable = fadeIn;
    }
}