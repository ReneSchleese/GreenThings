using UnityEngine;

[ExecuteInEditMode]
public class OrthographicToPerspectiveLerp : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [Range(0, 1)] [SerializeField] private float _lerp;

    private Matrix4x4 _ortho, _perspective;

    public float _fov = 60f,
        _near = .3f,
        _far = 1000f,
        _orthographicSize = 7.5f;

    private float _aspect;

    void Start()
    {
        _aspect = (float)Screen.width / Screen.height;
        _ortho = Matrix4x4.Ortho(-_orthographicSize * _aspect, _orthographicSize * _aspect, -_orthographicSize,
            _orthographicSize, _near, _far);
        _perspective = Matrix4x4.Perspective(_fov, _aspect, _near, _far);
        _camera.projectionMatrix = MatrixBlender.MatrixLerp(_ortho, _perspective, _lerp);
    }
}