using UnityEngine;

public class Coin : MonoBehaviour, ICollectable
{
    [SerializeField] private SpriteBlobShadow _blobShadow;
    [SerializeField] private PhysicsObject _physicsObject;

    public void OnPooled()
    {
        CollectionIsAllowed = false;
        MoneyValue = Random.Range(1, 31);
    }

    private void Update()
    {
        _blobShadow.UpdateShadow();
    }

    public void ApplyForce(Vector3 force) => _physicsObject.ApplyForce(force);

    public bool CollectionIsAllowed { get; set; }
    public int MoneyValue { get; private set; }
    public bool GroundedCheckIsEnabled
    {
        set => _physicsObject.GroundedCheckIsEnabled = value;
    }
}