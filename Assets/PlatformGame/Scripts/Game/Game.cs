using UnityEngine;

namespace PlatformGame
{
    /// <summary>
    /// 游戏主控制器，管理游戏的核心逻辑和状态
    /// </summary>

    public class Game : Singleton<Game>
    {
        protected override void Awake()
		{
			base.Awake();
			// retries = initialRetries;
			DontDestroyOnLoad(gameObject);
		}
    }
}
