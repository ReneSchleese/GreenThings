using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour, IFadeableCanvasGroup
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _backButton;
    [SerializeField] private Transform _shopItemsContainer;
    [SerializeField] private ShopItemView _shopItemPrefab;
    
    private List<ShopItemView> _shopItems = new();

    public event Action BackButtonPress;

    public void OnLoad()
    {
        _backButton.onClick.AddListener(() => BackButtonPress?.Invoke());
        App.Instance.Shop.Update += OnShopUpdated;
        OnShopUpdated();
    }

    public void OnUnload()
    {
        App.Instance.Shop.Update -= OnShopUpdated;
    }

    private void OnShopUpdated()
    {
        foreach (ShopItemView shopItem in _shopItems)
        {
            Destroy(shopItem.gameObject);
        }
        _shopItems.Clear();
        foreach (BottledMessageJson bottledMessageJson in App.Instance.Shop.Messages)
        {
            ShopItemView shopItemView = Instantiate(_shopItemPrefab,  _shopItemsContainer);
            shopItemView.Set(bottledMessageJson);
            _shopItems.Add(shopItemView);
        }
    }

    public CanvasGroup CanvasGroup => _canvasGroup;
}