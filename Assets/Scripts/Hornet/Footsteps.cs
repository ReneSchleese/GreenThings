using Audio;
using JetBrains.Annotations;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] _stepsGrass;

    [UsedImplicitly]
    public void OnFootStep()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        AudioManager.Instance.PlayEffect(_stepsGrass[Random.Range(0, _stepsGrass.Length)], pitch);
    }
}