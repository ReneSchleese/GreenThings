using UnityEngine;

public class InteractionVolume : MonoBehaviour
{
    public InteractionId InteractionId;
    public Transform TextAnchor;
    public string DisplayText;
}

public enum InteractionId
{
    Exit,
    TreasureHint
}