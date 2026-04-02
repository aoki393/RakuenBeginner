using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class IdlePlayerState : PlayerState
    {
        public override void OnContact(Player entity, Collider other)
        {
        }

        protected override void OnEnter(Player entity)
        {
        }

        protected override void OnExit(Player entity)
        {
        }

        protected override void OnStep(Player player)
        {
            player.Gravity();               // 应用重力
            // player.SnapToGround();          // 保持贴地
            player.Jump();                  // 允许跳跃
            player.Fall();                  // 检测下落
            // player.Spin();                  // 检测旋转动作
            // player.PickAndThrow();          // 检测拾取/投掷
            // player.RegularSlopeFactor();    // 处理坡面影响
            // player.Friction();              // 应用摩擦力

            // // 获取玩家输入方向
            var inputDirection = player.Inputs.GetMovementDirection();

            // // 如果有移动输入或水平速度 > 0 → 切换到 Walk 状态
            if (inputDirection.sqrMagnitude > 0 || player.lateralVelocity.sqrMagnitude > 0)
            {
                player.States.Change<WalkPlayerState>();
            }
            // // 如果按下下蹲/爬行 → 切换到 Crouch 状态
            // else if (player.inputs.GetCrouchAndCraw())
            // {
            //     player.States.Change<CrouchPlayerState>();
            // }
        }


    }
}
