using UnityEngine;
using UnityEngine.Splines;

namespace PLAYERTWO.PlatformerProject
{
	public abstract class EntityBase : MonoBehaviour
	{
		public EntityEvents entityEvents;

		protected Collider[] m_contactBuffer = new Collider[10];    // 碰撞检测缓冲区，用于存储接触的碰撞体
		protected Collider[] m_penetrationBuffer = new Collider[32]; // 碰撞渗透检测缓冲区，用于存储穿透的碰撞体

		protected readonly float m_groundOffset = 0.1f;              // 地面检测偏移
		protected readonly float m_penetrationOffset = -0.1f;        // 渗透检测偏移
		protected readonly float m_slopingGroundAngle = 20f;         // 斜坡角度阈值，判断是否处于斜坡

		public CharacterController controller { get; protected set; } // 角色控制器组件

		// 注意这个速度的属性设计，真正起作用的只是velocity，两个分量只是一层属性封装
		public Vector3 velocity { get; set; }
		public Vector3 lateralVelocity
		{
			get { return new Vector3(velocity.x, 0, velocity.z); }
			set { velocity = new Vector3(value.x, velocity.y, value.z); }
		}
		public Vector3 verticalVelocity
		{
			get { return new Vector3(0, velocity.y, 0); }
			set { velocity = new Vector3(velocity.x, value.y, velocity.z); }
		}

		public Vector3 lastPosition { get; set; }                     // 上一帧的位置

		// 实体当前位置（角色控制器中心加位置）
		public Vector3 Position => transform.position + center; // 注意这里的Position是角色控制器中心位置，不是Transform的位置，Transform的位置是胶囊体底部位置

		// 忽略碰撞器缩放的实体位置（用于某些计算）
		public Vector3 unsizedPosition => Position - transform.up * height * 0.5f + transform.up * originalHeight * 0.5f;

		// 脚步位置（底部位置，考虑了stepOffset）
		public Vector3 stepPosition => Position - transform.up * (height * 0.5f - controller.stepOffset);

		public float positionDelta { get; protected set; }            // 当前位置和上一帧位置的距离

		public float lastGroundTime { get; protected set; }           // 上一次处于地面时间

		public bool isGrounded { get; protected set; } = true;        // 是否在地面上

		public bool onRails { get; set; }                              // 是否处于轨道（Spline轨迹）上

		public float accelerationMultiplier { get; set; } = 1f;       // 加速度倍率

		public float gravityMultiplier { get; set; } = 1f;            // 重力倍率

		public float topSpeedMultiplier { get; set; } = 1f;           // 最高速度倍率

		public float turningDragMultiplier { get; set; } = 1f;        // 转向阻力倍率

		public float decelerationMultiplier { get; set; } = 1f;       // 减速度倍率

		public RaycastHit groundHit;                                   // 当前地面检测的碰撞信息

		public SplineContainer rails { get; protected set; }           // 当前轨道（Spline轨迹）

		public float groundAngle { get; protected set; }               // 当前地面角度

		public Vector3 groundNormal { get; protected set; }            // 当前地面法线

		public Vector3 localSlopeDirection { get; protected set; }     // 当前地面的局部斜坡方向

		public float originalHeight { get; protected set; }            // 初始碰撞器高度

		public float height => controller.height;                      // 碰撞器当前高度

		public float radius => controller.radius;                      // 碰撞器半径

		public Vector3 center => controller.center;                    // 碰撞器中心点

		protected CapsuleCollider m_collider;                          // 自定义胶囊碰撞器（用于自定义碰撞）

		protected BoxCollider m_penetratorCollider;                    // 用于检测碰撞渗透的盒状碰撞器

		protected Rigidbody m_rigidbody;                                // 刚体组件，用于物理模拟
		

		// 判断一个点是否在实体脚步位置下方（用于踩踏检测）
		public virtual bool IsPointUnderStep(Vector3 point) => stepPosition.y > point.y;

