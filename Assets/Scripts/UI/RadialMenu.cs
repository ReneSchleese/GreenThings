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

        LayoutItems();
        return;

        void CreateItem(string label)
        {
            RadialMenuItem item = Instantiate(_itemPrefab, _itemContainer);
            item.Init(label);
            _items.Add(item);
        }
    }

    private void LayoutItems()
    {
        if (_items == null || _items.Count == 0)
            return;
        
        float stepAngle = 360f / _items.Count;
        const bool clockwise = true;
        const float startAngle = 90f;
        const float radius = 150f;
        const float dir = clockwise ? -1f : 1f;

        for (int i = 0; i < _items.Count; i++)
        {
            float angle = startAngle + dir * stepAngle * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
            _items[i].RectTransform.anchoredPosition = pos;
            _items[i].RectTransform.localRotation = Quaternion.identity;
        }
    }
}