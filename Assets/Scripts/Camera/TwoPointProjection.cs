using System;
using UnityEngine;

[ExecuteInEditMode]
public class TwoPointProjection : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] [Range(0, 360)] private float _degrees;

    private void Update()
    {
        
    }
}