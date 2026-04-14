using System;
using UnityEngine;
using UnityEngine.UI;
using PlatformGame;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIAnimator))]
public class GameFinishScreen : MonoBehaviour, ILevelFinishService
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private UIAnimator uiAnimator;

    void Start()
    {
        LevelUIServiceLocator.RegisterLevelFinishService(this);
        Debug.Log("[GameFinishScreen] 已注册 LevelUI Finish 服务");
        
        restartButton.onClick.AddListener(OnRestartButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        menuButton.onClick.AddListener(OnMenuButtonClick);

        uiAnimator=GetComponent<UIAnimator>();

        gameObject.SetActive(false);
    }

    private void OnMenuButtonClick()
    {
        if(GameObject.Find("__GAME_Control__") != null)
            GameController.instance.LoadScene("MainMenu");
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }

    private void OnRestartButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Show()=>uiAnimator.Show();

    public void ShowFinishScreen() // 接口实现
    {
        Show();
    }
}
