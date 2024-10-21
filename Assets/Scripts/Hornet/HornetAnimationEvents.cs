using System;
using JetBrains.Annotations;
using UnityEngine;

public class HornetAnimationEvents : MonoBehaviour
{
    public event Action PlayFootStep;
    
    [UsedImplicitly]
    public void OnFootStep()
    {
        PlayFootStep?.Invoke();
    }
}
