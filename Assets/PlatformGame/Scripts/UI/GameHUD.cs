// InGameUI.cs
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 按理说数据不应该写在UI面板里，但目前数据少就直接写这了……
/// </summary>
public class GameHUD : MonoBehaviour
{
    [Header("UI Text 组件")]
    [SerializeField] private TextMeshProUGUI starNumText;
    [SerializeField] private TextMeshProUGUI coinNumText;

    private int currentStar;
    private int currentCoin;
    public int CurrentLevelIndex{ get; private set; } // 关卡保存时外部要读取
    private int totalStars;
    private int totalCoins;
    [SerializeField] private LevelDataSO levelDataSO; // 关卡配置数据，局内测试Test用
    private LevelConfig currentLevelConfig; 

    private void Start()
    {
        CurrentLevelIndex = GetCurrentLevelIndex();
        currentLevelConfig = GetCurrentLevelConfig();

        currentStar = 0;
        currentCoin = 0;

        totalStars = currentLevelConfig.totalStar;
        totalCoins = currentLevelConfig.totalCoin;
        
        UpdateStarUI();
        UpdateCoinUI();
    }

    // ========== 核心方法：收集时调用 ==========
    
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

    public int GetCurrentStar() => currentStar;

    public int GetCurrentCoin() => currentCoin;

    /// <summary>
    /// 获取当前关卡索引，从场景名中解析（要求场景命名与SO配置中对应，格式：Level 01）
    /// </summary>
    private int GetCurrentLevelIndex()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string[] parts = sceneName.Split(' ');
        
        if (!int.TryParse(parts[1], out int levelIndex))
        {
            Debug.LogError($"关卡数字解析失败：'{parts[1]}' 不是有效的数字（场景名：'{sceneName}'）");
            return -1;
        }
        
        return levelIndex;
    }

    private LevelConfig GetCurrentLevelConfig()
    {
        // 场景中没有"__LEVEL_Manager__"的时候说明在进行局内测试，直接用SO配置数据
        if(GameObject.Find("__LEVEL_Manager__") == null)
        {
            return levelDataSO.levels[CurrentLevelIndex];
        }

        var levels = LevelDataManager.Instance.GetAllLevels();
        return levels.Find(l => l.levelIndex == CurrentLevelIndex);
    }
}