using UnityEngine;

public class MoveSwitch : MonoBehaviour
{
    [Header("连接设置")]
    [Tooltip("拖入要移动的装置物体")]
    public Transform targetDevice;

    [Header("移动设置")]
    [Tooltip("装置激活时要移动的偏移量")]
    public Vector3 moveOffset = new Vector3(0, 3, 0);

    [Tooltip("装置移动的速度")]
    public float deviceMoveSpeed = 2.0f;

    [Tooltip("激活持续时间")]
    public float activeDuration = 3.0f;

    [Header("触发设置")]
    public string triggerTag1 = "Player";
    public string triggerTag2 = "PhantomPlayer";

    [Header("开关自身视觉")]
    public float buttonPressDepth = 0.1f;
    public float buttonAnimSpeed = 5f;

    // --- 内部变量 ---
    // 开关自身状态
    private Vector3 _btnOriginalPos;
    private Vector3 _btnTargetPos;

    // 装置状态
    private Vector3 _deviceStartPos;
    private Vector3 _deviceEndPos;
    private Vector3 _deviceCurrentTarget; // 装置当前想去哪

    private bool _isLocked = false;

    void Start()
    {
        // 1. 初始化开关位置
        _btnOriginalPos = transform.position;
        _btnTargetPos = _btnOriginalPos;

        // 2. 初始化装置位置
        if (targetDevice != null)
        {
            _deviceStartPos = targetDevice.position;
            _deviceEndPos = _deviceStartPos + moveOffset; // 计算终点
            _deviceCurrentTarget = _deviceStartPos;       // 默认待在起点
        }
        else
        {
            Debug.LogError($"【错误】开关 {name} 未绑定 TargetDevice！");
        }
    }

    void Update()
    {
        // --- 1. 处理开关自身的按钮动画 (Lerp 平滑) ---
        if (Vector3.Distance(transform.position, _btnTargetPos) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, _btnTargetPos, Time.deltaTime * buttonAnimSpeed);
        }

        // --- 2. 处理装置的移动 (MoveTowards 匀速机械运动) ---
        if (targetDevice != null)
        {
            // 如果还没到达目标点，就移动
            if (Vector3.Distance(targetDevice.position, _deviceCurrentTarget) > 0.001f)
            {
                targetDevice.position = Vector3.MoveTowards(
                    targetDevice.position,
                    _deviceCurrentTarget,
                    deviceMoveSpeed * Time.deltaTime
                );
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isLocked) return;

        if (other.CompareTag(triggerTag1) || other.CompareTag(triggerTag2))
        {
            ActivateDevice();
        }
    }

    void ActivateDevice()
    {
        _isLocked = true;

        // 1. 按钮下沉
        _btnTargetPos = _btnOriginalPos - new Vector3(0, buttonPressDepth, 0);

        // 2. 装置移向终点 (伸出)
        if (targetDevice != null)
        {
            _deviceCurrentTarget = _deviceEndPos;
        }

        // 3. 开始倒计时复位
        Invoke("ResetSwitch", activeDuration);
    }

    void ResetSwitch()
    {
        // 1. 按钮回弹
        _btnTargetPos = _btnOriginalPos;

        // 2. 装置移回起点 (缩回)
        if (targetDevice != null)
        {
            _deviceCurrentTarget = _deviceStartPos;
        }

        _isLocked = false;
    }

    // --- 调试辅助：画出装置会移动到哪里 ---
    private void OnDrawGizmosSelected()
    {
        if (targetDevice != null)
        {
            // 绿色线：连接开关和装置
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetDevice.position);

            // 黄色虚框：显示装置激活后的目标位置
            Gizmos.color = new Color(1, 0.92f, 0.016f, 0.5f); // 半透明黄
            Vector3 endPos = targetDevice.position + moveOffset;

            // 尝试获取装置的碰撞体大小来画框，如果没有就画个默认的
            Collider devCol = targetDevice.GetComponent<Collider>();
            if (devCol != null)
            {
                Gizmos.DrawWireCube(endPos, devCol.bounds.size);
                Gizmos.DrawLine(targetDevice.position, endPos); // 画出移动轨迹
            }
            else
            {
                Gizmos.DrawWireSphere(endPos, 0.5f);
            }
        }
    }
}