using Audio;
using DG.Tweening;
using UnityEngine;

public class ChainSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] _audioClips;

    public void PlayEchoed(int index, float clipSeconds, int repetitions)
    {
        AudioClip audioClip = _audioClips[index];
        float audioClipLength = audioClip.length;
        float volume = 0.5f;
        DOTween.Kill(this);
        Sequence sequence = DOTween.Sequence().AppendInterval(clipSeconds).AppendCallback(() =>
        {
            float pitch = Random.Range(1.2f, 1.35f);
            AudioManager.Instance.PlayVoice(audioClip, pitch, volume);
        }).SetId(this);
        for (int i = 0; i < repetitions; i++)
        {
            float pitch = Random.Range(0.98f, 1.4f);
            sequence.AppendInterval(Random.Range(0.1f * audioClipLength, 0.25f * audioClipLength));
            sequence.AppendCallback(() => PlayVoice(audioClip, pitch, volume));
        }
    }

    private void PlayVoice(AudioClip clip, float pitch, float volume)
    {
        AudioManager.Instance.PlayVoice(clip, pitch, volume);
    }
}