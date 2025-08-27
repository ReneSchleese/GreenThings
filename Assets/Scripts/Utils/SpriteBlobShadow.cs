using UnityEngine;

public class SpriteBlobShadow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _shadowRenderer;
    [SerializeField] private BoxCollider _boxCollider;
    private const float PUSH_UP_Y = 0.1f;
    private Box _box;
    private float _initialAlpha;
    private bool _wasGrounded;
    private float _smoothedAlphaVelocity;

    private void Awake()
    {
        _box = Box.FromCollider(_boxCollider);
        _initialAlpha = _shadowRenderer.color.a;
        SetAlpha(0f);
    }

    public void UpdateShadow()
    {
        Box worldBox = _box.ToWorld(transform);
        Vector3 rayOrigin = worldBox.TopCenter;
        float rayDistance = worldBox.Height;
        Physics.Raycast(new Ray(rayOrigin, Vector3.down), out RaycastHit hit, rayDistance);

        float distanceToGround = rayDistance;
        bool floorIsWithinBox = hit.collider != null;
        if (floorIsWithinBox)
        {
            _shadowRenderer.transform.rotation = Quaternion.LookRotation(hit.normal);
            Vector3 hitPointInLocal = transform.InverseTransformPoint(hit.point);
            float y = Mathf.Clamp(hitPointInLocal.y + PUSH_UP_Y, _box.BotCenter.y, _box.TopCenter.y);
            _shadowRenderer.transform.localPosition = Vector3.up * y;
            distanceToGround = hit.distance;
        }
        
        float targetAlpha = (rayDistance - distanceToGround)/rayDistance * _initialAlpha;
        float smoothedAlpha = Mathf.SmoothDamp(_shadowRenderer.color.a, targetAlpha, ref _smoothedAlphaVelocity, 0.05f);
        SetAlpha(smoothedAlpha);
        IsGrounded = distanceToGround < 0.25f;
    }
    
    private void SetAlpha(float alpha) => _shadowRenderer.color = new Color(_shadowRenderer.color.r, _shadowRenderer.color.g, _shadowRenderer.color.b, alpha);
    public bool IsGrounded { get; private set; }
}