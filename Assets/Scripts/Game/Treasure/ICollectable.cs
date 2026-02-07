using UnityEngine;

public interface ICollectable
{
    public void ApplyForce(Vector3 force);
    public bool CollectionIsAllowed { get; set; }
    public bool GroundedCheckIsEnabled { set; }
}