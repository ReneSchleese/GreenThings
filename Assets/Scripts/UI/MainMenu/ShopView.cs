using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _backButton;
    [SerializeField] private Transform _shopItemsContainer;
    [SerializeField] private ShopItemView _shopItemPrefab;
    [SerializeField] private TextMeshProUGUI _moneyTmPro;
    
    private readonly List<ShopItemView> _shopItems = new();

    public event Action BackButtonPress;

    public void OnLoad()
    {
        RootGroup = new  FadeableCanvasGroup(_canvasGroup, 0.5f);
        _backButton.onClick.AddListener(() => BackButtonPress?.Invoke());
        App.Instance.Shop.Update += OnShopUpdated;
        App.Instance.UserData.Update += OnUserDataUpdated;
        OnShopUpdated();
        OnUserDataUpdated();
    }

    public void OnUnload()
    {
        App.Instance.Shop.Update -= OnShopUpdated;
        App.Instance.UserData.Update -= OnUserDataUpdated;
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

    private void OnUserDataUpdated()
    {
        UserData userData = App.Instance.UserData;
        _moneyTmPro.text = $"Money: {userData.Money}";
        foreach (ShopItemView shopItem in _shopItems)
        {
            shopItem.Set(shopItem.Data, userData.OwnedMessageIds.Contains(shopItem.Data.id));
        }
    }

    private void OnItemBuyButtonPressed(ShopItemView item)
    {
        App.Instance.UserData.Buy(item.Data);
    }

    public FadeableCanvasGroup RootGroup { get; private set; }
}