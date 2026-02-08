using System;
using Audio;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ChainSounds
{
    [SerializeField] private AudioClip[] _primaryClips;
    [SerializeField] private AudioClip[] _secondaryClips;

    public void PlayEchoed(int index, float clipSeconds, int repetitions)
    {
        AudioClip audioClip = _primaryClips[index];
        float audioClipLength = audioClip.length;
        float volume = 0.4f;
        Sequence sequence = DOTween.Sequence().AppendInterval(clipSeconds).AppendCallback(() =>
        {
            float pitch = Random.Range(1.1f, 1.2f);
            App.Instance.AudioManager.PlayVoice(audioClip, pitch, volume);
        }).SetId(this);
        
        bool usedSecondaryLastTime = false;
        for (int i = 0; i < repetitions; i++)
        {
            float pitch = Random.Range(0.98f, 1.25f);
            AudioClip clip = usedSecondaryLastTime ? audioClip : _secondaryClips[index];
            sequence.AppendInterval(Random.Range(0.1f * audioClipLength, 0.25f * audioClipLength));
            sequence.AppendCallback(() => { PlayVoice(clip, pitch, volume); });
            usedSecondaryLastTime = !usedSecondaryLastTime;
        }
    }

    private void PlayVoice(AudioClip clip, float pitch, float volume)
    {
        App.Instance.AudioManager.PlayVoice(clip, pitch, volume);
    }
}