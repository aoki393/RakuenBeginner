using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    [RequireComponent(typeof(Player))]
    public class PlayerAnimator : MonoBehaviour
    {
        public Animator animator;
        private Player player;

        [Header("Parameters Names")] // Animator 参数的变量名
		public readonly string stateName = "State";   
        public string lateralSpeedName = "Lateral Speed";       // 横向速度
		public string verticalSpeedName = "Vertical Speed";     // 纵向速度
        public string isRunningName = "Is Running";             // 是否在跑步
		// public string lateralAnimationSpeedName = "Lateral Animation Speed"; // 横向动画速度
        void Awake()
        {
            player = GetComponentInParent<Player>();
            if(animator == null)
            {
                Debug.LogError("PlayerAnimator: Animator component is not assigned.");
            }
        }

        void LateUpdate()
        {
            // 更新 Animator 参数
            animator.SetInteger(stateName, player.States.index);
            // animator.SetFloat(lateralSpeedName, player.lateralVelocity.magnitude);
            animator.SetFloat(verticalSpeedName, player.verticalVelocity.y);
            animator.SetBool(isRunningName, player.IsInputRunning());

            // 其他参数根据需要添加
        }

    }
}
