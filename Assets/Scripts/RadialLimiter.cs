using UnityEngine;

public class RadialLimiter : MonoBehaviour
{
    [SerializeField] private RectTransform _stick, _root;
    private const float MAX_DISTANCE_IN_PX = 100;

    private void LateUpdate()
    {
        Vector2 direction = _stick.anchoredPosition - _root.anchoredPosition;
        float distance = direction.magnitude;
        if (distance > MAX_DISTANCE_IN_PX)
        {
            _stick.anchoredPosition = direction.normalized * MAX_DISTANCE_IN_PX;
        }
    }
}
