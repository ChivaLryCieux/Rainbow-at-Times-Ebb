using UnityEngine;
using System.Collections;

// 挂在开关物体上，需要 Collider (Is Trigger)
public class TimedGravitySwitch : MonoBehaviour
{
    [Header("连接设置")]
    [Tooltip("拖入控制重力的区域 (GravityZone)")]
    public GravityZone targetZone;

    [Header("参数")]
    [Tooltip("反重力持续时间 (秒)")]
    public float activeTime = 3.0f;
    public string triggerTag = "Player";

    [Header("视觉反馈")]
    public float pressDepth = 0.1f;
    public float animSpeed = 5f;

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
        // 按钮平滑移动
        if (Vector3.Distance(transform.position, _targetPos) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * animSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 冷却中 或 不是玩家 -> 忽略
        if (_isLocked || !other.CompareTag(triggerTag)) return;

        StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        _isLocked = true;

        // 1. 按钮下沉
        _targetPos = _originalPos - new Vector3(0, pressDepth, 0);

        // 2. 告诉区域：有一个开关激活了！(计数 +1)
        if (targetZone != null) targetZone.AddActivation();

        // 3. 等待
        yield return new WaitForSeconds(activeTime);

        // 4. 按钮回弹
        _targetPos = _originalPos;

        // 5. 告诉区域：有一个开关结束了！(计数 -1)
        if (targetZone != null) targetZone.RemoveActivation();

        _isLocked = false;
    }
}