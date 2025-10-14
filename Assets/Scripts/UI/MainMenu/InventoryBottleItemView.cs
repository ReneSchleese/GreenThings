using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
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
        StartCoroutine(DownloadAndSetRawImage(messageJson.thumbnail_url));
        
        
        // here we basically want: "give me the image. if it isn't on disk, download it, save it and notify me when you're done"
        
        
        
        IEnumerator DownloadAndSetRawImage(string imageUrl)
        {
            using var request = UnityWebRequestTexture.GetTexture(imageUrl);
            request.SetRequestHeader("x-api-key", BuildConfigLoader.Config.ApiKey);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download image: {request.error}");
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                _thumbnailRawImage.texture = texture;
            }
        }
    }

    public BottledMessageJson Data => _messageJson;
}