using UnityEngine;

public class InteractionVolume : MonoBehaviour
{
    public InteractionId InteractionId;
    public Transform TextAnchor;
    [SerializeField] private string _interactionText;

    public string GetInteractionDisplayText()
    {
        // TODO:
        //return $"<font-weight=600>{_interactionText}</font-weight>";
        return _interactionText;
    }
}

public enum InteractionId
{
    Exit,
    TreasureHint
}