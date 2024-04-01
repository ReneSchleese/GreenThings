using System.Collections;
using UnityEngine;

public class PoolableAudioSource : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    public IEnumerator PlayOneShot(AudioClip clip)
    {
        _audioSource.loop = false;
        _audioSource.clip = clip;
        _audioSource.PlayOneShot(clip);
        yield return new WaitUntil(() => !_audioSource.isPlaying);
        
    }

    public void OnReturn()
    {
        _audioSource.Stop();        
    }

    public float Pitch
    {
        get => _audioSource.pitch;
        set => _audioSource.pitch = value;
    }

    public bool Loop
    {
        get => _audioSource.loop;
        set => _audioSource.loop = value;
    }

    public bool IsPlaying => _audioSource.isPlaying;
}