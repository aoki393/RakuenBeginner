using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    [RequireComponent(typeof(EnemyStatsManager))]
    [RequireComponent(typeof(WaypointManager))]
    // [RequireComponent(typeof(Health))]
    public class Enemy : Entity<Enemy>
    {
        public EnemyEvents enemyEvents;
		protected Collider[] m_sightOverlaps = new Collider[1024];
		protected Collider[] m_contactAttackOverlaps = new Collider[1024];
        public EnemyStatsManager Stats { get; protected set; }

		public WaypointManager Waypoints { get; protected set; }

		// public Health health { get; protected set; }

		public Player Player { get; protected set; }
		public bool loseSight; // 失去视野标志，用于ChaseState丢失目标后缓冲时间内保持静止，也用来设置动画状态
		public float loseSightBufferTime = 3f; // 失去视野缓冲时间（TODO：改为从Stats里配置

		// 初始化组件引用
		protected virtual void InitializeStatsManager() => Stats = GetComponent<EnemyStatsManager>();
		protected virtual void InitializeWaypointsManager() => Waypoints = GetComponent<WaypointManager>();
		// protected virtual void InitializeHealth() => health = GetComponent<Health>();
		protected virtual void InitializeTag() => tag = GameTags.Enemy;
        public bool IsIdle => Waypoints.IsIdle; // 判断是否处于空闲状态

        protected override void Awake()
		{
			base.Awake();
			InitializeTag();              // 设置标签
			InitializeStatsManager();     // 初始化属性管理器
			InitializeWaypointsManager(); // 初始化路径点管理器
			// InitializeHealth();           // 初始化血量组件
		}
		
		protected override void OnUpdate()
		{
			HandleSight();   // 检测玩家
			ContactAttack(); // 检测接触攻击
		}

		/// <summary>
		/// 敌人受到伤害，根据血量状态触发相应事件
		/// </summary>
		/// <param name="amount">伤害值</param>
		/// <param name="origin">伤害来源位置</param>
		// public override void ApplyDamage(int amount, Vector3 origin)
		// {
		// 	if (!health.isEmpty && !health.recovering) // 确保敌人还有血且不在恢复中
		// 	{
		// 		health.Damage(amount); // 扣血
		// 		enemyEvents.OnDamage?.Invoke(); // 触发受伤事件

		// 		if (health.isEmpty) // 血量耗尽
		// 		{
		// 			controller.enabled = false; // 禁用控制器（停止移动）
		// 			enemyEvents.OnDie?.Invoke(); // 触发死亡事件
		// 		}
		// 	}
		// }
		
		public virtual void Accelerate(Vector3 direction, float acceleration, float topSpeed) =>
			Accelerate(direction, Stats.current.turningDrag, acceleration, topSpeed);

		public virtual void Decelerate() => Decelerate(Stats.current.deceleration);

		public virtual void Friction() => Decelerate(Stats.current.friction);

		public virtual void Gravity() => Gravity(Stats.current.gravity);

		// public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);

		public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, Stats.current.rotationSpeed);

        /// <summary>
        /// 接触攻击逻辑（如果敌人支持近身碰撞攻击）
        /// </summary>
        public virtual void ContactAttack()
        {
        	if (Stats.current.canAttackOnContact)
        	{
        		// 检测指定范围内的实体
        		var overlaps = OverlapEntity(m_contactAttackOverlaps, Stats.current.contactOffset);

        		for (int i = 0; i < overlaps; i++)
        		{
        			// 如果是玩家
        			if (m_contactAttackOverlaps[i].CompareTag(GameTags.Player) &&
        				m_contactAttackOverlaps[i].TryGetComponent<Player>(out var player))
        			{
        				// 计算脚下位置（防止玩家从上方踩到）
        				// controller.bounds.max：这是敌人的碰撞盒（Collider）的最高点坐标，通常是敌人头顶的位置。
        				// Vector3.down * stats.current.contactSteppingTolerance：沿着 Y 轴向下偏移一个很小的距离（contactSteppingTolerance），用来做容差范围。
        				// 合起来，stepping 代表敌人“头顶往下一点点”的位置，作为判断玩家是否站在敌人头上的参考点。
        				var stepping = controller.bounds.max + Vector3.down * Stats.current.contactSteppingTolerance;

        				//避免玩家从敌人上方踩踏时，被敌人错误判定为接触攻击
        				if (!player.IsPointUnderStep(stepping))
        				{
        					// 如果开启击退效果
        					if (Stats.current.contactPushback)
        					{
        						lateralVelocity = -transform.forward * Stats.current.contactPushBackForce;
        					}

        					// 对玩家造成伤害
        					player.ApplyDamage(Stats.current.contactDamage, transform.position);
        					enemyEvents.OnPlayerContact?.Invoke(); // 触发接触事件

                            // 暂停追击，回到巡逻状态
							// 现在的逻辑是player被攻击后瞬移回上一个检查点，此时逻辑会进入 HandleSight() 执行追逐时的缓冲，这里就不用再处理了
        				}
        			}
        		}
        	}
        }

        /// <summary>
        /// 敌人视野检测与玩家发现逻辑
        /// </summary>
        protected virtual void HandleSight()
        {
        	if (!Player) // 没有锁定玩家
        	{
        		// 检测视野范围内的碰撞体
        		var overlaps = Physics.OverlapSphereNonAlloc(Position, Stats.current.spotRange, m_sightOverlaps);

        		for (int i = 0; i < overlaps; i++)
        		{
        			// 如果是玩家
        			if (m_sightOverlaps[i].CompareTag(GameTags.Player))
        			{
        				if (m_sightOverlaps[i].TryGetComponent<Player>(out var player) && player.inEnemyIsland) // 确保玩家在敌人的岛屿上
        				{
        					Player = player; // 记录玩家引用
        					enemyEvents.OnPlayerSpotted?.Invoke(); // 触发发现玩家事件

                            loseSight = false; // 设置发现玩家标志
                            States.Change<ChaseEnemyState>();

        					return;
        				}
        			}
        		}
        	}
        	else // 已经锁定了玩家
        	{
        		// var distance = Vector3.Distance(Position, Player.Position);                

        		// 玩家离开敌人岛屿则解除锁定
        		if (!Player.inEnemyIsland)
        		{
                    Debug.Log("检测到玩家已离开敌人岛屿");
        				
        			Player = null; // 解除锁定
        			enemyEvents.OnPlayerScaped?.Invoke(); // 玩家逃脱时表现（暂无）

					loseSight = true; // 设置失去视野标志
					velocity = Vector3.zero; // 停止移动
					StartCoroutine(ChangeToPatrolState());     // 实现一个丢失玩家后的缓冲时间，不立刻转到巡逻状态
        		}
        	}
        }
		IEnumerator ChangeToPatrolState()
		{
			yield return new WaitForSeconds(loseSightBufferTime);
			// 如果玩家在这个缓冲时间又捕捉到，就没必要切换了
			if (Player == null)
			{
				States.Change<PatrolEnemyState>();
			}
		}

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (Stats != null)
            {
                Gizmos.DrawWireSphere(transform.position, Stats.current.spotRange);  
            }
        }
    }
}
