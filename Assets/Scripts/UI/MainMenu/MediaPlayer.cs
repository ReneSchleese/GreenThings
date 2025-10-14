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
    [SerializeField] private AspectRatioFitter _aspectRatioFitter;
    [SerializeField] private Button _playButton;

    public event Action BackButtonPress;
    
    private RenderTexture _videoRenderTexture;

    public void OnLoad()
    {
        _backButton.onClick.AddListener(() => BackButtonPress?.Invoke());
        _playButton.onClick.AddListener(OnPlayButtonPress);
        _videoPlayer.prepareCompleted += videoPlayer =>
        {
            var width = (int)videoPlayer.width;
            var height = (int)videoPlayer.height;

            _videoRenderTexture = RenderTexture.GetTemporary(width, height, 0);
            videoPlayer.targetTexture = _videoRenderTexture;
            _videoDisplay.texture = _videoRenderTexture;
            _aspectRatioFitter.aspectRatio = (float)width / height;
        };
    }

    private void OnPlayButtonPress()
    {
        if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Pause();
        }
        else
        {
            _videoPlayer.Play();
        }
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