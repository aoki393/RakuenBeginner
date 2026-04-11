using System;
using UnityEngine;
using UnityEngine.UI;
using PlatformGame;
using UnityEngine.SceneManagement;

public class GameFinishScreen : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button menuButton;

    void Start()
    {
        restartButton.onClick.AddListener(OnRestartButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        menuButton.onClick.AddListener(OnMenuButtonClick);
    }

    private void OnMenuButtonClick()
    {
        if(GameObject.Find("__GAME_Control__") != null)
            GameController.instance.LoadScene("MainMenu");
    }

    private void OnExitButtonClick()
    {
        
    }

    private void OnRestartButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
