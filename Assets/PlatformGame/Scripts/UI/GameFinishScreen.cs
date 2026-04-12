using System;
using UnityEngine;
using UnityEngine.UI;
using PlatformGame;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIAnimator))]
public class GameFinishScreen : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private UIAnimator uiAnimator;

    void Start()
    {
        restartButton.onClick.AddListener(OnRestartButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        menuButton.onClick.AddListener(OnMenuButtonClick);

        uiAnimator=GetComponent<UIAnimator>();
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

    public void Show()=>uiAnimator.Show();
}
