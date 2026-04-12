using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlatformGame
{
    /// <summary>
    /// 控制进入场景时对应的Canvas显示和隐藏
    /// </summary>
    public class CanvasSceneControl : Singleton<CanvasSceneControl>
    {
        public GameObject mainMenuCanvas;
        public GameObject gameCanvas;
        
        protected override void Awake()
        {
            base.Awake();
            // 挂在UI Root上，让其跨场景存在
            DontDestroyOnLoad(gameObject);
        }

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

        // private void OnDestroy() // UI Root只在游戏程序终止时销毁，因此无需手动移除监听
        // {
        //     GameLoader.instance.OnLoadFinish.RemoveListener(OnLoadFinish); // GameLoader先销毁会导致这里报错
        // }
    }
}
