using UnityEngine;

public class PlatformSwitch : MonoBehaviour
{
    [Header("连接")]
    [Tooltip("把跳台管理器拖到这里")]
    public PlatformManager targetManager; // 引用管理器而不是门

    [Header("视觉反馈")]
    [Tooltip("开关按下的位移距离")]
    public float pressDistance = 0.05f;
    public float pressSpeed = 5f; // 下沉速度

    private Vector3 _originalPos;
    private Vector3 _targetPos;
    private bool _isLocked = false; // 锁定状态，防止重复触发

    void Start()
    {
        _originalPos = transform.position;
        _targetPos = _originalPos;
    }

    void Update()
    {
        // 使用 Lerp 让按钮平滑移动，比直接修改坐标更自然
        transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * pressSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. 如果已经激活了，就不再响应
        if (_isLocked) return;

        // 2. 检查是不是玩家
        if (other.CompareTag("Player"))
        {
            ActivateSwitch();
        }
    }

    void ActivateSwitch()
    {
        _isLocked = true; // 锁定开关

        // 视觉：按钮下沉
        _targetPos = _originalPos - new Vector3(0, pressDistance, 0);

        // 逻辑：通知管理器开始干活
        if (targetManager != null)
        {
            targetManager.StartSequence(this);
        }
    }

    // 这个方法由管理器在时间结束后调用，用于复位开关
    public void ResetButton()
    {
        _isLocked = false;
        // 视觉：按钮回弹
        _targetPos = _originalPos;
    }
}