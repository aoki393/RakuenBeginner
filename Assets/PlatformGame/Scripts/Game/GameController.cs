using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlatformGame
{
	[AddComponentMenu("PLAYER TWO/Platformer Project/Game/Game Controller")]
    [RequireComponent(typeof(GameInputManager))]
    public class GameController : Singleton<GameController>
	{
		protected GameLoader m_loader => GameLoader.instance;
		protected GamePauser m_pauser => GamePauser.instance;
		protected GameInputManager m_inputManager;
		// public virtual void AddRetries(int amount) => m_game.retries += amount;

		public virtual void LoadScene(string scene) => m_loader.Load(scene);  // 开放给StartButton

		protected override void Awake()
		{
			base.Awake();
			// retries = initialRetries;
			DontDestroyOnLoad(gameObject);
		}

		void Start()
		{
			m_inputManager = GetComponent<GameInputManager>();
		}
		void Update()
		{
			if (SceneManager.GetActiveScene().name.StartsWith("Level")) // 关卡游戏场景中才能使用暂停功能
			{
				if(m_inputManager.GetPauseDown())
				{
					m_pauser.TogglePause();
				}
			}
			
		}

		/// <summary>
		/// 锁定鼠标光标，菜单呼出时才能看见鼠标
		/// </summary>
		/// <param name="value"></param>
		public static void SetCursorVisible(bool value )
		{
	#if UNITY_STANDALONE || UNITY_WEBGL
			Cursor.visible = value;
			// Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
	#endif
		}
	}
}