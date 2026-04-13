using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    [RequireComponent(typeof(PlayerInputManager))]   // 玩家输入管理器（处理按键、手柄等输入）
	[RequireComponent(typeof(PlayerStatsManager))]   // 玩家数值管理器（存储移动速度、跳跃力等数值配置）
	[RequireComponent(typeof(PlayerStateManager))]   // 玩家状态机管理器（Idle、Run、Jump 等状态）
    // [RequireComponent(typeof(Health))]
    public class Player : Entity<Player>
    {
        // States在Entity里已定义
        public PlayerInputManager Inputs { get; protected set; }
        public PlayerStatsManager Stats { get; protected set; }

		public PlayerEvents playerEvents;
		public bool canJumpInAir=true; // 调试用，允许在空中跳跃
		public bool onWater=false; // 是否在水中
		public Collider water;
		public float WaterSurface; // 水面高度，用于计算浮力
		[Range(0f,1f)]
		public float waterExposure=0.2f; // 浮起平衡状态时上半身的露出度

		// private Health health;
		public Transform LastCheckpoint;
		public bool inEnemyIsland=false;

		[Header("Wall Climbing")]
		public bool enableWallClimbing = true;                    // 总开关
		public float wallClimbDetectionDistance = 0.4f;          // 检测距离
		public float wallClimbAngleTolerance = 25f;              // 角度容差（度）
		public float wallClimbSpeed = 4f;                        // 攀爬速度
		protected float m_lastWallClimbExitTime = -1f;
		public float wallClimbExitCooldown = 0.6f;		// 退出后的短暂冷却，防止反复进出
		public Collider currentClimbableWall { get; set; }
		public Vector3 currentWallNormal { get; set; }
		public bool ClimbTop { get; set; } = false;

		

        protected override void Awake()
        {
            base.Awake();
            Inputs = GetComponent<PlayerInputManager>();
            Stats = GetComponent<PlayerStatsManager>();
            InitializeStateManager();

			// health = GetComponent<Health>();

        }
		void Start()
		{
			if(LastCheckpoint == null)
			{
				Debug.LogError("初始Checkpoint 没有配置！");
			}
		}
		protected virtual void OnTriggerStay(Collider other)
		{			
			if (other.CompareTag(GameTags.VolumeWater))
			{
				if(!onWater && other.bounds.Contains(Position))
				{
					EnterWater(other);
				}else if (onWater)
				{
					var exitPoint = Position + Vector3.down * height*0.1f; // 角色脚底再往下一点都不在水中时
					if (!other.bounds.Contains(exitPoint))
					{
						ExitWater();
					}
				}
			}
		}
		private void EnterWater(Collider other)
		{
			if(!onWater)
			{
				onWater = true;
				water = other;
				WaterSurface = other.bounds.max.y;
				States.Change<SwimPlayerState>();
			}
		}
		private void ExitWater()
		{
			onWater = false;
		}

        public virtual void Gravity()
		{
			if (!isGrounded && verticalVelocity.y > -Stats.current.gravityTopSpeed)
			{
				var speed = verticalVelocity.y;
				// 上升时用普通重力，下落时用更强的下落重力
				var force = verticalVelocity.y > 0 ? Stats.current.gravity : Stats.current.fallGravity;
				speed -= force * gravityMultiplier * Time.deltaTime;

				// 限制最大下落速度
				speed = Mathf.Max(speed, -Stats.current.gravityTopSpeed);
				verticalVelocity = new Vector3(0, speed, 0);
			}
		}
        // public virtual void SnapToGround() => SnapToGround(Stats.current.snapForce); // 落地时将角色位置与地面对齐
        // public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, Stats.current.rotationSpeed); // 平滑转向输入方向
        public virtual void Jump()
        {
			if (isGrounded || canJumpInAir)
			{
				if (Inputs.GetJumpDown()) // 按下跳跃键
				{
					Jump(Stats.current.maxJumpHeight);
					playerEvents.OnJump?.Invoke(); // 触发跳跃事件，通知动画更新
				}
			}
			
            // 是否可以进行二段 / 多段跳
			// var canMultiJump = (JumpCounter > 0) && (JumpCounter < Stats.current.multiJumps);
			// 土狼跳判定（离地一小段时间内仍然可以跳）
			// var canCoyoteJump = (JumpCounter == 0) && (Time.time < lastGroundTime + Stats.current.coyoteJumpThreshold);

			// 是否允许在持物状态下跳跃
			// var holdJump = !holding || Stats.current.canJumpWhileHolding;

			// 地面 / 轨道 / 多段跳 / 土狼跳条件满足时才允许跳跃
			// if ((isGrounded || onRails || canMultiJump || canCoyoteJump) && holdJump)
            // if (isGrounded || onRails)
			// {
			// 	if (Inputs.GetJumpDown()) // 按下跳跃键
			// 	{
			// 		Debug.Log("跳跃 GetJumpDown Log");
			// 		Jump(Stats.current.maxJumpHeight);
			// 		playerEvents.OnJump?.Invoke(); // 触发跳跃事件，通知动画更新
			// 	}
			// }

			// 松开跳跃键时，如果还在上升，限制为最小跳跃高度（实现“按得短跳得低”的效果）
			// if (Inputs.GetJumpUp() && (JumpCounter > 0) && (verticalVelocity.y > Stats.current.minJumpHeight))
			// {
			// 	verticalVelocity = Vector3.up * Stats.current.minJumpHeight;
			// }
        }
        public virtual void Jump(float height)
		{
			// JumpCounter++; // 增加跳跃计数
			verticalVelocity = Vector3.up * height; // 设置垂直速度
			States.Change<FallPlayerState>();       // 切换为下落状态（跳起后最终会落下）
			// playerEvents.OnJump?.Invoke();          // 触发跳跃事件
		}

        public virtual void Fall()
		{
			if (!isGrounded)
			{
				States.Change<FallPlayerState>();
			}
		}

        public virtual void Accelerate(Vector3 direction)
		{
			// 根据是否按下 Run 键、是否在地面，决定不同的转向阻尼与加速度
			var turningDrag = isGrounded && Inputs.GetRun() ? Stats.current.runningTurningDrag : Stats.current.turningDrag;
			var acceleration = isGrounded && Inputs.GetRun() ? Stats.current.runningAcceleration : Stats.current.acceleration;
			var finalAcceleration = isGrounded ? acceleration : Stats.current.airAcceleration; // 空中与地面不同
			var topSpeed = Inputs.GetRun() ? Stats.current.runningTopSpeed : Stats.current.topSpeed;
			// var turningDrag = Stats.current.turningDrag;
			// var acceleration = Stats.current.acceleration;
			// var finalAcceleration = isGrounded ? acceleration : Stats.current.airAcceleration; // 空中与地面不同
			// var topSpeed = Stats.current.topSpeed;

			// 调用底层 Accelerate(方向, 转向阻尼, 加速度, 最大速度)
			Accelerate(direction, turningDrag, finalAcceleration, topSpeed);

			playerEvents.OnRun?.Invoke(); // 触发跑步事件，通知动画更新

			// 如果刚松开跑步键，限制最大速度，避免瞬间超速
			// if (Inputs.GetRunUp())
			// {
			// 	lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed);
			// }
		}
		public virtual void AccelerateToInputDirection()
		{
			var inputDirection = Inputs.GetMovementCameraDirection(); // 输入相对于相机的方向
			Accelerate(inputDirection);
		}

        public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, Stats.current.rotationSpeed);
		public bool IsInputRunning() => Inputs.GetRun();

        public virtual void Decelerate() => Decelerate(Stats.current.deceleration);
        public virtual void Friction()
		{
			Decelerate(Stats.current.friction);

			// if (OnSlopingGround())
			// 	Decelerate(Stats.current.slopeFriction); // 在斜坡上使用斜坡摩擦
			// else
			// 	Decelerate(Stats.current.friction);      // 普通摩擦
		}

		public virtual void WaterAcceleration(Vector3 direction)
		{
			Accelerate(direction, Stats.current.waterTurningDrag, Stats.current.swimAcceleration, Stats.current.swimTopSpeed);
		}
		public virtual void WaterFaceDirection(Vector3 direction)
		{
			FaceDirection(direction, Stats.current.waterRotationSpeed);
		}

        public override void ApplyDamage(int amount, Vector3 origin)
        {
            // if(!health.IsEmpty && !health.Recovering)
			// {
			// 	health.Damage(amount);
			
			playerEvents.OnHurt?.Invoke(); // 触发受伤事件，通知声音播放

			// 受击回到上一个检查点位置
			FlashMoveToLastCheckpoint();

			// }
        }

		/// <summary>
		/// 受击回到上一个检查点位置，需要先解除CharacterController的控制并重置姿态
		/// </summary>
		private void FlashMoveToLastCheckpoint()
		{
			controller.enabled = false;

			velocity = Vector3.zero; // 清空速度
			transform.SetPositionAndRotation(LastCheckpoint.position, Quaternion.Euler(0, -90, 0)); // 重置姿态
			inEnemyIsland=false; // 狗日的，瞬移没受到Island边缘的Trigger检测，所以需要手动重置

            States.Change<IdlePlayerState>();

			controller.enabled = true;
		}

		void OnTriggerEnter(Collider other)
		{
			if(other.CompareTag(GameTags.Checkpoint))
			{
				LastCheckpoint = other.transform;
			}
			if(other.CompareTag(GameTags.EnemyIsland))
			{
				inEnemyIsland=true;
			}
			if(other.CompareTag(GameTags.LevelCompleteDoor))
			{
				// 玩家自身胜利动作播放之类的，数据保存和UI显示在LevelCompleteDoor上执行这里不用处理
				playerEvents.OnLevelComplete?.Invoke(); 
				States.Change<IdlePlayerState>();
				Inputs.actions.Disable(); // 禁用输入
				
			}
		}
		void OnTriggerExit(Collider other)
		{
			if(other.CompareTag(GameTags.EnemyIsland))
			{
				inEnemyIsland=false;
			}
		}

		protected override void OnUpdate()
        {
            base.OnUpdate();

			if(States.Current is not WallClimbingPlayerState)
			{
				TryStartWallClimbing();
			}			

        }

		public bool CanStartWallClimbing
		{
			get
			{
				if (m_lastWallClimbExitTime < 0) return true;
				return Time.time - m_lastWallClimbExitTime > wallClimbExitCooldown;
			}
		}

		/// <summary>
		/// 尝试进入墙面攀爬状态（在 Update 或状态机中调用）
		/// </summary>
		public virtual void TryStartWallClimbing()
		{
			if (!enableWallClimbing) return;			
			
			// 条件1：玩家按住了前进键（相对于相机的前方）
			var moveInput = Inputs.GetMovementCameraDirection();
			if (moveInput.z <= 0.1f) return;  // 没有向前推
			
			// 条件2：检测前方是否有可攀爬的墙面
			if (!CapsuleCast(transform.forward, wallClimbDetectionDistance, out var hit))
				return;
			
			// 条件3：墙面必须有 Climbable Tag
			if (!hit.collider.CompareTag("Climbable")) return;
			
			// 条件4：墙面角度检查
			if (Vector3.Angle(hit.normal, Vector3.up) < 90f - wallClimbAngleTolerance) return;
			
			// 条件5：玩家面朝方向与墙面朝向对齐
			var forward = transform.forward;
			var wallDirection = -hit.normal;  // 指向墙内的方向的反方向 = 指向玩家的方向
			if (Vector3.Angle(forward, wallDirection) > wallClimbAngleTolerance) return;

			// 附加：退出攀爬后的冷却时间未到
			if (!CanStartWallClimbing) return; 
			
			// 条件通过，进入攀爬状态
			// 先重置速度
			velocity = Vector3.zero;
			
			// 记录当前攀爬的墙面信息（供状态使用）
			currentClimbableWall = hit.collider;
			currentWallNormal = hit.normal;
			
			// 切换到攀爬状态
			States.Change<WallClimbingPlayerState>();
		}

		public virtual void OnWallClimbingExit()
		{
			m_lastWallClimbExitTime = Time.time;
		}

        // void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.red; // 红色表示攀爬检测
        //     Gizmos.DrawLine(transform.position, transform.position + transform.forward * wallClimbDetectionDistance);
        // }

    }
}
