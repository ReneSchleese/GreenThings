using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

public class ShopCarousel : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private Transform _bottleTarget;
    [SerializeField] private GameObject[] _visibleShopItems;
    private float _carouselPosition;
    private const float SCROLL_SPEED = 0.001f;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        _carouselPosition += eventData.delta.x * SCROLL_SPEED;

        int visibleBottleCount = _visibleShopItems.Length;
        const int totalItemCount = 10;
        Spline spline = _splineContainer.Spline;
        int baseIndex = Mathf.FloorToInt(_carouselPosition);
        for (var bottleIndex = 0; bottleIndex < visibleBottleCount; bottleIndex++)
        {
            int dataIndex = Mod(baseIndex + bottleIndex, totalItemCount);
            Debug.Log($"bottleIndex={bottleIndex}, dataIndex={dataIndex}");
            float splineT = Mathf.Repeat((_carouselPosition + bottleIndex) / visibleBottleCount, 1f);
            Vector3 pos = spline.EvaluatePosition(splineT);
            float3 tangent = spline.EvaluateTangent(splineT);
            GameObject bottle = _visibleShopItems[bottleIndex];
            bottle.transform.localPosition = pos;
            bottle.transform.localRotation = Quaternion.LookRotation(tangent);
        }

        return;

        int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
    }
}