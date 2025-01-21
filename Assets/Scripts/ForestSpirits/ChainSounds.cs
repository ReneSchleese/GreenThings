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
        DOTween.Kill(this);
        Sequence sequence = DOTween.Sequence().AppendInterval(clipSeconds).AppendCallback(() =>
        {
            AudioManager.Instance.PlayVoice(audioClip, Random.Range(1.2f, 1.35f));
        }).SetId(this);
        for (int i = 0; i < repetitions; i++)
        {
            sequence.AppendInterval(Random.Range(0.1f * audioClipLength, 0.25f * audioClipLength));
            sequence.AppendCallback(() => AudioManager.Instance.PlayVoice(audioClip, Random.Range(1.2f, 1.35f)));
        }
    }
}