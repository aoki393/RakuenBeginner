using UnityEngine;

public class LevelData
{
    public int Score{get; private set;}
    public int CoinNum{get; private set;}
    public void SetLevelData(int score, int coinNum = 0)
    {
        Score = score;
        CoinNum = coinNum;
    }
    public void SaveLevelData()
    {
        // TODO: 保存关卡数据
        PlayerPrefs.SetInt("Score", Score);
        PlayerPrefs.SetInt("CoinNum", CoinNum);
        PlayerPrefs.Save();
    }
}
