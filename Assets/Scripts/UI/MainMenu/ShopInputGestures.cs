using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

public class ShopInputGestures : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private Transform _bottleTarget;
    private float _normalizedPosition;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        _normalizedPosition = Mathf.Clamp01(_normalizedPosition + eventData.delta.x * 0.001f);
        
        var spline = _splineContainer.Spline;
        float3 position = spline.EvaluatePosition(_normalizedPosition);
        float3 tangent = spline.EvaluateTangent(_normalizedPosition);

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