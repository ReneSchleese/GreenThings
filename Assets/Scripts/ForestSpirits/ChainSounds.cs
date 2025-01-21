using Audio;
using DG.Tweening;
using UnityEngine;

public class ChainSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] _audioClips;

    public void PlayEchoed(int index, float clipSeconds)
    {
        DOTween.Kill(this);
        DOTween.Sequence().AppendInterval(clipSeconds).AppendCallback(() =>
        {
            AudioManager.Instance.PlayVoice(_audioClips[index], Random.Range(1.1f, 1.25f));
        }).SetId(this);
    }
}