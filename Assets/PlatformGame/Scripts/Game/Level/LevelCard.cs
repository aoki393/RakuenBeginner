// LevelCard.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCard : MonoBehaviour
{
    [Header("卡片UI组件")]
    [SerializeField] private TextMeshProUGUI levelIndexText;
    [SerializeField] private TextMeshProUGUI starText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Image completedImage;      // 完成时显示的标记（可选）
    [SerializeField] private Image notCompletedImage;   // 未完成时显示的图片（你的要求）

    private LevelConfig currentLevelConfig;
    private LevelRecord currentRecord;

    public void Setup(LevelConfig config)
    {
        currentLevelConfig = config;
        
        if (levelIndexText != null)
            levelIndexText.text = $"关卡 {config.levelIndex}";

        RefreshData();
    }

    public void RefreshData()
    {
        currentRecord = LevelDataManager.Instance.GetLevelRecord(currentLevelConfig.levelIndex);

        if (currentRecord.isCompleted)
        {
            // 显示完成数据
            if (starText != null)
                starText.text = $"{currentRecord.starEarned} / {currentLevelConfig.totalStar}";
            if (coinText != null)
                coinText.text = $"{currentRecord.coinEarned} / {currentLevelConfig.totalCoin}";
            
            if (completedImage != null) completedImage.gameObject.SetActive(true);
            if (notCompletedImage != null) notCompletedImage.gameObject.SetActive(false);
        }
        else
        {
            // 未完成：显示“暂未完成”图片
            if (starText != null) starText.text = "? / ?";
            if (coinText != null) coinText.text = "? / ?";
            
            if (completedImage != null) completedImage.gameObject.SetActive(false);
            if (notCompletedImage != null) notCompletedImage.gameObject.SetActive(true);
        }
    }

    // 点击卡片进入关卡（需要你自己实现场景加载）
    public void OnCardClick()
    {
        Debug.Log($"进入关卡 {currentLevelConfig.levelIndex}");
        // UnityEngine.SceneManagement.SceneManager.LoadScene(currentLevelConfig.sceneName);
    }
}