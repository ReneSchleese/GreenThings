using System;
using UnityEngine;

public class VirtualJoystick : MonoBehaviour
{
    [SerializeField] private RectTransform _stick, _root;
    private const float MAX_RADIUS_IN_PX = 80f;
    private const float DEADZONE_RADIUS_IN_PX = 25f;
    public event Action<Vector2> Move;
    private bool _isDragging;

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
        _isDragging = false;
        Clear();
    }

    private void Clear()
    {
        _stick.anchoredPosition = Vector2.zero;
    }
    
    private Vector2 Direction => _stick.anchoredPosition - _root.anchoredPosition;
}
