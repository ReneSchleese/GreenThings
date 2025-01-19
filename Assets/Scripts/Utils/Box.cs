using UnityEngine;

public struct Box
{
    private readonly Vector3[] _vertices;

    public Box(Vector3[] vertices)
    {
        _vertices = vertices;
    }

    public static Box FromCollider(BoxCollider collider)
    {
        Vector3 extends = 0.5f * collider.size;
        Vector3 lossyScale = collider.transform.lossyScale;
        Vector3 scaledExtends = new(
            lossyScale.x * extends.x,
            lossyScale.y * extends.y,
            lossyScale.z * extends.z);
        Vector3 worldCenter = collider.transform.TransformPoint(collider.center);
        Vector3[] vertices = {
            worldCenter - Vector3.up * scaledExtends.y - Vector3.right * scaledExtends.x - Vector3.forward * scaledExtends.z,
            worldCenter - Vector3.up * scaledExtends.y - Vector3.right * scaledExtends.x + Vector3.forward * scaledExtends.z,
            worldCenter - Vector3.up * scaledExtends.y + Vector3.right * scaledExtends.x + Vector3.forward * scaledExtends.z,
            worldCenter - Vector3.up * scaledExtends.y + Vector3.right * scaledExtends.x - Vector3.forward * scaledExtends.z,
            worldCenter + Vector3.up * scaledExtends.y - Vector3.right * scaledExtends.x - Vector3.forward * scaledExtends.z,
            worldCenter + Vector3.up * scaledExtends.y - Vector3.right * scaledExtends.x + Vector3.forward * scaledExtends.z,
            worldCenter + Vector3.up * scaledExtends.y + Vector3.right * scaledExtends.x + Vector3.forward * scaledExtends.z,
            worldCenter + Vector3.up * scaledExtends.y + Vector3.right * scaledExtends.x - Vector3.forward * scaledExtends.z,
        };
        return new Box(vertices);
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
}