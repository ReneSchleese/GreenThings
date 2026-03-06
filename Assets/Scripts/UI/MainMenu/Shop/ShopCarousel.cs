using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

public class ShopCarousel : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private Transform _bottleTarget;
    [SerializeField] private List<CarouselBottle> _visibleBottles;
    [SerializeField] private float _dragSensitivity = 0.01f;
    [SerializeField] private float _damping = 5f;
    [SerializeField] private float _minVelocity = 0.01f;
    
    private float _carouselPosition;
    private float _velocity;
    private bool _isDragging;
    private int _currentBaseIndex;

    public void Init()
    {
        App.Instance.DownloadableContent.TextureIsReady += OnTextureReady;
    }

    private void OnTextureReady(string url, Texture2D tex)
    {
        CarouselBottle bottle = _visibleBottles.FirstOrDefault(bottle => bottle.Url == url);
        bottle?.SetTexture(tex);
    }

    private void Update()
    {
        if (!IsEntered || _velocity == 0f)
        {
            return;
        }
        if (!_isDragging)
        {
            _carouselPosition += _velocity * Time.deltaTime;
            _velocity *= Mathf.Exp(-_damping * Time.deltaTime);
        }
        else
        {
            // make velocity decay fast during drag to prevent carousel being "kicked" with lingering
            // velocity after holding the drag for longer times
            _velocity *= Mathf.Exp(-20f * Time.deltaTime);
        }
        
        if (Mathf.Abs(_velocity) < _minVelocity)
        {
            _velocity = 0f;
        }

        UpdateBottles();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    private void UpdateBottles()
    {
        int visibleBottleCount = _visibleBottles.Count;
        int totalItemCount = App.Instance.Shop.Messages.Count;
        Spline spline = _splineContainer.Spline;
        
        int newBaseIndex = Mathf.FloorToInt(_carouselPosition);

        if (newBaseIndex != _currentBaseIndex)
        {
            int direction = newBaseIndex > _currentBaseIndex ? 1 : -1;

            if (direction > 0)
            {
                // right scroll → leftmost slot gets new data
                int slotIndex = Mod(newBaseIndex, visibleBottleCount);

                int newDataIndex = Mod(newBaseIndex + visibleBottleCount - 1, totalItemCount);
                string url = App.Instance.Shop.Messages[newDataIndex].thumbnail_url;
                _visibleBottles[slotIndex].Url = url;
                App.Instance.DownloadableContent.RequestTexture(url);
            }
            else
            {
                // left scroll → rightmost slot gets new data
                int slotIndex = Mod(newBaseIndex, visibleBottleCount);

                int newDataIndex = Mod(newBaseIndex, totalItemCount);
                string url = App.Instance.Shop.Messages[newDataIndex].thumbnail_url;
                _visibleBottles[slotIndex].Url = url;
                App.Instance.DownloadableContent.RequestTexture(url);
            }

            _currentBaseIndex = newBaseIndex;
        }

        
        for (var bottleIndex = 0; bottleIndex < visibleBottleCount; bottleIndex++)
        {
            float splineT = Mathf.Repeat((_carouselPosition + bottleIndex) / visibleBottleCount, 1f);
            Vector3 pos = spline.EvaluatePosition(splineT);
            float3 tangent = spline.EvaluateTangent(splineT);
            CarouselBottle bottle = _visibleBottles[bottleIndex];
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
        _velocity = Mathf.Lerp(_velocity, movement / Time.deltaTime, 0.5f);
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

    public bool IsEntered;
}