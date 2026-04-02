using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    [RequireComponent(typeof(PlayerInputManager))]   // 玩家输入管理器（处理按键、手柄等输入）
	[RequireComponent(typeof(PlayerStatsManager))]   // 玩家数值管理器（存储移动速度、跳跃力等数值配置）
	[RequireComponent(typeof(PlayerStateManager))]   // 玩家状态机管理器（Idle、Run、Jump 等状态）
    public class Player : Entity<Player>
    {
        public int JumpCounter{ get; protected set; }

        public PlayerInputManager Inputs { get; protected set; }
        public PlayerStatsManager Stats { get; protected set; }
        // States在Entity里已定义

        protected override void Awake()
        {
            base.Awake();
            Inputs = GetComponent<PlayerInputManager>();
            Stats = GetComponent<PlayerStatsManager>();
            InitializeStateManager();

            // 监听落地事件，重置跳跃/空中技能次数
			// entityEvents.OnGroundEnter.AddListener(() =>
			// {
				// ResetJumps();
				// ResetAirSpins();
				// ResetAirDash();
			// });

			// 监听进入轨道事件，重置空中技能并进入滑轨状态
			// entityEvents.OnRailsEnter.AddListener(() =>
			// {
				// ResetJumps();
				// ResetAirSpins();
				// ResetAirDash();
				// StartGrind();
			// });
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
            // 是否可以进行二段 / 多段跳
			// var canMultiJump = (JumpCounter > 0) && (JumpCounter < Stats.current.multiJumps);
			// 土狼跳判定（离地一小段时间内仍然可以跳）
			// var canCoyoteJump = (JumpCounter == 0) && (Time.time < lastGroundTime + Stats.current.coyoteJumpThreshold);

			// 是否允许在持物状态下跳跃
			// var holdJump = !holding || Stats.current.canJumpWhileHolding;

			// 地面 / 轨道 / 多段跳 / 土狼跳条件满足时才允许跳跃
			// if ((isGrounded || onRails || canMultiJump || canCoyoteJump) && holdJump)
            if (isGrounded || onRails)
			{
				if (Inputs.GetJumpDown()) // 按下跳跃键
				{
					Jump(Stats.current.maxJumpHeight);
				}
			}

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

			// 如果刚松开跑步键，限制最大速度，避免瞬间超速
			// if (Inputs.GetRunUp())
			// {
			// 	lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed);
			// }
		}
        public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, Stats.current.rotationSpeed);
		public bool IsInputRunning() => Inputs.GetRun();

        public virtual void Decelerate() => Decelerate(Stats.current.deceleration);
        public virtual void Friction()
		{
			if (OnSlopingGround())
				Decelerate(Stats.current.slopeFriction); // 在斜坡上使用斜坡摩擦
			else
				Decelerate(Stats.current.friction);      // 普通摩擦
		}

        
    }
}
