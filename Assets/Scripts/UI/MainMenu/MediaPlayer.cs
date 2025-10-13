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

    public void OnLoad()
    {
        _backButton.onClick.AddListener(() => BackButtonPress?.Invoke());
    }

    public IEnumerator Play(string filePath)
    {
        // Prepare VideoPlayer
        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.url = filePath;

        // Optional: Render to RawImage on Canvas
        _videoPlayer.renderMode = VideoRenderMode.APIOnly;
        _videoPlayer.targetTexture = new RenderTexture(1920, 1080, 0);
        _videoDisplay.texture = _videoPlayer.targetTexture;

        // Prepare before play (async)
        _videoPlayer.Prepare();

        while (!_videoPlayer.isPrepared)
            yield return null;

        Debug.Log("Video prepared, playing...");
        _videoPlayer.Play();
    }
    
    public CanvasGroup CanvasGroup => _canvasGroup;
}