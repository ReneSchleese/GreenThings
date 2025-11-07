using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RadialMenuItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _animatedScale;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void Init(string label, Action inputAction)
    {
        _label.text = $"<font-weight=400>{label}";
        InputAction = inputAction;
    }

    public void SetHighlighted(bool highlighted, bool animate)
    {
        DOTween.Kill(this);
        Vector3 targetScale = highlighted ? new Vector3(1.5f, 1.5f, 1f) : Vector3.one;
        float targetOpacity = highlighted ? 1 : 0.4f;
        if (animate)
        {
            Sequence sequence = DOTween.Sequence().SetId(this);
            const float duration = 0.25f;
            sequence.Insert(0f, _animatedScale.DOScale(targetScale, duration).SetEase(Ease.OutCubic));
            sequence.Insert(0f, _canvasGroup.DOFade(targetOpacity, duration).SetEase(Ease.OutCubic));
            sequence.OnComplete(OnComplete);
        }
        else
        {
            OnComplete();
        }

        return;

        void OnComplete()
        {
            _animatedScale.localScale = targetScale;
            _canvasGroup.alpha = targetOpacity;
        }
    }

    public RectTransform RectTransform => _rectTransform;
    public Action InputAction { get; private set; }
}