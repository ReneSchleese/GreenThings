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
            shopItem.WasBought -= OnItemBuyButtonPressed;
            Destroy(shopItem.gameObject);
        }
        _shopItems.Clear();
        foreach (BottledMessageJson bottledMessageJson in App.Instance.Shop.Messages)
        {
            ShopItemView shopItemView = Instantiate(_shopItemPrefab,  _shopItemsContainer);
            bool alreadyBought = App.Instance.UserData.OwnedMessageIds.Contains(bottledMessageJson.id);
            shopItemView.Set(bottledMessageJson, alreadyBought);
            shopItemView.WasBought += OnItemBuyButtonPressed;
            _shopItems.Add(shopItemView);
        }
    }

    private void OnItemBuyButtonPressed(ShopItemView item)
    {
        UserData userData = App.Instance.UserData;
        bool success = userData.Money >= item.Data.price;
        if(success)
        {
            userData.OwnedMessageIds.Add(item.Data.id);
            userData.Money = Mathf.Max(0,  userData.Money - item.Data.price);
            userData.Save();
            item.Set(item.Data, alreadyBought: true);
        }
    }

    public CanvasGroup CanvasGroup => _canvasGroup;
}