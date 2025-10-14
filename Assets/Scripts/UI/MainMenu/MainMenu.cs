using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour, IAppState
{
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private ShopView _shopView;
    [SerializeField] private InventoryView _inventoryView;
    [SerializeField] private MediaPlayer _mediaPlayer;
    
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

    private void ShowItemInMediaPlayer(InventoryBottleItemView obj)
    {
        ((IFadeableCanvasGroup)_mediaPlayer).Fade(fadeIn: true);
        StartCoroutine(DownloadThenPlay());
        
        
        // here we basically want: "give me the video. if you don't have it yet, download it, save it and notify me when you're done

        IEnumerator DownloadThenPlay()
        {
            string filePath = Path.Combine(Application.temporaryCachePath, "video.mp4");
            if (!File.Exists(filePath))
            {
                using var request = UnityWebRequest.Get(obj.Data.content_url);
                request.SetRequestHeader("x-api-key", BuildConfigLoader.Config.ApiKey);
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to download content: {request.error}");
                }
                else
                {
                    File.WriteAllBytes(filePath, request.downloadHandler.data);
                }
            }
            yield return _mediaPlayer.Play(filePath);
        }
    }

    private void CloseMediaPlayer()
    {
        ((IFadeableCanvasGroup)_mediaPlayer).Fade(fadeIn: false);
    }

    public AppState Id => AppState.MainMenu;
}
