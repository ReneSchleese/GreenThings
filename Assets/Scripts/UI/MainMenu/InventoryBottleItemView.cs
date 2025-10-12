using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBottleItemView : MonoBehaviour
{
    [SerializeField] private Image _thumbnailImage;
    [SerializeField] private Button _button;
    public event Action<InventoryBottleItemView> OnClick;
    private BottledMessageJson _messageJson;

    public void Set(BottledMessageJson messageJson)
    {
        _messageJson = messageJson;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => OnClick?.Invoke(this));
    }

    public BottledMessageJson Data => _messageJson;
}