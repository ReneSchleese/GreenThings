using UnityEngine;

public class InteractionObject : MonoBehaviour
{
    public InteractionId InteractionId;
    public Transform TextAnchor;
}

public enum InteractionId
{
    Exit,
    TreasureHint
}