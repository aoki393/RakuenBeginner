// LevelDataSO.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Game/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    public List<LevelConfig> levels = new List<LevelConfig>();
}

[System.Serializable]
public class LevelConfig
{
    public int levelIndex;
    public int totalStar;      // 关卡总星星数（例如3）
    public int totalCoin;      // 关卡总金币数
    public string sceneName;   // 对应场景名
}