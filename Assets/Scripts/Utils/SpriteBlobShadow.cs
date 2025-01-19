using UnityEngine;

public class SpriteBlobShadow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _blobShadow;

    private void Update()
    {
        UpdateShadow();
    }

    private void UpdateShadow()
    {
        Physics.Raycast(new Ray(_blobShadow.transform.position, Vector3.down), out RaycastHit hit , 2f);
        bool isGrounded = hit.collider != null;
        if (isGrounded)
        {
            _blobShadow.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
    }
}