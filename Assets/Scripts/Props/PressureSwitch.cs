using UnityEngine;

public class PressureSwitch : MonoBehaviour
{
    [Header("连接设置")]
    [Tooltip("拖入要移动的装置物体")]
    public Transform targetDevice;

    [Header("移动设置")]
    [Tooltip("装置激活时要移动的偏移量")]
    public Vector3 moveOffset = new Vector3(0, 3, 0);

    [Tooltip("装置移动的速度")]
    public float deviceMoveSpeed = 2.0f;

    [Header("延迟设置")]
    [Tooltip("踩上去后，延迟多久装置才开始响应")]
    public float activationDelay = 0.2f;

    [Tooltip("离开后，延迟多久装置才开始复位")]
    public float deactivationDelay = 1.0f;

    [Header("触发设置")]
    public string triggerTag1 = "Player";
    public string triggerTag2 = "PhantomPlayer";

    [Header("开关自身视觉")]
    public float buttonPressDepth = 0.1f;
    public float buttonAnimSpeed = 10f; // 建议加快按钮自身的反应速度，更有打击感

    // --- 内部变量 ---
    // 开关自身位置
    private Vector3 _btnOriginalPos;
    private Vector3 _btnPressedPos;
    private Vector3 _btnCurrentTargetPos;

    // 装置位置
    private Vector3 _deviceStartPos;
    private Vector3 _deviceEndPos;
    private Vector3 _deviceCurrentTarget;

    // 逻辑状态
    private int _colliderCount = 0; // 记录有多少个有效物体踩在开关上
    private float _stateTimer = 0f; // 状态切换计时器
    private bool _isLogicallyActive = false; // 装置当前的逻辑状态 (开/关)

    void Start()
    {
        // 1. 初始化开关位置
        _btnOriginalPos = transform.position;
        _btnPressedPos = _btnOriginalPos - new Vector3(0, buttonPressDepth, 0);
        _btnCurrentTargetPos = _btnOriginalPos;

        // 2. 初始化装置位置
        if (targetDevice != null)
        {
            _deviceStartPos = targetDevice.position;
            _deviceEndPos = _deviceStartPos + moveOffset;
            _deviceCurrentTarget = _deviceStartPos;
        }
        else
        {
            Debug.LogError($"【错误】开关 {name} 未绑定 TargetDevice！");
        }
    }

    void Update()
    {
        UpdateLogicState();   // 处理延迟和状态判断
        UpdateVisuals();      // 处理物体移动
    }

    // --- 核心逻辑：判断是否应该激活 ---
    void UpdateLogicState()
    {
        // 物理上是否有人踩着？ (计数器 > 0)
        bool isPhysicallyPressed = _colliderCount > 0;

        // 按钮自身的视觉：应该立刻反应物理状态 (踩下就立刻沉下去，给玩家反馈)
        _btnCurrentTargetPos = isPhysicallyPressed ? _btnPressedPos : _btnOriginalPos;

        // 装置的逻辑：需要经过延迟判断
        if (isPhysicallyPressed != _isLogicallyActive)
        {
            // 状态不一致，开始计时
            _stateTimer += Time.deltaTime;

            // 根据是“想开启”还是“想关闭”决定用哪个延迟时间
            float requiredDelay = isPhysicallyPressed ? activationDelay : deactivationDelay;

            if (_stateTimer >= requiredDelay)
            {
                // 计时结束，正式切换状态
                _isLogicallyActive = isPhysicallyPressed;
                _stateTimer = 0; // 重置计时器

                // 更新装置的目标点
                if (targetDevice != null)
                {
                    _deviceCurrentTarget = _isLogicallyActive ? _deviceEndPos : _deviceStartPos;
                }

                // 打印日志方便调试
                if (_isLogicallyActive) Debug.Log("<color=green>开关生效：装置启动</color>");
                else Debug.Log("<color=orange>开关复位：装置回退</color>");
            }
        }
        else
        {
            // 状态一致 (比如已经开着，且人还踩着)，重置计时器
            _stateTimer = 0;
        }
    }

    // --- 视觉更新：处理平滑移动 ---
    void UpdateVisuals()
    {
        // 1. 开关自身的按钮动画
        if (Vector3.Distance(transform.position, _btnCurrentTargetPos) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, _btnCurrentTargetPos, Time.deltaTime * buttonAnimSpeed);
        }

        // 2. 装置的移动
        if (targetDevice != null && Vector3.Distance(targetDevice.position, _deviceCurrentTarget) > 0.001f)
        {
            targetDevice.position = Vector3.MoveTowards(
                targetDevice.position,
                _deviceCurrentTarget,
                deviceMoveSpeed * Time.deltaTime
            );
        }
    }

    // --- 触发器逻辑：使用计数器 ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag1) || other.CompareTag(triggerTag2))
        {
            _colliderCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag1) || other.CompareTag(triggerTag2))
        {
            _colliderCount--;
            // 防止意外情况导致的计数器变为负数
            if (_colliderCount < 0) _colliderCount = 0;
        }
    }

    // --- 调试辅助 ---
    private void OnDrawGizmosSelected()
    {
        if (targetDevice != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetDevice.position);

            Gizmos.color = new Color(1, 0.92f, 0.016f, 0.5f);
            Vector3 endPos = targetDevice.position + moveOffset;

            Collider devCol = targetDevice.GetComponent<Collider>();
            if (devCol != null)
            {
                Gizmos.DrawWireCube(endPos, devCol.bounds.size);
                Gizmos.DrawLine(targetDevice.position, endPos);
            }
            else
            {
                Gizmos.DrawWireSphere(endPos, 0.5f);
            }
        }
    }
}