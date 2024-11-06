﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystickRegion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private VirtualJoystick _virtualJoystick;
    [SerializeField] private Transform _virtualJoystickRoot;

    private Vector3 _initialRootPos; 

    public void Awake()
    {
        _initialRootPos = _virtualJoystickRoot.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PointerDown");
        _virtualJoystickRoot.position = eventData.position;
        _virtualJoystick.OnPointerDown(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _virtualJoystick.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _virtualJoystick.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _virtualJoystick.OnEndDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp?");
        _virtualJoystickRoot.position = _initialRootPos;
    }

    public VirtualJoystick VirtualJoystick => _virtualJoystick;
}