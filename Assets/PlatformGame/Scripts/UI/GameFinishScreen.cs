using System;
using UnityEngine;
using UnityEngine.UI;
using PlatformGame;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

[RequireComponent(typeof(UIAnimator))]
public class GameFinishScreen : MonoBehaviour, ILevelFinishService
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private UIAnimator uiAnimator;
    public UnityEvent OnLevelRetryEvent;

    void Start()
    {
        LevelUIServiceLocator.RegisterLevelFinishService(this);
        // Debug.Log("[GameFinishScreen] 已注册 LevelUI Finish 服务");
        
        restartButton.onClick.AddListener(OnRestartButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        menuButton.onClick.AddListener(OnMenuButtonClick);

        uiAnimator=GetComponent<UIAnimator>();

        gameObject.SetActive(false);
        // Debug.Log("[GameFinishScreen] Start 初始状态设置完成");
    }

    private void OnMenuButtonClick()
    {
        if(GameObject.Find("__GAME_Control__") != null) // 正式游戏时
        {
            GameController.Instance.LoadScene("MainMenu");
        }            
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }

    private void OnRestartButtonClick()
    {
        OnLevelRetryEvent.Invoke(); // 触发重试事件，通知游戏控制器重置关卡
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Show()=>uiAnimator.Show();

    public void ShowFinishScreen() // 接口实现
    {
        Show();
    }
}
