using System;
using UnityEngine;

public class LevelCompleteDoor : MonoBehaviour
{
    // public GameHUD gameHUD; // 获取数据
    // public GameFinishScreen gameFinishScreen; // 获取通关界面


    // ========== 通关逻辑 ==========

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnLevelComplete();
        }
    }

    /// <summary>
    /// 通关时调用，保存数据
    /// </summary>
    public void OnLevelComplete()
    {
        // if(GameObject.Find("__LEVEL_Manager__") != null)
        // {
        //     LevelDataManager.Instance.SaveLevelResult(gameHUD.CurrentLevelIndex, gameHUD.GetCurrentStar(), gameHUD.GetCurrentCoin(), DateTime.Now); // 保存数据
        //     Debug.Log("LevelCompleteDoor: 保存数据成功");
        // }
        // // "__LEVEL_Manager__"
            

        // gameFinishScreen.Show(); // 显示通关界面

        ILevelHUDService hudService = LevelUIServiceLocator.GetHUDService();
        if (hudService == null)
        {
            Debug.LogError("[LevelCompleteDoor] 无法获取 UI 服务");
            return;
        }
        
        // 保存数据
        if (GameObject.Find("__LEVEL_Manager__") != null)
        {
            LevelDataManager.Instance.SaveLevelResult(
                hudService.CurrentLevelIndex,
                hudService.GetCurrentStar(),
                hudService.GetCurrentCoin(),
                DateTime.Now
            );
            Debug.Log("[LevelCompleteDoor] 保存数据成功");
        }

        ILevelFinishService finishService = LevelUIServiceLocator.GetLevelFinishService();
        
        // 显示通关界面
        finishService.ShowFinishScreen();
    }
}
