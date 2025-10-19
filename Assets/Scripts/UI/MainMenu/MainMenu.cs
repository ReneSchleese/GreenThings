using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour, IAppState
{
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private ShopView _shopView;
    [SerializeField] private InventoryView _inventoryView;
    [SerializeField] private MediaPlayer _mediaPlayer;

    private string _requestedVideoUrl;
    
    private void Awake()
    {
        App.Instance.NotifyAwakeAppState(this);
    }

    public IEnumerator OnLoad()
    {
        _mainMenuView.OnLoad();
        _shopView.OnLoad();
        _inventoryView.OnLoad();
        _mediaPlayer.OnLoad();
        _mainMenuView.ShopButtonPress += SwitchToShopView;
        _mainMenuView.InventoryButtonPress += SwitchToInventoryView;
        _shopView.BackButtonPress += SwitchToMainMenuView;
        _inventoryView.BackButtonPress += SwitchToMainMenuView;
        _inventoryView.ItemClick += ShowItemInMediaPlayer;
        _mediaPlayer.BackButtonPress += CloseMediaPlayer;
        yield return new WaitUntil(() => BuildConfigLoader.IsLoaded);
    }

    public void OnUnload()
    {
        _mainMenuView.OnUnload();
        _shopView.OnUnload();
        _inventoryView.OnUnload();
        _mainMenuView.ShopButtonPress -= SwitchToShopView;
        _mainMenuView.InventoryButtonPress -= SwitchToInventoryView;
        _shopView.BackButtonPress -= SwitchToMainMenuView;
        _inventoryView.BackButtonPress -= SwitchToMainMenuView;
        _inventoryView.ItemClick -= ShowItemInMediaPlayer;
        _mediaPlayer.BackButtonPress -= CloseMediaPlayer;
    }

    private void SwitchToMainMenuView()
    {
        ((IFadeableCanvasGroup)_mainMenuView).Fade(fadeIn: true);
        ((IFadeableCanvasGroup)_shopView).Fade(fadeIn: false);
        ((IFadeableCanvasGroup)_inventoryView).Fade(fadeIn: false);
    }

    private void SwitchToShopView()
    {
        ((IFadeableCanvasGroup)_mainMenuView).Fade(fadeIn: false);
        ((IFadeableCanvasGroup)_shopView).Fade(fadeIn: true);
        ((IFadeableCanvasGroup)_inventoryView).Fade(fadeIn: false);
    }

    private void SwitchToInventoryView()
    {
        ((IFadeableCanvasGroup)_mainMenuView).Fade(fadeIn: false);
        ((IFadeableCanvasGroup)_shopView).Fade(fadeIn: false);
        ((IFadeableCanvasGroup)_inventoryView).Fade(fadeIn: true);
    }

    public IEnumerator TransitionIn()
    {
        Debug.Log($"{nameof(MainMenu)}.{nameof(TransitionIn)}");
        ((IFadeableCanvasGroup)_mainMenuView).FadeInstantly(fadeIn: true);
        ((IFadeableCanvasGroup)_shopView).FadeInstantly(fadeIn: false);
        ((IFadeableCanvasGroup)_inventoryView).FadeInstantly(fadeIn: false);
        ((IFadeableCanvasGroup)_mediaPlayer).FadeInstantly(fadeIn: false);
        _inventoryView.OnTransitionIn();
        yield break;
    }

    public IEnumerator TransitionOut()
    {
        Debug.Log($"{nameof(MainMenu)}.{nameof(TransitionOut)}");
        yield break;
    }

    private void ShowItemInMediaPlayer(InventoryBottleItemView item)
    {
        ((IFadeableCanvasGroup)_mediaPlayer).Fade(fadeIn: true);
        App.Instance.DownloadableContent.VideoIsReady -= OnVideoIsReady;
        App.Instance.DownloadableContent.VideoIsReady += OnVideoIsReady;
        _requestedVideoUrl = item.Data.content_url;
        App.Instance.DownloadableContent.RequestVideo(_requestedVideoUrl);
    }

    private void OnVideoIsReady(string url)
    {
        if (url != _requestedVideoUrl) return;
        if (_mediaPlayer.IsPlaying) return;
        string localFilePath = DownloadableContent.ToLocalFilePath(url);
        StartCoroutine(_mediaPlayer.Play(localFilePath));
    }

    private void CloseMediaPlayer()
    {
        App.Instance.DownloadableContent.VideoIsReady -= OnVideoIsReady;
        _requestedVideoUrl = null;
        ((IFadeableCanvasGroup)_mediaPlayer).Fade(fadeIn: false);
    }

    public AppState Id => AppState.MainMenu;
}
