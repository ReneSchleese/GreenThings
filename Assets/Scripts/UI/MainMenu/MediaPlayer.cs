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
    }

    private void OnDestroy()
    {
        Clear();
    }

    private void Clear()
    {
        _videoPlayer.Stop();
        _videoPlayer.targetTexture = null;
        _videoPlayer.url = string.Empty;
        if (_videoRenderTexture != null)
        {
            RenderTexture.ReleaseTemporary(_videoRenderTexture);
            _videoRenderTexture = null;
            _videoDisplay.texture = null;
        }
    }

    public IEnumerator Play(string filePath)
    {
        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.url = filePath;
        _videoPlayer.Prepare();
        
        while (!_videoPlayer.isPrepared)
        {
            yield return null;
        }
        
        var width = (int)_videoPlayer.width;
        var height = (int)_videoPlayer.height;
        _videoRenderTexture = RenderTexture.GetTemporary(width, height, 0);
        _videoPlayer.targetTexture = _videoRenderTexture;
        _videoDisplay.texture = _videoRenderTexture;
        _aspectRatioFitter.aspectRatio = (float)width / height;

        _videoPlayer.Play();
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

    public void OnFadeComplete(bool fadeIn)
    {
        if(!fadeIn)
        {
            Clear();
        }
    }

    public CanvasGroup CanvasGroup => _canvasGroup;
}