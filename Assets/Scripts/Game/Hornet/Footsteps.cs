using Audio;
using JetBrains.Annotations;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] _stepsGrass;
    private PseudoRandomIndex _pseudoRandom;

    private void Awake()
    {
        _pseudoRandom = new PseudoRandomIndex(length: _stepsGrass.Length);
    }

    [UsedImplicitly]
    public void OnFootStep()
    {
        float pitch = Random.Range(0.7f, 1.2f);
        AudioManager.Instance.PlayEffect(_stepsGrass[_pseudoRandom.Get()], pitch);
    }
}