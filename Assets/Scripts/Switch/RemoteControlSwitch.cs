using UnityEngine;

public class DeceleratorSwitch : MonoBehaviour
{
    [Header("连接")]
    [Tooltip("拖入物体")]
    public PathEventMover targetMover;

    [Header("游戏逻辑")]
    [Tooltip("每次减少多少速度")]
    public float speedReduction = 5.0f;

    [Tooltip("是否切换方向")]
    public bool toggleDirectionOnPress = false;

    [Header("视觉反馈")]
    public float pressDistance = 0.05f;
    public float pressSpeed = 5f;
    public float resetDelay = 0.5f; // 按钮回弹的冷却时间

    // 内部变量
    private Vector3 _originalPos;
    private Vector3 _targetPos;
    private bool _isLocked = false;

    void Start()
    {
        _originalPos = transform.position;
        _targetPos = _originalPos;
    }

    void Update()
    {
        // 视觉平滑移动
        if (Vector3.Distance(transform.position, _targetPos) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * pressSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isLocked) return;

        if (other.CompareTag("Player"))
        {
            TriggerAction();
        }
    }

    void TriggerAction()
    {
        _isLocked = true; // 锁定

        // 1. 视觉：下沉
        _targetPos = _originalPos - new Vector3(0, pressDistance, 0);

        // 2. 逻辑：减速
        if (targetMover != null)
        {
            // 调用减速方法
            targetMover.ReduceSpeed(speedReduction);

            if (toggleDirectionOnPress)
            {
                targetMover.ToggleDirection();
            }
        }

        // 3. 计时：回弹
        Invoke("ResetButton", resetDelay);
    }

    private void ResetButton()
    {
        _isLocked = false;
        // 视觉：回弹
        _targetPos = _originalPos;
    }
}