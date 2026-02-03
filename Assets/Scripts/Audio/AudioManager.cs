using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioSource _ambientSource;
        [SerializeField] private Transform _effectsTransform;
        [SerializeField] private Transform _voiceTransform;
        [SerializeField] private PoolableAudioSource _audioSourcePrefab;
        [SerializeField] private Transform _inactiveSourcesContainer;
        [SerializeField] private AudioMixerGroup _masterGroup, _voiceGroup;
        
        private PrefabPool<PoolableAudioSource> _effectSourcePool;
        private PrefabPool<PoolableAudioSource> _voiceSourcePool;

        private void Awake()
        {
            _effectSourcePool = new PrefabPool<PoolableAudioSource>(_audioSourcePrefab, _effectsTransform,
                _inactiveSourcesContainer, onBeforeReturn: s => s.OnReturn());
            _voiceSourcePool = new PrefabPool<PoolableAudioSource>(_audioSourcePrefab, _voiceTransform,
                _inactiveSourcesContainer, onBeforeReturn: s => s.OnReturn());
        }

        public void PlayAmbient(AudioClip clip, bool loop)
        {
            _ambientSource.clip = clip;
            _ambientSource.loop = loop;
            _ambientSource.Play();
        }

        public void PlayEffect(AudioClip clip, float pitch = 1.0f, float volume = 1.0f, float delay = 0f)
        {
            PlayPoolable(_effectSourcePool, clip, pitch, group: null, volume, delay);
        }
        
        public void PlayVoice(AudioClip clip, float pitch = 1.0f, float volume = 1.0f)
        {
            PlayPoolable(_voiceSourcePool, clip, pitch, _voiceGroup, volume);
        }

        private void PlayPoolable(PrefabPool<PoolableAudioSource> pool, AudioClip clip, float pitch, AudioMixerGroup group = null, float volume = 1.0f, float delay = 0f)
        {
            PoolableAudioSource audioSource = pool.Get();
            audioSource.Pitch = pitch;
            audioSource.Volume = volume;
            audioSource.AudioMixerGroup = group ? group : _masterGroup;
            StartCoroutine(PlayOneShotThenReturn());
            return;

            IEnumerator PlayOneShotThenReturn()
            {
                if (delay > 0f)
                {
                    yield return new WaitForSeconds(delay);
                }
                yield return audioSource.PlayOneShot(clip);
                pool.Return(audioSource);
            }
        }
    }
}