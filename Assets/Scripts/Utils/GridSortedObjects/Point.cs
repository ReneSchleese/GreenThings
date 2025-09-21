using UnityEngine;

public struct Point
{
    public float X, Y, Z;
    public Quaternion Rotation;

    public static Point FromTransform(Transform transform) => new Point()
    {
        X = transform.position.x,
        Y = transform.position.y,
        Z = transform.position.z,
        Rotation = transform.rotation
    };
    
    public Vector3 ToVector3() =>  new Vector3(X, Y, Z);
}