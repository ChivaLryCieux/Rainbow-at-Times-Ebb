using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target;

    // 改用平滑时间，更直观且不容易出错
    [Tooltip("平滑时间：数值越小越快，0.1~0.3 是不错的范围。绝对不能为0！")]
    public float smoothTime = 0.2f;

    [Header("位置偏移")]
    public Vector3 offset = new Vector3(0, 2, -10);

    // 速度缓存变量
    private float _xVelocity = 0f;

    void Start()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- 保险 1：检查平滑时间 ---
        // 防止 smoothTime 被意外设为 0 或负数，导致除以零错误
        if (smoothTime <= 0.001f) smoothTime = 0.001f;

        // --- 保险 2：检查摄像机自身坐标是否已经坏掉 ---
        // 如果摄像机当前已经是 NaN 了，立刻重置到目标位置，防止错误死循环
        if (float.IsNaN(transform.position.x) || float.IsNaN(transform.position.y) || float.IsNaN(transform.position.z))
        {
            Debug.LogWarning("摄像机坐标出现 NaN，正在尝试自动修复...");
            transform.position = target.position + offset;
            _xVelocity = 0f; // 重置速度
            return; // 这一帧先跳过
        }

        // --- 保险 3：检查速度变量是否坏掉 ---
        // SmoothDamp 的 ref 变量如果变成了 NaN，以后每一帧都会算错
        if (float.IsNaN(_xVelocity))
        {
            _xVelocity = 0f;
        }

        // --- 正常计算逻辑 ---
        float targetX = target.position.x;

        // 使用 smoothTime 而不是 1/speed
        float currentX = Mathf.SmoothDamp(transform.position.x, targetX, ref _xVelocity, smoothTime);

        // 如果算出来的结果还是 NaN (极罕见)，直接硬性赋值，防止报错
        if (float.IsNaN(currentX))
        {
            currentX = targetX;
            _xVelocity = 0f;
        }

        // 应用坐标
        transform.position = new Vector3(currentX, target.position.y + offset.y, target.position.z + offset.z);
    }
}