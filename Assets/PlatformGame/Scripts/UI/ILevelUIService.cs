public interface ILevelHUDService
{
    // 收集相关
    void AddStar(int amount);
    void AddCoin(int amount);
    
    // 获取当前数据
    int GetCurrentStar();
    int GetCurrentCoin();
    
    // 通关界面
    void ShowFinishScreen();
    
    // 可选：获取关卡配置
    int GetTotalStars();
    int GetTotalCoins();

    // 关卡索引
    int CurrentLevelIndex { get; }
}
public interface ILevelFinishService
{
    // 通关界面
    void ShowFinishScreen();
}