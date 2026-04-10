// InGameUI.cs
using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("UI Text 组件")]
    [SerializeField] private TextMeshProUGUI starNumText;
    [SerializeField] private TextMeshProUGUI coinNumText;

    private int currentStar;
    private int currentCoin;
    private int currentLevelIndex;
    private int totalStars;
    private int totalCoins;
    public LevelDataSO levelDataSO; // 局内Test
    private LevelConfig currentLevelConfig; 

    private void Start()
    {
        // currentLevelIndex = GetCurrentLevelIndex();
        // currentLevelConfig = GetCurrentLevelConfig();

        // 局内Test
        currentLevelIndex = 1;
        currentLevelConfig = levelDataSO.levels[currentLevelIndex];
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

    // ========== 通关逻辑 ==========

    /// <summary>
    /// 通关时调用，保存数据
    /// </summary>
    public void OnLevelComplete()
    {
        LevelDataManager.Instance.SaveLevelResult(currentLevelIndex, currentStar, currentCoin);
    }

    // ========== 辅助方法 ==========

    /// <summary>
    /// 获取当前收集到的星星数
    /// </summary>
    public int GetCurrentStar() => currentStar;

    /// <summary>
    /// 获取当前收集到的金币数
    /// </summary>
    public int GetCurrentCoin() => currentCoin;

    private int GetCurrentLevelIndex()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }

    private LevelConfig GetCurrentLevelConfig()
    {
        var levels = LevelDataManager.Instance.GetAllLevels();
        return levels.Find(l => l.levelIndex == currentLevelIndex);
    }
}