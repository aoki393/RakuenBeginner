using System.Collections;
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
        [Header("Game Level UI")]
        private PausePanelController pausePanelController;
        private GameHUD gameHUD;
        private GameFinishScreen gameFinishScreen;
        private LevelStartPanel levelStartPanel;

        protected override void Awake()
        {
            base.Awake();
            // 挂在UI Root上，让其跨场景存在
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if(mainMenuCanvas == null || gameCanvas == null)
            {
                Debug.LogError("Canvas未赋值");
            }

            GameLoader.instance.OnLoadFinish.AddListener(OnLoadFinish); // 监听直至游戏结束，无需手动移除监听

            gameCanvas.SetActive(true);
            GetLevelUIComponent(); // 一开始在MainMenu就初始化一下Level相关的UI

            StartCoroutine(InitialCanvasState()); 
        }
        IEnumerator InitialCanvasState()
        {
            yield return null; // 等待一帧，同一帧多次SetActive是无效的
            gameCanvas.SetActive(false);
        }

        public void RetryLevel()
        {
            Debug.Log("CanvasSceneControl: 关卡Retry重置UI");
            if(gameFinishScreen != null)
            {
                gameFinishScreen.gameObject.SetActive(false); // 隐藏关卡完成界面
            }
            if(levelStartPanel  != null)
            {
                levelStartPanel.gameObject.SetActive(true); // 显示关卡开始界面
            }
            if (gameHUD != null)            {
                gameHUD.InitializeLevelData(); // 重置HUD显示
            }
        }

        private void OnLoadFinish()
        {
            Debug.Log("CanvasSceneControl: 场景加载完成，控制Canvas显示");
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
                // 初始化其下的面板
                LevelUIInit();
            }
        }


        private void LevelUIInit()
        {
            Debug.Log("CanvasSceneControl: 初始化关卡UI显示");

            gameHUD.InitializeLevelData(); // 每次进入关卡都初始化数据
            gameFinishScreen.gameObject.SetActive(false);
            levelStartPanel.gameObject.SetActive(true); // 显示关卡开始界面
        }
        private void GetLevelUIComponent()
        {
            Debug.Log("CanvasSceneControl: 获取关卡UI组件");
            if (pausePanelController == null) // 只有第一次进入关卡需要获取
            {
                pausePanelController = gameCanvas.GetComponentInChildren<PausePanelController>(true);
                pausePanelController.gameObject.SetActive(true); // 先激活使事件订阅生效，在面板里再隐藏
            }

            if (gameHUD == null)
            {
                gameHUD = gameCanvas.GetComponentInChildren<GameHUD>(true);
                gameHUD.gameObject.SetActive(true); // 先激活使UI服务注册，在面板里再隐藏                
            }            
            
            if (gameFinishScreen == null)
            {
                gameFinishScreen = gameCanvas.GetComponentInChildren<GameFinishScreen>(true);
                gameFinishScreen.gameObject.SetActive(true); // 先激活使UI服务注册，在面板里再隐藏

                gameFinishScreen.OnLevelRetryEvent.AddListener(RetryLevel); // 监听直至游戏结束，因此无需手动移除监听
            }            

            if (levelStartPanel == null)
            {
                levelStartPanel = gameCanvas.GetComponentInChildren<LevelStartPanel>(true);
            }
        }
    }
}
