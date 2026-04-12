using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace PlatformGame
{
    // [RequireComponent(typeof(UIAnimator))]
    /// <summary>
    /// 暂停面板控制器  
    /// 负责暂停游戏时显示和隐藏暂停面板。
    /// </summary>
    public class PausePanelController : MonoBehaviour
    {
        // public GamePauser gamePauser=GamePauser.instance; // 响应游戏暂停事件，同时也调用游戏暂停方法
        private UIAnimator uiAnimator; // Show和Hide动画交给 UIAnimator 组件处理
        public Button btnresume;
        public Button btnrestart;
        public Button btnmenu;
        public Button btnquit;
        void Start()
        {
            uiAnimator = GetComponent<UIAnimator>(); // 面板上面需要挂载 UIAnimator 组件

            GamePauser.instance.OnPauseEvent.AddListener(OnPause);

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
            GamePauser.instance.TogglePause();
        }
        void OnRestartClicked()
        {
            // Hide();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GamePauser.instance.TogglePause(); // 😅
            Debug.Log("TODO: 重新开始当前关卡");
        }
        void OnMenuClicked()
        {
            // TODO: 保存关卡数据
            GameController.instance.LoadScene("MainMenu");
            GamePauser.instance.TogglePause(); // 必须恢复TimeScale，否则LoadScene的协程会卡住😅
        }
        void OnQuitClicked()
        {
            // TODO: 退出游戏，需要保存数据等
            Debug.Log("TODO: 退出游戏");
        }

        private void Show() => uiAnimator.Show(); 

        private void Hide(){
            uiAnimator.Hide(); 
            GameController.SetCursorVisible(false);
        }

        // 暂停面板控制器在游戏程序终止时才销毁，因此无需手动移除监听
        // private void OnDestroy()
        // {
        //     // GamePauser.instance.OnPauseEvent.RemoveListener(OnPause); // GamePauser先销毁会导致这里报错
        //     btnresume.onClick.RemoveListener(OnResumeClicked);
        //     btnrestart.onClick.RemoveListener(OnRestartClicked);    
        //     btnmenu.onClick.RemoveListener(OnMenuClicked);
        //     btnquit.onClick.RemoveListener(OnQuitClicked);            
        // }
    }
}
