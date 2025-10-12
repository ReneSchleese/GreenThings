using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour, IFadeableCanvasGroup
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _backButton;
    [SerializeField] private InventoryBottleItemView _bottleItemViewPrefab;
    [SerializeField] private Transform _bottleItemsContainer;
    
    private readonly List<InventoryBottleItemView> _bottleItemViews = new();
    
    public event Action BackButtonPress;

    public void OnLoad()
    {
        _backButton.onClick.AddListener(() => BackButtonPress?.Invoke());
        App.Instance.UserData.Update += UpdateItems;
        App.Instance.Shop.Update += UpdateItems;
        UpdateItems();
    }

    public void OnUnload()
    {
        App.Instance.UserData.Update -= UpdateItems;
        App.Instance.Shop.Update -= UpdateItems;
    }

    private void UpdateItems()
    {
        foreach (InventoryBottleItemView bottleItemView in _bottleItemViews)
        {
            Destroy(bottleItemView.gameObject);
        }
        _bottleItemViews.Clear();
        foreach (string messageId in App.Instance.UserData.OwnedMessageIds)
        {
            BottledMessageJson messageJson = App.Instance.Shop.Messages.First(message => message.id == messageId);
            InventoryBottleItemView bottleItemView = Instantiate(_bottleItemViewPrefab,  _bottleItemsContainer);
            bottleItemView.Set(messageJson);
            _bottleItemViews.Add(bottleItemView);
        }
    }

    public CanvasGroup CanvasGroup => _canvasGroup;
}