using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlatformGame;

[RequireComponent(typeof(UIAnimator))]
public class LevelSelectScreen : MonoBehaviour
{
    public Button closeButton;
    
    [Header("卡片预制体和容器")]
    [SerializeField] private GameObject levelCardPrefab;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private UIAnimator uiAnimator;

    [Header("卡片上的UI组件引用（需绑定到预制体）")]
    // 这些是预制体上组件的类型，实际绑定时需要拖拽
    // 也可以用Find或GetComponentInChildren，但为了性能建议拖拽

    private List<LevelCard> levelCards = new List<LevelCard>();

    void Start()
    {
        uiAnimator=GetComponent<UIAnimator>();
    }

    private void OnEnable() // 只要界面显示就要初始化一次
    {
        // Debug.Log("LevelSelectScreen: OnEnable");
        InitializeLevelSelectUI(); // 初始化UI
    }

    private void InitializeLevelSelectUI()
    {
        var allLevels = LevelDataManager.Instance.GetAllLevels();

        // 清空之前生成的所有卡片子物体
        for (int i = 0; i < cardsContainer.childCount; i++)
        {
            Transform child = cardsContainer.GetChild(i);
            // Debug.Log($"删除: {child.name}, 当前i={i}, childCount={cardsContainer.childCount}");
            Destroy(child.gameObject);
        }
        levelCards.Clear();

        foreach (var level in allLevels)
        {
            GameObject card = Instantiate(levelCardPrefab, cardsContainer);
            LevelCard cardScript = card.GetComponent<LevelCard>();
            
            if (cardScript != null)
            {
                cardScript.Setup(level, this);
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
    
    public void Show()=>uiAnimator.Show();

    // public void Hide()=>uiAnimator.Hide();
    public void Hide()=>gameObject.SetActive(false);
}
