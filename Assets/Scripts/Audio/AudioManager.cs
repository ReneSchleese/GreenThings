using System;
using System.Collections;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _ambientSource;
        [SerializeField] private AudioSource _effectSource;
        [SerializeField] private PoolableAudioSource _audioSourcePrefab;
        [SerializeField] private Transform _inactiveSourcesContainer;
        
        private static AudioManager _instance;
        private PrefabPool<PoolableAudioSource> _effectSourcePool;

        private void Awake()
        {
            _effectSourcePool = new PrefabPool<PoolableAudioSource>(_audioSourcePrefab, _effectSource.transform,
                _inactiveSourcesContainer, onBeforeReturn: s => s.OnReturn());
        }

        public void PlayAmbient(AudioClip clip, bool loop)
        {
            _ambientSource.clip = clip;
            _ambientSource.loop = loop;
            _ambientSource.Play();
        }

        public void PlayEffect(AudioClip clip, float pitch = 1.0f)
        {
            PoolableAudioSource audioSource = _effectSourcePool.Get();
            audioSource.Pitch = pitch;
            StartCoroutine(PlayOneShotThenReturn());

            IEnumerator PlayOneShotThenReturn()
            {
                yield return audioSource.PlayOneShot(clip);
                _effectSourcePool.Return(audioSource);
            }
        }
        
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                }
                return _instance;
            }
        }
    }
}