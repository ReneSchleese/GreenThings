using DG.Tweening;
using UnityEngine;

public class SpriteBlobShadow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _shadowRenderer;
    [SerializeField] private BoxCollider _boxCollider;
    private const float PUSH_UP_Y = 0.1f;
    private Box _box;
    private float _initialAlpha;
    private bool _wasGrounded;

    private void Awake()
    {
        _box = Box.FromCollider(_boxCollider);
        _initialAlpha = _shadowRenderer.color.a;
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

    private void Update()
    {
        UpdateShadow();
    }

    private void UpdateShadow()
    {
        Box worldBox = _box.ToWorld(transform);
        Vector3 origin = worldBox.TopCenter;
        float distance = worldBox.Height;
        Physics.Raycast(new Ray(origin, Vector3.down), out RaycastHit hit, distance);
        
        bool isGrounded = hit.collider != null;
        if (isGrounded)
        {
            _shadowRenderer.transform.rotation = Quaternion.LookRotation(hit.normal);
            Vector3 hitPointInLocal = transform.InverseTransformPoint(hit.point);
            float y = Mathf.Clamp(hitPointInLocal.y + PUSH_UP_Y, _box.BotCenter.y, _box.TopCenter.y);
            _shadowRenderer.transform.localPosition = Vector3.up * y;
        }

        bool changed = _wasGrounded != isGrounded;
        _wasGrounded = isGrounded;
        if (changed)
        {
            DOTween.Kill(this);
            _shadowRenderer.DOFade(isGrounded ? _initialAlpha : 0f, 0.2f).SetId(this);
        }
    }
}