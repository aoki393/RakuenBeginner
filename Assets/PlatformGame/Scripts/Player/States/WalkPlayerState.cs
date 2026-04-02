using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class WalkPlayerState : PlayerState
    {
        protected override void OnEnter(Player player){ }

        protected override void OnExit(Player entity){ }

        protected override void OnStep(Player player)
        {
            // 重力、贴地、跳跃、下落、旋转、拾取/投掷、冲刺、坡面处理
            // ToDO: 这些功能的具体实现需要在 Player 类中定义对应的方法，这里只是调用示例
            player.Gravity();
            // player.SnapToGround();
            player.Jump();
            player.Fall();
            // player.Spin();
            // player.PickAndThrow();
            // player.Dash();
            // player.RegularSlopeFactor();

            var inputDirection = player.Inputs.GetMovementCameraDirection();

            if(inputDirection.sqrMagnitude > 0)
            {
                var dot = Vector3.Dot(inputDirection, player.lateralVelocity);
                // 超过刹车值正常加速并转向，低于刹车值进入刹车状态
                if(dot >= player.Stats.current.brakeThreshold)
                {
                    player.Accelerate(inputDirection);
                    player.FaceDirectionSmooth(player.lateralVelocity);
                }
                else
                {
                    Debug.Log("进入刹车状态");
                    // TODO: 刹车处理而非直接停止，需要一个BrakePlayerState
                    // player.States.Change<BrakePlayerState>();
                }
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