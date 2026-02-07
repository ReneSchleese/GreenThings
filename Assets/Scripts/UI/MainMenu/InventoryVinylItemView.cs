using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryVinylItemView : MonoBehaviour
{
    [SerializeField] private RawImage _thumbnailImage;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _tmPro;
    [SerializeField] private Image _newIcon;

    public void Set(VinylId id)
    {
        VinylData vinylData = App.Instance.BuiltInContent.GetVinylData(id);
        _thumbnailImage.texture = vinylData.Thumbnail;
        _tmPro.text = vinylData.Title;
    }
}