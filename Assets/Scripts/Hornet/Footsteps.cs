using JetBrains.Annotations;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] _stepsGrass;

    [UsedImplicitly]
    public void OnFootStep()
    {
        Debug.Log("Footstep!");
    }
}