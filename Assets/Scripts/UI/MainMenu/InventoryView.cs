using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour, IFadeableCanvasGroup
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _backButton;
    [SerializeField] private InventoryBottleItemView _bottleItemViewPrefab;
    
    public event Action BackButtonPress;

    public void OnLoad()
    {
        _backButton.onClick.AddListener(() => BackButtonPress?.Invoke());    
    }

    public void OnUnload()
    {
        
    }
    
    public CanvasGroup CanvasGroup => _canvasGroup;
}