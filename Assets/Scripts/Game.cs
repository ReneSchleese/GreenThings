using Audio;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private AudioClip _ambientClip;

    public void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        SpawnForestSpirits();
        AudioManager.Instance.PlayAmbient(_ambientClip, loop: true);
    }

    private void SpawnForestSpirits()
    {
        
    }
}