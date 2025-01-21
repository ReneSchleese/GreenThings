using Audio;
using DG.Tweening;
using UnityEngine;

public class ChainSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] _audioClips;

    public void PlayEchoed(int index)
    {
        DOTween.Kill(this);
        DOTween.Sequence().AppendInterval(1f).AppendCallback(() =>
        {
            AudioManager.Instance.PlayVoice(_audioClips[index], Random.Range(1.1f, 1.25f));
        }).SetId(this);
    }
}