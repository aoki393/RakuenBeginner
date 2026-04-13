using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class WalkPlayerState : PlayerState
    {
        protected override void OnEnter(Player player){ }

        protected override void OnExit(Player entity){ }

        protected override void OnStep(Player player)
        {
            player.Gravity();
            // player.SnapToGround();
            player.Jump(); // 施加起跳后是切换到Fall状态
            player.Fall(); // 非跳跃切换到Fall，比如走到悬崖掉下去

            var inputDirection = player.Inputs.GetMovementCameraDirection();

            if(inputDirection.sqrMagnitude > 0)
            {               
                player.Accelerate(inputDirection);
                player.FaceDirectionSmooth(player.lateralVelocity);

                // TODO: 增加刹车处理而非直接停止，需要一个BrakePlayerState

                // 超过刹车值正常加速并转向，低于刹车值进入刹车状态
                // var dot = Vector3.Dot(inputDirection, player.lateralVelocity);
                // if(dot >= player.Stats.current.brakeThreshold)
                // {
                //     player.Accelerate(inputDirection);
                //     player.FaceDirectionSmooth(player.lateralVelocity);
                // }
                // else
                // {
                //     Debug.Log("进入刹车状态");
                //     // player.States.Change<BrakePlayerState>();
                // }
            }
            else
            {
                // 没有输入 → 使用摩擦力减速
                player.Friction();

                // 当水平速度为零 → 切换到闲置状态
                if (player.lateralVelocity.sqrMagnitude <= 0)
                {
                    player.States.Change<IdlePlayerState>();
                }
            }
        }
        public override void OnContact(Player player, Collider other)
        {
            // throw new System.NotImplementedException();
        }
    }
}