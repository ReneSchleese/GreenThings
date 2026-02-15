using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

public class ShopCarousel : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private Transform _bottleTarget;
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
        
        var spline = _splineContainer.Spline;
        float3 position = spline.EvaluatePosition(_carouselPosition);
        float3 tangent = spline.EvaluateTangent(_carouselPosition);

        _bottleTarget.localPosition = position;
        _bottleTarget.localRotation = Quaternion.LookRotation(tangent);
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