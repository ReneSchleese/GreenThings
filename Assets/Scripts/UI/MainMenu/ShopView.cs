using UnityEngine;

public class ShopView : MonoBehaviour, IFadeableCanvasGroup
{
    [SerializeField] private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup => _canvasGroup;
}