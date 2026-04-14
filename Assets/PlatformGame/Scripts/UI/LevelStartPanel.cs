using System;
using UnityEngine;
using UnityEngine.UI;
using PLAYERTWO.PlatformerProject;
using PlatformGame;
using System.Collections;

public class LevelStartPanel : MonoBehaviour
{
    public Button startButton;
    public Player player;
    // void Awake()
    // {
    //     Debug.Log("LevelStartPanel Awake");
    //     gameObject.SetActive(true); // 关卡开始时显示面板
    // }
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

        GameController.SetCursorVisible(false);
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
