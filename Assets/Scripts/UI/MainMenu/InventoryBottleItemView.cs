using UnityEngine;
using UnityEngine.UI;

public class InventoryBottleItemView : MonoBehaviour
{
    [SerializeField] private Image _thumbnailImage;
    private BottledMessageJson _messageJson;

    public void Set(BottledMessageJson messageJson)
    {
        _messageJson = messageJson;
    }
}