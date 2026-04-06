using UnityEngine;
using UnityEngine.UI;

namespace PlatformGame
{
    // [RequireComponent(typeof(UIAnimator))]
    /// <summary>
    /// 暂停面板控制器  
    /// 负责暂停游戏时显示和隐藏暂停面板。
    /// </summary>
    public class PausePanelController : MonoBehaviour
    {
        public GamePause gamePause; // 响应游戏暂停事件，同时也调用游戏暂停方法
        private UIAnimator uiAnimator; // Show和Hide动画交给 UIAnimator 组件处理
        public Button btnresume;
        public Button btnrestart;
        public Button btnmenu;
        public Button btnquit;
        void Start()
        {
            if(gamePause == null)
            {
                gamePause = FindFirstObjectByType<GamePause>(); // 挂载在__GameController__上
                Debug.Log("已自动引用GamePause组件");
            }
            uiAnimator = GetComponent<UIAnimator>(); // 面板上面需要挂载 UIAnimator 组件

            gamePause.OnPauseEvent.AddListener(OnPause);

            btnresume.onClick.AddListener(OnResumeClicked);
            btnrestart.onClick.AddListener(OnRestartClicked);
            btnmenu.onClick.AddListener(OnMenuClicked);
            btnquit.onClick.AddListener(OnQuitClicked);   

            gameObject.SetActive(false); // 初始状态隐藏面板，等待游戏暂停时显示         
        }

        private void OnPause(bool isPaused){
            if(isPaused)
            {
                Show(); // 显示暂停面板
            }
            else
            {
                Hide(); // 隐藏暂停面板
            }
        }

        void OnResumeClicked()
        {
            gamePause.TogglePause();
        }
        void OnRestartClicked()
        {
            // TODO：重新加载当前场景
            // SceneManagement.SceneManager.LoadScene(SceneManagement.SceneManager.GetActiveScene().name); 
            Debug.Log("TODO: 重新开始当前关卡");
        }
        void OnMenuClicked()
        {
            // TODO: 返回主菜单
            Debug.Log("TODO: 返回主菜单");
        }
        void OnQuitClicked()
        {
            // TODO: 退出游戏，需要保存数据等
            Debug.Log("TODO: 退出游戏");
        }

        private void Show() => uiAnimator.Show(); 

        private void Hide()=>uiAnimator.Hide(); 
        
        private void OnDestroy()
        {
            gamePause.OnPauseEvent.RemoveListener(OnPause);
            btnresume.onClick.RemoveListener(OnResumeClicked);
            btnrestart.onClick.RemoveListener(OnRestartClicked);    
            btnmenu.onClick.RemoveListener(OnMenuClicked);
            btnquit.onClick.RemoveListener(OnQuitClicked);            
        }
    }
}
