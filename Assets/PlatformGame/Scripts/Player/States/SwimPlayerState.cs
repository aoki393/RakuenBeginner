using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class SwimPlayerState : PlayerState
    {
        protected override void OnEnter(Player player){
            player.velocity *= player.Stats.current.waterConversion; // 不在这里调速度只靠Step里面的浮力就会下降很多才上浮
        }

        protected override void OnExit(Player entity){}

        protected override void OnStep(Player player)
        {
            if(!player.onWater)
            {
                player.States.Change<IdlePlayerState>();
            }
            else
            {
                var inputDirection = player.Inputs.GetMovementCameraDirection();

                // 水中加速
                player.WaterAcceleration(inputDirection);
                player.WaterFaceDirection(player.lateralVelocity);

                // 浮力
                if(player.Position.y + player.height*0.5*(1-player.waterExposure) < player.WaterSurface) // waterExposure决定浮起平衡状态时上半身的露出度
                {
                    player.verticalVelocity += Vector3.up * player.Stats.current.waterUpwardsForce * Time.deltaTime;
                }
                else
                {
                    player.verticalVelocity = Vector3.zero; // 超出水面垂直速度归零

                    if(player.Inputs.GetJumpDown())
                    {
                        player.Jump(player.Stats.current.waterJumpHeight);
                        player.States.Change<FallPlayerState>(); // 水面跳跃后切换到Fall状态
                    }
                }

                if(inputDirection.magnitude == 0)
                {
                    player.Decelerate(player.Stats.current.swimDeceleration);
                }
            }

        }
        public override void OnContact(Player player, Collider other)
        {
            // throw new System.NotImplementedException();
        }
    }
}
