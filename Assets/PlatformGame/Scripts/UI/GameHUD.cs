// InGameUI.cs
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using PlatformGame;

/// <summary>
/// 按理说数据不应该写在UI面板里，但目前数据少就直接写这了……
/// </summary>
public class GameHUD : MonoBehaviour, ILevelHUDService
{
    [Header("UI Text 组件")]
    [SerializeField] private TextMeshProUGUI starNumText;
    [SerializeField] private TextMeshProUGUI coinNumText;
    public int CurrentLevelIndex{ get; private set; } // 关卡保存时外部要读取
    private int currentStar;
    private int currentCoin;    
    private int totalStars;
    private int totalCoins;
    [SerializeField] private LevelDataSO levelDataSO; // 关卡配置数据，局内测试Test用
    private LevelConfig currentLevelConfig; 

    private void Start()
    {
        // InitializeLevelData();
        TryRegisterAsService();
        // Debug.Log("[GameHUD] Start 初始状态设置完成");
    }

    public void InitializeLevelData() // 每次进入关卡都要初始化数据
    {
        CurrentLevelIndex = GetCurrentLevelIndex();
        currentLevelConfig = GetCurrentLevelConfig();

        currentStar = 0;
        currentCoin = 0;

        totalStars = currentLevelConfig.totalStar;
        totalCoins = currentLevelConfig.totalCoin;
        
        UpdateStarUI();
        UpdateCoinUI();

        Debug.Log($"[GameHUD] 初始化关卡数据：{currentLevelConfig.sceneName}，总星星 {totalStars}，总金币 {totalCoins}");
    }
    private void TryRegisterAsService()
    {
        LevelUIServiceLocator.RegisterHUDService(this);
        // Debug.Log("[GameHUD] 已注册 LevelUI HUD 服务");
    }
    // ========== 实现 ILevelUIService 接口 ==========

    /// <summary>
    /// 增加星星（收集星星时调用）
    /// </summary>
    public void AddStar(int amount = 1)
    {
        currentStar += amount;
        UpdateStarUI();
    }

    /// <summary>
    /// 增加金币（收集金币时调用）
    /// </summary>
    public void AddCoin(int amount = 1)
    {
        currentCoin += amount;        
        UpdateCoinUI();
    }
    
    public int GetCurrentStar() => currentStar;
    
    public int GetCurrentCoin() => currentCoin;
    
    public int GetTotalStars() => totalStars;
    
    public int GetTotalCoins() => totalCoins;
    
    public void ShowFinishScreen()
    {
        // GameHUD 不负责显示通关界面，这个方法留空
        // 实际由 GameFinishScreen 实现
        Debug.LogWarning("[GameHUD] ShowFinishScreen 应由 GameFinishScreen 实现");
    }



    // ========== UI 更新方法 ==========

    /// <summary>
    /// 更新星星UI显示
    /// </summary>
    public void UpdateStarUI()
    {
        if (starNumText != null)
            starNumText.text = currentStar.ToString()+" / " + totalStars.ToString();
    }

    /// <summary>
    /// 更新金币UI显示
    /// </summary>
    public void UpdateCoinUI()
    {
        if (coinNumText != null)
            coinNumText.text = currentCoin.ToString() +" / " + totalCoins.ToString();  
    }

    

    // ========== 辅助方法 ==========

    /// <summary>
    /// 获取当前关卡索引，从场景名中解析（要求场景命名与SO配置中对应，格式：Level 01）
    /// </summary>
    private int GetCurrentLevelIndex()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string[] parts = sceneName.Split(' ');
        
        if (parts.Length < 2 || !int.TryParse(parts[1], out int levelIndex))
        {
            Debug.LogError($"关卡数字解析失败：场景名'{sceneName}'不符合'Level XX'格式");
            return -1;
        }
        
        return levelIndex;
    }

    private LevelConfig GetCurrentLevelConfig()
    {
        // 场景中没有"__LEVEL_Manager__"的时候说明在进行局内测试，直接用SO配置数据
        if(GameObject.Find("__LEVEL_Manager__") == null)
        {
            if (levelDataSO != null && CurrentLevelIndex < levelDataSO.levels.Count)
                return levelDataSO.levels.Find(l => l.levelIndex == CurrentLevelIndex);
            
            Debug.LogError("[GameHUD] 无法获取测试用关卡配置");
            return null;
        }

        var levels = LevelDataManager.Instance.GetAllLevels();
        return levels.Find(l => l.levelIndex == CurrentLevelIndex);
    }
}