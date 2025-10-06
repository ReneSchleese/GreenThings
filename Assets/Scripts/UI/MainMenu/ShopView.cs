using System;
using UnityEngine;

public class ShopView : MonoBehaviour, IFadeableCanvasGroup
{
    [SerializeField] private CanvasGroup _canvasGroup;

    public event Action BackButtonPress;
    
    public CanvasGroup CanvasGroup => _canvasGroup;
}