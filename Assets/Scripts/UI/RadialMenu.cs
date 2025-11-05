using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] private RadialMenuItem _itemPrefab;

    public void Init()
    {
        RadialMenuItem radialMenuItem = Instantiate(_itemPrefab);
        radialMenuItem.Init("Test");
    }
}