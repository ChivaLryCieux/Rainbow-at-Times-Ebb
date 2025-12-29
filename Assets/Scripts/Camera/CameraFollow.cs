using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target; // 玩家
    public float followSpeed = 10f; // 水平跟随速度
    
    [Header("位置偏移")]
    public Vector3 offset = new Vector3(0, 2, -10);

    private float _xVelocity;

    void Start()
    {
        if (target != null)
        {
            // 在游戏开始时，立刻对准玩家并应用偏移
            transform.position = target.position + offset;
        }
    }

    // 所有脚本的 LateUpdate() 方法会在每一帧的最后运行，确保之前的逻辑和动画都已完成
    void LateUpdate()
    {
        if (target == null) return;

        // --- 1. 计算目标X坐标 ---
        float targetX = target.position.x;

        // --- 2. 平滑移动到目标X坐标 ---
        float currentX = Mathf.SmoothDamp(transform.position.x, targetX, ref _xVelocity, 1 / followSpeed);

        // --- 3. 更新摄像机位置 ---
        // 只更新X坐标，Y和Z坐标基于玩家位置+偏移量，保持固定
        transform.position = new Vector3(currentX, target.position.y + offset.y, target.position.z + offset.z);
    }
}