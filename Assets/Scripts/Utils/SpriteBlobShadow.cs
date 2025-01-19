using System;
using UnityEngine;

public class SpriteBlobShadow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _shadowRenderer;
    [SerializeField] private BoxCollider _boxCollider;
    private const float PUSH_UP_Y = 0.2f;
    private Bounds _bounds;
    private Box _box;

    private void Awake()
    {
        _bounds = _boxCollider.bounds;
        _box = Box.FromCollider(_boxCollider);
    }

    private void Update()
    {
        UpdateShadow();
        _box.ToWorld(transform).Draw(Color.red, 0.01f);
    }

    private void UpdateShadow()
    {
        Box worldBox = _box.ToWorld(transform);
        Vector3 origin = worldBox.TopCenter;
        float distance = worldBox.Height;
        Physics.Raycast(new Ray(origin, Vector3.down), out RaycastHit hit, distance);
        Debug.DrawRay(origin, Vector3.down * distance, Color.red);
        bool isGrounded = hit.collider != null;
        if (isGrounded)
        {
            _shadowRenderer.transform.rotation = Quaternion.LookRotation(hit.normal);
            Vector3 hitPointInLocal = transform.InverseTransformPoint(hit.point);
            float y = Mathf.Clamp(hitPointInLocal.y + PUSH_UP_Y, _box.BotCenter.y, _box.TopCenter.y);
            _shadowRenderer.transform.localPosition = Vector3.up * y;
        }
    }
}