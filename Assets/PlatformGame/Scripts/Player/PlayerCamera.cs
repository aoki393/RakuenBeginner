using Unity.Cinemachine;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class PlayerCamera : MonoBehaviour
    {        
        public float initialAngle = 0f;   // 初始俯仰角（相机上下角度）
        [Header("References")]
        public Player player;
        public Transform followTarget;      // 相机跟随的目标（通常是玩家）
        [Header("Orbit Settings")]
        public bool canOrbit = true;  
        public bool canOrbitWithVelocity = true; 
        public float orbitVelocityMultiplier = 5;
        protected float m_cameraTargetYaw;            // 相机目标的水平角度
		protected float m_cameraTargetPitch;          // 相机目标的俯仰角
        [Range(0, 90)]
		public float verticalMaxRotation = 80;        // 相机俯仰角最大值
		[Range(-90, 0)]
		public float verticalMinRotation = -20;       // 相机俯仰角最小值

        [Header("Rotation Settings")]
        public float horizontalSensitivity = 2f;   // 水平旋转灵敏度
        public float verticalSensitivity = 2f;     // 垂直旋转灵敏度
        public float verticalMinAngle = -20f;      // 最小俯角（向下看）
        public float verticalMaxAngle = 80f;       // 最大仰角（向上看）
        
        [Header("Smoothing")]
        public float rotationSmoothing = 0.1f;     // 旋转平滑度（0 = 瞬间，越大越平滑）

        [Range(0.2f, 1)]
        [SerializeField] private float mouseSensitivity = 0.8f;
        [Range(0.2f, 1)]
        [SerializeField] private float gamepadSensitivity = 1f;
        private CinemachineCamera m_camera;

        void Start()
        {
            InitializeComponents();
            InitializeFollow();
        }
        protected virtual void InitializeComponents()
		{
			if (!player)
			{
				player = FindFirstObjectByType<Player>();
			}
            m_camera = GetComponent<CinemachineCamera>();
        }

        protected virtual void InitializeFollow()
        {
            if(followTarget == null)
            {
                Debug.LogWarning("PlayerCamera: Follow target is not assigned. Attempting to find player transform.");
            }
            m_camera.Follow = followTarget;
            m_camera.LookAt = player.transform;

            Reset();
        }

        public virtual void Reset()
		{
			m_cameraTargetPitch = initialAngle; // 设定初始俯仰角
			m_cameraTargetYaw = player.transform.rotation.eulerAngles.y; // 根据玩家朝向设定相机水平角

            // Debug.Log($"PlayerCamera: Reset to initial angle: {initialAngle}, yaw: {m_cameraTargetYaw}");

			MoveTarget();
			// m_brain.ManualUpdate(); // 强制刷新相机
		}

        protected virtual void MoveTarget()
		{
			followTarget.rotation = Quaternion.Euler(m_cameraTargetPitch, m_cameraTargetYaw, 0.0f);
		}

        void LateUpdate()
        {
            HandleOrbit(); 
            MoveTarget(); 
        }
        protected virtual void HandleOrbit()
		{
			if (canOrbit)
			{
				var direction = player.Inputs.GetLookDirection();

				if (direction.sqrMagnitude > 0)
				{
					var usingMouse = player.Inputs.IsLookingWithMouse();
					float deltaTimeMultiplier = usingMouse ? Time.timeScale : Time.deltaTime;

                    float sensitivity = usingMouse ? mouseSensitivity : gamepadSensitivity;

					// 修改相机角度
					m_cameraTargetYaw += direction.x * deltaTimeMultiplier * sensitivity;
					m_cameraTargetPitch -= direction.z * deltaTimeMultiplier * sensitivity;
					m_cameraTargetPitch = ClampAngle(m_cameraTargetPitch, verticalMinRotation, verticalMaxRotation);
				}
			}
		}
        protected virtual float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360) angle += 360;
			if (angle > 360) angle -= 360;

			return Mathf.Clamp(angle, min, max);
		}
    }
}
