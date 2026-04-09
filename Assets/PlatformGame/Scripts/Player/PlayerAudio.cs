using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class PlayerAudio : MonoBehaviour
    {
        // [SerializeField] private AudioSource _audioSource;
        protected AudioSource m_audio;
        [SerializeField] private float defaultVolume = 0.5f;
        [SerializeField] private AudioClip[] _hurtSound;
        // [SerializeField] private AudioClip _deathSound;

        private void Start()
        {
            if (!TryGetComponent(out m_audio))
			{
				m_audio = gameObject.AddComponent<AudioSource>();
			}
        }
        protected virtual void PlayRandomClip(AudioClip[] clips)
		{
			if (clips.Length > 0)
			{
				var index = Random.Range(0, clips.Length);
				m_audio.PlayOneShot(clips[index], defaultVolume);
			}
		}

        public void PlayHurtSound()
        {
            PlayRandomClip(_hurtSound);
        }

        public void PlayDeathSound()
        {
            
        }
    }
    
}
