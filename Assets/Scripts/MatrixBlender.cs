using UnityEngine;

public static class MatrixBlender
{
    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float lerp)
    {
        Matrix4x4 ret = new();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], lerp);
        return ret;
    }
}