using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBottleItemView : MonoBehaviour
{
    [SerializeField] private Image _thumbnailImage;
    [SerializeField] private Button _button;
    [SerializeField] private RawImage _thumbnailRawImage;
    public event Action<InventoryBottleItemView> OnClick;
    private BottledMessageJson _messageJson;

    public void Set(BottledMessageJson messageJson)
    {
        _messageJson = messageJson;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => OnClick?.Invoke(this));
        App.Instance.DownloadableContent.GetTexture(messageJson.thumbnail_url, OnTextureLoaded);
    }

    private void OnTextureLoaded(Texture2D texture)
    {
        _thumbnailRawImage.texture = texture;
    }

    public BottledMessageJson Data => _messageJson;
}