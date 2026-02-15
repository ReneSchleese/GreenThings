using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

public class ShopCarousel : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private Transform _bottleTarget;
    [SerializeField] private GameObject[] _visibleShopItems;
    [SerializeField] private float _dragSensitivity = 0.01f;
    [SerializeField] private float _damping = 5f;
    [SerializeField] private float _minVelocity = 0.01f;
    private float _carouselPosition;
    private float _velocity;
    private bool _isDragging;

    private void Update()
    {
        if (!_isDragging)
        {
            _carouselPosition += _velocity * Time.deltaTime;
            _velocity *= Mathf.Exp(-_damping * Time.deltaTime);
            if (Mathf.Abs(_velocity) < _minVelocity)
            {
                _velocity = 0f;
            }
        }

        UpdateBottles();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    private void UpdateBottles()
    {
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

    public void OnDrag(PointerEventData eventData)
    {
        float movement = eventData.delta.x * _dragSensitivity;
        _carouselPosition += movement;
        _velocity = movement / Time.deltaTime;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
        _velocity = 0f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
    }
}