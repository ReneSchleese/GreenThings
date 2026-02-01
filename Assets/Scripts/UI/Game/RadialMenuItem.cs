using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RadialMenuItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private RectTransform _rectTransform, _animatedScale, _textContainer;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void Init(string label, Action inputAction)
    {
        _label.text = $"<font-weight=600>{label}";
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

    public void Layout(Vector2 anchoredPos)
    {
        bool isCentered = Mathf.Abs(anchoredPos.x) < 1f;
        bool isLeftAligned = !isCentered && anchoredPos.x < 0;
        _rectTransform.anchoredPosition = anchoredPos;
        _rectTransform.rotation = Quaternion.identity;
        if (isCentered)
        {
            const float offsetInPx = 90;
            _rectTransform.anchoredPosition += Vector2.up * (offsetInPx * (anchoredPos.y > 0 ? 1 : -0.5f));
        }
        else
        {
            _textContainer.pivot = new Vector2(isLeftAligned ? 1f : 0f, 0.5f);
            _textContainer.anchoredPosition = Vector2.zero;
        }
    }
    
    public void SetText(string text) => _label.text = text;

    public Action InputAction { get; private set; }
}