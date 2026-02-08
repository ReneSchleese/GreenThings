using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryVinylItemView : MonoBehaviour
{
    [SerializeField] private RawImage _thumbnailImage;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _tmPro;
    [SerializeField] private Image _newIcon;
    [SerializeField] private Image _enabledImage;

    private bool _isSet;
    private VinylId _vinylId;
    private bool _vinylIsEnabled;

    public void Init()
    {
        _button.onClick.AddListener(OnClicked);
    }

    public void Set(VinylId id)
    {
        VinylData vinylData = App.Instance.BuiltInContent.GetVinylData(id);
        _vinylId = vinylData.Id;
        _thumbnailImage.texture = vinylData.Thumbnail;
        _tmPro.text = vinylData.Title;
        _vinylIsEnabled = App.Instance.UserData.EnabledVinylIds.Contains(id);
        _enabledImage.enabled = _vinylIsEnabled;
        _isSet = true;
    }

    private void OnClicked()
    {
        Debug.Assert(_isSet, "Item was not set correctly.");
        if (_vinylIsEnabled)
        {
            App.Instance.UserData.EnabledVinylIds.Remove(_vinylId);
        }
        else
        {
            App.Instance.UserData.EnabledVinylIds.Add(_vinylId);
        }
        Set(_vinylId);
    }
}