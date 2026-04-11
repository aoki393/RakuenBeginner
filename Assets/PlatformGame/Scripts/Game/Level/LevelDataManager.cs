// LevelDataManager.cs
using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelDataManager : MonoBehaviour
{
    public static LevelDataManager Instance { get; private set; }

    [SerializeField] private LevelDataSO levelDataSO;

    // 存储每个关卡完成时获得的数据
    private Dictionary<int, LevelRecord> levelRecords = new Dictionary<int, LevelRecord>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllRecords();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 加载所有关卡的存档
    private void LoadAllRecords()
    {
        foreach (var level in levelDataSO.levels)
        {
            int star = PlayerPrefs.GetInt($"Level_{level.levelIndex}_Star", -1);
            int coin = PlayerPrefs.GetInt($"Level_{level.levelIndex}_Coin", -1);

            if (star != -1 && coin != -1)
            {
                levelRecords[level.levelIndex] = new LevelRecord
                {
                    isCompleted = true,
                    starEarned = star,
                    coinEarned = coin,
                    completedTime = DateTime.Parse(PlayerPrefs.GetString($"Level_{level.levelIndex}_CompletedTime", "0000-00-00 00:00:00"))
                };
            }
            else
            {
                levelRecords[level.levelIndex] = new LevelRecord
                {
                    isCompleted = false,
                    starEarned = 0,
                    coinEarned = 0
                };
            }
        }
    }

    // 保存关卡完成数据
    public void SaveLevelResult(int levelIndex, int starEarned, int coinEarned, DateTime completedTime)
    {
        PlayerPrefs.SetInt($"Level_{levelIndex}_Star", starEarned);
        PlayerPrefs.SetInt($"Level_{levelIndex}_Coin", coinEarned);
        PlayerPrefs.SetString($"Level_{levelIndex}_CompletedTime", completedTime.ToString("yyyy-MM-dd HH:mm:ss"));
        PlayerPrefs.Save();

        if (levelRecords.ContainsKey(levelIndex))
        {
            levelRecords[levelIndex] = new LevelRecord
            {
                isCompleted = true,
                starEarned = starEarned,
                coinEarned = coinEarned,
                completedTime = completedTime
            };
        }
        else
        {
            levelRecords.Add(levelIndex, new LevelRecord
            {
                isCompleted = true,
                starEarned = starEarned,
                coinEarned = coinEarned,
                completedTime = completedTime
            });
        }
    }

    // 获取关卡记录
    public LevelRecord GetLevelRecord(int levelIndex)
    {
        if (levelRecords.ContainsKey(levelIndex))
            return levelRecords[levelIndex];
        return new LevelRecord { isCompleted = false, starEarned = 0, coinEarned = 0, completedTime = DateTime.MinValue };
    }

    // 获取所有关卡配置
    public List<LevelConfig> GetAllLevels()
    {
        return levelDataSO.levels;
    }
}

[System.Serializable]
public class LevelRecord
{
    public bool isCompleted;
    public int starEarned;
    public int coinEarned;
    public DateTime completedTime;
}