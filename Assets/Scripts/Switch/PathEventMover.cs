using UnityEngine;

public class PathEventMover : MonoBehaviour
{
    [Header("路径设置")]
    public Transform[] waypoints;

    [Header("速度控制")]
    [Tooltip("当前起始速度")]
    public float currentSpeed = 5.0f;
    
    [Tooltip("每经过一个路点，自动增加多少速度")]
    public float accelerationPerPoint = 2.0f; 

    [Tooltip("最低速度限制")]
    public float minSpeed = 2.0f;
    
    [Tooltip("最高速度限制")]
    public float maxSpeed = 50.0f;

    [Header("方向")]
    public bool isClockwise = true;

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

        // --- 到达路点检测 ---
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            // 1. 计算下一个点
            CalculateNextPoint();
            
            // 2. 逻辑修改：到达路点后自动加速
            currentSpeed += accelerationPerPoint;
            
            // 限制最大速度
            if (currentSpeed > maxSpeed) currentSpeed = maxSpeed;
            
            Debug.Log($"到达路点！自动加速至: {currentSpeed}");
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

    // --- 外部接口：供开关调用 ---

    // 逻辑修改：减速方法
    public void ReduceSpeed(float amount)
    {
        currentSpeed -= amount;
        
        // 限制最低速度
        if (currentSpeed < minSpeed) currentSpeed = minSpeed;
        
        Debug.Log($"玩家触发开关！速度降低至: {currentSpeed}");
    }
    
    // 切换方向 (可选保留)
    public void ToggleDirection()
    {
        isClockwise = !isClockwise;
        CalculateNextPoint();
    }
}