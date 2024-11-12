using System;
using UnityEngine;

public class VirtualJoystick : MonoBehaviour
{
    [SerializeField] private RectTransform _stick, _root;
    [SerializeField] private CanvasGroup _joystickGroup;
    private const float MAX_RADIUS_IN_PX = 80f;
    private const float DEADZONE_RADIUS_IN_PX = 25f;
    public event Action<Vector2> Move;
    private bool _isDragging;

    public void Clear()
    {
        _stick.anchoredPosition = Vector2.zero;
        _isDragging = false;
        UpdateAppearance();
    }

    private void Update()
    {
        if (_isDragging == false)
        {
            return;
        }

        float distance = Direction.magnitude;
        if (distance <= DEADZONE_RADIUS_IN_PX)
        {
            return;
        }

        float relativeDistance = distance / MAX_RADIUS_IN_PX;
        Vector2 moveAmount = relativeDistance * Direction.normalized;
        moveAmount = new Vector2(ClampMinusOneToOne(moveAmount.x), ClampMinusOneToOne(moveAmount.y));
        Move?.Invoke(moveAmount);

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

    public void SimulateBeginDrag()
    {
        Clear();
        _isDragging = true;
        UpdateAppearance();
    }

    public void SimulateDrag(Vector2 delta)
    {
        _stick.anchoredPosition += delta;
        float distance = Direction.magnitude;
        switch (distance)
        {
            case <= DEADZONE_RADIUS_IN_PX:
                return;
            case > MAX_RADIUS_IN_PX:
                _stick.anchoredPosition = Direction.normalized * MAX_RADIUS_IN_PX;
                break;
        }
    }

    public void SimulateEndDrag()
    {
        Move?.Invoke(Vector2.zero);
        Clear();
    }

    private void UpdateAppearance()
    {
        _joystickGroup.alpha = _isDragging ? 1f : 0.4f;
    }

    private Vector2 Direction => _stick.anchoredPosition - _root.anchoredPosition;
}
