using System;
using UnityEngine;
using UnityEngine.UI;
using PLAYERTWO.PlatformerProject;
using System.Collections;

public class LevelStartPanel : MonoBehaviour
{
    public Button startButton;
    public Player player;
    void Start()
    {
        if(player == null){
            player = FindFirstObjectByType<Player>();
        }

        startButton.onClick.AddListener(OnStartButtonClick);

        StartCoroutine(Routine());     
        
    }

    private void OnStartButtonClick()
    {
        // 隐藏面板、启用相机环绕、playerInput
        gameObject.SetActive(false);

        player.Inputs.actions.Enable();

        SetCursorVisible(false);
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
    protected virtual IEnumerator Routine()
    {
        yield return null; // 等待一帧
        player.Inputs.actions.Disable();

        // Game.LockCursor();                              // 锁定鼠标光标（隐藏并锁定在窗口中央）
        // m_pauser.canPause = true;                       // 允许游戏暂停
        // OnStart?.Invoke();                             // 触发关卡开始事件，通知其他系统
    }
    
}
