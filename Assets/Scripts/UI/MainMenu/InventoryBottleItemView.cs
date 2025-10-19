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

    private void OnDestroy()
    {
        App.Instance.DownloadableContent.TextureIsReady -= OnTextureLoaded;
    }

    public void Set(BottledMessageJson messageJson)
    {
        _messageJson = messageJson;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => OnClick?.Invoke(this));
        DownloadableContent downloadableContent = App.Instance.DownloadableContent;
        downloadableContent.TextureIsReady -= OnTextureLoaded;
        downloadableContent.TextureIsReady += OnTextureLoaded;
        downloadableContent.RequestTexture(messageJson.thumbnail_url);
    }

    private void OnTextureLoaded(string url, Texture2D texture)
    {
        if (Data.thumbnail_url != url) return;
        _thumbnailRawImage.texture = texture;
    }

    public BottledMessageJson Data => _messageJson;
}