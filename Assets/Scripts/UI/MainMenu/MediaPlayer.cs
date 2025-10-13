using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MediaPlayer : MonoBehaviour, IFadeableCanvasGroup
{
    [SerializeField] private Button _backButton;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RawImage _videoDisplay;
    [SerializeField] private VideoPlayer _videoPlayer;

    public event Action BackButtonPress;
    
    private RenderTexture _videoRenderTexture;

    public void OnLoad()
    {
        _backButton.onClick.AddListener(() => BackButtonPress?.Invoke());
        _videoPlayer.prepareCompleted += videoPlayer =>
        {
            var width = (int)videoPlayer.width;
            var height = (int)videoPlayer.height;

            _videoRenderTexture = RenderTexture.GetTemporary(width, height, 0);
            videoPlayer.targetTexture = _videoRenderTexture;
            _videoDisplay.texture = _videoRenderTexture;
            Debug.Log("width: " + width + " height: " + height);
        };
    }
    
    void OnDestroy()
    {
        Cleanup();
    }

    void Cleanup()
    {
        if (_videoPlayer != null)
        {
            _videoPlayer.Stop();
            _videoPlayer.targetTexture = null;
        }

        if (_videoRenderTexture != null)
        {
            RenderTexture.ReleaseTemporary(_videoRenderTexture);
            _videoRenderTexture = null;
        }
    }

    public IEnumerator Play(string filePath)
    {
        // Prepare VideoPlayer
        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.url = filePath;

        // Prepare before play (async)
        _videoPlayer.Prepare();

        while (!_videoPlayer.isPrepared)
            yield return null;

        Debug.Log("Video prepared, playing...");
        _videoPlayer.Play();
    }
    
    public CanvasGroup CanvasGroup => _canvasGroup;
}