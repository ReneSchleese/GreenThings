using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTmPro;
    [SerializeField] private TextMeshProUGUI _typeTmPro;
    [SerializeField] private TextMeshProUGUI _descriptionTmPro;
    [SerializeField] private TextMeshProUGUI _urlTmPro;
    [SerializeField] private Button _buyButton;
    [SerializeField] private TextMeshProUGUI _buyTmPro;
    [SerializeField] private Image _background;

    public event Action<ShopItemView> WasBought;

    public void Set(BottledMessageJson bottledMessageJson, bool alreadyBought)
    {
        Data = bottledMessageJson;
        
        _titleTmPro.text = bottledMessageJson.title;
        _urlTmPro.text = bottledMessageJson.content_url;
        _background.color = new Color(1, 1, 1, alreadyBought ? 0.4f : 0.8f);
        
        bool showBuyButton = !alreadyBought;
        _buyButton.gameObject.SetActive(showBuyButton);
        if (showBuyButton)
        {
            _buyTmPro.text = $"Buy ({bottledMessageJson.price})";
            _buyButton.onClick.AddListener(() =>
            {
                WasBought?.Invoke(this);
            });
        }
    }
    
    public BottledMessageJson Data { get; private set; }
}