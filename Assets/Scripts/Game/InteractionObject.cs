using UnityEngine;

public class InteractionObject : MonoBehaviour
{
    public InteractionId InteractionId;
}

public enum InteractionId
{
    Exit,
    TreasureHint
}