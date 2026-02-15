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

        int visibleItemCount = _visibleShopItems.Length;
        float normalizedSpacing = 1f / visibleItemCount;
        const float totalItemCount = 10;
        Spline spline = _splineContainer.Spline;
        for (var i = 0; i < visibleItemCount; i++)
        {
            float itemLogicalIndex = _carouselPosition + i;
            float wrappedIndex = Mathf.Repeat(itemLogicalIndex, totalItemCount);
            float splineT = Mathf.Repeat(itemLogicalIndex / visibleItemCount, 1f);
            Vector3 pos = spline.EvaluatePosition(splineT);
            float3 tangent = spline.EvaluateTangent(splineT);
            _visibleShopItems[i].transform.localPosition = pos;
            _visibleShopItems[i].transform.localRotation = Quaternion.LookRotation(tangent);
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