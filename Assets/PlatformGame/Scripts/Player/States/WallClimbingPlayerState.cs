using System.Collections;
using UnityEngine;
namespace PLAYERTWO.PlatformerProject
{
    public class WallClimbingPlayerState : PlayerState
    {       
        protected Vector3 m_wallNormal;      // 墙面法线
        protected Vector3 m_wallRight;       // 墙面的右方向
        protected Vector3 m_wallUp;          // 墙面的上方向（实际就是 Vector3.up）        
        protected float m_speed;              // 攀爬速度（从 Player 读取）
        private bool m_isClimbingTop;         // 是否正在翻越顶端


        protected override void OnEnter(Player player)
        {
            // Debug.Log("WallClimbingPlayerState: OnEnter");
            // 获取墙面信息
            m_wallNormal = player.currentWallNormal;
            m_wallRight = Vector3.Cross(m_wallNormal, Vector3.up).normalized;
            m_wallUp = Vector3.up;
            m_speed = player.wallClimbSpeed;
            
            // 将玩家吸附到墙面上
            var snapOffset = - m_wallNormal * (player.radius / 2f + 0.1f );
            var initMoveUp = m_wallUp * (player.height / 4f);
            player.controller.Move(initMoveUp + snapOffset);
            
            // 面向墙面
            player.FaceDirection(-m_wallNormal);

            // Debug.Log("WallClimb m_wallNormal: " + m_wallNormal);
        }

        protected override void OnExit(Player player)
        {
            player.OnWallClimbingExit();
            // Debug.Log("WallClimbingPlayerState: OnExit");
        }

        protected override void OnStep(Player player)
        {
            if (m_isClimbingTop)
            {
                // 正在翻越顶端则等待协程完成
                return;
            }

            // 检查墙面是否还存在/有效
            if (!IsWallStillValid(player))
            {
                // Debug.Log("WallClimbingPlayerState: Wall lost, exiting to Fall");
                ExitToFall(player);
                return;
            }

            // 检测到player输入主动离开墙面
            if (player.Inputs.GetReleaseClimb())
            {
                ExitToFall(player);
                return;
            }            
            

            var inputDir = player.Inputs.GetMovementCameraDirection();
            // 输入方向转换为墙面方向
            var moveDirection = (m_wallRight * inputDir.x + m_wallUp * inputDir.z).normalized;
            
            // 应用移动
            var moveDelta = moveDirection * m_speed * Time.deltaTime;
            player.controller.Move(moveDelta); // 墙面移动不像walk一样设置加速减速过程，所以直接设置controller移动

            // 可选：保持玩家面向墙面
            player.FaceDirection(-m_wallNormal);

            // 顶端检测：如果玩家头顶没有障碍物且前方有平台，则自动翻越
            CheckAndClimbTop(player);

            // 底端检测：如果玩家脚下有地面，则落地退出
            CheckAndLandBottom(player);
            
        }
        public override void OnContact(Player entity, Collider other)
        {
            
        }

        /// <summary>
        /// 退出到下落状态
        /// </summary>
        protected virtual void ExitToFall(Player player)
        {
            // Debug.Log("WallClimbingPlayerState: Exiting to Fall");
            // 清除墙面记录
            player.currentClimbableWall = null;
            player.currentWallNormal = Vector3.zero;
            
            // 清除水平速度，保留重力下落
            player.lateralVelocity = Vector3.zero;
            
            // 切换到下落状态
            player.States.Change<FallPlayerState>();
        }
        /// <summary>
        /// 底端落地检测
        /// </summary>
        protected virtual void CheckAndLandBottom(Player player)
        {
            // 向下检测地面
            // if (player.isGrounded)
            if(Physics.Raycast(player.transform.position, Vector3.down, 0.1f, ~0, QueryTriggerInteraction.Ignore))
            {
                // Debug.Log("WallClimbingPlayerState: Ground detected, exiting to Idle");
                player.lateralVelocity = Vector3.zero;
                player.verticalVelocity = Vector3.zero;
                player.States.Change<IdlePlayerState>();
            }
        }
        /// <summary>
        /// 顶端检测与自动翻越
        /// </summary>
        protected virtual void CheckAndClimbTop(Player player)
        {
            // 检测头顶是否有障碍物（如果有，说明上面还有墙，不能翻）
            var headCheckOrigin = player.transform.position + Vector3.up * (player.height - 0.1f);
            if (Physics.CheckSphere(headCheckOrigin, player.radius * 0.5f, ~0, QueryTriggerInteraction.Ignore))
                return;
            
            // 检测前方上方是否有可站立的平台
            var forwardTopOrigin = player.transform.position + Vector3.up * player.height + player.transform.forward * player.radius * 1.5f;
            if (Physics.Raycast(forwardTopOrigin, Vector3.down, out var hit, player.height + 0.5f, ~0, QueryTriggerInteraction.Ignore))
            {
                // 找到平台，将玩家移动到平台上
                var targetY = hit.point.y;// + player.height * 0.5f;
                var targetPos = new Vector3(player.transform.position.x, targetY, player.transform.position.z) - m_wallNormal*player.radius * 2f;
                
                // 确保位置足够
                // if (player.FitsIntoPosition(targetPos))
                // {
                    player.playerEvents.OnClimbTop?.Invoke(); // 暂无
                    player.StartCoroutine(ClimbOverTopRoutine(player, targetPos));  // 协程进行实际位移         
                // }
            }
        }

        /// <summary>
        /// 翻越顶端的协程
        /// </summary>
        protected virtual IEnumerator ClimbOverTopRoutine(Player player, Vector3 targetPos)
        {
            m_isClimbingTop = true;

            var duration = 0.9f;           // 动画时长
            var elapsed = 0f;

            var totalDelta = targetPos - player.transform.position; // 总位移
            // Debug.Log($"totalDelta: {totalDelta}");

            player.ClimbTop = true;            
            // 动画期间平滑移动
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;
                
                // 使用曲线让移动更自然（可选）
                // t = Mathf.SmoothStep(0, 1, t);

                var currentDelta = totalDelta * t;
                var previousDelta = totalDelta * (elapsed - Time.deltaTime) / duration;
                var moveDelta = currentDelta - previousDelta;
                // Debug.Log($"currentDelta: {currentDelta}");
                
                player.controller.Move(moveDelta);

                yield return null;
            }
            player.ClimbTop = false;
            
            // 确保最终位置精确
            var finalDelta = targetPos - player.transform.position;
            if (finalDelta.magnitude > 0.001f)
            {
                // Debug.Log($"finalDelta: {finalDelta}");
                player.controller.Move(finalDelta);
            }

            m_isClimbingTop = false;
            
            // 重置速度
            player.verticalVelocity = Vector3.zero;
            player.lateralVelocity = Vector3.zero;
            
            // 切换到 Idle 状态
            player.States.Change<IdlePlayerState>();
        }

        /// <summary>
        /// 检查墙面是否仍然有效
        /// </summary>
        protected virtual bool IsWallStillValid(Player player)
        {
            if (player.currentClimbableWall == null) return false;
            
            // 向前检测是否仍然接触墙面
            if (!player.CapsuleCast(player.transform.forward, player.wallClimbDetectionDistance + player.radius))
                return false;
            
            // 检查是否是同一个墙面（可选）
            // if (hit.collider != player.currentClimbableWall) return false;
            
            // 检查法线是否大致一致
            // if (Vector3.Angle(hit.normal, m_wallNormal) > 30f) return false;
            
            return true;
        }
    }
}
