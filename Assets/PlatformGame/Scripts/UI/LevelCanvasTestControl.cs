using PlatformGame;
using UnityEngine;

public class LevelCanvasTestControl : MonoBehaviour
{
    public LevelStartPanel levelStartPanel;
    public GameHUD gameHUD;
    public GameFinishScreen gameFinishScreen;
    public PausePanelController pausePanelController;
    void Awake()
    {
        if(GameObject.Find("__GAME_Control__") != null)
        {
            Debug.Log("存在__GAME_Control__，不使用局内测试用Canvas");
            gameObject.SetActive(false);
            return;
        }

        if(levelStartPanel == null)
        {
            // levelStartPanel = FindFirstObjectByType<LevelStartPanel>(); // 未激活对象的脚本不支持
            // levelStartPanel = transform.Find("__LEVEL_Canvas__（局内测试用）/HUD Panel").GetComponent<LevelStartPanel>(); // 支持查找未激活对象的脚本
            // levelStartPanel = transform.GetChild(1).GetComponent<LevelStartPanel>(); // 只查直接子物体，支持查找未激活对象的脚本
            // levelStartPanel = GetComponentInChildren<LevelStartPanel>(); // 未激活对象的脚本不支持
            levelStartPanel = GetComponentInChildren<LevelStartPanel>(true); // 支持查找未激活对象的脚本
        }
        if(gameHUD == null)
        {
            gameHUD = GetComponentInChildren<GameHUD>(true);
        }
        if(pausePanelController == null)
        {
            pausePanelController = GetComponentInChildren<PausePanelController>(true);
        }
        if(gameFinishScreen == null)
        {
            gameFinishScreen = GetComponentInChildren<GameFinishScreen>(true);
        }
        
        levelStartPanel.gameObject.SetActive(true);
        gameHUD.gameObject.SetActive(true);
        pausePanelController.gameObject.SetActive(true); // 先激活使事件订阅生效，在面板里再隐藏
        gameFinishScreen.gameObject.SetActive(true); // 先激活使UIServiceLocator 注册，在面板里再隐藏
    }
}