		// 判断实体是否在斜坡上
		// public virtual bool OnSlopingGround()
		// {
		//     // 如果实体接触地面并且地面角度大于斜坡的角度阈值
		//     if (isGrounded && groundAngle > m_slopingGroundAngle)
		//     {
		//         // 从当前实体位置沿着下方发射射线进行检测
		//         if (Physics.Raycast(transform.position, -transform.up, out var hit, height * 2f,
		//             Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
		//         {
		//             // 如果射线命中的法线与世界上方向（Vector3.up）之间的夹角大于斜坡角度阈值，则认为是在斜坡上
		//             return Vector3.Angle(hit.normal, Vector3.up) > m_slopingGroundAngle;
		//         }
		//         else
		//             // 如果射线检测没有命中地面，认为是在斜坡上
		//             return true;
		//     }
		//     return false;
		// }

		// 调整角色控制器碰撞器高度
		public virtual void ResizeCollider(float height)
		{
		    // 计算新的高度和当前高度的差值
		    var delta = height - this.height;
		    // 修改角色控制器的高度
		    controller.height = height;
		    // 调整角色控制器的中心位置，使其根据高度变化自动平移
		    controller.center += Vector3.up * delta * 0.5f;
		}

		// 胶囊体检测（无返回检测信息，只返回是否检测到碰撞）
		public virtual bool CapsuleCast(Vector3 direction, float distance, int layer = Physics.DefaultRaycastLayers,
		    QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
		{
		    // 调用带有返回检测信息的方法，并忽略返回的hit信息
		    return CapsuleCast(direction, distance, out _, layer, queryTriggerInteraction);
		}

		// 胶囊体检测（返回检测信息，检测是否与其他物体发生碰撞）
		public virtual bool CapsuleCast(Vector3 direction, float distance,
		    out RaycastHit hit, int layer = Physics.DefaultRaycastLayers,
		    QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
		{
		    // 计算胶囊体的起始位置
		    var origin = Position - direction * radius + center;
		    // 计算偏移量，调整胶囊体的上下半部分，使得碰撞器的中心处于正确位置
		    var offset = transform.up * (height * 0.5f - radius);
		    // 计算胶囊体的顶部和底部位置
		    var top = origin + offset;
		    var bottom = origin - offset;

		    // 使用物理引擎进行胶囊体碰撞检测
		    return Physics.CapsuleCast(top, bottom, radius, direction,
		        out hit, distance + radius, layer, queryTriggerInteraction);
		}

		// 球形检测（无返回检测信息，只返回是否检测到碰撞）
		public virtual bool SphereCast(Vector3 direction, float distance, int layer = Physics.DefaultRaycastLayers,
		    QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
		{
		    // 调用带有返回检测信息的方法，并忽略返回的hit信息
		    return SphereCast(direction, distance, out _, layer, queryTriggerInteraction);
		}

		// 球形检测（返回检测信息，检测是否与其他物体发生碰撞）
		public virtual bool SphereCast(Vector3 direction, float distance,
		    out RaycastHit hit, int layer = Physics.DefaultRaycastLayers,
		    QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
		{
		    // 计算球形检测的有效距离，确保球形的检测范围符合预期
		    var castDistance = Mathf.Abs(distance - radius);

		    // 使用物理引擎进行球形碰撞检测
		    return Physics.SphereCast(Position, radius, direction,
		        out hit, castDistance, layer, queryTriggerInteraction);
		}


		// 检测与其他实体的重叠，结果存储到传入的数组
		public virtual int OverlapEntity(Collider[] result, float skinOffset = 0)
		{
			// 计算接触偏移量（包括碰撞器的皮肤宽度和默认的接触偏移量）
			var contactOffset = skinOffset + controller.skinWidth + Physics.defaultContactOffset;
			// 计算重叠半径（包括胶囊碰撞器的半径和接触偏移量）
			var overlapsRadius = radius + contactOffset;
			// 计算碰撞器顶部和底部的偏移位置
			var offset = (height + contactOffset) * 0.5f - overlapsRadius;
			// 计算胶囊碰撞器的顶部位置（是球心位置）
			var top = Position + Vector3.up * offset;
			// 计算胶囊碰撞器的底部位置（是球心位置）
			var bottom = Position + Vector3.down * offset;
			// 使用Physics.OverlapCapsuleNonAlloc来检测与其他实体的重叠，返回重叠的实体数量
			return Physics.OverlapCapsuleNonAlloc(top, bottom, overlapsRadius, result);
		}


		// 受到伤害（空实现，子类重写）
		public virtual void ApplyDamage(int damage, Vector3 origin) { }
	}
}