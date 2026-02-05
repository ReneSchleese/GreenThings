using UnityEngine;

public class Vinyl : MonoBehaviour, ICollectable
{
    [SerializeField] private SpriteBlobShadow _blobShadow;
    [SerializeField] private PhysicsObject _physicsObject;
    
    private void Update()
    {
        _blobShadow.UpdateShadow();
    }

    public void ApplyForce(Vector3 force) => _physicsObject.ApplyForce(force);

    public bool GroundedCheckIsEnabled
    {
        set => _physicsObject.GroundedCheckIsEnabled = value;
    }
    
    public VinylId? Id { get; set; }
    public bool CollectionIsAllowed { get; set; }
}

public enum VinylId
{
    Leyndell,
    GreenPath
}