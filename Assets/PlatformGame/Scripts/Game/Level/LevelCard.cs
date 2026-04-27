// LevelCard.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlatformGame;

public class LevelCard : MonoBehaviour
{
    [Header("卡片UI组件")]
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI starText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI completedTimeText;
    [SerializeField] private GameObject completedSkin;      // 完成时显示的卡片样式
    [SerializeField] private GameObject completedStarImage;
    [SerializeField] private GameObject completedCoinImage;
    // [SerializeField] private GameObject notCompletedImage;   // 未完成时显示的图片
    [SerializeField] private Button cardButton;         // 卡片按钮（用于点击进入关卡）

    private LevelConfig currentLevelConfig;
    private LevelRecord currentRecord;
    private LevelSelectScreen levelSelectScreen; // 用来卡片点击后Hide整个选择界面

    public void Setup(LevelConfig config, LevelSelectScreen selectScreen)
    {
        currentLevelConfig = config;
        levelSelectScreen = selectScreen;
        
        if (levelNameText != null)
            levelNameText.text = $"{config.sceneName}";

        RefreshData();

        cardButton.onClick.AddListener(OnCardClick);
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
            if (completedTimeText != null)
                completedTimeText.text = $"{currentRecord.completedTime}";
            
            if (completedSkin != null) completedSkin.SetActive(true);

            if(currentRecord.starEarned >= currentLevelConfig.totalStar)
            {
                if (completedStarImage != null) completedStarImage.SetActive(true);
            }
            if(currentRecord.coinEarned >= currentLevelConfig.totalCoin)
            {
                if (completedCoinImage != null) completedCoinImage.SetActive(true);
            }
        }
        else
        {
            if (starText != null) starText.text = $"0 / {currentLevelConfig.totalStar}";
            if (coinText != null) coinText.text = $"0 / {currentLevelConfig.totalCoin}"; 
            
            if (completedSkin != null) completedSkin.gameObject.SetActive(false);
        }
    }

    // 点击卡片进入关卡
    public void OnCardClick()
    {
        GameController.instance.LoadScene(currentLevelConfig.sceneName); // 用的配置里的名字，所以配置里sceneName要和场景文件名一致     
        levelSelectScreen.Hide(); // 隐藏选择界面
    }
}