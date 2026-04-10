// LevelSelectUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelSelectUI : MonoBehaviour
{
    [Header("卡片预制体和容器")]
    [SerializeField] private GameObject levelCardPrefab;
    [SerializeField] private Transform cardsContainer;

    [Header("卡片上的UI组件引用（需绑定到预制体）")]
    // 这些是预制体上组件的类型，实际绑定时需要拖拽
    // 也可以用Find或GetComponentInChildren，但为了性能建议拖拽

    private List<LevelCard> levelCards = new List<LevelCard>();

    private void Start()
    {
        InitializeLevelSelectUI();
    }

    private void InitializeLevelSelectUI()
    {
        var allLevels = LevelDataManager.Instance.GetAllLevels();

        foreach (var level in allLevels)
        {
            GameObject card = Instantiate(levelCardPrefab, cardsContainer);
            LevelCard cardScript = card.GetComponent<LevelCard>();
            
            if (cardScript != null)
            {
                cardScript.Setup(level);
                levelCards.Add(cardScript);
            }
        }
    }

    // 刷新所有卡片（例如从其他界面返回时）
    public void RefreshAllCards()
    {
        foreach (var card in levelCards)
        {
            card.RefreshData();
        }
    }
}