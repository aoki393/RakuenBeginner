// CollectableBindHUD.cs
using UnityEngine;
using PLAYERTWO.PlatformerProject;

/// <summary>
/// 动态绑定收集品到 HUD 服务
/// 根据父级物体名称区分 Star 和 Coin（仅查找分组下的收集品，性能优化）
/// </summary>
public class CollectableBindHUD : MonoBehaviour
{
    [Header("Tag 名称（区分星星和金币）")]
    [SerializeField] private string starTag = GameTags.Star;
    [SerializeField] private string coinTag = GameTags.Coin;
    
    [SerializeField] private bool bindOnStart = true;
    [SerializeField] private float bindDelay = 2.5f; // 延迟绑定，确保所有物体都已加载
    
    private ILevelHUDService _hudService;
    private float _startTime;
    
    private void Start()
    {
        _startTime = Time.time;

        if (bindOnStart)
        {
            if (bindDelay > 0)
                Invoke(nameof(BindAllCollectables), bindDelay);
            else
                BindAllCollectables();
        }
    }
    
    /// <summary>
    /// 绑定场景中所有收集品
    /// </summary>
    public void BindAllCollectables()
    {
        _hudService = LevelUIServiceLocator.GetHUDService();
        if (_hudService == null)
        {
            Debug.LogError($"[CollectableBindHUD] 无法获取 HUD 服务，绑定失败，实际绑定延迟为 {Time.time - _startTime:F2}");
            return;
        }
        
        int starCount = 0;
        int coinCount = 0;

        Collectable[] collectables = FindObjectsByType<Collectable>(FindObjectsSortMode.None);
        foreach (var collectable in collectables)
        {
            if (collectable.CompareTag(starTag))
            {
                if (TryBindCollectable(collectable, true))
                    starCount++;
            }
            else if (collectable.CompareTag(coinTag))
            {
                if (TryBindCollectable(collectable, false))
                    coinCount++;
            }
        }

        Debug.Log($"[CollectableBindHUD] 绑定完成：StarCount={starCount}, CoinCount={coinCount}");
    }
    
    
    /// <summary>
    /// 尝试绑定单个收集品
    /// </summary>
    private bool TryBindCollectable(Collectable collectable, bool isStar)
    {
        if (collectable == null) return false;
        
        // 清除已有监听（避免重复绑定）
        collectable.onCollect.RemoveAllListeners();
        
        // 绑定对应方法
        if (isStar)
        {
            collectable.onCollect.AddListener((player) => _hudService.AddStar(1));
        }
        else
        {
            collectable.onCollect.AddListener((player) => _hudService.AddCoin(1));
        }
        
        return true;
    }
}