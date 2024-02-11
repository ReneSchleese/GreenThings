using UnityEngine;

[ExecuteInEditMode]
public class TwoPointProjection : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] [Range(0, 360)] private float _q = 180;
    [SerializeField] [Range(1, 1000)] private float _d = 300;

    private void OnEnable()
    {
        Matrix4x4 projectionMatrix = new(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(Mathf.Sin(_q) / _d, 0, Mathf.Cos(_q) / _d, 0));
        _camera.projectionMatrix = projectionMatrix;
    }
}