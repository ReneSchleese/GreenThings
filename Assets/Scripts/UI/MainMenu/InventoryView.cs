using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _backButton, _messagesTabButton, _vinylsTabButton;
    [SerializeField] private InventoryBottleItemView _bottleItemViewPrefab;
    [SerializeField] private InventoryVinylItemView _vinylItemViewPrefab;
    [SerializeField] private Transform _bottleItemsContainer, _vinylsItemsContainer;
    
    public event Action BackButtonPress;
    public event Action<InventoryBottleItemView> BottleItemClick;
    
    private enum Tab
    {
        Messages,
        Vinyls
    }
    
    private readonly List<InventoryBottleItemView> _bottleItemViews = new();
    private readonly List<InventoryVinylItemView> _vinylItemViews = new();
    private Tab _activeTab;

    public void OnLoad()
    {
        RootGroup = new FadeableCanvasGroup(_canvasGroup, 0.5f);
        _backButton.onClick.AddListener(() => BackButtonPress?.Invoke());
        _messagesTabButton.onClick.AddListener(() => SetActiveTab(Tab.Messages));
        _vinylsTabButton.onClick.AddListener(() => SetActiveTab(Tab.Vinyls));
        
        _activeTab = Tab.Messages;
        UpdateTabView();
        
        App.Instance.UserData.Update += UpdateItems;
        App.Instance.Shop.Update += UpdateItems;
        return;

        void SetActiveTab(Tab tab)
        {
            if (_activeTab == tab)
            {
                return;
            }
            _activeTab = tab;
            UpdateTabView();
        }
    }

    public void OnUnload()
    {
        App.Instance.UserData.Update -= UpdateItems;
        App.Instance.Shop.Update -= UpdateItems;
    }

    public void OnTransitionIn()
    {
        UpdateItems();
    }

    private void UpdateTabView()
    {
        _bottleItemsContainer.gameObject.SetActive(_activeTab == Tab.Messages);
        _vinylsItemsContainer.gameObject.SetActive(_activeTab == Tab.Vinyls);
    }

    private void UpdateItems()
    {
        foreach (InventoryBottleItemView bottleItemView in _bottleItemViews)
        {
            bottleItemView.OnClick -= OnBottleItemClicked;
            Destroy(bottleItemView.gameObject);
        }
        _bottleItemViews.Clear();
        
        foreach (InventoryVinylItemView vinylItemView in _vinylItemViews)
        {
            Destroy(vinylItemView.gameObject);
        }
        _vinylItemViews.Clear();
        
        foreach (string messageId in App.Instance.UserData.OwnedMessageIds)
        {
            BottledMessageJson messageJson = App.Instance.Shop.Messages.FirstOrDefault(message => message.id == messageId);
            if (messageJson == null)
            {
                continue;
            }
            InventoryBottleItemView bottleItemView = Instantiate(_bottleItemViewPrefab, _bottleItemsContainer);
            bottleItemView.Set(messageJson);
            _bottleItemViews.Add(bottleItemView);
            bottleItemView.OnClick += OnBottleItemClicked;
        }

        foreach (VinylId vinylId in App.Instance.UserData.OwnedVinylIds)
        {
            InventoryVinylItemView vinylItemView = Instantiate(_vinylItemViewPrefab, _vinylsItemsContainer);
            vinylItemView.Init();
            vinylItemView.Set(vinylId);
            _vinylItemViews.Add(vinylItemView);
        }
    }

    private void OnBottleItemClicked(InventoryBottleItemView bottleItemView)
    {
        BottleItemClick?.Invoke(bottleItemView);
    }

    public FadeableCanvasGroup RootGroup { get; private set; }
}