using UnityEngine;
using UnityEngine.UI;

public class RadialLimiter : MonoBehaviour
{
    [SerializeField] private RectTransform _stick, _root;
    [SerializeField] private ScrollRect _scrollRect;
    private const float MAX_DISTANCE_IN_PX = 100;

    private void Awake()
    {
        _scrollRect.onValueChanged.AddListener(OnJoyStickMoved);
    }

    private void OnJoyStickMoved(Vector2 delta)
    {
        Vector2 direction = _stick.anchoredPosition - _root.anchoredPosition;
        float distance = direction.magnitude;
        if (distance > MAX_DISTANCE_IN_PX)
        {
            _stick.anchoredPosition = direction.normalized * MAX_DISTANCE_IN_PX;
        }
    }
}
