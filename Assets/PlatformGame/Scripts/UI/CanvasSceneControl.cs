using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlatformGame
{
    /// <summary>
    /// 控制进入场景时对应的Canvas显示和隐藏
    /// </summary>
    public class CanvasSceneControl : MonoBehaviour
    {
        public GameObject mainMenuCanvas;
        public GameObject gameCanvas;

        private void Start()
        {
            if(mainMenuCanvas == null || gameCanvas == null)
            {
                Debug.LogError("Canvas未赋值");
            }

            GameLoader.instance.OnLoadFinish.AddListener(OnLoadFinish);
        }

        private void OnLoadFinish()
        {
            // 加载完成后，根据场景名称控制 Canvas 显示
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "MainMenu")
            {
                mainMenuCanvas.SetActive(true);
                gameCanvas.SetActive(false);
            }
            else if (sceneName.StartsWith("Level"))
            {
                mainMenuCanvas.SetActive(false);
                gameCanvas.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            GameLoader.instance.OnLoadFinish.RemoveListener(OnLoadFinish);
        }
    }
}
