using UnityEngine;

public struct Box
{
    private readonly Vector3[] _vertices;
    private readonly Vector3[] _verticesWorldBuffer;

    public Box(Vector3[] vertices)
    {
        _vertices = vertices;
        _verticesWorldBuffer = new Vector3[vertices.Length];
    }

    public static Box FromCollider(BoxCollider collider)
    {
        Vector3 extends = 0.5f * collider.size;
        Vector3 center = collider.center;
        Vector3[] vertices = {
            center - Vector3.up * extends.y - Vector3.right * extends.x - Vector3.forward * extends.z,
            center - Vector3.up * extends.y - Vector3.right * extends.x + Vector3.forward * extends.z,
            center - Vector3.up * extends.y + Vector3.right * extends.x + Vector3.forward * extends.z,
            center - Vector3.up * extends.y + Vector3.right * extends.x - Vector3.forward * extends.z,
            center + Vector3.up * extends.y - Vector3.right * extends.x - Vector3.forward * extends.z,
            center + Vector3.up * extends.y - Vector3.right * extends.x + Vector3.forward * extends.z,
            center + Vector3.up * extends.y + Vector3.right * extends.x + Vector3.forward * extends.z,
            center + Vector3.up * extends.y + Vector3.right * extends.x - Vector3.forward * extends.z,
        };
        return new Box(vertices);
    }
    
    public Vector3[] ToWorld(Transform target)
    {
        for (var i = 0; i < _verticesWorldBuffer.Length; i++)
        {
            _verticesWorldBuffer[i] = target.TransformPoint(_vertices[i]);
        }

        return _verticesWorldBuffer;
    }

    public static void Draw(Vector3[] vertices, Color color, float duration)
    {
        Debug.DrawLine(vertices[0], vertices[1], color, duration);
        Debug.DrawLine(vertices[0], vertices[3], color, duration);
        Debug.DrawLine(vertices[1], vertices[2], color, duration);
        Debug.DrawLine(vertices[3], vertices[2], color, duration);
        
        Debug.DrawLine(vertices[4], vertices[5], color, duration);
        Debug.DrawLine(vertices[4], vertices[7], color, duration);
        Debug.DrawLine(vertices[5], vertices[6], color, duration);
        Debug.DrawLine(vertices[7], vertices[6], color, duration);
        
        Debug.DrawLine(vertices[0], vertices[4], color, duration);
        Debug.DrawLine(vertices[1], vertices[5], color, duration);
        Debug.DrawLine(vertices[2], vertices[6], color, duration);
        Debug.DrawLine(vertices[3], vertices[7], color, duration);
    }
}