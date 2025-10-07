using TMPro;
using UnityEngine;

public class ShopItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTmPro;
    [SerializeField] private TextMeshProUGUI _typeTmPro;
    [SerializeField] private TextMeshProUGUI _descriptionTmPro;
    [SerializeField] private TextMeshProUGUI _urlTmPro;

    public void Set(BottledMessageJson bottledMessageJson)
    {
        _titleTmPro.text = bottledMessageJson.title;
        _typeTmPro.text = bottledMessageJson.type;
        _descriptionTmPro.text = bottledMessageJson.description;
        _urlTmPro.text = bottledMessageJson.url;
    }
}