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

    void Start()
    {
        if(player == null){
            player = FindFirstObjectByType<Player>();
        }

        startButton.onClick.AddListener(OnStartButtonClick);
    }

    private void OnStartButtonClick()
    {
        // 隐藏面板、启用相机环绕、playerInput
        gameObject.SetActive(false);

        if(player == null){ // Retry的时候场景重置会导致player丢失……
            // Debug.LogWarning("LevelStartPanel: 未找到Player对象"); 
            player = FindFirstObjectByType<Player>();
        }

        player.Inputs.actions.Enable();  // 启用玩家输入

        GameController.SetCursorVisible(false);
    }
    
}
