using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform _stick, _root;
    private const float MAX_RADIUS_IN_PX = 120f;
    private const float DEADZONE_RADIUS_IN_PX = 25f;
    public event Action<Vector2> Move;
    private Tween _resetTween;
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnCustomBeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnCustomDrag(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnCustomEndDrag();
    }

    public void OnCustomBeginDrag()
    {
        TryKillTween();
        _isDragging = true;
    }

    public void OnCustomDrag(Vector2 delta)
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

    public void OnCustomEndDrag()
    {
        Move?.Invoke(Vector2.zero);
        _isDragging = false;
        _resetTween = _stick.DOAnchorPos(Vector2.zero, 0.1f).SetEase(Ease.InOutSine);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TryKillTween();
    }
    
    private void TryKillTween()
    {
        if (_resetTween is { active: true })
        {
            _resetTween.Kill();
        }
    }
    
    private Vector2 Direction => _stick.anchoredPosition - _root.anchoredPosition;
}
