using UnityEngine;

public class InteractionObject : MonoBehaviour
{
    public InteractionId InteractionId;
    public Transform TextAnchor;
    [SerializeField] private string _interactionText;

    public string GetInteractionDisplayText()
    {
        return $"<font-weight=600>{_interactionText}</font-weight>";
    }
}

public enum InteractionId
{
    Exit,
    TreasureHint
}