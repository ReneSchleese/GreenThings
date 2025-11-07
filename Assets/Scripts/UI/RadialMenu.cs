using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour, IFadeableCanvasGroup
{
    [SerializeField] private RadialMenuItem _itemPrefab;
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private RectTransform _radiusHandle;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _cursorRectTransform;
    [SerializeField] private Image _cursorImage;

    private readonly List<RadialMenuItem> _items = new();
    private VirtualJoystick _virtualJoystick;
    private int _selectedIndex = -1;

    public void Init(VirtualJoystickRegion virtualJoystickRegion)
    {
        _virtualJoystick = virtualJoystickRegion.VirtualJoystick;
        virtualJoystickRegion.TeleportStickToPointerDownPos = false;
        virtualJoystickRegion.OverwriteInitialStickPosition(transform.position);

        _virtualJoystick.DeadZoneRadiusInPx = 0f;
        _virtualJoystick.RadiusInPx = 160f;
        _virtualJoystick.StickInput += OnInput;
        _virtualJoystick.StickInputBegin += () => { ((IFadeableCanvasGroup)this).Fade(fadeIn: true); };
        _virtualJoystick.StickInputEnd += () => 
        {
            ((IFadeableCanvasGroup)this).Fade(fadeIn: false);
            OnInputEnd();
        };
        
        InputManager inputManager = App.Instance.InputManager;
        CreateItem("Interact", () => inputManager.InvokeInteract());
        CreateItem("Scan", () => inputManager.InvokeScan());
        CreateItem("Dig", () => inputManager.InvokeDig());
        CreateItem("Toggle Mode", () => inputManager.InvokeToggleFormation());
        CreateItem("Battlecry", () => inputManager.InvokeBattleCry());

        LayoutItems();
        return;

        void CreateItem(string label, Action inputAction)
        {
            RadialMenuItem item = Instantiate(_itemPrefab, _itemContainer);
            item.Init(label,  inputAction);
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
        Vector2 screenPosA = RectTransformUtility.WorldToScreenPoint(null, _radiusHandle.position);
        Vector2 screenPosB = RectTransformUtility.WorldToScreenPoint(null, GetComponent<RectTransform>().position);
        float pixelDistance = Vector2.Distance(screenPosA, screenPosB);
        const float dir = clockwise ? -1f : 1f;

        for (int i = 0; i < _items.Count; i++)
        {
            float angle = startAngle + dir * stepAngle * i;
            float rad = angle * Mathf.Deg2Rad;

            RadialMenuItem item = _items[i];
            Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * pixelDistance;
            item.RectTransform.anchoredPosition = pos;
            item.RectTransform.localRotation = Quaternion.identity;
            item.SetHighlighted(highlighted: false, animate: false);
        }
    }

    private void OnInput(Vector2 input)
    {
        _cursorRectTransform.position = _virtualJoystick.JoystickPosition;
        if (_virtualJoystick.RelativeDistanceToRoot < 0.5f)
        {
            if(_selectedIndex != -1)
            {
                _items[_selectedIndex].SetHighlighted(highlighted: false, animate: true);
                _selectedIndex = -1;
            }

            UpdateCursorAppearance();
            return;
        }
        int itemIndex = GetItemIndexFromDirection(input, _items.Count);
        if (itemIndex < 0 || itemIndex >= _items.Count)
        {
            UpdateCursorAppearance();
            return;
        }
        
        bool didChange = _selectedIndex != itemIndex;
        if (!didChange)
        {
            return;
        }
        
        if (_selectedIndex != -1)
        {
            _items[_selectedIndex].SetHighlighted(highlighted: false, animate: true);
        }
        _selectedIndex = itemIndex;
        UpdateCursorAppearance();
        _items[_selectedIndex].SetHighlighted(highlighted: true, animate: true);
    }

    private void OnInputEnd()
    {
        if (_selectedIndex != -1)
        {
            _items[_selectedIndex].InputAction.Invoke();
        }
        _selectedIndex = -1;
        foreach (RadialMenuItem item in _items)
        {
            item.SetHighlighted(highlighted: false, animate: true);
        }
    }

    private void UpdateCursorAppearance()
    {
        _cursorImage.color = new Color(1, 1, 1, _selectedIndex != -1 ? 1f : 0.4f);
    }

    private static int GetItemIndexFromDirection(Vector2 direction, int itemCount)
    {
        if (direction == Vector2.zero)
        {
            return -1;
        }

        // use -direction.y to get clockwise direction
        float angle = Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg;
        
        // add offset so we start at the top
        angle += 90;
        
        // add another offset so that items are centered ((-0.5, 0.5) instead of (0, 1))
        float degreesPerItem = 360f / itemCount;
        angle += degreesPerItem * 0.5f;
        
        // finally, make angle overflow
        if (angle < 0)
        {
            angle += 360f;
        }
        
        int index = Mathf.FloorToInt(angle / degreesPerItem);
        index = Mathf.Clamp(index, 0, itemCount - 1);
        return index;
    }

    public CanvasGroup CanvasGroup => _canvasGroup;
}