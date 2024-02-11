using UnityEngine;

[ExecuteInEditMode]
public class TwoPointProjection : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    public float left = -0.2F;
    public float right = 0.2F;
    public float top = 0.2F;
    public float bottom = -0.2F;
    void LateUpdate()
    {
        // First calculate the regular worldToCameraMatrix.
        // Start with transform.worldToLocalMatrix.
        var m = _camera.transform.worldToLocalMatrix;
        // Then, since Unity uses OpenGL's view matrix conventions
        // we have to flip the z-value.
        m.SetRow(2, -m.GetRow(2));

        // Now for the custom projection.
        // Set the world's up vector to always align with the camera's up vector.
        // Add a small amount of the original up vector to
        // ensure the matrix will be invertible.
        // Try changing the vector to see what other projections you can get.
        m.SetColumn(0, 1e-3f*m.GetColumn(2) - new Vector4(0, 1, 0, 0));

        _camera.ResetWorldToCameraMatrix();
    }

    static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        float x = 2.0F * near / (right - left);
        float y = 2.0F * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0F * far * near) / (far - near);
        float e = -1.0F;
        Matrix4x4 m = new();
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;
        return m;
    }
}