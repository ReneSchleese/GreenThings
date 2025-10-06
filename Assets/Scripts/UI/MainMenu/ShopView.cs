using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour, IFadeableCanvasGroup
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _backButton;

    public event Action BackButtonPress;

    public void OnLoad()
    {
        _backButton.onClick.AddListener(() => BackButtonPress?.Invoke());
    }
    
    public CanvasGroup CanvasGroup => _canvasGroup;
}