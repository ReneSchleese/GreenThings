using System;
using UnityEngine;

public struct Point : IEquatable<Point>
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

    public bool Equals(Point other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && Rotation.Equals(other.Rotation);
    }

    public override bool Equals(object obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, Rotation);
    }
}