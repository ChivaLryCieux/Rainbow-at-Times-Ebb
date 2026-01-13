using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f; 
    public float gravity = -20f; // 自定义重力
    public float stickToGroundForce = -2f;

    // ================== 物理交互参数 ==================
    [Header("物理交互 (推箱子 & 踩天平)")]
    [Tooltip("推箱子的力度")]
    public float pushPower = 2.0f;
    [Tooltip("踩在天平上模拟的质量 (kg)")]
    public float simulatedMass = 60f; 
    [Tooltip("重力传导系数，觉得压不动天平就调大这个")]
    public float weightGravityMultiplier = 1.0f;
    // ======================================================

    private CharacterController controller;
    private Vector3 velocity;
    private float horizontalInput;
    private float initialZPosition; 

    // 接口1：供 Visuals 读取
    public bool IsGrounded { get; private set; }

    // 接口2：供 Visuals 订阅
    public event Action OnJump;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        initialZPosition = transform.position.z;
    }

    void Update()
    {        
        // 1. 获取输入
        horizontalInput = Input.GetAxis("Horizontal");

        // 2. 状态检测
        IsGrounded = controller.isGrounded;

        // 如果在地面且速度向下，给一个贴地力，防止下坡飞出
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = stickToGroundForce;
        }

        // --- 移动方向计算 ---
        Vector3 move = Vector3.right * horizontalInput; 

        if (move.x != 0)
        {
            transform.rotation = Quaternion.Euler(0, move.x > 0 ? 90 : -90, 0);
        }

        // 应用移动 (水平)

        controller.Move(move * moveSpeed * Time.deltaTime);

        // 3. 跳跃逻辑
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            Jump();
        }

        // 4. 应用重力 (垂直)
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 5. Z轴锁定
        if (transform.position.z != initialZPosition)
        {
            Vector3 pos = transform.position;
            pos.z = initialZPosition;
            transform.position = pos;
        }
    }

    void Jump()
    {
        velocity.y = jumpForce;
        OnJump?.Invoke();
    }

    public float GetCurrentSpeed()
    {
        return Mathf.Abs(horizontalInput * moveSpeed);
    }

    // ================== 核心物理交互逻辑 ==================
    // 当 CharacterController 碰到任何带 Collider 的物体时自动调用
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // 1. 安全检查：如果没有刚体，或者刚体设为了 Kinematic，则不处理
        if (body == null || body.isKinematic)
        {
            return;
        }

        // 2. 判断碰撞方向
        // hit.moveDirection.y < -0.3 意味着玩家正在“向下”运动（踩）
        if (hit.moveDirection.y < -0.3f)
        {
            // === 功能 A: 踩天平 (垂直施力) ===
            Vector3 forceDirection = Vector3.down;
            // 计算力：质量 * 重力 * 自定义系数
            // 使用 Physics.gravity.y 读取Unity全局重力，通常为 -9.81
            float forceMagnitude = simulatedMass * -Physics.gravity.y * weightGravityMultiplier;
            
            // 在踩踏的具体点位施加力（这会产生力矩，让天平倾斜）
            body.AddForceAtPosition(forceDirection * forceMagnitude * Time.deltaTime, hit.point);
        }
        else
        {
            // === 功能 B: 推箱子 (水平推力) ===
            // 只有当我们在水平方向上有移动时才推
            if (hit.moveDirection.y > -0.3f) 
            {
                // 限制推力只在 X 轴方向 (适应你的 2.5D 移动)
                Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, 0);

                // 如果推的方向太小，忽略（防止轻微抖动）
                if (pushDir.magnitude < 0.1f) return;

                // 施加推力
                // 使用 Velocity 方式推箱子通常比 AddForce 手感更线性、更可控
                body.velocity = pushDir * pushPower; 
            }
        }
    }
}