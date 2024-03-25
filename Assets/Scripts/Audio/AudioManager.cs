using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _ambientSource;
        [SerializeField] private AudioSource _effectSource;
        
        private static AudioManager _instance;
        
        public void PlayAmbient(AudioClip clip, bool loop)
        {
            _ambientSource.clip = clip;
            _ambientSource.loop = loop;
            _ambientSource.Play();
        }

        public void PlayEffect(AudioClip clip)
        {
            _effectSource.PlayOneShot(clip);
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