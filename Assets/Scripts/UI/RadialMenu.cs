using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{
    private const string INTERACT_TEXT = "Interact";
    [SerializeField] private RadialMenuItem _itemPrefab;
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private RectTransform _radiusHandle;
    [SerializeField] private CanvasGroup _itemsGroup, _selectedItemGroup;
    [SerializeField] private Image _cursorImage;
    [SerializeField] private RadialMenuCursor _cursor;

    public event Action BeingUsedChanged;

    private readonly List<RadialMenuItem> _items = new();
    private VirtualJoystick _virtualJoystick;
    private int _selectedIndex = -1;
    private FadeableCanvasGroup _fadeableItemsGroup;
    private FadeableCanvasGroup _fadeableCursorGroup;
    private FadeableCanvasGroup _fadeableSelectedItemGroup;

    public void Init(VirtualJoystickRegion virtualJoystickRegion)
    {
        _fadeableItemsGroup = new FadeableCanvasGroup(_itemsGroup, 0.5f);
        _fadeableCursorGroup = new FadeableCanvasGroup(_cursor.RootGroup, 0.3f);
        _fadeableSelectedItemGroup = new FadeableCanvasGroup(_selectedItemGroup, 0.3f);
        
        _virtualJoystick = virtualJoystickRegion.VirtualJoystick;
        virtualJoystickRegion.TeleportStickToPointerDownPos = false;
        virtualJoystickRegion.OverwriteInitialStickPosition(transform.position);

        _virtualJoystick.DeadZoneRadiusInPx = 0f;
        _virtualJoystick.RadiusInPx = 160f;
        _virtualJoystick.DragAcceleration = 2f;
        _virtualJoystick.StickInput += OnInput;
        _virtualJoystick.StickInputBegin += () =>
        {
            Sequence?.Kill();
            foreach (RadialMenuItem item in _items)
            {
                ChangeItemHighlight(item, highlighted: false);
            }
            _cursor.SetStyle(setShellCursorActive: true, animate: false);
            
            Sequence = DOTween.Sequence().SetId($"{this}.FadeIn");
            Sequence.Insert(0f, _fadeableCursorGroup.Fade(fadeIn: true));
            Sequence.InsertCallback(0f, () => { BeingUsedChanged?.Invoke(); });
            Sequence.Insert(0f, _fadeableSelectedItemGroup.Fade(fadeIn: true));
            Sequence.Insert(0.25f, _fadeableItemsGroup.Fade(fadeIn: true, 1f));
        };
        _virtualJoystick.StickInputEnd += () => 
        {
            Sequence?.Kill();
            Sequence = DOTween.Sequence().SetId($"{this}.FadeOut");
            Sequence.Insert(0f, _fadeableItemsGroup.Fade(fadeIn: false));
            Sequence.Insert(0f, _fadeableCursorGroup.Fade(fadeIn: false));
            Sequence.InsertCallback(0f, () => { BeingUsedChanged?.Invoke(); });
            Sequence.Insert(0f, _fadeableSelectedItemGroup.Fade(fadeIn: false, 0.66f));
            Sequence.OnComplete(() => { BeingUsedChanged?.Invoke(); });
            if (_selectedIndex != -1)
            {
                _items[_selectedIndex].InputAction.Invoke();
            }
            _selectedIndex = -1;
        };
        
        _cursor.SetStyle(setShellCursorActive: true, animate: false);
        
        InputManager inputManager = App.Instance.InputManager;
        CreateItem("Scan", () => inputManager.InvokeScan());
        CreateItem("Stay", () => inputManager.InvokeScan());
        CreateItem("Toggle\nFollow", () => inputManager.InvokeToggleFormation());
        CreateItem("Dig", () => inputManager.InvokeDig());
        CreateItem("Battlecry", () => inputManager.InvokeBattleCry());
        CreateItem(INTERACT_TEXT, () => inputManager.InvokeInteract());
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
            item.Layout(pos);
            item.SetHighlighted(highlighted: false, animate: false);
        }
    }

    public void FadeInstantly(bool fadeIn)
    {
        DOTween.Kill(this);
        _fadeableItemsGroup.FadeInstantly(fadeIn);
        _fadeableCursorGroup.FadeInstantly(fadeIn);
        _fadeableSelectedItemGroup.FadeInstantly(fadeIn);
    }

    private void OnInput(Vector2 input)
    {
        bool didNotMoveFarEnough = _virtualJoystick.RelativeDistanceToRoot < 0.6f;
        _cursor.RectTransform.position = _virtualJoystick.JoystickPosition;
        _cursor.LeafCursorTransform.rotation = Quaternion.Euler(0f, 0f, InputDirectionToAngle(input));
        
        if (didNotMoveFarEnough)
        {
            if(_selectedIndex != -1)
            {
                ChangeItemHighlight(_items[_selectedIndex], highlighted: false);
                _selectedIndex = -1;
                UpdateCursorAppearance();
            }
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
            ChangeItemHighlight(_items[_selectedIndex], highlighted: false);
        }
        
        _selectedIndex = itemIndex;
        ChangeItemHighlight(_items[_selectedIndex], highlighted: true);
        UpdateCursorAppearance();
    }

    private void ChangeItemHighlight(RadialMenuItem item, bool highlighted)
    {
        Transform parent = highlighted ? _selectedItemGroup.transform : _itemsGroup.transform;
        item.SetHighlighted(highlighted, animate: true);
        item.transform.SetParent(parent);
    }

    private void UpdateCursorAppearance()
    {
        _cursor.SetStyle(setShellCursorActive: _selectedIndex == -1, animate: true);
        //_cursorImage.color = new Color(1, 1, 1, _selectedIndex != -1 ? 1f : 0.4f);
    }

    private static float InputDirectionToAngle(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            return 0f;
        }

        // use -direction.y to get clockwise direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // add offset so we start at the top
        angle -= 90;
        return angle;
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

    public void UpdateWithInteraction(PlayerInteractionState interactionState)
    {
        RadialMenuItem interactionItem = _items[^1];
        if (interactionState.InteractionVolume is null)
        {
            interactionItem.SetText($"<font-weight=600>{INTERACT_TEXT}");
        }
        else
        {
            interactionItem.SetText($"<font-weight=600><size=60%>{INTERACT_TEXT}\n<size=100%>{interactionState.InteractionVolume.GetInteractionDisplayText()}");
        }
    }
    
    private Sequence Sequence {  get; set; }

    public bool IsBeingUsed => _cursor.RootGroup.alpha > 0f;
    public bool IsFadingOut => Sequence is { active: true } && Sequence.stringId.Equals($"{this}.FadeOut");
    public bool IsFadingIn => Sequence is { active: true } && Sequence.stringId.Equals($"{this}.FadeIn");
}