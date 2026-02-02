using System.Collections;
using Cinemachine;
using UnityEngine;

public class MainMenu : MonoBehaviour, IAppState
{
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private ShopView _shopView;
    [SerializeField] private InventoryView _inventoryView;
    [SerializeField] private MediaPlayer _mediaPlayer;
    [SerializeField] private CinemachineVirtualCamera _homeCam, _shopCam, _inventoryCam;

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
        _mainMenuView.RootGroup.Fade(fadeIn: true);
        _shopView.RootGroup.Fade(fadeIn: false);
        _inventoryView.RootGroup.Fade(fadeIn: false);
        SetActiveCamera(_homeCam);
    }

    private void SwitchToShopView()
    {
        _mainMenuView.RootGroup.Fade(fadeIn: false);
        _shopView.RootGroup.Fade(fadeIn: true);
        _inventoryView.RootGroup.Fade(fadeIn: false);
        SetActiveCamera(_shopCam);
    }

    private void SwitchToInventoryView()
    {
        _mainMenuView.RootGroup.Fade(fadeIn: false);
        _shopView.RootGroup.Fade(fadeIn: false);
        _inventoryView.RootGroup.Fade(fadeIn: true);
        SetActiveCamera(_inventoryCam);
    }

    private void SetActiveCamera(CinemachineVirtualCamera cam)
    {
        _homeCam.Priority = 0;
        _inventoryCam.Priority = 0;
        _shopCam.Priority = 0;
        cam.Priority = 10;
    }

    public IEnumerator TransitionIn()
    {
        Debug.Log($"{nameof(MainMenu)}.{nameof(TransitionIn)}");
        _mainMenuView.RootGroup.FadeInstantly(fadeIn: true);
        _shopView.RootGroup.FadeInstantly(fadeIn: false);
        _inventoryView.RootGroup.FadeInstantly(fadeIn: false);
        _mediaPlayer.RootGroup.FadeInstantly(fadeIn: false);
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
        _mediaPlayer.RootGroup.Fade(fadeIn: true);
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
        _mediaPlayer.RootGroup.Fade(fadeIn: false);
    }

    public AppState Id => AppState.MainMenu;
}
