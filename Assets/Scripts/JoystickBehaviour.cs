using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickBehaviour : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform _stick, _root;
    private const float MAX_RADIUS_IN_PX = 100f;
    private const float DEADZONE_RADIUS_IN_PX = 5f;
    public event Action<Vector2> Move;

    public void OnDrag(PointerEventData eventData)
    {
        _stick.anchoredPosition += eventData.delta;
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

        float factor = distance / MAX_RADIUS_IN_PX;
        Vector2 delta = factor * directionNormalized;
        Move?.Invoke(delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");
    }
}
