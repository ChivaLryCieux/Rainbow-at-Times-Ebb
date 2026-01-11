using UnityEngine;

public class PathEventMover : MonoBehaviour
{
    [Header("路径设置")]
    [Tooltip("按顺序拖入路径点")]
    public Transform[] waypoints;

    [Header("运动参数")]
    public float currentSpeed = 5.0f;  // 当前速度
    public float maxSpeed = 30.0f;     // 速度上限（防止飞出地图）
    public bool isClockwise = true;    // 初始方向

    // 内部状态
    private int _targetIndex = 0;

    void Start()
    {
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
            _targetIndex = 1;
        }
    }

    void FixedUpdate()
    {
        if (waypoints.Length < 2) return;

        Transform targetPoint = waypoints[_targetIndex];

        // 匀速移动
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, currentSpeed * Time.fixedDeltaTime);

        // 到达检测
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            CalculateNextPoint();
        }
    }

    void CalculateNextPoint()
    {
        if (isClockwise)
            _targetIndex = (_targetIndex + 1) % waypoints.Length;
        else
        {
            _targetIndex--;
            if (_targetIndex < 0) _targetIndex = waypoints.Length - 1;
        }
    }

    // --- 外部接口 ---

    // 1. 切换方向（如果需要的话，不需要可以注释掉）
    public void ToggleDirection()
    {
        isClockwise = !isClockwise;
        CalculateNextPoint(); // 立即重新计算目标，实现瞬间掉头
    }

    // 2. 加速（核心需求）
    public void AddSpeed(float amount)
    {
        currentSpeed += amount;
        // 限制最大速度
        if (currentSpeed > maxSpeed) currentSpeed = maxSpeed;

        Debug.Log($"物体加速！当前速度: {currentSpeed}");
    }
}