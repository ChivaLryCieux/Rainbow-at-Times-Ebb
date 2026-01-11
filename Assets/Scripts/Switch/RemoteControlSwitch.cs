using UnityEngine;

public class RemoteControlSwitch : MonoBehaviour
{
    [Header("连接")]
    [Tooltip("正在移动的物体")]
    public PathEventMover targetMover;

    [Header("控制参数")]
    [Tooltip("每次踩下增加多少速度")]
    public float speedIncrement = 2.0f;

    [Tooltip("是否切换方向")]
    public bool toggleDirectionOnPress = true;

    [Header("视觉反馈")]
    public float pressDistance = 0.05f; // 下沉深度
    public float pressSpeed = 5f;       // 动画平滑度
    public float resetDelay = 0.5f;     // 按钮保持按下的时间(秒)，之后自动弹起

    // 内部变量
    private Vector3 _originalPos;
    private Vector3 _targetPos;
    private bool _isLocked = false;     // 冷却锁

    void Start()
    {
        _originalPos = transform.position;
        _targetPos = _originalPos;
    }

    void Update()
    {
        // 这里的 Lerp 逻辑和你提供的一模一样，负责平滑移动
        if (Vector3.Distance(transform.position, _targetPos) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * pressSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. 如果还在冷却/按下状态，忽略
        if (_isLocked) return;

        // 2. 只有玩家能触发
        if (other.CompareTag("Player"))
        {
            TriggerAction();
        }
    }

    void TriggerAction()
    {
        _isLocked = true; // 锁定，防止短时间内重复触发

        // --- 视觉：按钮下沉 ---
        _targetPos = _originalPos - new Vector3(0, pressDistance, 0);

        // --- 逻辑：控制远处的物体 ---
        if (targetMover != null)
        {
            // 加速
            targetMover.AddSpeed(speedIncrement);

            // 变向 (如果在 Inspector 勾选了的话)
            if (toggleDirectionOnPress)
            {
                targetMover.ToggleDirection();
            }
        }

        // --- 定时复位：几秒后按钮自动弹起，允许再次踩踏 ---
        Invoke("ResetButton", resetDelay);
    }

    // 自动回弹函数
    private void ResetButton()
    {
        _isLocked = false;
        // 视觉：按钮回弹
        _targetPos = _originalPos;
    }
}