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

    public event Action<string> WasBought;

    public void Set(BottledMessageJson bottledMessageJson)
    {
        _titleTmPro.text = bottledMessageJson.title;
        _typeTmPro.text = bottledMessageJson.type;
        _descriptionTmPro.text = bottledMessageJson.description;
        _urlTmPro.text = bottledMessageJson.url;
        _buyTmPro.text = $"Buy ({bottledMessageJson.price})";
        _buyButton.onClick.AddListener(() =>
        {
            WasBought?.Invoke(bottledMessageJson.id);
        });
    }
}