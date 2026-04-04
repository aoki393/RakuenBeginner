using UnityEngine;
namespace PLAYERTWO.PlatformerProject
{
    public class FallPlayerState : PlayerState
    {       
        protected override void OnEnter(Player entity){}

        protected override void OnExit(Player entity){}

        protected override void OnStep(Player player)
        {
            player.Gravity();               // 应用重力
            // player.SnapToGround();          // 保持贴地
            player.Jump();                  // 空中允许跳跃
            player.AccelerateToInputDirection(); // 空中根据输入方向加速

            if (player.isGrounded)
            {
                player.States.Change<IdlePlayerState>();
            }
        }
        public override void OnContact(Player player, Collider other)
        {
            // throw new System.NotImplementedException();
        }
    }
}
