using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PlatformGame
{
    /// <summary>
    /// 游戏系统级输入管理器（与Player的玩家输入管理器分离）。  
    /// 涉及功能：1、暂停游戏。
    /// </summary>

    public class GameInputManager : MonoBehaviour
    {
        // 输入动作资源（在 Input System 中配置的 InputActionAsset）
        public InputActionAsset actions;

        protected InputAction m_pause;
        public virtual bool GetPauseDown() => m_pause.WasPressedThisFrame();

        void Awake()
        {
            m_pause = actions["Pause"];
        }


    }
}