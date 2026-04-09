using UnityEngine;
using System.Collections.Generic;

namespace PLAYERTWO.PlatformerProject
{
    [RequireComponent(typeof(Player))]
    [AddComponentMenu("PLAYER TWO/Platformer Project/Player/Player Footsteps")]
    [Tooltip("播放玩家脚步声和落地音效")]
    public class PlayerFootstepAudio : MonoBehaviour
    {
        [System.Serializable]
		public class Surface
		{
			public string tag; // 地面类型标签
			public AudioClip[] footsteps;
			public AudioClip[] landings;
		}

        private Player m_player;
        protected AudioSource m_audio;
        public Surface[] surfaces;
        public AudioClip[] defaultFootsteps;
        public AudioClip[] defaultLandings;
        [Header("General Settings")]
        public float stepOffset = 1.25f; // 每走多少距离触发一次脚步声
        [SerializeField] private float footstepVolume = 0.5f;
		[SerializeField] private const float defaultVolume = 0.5f;
        protected Vector3 m_lastLateralPosition;
        protected Dictionary<string, AudioClip[]> m_footsteps = new Dictionary<string, AudioClip[]>();
        protected Dictionary<string, AudioClip[]> m_landings = new Dictionary<string, AudioClip[]>();

		public AudioClip[] hurtSounds; // 受伤音效
        protected virtual void Start()
		{
			m_player = GetComponent<Player>();
			m_player.entityEvents.OnGroundEnter.AddListener(Landing);

			if (!TryGetComponent(out m_audio))
			{
				m_audio = gameObject.AddComponent<AudioSource>();
			}

			foreach (var surface in surfaces)
			{
				m_footsteps.Add(surface.tag, surface.footsteps);
				m_landings.Add(surface.tag, surface.landings);
			}
		}

        protected virtual void Update()
		{
			if (m_player.isGrounded && m_player.States.IsCurrentOfType(typeof(WalkPlayerState)))
			{
				var position = transform.position;
				var lateralPosition = new Vector3(position.x, 0, position.z);
				var distance = (m_lastLateralPosition - lateralPosition).magnitude;

				if (distance >= stepOffset)
				{
					if (m_footsteps.ContainsKey(m_player.groundHit.collider.tag))
					{
						PlayRandomClip(m_footsteps[m_player.groundHit.collider.tag], footstepVolume);
					}
					else
					{
						PlayRandomClip(defaultFootsteps, footstepVolume);
					}

					m_lastLateralPosition = lateralPosition;
				}
			}
		}
        protected virtual void Landing()
		{
			if (!m_player.onWater)
			{
				if (m_landings.ContainsKey(m_player.groundHit.collider.tag))
				{
					PlayRandomClip(m_landings[m_player.groundHit.collider.tag], footstepVolume);
				}
				else
				{
					PlayRandomClip(defaultLandings, footstepVolume);
				}
			}
		}

		/// <summary>
		///  播放受伤音效，通过PlayerEvents的OnHurt事件调用
		/// </summary>
		public void PlayHurtSound()
		{
			PlayRandomClip(hurtSounds);
		}
        
        /// <summary>
		/// 从给定的音效数组中随机播放一个音效
		/// </summary>
        protected virtual void PlayRandomClip(AudioClip[] clips, float volume = defaultVolume)
		{
			if (clips.Length > 0)
			{
				var index = Random.Range(0, clips.Length);
				m_audio.PlayOneShot(clips[index], volume);
			}
		}
    }

}
