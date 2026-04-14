using UnityEngine;

public class LevelCanvasTestControl : MonoBehaviour
{
    public LevelStartPanel levelStartPanel;
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
        if (levelStartPanel != null)
        {
            levelStartPanel.gameObject.SetActive(true);
        }
    }
}
