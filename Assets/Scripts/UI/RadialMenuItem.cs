using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RadialMenuItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _animatedScale;

    public void Init(string label, Action inputAction)
    {
        _label.text = label;
        InputAction = inputAction;
    }

    public void SetHighlighted(bool highlighted)
    {
        DOTween.Kill(this);
        Vector3 targetScale = highlighted ? new Vector3(1.4f, 1.4f, 1f) : Vector3.one; 
        _animatedScale.DOScale(targetScale, 0.33f).SetId(this).SetEase(Ease.OutCubic);
    }

    public RectTransform RectTransform => _rectTransform;
    public Action InputAction { get; private set; }
}