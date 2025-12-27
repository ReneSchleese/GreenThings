using System;

public class PlayerInteractionState
{
    public event Action Changed;
    
    public void OnEnteredVolume(InteractionVolume interaction)
    {
        InteractionVolume = interaction;
        Changed?.Invoke();
    }
    
    public void OnExitedVolume(InteractionVolume interaction)
    {
        InteractionVolume = null;
        Changed?.Invoke();
    }
    
    public InteractionVolume InteractionVolume { get; private set; }
}