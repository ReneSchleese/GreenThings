using System;
using UnityEngine;

public class VirtualJoystick : MonoBehaviour
{
    [SerializeField] private RectTransform _stick, _root;
    [SerializeField] private CanvasGroup _joystickGroup;
    [SerializeField] private bool _showGraphics;
    
    public event Action StickInputBegin;
    public event Action StickInputEnd;
    public event Action<Vector2> StickInput;
    private bool _isDragging;

    public void Clear()
    {
        _stick.anchoredPosition = Vector2.zero;
        _isDragging = false;
        UpdateAppearance();
    }

    private void Update()
    {
        if (!_isDragging)
        {
            return;
        }

        float distance = Direction.magnitude;
        if (distance <= DeadZoneRadiusInPx)
        {
            return;
        }

        RelativeDistanceToRoot = distance / RadiusInPx;
        Vector2 moveAmount = RelativeDistanceToRoot * Direction.normalized;
        moveAmount = new Vector2(ClampMinusOneToOne(moveAmount.x), ClampMinusOneToOne(moveAmount.y));
        StickInput?.Invoke(moveAmount);
        return;

        float ClampMinusOneToOne(float value)
        {
            return value switch
            {
                < -1 => -1,
                > 1 => 1,
                _ => value
            };
        }
    }

    public void OnBeginDrag()
    {
        Clear();
        _isDragging = true;
        UpdateAppearance();
        StickInputBegin?.Invoke();
    }

    public void OnDrag(Vector2 delta)
    {
        _stick.anchoredPosition += delta * DragAcceleration;
        float distance = Direction.magnitude;
        if (distance <= DeadZoneRadiusInPx)
        {
            return;
        }
        if (distance > RadiusInPx)
        {
            _stick.anchoredPosition = Direction.normalized * RadiusInPx;
        }
    }

    public void OnEndDrag()
    {
        StickInputEnd?.Invoke();
        Clear();
    }

    private void UpdateAppearance()
    {
        if (_showGraphics)
        {
            _joystickGroup.alpha = _isDragging ? 1f : 0.4f;   
        }
        else
        {
            _joystickGroup.alpha = 0f;
        }
    }

    private Vector2 Direction => _stick.anchoredPosition - _root.anchoredPosition;
    public Vector3 JoystickPosition => _stick.transform.position;
    public float RelativeDistanceToRoot { get; private set; }
    public float RadiusInPx { get; set; } = 80f;
    public float DeadZoneRadiusInPx { get; set; } = 25f;
    public float DragAcceleration { get; set; } = 1f;
}
