using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] private RadialMenuItem _itemPrefab;
    [SerializeField] private Transform _itemContainer;
    
    private List<RadialMenuItem> _items = new();

    public void Init()
    {
        CreateItem("Interact");
        CreateItem("Scan");
        CreateItem("Dig");
        CreateItem("Toggle Mode");
        CreateItem("Scream");
        return;

        void CreateItem(string label)
        {
            RadialMenuItem item = Instantiate(_itemPrefab, _itemContainer);
            item.Init(label);
            _items.Add(item);
        }
    }
}