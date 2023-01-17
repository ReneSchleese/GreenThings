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
    private Vector2 _lastDragDelta;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_resetTween is { active: true })
        {
            _resetTween.Kill();
        }

        _isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _lastDragDelta = eventData.delta;
        
        _stick.anchoredPosition += _lastDragDelta;
        Vector2 direction = _stick.anchoredPosition - _root.anchoredPosition;
        float distance = direction.magnitude;
        if (distance <= DEADZONE_RADIUS_IN_PX)
        {
            return;
        }
        Vector2 directionNormalized = direction.normalized;
        if (distance > MAX_RADIUS_IN_PX)
        {
            _stick.anchoredPosition = directionNormalized * MAX_RADIUS_IN_PX;
        }
    }

    private void Update()
    {
        if (_isDragging == false)
        {
            return;
        }
        
        
        Vector2 direction = _stick.anchoredPosition - _root.anchoredPosition;
        float distance = direction.magnitude;
        if (distance <= DEADZONE_RADIUS_IN_PX)
        {
            return;
        }
        Vector2 directionNormalized = direction.normalized;

        float factor = distance / MAX_RADIUS_IN_PX;
        Vector2 delta = factor * directionNormalized;
        delta = new Vector2(Mathf.Min(1f, delta.x), Mathf.Min(1f, delta.y));
        Move?.Invoke(delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
        _resetTween = _stick.DOAnchorPos(Vector2.zero, 0.1f).SetEase(Ease.InOutSine);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_resetTween is { active: true })
        {
            _resetTween.Kill();
        }
    }
}
