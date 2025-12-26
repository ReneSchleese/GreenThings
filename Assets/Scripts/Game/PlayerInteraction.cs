using System;

public class PlayerInteraction
{
    public event Action Changed;
    
    public void OnEnteredVolume(InteractionObject interaction)
    {
        InteractionObject = interaction;
        Changed?.Invoke();
    }
    
    public void OnExitedVolume(InteractionObject interaction)
    {
        InteractionObject = null;
        Changed?.Invoke();
    }
    
    public InteractionObject InteractionObject { get; private set; }
}