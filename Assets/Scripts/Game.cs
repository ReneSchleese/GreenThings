using Audio;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private AudioClip _ambientClip;

    private void Start()
    {
        AudioManager.Instance.PlayAmbient(_ambientClip, loop: true);
    }
}