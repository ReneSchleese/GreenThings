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
    
    public Box ToWorld(Transform target)
    {
        for (var i = 0; i < _verticesWorldBuffer.Length; i++)
        {
            _verticesWorldBuffer[i] = target.TransformPoint(_vertices[i]);
        }

        return new Box(_verticesWorldBuffer);
    }

    public void Draw(Color color, float duration)
    {
        Debug.DrawLine(_vertices[0], _vertices[1], color, duration);
        Debug.DrawLine(_vertices[0], _vertices[3], color, duration);
        Debug.DrawLine(_vertices[1], _vertices[2], color, duration);
        Debug.DrawLine(_vertices[3], _vertices[2], color, duration);
        
        Debug.DrawLine(_vertices[4], _vertices[5], color, duration);
        Debug.DrawLine(_vertices[4], _vertices[7], color, duration);
        Debug.DrawLine(_vertices[5], _vertices[6], color, duration);
        Debug.DrawLine(_vertices[7], _vertices[6], color, duration);
        
        Debug.DrawLine(_vertices[0], _vertices[4], color, duration);
        Debug.DrawLine(_vertices[1], _vertices[5], color, duration);
        Debug.DrawLine(_vertices[2], _vertices[6], color, duration);
        Debug.DrawLine(_vertices[3], _vertices[7], color, duration);
    }

    public Vector3 TopCenter => _vertices[4] + 0.5f * (_vertices[6] - _vertices[4]);
    public Vector3 BotCenter => _vertices[0] + 0.5f * (_vertices[2] - _vertices[0]);
    public float Height => Vector3.Distance(TopCenter, BotCenter);
}