using UnityEngine;
using UnityEngine.Events;

namespace PlatformGame
{
    /// <summary>
    /// 游戏暂停控制器，管理游戏的暂停状态和相关事件
    /// </summary>

    [RequireComponent(typeof(GameInputManager))]
    public class GamePause : MonoBehaviour
    {
        protected GameInputManager m_inputManager;
        private bool isPaused = false; 
        public UnityEvent<bool> OnPauseEvent; // 暂停事件，让UI、Audio等订阅        

        void Awake()
        {
            m_inputManager = GetComponent<GameInputManager>(); 
        }

        void Update()
        {
            if (m_inputManager.GetPauseDown())
            {
                TogglePause();
            }
        }
        public void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f; // 暂停或恢复游戏
            OnPauseEvent?.Invoke(isPaused);      // 触发暂停事件，执行暂停面板显隐、音效播放等
        }
    }
}
