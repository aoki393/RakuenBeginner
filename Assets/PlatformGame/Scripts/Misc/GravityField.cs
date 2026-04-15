using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    [RequireComponent(typeof(Collider))]
    public class GravityField : MonoBehaviour
    {
        [SerializeField] private float force = 75f;

        private Collider m_collider;

        /// <summary>
        /// 初始化 Collider，将其设置为触发器（Trigger），
        /// 这样物体不会发生物理碰撞，而是仅检测进入/停留/退出的触发事件。
        /// </summary>
        void Start()
        {
            m_collider = GetComponent<Collider>();
            m_collider.isTrigger = true;
        }

        /// <summary>
        /// 当其他物体进入并停留在该触发器区域时会调用。
        /// 如果检测到是玩家，则对其施加一个向上的力。
        /// </summary>
        /// <param name="other">进入触发器的物体的 Collider</param>
        void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(GameTags.Player))
            {
                if (other.TryGetComponent<Player>(out var player))
                {
                    // 如果玩家处于地面状态，则清空竖直速度
                    // 这样避免角色因地面检测而被拉住，确保能被场的力抬起
                    if (player.isGrounded)
                    {
                        player.verticalVelocity = Vector3.zero;
                    }

                    // 给玩家的速度施加一个“沿着当前物体的 up 方向”的力
                    // Time.deltaTime 确保力是逐帧平滑的
                    player.velocity += transform.up * force * Time.deltaTime;
                }
            }
        }
    }
}