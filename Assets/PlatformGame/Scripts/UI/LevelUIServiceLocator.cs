using UnityEngine;

public static class LevelUIServiceLocator
{
    private static ILevelHUDService _hudService;
    private static ILevelFinishService _finishService;

    // 由外部在运行时注册
    public static void RegisterHUDService(ILevelHUDService service)
    {
        _hudService = service;
        Debug.Log($"LevelUIServiceLocator 已注册：{_hudService.GetType().Name}");
    }
    public static ILevelHUDService GetHUDService()
    {
        if (_hudService == null)
        {
            Debug.LogError("LevelUIServiceLocator 未注册！");
        }
        return _hudService;
    }

    public static void RegisterLevelFinishService(ILevelFinishService service)
    {
        _finishService = service;
        Debug.Log($"LevelUIServiceLocator 已注册：{_finishService.GetType().Name}");
    }
    public static ILevelFinishService GetLevelFinishService()
    {
        if (_finishService == null)
        {
            Debug.LogError("LevelUIServiceLocator 未注册！");
        }
        return _finishService;
    }

    public static bool HasHUDService()=>_hudService!=null;

    public static bool HasFinishService()=>_finishService!=null;
    
    // public static ILevelUIService Get()
    // {
    //     if (_service == null)
    //     {
    //         _service = FindFirstObjectByType<ILevelUIService>(); // 静态类不继承Mono用不了这个查找，所以使用自行注册做法
    //         if (_service == null)
    //             _service = FindObjectOfType<TestLevelUIService>();
    //     }
    //     return _service;
    // }
}

// 使用
// UIServiceLocator.Get()?.AddStar(1);
// UIServiceLocator.Get()?.AddCoin(10);
// UIServiceLocator.GetLevelFinishService()?.ShowFinishScreen();
